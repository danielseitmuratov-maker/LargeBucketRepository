using System;
using System.Collections.Generic;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.Character.CharacterGameRoles.States.Murder;
using _Root._Scripts.Core.Character.Handlers;
using _Root._Scripts.Core.Roles;
using _Root._Scripts.Infrastructure;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Fx;
using _Root._Scripts.Infrastructure.Services.Input;
using _Root._Scripts.Infrastructure.Services.Interfaces;
using _Root._Scripts.Infrastructure.Services.Saves;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using _Root._Scripts.Infrastructure.Services.Sfx.Character;
using _Root._Scripts.Infrastructure.Services.Teleporters;
using _Root._Scripts.Tools.ShaderTools;
using _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine;

namespace _Root._Scripts.Core.Character
{
    public class CharacterRoot : MonoBehaviour
    {
        [Header("MovementSettings")] [SerializeField]
        private KinematicCharacterMotor _motor;

        [SerializeField] private ExampleCharacterController _characterController;
        [SerializeField] private ExampleCharacterCamera _camera;
        [SerializeField] private Transform _cameraFollowPoint;
        [SerializeField] private Animator _animator;

        [Header("General")] [SerializeField] private CharacterConfig _config;
        [SerializeField] private List<InteractiveObjectShaderApplier> _shaderAppliers;
        [SerializeField] private Transform _auraOutlinSpawnTransform;


        private IMovement _movement;
        private CharacterAnimator _characterAnimator;

        private IInputService _inputService;
        private ISaveLoadService _saveLoadService;
        private IHealth _health;

        private ITeleporter _teleporter;
        private CharacterSoundHandler _soundHandler;
        private CharacterEffectsHandler _effectsHandler;
        private ICharacterSfxPlayer _characterSfxPlayer;
        private IConfigProvider _configProvider;
        private ISfxPlayer _sfxPlayer;
        private ICoroutineRunner _coroutineRunner;
        private ICharacterAttack _attack;
        private IRoleDispatcher _roleDispatcher;
        private IGameRoleFortuneWheel _gameRoleFortuneWheel;
        private GameRole _currentGameRole;
        private IFxPlayer _fxPlayer;

        public void Init(IInputService inputService, ISaveLoadService saveLoadService, IConfigProvider configProvider,
            ISfxPlayer sfxPlayer, ICoroutineRunner coroutineRunner,
            IGameRoleFortuneWheel gameRoleFortuneWheel, IFxPlayer fxPlayer)
        {
            _inputService = inputService;
            _saveLoadService = saveLoadService;
            _configProvider = configProvider;
            _sfxPlayer = sfxPlayer;
            _coroutineRunner = coroutineRunner;
            _gameRoleFortuneWheel = gameRoleFortuneWheel;
            _fxPlayer = fxPlayer;

            InitializeComponents();
            SubscribeToEvents();
        }

        private void InitializeComponents()
        {
            InitializeCoreComponents();
            InitializeHandlers();
            InitializeShaderComponents();
        }

        private void InitializeCoreComponents()
        {
            _motor.Init();

            _characterController.Init();
            _camera.Init();
            _camera.SetFollowTransform(_cameraFollowPoint);

            _teleporter = new СharacterTeleporter(_motor.transform, _coroutineRunner);
            _health = new CharacterHealth();
            _movement = new CharacterMovement(_inputService, _configProvider);
            _attack = new CharacterAttack(transform, _configProvider);
            _movement.Init(_characterController, _camera);
        }

        private void InitializeShaderComponents()
        {
            for (int i = 0; i < _shaderAppliers.Count; i++)
                _shaderAppliers[i].Init();
        }

        private void InitializeHandlers()
        {
            _characterSfxPlayer = new CharacterSfxPlayer(_sfxPlayer, _configProvider, transform);
            _characterAnimator = new CharacterAnimator(_animator, _movement,_inputService);
            _soundHandler = new CharacterSoundHandler(_characterSfxPlayer, _movement, _configProvider);
            _effectsHandler = new CharacterEffectsHandler(_configProvider, _movement);
        }

        private void SubscribeToEvents()
        {
            _gameRoleFortuneWheel.OnSpinCompleted += OnGameRoleFortuneWheelSpinCompleted;
        }

        private void UnsubscribeFromEvents()
        {
            _gameRoleFortuneWheel.OnSpinCompleted -= OnGameRoleFortuneWheelSpinCompleted;
        }

        private void Update()
        {
            if (_movement == null)
                return;

            _movement.Move(Time.deltaTime);

            _roleDispatcher?.Tick();
        }

        public void TeleportSelfWithDelay(Vector3 to, float delay = 0f) =>
            _motor.SetPosition(_teleporter.Teleport(to, delay));

//================================= event handlers ====================================================
        private void OnGameRoleFortuneWheelSpinCompleted(GameRoleFortuneWheelSegment obj) => 
            InitializeRoleDispatcher(obj.GameRole);

        private void InitializeRoleDispatcher(GameRole gameRole)
        {
            switch (gameRole)
            {
                case GameRole.Peaceful:
                {
                    break;
                }
                case GameRole.Murder:
                {
                    _roleDispatcher = new MurderRoleDispatcher( gameRole, _fxPlayer, _characterSfxPlayer,
                        _configProvider, transform, _coroutineRunner,_attack,_inputService);
                    Debug.Log($"current roleDispatcher : {_roleDispatcher} ");
                    break;
                }
                case GameRole.Sheriff:
                {
                    break;
                }
                case GameRole.Doctor:
                {
                    break;
                }
            }
        }

        private void OnDestroy()
        {
            if (_movement is IDisposable disposableMovement)
                disposableMovement.Dispose();

            _soundHandler?.Dispose();
            _effectsHandler?.Dispose();

            UnsubscribeFromEvents();
        }
    }
}