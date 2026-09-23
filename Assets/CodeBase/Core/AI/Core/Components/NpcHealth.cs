using System;
using _Root._Scripts.Infrastructure.Services.Interfaces;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core.Components
{
    public class NpcHealth : MonoBehaviour, IHealth
    {
        public event Action<float> HealthChanged;
        public event Action Died;

        private float _maxHealth;
        private float _currentHealth;

        public void Init(float maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
        }

        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;
        public bool IsDead => _currentHealth <= 0f;

        public void TakeDamage(float damage)
        {
            if (IsDead)
                return;

            _currentHealth = Math.Max(0f, _currentHealth - damage);
            Debug.Log($"NpcHealth: took {damage} damage, current health {_currentHealth}");
            HealthChanged?.Invoke(_currentHealth);

            if (IsDead)
                Died?.Invoke();
        }

        public void Heal(float value)
        {
            if (IsDead) return;

            _currentHealth = Math.Min(_maxHealth, _currentHealth + value);
            HealthChanged?.Invoke(_currentHealth);
        }

        public void ResetHealth()
        {
            _currentHealth = _maxHealth;
            HealthChanged?.Invoke(_currentHealth);
        }
    }
}