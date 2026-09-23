using System;
using System.Collections;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Fx;
using UnityEngine;

namespace _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel
{
    public class CharacterRoleWheelFxHandler : MonoBehaviour
    {
        private IGameRoleFortuneWheel _gameRoleFortuneWheel;
        private IConfigProvider _configProvider;

        private VfxConfig _config;
        private IFxPlayer _fxPlayer;
        private ICoroutineRunner _coroutineRunner;

        private Coroutine _spinStartedRoutine;
        private Coroutine _spinCompletedRoutine;

        private float _destroyingParticleDelay;

        public void Init(IGameRoleFortuneWheel gameRoleFortuneWheel, IConfigProvider configProvider, IFxPlayer fxPlayer,
            ICoroutineRunner coroutineRunner)
        {
            _gameRoleFortuneWheel = gameRoleFortuneWheel;
            _configProvider = configProvider;
            _fxPlayer = fxPlayer;
            _coroutineRunner = coroutineRunner;

            SetUpValues();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _gameRoleFortuneWheel.OnSpinStarted += OnSpinStarted;
            _gameRoleFortuneWheel.OnSpinCompleted += OnSpinCompleted;
        }
        
        private void UnsubscribeFromEvents()
        {
            _gameRoleFortuneWheel.OnSpinStarted -= OnSpinStarted;
            _gameRoleFortuneWheel.OnSpinCompleted -= OnSpinCompleted;
        }
        

        private IEnumerator PlaySpinStartedRoutine()
        {
            ParticleSystem particle = _config.FortuneWheelSpinStartedParticle;
            _fxPlayer.PlayFx(particle);

            yield return new WaitForSeconds(_destroyingParticleDelay);
            _fxPlayer.StopFx(particle);
        }

        private IEnumerator PlaySpinCompletedRoutine()
        {
            ParticleSystem particle = _config.FortuneWheelSpinCompletedParticle;
            _fxPlayer.PlayFx(particle);

            yield return new WaitForSeconds(_destroyingParticleDelay);
            _fxPlayer.StopFx(particle);
        }
        
        private void SetUpValues()
        {
            _config = _configProvider.GetConfig<VfxConfig>(Paths.GlobalValues.ParticleConfigPath);
            _destroyingParticleDelay = _config.FortuneWheelSpinDestroingDelay;
        }
        
        private void OnSpinStarted(GameRoleFortuneWheelSegment obj) =>
            _spinStartedRoutine = _coroutineRunner.StartCoroutine(PlaySpinStartedRoutine());

        private void OnSpinCompleted(GameRoleFortuneWheelSegment obj) => 
            _spinCompletedRoutine = _coroutineRunner.StartCoroutine(PlaySpinCompletedRoutine());

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    }
}