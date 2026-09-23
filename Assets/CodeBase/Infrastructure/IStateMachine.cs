namespace _Root._Scripts.Infrastructure
{
    public interface IStateMachine
    {
        void Enter<TState>() where TState : class, IState;
    }
    
}