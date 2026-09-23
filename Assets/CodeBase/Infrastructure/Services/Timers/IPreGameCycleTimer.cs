using System;

namespace _Root._Scripts.Infrastructure.Services.Timers
{
    public interface IPreGameCycleTimer
    {
        float GetCurrentTime();
        bool IsRunning { get; }
        
        event Action<float> TimeUpdated;
        event Action OnCompleted;
        void StartCountDown();
        void StopCountDown();
        void ResetTimer(float newTime = -1f);
    }
}