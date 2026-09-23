using _Root._Scripts.Core.AI.Core;

namespace _Root._Scripts.Infrastructure.Services.Npc.StateMachine
{
    public interface IStateBehaviour
    {
        void Enter(NpcContext localContext, GlobalNpcContext globalContext);
        void Tick(NpcContext localContext, GlobalNpcContext globalContext);
        void Exit(NpcContext localContext, GlobalNpcContext globalContext);
    }
}