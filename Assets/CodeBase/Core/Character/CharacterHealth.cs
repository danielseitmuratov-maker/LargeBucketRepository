using System;
using CodeBase.Infrastructure.Services.Interfaces;

namespace CodeBase.Core.Character
{
    public class CharacterHealth : ICharacterHealth
    {
        public event Action<float> HealthChanged;
        public event Action Died;

        public CharacterHealth(float maxHealth)
        {
            MaxHealth = maxHealth;
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