using System;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.Lobby;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Sfx.Timer;
using _Root._Scripts.Infrastructure.Services.Timers;
using Object = UnityEngine.Object;

namespace _Root._Scripts.Infrastructure.Services.Loaders
{
    public class GameLobbyLoader : ILoader<GameLobbyRoot>
    {
        public event Action Started;
        public event Action<GameLobbyRoot, int> Loaded;
        public event Action<GameLobbyRoot> UnLoaded;
        
        private readonly IConfigProvider _configProvider;
        private readonly IPreGameCycleTimer _timer;
        private readonly ITimerSfxPlayer _timerSfxPlayer;
        
        private GameLobbyConfig _config;
        private GameLogicConfig _gameLogicConfig;
        
        private GameLobbyRoot _prefab;


        public GameLobbyLoader(IConfigProvider configProvider,IPreGameCycleTimer timer,ITimerSfxPlayer timerSfxPlayer)
        {
            _configProvider = configProvider;
            _timer = timer;
            _timerSfxPlayer = timerSfxPlayer;

            SetUpValues();
        }
        
        public GameLobbyRoot Load(int id)
        {
            Started?.Invoke();
            
            var spawnPointPosition = _gameLogicConfig.GameLobbySpawnPointPosition;
            var lobbyRoot = Object.Instantiate(_prefab, spawnPointPosition, _prefab.transform.rotation);
            InitRoot(lobbyRoot);
            
            Loaded?.Invoke(lobbyRoot,id);
            return lobbyRoot;
        }

        public void Unload(GameLobbyRoot unLoaded)
        {
            unLoaded.DeInitialize();
            UnLoaded?.Invoke(unLoaded);
            
            Object.Destroy(unLoaded);
        }
        
        private void InitRoot(GameLobbyRoot lobbyRoot)
        {
            lobbyRoot.Init(_timer,_configProvider,_timerSfxPlayer);
        }
        
        private void SetUpValues()
        {
            _config = _configProvider.GetConfig<GameLobbyConfig>(Paths.GlobalValues.GameLobbyConfigPath);

            _gameLogicConfig = _configProvider.GetConfig<GameLogicConfig>(Paths.GlobalValues.GameLogicConfigPath);
            _prefab = _config.Prefab;
        }
    }
}