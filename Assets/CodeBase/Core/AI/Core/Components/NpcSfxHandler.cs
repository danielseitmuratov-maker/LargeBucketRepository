using System;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Interfaces;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core.Components
{
    public class NpcSfxHandler : IDisposable
    {
        private readonly ISfxPlayer _sfxPlayer;
        private readonly IConfigProvider _configProvider;
        private readonly IHealth _health;
        private readonly Transform _npcTransform;
        private SoundsConfig _config;

        public NpcSfxHandler(ISfxPlayer sfxPlayer, IConfigProvider configProvider, IHealth health,
            Transform npcTransform)
        {
            _sfxPlayer = sfxPlayer;
            _configProvider = configProvider;
            _health = health;
            _npcTransform = npcTransform;

            GetConfig();
            SubscribeToEvents();
        }

        private void GetConfig()
        {
            _config = _configProvider.GetConfig<SoundsConfig>(Paths.GlobalValues.SoundsConfigPath);
        }

        private void SubscribeToEvents()
        {
            _health.HealthChanged += OnHealthChanged;
            _health.Died += OnDied;
        }

        private void OnDied()
        {
            _sfxPlayer.PlaySfx(_config.NpcDiedSound,_npcTransform.position);
        }

        private void OnHealthChanged(float obj)
        {
            _sfxPlayer.PlaySfx(_config.NpcGetDamageSound, _npcTransform.position);
        }

        private void UnsubscribeFromEvents()
        {
            _health.HealthChanged -= OnHealthChanged;
            _health.Died -= OnDied;
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();
        }
    }
}