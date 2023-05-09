using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using KeePassLib;

namespace KeepassVaultSync
{
    public partial class SyncStatusForm : Form
    {
        public event EventHandler StartClicked;
        public event EventHandler StopClicked;
        public event EventHandler RefreshSyncs;
        
        private bool _started = false;

        public UserConfig _userConfig;
        public UserConfig UserConfig
        {
            get
            {
                if (_userConfig == null)
                {
                    _userConfig = new UserConfig();
                    _userConfig.SetOptionItems(optionsListBox);
                }

                _userConfig.SetSelfOptionsItemsFromSelectedIndices(optionsListBox.CheckedIndices);
                
                return _userConfig;
            }
            set
            {
                _userConfig = value;
                _userConfig.SetOptionItems(optionsListBox);
            }
        }

        public SyncStatusForm()
        {
            InitializeComponent();
            this.Closing += (sender, args) => { args.Cancel = true; Hide(); StopClicked?.Invoke(sender, args); };
            LogInfo("Initialized");
            UpdateStatusBar("Idle");
            startStopButton.Click += (sender, args) =>
            {
                _started = !_started;
                startStopButton.Text = _started ? "Stop" : "Start";
                UpdateStatusBar(_started ? "Running" : "Idle");
                if (_started)
                {
                    StartClicked?.Invoke(sender, args);
                }
                else
                {
                    StopClicked?.Invoke(sender, args);
                }
            };
            refreshSyncsButton.Click += (sender, args) => { RefreshSyncs?.Invoke(sender, args); };
            clearLogButton.Click += (sender, args) => { syncStatusLogTB.Clear(); };
        }
        
        // <> status updates

        public void LogStatusItem(string severity, string log)
        {
            LogStatusItem(severity, syncStatusLogTB.ForeColor, log);
        }
        public void LogStatusItem(string severity, Color severityColor, string log)
        {

            syncStatusLogTB.AppendText(severity, severityColor);
            syncStatusLogTB.AppendText($" [{DateTime.Now}] ");
            syncStatusLogTB.AppendText(log);
            syncStatusLogTB.AppendText(Environment.NewLine);
        }
        
        public void FinishSyncStatus()
        {
            UpdateStatusBar("Idle");
            syncProgressBar.Value = 0;
            startStopButton.Text = "Start";
            _started = false;
        }
        
        public void UpdateStatusBar(string status)
        {
            syncStatusBar.Text = status;
        }
        
        public void StartProgress(int total)
        {
            syncProgressBar.Minimum = 0;
            syncProgressBar.Maximum = total;
            syncProgressBar.Value = 0;
        }
        
        public void UpdateProgressCurrent(int current)
        {
            syncProgressBar.Value = current;
        }
        
        public void IncrementProgress(int delta)
        {
            syncProgressBar.Value += delta;
        }
        
        void ScrollLogToEnd()
        {
            syncStatusLogTB.SelectionStart = syncStatusLogTB.Text.Length;
            syncStatusLogTB.ScrollToCaret();
        }

        public void LogInfo(string log)
        {
            LogStatusItem("INFO",  log);
            ScrollLogToEnd();
        }
        
        public void LogSuccess(string log)
        {
            LogStatusItem("SUCCESS", Color.Green, log);
            ScrollLogToEnd();
        }
        
        public void LogWarning(string log)
        {
            LogStatusItem("WARNING", Color.Orange, log);
            ScrollLogToEnd();
        }

        public void LogError(string log)
        {
            LogStatusItem("ERROR", Color.Red, log);
            ScrollLogToEnd();
        }
        
        // </> status updates
        
        // <> configs
        public void UpdateAvailableSyncs(IList<SyncConfig> syncConfigs)
        {
            var originallySelected = new Dictionary<PwUuid, bool>();
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < syncItemSelector.Items.Count; i++)
            {
                var item = syncItemSelector.Items[i];
                if (item is SyncConfig syncConfig)
                    originallySelected[syncConfig.Entry.Uuid] = syncItemSelector.CheckedItems.Contains(item);
            }
            syncItemSelector.Items.Clear();
            syncItemSelector.Items.AddRange(syncConfigs.ToArray<object>());
            
            for (var i = 0; i < syncItemSelector.Items.Count; i++)
            {
                var syncConfig = (SyncConfig) syncItemSelector.Items[i];
                syncItemSelector.SetItemChecked(i, 
                    !originallySelected.ContainsKey(syncConfig.Entry.Uuid) || originallySelected[syncConfig.Entry.Uuid]);
            }
        }
        public IList<SyncConfig> GetSelectedSyncs()
        {
            return syncItemSelector.CheckedItems.Cast<SyncConfig>().ToList();
        }
        
    }
}