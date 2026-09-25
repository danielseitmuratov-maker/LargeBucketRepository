using System;
using System.Collections;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Timers;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Npc.Providers
{
    public class GlobalNpcContextProvider : IGlobalNpcContextProvider ,IDisposable
    {
        private readonly IGameLoopTimer _gameLoopTimer;
        private readonly IConfigProvider _configProvider;
        private readonly ICoroutineRunnerService _coroutineRunnerService;
        private GlobalNpcContext _globalContext;
        
        private GameLogicConfig _gameLogicConfig;
        private Coroutine _updateRoutine;
        private bool _isContextUpdating;

        public GlobalNpcContextProvider(IGameLoopTimer gameLoopTimer,IConfigProvider configProvider,ICoroutineRunnerService coroutineRunnerService)
        {
            _gameLoopTimer = gameLoopTimer;
            _configProvider = configProvider;
            _coroutineRunnerService = coroutineRunnerService;

            _gameLogicConfig = _configProvider.GetConfig<GameLogicConfig>(Paths.GlobalValues.GameLogicConfigPath);

            _globalContext = new GlobalNpcContext();
            _isContextUpdating = false;
        }

        public GlobalNpcContext GetGlobalContext() =>
            _globalContext;

        public void StartUpdateContext()
        {
            _isContextUpdating = true;
            _updateRoutine = _coroutineRunnerService.StartCoroutine(UpdateContextValuesRoutine());
        }

        public void StopUpdateContext()
        {
            _isContextUpdating = false;
            _coroutineRunnerService.StopCoroutine(_updateRoutine);
        }

        private IEnumerator UpdateContextValuesRoutine()
        {
            while (_isContextUpdating)
            {
                UpdateContextValues();
                yield return new WaitForSeconds(1);
            }
        }

        private void UpdateContextValues()
        {
            bool isPrepareTimeOut = _globalContext.IsPrepareTimeOut = IsGameLoopPrepareTimeOut();
        }

        private bool IsGameLoopPrepareTimeOut()
        {
            float currentGameTime = _gameLoopTimer.GetCurrentTime();
            
            if (currentGameTime >= _gameLogicConfig.GameLoopNpcPrepareTime)
                return true;

            Debug.Log(currentGameTime);
            return false;
        }

        public void Dispose()
        {
            if (_coroutineRunnerService != null) 
                _coroutineRunnerService.StopCoroutine(_updateRoutine);
        }
    }
}