using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using UnityEngine;

namespace _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel
{
    public class CharacterRoleWheelSfxHandler : MonoBehaviour
    {
        private IGameRoleFortuneWheel _gameRoleFortuneWheel;
        private ISfxPlayer _sfxPlayer;
        private IConfigProvider _configProvider;

        private SoundsConfig _config;

        public void Init(IGameRoleFortuneWheel gameRoleFortuneWheel, ISfxPlayer sfxPlayer,
            IConfigProvider configProvider)
        {
            _gameRoleFortuneWheel = gameRoleFortuneWheel;
            _sfxPlayer = sfxPlayer;
            _configProvider = configProvider;

            SetUpConfig();
            SubscribeToEvents();
        }

        private void SetUpConfig()
        {
            _config = _configProvider.GetConfig<SoundsConfig>(Paths.GlobalValues.SoundsConfigPath);
        }

        private void SubscribeToEvents()
        {
            _gameRoleFortuneWheel.OnSpinStarted += OnFortuneWheelSpinStarted;
            _gameRoleFortuneWheel.OnSpinCompleted += OnFortuneWheelSpinCompleted;
        }
        
        private void OnFortuneWheelSpinStarted(GameRoleFortuneWheelSegment obj)
        {
            _sfxPlayer.PlaySfx(_config.FortuneWheelSpinStartedSond,1f,false);
            _sfxPlayer.PlaySfx(_config.FortuneWheelSpinLoopSound,1f,true);
        }

        private void OnFortuneWheelSpinCompleted(GameRoleFortuneWheelSegment obj)
        {
            _sfxPlayer.PlaySfx(_config.FortuneWheelSpinCompletedSound,1f,false);
        }

        private void UnsubscribeFromEvents()
        {
            _gameRoleFortuneWheel.OnSpinStarted -= OnFortuneWheelSpinStarted;
            _gameRoleFortuneWheel.OnSpinCompleted -= OnFortuneWheelSpinCompleted;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    }
}