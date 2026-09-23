using System.Collections.Generic;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.AI.Behaviours;
using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Npc.Providers;
using _Root._Scripts.Infrastructure.Services.Npc.Registrars;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;

namespace _Root._Scripts.Core.AI.Npcs.Brains
{
    public class LobbyNpcBrain : INpcBrain
    {
        private readonly IConfigProvider _configProvider;
        private readonly INpcBehavioursProvider _npcBehavioursProvider;
        private readonly INpcStateMachineRegistrar _stateMachineRegistrar;
        
        private NpcRoleConfigSo _roleConfig;
        private List<IStateBehaviour> _behaviors = new List<IStateBehaviour>();
        private IBehaviourStateMachine _stateMachine;


        private bool _isInitialized;
        private IGlobalNpcContextProvider _globalNpcContextProvider;
        private readonly NpcContext _localContext;
        private GlobalNpcContext _globalContext;

        public LobbyNpcBrain(IConfigProvider configProvider, INpcBehavioursProvider npcBehavioursProvider,
            INpcStateMachineRegistrar stateMachineRegistrar,IGlobalNpcContextProvider globalNpcContextProvider,NpcContext localContext)
        {
            _configProvider = configProvider;
            _npcBehavioursProvider = npcBehavioursProvider;
            _stateMachineRegistrar = stateMachineRegistrar;
            _globalNpcContextProvider = globalNpcContextProvider;
            _localContext = localContext;

            SetUpRoleConfig();
            SetUpBehaviours();
            GetGlobalContext();
            SetUpStateMachine();

           
            _stateMachine.Enter<RunOnNavMeshBehaviour>();


            _isInitialized = true;
        }

        private void SetUpRoleConfig() => 
            _roleConfig = _configProvider.GetConfig<NpcRoleConfigSo>(Paths.NpcData.Roles.LobbyNpcConfigRolePath);
        
        private void SetUpBehaviours() => 
            _behaviors = GetBehavioursByRole(_roleConfig);

        private void GetGlobalContext() => 
            _globalContext = _globalNpcContextProvider.GetGlobalContext();

        private void SetUpStateMachine()
        {
            _stateMachine = _stateMachineRegistrar.CreateStateMachine(_behaviors);
            
            for (int i = 0; i < _behaviors.Count; i++) 
                _stateMachine.RegisterBehaviour(_behaviors[i]);

            _stateMachine.SetContexts(_localContext,_globalContext);
        }

        private List<IStateBehaviour> GetBehavioursByRole(NpcRoleConfigSo roleConfig) =>
            _npcBehavioursProvider.CreateBehavioursForRole(roleConfig);

        public void Tick()
        {
            if (!_isInitialized)
                return;
            
            // go to special zone 
            // transformation
            // collect coins

            // if murmder nearby - run away 
            
            _stateMachine.Tick();
        }
    }
}