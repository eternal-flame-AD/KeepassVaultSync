using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using KeePass.Plugins;
using KeePassLib;

namespace KeepassVaultSync
{
    public sealed class KeepassVaultSyncExt : Plugin
    {
        private IPluginHost _host;
        private SyncStatusForm _syncStatusForm;
        
        private CancellationTokenSource _cancellationTokenSource;

        public override bool Initialize(IPluginHost host)
        {
            if (host == null) return false;
            
            _host = host;
            
            _syncStatusForm = new SyncStatusForm();
            _syncStatusForm.StartClicked += OnStartSync;
            _syncStatusForm.StopClicked += OnStopSync;
            _syncStatusForm.RefreshSyncs += (o, e) => UpdateAvailableSyncs();
            
            // prevent file from closing while sync is running
            _host.MainWindow.FileClosingPre += (sender, args) =>
            {
                if (_cancellationTokenSource != null)
                {
                    if (_syncStatusForm.UserConfig.InhibitLockDuringSync)
                    {
                        args.Cancel = true;
                        _syncStatusForm.LogInfo("Inhibiting file close while sync is running");
                    }
                    else
                    {
                        OnStopSync(sender, args);
                    }

                    return;
                }
                
                _syncStatusForm.Hide();
            };
            
            return true;
        }

        public override ToolStripMenuItem GetMenuItem(PluginMenuType t)
        {
            if (PluginMenuType.Main == t)
            {
                var tsmi = new ToolStripMenuItem("KeepassVaultSync");

                var tsmiOpenPanel = new ToolStripMenuItem("Open Sync Panel");
                tsmiOpenPanel.Click += OnShowStatusForm;
                tsmi.DropDownItems.Add(tsmiOpenPanel);

                return tsmi;
            }

            return null;
        }

        delegate Task EntryProcessor(PwEntry entry, IList<string> path);
        
        private async Task PerformSync(SyncConfig syncConfig, CancellationToken cancellationToken = default(CancellationToken))
        {
            var vaultClient = syncConfig.GetVaultClient();
            var filters = new List<PwEntryFilter>
            {
                new PwEntryFilterSyncConfig(),
                new PwEntryFilterByTag(syncConfig), 
                new PwEntryFilterPathRegexp(syncConfig),
            };
            var countUploaded = 0;
            var countDeleted = 0;
            var countFailed = 0;
            
            var vaultPathsToDelete = 
                syncConfig.DeleteOrphans ?
                await VaultUtil.GetVaultKv1Paths(vaultClient, syncConfig.VaultMount, cancellationToken) :
                    new List<string>();
            
            if (syncConfig.DeleteOrphans)
                _syncStatusForm.LogInfo($"Discovered {vaultPathsToDelete.Count} existing entries in vault");

            // Logs a cancellation message and throws if cancellation is requested.
            void CheckCancellation()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _syncStatusForm.LogWarning("Cancelling sync");
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            

            // performs a BFS on the PwGroup and applies the <c>entryProcessor</c> to each entry.
            async Task ForEachEntryAsync(PwGroup rootGroup, EntryProcessor entryProcessor,
                IList<PwEntryFilter> entryFilters)
            {
                var bfs = new Queue<(PwGroup, List<string>)>();
                bfs.Enqueue((rootGroup, new List<string>()));
                var total = 0;
                while (bfs.Count > 0)
                {
                    CheckCancellation();
                    var (group, path) = bfs.Dequeue();
                    foreach (var entry in group.Entries)
                    {
                        if (PwEntryFilter.ApplyFilters(entry, new UniversalEntry(entry, path), entryFilters))
                        {
                            total++;
                        } else if (syncConfig.Verbose)
                        {
                            _syncStatusForm.LogInfo($"skip {string.Join("/", path)}/{entry.Strings.ReadSafe("Title")}");
                        }
                    }

                    foreach (var subgroup in group.Groups)
                    {
                        var newPath = new List<string>(path) { subgroup.Name };
                        bfs.Enqueue((subgroup, newPath));
                    }
                }
                
                _syncStatusForm.StartProgress(total);
                var index = 0;
                bfs.Enqueue((rootGroup, new List<string>()));
                while (bfs.Count > 0)
                {
                    CheckCancellation();
                    var (group, path) = bfs.Dequeue();
                    foreach (var entry in group.Entries)
                    {
                        if (PwEntryFilter.ApplyFilters(entry, new UniversalEntry(entry, path), entryFilters))
                        {
                            _syncStatusForm.IncrementProgress(1);
                            _syncStatusForm.UpdateStatusBar($"[{index}/{total}] {string.Join("/", path)}/{entry.Strings.ReadSafe("Title")}");
                            index += 1;
                            await entryProcessor(entry, path);
                        }
                    }

                    foreach (var subgroup in group.Groups)
                    {
                        var newPath = new List<string>(path) { subgroup.Name };
                        bfs.Enqueue((subgroup, newPath));
                    }
                }
            }

            // EntryProcessor for uploading an entry to Vault.
            async Task UploadEntry(PwEntry entry, IList<string> path)
            {
                CheckCancellation();

                var univEntry = new UniversalEntry(entry, path);
                var entryDisplayName = $"{syncConfig.VaultMount}/{univEntry.VaultFullPath()}";

                if (syncConfig.Verbose) 
                    _syncStatusForm.LogInfo($"sync {entryDisplayName}");
                
                vaultPathsToDelete.Remove(univEntry.VaultFullPath());
                try
                {
                    var retries = 0;
                    while (true)
                    {
                        try
                        {
                            await vaultClient.V1.Secrets.KeyValue.V1.WriteSecretAsync(
                                univEntry.VaultFullPath(),
                                univEntry.ToSecretData(),
                                syncConfig.VaultMount
                            );
                            break;
                        } catch (Exception e)
                        {
                            retries += 1;
                            _syncStatusForm.LogError($"[retry {retries} of 3] Error writing entry {syncConfig.VaultMount}/{univEntry.VaultFullPath()}: {e}");
                            if (retries >= 3)
                            {
                                _syncStatusForm.LogError($"Giving up on entry {syncConfig.VaultMount}/{univEntry.VaultFullPath()}");
                                countFailed += 1;
                                break;
                            }
                        }
                    }

                    countUploaded += 1;
                } catch (Exception e)
                {
                    _syncStatusForm.LogError($"Error writing entry {syncConfig.VaultMount}/{univEntry.VaultFullPath()}: {e}");
                }
            }


            await ForEachEntryAsync(_host.Database.RootGroup, UploadEntry, filters);
            
            CheckCancellation();

            if (vaultPathsToDelete.Count > 0)
            {
                _syncStatusForm.UpdateStatusBar("Deleting orphan entries");
                _syncStatusForm.LogInfo($"Deleting {vaultPathsToDelete.Count} orphan entries");
                _syncStatusForm.StartProgress(vaultPathsToDelete.Count);
                foreach (var path in vaultPathsToDelete)
                {
                    CheckCancellation();
                    if (syncConfig.Verbose)
                        _syncStatusForm.LogInfo($"delete {syncConfig.VaultMount}/{path}");
                    try
                    {
                        await vaultClient.V1.Secrets.KeyValue.V1.DeleteSecretAsync(path, syncConfig.VaultMount);
                        countDeleted += 1;
                    }
                    catch (Exception e)
                    {
                        _syncStatusForm.LogError($"Error deleting entry {syncConfig.VaultMount}/{path}: {e}");
                    }
                    finally
                    {
                        _syncStatusForm.IncrementProgress(1);
                    }
                }
            }

            _syncStatusForm.LogSuccess(syncConfig.DeleteOrphans
                ? $"Sync complete: {countUploaded} uploaded ({countFailed} of which failed), {countDeleted} deleted"
                : $"Sync complete: {countUploaded} uploaded ({countFailed} of which failed)");
        }
        
