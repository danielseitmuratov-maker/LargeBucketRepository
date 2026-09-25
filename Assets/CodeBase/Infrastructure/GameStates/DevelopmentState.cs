using CodeBase.Infrastructure;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.GameStates
{
    public class DevelopmentState : IState
    {
        private readonly StateMachine _stateMachine;

        public DevelopmentState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Exit()
        {
        }

        public void Enter()
        {
            Debug.Log("gameloop");
        }
    }
}