using System;
using _Root._Scripts.Core.AI.Core.Components.Animations;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core.Components
{
    public sealed class NpcAnimationPlayer : MonoBehaviour, INpcAnimator
    {
        public event Action AttackAnimationFinished;
        public event Action GetDamageAnimationFinished;
        public event Action SpawnedAnimationFinished;
        public event Action DeathAnimationFinished;

        public bool IsInitialized => _isInitialized;

        public bool IsDead => _isDead;

        public NpcAnimationState CurrentState => _currentState;
        public Animator UnityAnimator => _animator;

        private static readonly int SpeedMagnitude = Animator.StringToHash("SpeedMagnitude");
        private static readonly int AnimationState = Animator.StringToHash("AnimationState");
        private static readonly int IsAttack = Animator.StringToHash("IsAttack");
        private static readonly int IsDamaged = Animator.StringToHash("IsDamaged");
        private static readonly int IsSpawn = Animator.StringToHash("IsSpawn");
        private static readonly int IsDied = Animator.StringToHash("IsDied");
        private static readonly int IsHeal = Animator.StringToHash("IsHeal");
        private static readonly int IsJump = Animator.StringToHash("IsJump");


        [SerializeField] private Animator _animator;

        private bool _isInitialized;
        private bool _isDead;
        private NpcAnimationState _currentState = NpcAnimationState.Idle;

        public void Init()
        {
            if (_isInitialized)
                return;

            if (_animator == null) 
                _animator = GetComponent<Animator>();

            if (_animator == null)
            {
                Debug.LogError(
                    $"[{name}] Animator не найден.",
                    this);

                return;
            }

            SetUpStartValues();

            _isInitialized = true;
        }

        public void SetState(NpcAnimationState state)
        {
            if (!_isInitialized || _animator == null || _isDead)
                return;

            if (_currentState == state)
                return;

            _currentState = state;

            _animator.SetInteger(AnimationState, (int)state);
        }

        public void PlayMovement(float magnitude)
        {
            if (!_isInitialized || _animator == null || _isDead)
                return;

            _animator.SetFloat(SpeedMagnitude, Mathf.Max(0f, magnitude));
        }

        public void PlayJump()
        {
            _animator.SetTrigger(IsJump);
        }

        public void PlayAttack()
        {
            if (!_isInitialized || _animator == null ||_isDead)
                return;

            ResetTrigger(IsAttack);
            _animator.SetTrigger(IsAttack);
        }

        public void GetDamage()
        {
            if (!_isInitialized || _animator == null || _isDead)
                return;

            ResetTrigger(IsDamaged);
            _animator.SetTrigger(IsDamaged);
        }

        public void Spawn()
        {
            if (!_isInitialized || _animator == null || _isDead)
                return;

            ResetTrigger(IsSpawn);
            _animator.SetTrigger(IsSpawn);
        }

        public void PlayDeath()
        {
            if (!_isInitialized || _animator == null || _isDead)
                return;

            _isDead = true;
            _currentState = NpcAnimationState.Dead;

            _animator.SetFloat(SpeedMagnitude, 0f);

            _animator.SetInteger(AnimationState, (int)NpcAnimationState.Dead);

            ResetTrigger(IsAttack);
            ResetTrigger(IsDamaged);
            ResetTrigger(IsSpawn);

            _animator.SetTrigger(IsDied);
        }

        public void PlayHeal()
        {
            _animator.SetTrigger(IsHeal);
        }
        

        public void NotifyAttackFinished()
        {
            if (!_isDead)
                AttackAnimationFinished?.Invoke();
        }

        public void NotifyGetDamageFinished()
        {
            if (!_isDead)
                GetDamageAnimationFinished?.Invoke();
        }

        public void NotifySpawnFinished()
        {
            if (!_isDead)
                SpawnedAnimationFinished?.Invoke();
        }

        public void NotifyDeathFinished()
        {
            DeathAnimationFinished?.Invoke();
        }

        public void Dispose()
        {
            _isInitialized = false;
            _isDead = false;
            _currentState = NpcAnimationState.Idle;
        }

        private void ResetTrigger(int triggerHash) => 
            _animator.ResetTrigger(triggerHash);

        private void SetUpStartValues()
        {
            _animator.applyRootMotion = false;
            _animator.SetInteger(AnimationState, (int) NpcAnimationState.Idle);
            _animator.SetFloat(SpeedMagnitude, 0f);
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}