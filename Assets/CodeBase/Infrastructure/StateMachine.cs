using System;
using System.Collections.Generic;
using _Root._Scripts.Core.AI.Npcs;
using _Root._Scripts.Core.Character;
using _Root._Scripts.Core.GameMap;
using _Root._Scripts.Infrastructure;
using _Root._Scripts.Infrastructure.GameStates;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Input;
using _Root._Scripts.Infrastructure.Services.Loaders;
using _Root._Scripts.Infrastructure.Services.Npc.Providers;
using _Root._Scripts.Infrastructure.Services.RandomPoints;
using _Root._Scripts.Infrastructure.Services.Saves;
using _Root._Scripts.Infrastructure.Services.Spawners;
using _Root._Scripts.Infrastructure.Services.Timers;
using _Root._Scripts.Ui;
using _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel;
using CodeBase.Core.AI.Npcs;
using CodeBase.Core.Character;
using CodeBase.Infrastructure.GameStates;
using CodeBase.Infrastructure.Services.Factories;
using CodeBase.Infrastructure.Services.Input;
using UnityEngine.Audio;

namespace CodeBase.Infrastructure
{
    public class StateMachine : IStateMachine
    {
        private Dictionary<Type, IState> _states;
        private IExitableState _currentState;
      //  private Volume _globalVolume;


        private AudioMixerGroup _sfxAudioMixerGroup;

        private MainHudHandler _mainHudHandler;

        private GameRoleFortuneWheelRoot _gameRoleFortuneWheelRoot;
        private StartGameButtonHandler _startGameButtonHandler;

        public StateMachine(MainHudHandler mainHudHandler,
            StartGameButtonHandler startGameButtonHandler, GameRoleFortuneWheelRoot gameRoleFortuneWheelRoot)
        {
            _mainHudHandler = mainHudHandler;
            _startGameButtonHandler = startGameButtonHandler;
            _gameRoleFortuneWheelRoot = gameRoleFortuneWheelRoot;


            _states = new Dictionary<Type, IState>()
            {
                [typeof(BootstrapState)] =
                    new BootstrapState(this),

                [typeof(MainMenuState)] = new MainMenuState(this, _startGameButtonHandler,
                    _mainHudHandler),

                [typeof(TutorialState)] = new TutorialState(this),
                
                [typeof(DevelopmentState)] =
                    new DevelopmentState(this)
            };
        }

        public void Enter<TState>() where TState : class, IState
        {
            TState state = ChangeState<TState>();

            state.Enter();
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _currentState?.Exit();

            TState state = GetState<TState>();
            _currentState = state;

            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState
        {
            return (TState) _states[typeof(TState)];
        }
    }
}