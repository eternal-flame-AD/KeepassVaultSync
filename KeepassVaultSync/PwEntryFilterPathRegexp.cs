using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KeePassLib;

namespace KeepassVaultSync
{
    struct FilterRegexp
    {
        public Regex Regexp { get; set; }
        public bool Negate { get; set; }
        
        public bool Match(string input)
        {
            return Regexp.IsMatch(input) != Negate;
        }
    }
    public class PwEntryFilterPathRegexp : PwEntryFilter
    {
        private readonly List<FilterRegexp> _regexps = new List<FilterRegexp>();

        public PwEntryFilterPathRegexp(SyncConfig config)
        {
            var syncEntry = config.Entry;
            foreach (var kv in syncEntry.Strings)
            {
                if (kv.Key.ToLower() == "path_regex_whitelist" || kv.Key.ToLower() == "path_regex_blacklist")
                {
                    var linesRaw = Regex.Split(kv.Value.ReadString(), "\r\n|\r|\n");
                    var lines = linesRaw
                        .Where(e => !string.IsNullOrEmpty(e))
                        .Select(e => e.Trim());
                    foreach (var line in lines)
                    {
                        var negate = kv.Key.ToLower() == "path_regex_blacklist";
                        _regexps.Add(new FilterRegexp
                        {
                            Regexp = new Regex(line),
                            Negate = negate
                        });
                    }
                }
            }
        }

        public override bool FilterEntry(PwEntry entry, UniversalEntry universalEntry)
        {
            var path = universalEntry.VaultFullPath();
            return _regexps.All(e => e.Match(path));
        }
    }
}