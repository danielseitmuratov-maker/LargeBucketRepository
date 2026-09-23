using System;
using _Root._Scripts.Infrastructure.Services.Sfx.Timer;
using _Root._Scripts.Infrastructure.Services.Timers;

namespace _Root._Scripts.Ui.Core.Timers
{
    public class PreGameCycleTimerSfxHandler : IDisposable 
    {
        private IPreGameCycleTimer _timer;
        private readonly ITimerSfxPlayer _sfxPlayer;


        public PreGameCycleTimerSfxHandler(IPreGameCycleTimer timer,ITimerSfxPlayer sfxPlayer)
        {
            _timer = timer;
            _sfxPlayer = sfxPlayer;

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _timer.TimeUpdated += OnTimeUpdated;
        }

        private void UnsubscribeFromEvents()
        {
            _timer.TimeUpdated -= OnTimeUpdated;
        }
        
        private void OnTimeUpdated(float obj)
        {
            _sfxPlayer.PlayTick();
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();
        }
    }
}