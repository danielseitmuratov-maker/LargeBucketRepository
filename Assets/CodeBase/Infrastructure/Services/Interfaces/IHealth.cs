using System;

namespace CodeBase.Infrastructure.Services.Interfaces
{
    public interface IHealth
    {
        event Action<float> HealthChanged;
        event Action Died;

        float MaxHealth { get; }
        float CurrentHealth { get; }

        void TakeDamage(float damage);

        void Heal(float value);
    }
}