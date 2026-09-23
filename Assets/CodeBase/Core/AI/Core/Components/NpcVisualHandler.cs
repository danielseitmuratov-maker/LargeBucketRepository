using System;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core.Components
{
    public class NpcVisualHandler : IDisposable
    {
        private readonly IConfigProvider _configProvider;
        private readonly IHealth _health;
        private readonly Transform _npcTransform;
        private DoTweenAnimationsConfig _config;

        public NpcVisualHandler(IConfigProvider configProvider, IHealth health, Transform npcTransform)
        {
            _configProvider = configProvider;
            _health = health;
            _npcTransform = npcTransform;

            GetConfig();
            SubscribeToEvents();
        }

        private void GetConfig()
        {
            _config = _configProvider.GetConfig<DoTweenAnimationsConfig>(Paths.GlobalValues
                .DOTweenAnimationsConfigPath);
        }

        private void SubscribeToEvents()
        {
            _health.HealthChanged += OnHealthChanged;
        }

        private void UnsubscribeFromEvents()
        {
            _health.HealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(float damage)
        {
            if (damage < 0)
                return;

            Vector3 resultVector = CalculateResultVector(damage);

            _npcTransform
                .DOPunchScale(resultVector, .5f, 1, .5f)
                .SetEase(Ease.OutBounce);
        }

        private Vector3 CalculateResultVector(float damage)
        {
            float currentHealth = _health.CurrentHealth;
            float currentRateOfChange = currentHealth / damage;
            
            float xResult = (_npcTransform.localScale.x + _config.GetDamagePunchScale.x) * currentRateOfChange;
            float yResult = (_npcTransform.localScale.y + _config.GetDamagePunchScale.y) * currentRateOfChange;
            float zResult = (_npcTransform.localScale.z + _config.GetDamagePunchScale.z) * currentRateOfChange;

            Vector3 resultVector = new Vector3(xResult, yResult, zResult);

            return resultVector;
        }


        public void Dispose()
        {
            UnsubscribeFromEvents();
        }
    }
}