using System;
using System.Collections.Generic;
using KeePassLib;
using KeePassLib.Security;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines.TOTP;

namespace KeepassVaultSync
{
    public struct UniversalEntry
    {
        public PwUuid UUID { get; set; }
        public IList<string> Paths { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public ProtectedString Password { get; set; }
        
        public IList<string> Tags { get; set; }
        public IDictionary<string, string> Fields { get; set; }

        public UniversalEntry(PwEntry entry, IList<string> paths)
        {
            UUID = entry.Uuid;
            Paths = paths;
            Name = entry.Strings.ReadSafeEx("Title");
            Tags = entry.Tags;
            UserName = entry.Strings.ReadSafeEx("UserName");
            Password = entry.Strings.GetSafe("Password");
            
            Fields = new Dictionary<string, string>();
            foreach (var kv in entry.Strings)
            {
                if (kv.Key == "Title" || kv.Key == "UserName" || kv.Key == "Password")
                    continue;
                Fields.Add(kv.Key, kv.Value.ReadString());
            }
        }

        public UniversalEntry(SecretData secret, IList<string> paths)
        {
            var uuidBytes = new byte[16];
            var uuidHexString = secret.Data["keepass_uuid"].ToString();
            if (string.IsNullOrEmpty(uuidHexString))
                throw new SyncException("keepass_uuid is missing");
            
            for (var i = 0; i < 16; i++)
            {
                uuidBytes[i] = Convert.ToByte(uuidHexString.Substring(i * 2, 2), 16);
            }
            UUID = new PwUuid(uuidBytes);
            
            Name = paths[paths.Count - 1];
            paths.RemoveAt(paths.Count - 1);
            Paths = paths;
            
            Tags = secret.Data["keepass_tags"].ToString().Split(',');
            UserName = secret.Data["username"].ToString();
            Password = new ProtectedString(true, secret.Data["password"].ToString());
            
            Fields = new Dictionary<string, string>();
            foreach (var kv in secret.Data)
            {
                if (kv.Key == "keepass_uuid" || kv.Key == "keepass_tags" || kv.Key == "username" || kv.Key == "password")
                    continue;
                Fields.Add(kv.Key, kv.Value.ToString());
            }
        }

        public string VaultFullPath()
        {
            var ret = "";
            foreach (var path in Paths)
            {
                var pathEsc = VaultUtil.EscapeVaultPath(path);
                ret += pathEsc + "/";
            }

            ret += VaultUtil.EscapeVaultPath(Name);

            return ret;
        }

        public Dictionary<string, object> ToSecretData()
        {
            var ret = new Dictionary<string, object>();
            ret.Add("keepass_uuid", UUID.ToHexString());
            ret.Add("username", UserName);
            ret.Add("password", Password.ReadString());
            ret.Add("keepass_tags", string.Join(",", Tags));
            foreach (var kv in Fields)
            {
                ret.Add(kv.Key, kv.Value);
            }
            
            return ret;
        }

        public override bool Equals(object obj)
        {
            if (obj is UniversalEntry other)
            {
                return UUID.Equals(other.UUID) && VaultFullPath().Equals(other.VaultFullPath());
            }
            
            return false;
        }

        public override int GetHashCode()
        {
            return UUID.GetHashCode() ^ VaultFullPath().GetHashCode();
        }
    }
}