        private void OnShowStatusForm(object sender, EventArgs e)
        {
            _syncStatusForm.Show();

            UpdateAvailableSyncs();
            _syncStatusForm.LogInfo($"Plugin window opened, read config={_syncStatusForm.UserConfig}");
        }

        private void UpdateAvailableSyncs()
        {
            IList<SyncException> exceptions = new List<SyncException>();
            IList<SyncConfig> syncConfigs = new List<SyncConfig>();

            try
            {
                syncConfigs = SyncConfig.GetSyncConfigs(_host.Database, out exceptions);
                if (syncConfigs == null)
                {
                    _syncStatusForm.LogError("Fatal error obtaining sync configs, abort.");
                    return;
                }
                foreach (var exception in exceptions)
                {
                    _syncStatusForm.LogError($"Error obtaining sync configs: {exception}");
                }
                _syncStatusForm.UpdateAvailableSyncs(syncConfigs);
            } catch (Exception ex)
            {
                _syncStatusForm.LogError($"Fatal error obtaining sync configs: {ex}");
            }
        }

        private async void OnStartSync(object sender, EventArgs e)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var syncConfigs = _syncStatusForm.GetSelectedSyncs();
            if (syncConfigs == null)
            {
                _syncStatusForm.LogError("Fatal error obtaining sync configs, abort.");
                _cancellationTokenSource.Dispose();
                _syncStatusForm.FinishSyncStatus();
                _cancellationTokenSource = null;
                return;
            }
            
            if  (syncConfigs.Count == 0)
            {
                _syncStatusForm.LogError("No sync configs selected, abort.");
            }
            foreach (var syncConfig in syncConfigs)
            {
                _syncStatusForm.LogInfo($"Syncing {syncConfig.VaultAddr}/{syncConfig.VaultMount}");
                _syncStatusForm.UpdateStatusBar($"Syncing {syncConfig.VaultAddr}/{syncConfig.VaultMount}");
                _syncStatusForm.LogInfo(syncConfig.ToString());
                try
                {
                    await PerformSync(syncConfig, _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    _syncStatusForm.LogWarning($"Sync cancelled for {syncConfig.VaultMount}@{syncConfig.VaultAddr}");
                    break;
                }
                catch (Exception exception)
                {
                    _syncStatusForm.LogError($"Fatal error syncing {syncConfig.VaultMount}@{syncConfig.VaultAddr}: {exception}");
                    break;
                }
            }
            
            _cancellationTokenSource.Dispose();
            _syncStatusForm.FinishSyncStatus();
            _cancellationTokenSource = null;
        }
        
        private void OnStopSync(object sender, EventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        public override void Terminate()
        {
            OnStopSync(null, EventArgs.Empty);  
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            _syncStatusForm?.Dispose();
            _syncStatusForm = null;
            
            base.Terminate();
        }
    }
}