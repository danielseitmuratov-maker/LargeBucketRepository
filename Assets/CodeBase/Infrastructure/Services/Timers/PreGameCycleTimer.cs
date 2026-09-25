using System;
using System.Collections;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Timers
{

    public class PreGameCycleTimer : IPreGameCycleTimer
    {
        public float GetCurrentTime() => _currentTime;
        public bool IsRunning => _isRunning;
        
        private readonly IConfigProvider _configProvider;
        private readonly ICoroutineRunnerService _coroutineRunnerService;
        private TimerConfig _config;

        private float _currentTime;
        private Coroutine _countdownCoroutine;
        private bool _isRunning;

        public event Action<float> TimeUpdated;
        public event Action OnCompleted;

        public PreGameCycleTimer(IConfigProvider configProvider, ICoroutineRunnerService coroutineRunnerService)
        {
            _configProvider = configProvider;
            _coroutineRunnerService = coroutineRunnerService;
            _config = _configProvider.GetConfig<TimerConfig>(Paths.Timers.TimerConfigPath);
        }

        public void StartCountDown()
        {
            if (_isRunning) return;

            _currentTime = _config.StandardPreGameCycleTime;
            _isRunning = true;
            _countdownCoroutine = _coroutineRunnerService.StartCoroutine(CountdownCoroutine());
        }

        private IEnumerator CountdownCoroutine()
        {
            while (_currentTime > 0f)
            {
                _currentTime -= Time.deltaTime;
                if (_currentTime < 0f) _currentTime = 0f;

                TimeUpdated?.Invoke(_currentTime);
                yield return null;
            }

            _isRunning = false;
            _countdownCoroutine = null;
            
            OnCompleted?.Invoke();
        }

        public void StopCountDown()
        {
            if (!_isRunning) 
                return;

            if (_countdownCoroutine != null)
                _coroutineRunnerService.StopCoroutine(_countdownCoroutine);

            _isRunning = false;
            _countdownCoroutine = null;
        }

        public void ResetTimer(float newTime = -1f)
        {
            StopCountDown();
            _currentTime = newTime >= 0 ? newTime : _config.StandardPreGameCycleTime;
        }
    }
}