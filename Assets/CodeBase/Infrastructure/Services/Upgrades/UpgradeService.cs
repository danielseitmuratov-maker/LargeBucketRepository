using System.Collections.Generic;

namespace _Root._Scripts.Infrastructure.Services.Upgrades
{
    public class UpgradeService : IUpgradeService
    {
        private readonly Dictionary<string, IUpgrade> _upgrades;

        public IReadOnlyList<IUpgrade> AllUpgrades => _allUpgrades;
        private readonly List<IUpgrade> _allUpgrades;

        public UpgradeService()
        {
            _allUpgrades = new List<IUpgrade>
            {
              //  new CartSpeedUpgrade(cartService),
              //  new IncomeMultiplierUpgrade(economy),
                // ... ещё ~8 апгрейдов
            };

           // _upgrades = _allUpgrades.ToDictionary(u => u.Id);
        }

        public IUpgrade GetUpgrade(string id) =>
            _upgrades.TryGetValue(id, out var u) ? u : null;

        public bool TryLevelUp(string id)
        {
            if (!_upgrades.TryGetValue(id, out var upgrade))
                return false;

            return true;
        }

        public void LoadFromSave(UpgradeSaveData saveData)
        {
            if (saveData?.Entries == null)
                return;

            foreach (var entry in saveData.Entries)
            {
                if (_upgrades.TryGetValue(entry.Id, out var upgrade))
                {
                    upgrade.LoadState(entry.Level);
                }
            }
        }

        public UpgradeSaveData GetSaveData()
        {
            var data = new UpgradeSaveData
            {
                Entries = new List<UpgradeSaveData.UpgradeEntry>()
            };

            foreach (var upgrade in _allUpgrades)
            {
                data.Entries.Add(new UpgradeSaveData.UpgradeEntry
                {
                    Id = upgrade.Id,
                    Level = upgrade.Level
                });
            }

            return data;
        }
    }
}