using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Core.AI.Core.Components;
using _Root._Scripts.Core.AI.Npcs.Brains;
using _Root._Scripts.Core.AI.Npcs.Core;
using _Root._Scripts.Core.GameMap;
using _Root._Scripts.Infrastructure;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Fx;
using _Root._Scripts.Infrastructure.Services.Npc.Providers;
using _Root._Scripts.Infrastructure.Services.Npc.Registrars;
using _Root._Scripts.Infrastructure.Services.RandomPoints;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine;
using UnityEngine.AI;

namespace _Root._Scripts.Core.AI.Npcs
{
    public class PeacefulNpcRoot : NpcRoot
    {
        public override Transform Transform => transform;

        [Header("Transformation")] [SerializeField]
        private Transform _normalModel;

        [SerializeField] private Transform _transformedModel;

        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private ExampleCharacterController _controller;
        [SerializeField] private NpcAnimationPlayer _animationPlayer;
        [SerializeField] private KinematicCharacterMotor _motor;

        [Header("Health")] [SerializeField] private NpcHealth _health;
        [SerializeField] private NpcHealthShaderChanger _healthShaderChanger;

        [SerializeField] private NpcRoleConfigSo _roleConfig;


        private IRandomNavMeshPointService _randomNavMeshPointService;
        private ICoroutineRunnerService _coroutineRunnerService;

        private INpcBrain _npcBrain;
        private NpcContext _localContext;
        private IGameMapSpecialPointsProvider _gameMapSpecialPointsProvider;
        
        private NpcSfxHandler _npcSfxHandler;
        private NpcVfxHandler _npcVfxHandler;
        private NpcVisualHandler _npcVisualHandler;

        public void Init(IConfigProvider configProvider, INpcBehavioursProvider npcBehavioursProvider,
            INpcStateMachineRegistrar stateMachineRegistrar, IGlobalNpcContextProvider globalNpcContextProvider,
            IRandomNavMeshPointService randomNavMeshPointService, ICoroutineRunnerService coroutineRunnerService,
            IGameMapSpecialPointsProvider gameMapSpecialPointsProvider,ISfxPlayer sfxPlayer,IFxPlayer fxPlayer)
        {
            base.Init(configProvider,npcBehavioursProvider,stateMachineRegistrar,globalNpcContextProvider, sfxPlayer, fxPlayer);

            _randomNavMeshPointService = randomNavMeshPointService;
            _coroutineRunnerService = coroutineRunnerService;
            _gameMapSpecialPointsProvider = gameMapSpecialPointsProvider;

            SetUpLocalContext();
            InitializeComponents();
            SubscribeToEvents();
        }


        private void SubscribeToEvents()
        {
            if (_health != null)
                _health.HealthChanged += OnHealthChanged;
        }

        private void UnsubscribeFromEvents()
        {
            if (_health != null)
                _health.HealthChanged -= OnHealthChanged;
        }

        private void SetUpLocalContext()
        {
            _localContext = new NpcContext
            {
                Self = transform,
                Agent = _agent,
                CharacterController = _controller,
                AnimationPlayer = _animationPlayer,
                Animator = _animationPlayer.UnityAnimator,
                RandomNavMeshPointService = _randomNavMeshPointService,
                NormalModel = _normalModel,
                TransformedModel = _transformedModel,
                HidingPoint = _gameMapSpecialPointsProvider.GetRandomHidingPoint(),
                PersonSimulationTimeAmount = 5f,
            };
        }

        private void InitializeComponents()
        {
            _health.Init(_roleConfig.MaxHealth);

            _motor.Init();
            _controller.Init();
            _animationPlayer.Init();
            _healthShaderChanger.Init(_health, ConfigProvider, _coroutineRunnerService);

            _npcBrain = new PeacefulNpcBrain(ConfigProvider, NpcBehavioursProvider, Registrar, GlobalNpcContextProvider,
                _localContext);

            _npcSfxHandler = new NpcSfxHandler(SfxPlayer, ConfigProvider, _health, transform);
            _npcVfxHandler = new NpcVfxHandler(FxPlayer,ConfigProvider,_health,transform,_coroutineRunnerService);
            _npcVisualHandler = new NpcVisualHandler(ConfigProvider,_health,transform);
        }

        private void FixedUpdate()
        {
            _npcBrain.Tick();
        }


        private void OnHealthChanged(float obj) =>
            Debug.Log(obj);

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    }
}