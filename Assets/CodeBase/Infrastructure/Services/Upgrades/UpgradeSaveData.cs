using System;
using System.Collections.Generic;

namespace _Root._Scripts.Infrastructure.Services.Upgrades
{
    [Serializable]
    public class UpgradeSaveData
    {
        public List<UpgradeEntry> Entries;

        [Serializable]
        public class UpgradeEntry
        {
            public string Id;
            public int Level;
        }
    }
}