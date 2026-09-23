namespace _Root._Scripts.Infrastructure
{
    public interface IState : IExitableState
    {
        public void Enter();
    }
}