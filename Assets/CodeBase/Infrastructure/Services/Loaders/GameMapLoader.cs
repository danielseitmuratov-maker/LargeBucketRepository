using System;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.GameMap;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using Object = UnityEngine.Object;

namespace _Root._Scripts.Infrastructure.Services.Loaders
{
    public class GameMapLoader : ILoader<GameMapRoot>
    {
        public event Action Started;
        public event Action<GameMapRoot, int> Loaded;
        public event Action<GameMapRoot> UnLoaded;
        
        private readonly IConfigProvider _configProvider;
        
        private GameMapConfig _config;
        private GameMapRoot _prefab;
        private GameLogicConfig _gameLogicConfig;

        public GameMapLoader(IConfigProvider configProvider)
        {
            _configProvider = configProvider;

            SetUpValues();
        }
        
        public GameMapRoot Load(int id)
        {
            Started?.Invoke();
            
            var spawnPointPosition = _gameLogicConfig.GameMapSpawnPointPosition;
            var gameMap = Object.Instantiate(_prefab, spawnPointPosition, _prefab.transform.rotation);
            gameMap.Init();
            
            Loaded?.Invoke(gameMap,id);
            return gameMap;
        }

        public void Unload(GameMapRoot unLoaded)
        {
            unLoaded.DeInitialize();
            UnLoaded?.Invoke(unLoaded);
            
            Object.Destroy(unLoaded);
        }
        
        private void SetUpValues()
        {
            _config = _configProvider.GetConfig<GameMapConfig>(Paths.GlobalValues.GameMapConfigPath);

            _gameLogicConfig = _configProvider.GetConfig<GameLogicConfig>(Paths.GlobalValues.GameLogicConfigPath);
            _prefab = _config.Prefab;
        }
    }
}