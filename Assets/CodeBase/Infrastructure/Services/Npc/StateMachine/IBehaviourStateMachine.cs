using _Root._Scripts.Core.AI.Core;

namespace _Root._Scripts.Infrastructure.Services.Npc.StateMachine
{
    public interface IBehaviourStateMachine
    {
        void SetContexts(NpcContext localContext, GlobalNpcContext globalContext);
        void Enter<T>() where T : class, IStateBehaviour;
        void Tick();
        void RegisterBehaviour<T>(T behaviour) where T : IStateBehaviour;
    }
}