using System;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using UnityEngine;

namespace _Root._Scripts.Core.Character.Handlers
{
    public class CharacterEffectsHandler : IDisposable
    {
        private readonly IConfigProvider _configProvider;
        private readonly IMovement _movement;

        private readonly ParticleSystem _runEffectPrefab;
        private readonly ParticleSystem _jumpEffectPrefab;

        private ParticleSystem _runEffectInstance;

        private const float RunMinSpeedThreshold = 0.1f;

        private bool _wasRunning;
        private bool _isDisposed;

        public CharacterEffectsHandler(IConfigProvider configProvider, IMovement movement)
        {
            _configProvider = configProvider;
            _movement = movement;

            VfxConfig effectConfig =
                _configProvider.GetConfig<VfxConfig>(
                    Paths.GlobalValues.ParticleConfigPath);

            if (effectConfig == null)
            {
                Debug.LogError(
                    "[CharacterEffectsHandler] ParticleEffectConfig не найден.");
                return;
            }

            _runEffectPrefab = effectConfig.RunEffect;
            _jumpEffectPrefab = effectConfig.JumpEffect;

            SubscribeToMovement();
        }

        private void SubscribeToMovement()
        {
            if (_movement == null)
            {
                Debug.LogError(
                    "[CharacterEffectsHandler] IMovement равен null.");
                return;
            }

            _movement.MoveSpeedChanged += OnMoveSpeedChanged;
            _movement.Jumped += OnJumped;
        }

        private void UnsubscribeFromMovement()
        {
            if (_movement == null)
                return;

            _movement.MoveSpeedChanged -= OnMoveSpeedChanged;
            _movement.Jumped -= OnJumped;
        }

        private void OnMoveSpeedChanged(float speed)
        {
            if (_isDisposed)
                return;

            bool isRunning = speed > RunMinSpeedThreshold;

            if (isRunning)
            {
                StartRunEffect();
                UpdateRunEffectPosition();
            }
            else
            {
                StopRunEffect();
            }

            _wasRunning = isRunning;
        }

        private void StartRunEffect()
        {
            if (_runEffectInstance != null)
                return;

            if (_runEffectPrefab == null)
            {
                Debug.LogWarning(
                    "[CharacterEffectsHandler] RunEffect prefab не назначен.");
                return;
            }

            _runEffectInstance = UnityEngine.Object.Instantiate(
                _runEffectPrefab,
                _movement.Position,
                Quaternion.identity);

            _runEffectInstance.name = $"{_runEffectPrefab.name}_Runtime";
            _runEffectInstance.Play();
        }

        private void UpdateRunEffectPosition()
        {
            if (_runEffectInstance == null || _movement == null)
                return;

            _runEffectInstance.transform.position = _movement.Position;
        }

        private void StopRunEffect()
        {
            if (_runEffectInstance == null)
                return;

            _runEffectInstance.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear);

            UnityEngine.Object.Destroy(_runEffectInstance.gameObject);
            _runEffectInstance = null;
        }

        private void OnJumped()
        {
            if (_isDisposed)
                return;

            if (_jumpEffectPrefab == null)
            {
                Debug.LogWarning(
                    "[CharacterEffectsHandler] JumpEffect prefab не назначен.");
                return;
            }

            ParticleSystem jumpEffect = UnityEngine.Object.Instantiate(
                _jumpEffectPrefab,
                _movement.Position,
                Quaternion.identity);

            jumpEffect.name = $"{_jumpEffectPrefab.name}_Runtime";
            jumpEffect.Play();

            // Дополнительное удаление, если на prefab не установлен Stop Action = Destroy.
            float lifetime = GetParticleLifetime(jumpEffect);
            UnityEngine.Object.Destroy(jumpEffect.gameObject, lifetime);
        }

        private float GetParticleLifetime(ParticleSystem particleSystem)
        {
            ParticleSystem.MainModule main = particleSystem.main;

            float lifetime = main.duration;

            if (main.startLifetime.mode == ParticleSystemCurveMode.Constant)
                lifetime += main.startLifetime.constant;
            else
                lifetime += 2f;

            return Mathf.Max(0.1f, lifetime);
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            UnsubscribeFromMovement();
            StopRunEffect();
        }
    }
}