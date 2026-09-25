using System;
using _Root._Scripts.Infrastructure.Services.Input;
using _Root._Scripts.Ui;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.GameStates;

namespace _Root._Scripts.Infrastructure.GameStates
{
    public class MainMenuState : IState, IDisposable
    {
        private readonly StateMachine _stateMachine;
        private readonly StartGameButtonHandler _startGameButtonHandler;
        private readonly MainHudHandler _mainHudHandler;

        public MainMenuState(StateMachine stateMachine, StartGameButtonHandler startGameButtonHandler,
            MainHudHandler mainHudHandler)
        {
            _stateMachine = stateMachine;
            _startGameButtonHandler = startGameButtonHandler;
            _mainHudHandler = mainHudHandler;

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _startGameButtonHandler.Performed += OnStartGameButtonHandlerPerformed;
        }

        private void UnsubscribeFromEvents()
        {
            _startGameButtonHandler.Performed -= OnStartGameButtonHandlerPerformed;
        }

        public void Enter()
        {
        }

        public void Exit()
        {
        }

        private void OnStartGameButtonHandlerPerformed()
        {
            _mainHudHandler.SwitchVisibility();
            _stateMachine.Enter<GameLoopState>();
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();
        }
    }
}