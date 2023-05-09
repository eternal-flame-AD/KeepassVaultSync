using System;

namespace KeepassVaultSync
{
    [Serializable]
    public class SyncException : Exception
    {
        public SyncException() : base("Unknown sync error")
        {
        }
        public SyncException(string message) : base(message)
        {
        }
        public SyncException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}