using System;
using System.Collections.Generic;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.AI.Npcs;
using _Root._Scripts.Core.Character;
using _Root._Scripts.Core.GameMap;
using _Root._Scripts.Core.Lobby;
using _Root._Scripts.Core.Roles;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Factories;
using _Root._Scripts.Infrastructure.Services.Loaders;
using _Root._Scripts.Infrastructure.Services.Npc.Providers;
using _Root._Scripts.Infrastructure.Services.RandomPoints;
using _Root._Scripts.Infrastructure.Services.Saves;
using _Root._Scripts.Infrastructure.Services.Spawners;
using _Root._Scripts.Infrastructure.Services.Timers;
using _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel;
using Unity.AI.Navigation;
using UnityEngine;
using YG;

namespace _Root._Scripts.Infrastructure.GameStates
{
    public class GameLoopState : IState, IDisposable
    {
        private readonly StateMachine _stateMachine;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IConfigProvider _configProvider;
        private readonly IFactory<CharacterRoot> _characterFactory;
        private readonly GameRoleFortuneWheelRoot _gameRoleFortuneWheelRoot;
        private readonly INpcSpawner<LobbyNpcRoot> _lobbyNpcSpawner;
        private readonly ILoader<GameLobbyRoot> _gameLobbyLoader;
        private readonly ILoader<GameMapRoot> _gameMapLoader;
        private readonly IRandomNavMeshPointService _randomNavMeshPointService;

        private NavMeshSurface _currentNavMeshSurface;
        private SavesYG _data;

        private GameRole _characterRole;
        private GameLogicConfig _gameLogicConfig;
        private GameLobbyRoot _currentLobbyRoot;

        private GameMapRoot _currentGameMapRoot;
        private CharacterRoot _characterRoot;
        private INpcSpawnerWithRoles _gameNpcSpawner;
        private readonly IGameLoopTimer _gameLoopTimer;
        private readonly IGlobalNpcContextProvider _globalNpcContextProvider;
        private List<NpcRoot> _spawnedNpcs;
        private bool _isRoleReceived;
        private bool _isLobbyLoaded;

        public GameLoopState(StateMachine stateMachine, ISaveLoadService saveLoadService,
            IConfigProvider configProvider,
            IFactory<CharacterRoot> characterFactory,
            ILoader<GameLobbyRoot> gameLobbyLoader, ILoader<GameMapRoot> gameMapLoader,
            IRandomNavMeshPointService randomNavMeshPointService, GameRoleFortuneWheelRoot gameRoleFortuneWheelRoot,
            INpcSpawner<LobbyNpcRoot> lobbyNpcSpawner, INpcSpawnerWithRoles gameNpcSpawner,
            IGameLoopTimer gameLoopTimer, IGlobalNpcContextProvider globalNpcContextProvider)
        {
            _saveLoadService = saveLoadService;
            _configProvider = configProvider;
            _characterFactory = characterFactory;
            _gameLobbyLoader = gameLobbyLoader;
            _gameMapLoader = gameMapLoader;
            _randomNavMeshPointService = randomNavMeshPointService;
            _gameRoleFortuneWheelRoot = gameRoleFortuneWheelRoot;
            _lobbyNpcSpawner = lobbyNpcSpawner;
            _gameNpcSpawner = gameNpcSpawner;
            _gameLoopTimer = gameLoopTimer;
            _globalNpcContextProvider = globalNpcContextProvider;
            _stateMachine = stateMachine;

            _data = _saveLoadService.Data;

            GetGameLogicConfig();
            SetUpStartValues();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _gameLobbyLoader.Loaded += OnGameLobbyLoaded;
            _gameMapLoader.Loaded += OnGameMapLoaded;
            _gameRoleFortuneWheelRoot.SpinCompleted += OnGameRoleFortuneWheelSpinCompleted;
            _gameLoopTimer.OnCompleted += OnGameLoopTimerCompleted;
        }

        private void UnsubscribeFromEvents()
        {
            _gameLobbyLoader.Loaded -= OnGameLobbyLoaded;
            _gameMapLoader.Loaded -= OnGameMapLoaded;
            _gameRoleFortuneWheelRoot.SpinCompleted -= OnGameRoleFortuneWheelSpinCompleted;

            if (_currentLobbyRoot != null)
                _currentLobbyRoot.LobbyTimeCompleted -= OnLobbyTimeCompleted;

            _gameLoopTimer.OnCompleted -= OnGameLoopTimerCompleted;
        }

        public void Exit()
        {
        }

