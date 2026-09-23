using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Core.AI.Core.Components;
using _Root._Scripts.Core.AI.Npcs.Brains;
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
    public class LobbyNpcRoot : NpcRoot
    {
        public override Transform Transform => transform;

        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private ExampleCharacterController _controller;
        [SerializeField] private NpcAnimationPlayer _animationPlayer;
        [SerializeField] private KinematicCharacterMotor _motor;

        private INpcBrain _brain;
        private NpcContext _localContext;
        private IRandomNavMeshPointService _randomNavMeshPointService;

        public void Init(IConfigProvider configProvider, INpcBehavioursProvider npcBehavioursProvider,
            INpcStateMachineRegistrar registrar, IGlobalNpcContextProvider globalNpcContextProvider,
            IRandomNavMeshPointService randomNavMeshPointService,ISfxPlayer sfxPlayer,IFxPlayer fxPlayer)
        {
            base.Init(configProvider, npcBehavioursProvider, registrar, globalNpcContextProvider,sfxPlayer, fxPlayer);

            _randomNavMeshPointService = randomNavMeshPointService;
            
            SetUpLocalContext();
            InitializeComponents();
        }


        private void SetUpLocalContext()
        {
            _localContext = new NpcContext()
            {
                Self = transform,
                Agent = _agent,
                CharacterController = _controller,
                AnimationPlayer = _animationPlayer,
                Animator = _animationPlayer.UnityAnimator,
                RandomNavMeshPointService = _randomNavMeshPointService,
            };
        }

        private void InitializeComponents()
        {
            _motor.Init();
            _controller.Init();
            _animationPlayer.Init();

            _brain = new LobbyNpcBrain(ConfigProvider, NpcBehavioursProvider, Registrar, GlobalNpcContextProvider,
                _localContext);
        }

        private void FixedUpdate()
        {
            if (_brain != null)
                _brain.Tick();
        }
    }
}