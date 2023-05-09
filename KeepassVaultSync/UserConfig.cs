using System.Collections.Generic;
using System.Windows.Forms;

namespace KeepassVaultSync
{
    [System.Serializable]
    public class UserConfig
    {
        public UserConfig()
        {
            InhibitLockDuringSync = true;
        }
        public UserConfig(bool inhibitLockDuringSync)
        {
            InhibitLockDuringSync = inhibitLockDuringSync;
        }

        public bool InhibitLockDuringSync { get; private set; }

        public void SetOptionItems(CheckedListBox owner)
        {
            owner.Items .Clear();
            owner.Items.Add("Inhibit Lock During Sync");
            owner.SelectedIndices.Clear();
            owner.SetItemChecked(0, InhibitLockDuringSync);
        }
        public void SetSelfOptionsItemsFromSelectedIndices(CheckedListBox.CheckedIndexCollection indices)
        {
            InhibitLockDuringSync = indices.Contains(0);
        }

        public override string ToString()
        {
            return $"SyncConfig: inhibit_lock_during_sync={InhibitLockDuringSync}";
        }
    }
}