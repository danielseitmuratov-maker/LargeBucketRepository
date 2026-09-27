using System;
using _Root._Scripts.Infrastructure.Services.Sfx.Timer;
using _Root._Scripts.Infrastructure.Services.Timers;
using _Root._Scripts.Ui.Core.Timers;
using UnityEngine;
using Zenject;

namespace CodeBase.Ui.Core.Timers
{
    public class PreGameCycleTimerRoot : MonoBehaviour ,IInitializable,IDisposable
    {
        public event Action TimerCompleted;
        
        private ITimerSfxPlayer _sfxPlayer;
        private PreGameCycleTimerSfxHandler _sfxHandler;
        private IPreGameCycleTimer _timerModel;


        [Inject]
        public void Construct(IPreGameCycleTimer preGameCycleTimer,ITimerSfxPlayer sfxPlayer)
        {
            _timerModel = preGameCycleTimer;
            _sfxPlayer = sfxPlayer;
        }
        public void Initialize()
        {
            InitializeComponents();
            SubscribeToEvents();
        }
        

        private void InitializeComponents()
        {
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
        
        public void Dispose()
        {
            _sfxHandler?.Dispose();
            UnsubscribeFromEvents();
        }
    }
}