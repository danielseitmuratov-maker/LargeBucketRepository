using System;
using _Root._Scripts.Infrastructure.Services.Interfaces;

namespace _Root._Scripts.Core.Character
{
    public class CharacterHealth : IHealth
    {
        public event Action<float> HealthChanged;
        public event Action Died;

        public void Init(float maxHealth)
        {
            
        }

        public float MaxHealth { get; }
        public float CurrentHealth { get; }
        

        public void TakeDamage(float damage)
        {
        }

        public void Heal(float value)
        {
        }
    }
}