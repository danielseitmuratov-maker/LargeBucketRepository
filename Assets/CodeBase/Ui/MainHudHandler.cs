using _Root._Scripts.Cameras;
using _Root._Scripts.Core.GameMap;
using _Root._Scripts.Infrastructure;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Loaders;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using _Root._Scripts.Ui.Core.Animators;
using DG.Tweening;
using UnityEngine;

namespace _Root._Scripts.Ui
{
    public class MainHudHandler : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private LoadingScreenAnimator _loadingScreenAnimator;
        [SerializeField] private MainMenuCharacterCameraHandler _menuCharacterCameraHandler;

        private IConfigProvider _configProvider;
        private ICoroutineRunnerService _coroutineRunnerService;
        private ILoader<GameMapRoot> _gameMapLoader;

        private bool _isVisible;
        private ISfxPlayer _sfxPlayer;

        public void Init(IConfigProvider configProvider, ICoroutineRunnerService coroutineRunnerService,
            ILoader<GameMapRoot> gameMapLoader, ISfxPlayer sfxPlayer)
        {
            _configProvider = configProvider;
            _coroutineRunnerService = coroutineRunnerService;
            _gameMapLoader = gameMapLoader;
            _sfxPlayer = sfxPlayer;

            InitializeComponents();
            SetUpStartVisibility();
            SubscribeToEvents();
        }

        private void InitializeComponents()
        {
            _loadingScreenAnimator.Init(_coroutineRunnerService, _configProvider, _gameMapLoader);
            _menuCharacterCameraHandler.Init(_configProvider, _sfxPlayer);
        }

        private void SubscribeToEvents()
        {
        }

        private void UnsubscribeFromEvents()
        {
        }


        public void SwitchVisibility(float duration = 0.3f)
        {
            if (_canvasGroup == null)
                return;

            _isVisible = !_isVisible;

            _canvasGroup.DOFade(_isVisible ? 1f : 0f, duration)
                .OnStart(() =>
                {
                    _canvasGroup.interactable = false;
                    _canvasGroup.blocksRaycasts = false;
                })
                .OnComplete(() =>
                {
                    _canvasGroup.interactable = _isVisible;
                    _canvasGroup.blocksRaycasts = _isVisible;
                });
        }

        private void SetUpStartVisibility()
        {
            _isVisible = true;
        }


        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    }
}