        public void Enter()
        {
            _isRoleReceived = false;
            
            if (_currentLobbyRoot == null && _gameLobbyLoader != null)
            {
                _currentLobbyRoot = _gameLobbyLoader.Load(0);
            }

            _gameRoleFortuneWheelRoot.Spin();
        }

        private void OnGameRoleFortuneWheelSpinCompleted(GameRoleFortuneWheelSegment winSegment)
        {
            _characterRole = winSegment.GameRole;
            _isRoleReceived = true;
            
            if (_isLobbyLoaded && _currentLobbyRoot != null) 
                StartLobbyTimer();
        }

        private void OnGameLobbyLoaded(GameLobbyRoot lobbyRoot, int arg2)
        {
            _currentLobbyRoot = lobbyRoot;
            _isLobbyLoaded = true;
            SetUpLobby(lobbyRoot);
            PrepareGameLoop();
            _currentLobbyRoot.LobbyTimeCompleted += OnLobbyTimeCompleted;
            
            if (_isRoleReceived) 
                StartLobbyTimer();
        }

        private void OnLobbyTimeCompleted(GameLobbyRoot obj)
        {
            DespawnLobbyNpcs();
            LoadGameMap();
        }

        private void OnGameMapLoaded(GameMapRoot gameMapRoot, int arg2)
        {
            SetUpGameMap(gameMapRoot);
            RebuildNavMesh();
            TeleportCharacterToMap();
            SpawnGameMapNpcs();
            UnloadLobby();

            _gameLoopTimer.StartCountDown();
            _globalNpcContextProvider.StartUpdateContext();
        }

        private void OnGameLoopTimerCompleted()
        {
            // unload game map npcs
            // unload game map
            // teleport character to win/lose window
            // go to win window state
        }

        private void SetUpStartValues() =>
            _spawnedNpcs = new List<NpcRoot>();

        private void GetGameLogicConfig() =>
            _gameLogicConfig = _configProvider.GetConfig<GameLogicConfig>(Paths.GlobalValues.GameLogicConfigPath);


        private void SetUpGameMap(GameMapRoot gameMapRoot)
        {
            _currentGameMapRoot = gameMapRoot;
            _currentNavMeshSurface = gameMapRoot.MeshSurface;
        }

        private void RebuildNavMesh() =>
            _randomNavMeshPointService.RebuildTriangulation();

        private void LoadGameMap() =>
            _currentGameMapRoot = _gameMapLoader.Load(0);

        private void TeleportCharacterToMap()
        {
            Vector3 randomPoint;

            if (_randomNavMeshPointService.TryGetRandomPoint(out randomPoint))
            {
                float delay = _gameLogicConfig.GameTeleportDelay;
                _characterRoot.TeleportSelfWithDelay(randomPoint, delay);
            }
            else
            {
                Debug.LogWarning("Не удалось найти точку для телепортации.");
            }
        }

        private void SetUpLobby(GameLobbyRoot lobbyRoot)
        {
            _currentLobbyRoot = lobbyRoot;
            _currentNavMeshSurface = lobbyRoot.MeshSurface;
        }

        private void PrepareGameLoop()
        {
            _characterRoot = _characterFactory.Create(Vector3.one);
            int spawnedNpcAmount = _gameLogicConfig.LobbyNpcsAmount;
            _spawnedNpcs.AddRange(_lobbyNpcSpawner.Spawn(spawnedNpcAmount, _currentNavMeshSurface));
        }

        private void StartLobbyTimer() =>
            _currentLobbyRoot.StartCountDownTimer();

        private void UnloadLobby() =>
            _gameLobbyLoader.Unload(_currentLobbyRoot);


        private void DespawnLobbyNpcs()
        {
            if (_spawnedNpcs.Count > 0)
                for (int i = 0; i < _spawnedNpcs.Count; i++)
                    _lobbyNpcSpawner.Despawn(_spawnedNpcs[i] as LobbyNpcRoot);

            _spawnedNpcs.Clear();
        }

        private void SpawnGameMapNpcs()
        {
            int spawnedNpcAmount = _gameLogicConfig.NpcPerRound;
            _spawnedNpcs.AddRange(_gameNpcSpawner.Spawn(spawnedNpcAmount, _characterRole, _currentNavMeshSurface));
        }
        
        public void Dispose()
        {
            UnsubscribeFromEvents();
            _gameLoopTimer?.StopCountDown();
            _globalNpcContextProvider?.StopUpdateContext();
        }
    }
}