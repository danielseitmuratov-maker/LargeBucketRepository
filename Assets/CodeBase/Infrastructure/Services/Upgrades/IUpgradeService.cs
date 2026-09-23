using System.Collections.Generic;

namespace _Root._Scripts.Infrastructure.Services.Upgrades
{
    public interface IUpgradeService
    {
        IReadOnlyList<IUpgrade> AllUpgrades { get; }
        IUpgrade GetUpgrade(string id);

        bool TryLevelUp(string id);       // инкапсулирует цену, проверки и Apply
        void LoadFromSave(UpgradeSaveData saveData);
        UpgradeSaveData GetSaveData();
    }
}