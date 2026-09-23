using System;
using System.Collections.Generic;
using _Root._Scripts.Core.AI;
using _Root._Scripts.Core.AI.Npcs;
using _Root._Scripts.Core.Character;
using _Root._Scripts.Core.GameMap;
using _Root._Scripts.Core.Lobby;
using _Root._Scripts.Infrastructure.GameStates;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Factories;
using _Root._Scripts.Infrastructure.Services.Input;
using _Root._Scripts.Infrastructure.Services.Loaders;
using _Root._Scripts.Infrastructure.Services.Npc.Providers;
using _Root._Scripts.Infrastructure.Services.RandomPoints;
using _Root._Scripts.Infrastructure.Services.Saves;
using _Root._Scripts.Infrastructure.Services.Spawners;
using _Root._Scripts.Infrastructure.Services.Timers;
using _Root._Scripts.Ui;
using _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel;
using UnityEngine.Audio;
using UnityEngine.Rendering;

namespace _Root._Scripts.Infrastructure
{
    public class StateMachine : IStateMachine
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private Dictionary<Type, IState> _states;
        private IExitableState _currentState;
        private Volume _globalVolume;

        private AudioMixer _audioMixer;

        private AudioMixerGroup _sfxAudioMixerGroup;

        private AudioMixerGroup _audioMixerGroup;
        private MainHudHandler _mainHudHandler;

        private GameRoleFortuneWheelRoot _gameRoleFortuneWheelRoot;
        private StartGameButtonHandler _startGameButtonHandler;
        private GameTester _gameTester;

        public StateMachine(ICoroutineRunner coroutineRunner, IJumpButton jumpButton,
            IAttackButton attackButton, IAutoAttackButton autoAttackButton, IAutoRunButton autoRunButton,
            AudioMixer audioMixer,
            AudioMixerGroup audioMixerGroup, MainHudHandler mainHudHandler,
            StartGameButtonHandler startGameButtonHandler, GameRoleFortuneWheelRoot gameRoleFortuneWheelRoot,GameTester gameTester)
        {
            _coroutineRunner = coroutineRunner;
            _audioMixer = audioMixer;
            _audioMixerGroup = audioMixerGroup;
            _mainHudHandler = mainHudHandler;
            _startGameButtonHandler = startGameButtonHandler;
            _gameRoleFortuneWheelRoot = gameRoleFortuneWheelRoot;
            _gameTester = gameTester;


            _states = new Dictionary<Type, IState>()
            {
                [typeof(BootstrapState)] =
                    new BootstrapState(this, _coroutineRunner, jumpButton, attackButton, autoRunButton,
                        autoAttackButton, _audioMixer, _audioMixerGroup, _mainHudHandler, _gameRoleFortuneWheelRoot,_gameTester),

                [typeof(MainMenuState)] = new MainMenuState(this, _startGameButtonHandler,
                    _mainHudHandler),

                [typeof(TutorialState)] = new TutorialState(this),

                [typeof(GameLoopState)] =
                    new GameLoopState(this,
                        G.Get<ISaveLoadService>(),
                        G.Get<IConfigProvider>(),
                        G.Get<IFactory<CharacterRoot>>(),
                        G.Get<ILoader<GameLobbyRoot>>(),
                        G.Get<ILoader<GameMapRoot>>(),
                        G.Get<IRandomNavMeshPointService>(),
                        _gameRoleFortuneWheelRoot,
                        G.Get<INpcSpawner<LobbyNpcRoot>>(),
                        G.Get<INpcSpawnerWithRoles>(),
                        G.Get<IGameLoopTimer>(),
                        G.Get<IGlobalNpcContextProvider>()),

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