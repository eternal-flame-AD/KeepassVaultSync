using System.Collections.Generic;
using KeePassLib;

namespace KeepassVaultSync
{
    public abstract class PwEntryFilter
    {
        public abstract bool FilterEntry(PwEntry entry, UniversalEntry universalEntry);
        
        public static bool ApplyFilters(PwEntry entry, UniversalEntry universalEntry, IList<PwEntryFilter> filters)
        {
            if (filters == null)
                return true;
            
            foreach (var filter in filters)
            {
                if (!filter.FilterEntry(entry, universalEntry))
                    return false;
            }
            return true;
        }
    }
}
