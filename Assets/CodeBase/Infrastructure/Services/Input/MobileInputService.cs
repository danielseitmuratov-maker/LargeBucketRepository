using System;
using _Root._Scripts.Infrastructure.Services.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CodeBase.Infrastructure.Services.Input
{
    public class MobileInputService : IInputService, IDisposable
    {
        public event Action<Vector2> SwipeDirectionChanged;
        public event Action JumpPerformed;
        public event Action AttackPerformed;
        public event Action AutoRunToggled;
        public event Action AutoAttackToggled;

        public Vector2 SwipeDirection { get; private set; }
        public Vector2 MoveAxis { get; }

        private readonly IJumpButton _jumpButton;
        private readonly IAttackButton _attackButton;
        private readonly IAutoRunButton _autoRunButton;
        private readonly IAutoAttackButton _autoAttackButton;

        private GameInput _input;

        public MobileInputService(IJumpButton jumpButton, IAttackButton attackButton, IAutoRunButton autoRunButton,
            IAutoAttackButton autoAttackButton)
        {
            _jumpButton = jumpButton;
            _attackButton = attackButton;
            _autoRunButton = autoRunButton;
            _autoAttackButton = autoAttackButton;

            _input = new GameInput();
            _input.Enable();

            _input.Player.Swipe.performed += OnSwipePerformed;

            _jumpButton.Performed += OnJumpPerformed;
            _attackButton.Performed += OnAttackPerformed;
            _autoAttackButton.Performed += OnAutoAttackPerformed;
            _autoRunButton.Performed += OnAutoRunPerformed;
        }


        public Vector2 ReadMovement() =>
            _input.Player.Move.ReadValue<Vector2>();

        public Vector2 ReadLook() =>
            _input.Player.Look.ReadValue<Vector2>();

        public Vector2 ReadZoom() =>
            _input.Player.Zoom.ReadValue<Vector2>();
        
        public Vector2 ReadCameraRotation() => 
            _input.Player.CameraRotation.ReadValue<Vector2>();

        private void OnSwipePerformed(InputAction.CallbackContext context)
        {
            Vector2 delta = context.ReadValue<Vector2>();

            if (delta.sqrMagnitude > 0.0001f)
                SwipeDirection = delta.normalized;
            else
                SwipeDirection = Vector2.zero;

            SwipeDirectionChanged?.Invoke(SwipeDirection);
        }

        private void OnAutoRunPerformed() =>
            AutoRunToggled?.Invoke();

        private void OnAutoAttackPerformed() =>
            AutoAttackToggled?.Invoke();

        private void OnAttackPerformed() =>
            AttackPerformed?.Invoke();

        private void OnJumpPerformed() =>
            JumpPerformed?.Invoke();

        public void Dispose()
        {
            _input.Player.Swipe.performed -= OnSwipePerformed;
            _jumpButton.Performed -= OnJumpPerformed;
            _attackButton.Performed -= OnAttackPerformed;
            _autoAttackButton.Performed -= OnAutoAttackPerformed;
            _autoRunButton.Performed -= OnAutoRunPerformed;

            _input.Disable();
            _input?.Dispose();
        }
    }
}