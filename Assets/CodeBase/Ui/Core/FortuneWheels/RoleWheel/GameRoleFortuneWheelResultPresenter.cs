using System.Collections;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Tools.ShaderTools;
using UnityEngine;

namespace _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel
{
    public class GameRoleFortuneWheelResultPresenter : MonoBehaviour, IGameRoleFortuneWheelResultPresenter
    {
        [SerializeField] private Camera _resultCamera;
        [SerializeField] private Transform _background; // простая стена
        [SerializeField] private Transform _resultElementsContainer;

        private ICoroutineRunnerService _coroutineRunnerService;
        private Coroutine _showCoroutine;
        private GameObject _currentSegmentInstance;
        private IConfigProvider _configProvider;
        private GameRoleFortuneWheelConfig _settings;

        public void Init(ICoroutineRunnerService coroutineRunnerService,IConfigProvider configProvider)
        {
            _coroutineRunnerService = coroutineRunnerService;
            _configProvider = configProvider;
            
            GetConfigs();
            SetUpStartSettings();
        }

        private void GetConfigs()
        {
            _settings = _configProvider.GetConfig<GameRoleFortuneWheelConfig>(Paths.FortuneWheel
                .CharacterGameRoleFortuneWheelConfigPath);
        }

        private void SetUpStartSettings()
        {
            
            
            _resultCamera.enabled = false;
            _resultCamera.gameObject.SetActive(false);
            _background.gameObject.SetActive(false);
        }

        public void ShowResult(GameRoleFortuneWheelSegment winningSegment, Color backgroundColor)
        {
            if (winningSegment == null || winningSegment.gameObject == null)
            {
                Debug.LogWarning("ShowResult: winningSegment is null or destroyed.");
                return;
            }

            if (_showCoroutine != null)
                _coroutineRunnerService.StopCoroutine(_showCoroutine);

            // Клонируем выигрышный сегмент в контейнер результата
            if (_currentSegmentInstance != null)
                Destroy(_currentSegmentInstance);

            _currentSegmentInstance = Instantiate(winningSegment.gameObject, _resultElementsContainer);
            _currentSegmentInstance.transform.localPosition = Vector3.zero;
            _currentSegmentInstance.transform.localRotation = Quaternion.identity;

            // Включаем анимацию Win на сегменте (если есть)
            var segmentComponent = _currentSegmentInstance.GetComponent<GameRoleFortuneWheelSegment>();
            segmentComponent?.PlayWinAnimation();

            // Устанавливаем цвет фона
            _background.gameObject.SetActive(true);
            FullCustomShaderApplier fullCustomShaderApplier = _background.GetComponent<FullCustomShaderApplier>();
            if (fullCustomShaderApplier != null)
                fullCustomShaderApplier.UpdateMainColor(backgroundColor);

            // Включаем камеру результата
            _resultCamera.enabled = true;
            _resultCamera.gameObject.SetActive(true);

            _showCoroutine = _coroutineRunnerService.StartCoroutine(ShowResultRoutine());
        }

        private IEnumerator ShowResultRoutine()
        {
            yield return new WaitForSeconds(_settings.resultShowDuration);
            Hide();
        }

        public void Hide()
        {
            if (_showCoroutine != null)
            {
                _coroutineRunnerService.StopCoroutine(_showCoroutine);
                _showCoroutine = null;
            }

            _resultCamera.enabled = false;
            _resultCamera.gameObject.SetActive(false);
            _background.gameObject.SetActive(false);

            if (_currentSegmentInstance != null)
            {
                Destroy(_currentSegmentInstance);
                _currentSegmentInstance = null;
            }
        }

        private void OnDestroy()
        {
            Hide();
        }
    }
}