using System;
using System.Collections;
using System.Collections.Generic;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.Character;
using _Root._Scripts.Core.Roles;
using _Root._Scripts.Infrastructure;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel
{
    public class GameRoleFortuneWheel : IGameRoleFortuneWheel
    {
        public event Action<GameRoleFortuneWheelSegment> OnSpinStarted;
        public event Action<GameRoleFortuneWheelSegment> OnSpinCompleted;
        public event Action<IReadOnlyList<GameRoleFortuneWheelSegment>> OnSegmentsRebuilt;

        public IReadOnlyList<GameRoleFortuneWheelSegment> Segments => _segments.AsReadOnly();

        private readonly IConfigProvider _configProvider;
        private readonly ICharacterRoleAdjuster _characterRoleAdjuster;
        private readonly ICoroutineRunnerService _coroutineRunnerService;

        private readonly List<GameRoleFortuneWheelSegment> _segments = new List<GameRoleFortuneWheelSegment>();
        private GameRoleFortuneWheelConfig _config;
        private GameRole _currentGameRole;

        public GameRoleFortuneWheel(IConfigProvider configProvider, ICharacterRoleAdjuster characterRoleAdjuster,
            ICoroutineRunnerService coroutineRunnerService)
        {
            _configProvider = configProvider;
            _characterRoleAdjuster = characterRoleAdjuster;
            _coroutineRunnerService = coroutineRunnerService;

            _config = _configProvider.GetConfig<GameRoleFortuneWheelConfig>(
                Paths.FortuneWheel.CharacterGameRoleFortuneWheelConfigPath);
        }

        public void Spin(float duration, Transform parent)
        {
            // Очистка старых сегментов
            foreach (var segment in _segments)
            {
                if (segment != null && segment.gameObject != null)
                    Object.Destroy(segment.gameObject);
            }
            _segments.Clear();

            if (_config == null || _config.FieldsConfigs == null || _config.FieldsConfigs.Count == 0)
            {
                Debug.LogError("GameRoleFortuneWheel: нет конфигураций сегментов!");
                return;
            }

            // Создание новых сегментов
            foreach (var fieldConfig in _config.FieldsConfigs)
            {
                GameRoleFortuneWheelSegment segment = Object.Instantiate(fieldConfig.Prefab, parent);
                segment.Init(fieldConfig);
                _segments.Add(segment);
            }

            Debug.Log($"Создано {_segments.Count} сегментов.");
            OnSegmentsRebuilt?.Invoke(_segments.AsReadOnly());

            _currentGameRole = _characterRoleAdjuster.Adjust();
            GameRoleFortuneWheelSegment winSegment = ChooseWinSegment();

            if (winSegment == null)
            {
                Debug.LogError("Не удалось выбрать выигрышный сегмент!");
                return;
            }

            Debug.Log($"Выбран выигрышный сегмент: {winSegment.name}");
            OnSpinStarted?.Invoke(winSegment);

            _coroutineRunnerService.StartCoroutine(SpinCoroutine(duration, winSegment));
        }

        private IEnumerator SpinCoroutine(float duration, GameRoleFortuneWheelSegment winSegment)
        {
            yield return new WaitForSeconds(duration);
            OnSpinCompleted?.Invoke(winSegment);
        }

        private GameRoleFortuneWheelSegment ChooseWinSegment()
        {
            if (_segments == null || _segments.Count == 0)
                return null;

            foreach (var segment in _segments)
                if (segment.GameRole == _currentGameRole)
                    return segment;

            return _segments[Random.Range(0, _segments.Count)];
        }

        public GameRole GiveOutRole() => 
            _currentGameRole;
    }
}