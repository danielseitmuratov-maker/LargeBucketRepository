using System;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Sfx.Character;
using UnityEngine;

namespace _Root._Scripts.Core.Character.Handlers
{
    public class CharacterSoundHandler : IDisposable
    {
        private readonly ICharacterSfxPlayer _sfxPlayer;
        private readonly IMovement _movement;
        private readonly CharacterConfig _config;

        private const float RunMinSpeedThreshold = 0.1f;

        private float _lastRunSfxTime = float.NegativeInfinity;
        private bool _isDisposed;
        private IConfigProvider _configProvider;

        public CharacterSoundHandler(
            ICharacterSfxPlayer sfxPlayer,
            IMovement movement,
            IConfigProvider configProvider)
        {
            _sfxPlayer = sfxPlayer;
            _movement = movement;
            _configProvider = configProvider;

            _config = configProvider.GetConfig<CharacterConfig>(
                Paths.GlobalValues.CharacterConfigPath);

            SubscribeToMovement();
        }

        private void SubscribeToMovement()
        {
            if (_movement == null)
                return;

            _movement.MoveSpeedChanged += OnMoveSpeedChanged;
            _movement.Jumped += OnJumped;
        }

        private void UnsubscribeFromEvents()
        {
            if (_movement == null)
                return;

            _movement.MoveSpeedChanged -= OnMoveSpeedChanged;
            _movement.Jumped -= OnJumped;
        }

        private void OnMoveSpeedChanged(float speed)
        {
            if (_isDisposed || _config == null)
                return;

            bool shouldRun = speed > RunMinSpeedThreshold;

            if (!shouldRun)
            {
                _lastRunSfxTime = float.NegativeInfinity;
                return;
            }

            float runSoundInterval = Mathf.Max(0.01f, _config.RunSoundInterval);

            if (Time.time - _lastRunSfxTime >= runSoundInterval)
            {
                _sfxPlayer.PlayRun();
                _lastRunSfxTime = Time.time;
            }
        }

        private void OnJumped()
        {
            if (_isDisposed)
                return;

            _sfxPlayer.PlayJump();
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            UnsubscribeFromEvents();
        }
    }
}