using System.Collections.Generic;
using KeePassLib;

namespace KeepassVaultSync
{
    public class PwEntryFilterByTag : PwEntryFilter
    {
        public List<string> Tags { get; }

        public PwEntryFilterByTag(SyncConfig syncConfig)
        {
            var syncEntry = syncConfig.Entry;
            Tags = syncEntry.Tags;
        }
        
        public override bool FilterEntry(PwEntry entry, UniversalEntry universalEntry)
        {
            if (Tags == null || Tags.Count == 0)
                return true;
            
            var hasTag = false;
            
            foreach (var tag in entry.Tags)
            {
                foreach (var tagSpecified in Tags)
                {
                    if (tagSpecified == tag)
                    {
                        hasTag = true;
                        goto finish;
                    }
                }
            }
            finish:
            return hasTag;
        }
    }
}