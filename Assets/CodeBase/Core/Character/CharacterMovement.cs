using System;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Input;
using UnityEngine;
using KinematicCharacterController.Examples;

namespace _Root._Scripts.Core.Character
{
    public class CharacterMovement : IMovement, IDisposable
    {
        public event Action<float> MoveSpeedChanged;
        public event Action Jumped;

        public Vector3 Position =>
            _characterController != null
                ? _characterController.transform.position
                : Vector3.zero;

        private readonly IInputService _inputService;
        private readonly IConfigProvider _configProvider;

        private ExampleCharacterController _characterController;
        private ExampleCharacterCamera _camera;
        private CharacterConfig _config;

        private bool _isAutoRunEnabled;
        private Vector2 _lastMovementInput;
        private bool _pendingJump;


        public CharacterMovement(IInputService inputService, IConfigProvider configProvider)
        {
            _inputService = inputService;
            _configProvider = configProvider;
        }

        public void Init(ExampleCharacterController characterController, ExampleCharacterCamera camera)
        {
            _characterController = characterController;
            _camera = camera;

            GetConfig();

            _inputService.JumpPerformed += OnJumpRequested;
            _inputService.AutoRunToggled += OnAutoRunToggled;
        }

        private void GetConfig()
        {
            _config = _configProvider.GetConfig<CharacterConfig>(
                Paths.GlobalValues.CharacterConfigPath);
        }

        public void Move(float deltaTime)
        {
            if (_characterController == null || _camera == null || _config == null)
            {
                return;
            }

            Vector2 moveInput = _inputService.ReadMovement();
            Vector3 cameraRotationInput = _inputService.ReadCameraRotation();

            _camera.UpdateWithInput(
                deltaTime,
                _inputService.ReadZoom().y,
                new Vector3(cameraRotationInput.x, cameraRotationInput.y, 0f));

            PlayerCharacterInputs inputs = new PlayerCharacterInputs
            {
                MoveAxisForward = moveInput.y,
                MoveAxisRight = moveInput.x,
                CameraRotation = _camera.Transform.rotation,
                JumpDown = false,
                CrouchDown = false,
                CrouchUp = false
            };

            // Jump срабатывает через событие, но ExampleCharacterController ждёт флаг в SetInputs
            // Поэтому держим отдельный флаг на 1 кадр
            if (_pendingJump)
            {
                inputs.JumpDown = true;
                _pendingJump = false;
            }

            _characterController.SetInputs(ref inputs);

            // Примерный speed для UI/FX/SFX
            Vector3 planarVelocity = Vector3.ProjectOnPlane(
                _characterController.Motor.BaseVelocity,
                _characterController.Motor.CharacterUp);

            MoveSpeedChanged?.Invoke(planarVelocity.magnitude);

            _lastMovementInput = moveInput;
        }


        private void OnJumpRequested()
        {
            _pendingJump = true;
            Jumped?.Invoke();
        }

        private void OnAutoRunToggled()
        {
            _isAutoRunEnabled = !_isAutoRunEnabled;
        }

        public void Dispose()
        {
            _inputService.JumpPerformed -= OnJumpRequested;
            _inputService.AutoRunToggled -= OnAutoRunToggled;
        }
    }
}

