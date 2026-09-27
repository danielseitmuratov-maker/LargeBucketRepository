using System;
using CodeBase.Ui.Core.Timers;
using Unity.AI.Navigation;
using UnityEngine;
using Zenject;

namespace CodeBase.Core.Lobby
{
    public class GameLobbyRoot : MonoBehaviour ,IInitializable,IDisposable
    {
        public event Action<GameLobbyRoot> LobbyTimeCompleted;

        [field: SerializeField] public NavMeshSurface MeshSurface { get; private set; }
        
        [SerializeField] private PreGameCycleTimerRoot _preGameCycleTimerRoot;


        public void Initialize()
        {
            SubscribeToEvents();
        }


        private void SubscribeToEvents()
        {
            _preGameCycleTimerRoot.TimerCompleted += OnTimerCompleted;
        }

        private void UnsubscribeFromEvents()
        {
            _preGameCycleTimerRoot.TimerCompleted -= OnTimerCompleted;
        }

        public void StartCountDownTimer()
        {
            _preGameCycleTimerRoot.StartTimer();
        }

        public void StopCountDownTimer()
        {
            _preGameCycleTimerRoot.StopTimer();
        }

        private void OnTimerCompleted()
        {
            LobbyTimeCompleted?.Invoke(this);
        }
        

        public void DeInitialize()
        {
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();
        }
    }
}