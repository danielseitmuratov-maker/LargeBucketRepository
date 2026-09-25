using System;
using System.Collections;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.GameMap;
using _Root._Scripts.Infrastructure;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Loaders;
using CodeBase.Infrastructure;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Root._Scripts.Ui.Core.Animators
{
    public class LoadingScreenAnimator : MonoBehaviour
    {
        public event Action LoadingScreenHidden;
        
        [SerializeField] private Image _loadingScreenImage;
        [SerializeField] private Image _loadingRotateImage;

        private ICoroutineRunnerService _coroutineRunnerService;
        private IConfigProvider _configProvider;
        private ILoader<GameMapRoot> _gameMapLoader;

        private LoadingScreenConfig _config;

        private Transform _rotateTarget;
        private Tween _rotationTween;
        private Tween _fadeTween;

        private bool _isShowing;

        public void Init(ICoroutineRunnerService coroutineRunnerService, IConfigProvider configProvider,
            ILoader<GameMapRoot> gameMapLoader)
        {
            _coroutineRunnerService = coroutineRunnerService;
            _configProvider = configProvider;
            _gameMapLoader = gameMapLoader;

            GetValues();
            SubscribeToEvents();

            SetVisible(false, instant: true);
        }

        private void SubscribeToEvents()
        {
            _gameMapLoader.Started += OnGameMapLoaderStarted;
            _gameMapLoader.Loaded += OnGameMapLoaderLoaded;
        }

        private void UnsubscribeFromEvents()
        {
            _gameMapLoader.Started -= OnGameMapLoaderStarted;
            _gameMapLoader.Loaded -= OnGameMapLoaderLoaded;
        }

        private void GetValues()
        {
            _config = _configProvider.GetConfig<LoadingScreenConfig>(Paths.GlobalValues.LoadingScreenConfigPath);

            _loadingScreenImage.sprite = _config.LoadingScreenSprite;
            _loadingRotateImage.sprite = _config.LoadingRotateSprite;
        }

        private void OnGameMapLoaderStarted()
        {
            _coroutineRunnerService.StartCoroutine(PlayLoadingScreen());
        }

        private IEnumerator PlayLoadingScreen()
        {
            Show();
            StartRotation();
            yield return null;
        }

        private void OnGameMapLoaderLoaded(GameMapRoot mapRoot, int arg2)
        {
            Hide();
            LoadingScreenHidden?.Invoke();
        }

        private void Show()
        {
            if (_isShowing) return;

            _fadeTween?.Kill();
            SetVisible(true, instant: true);
            _isShowing = true;
        }

        private void Hide()
        {
            if (!_isShowing) return;

            _rotationTween?.Kill();
            _rotationTween = null;

            _fadeTween?.Kill();

            float duration = _config.AnimationDuration;

            Sequence fadeSequence = DOTween.Sequence();
            fadeSequence.Join(_loadingScreenImage.DOFade(0f, duration));
            fadeSequence.Join(_loadingRotateImage.DOFade(0f, duration));
            fadeSequence.OnComplete(() =>
            {
                SetVisible(false, instant: true);
                _isShowing = false;
            });

            _fadeTween = fadeSequence;
        }

        private void SetVisible(bool visible, bool instant = false)
        {
            float targetAlpha = visible ? 1f : 0f;
            bool interactable = visible;

            if (instant)
            {
                _loadingScreenImage.color = new Color(1, 1, 1, targetAlpha);
                _loadingRotateImage.color = new Color(1, 1, 1, targetAlpha);
                _loadingScreenImage.raycastTarget = interactable;
                _loadingRotateImage.raycastTarget = interactable;
            }
            else
            {
                _loadingScreenImage.color = new Color(1, 1, 1, targetAlpha);
                _loadingRotateImage.color = new Color(1, 1, 1, targetAlpha);
                _loadingScreenImage.raycastTarget = interactable;
                _loadingRotateImage.raycastTarget = interactable;
            }
        }

        private void StartRotation()
        {
            if (_rotateTarget == null)
                _rotateTarget = _loadingRotateImage.rectTransform;

            _rotationTween?.Kill();

            _rotationTween = _rotateTarget.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();

            _rotationTween?.Kill();
            _fadeTween?.Kill();

            SetVisible(false, instant: true);
            _isShowing = false;
        }
    }
}