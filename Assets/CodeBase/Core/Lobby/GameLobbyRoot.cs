using System;
using System.Collections.Generic;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Sfx.Timer;
using _Root._Scripts.Infrastructure.Services.Timers;
using _Root._Scripts.Tools.ShaderTools;
using _Root._Scripts.Ui.Core.Timers;
using Unity.AI.Navigation;
using UnityEngine;

namespace _Root._Scripts.Core.Lobby
{
    public class GameLobbyRoot : MonoBehaviour
    {
        public event Action<GameLobbyRoot> LobbyTimeCompleted;

        [field: SerializeField] public NavMeshSurface MeshSurface { get; private set; }

        [SerializeField] private List<FullCustomShaderApplier> _fullCustomShaderAppliers;

        [SerializeField] private PreGameCycleTimerRoot _preGameCycleTimerRoot;

        private IPreGameCycleTimer _timer;
        private IConfigProvider _configProvider;
        private ITimerSfxPlayer _timerSfxPlayer;

        public void Init(IPreGameCycleTimer timer, IConfigProvider configProvider, ITimerSfxPlayer timerSfxPlayer)
        {
            _timer = timer;
            _configProvider = configProvider;
            _timerSfxPlayer = timerSfxPlayer;

            InitializeComponents();
            SubscribeToEvents();
        }

        private void InitializeComponents()
        {
            if (_fullCustomShaderAppliers.Count > 0)
                for (int i = 0; i < _fullCustomShaderAppliers.Count; i++)
                    _fullCustomShaderAppliers[i].Init();

            _preGameCycleTimerRoot.Init(_timer, _configProvider, _timerSfxPlayer);
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

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    }
}