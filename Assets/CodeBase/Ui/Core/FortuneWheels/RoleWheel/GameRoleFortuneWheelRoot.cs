using System;
using System.Collections.Generic;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.Character;
using _Root._Scripts.Core.Roles;
using _Root._Scripts.Infrastructure;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Fx;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using UnityEngine;

namespace _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel
{
    public class GameRoleFortuneWheelRoot : MonoBehaviour
    {
        public event Action<GameRoleFortuneWheelSegment> SpinStarted;
        public event Action<GameRoleFortuneWheelSegment> SpinCompleted;

        [SerializeField] private GameRoleFortuneWheelView _view;
        [SerializeField] private CharacterRoleFortuneWheelCameraHandler _cameraHandler;
        [SerializeField] private GameRoleFortuneWheelResultPresenter _resultPresenter; // добавлен
        [SerializeField] private CharacterRoleWheelFxHandler _fxHandler;
        [SerializeField] private CharacterRoleWheelSfxHandler _sfxHandler;
        [SerializeField] private GameRoleFortuneWheelConfig _config;

        private IGameRoleFortuneWheel _fortuneWheel;
        private IConfigProvider _configProvider;
        private ICharacterRoleAdjuster _characterRoleAdjuster;
        private ICoroutineRunnerService _coroutineRunnerService;
        private ISfxPlayer _sfxPlayer;
        private IFxPlayer _fxPlayer;

        private GameLogicConfig _gameLogicConfig;
        private bool _isSpinning;

        public void Init(IConfigProvider configProvider, ICharacterRoleAdjuster characterRoleAdjuster,
            ICoroutineRunnerService coroutineRunnerService, ISfxPlayer sfxPlayer, IFxPlayer fxPlayer,
            IGameRoleFortuneWheel fortuneWheel)
        {
            _configProvider = configProvider;
            _characterRoleAdjuster = characterRoleAdjuster;
            _fortuneWheel = fortuneWheel;
            _coroutineRunnerService = coroutineRunnerService;
            _sfxPlayer = sfxPlayer;
            _fxPlayer = fxPlayer;

            GetConfig();
            SetUpFortuneWheel();
            InitializeComponents();
            SubscribeToEvents();
        }

        private void GetConfig()
        {
            _config = _configProvider.GetConfig<GameRoleFortuneWheelConfig>(
                Paths.FortuneWheel.CharacterGameRoleFortuneWheelConfigPath);
        }

        private void SetUpFortuneWheel()
        {
            _gameLogicConfig = _configProvider.GetConfig<GameLogicConfig>(Paths.GlobalValues.GameLogicConfigPath);
        }

        private void InitializeComponents()
        {
            _view.Init(_fortuneWheel, _configProvider);
            _cameraHandler.Init(_fortuneWheel, _config);
            _resultPresenter.Init(_coroutineRunnerService, _configProvider);
            
            _sfxHandler.Init(_fortuneWheel, _sfxPlayer, _configProvider);
            _fxHandler.Init(_fortuneWheel, _configProvider, _fxPlayer, _coroutineRunnerService);
        }

        private void SubscribeToEvents()
        {
            _fortuneWheel.OnSpinStarted += OnFortuneWheelOnOnSpinStarted;
            _fortuneWheel.OnSpinCompleted += OnFortuneWheelOnOnSpinCompleted;
            _fortuneWheel.OnSegmentsRebuilt += OnSegmentsRebuilt;
        }

        private void UnsubscribeFromEvents()
        {
            _fortuneWheel.OnSpinStarted -= OnFortuneWheelOnOnSpinStarted;
            _fortuneWheel.OnSpinCompleted -= OnFortuneWheelOnOnSpinCompleted;
            _fortuneWheel.OnSegmentsRebuilt -= OnSegmentsRebuilt;
        }

        public void Spin()
        {
            if (_view == null || _view.SegmentsContainer == null)
            {
                Debug.LogError("GameRoleFortuneWheelRoot: View или SegmentsContainer не назначены!");
                return;
            }
            
            if (_isSpinning)
            {
                Debug.LogWarning("Spin already in progress.");
                return;
            }

            _isSpinning = true;
            _fortuneWheel.Spin(_gameLogicConfig.SpinGameRoleDuration, _view.SegmentsContainer);
        }

        private void OnSegmentsRebuilt(IReadOnlyList<GameRoleFortuneWheelSegment> segments)
        {
            List<GameRoleFortuneWheelSegment> list = new List<GameRoleFortuneWheelSegment>(segments);
            _view.RebuildSegments(list);
        }

        private void OnFortuneWheelOnOnSpinStarted(GameRoleFortuneWheelSegment obj)
        {
            SpinStarted?.Invoke(obj);
        }

        private void OnFortuneWheelOnOnSpinCompleted(GameRoleFortuneWheelSegment obj)
        {
            _isSpinning = false;
            _resultPresenter.ShowResult(obj, GetColorForRole(obj.GameRole));
            
            SpinCompleted?.Invoke(obj);
        }

        private Color GetColorForRole(GameRole role)
        {
            int index = (int)role;
            if (index >= 0 && index < _config.roleColors.Length)
                return _config.roleColors[index];
            return Color.white;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        public GameRole GiveOutRole() =>
            _fortuneWheel.GiveOutRole();
    }
}