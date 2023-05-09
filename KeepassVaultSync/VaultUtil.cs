using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.Core;

namespace KeepassVaultSync
{
    public static class VaultUtil
    {
        public static string EscapeVaultPath(string path)
        {
            return
                Uri.EscapeDataString(path);
        }
        
        public static async Task<IList<string>> GetVaultKv1Paths(
            VaultClient vaultClient,
            string mountPoint = null,
            CancellationToken cancellationToken = default(CancellationToken)
            )
        {
            var ret = new List<string>();
            var bfs = new Queue<string>();
            bfs.Enqueue("");
            while (bfs.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var currentPath = bfs.Dequeue();

                try
                {
                    var subFolders =
                        await vaultClient.V1.Secrets.KeyValue.V1.ReadSecretPathsAsync(currentPath, mountPoint);
                    foreach (var subItem in subFolders.Data.Keys)
                    {
                        if (subItem.EndsWith("/"))
                        {
                            var fullPath = currentPath + subItem;
                            bfs.Enqueue(fullPath);
                        }
                        else
                        {
                            ret.Add(currentPath + subItem);
                        }
                    }
                } catch (VaultApiException e)
                {
                    if (e.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // Ignore
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return ret;
        }
    }
}
