using System;
using _Root._Scripts.Core.Character.CharacterGameRoles.States.Murder;
using _Root._Scripts.Infrastructure.Services.Input;
using UnityEngine;

namespace _Root._Scripts.Core.Character
{
    public class CharacterAnimator : IDisposable
    {
        private static readonly int MoveSpeed = Animator.StringToHash("moveSpeed");
        private static readonly int JumpTrigger = Animator.StringToHash("jumpTrigger");
        private static readonly int AttackTrigger = Animator.StringToHash("attackTrigger");
        
        private readonly Animator _animator;
        private readonly IMovement _movement;
        private readonly IInputService _inputService;

        public CharacterAnimator(Animator animator,IMovement movement,IInputService inputService)
        {
            _animator = animator;
            _movement = movement;
            _inputService = inputService;
            
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            if (_movement != null)
            {
                _movement.MoveSpeedChanged += OnMoveSpeedChanged;
                _movement.Jumped += OnJumped;
            }

            if (_inputService != null)
            {
                _inputService.AttackPerformed += OnAttackPerformed;
            }
        }
        
        
        private void UnsubscribeFromEvents()
        {
            if (_movement != null)
            {
                _movement.MoveSpeedChanged -= OnMoveSpeedChanged;
                _movement.Jumped -= OnJumped;
            }
            
            if (_inputService != null)
            {
                _inputService.AttackPerformed -= OnAttackPerformed;
            }
        }

        private void OnMoveSpeedChanged(float obj) => 
            _animator.SetFloat(MoveSpeed,obj);

        private void OnJumped() => 
            _animator.SetTrigger(JumpTrigger);

        private void OnAttackPerformed() => 
            _animator.SetTrigger(AttackTrigger);


        public void Dispose()
        {
           UnsubscribeFromEvents();
        }
    }
}