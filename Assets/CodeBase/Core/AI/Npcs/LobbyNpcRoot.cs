using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Core.AI.Npcs.Brains;
using _Root._Scripts.Infrastructure.Services.RandomPoints;
using CodeBase.Core.AI.Core.Components;
using CodeBase.Core.AI.Core.Components.Animations;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace CodeBase.Core.AI.Npcs
{
    public class LobbyNpcRoot : NpcRoot ,IInitializable
    {
        public override Transform Transform => transform;

        [SerializeField] private NavMeshAgent _agent;
        
        private ExampleCharacterController _controller;
        private NpcAnimationPlayer _animationPlayer; 
        private KinematicCharacterMotor _motor;

        private INpcBrain _brain;
        private NpcContext _localContext;
        private IRandomNavMeshPointService _randomNavMeshPointService;

        [Inject]
        public void Construct(KinematicCharacterMotor motor, NpcAnimationPlayer animationPlayer,
            ExampleCharacterController controller,INpcAnimator npcAnimator,IRandomNavMeshPointService randomNavMeshPointService)
        {
            _motor = motor;
            _animationPlayer = animationPlayer;
            _controller = controller;
            _randomNavMeshPointService = randomNavMeshPointService;
        }

        public void Initialize()
        {
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