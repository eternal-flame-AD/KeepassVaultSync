using KeePassLib;

namespace KeepassVaultSync
{
    public class PwEntryFilterSyncConfig : PwEntryFilter
    {
        public PwEntryFilterSyncConfig()
        {
        }
        
        public override bool FilterEntry(PwEntry entry, UniversalEntry universalEntry)
        {
            return !SyncConfig.IsVaultSyncEntry(universalEntry.Paths);
        } 
    }
}