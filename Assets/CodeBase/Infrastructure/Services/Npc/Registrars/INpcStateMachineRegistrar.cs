using System.Collections.Generic;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;

namespace _Root._Scripts.Infrastructure.Services.Npc.Registrars
{
    public interface INpcStateMachineRegistrar
    {
        IBehaviourStateMachine CreateStateMachine(List<IStateBehaviour> behaviours);
    }
}