using _Root._Scripts.Infrastructure.GameStates;
using _Root._Scripts.Infrastructure.Services.Input;
using _Root._Scripts.Ui;
using _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel;
using UnityEngine;
using UnityEngine.Audio;

namespace _Root._Scripts.Infrastructure
{
    public class Bootstrap : MonoBehaviour, ICoroutineRunner
    {
        [Header("Tests")]
        [SerializeField] private GameTester _gameTester;

        [Header(" UI ")]
        [SerializeField] private MainHudHandler _mainHudHandler;
        [SerializeField] private StartGameButtonHandler _startGameButtonHandler;

        [Header("Mobile UI Buttons")] [SerializeField]
        private JumpButtonHandler _jumpButton;

        [SerializeField] private AttackButtonHandler _attackButton;
        [SerializeField] private AutoAttackButtonHandler _autoAttackButton;
        [SerializeField] private AutoRunButtonHandler _autoRunButton;

        [Header("Audio")]
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioMixerGroup _audioMixerGroup;

        [Header("GameRole")] 
        [SerializeField] private GameRoleFortuneWheelRoot _gameRoleFortuneWheelRoot;

        private StateMachine _stateMachine;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            _stateMachine = new StateMachine(this, _jumpButton, _attackButton,
                _autoAttackButton, _autoRunButton, _audioMixer, _audioMixerGroup, _mainHudHandler,
                _startGameButtonHandler,_gameRoleFortuneWheelRoot,_gameTester);

            _stateMachine.Enter<BootstrapState>();
        }
    }
}