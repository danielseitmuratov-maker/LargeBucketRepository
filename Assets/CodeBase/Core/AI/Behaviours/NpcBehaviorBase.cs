using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;

namespace _Root._Scripts.Core.AI.Behaviours
{
    public abstract class NpcBehaviorBase : IStateBehaviour
    {
        public abstract void Enter(NpcContext localContext, GlobalNpcContext globalContext);

        public abstract void Tick(NpcContext localContext, GlobalNpcContext globalContext);

        public abstract void Exit(NpcContext localContext, GlobalNpcContext globalContext);
    }
}