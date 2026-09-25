using System;
using System.Collections;
using UnityEngine;
using YG;

namespace _Root._Scripts.Infrastructure.Services.Saves
{
    public class SaveLoadService : ISaveLoadService, IDisposable
    {
        private readonly ICoroutineRunnerService _coroutineRunnerService;
        private Coroutine _autoSaveCoroutine;
        private bool _isDirty;

        private const float AutoSaveInterval = 5f;

        public SavesYG Data => YG2.saves;

        public SaveLoadService(ICoroutineRunnerService coroutineRunnerService = null)
        {
            _coroutineRunnerService = coroutineRunnerService;

            // Если нужно что-то сделать при первом запуске — делай это через Data
            // Например, задать дефолты, если какая-то часть данных пустая

            MarkDirty();
          //  Save();

            if (_coroutineRunnerService != null)
                StartAutoSave();
        }

        public void Save()
        {
            try
            {
             //   Data.lastSaveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                YG2.SaveProgress();

                _isDirty = false;
                
                Debug.Log("[SaveLoadService] Game saved via YG2.SaveProgress()");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveLoadService] Save failed (YG2): {e.Message}\n{e.StackTrace}");
            }
        }

        public void ResetData()
        {
            // Здесь ты либо создаёшь новый SavesYG, либо обнуляешь поля текущего
        //  var d = Data;

        //  d.coins = 0;
        //  d.gems = 0;
        //  d.goldenTickets = 0;
        //  d.currentLevel = 0;
        //  d.OwnedPets.Clear();
        //  d.EquippedPets.Clear();
        //  d.maxEggsSlots = 5;
        //  d.maxSpawnAnimals = 3;
        //  d.moveSpeedBaseMultiplier = 5f;
        //  d.luckyBaseMultiplier = 1f;
        //  d.eggSellBaseMultiplier = 1f;
        //  d.moveSpeedBonusMultiplier = 1f;
        //  d.eggSellBonusMultiplier = 1f;
        //  d.luckyBonusMultiplier = 1f;
        //  d.currentSpawnedAnimalsCount = 0;
        //  d.spawnedAnimals.Clear();
        //  d.rebirthCount = 0;
        //  d.rebirthPrice = 5000f;
        //  d.maxEggSlotsUpgradePrice = 100f;
        //  d.moveSpeedUpgradePrice = 150f;
        //  d.maxSpawnAnimalUpgradePrice = 200;
        //  d.totalEggsCollected = 0;
        //  d.totalWheatCollected = 0;
        //  d.totalCoinEarned = 0;
        //  d.totalGemsEarned = 0;
        //  d.totalGoldenTicketsEarned = 0;
        //  d.totalRebirths = 0;
        //  d.totalPlayTime = 0;
        //  d.totalPetsBought = 0;
        //  d.totalPetsSold = 0;
        //  d.totalAnimalsBought = 0;
        //  d.soundEnabled = true;
        //  d.musicEnabled = true;
        //  d.masterVolume = 1f;
        //  d.musicVolume = 0.7f;
        //  d.cameraSensitivity = 0.5f;
        //  d.tutorialFinished = false;
        //  d.currentTrailMultiplier = 1f;
        //  d.currentTrailColor = new SerializableColor32(0x00, 0x96, 0xFF, 0xFF);
        //  d.boughtTrailsId.Clear();
        //  d.boughtTrailsColor.Clear();
        //  d.buildingParts.Clear();
        //  d.currentTrailId = 0;

            MarkDirty();
            Save();

            Debug.Log("[SaveLoadService] Game data reset to defaults (YG2)");
        }

        public bool HasSave()
        {
            // В YG2 обычно есть флаг loaded/hasSave;
            // временно можно вернуть true, если данные уже загружены SDK.
            return true;
        }

        public void MarkDirty()
        {
            _isDirty = true;
        }

        public void ForceSave()
        {
            _isDirty = true;
            Save();
        }

        private void StartAutoSave()
        {
            if (_coroutineRunnerService == null)
                return;

            _autoSaveCoroutine = _coroutineRunnerService.StartCoroutine(AutoSaveRoutine());
        }

        private IEnumerator AutoSaveRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(AutoSaveInterval);

            while (true)
            {
                yield return wait;

                if (_isDirty)
                {
                    Save();
                    Debug.Log("[SaveLoadService] Auto-save completed (YG2)");
                }
            }
        }

        public void Dispose()
        {
            if (_autoSaveCoroutine != null && _coroutineRunnerService != null)
            {
                _coroutineRunnerService.StopCoroutine(_autoSaveCoroutine);
                _autoSaveCoroutine = null;
            }

            if (_isDirty)
            {
                Save();
                Debug.Log("[SaveLoadService] Final save on dispose (YG2)");
            }
        }
    }
}