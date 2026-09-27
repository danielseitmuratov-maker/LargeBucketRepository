using System;
using _Root._Scripts.Core.Character;
using _Root._Scripts.Core.Character.CharacterGameRoles.States.Murder;
using _Root._Scripts.Core.Character.Handlers;
using _Root._Scripts.Core.Roles;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Fx;
using _Root._Scripts.Infrastructure.Services.Input;
using _Root._Scripts.Infrastructure.Services.Interfaces;
using _Root._Scripts.Infrastructure.Services.Saves;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using _Root._Scripts.Infrastructure.Services.Sfx.Character;
using _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services.Teleporters;
using KinematicCharacterController.Examples;
using UnityEngine;
using Zenject;

namespace CodeBase.Core.Character
{
    public class CharacterRoot : MonoBehaviour ,IInitializable,IDisposable
    {

        [SerializeField] private Animator _animator;

        private IMovement _movement;
        private CharacterAnimator _characterAnimator;

        private IInputService _inputService;
        private IHealth _health;

        private ITeleporter _teleporter;
        private CharacterSoundHandler _soundHandler;
        private CharacterEffectsHandler _effectsHandler;
        private ICharacterSfxPlayer _characterSfxPlayer;
        private IConfigProvider _configProvider;
        private ISfxPlayer _sfxPlayer;
        private ICoroutineRunnerService _coroutineRunnerService;
        private ICharacterAttack _attack;
        private IRoleDispatcher _roleDispatcher;
        private IGameRoleFortuneWheel _gameRoleFortuneWheel;
        private GameRole _currentGameRole;
        private IFxPlayer _fxPlayer;
        
        private Transform _cameraFollowPoint;
        private ExampleCharacterCamera _camera;
        private ExampleCharacterController _characterController;

        [Inject]
        public void Construct(ExampleCharacterCamera camera, Transform cameraFollowPoint)
        {
            _camera = camera;
            _cameraFollowPoint = cameraFollowPoint;
        }

        
        public void Initialize()
        {
            InitializeCoreComponents();
            SubscribeToEvents();
        }
        

        public void Init(IInputService inputService, ISaveLoadService saveLoadService, IConfigProvider configProvider,
            ISfxPlayer sfxPlayer, ICoroutineRunnerService coroutineRunnerService,
            IGameRoleFortuneWheel gameRoleFortuneWheel, IFxPlayer fxPlayer)
        {
            _inputService = inputService;
            _configProvider = configProvider;
            _sfxPlayer = sfxPlayer;
            _coroutineRunnerService = coroutineRunnerService;
            _gameRoleFortuneWheel = gameRoleFortuneWheel;
            _fxPlayer = fxPlayer;

            SubscribeToEvents();
        }

        private void InitializeComponents()
        {
            InitializeCoreComponents();
            InitializeHandlers();
        }

        private void InitializeCoreComponents()
        {
            _camera.SetFollowTransform(_cameraFollowPoint);
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
                        _configProvider, transform, _coroutineRunnerService,_attack,_inputService);
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

        public void Dispose()
        {
            _characterAnimator?.Dispose();
            _soundHandler?.Dispose();
            _effectsHandler?.Dispose();
            
            UnsubscribeFromEvents();
        }
    }
}