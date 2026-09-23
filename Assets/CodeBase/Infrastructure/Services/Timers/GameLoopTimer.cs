using System;
using System.Collections;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Timers
{
    public class GameLoopTimer : IGameLoopTimer
    {
        public float GetCurrentTime() => _currentTime;
        public bool IsRunning => _isRunning;
        
        private readonly IConfigProvider _configProvider;
        private readonly ICoroutineRunner _coroutineRunner;
        private TimerConfig _config;

        private float _currentTime;
        private float _maxTime;
        private Coroutine _countdownCoroutine;
        private bool _isRunning;

        public event Action<float> TimeUpdated;
        public event Action OnCompleted;

        public GameLoopTimer(IConfigProvider configProvider, ICoroutineRunner coroutineRunner)
        {
            _configProvider = configProvider;
            _coroutineRunner = coroutineRunner;
            _config = _configProvider.GetConfig<TimerConfig>(Paths.Timers.TimerConfigPath);
            _maxTime = _config.StandardGameCycleTime;
        }

        public void StartCountDown()
        {
            if (_isRunning) return;

            _currentTime = 0f;
            _isRunning = true;
            _countdownCoroutine = _coroutineRunner.StartCoroutine(CountupCoroutine());
        }

        private IEnumerator CountupCoroutine()
        {
            while (_currentTime < _maxTime)
            {
                _currentTime += Time.deltaTime;
                if (_currentTime > _maxTime) _currentTime = _maxTime;

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
                _coroutineRunner.StopCoroutine(_countdownCoroutine);

            _isRunning = false;
            _countdownCoroutine = null;
        }

        public void ResetTimer(float newTime = -1f)
        {
            StopCountDown();
            _currentTime = newTime >= 0 ? newTime : 0f; 
        }
    }
}