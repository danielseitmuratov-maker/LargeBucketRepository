using System;

namespace _Root._Scripts.Infrastructure.GameStates
{
    public class TutorialState : IState, IDisposable
    {
        private readonly StateMachine _stateMachine;

        public TutorialState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }


        private void OnTutorialFinished()
        {
        }

        public void Enter()
        {
            _stateMachine.Enter<GameLoopState>();
        }

        public void Exit()
        {
        }

        public void Dispose()
        {
        }
    }
}