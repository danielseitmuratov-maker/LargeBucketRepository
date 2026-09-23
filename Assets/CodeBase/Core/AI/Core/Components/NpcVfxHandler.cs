using System;
using System.Collections;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Fx;
using _Root._Scripts.Infrastructure.Services.Interfaces;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core.Components
{
    public class NpcVfxHandler : IDisposable
    {
        private readonly IFxPlayer _fxPlayer;
        private readonly IConfigProvider _configProvider;
        private readonly IHealth _health;
        private readonly Transform _npcTransform;
        private readonly ICoroutineRunner _coroutineRunner;
        private VfxConfig _config;
        private Coroutine _onDiedRoutine;
        private Coroutine _onHealthChangedRoutine;

        public NpcVfxHandler(IFxPlayer fxPlayer, IConfigProvider configProvider, IHealth health,
            Transform npcTransform, ICoroutineRunner coroutineRunner)
        {
            _fxPlayer = fxPlayer;
            _configProvider = configProvider;
            _health = health;
            _npcTransform = npcTransform;
            _coroutineRunner = coroutineRunner;

            GetConfig();
            SubscribeToEvents();
        }

        private void GetConfig()
        {
            _config = _configProvider.GetConfig<VfxConfig>(Paths.GlobalValues.ParticleConfigPath);
        }

        private void SubscribeToEvents()
        {
            _health.HealthChanged += OnHealthChanged;
            _health.Died += OnDied;
        }

        private void OnDied()
        {
            if (_onDiedRoutine != null)
                _coroutineRunner.StopCoroutine(_onDiedRoutine);

            _onDiedRoutine = _coroutineRunner.StartCoroutine(OnDiedCoroutine());
        }

        private void OnHealthChanged(float damage)
        {
            if (damage < 0)
                return;

            if (_onHealthChangedRoutine != null) 
                _coroutineRunner.StopCoroutine(_onHealthChangedRoutine);

            _onHealthChangedRoutine = _coroutineRunner.StartCoroutine(OnHealthChangedCoroutine());
        }

        private IEnumerator OnDiedCoroutine()
        {
            ParticleSystem particle = _config.OnNpcDiedEffect;
            _fxPlayer.PlayFx(particle, _npcTransform);

            yield return new WaitForSeconds(_config.NpcOnDiedDestroyDelay);

            _fxPlayer.StopFx(particle);
        }

        private IEnumerator OnHealthChangedCoroutine()
        {
            ParticleSystem particle = _config.OnNpcHealthChangedEffect;
            _fxPlayer.PlayFx(particle, _npcTransform);

            yield return new WaitForSeconds(_config.NpcOnHealthChangedDestroyDelay);

            _fxPlayer.StopFx(particle);
        }

        private void UnsubscribeFromEvents()
        {
            _health.HealthChanged -= OnHealthChanged;
            _health.Died -= OnDied;
        }

        public void Dispose()
        {
            StopAllCoroutines();
            UnsubscribeFromEvents();
        }

        private void StopAllCoroutines()
        {
            if (_coroutineRunner != null)
            {
                _coroutineRunner.StopCoroutine(_onDiedRoutine);
                _coroutineRunner.StopCoroutine(_onHealthChangedRoutine);
            }
        }
    }
}