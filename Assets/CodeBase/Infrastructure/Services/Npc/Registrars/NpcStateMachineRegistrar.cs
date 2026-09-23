using System.Collections.Generic;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;

namespace _Root._Scripts.Infrastructure.Services.Npc.Registrars
{
    public class NpcStateMachineRegistrar : INpcStateMachineRegistrar
    {
        public IBehaviourStateMachine CreateStateMachine(List<IStateBehaviour> behaviours)
        {
            IBehaviourStateMachine stateMachine = new BehaviourStateMachine(behaviours);
            return stateMachine;
        }
    }
}