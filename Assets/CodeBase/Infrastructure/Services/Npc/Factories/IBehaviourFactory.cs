using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;

namespace _Root._Scripts.Infrastructure.Services.Npc.Factories
{
    public interface IBehaviourFactory
    {
        IStateBehaviour Create();
    }
}