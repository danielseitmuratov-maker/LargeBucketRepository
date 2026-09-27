using System;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using CodeBase.Core.Lobby;
using Zenject;
using Object = UnityEngine.Object;

namespace CodeBase.Infrastructure.Services.Loaders
{
    public class GameLobbyLoader : IGameLobbyLoader
    {
        public event Action Started;
        public event Action<GameLobbyRoot, int> Loaded;
        public event Action<GameLobbyRoot> UnLoaded;

        private readonly IInstantiator _instantiator;
        private readonly IConfigProvider _configProvider;

        private GameLobbyConfig _config;
        private GameLogicConfig _gameLogicConfig;

        private GameLobbyRoot _prefab;


        public GameLobbyLoader(IInstantiator instantiator, IConfigProvider configProvider)
        {
            _instantiator = instantiator;
            _configProvider = configProvider;

            SetUpValues();
        }

        public GameLobbyRoot Load(int id)
        {
            Started?.Invoke();

            var spawnPointPosition = _gameLogicConfig.GameLobbySpawnPointPosition;
            var lobbyRoot = _instantiator
                .InstantiatePrefabForComponent<GameLobbyRoot>(_prefab, spawnPointPosition,
                _prefab.transform.rotation, null);

            Loaded?.Invoke(lobbyRoot, id);
            return lobbyRoot;
        }

        public void Unload(GameLobbyRoot unLoaded)
        {
            unLoaded.DeInitialize();
            UnLoaded?.Invoke(unLoaded);

            Object.Destroy(unLoaded);
        }


        private void SetUpValues()
        {
            _config = _configProvider.GetConfig<GameLobbyConfig>(Paths.GlobalValues.GameLobbyConfigPath);

            _gameLogicConfig = _configProvider.GetConfig<GameLogicConfig>(Paths.GlobalValues.GameLogicConfigPath);
            _prefab = _config.Prefab;
        }
    }
}