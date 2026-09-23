using System;

namespace _Root._Scripts.Infrastructure.Services.Interfaces
{
    public interface IHealth
    {
        event Action<float> HealthChanged;
        event Action Died;

        void Init(float maxHealth);
        float MaxHealth { get; }
        float CurrentHealth { get; }

        void TakeDamage(float damage);

        void Heal(float value);
    }
}