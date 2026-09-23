using System;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Sfx.Timer;
using _Root._Scripts.Infrastructure.Services.Timers;
using UnityEngine;

namespace _Root._Scripts.Ui.Core.Timers
{
    public class PreGameCycleTimerRoot : MonoBehaviour
    {
        public event Action TimerCompleted;
        
        [SerializeField] private PreGameCycleTimerView _view;
        
        private IConfigProvider _configProvider;
        private ITimerSfxPlayer _sfxPlayer;


        private PreGameCycleTimerSfxHandler _sfxHandler;
        private IPreGameCycleTimer _timerModel;
        
        
        public void Init(IPreGameCycleTimer preGameCycleTimer,IConfigProvider configProvider,ITimerSfxPlayer sfxPlayer)
        {
            _timerModel = preGameCycleTimer;
            _configProvider = configProvider;
            _sfxPlayer = sfxPlayer;
            
            InitializeComponents();
            SubscribeToEvents();
        }
        
        private void InitializeComponents()
        {
            _view.Init(_timerModel,_configProvider);

            _sfxHandler = new PreGameCycleTimerSfxHandler(_timerModel,_sfxPlayer);
        }

        private void SubscribeToEvents()
        {
            _timerModel.OnCompleted += OnTimerCompleted;
        }

        private void UnsubscribeFromEvents()
        {
            _timerModel.OnCompleted -= OnTimerCompleted;
        }

        public void StartTimer()
        {
            _timerModel.StartCountDown();
        }

        public void StopTimer()
        {
            _timerModel.StopCountDown();
        }
        
        private void OnTimerCompleted() => 
            TimerCompleted?.Invoke();

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    }
}