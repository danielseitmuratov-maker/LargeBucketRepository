using System;
using _Root._Scripts.Infrastructure.Services.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CodeBase.Infrastructure.Services.Input
{
    public class PcInputService : IInputService, IDisposable
    {
        
        public event Action<Vector2> SwipeDirectionChanged;
        public event Action JumpPerformed;
        public event Action AttackPerformed;
        public event Action AutoRunToggled;
        public event Action AutoAttackToggled;
        
        private readonly IAutoRunButton _autoRunButton;
        private readonly IAutoAttackButton _autoAttackButton;
        
        public Vector2 SwipeDirection { get; private set; }
        public Vector2 MoveAxis { get; }
        

        private GameInput _input;

        public PcInputService(IAutoRunButton autoRunButton,IAutoAttackButton autoAttackButton)
        {
            _autoRunButton = autoRunButton;
            _autoAttackButton = autoAttackButton;

            _input = new GameInput();
            _input.Enable();

            _input.Player.Jump.performed += context => JumpPerformed?.Invoke();
            _input.Player.Attack.performed += context => AttackPerformed?.Invoke();
            
            _input.Player.Swipe.performed += OnSwipePerformed;
            _input.Player.Touch.canceled += OnTouchCanceled;

            _autoRunButton.Performed += OnAutoRunPerformed;
            _autoAttackButton.Performed += OnAutoAttackPerformed;
        }

        private Vector2 CalculateSwipeDirection(Vector2 swipeDirection)
        {
            if (swipeDirection.x > 0)
                swipeDirection.x = 1;
            if (swipeDirection.x < 0)
                swipeDirection.x = -1;
            if (swipeDirection.y > 0)
                swipeDirection.y = 1;
            if (swipeDirection.y < 0)
                swipeDirection.y = -1;
            
            return swipeDirection;
        }

        public Vector2 ReadMovement() =>
            _input.Player.Move.ReadValue<Vector2>();

        public Vector2 ReadLook() =>
            _input.Player.Look.ReadValue<Vector2>();

        public Vector2 ReadZoom() => 
            _input.Player.Zoom.ReadValue<Vector2>();

        public Vector2 ReadCameraRotation() => 
            _input.Player.CameraRotation.ReadValue<Vector2>();

        private void OnTouchCanceled(InputAction.CallbackContext obj)
        {
            Vector2 swipeDirection = SwipeDirection;
            
            swipeDirection = CalculateSwipeDirection(swipeDirection);

            SwipeDirectionChanged?.Invoke(swipeDirection);
        }
        
        private void OnSwipePerformed(InputAction.CallbackContext context)
        {
            Vector2 delta = context.ReadValue<Vector2>();

            if (delta.sqrMagnitude > 0.0001f)
                SwipeDirection = delta.normalized;
            else
                SwipeDirection = Vector2.zero;
        }

        private void OnAutoAttackPerformed() => 
            AutoAttackToggled?.Invoke();

        private void OnAutoRunPerformed() => 
            AutoRunToggled?.Invoke();

        public void Dispose()
        {
            _input.Player.Swipe.performed -= OnSwipePerformed;
            _input.Player.Touch.canceled -= OnTouchCanceled;

            _autoRunButton.Performed -= OnAutoRunPerformed;
            _autoAttackButton.Performed -= OnAutoAttackPerformed;
            
            _input.Disable();
            _input?.Dispose();
        }
    }
}