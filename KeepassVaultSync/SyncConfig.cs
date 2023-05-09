using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.IO;
using KeePassLib;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.UserPass;

namespace KeepassVaultSync
{
    public sealed class SyncConfig
    {
        public string VaultAddr { get; set; }

        public string VaultUser { get; set; }
        public string VaultPass { get; set; }
        public string VaultMount { get; set; }
        public bool Verbose { get; set; }
        public bool DeleteOrphans { get; set; }
        
        public readonly PwEntry Entry;

        public string VaultToken
        {
            get => VaultPass;
            set => VaultPass = value;
        }
        
        public SyncConfig(PwEntry entry)
        {
            VaultMount = entry.Strings.ReadSafe("Title");
            VaultUser = entry.Strings.ReadSafe("UserName");
            VaultPass = entry.Strings.ReadSafe("Password");
            if (string.IsNullOrEmpty(VaultPass))
            {
                VaultToken = VaultTokenFromEnvironment();
            }
            VaultAddr = entry.Strings.ReadSafe("URL");
            if (string.IsNullOrEmpty(VaultAddr))
                VaultAddr = VaultAddrFromEnvironment();

            foreach (var kv in entry.Strings)
            {
                if (kv.Key.ToLower() == "verbose" && kv.Value.ReadString().Length > 0)
                    Verbose = true;
                if (kv.Key.ToLower() == "delete_orphans" && kv.Value.ReadString().Length > 0)
                    DeleteOrphans = true;
            }

            Entry = entry;
        }
        
         const string VaultSyncGroupName = "Vault Sync";
         const string VaultSyncGroupNameLower = "vault sync";

        public static bool IsVaultSyncEntry(IList<string> paths)
        {
            if (paths.Count == 0)
                return false;
            return paths[0].ToLower() == VaultSyncGroupNameLower;
        }

        static string VaultTokenFromEnvironment()
        {
            var tokenFromEnv = Environment.GetEnvironmentVariable("VAULT_TOKEN");
            if (!string.IsNullOrEmpty(tokenFromEnv))
                return tokenFromEnv;
            
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var vaultTokenFile = Path.Combine(homeDir, ".vault-token");
            if (!File.Exists(vaultTokenFile))
                throw new SyncException("Vault token file not found, nor is VAULT_TOKEN environment variable set");
            var tokenFromFile =  File.ReadAllText(vaultTokenFile).Trim();
            return tokenFromFile;
        }

        static string VaultAddrFromEnvironment()
        {
            var addrFromEnv = Environment.GetEnvironmentVariable("VAULT_ADDR");
            if (!string.IsNullOrEmpty(addrFromEnv))
                return  addrFromEnv;
            
            throw new SyncException("Vault address not found, nor is VAULT_ADDR environment variable set");
        }

        public static IList<SyncConfig> GetSyncConfigs(PwGroup group, out IList<SyncException> exceptions)
        {
            var syncConfigs = new List<SyncConfig>();
            
            exceptions = new List<SyncException>();
            foreach (var entry in group.Entries)
            {
                try
                {
                    syncConfigs.Add(new SyncConfig(entry));
                }
                catch (SyncException e)
                {
                    if (exceptions != null)
                        exceptions.Add(e);
                }
            }

            return syncConfigs;
        }
        
        public static IList<SyncConfig> GetSyncConfigs(PwDatabase database, out IList<SyncException> exceptions)
        {
            var root = database.RootGroup;
            foreach (var group in root.Groups)
            {
                if (group.Name.ToLower() == VaultSyncGroupNameLower)
                {
                    return GetSyncConfigs(group, out exceptions);
                } 
            }
            exceptions = new List<SyncException>(new []{new SyncException($"Group matching name \"{VaultSyncGroupName}\" not found")});
            return null;
        }

        public VaultClient GetVaultClient()
        {
            var auth = string.IsNullOrEmpty(VaultUser)
                ? new TokenAuthMethodInfo(VaultToken) as IAuthMethodInfo
                : new UserPassAuthMethodInfo(VaultUser, VaultPass) as IAuthMethodInfo;

            var settings = new VaultClientSettings(VaultAddr, auth);
            settings.VaultServiceTimeout = TimeSpan.FromSeconds(10);
            
            return new VaultClient(settings);
        }

        public override string ToString()
        {
            var authMethod = string.IsNullOrEmpty(VaultUser) ? "token" : "userpass";
            var ret =  $"SyncConfig: {VaultAddr}/{VaultMount} auth={authMethod}";
            if (authMethod == "userpass")
                ret += $" user={VaultUser} pass={new string('*', VaultPass.Length)}";
            else 
                ret += $" token={new string('*', VaultToken.Length)}";
            
            return ret;
        }
        
    }
}