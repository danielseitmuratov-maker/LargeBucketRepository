using System;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core.Components.Animations
{
    public interface INpcAnimator : IDisposable
    {
        event Action AttackAnimationFinished;
        event Action GetDamageAnimationFinished;
        event Action SpawnedAnimationFinished;
        event Action DeathAnimationFinished;

        bool IsInitialized { get; }
        bool IsDead { get; }

        NpcAnimationState CurrentState { get; }
        Animator UnityAnimator { get; }

        void Init();
        void SetState(NpcAnimationState state);
        void PlayMovement(float magnitude);
        void PlayJump();
        void PlayAttack();
        void GetDamage();
        void Spawn();
        void PlayDeath();
        void PlayHeal();
    }
}