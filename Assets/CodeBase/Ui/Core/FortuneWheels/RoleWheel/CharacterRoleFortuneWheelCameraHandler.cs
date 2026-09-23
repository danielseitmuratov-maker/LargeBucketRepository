using _Root._Scripts.Configs;
using DG.Tweening;
using UnityEngine;

namespace _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel
{
    public class CharacterRoleFortuneWheelCameraHandler : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _wheelCenter;
        [SerializeField] private GameObject _winBackground;

        private IGameRoleFortuneWheel _gameRoleFortuneWheel;
        private GameRoleFortuneWheelConfig _settings;
        private Tween _lookAtTween;
        private Tween _moveTween;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;
        private Coroutine _winShowCoroutine;
        private GameRoleFortuneWheelSegment _currentWinSegment;

        public void Init(IGameRoleFortuneWheel gameRoleFortuneWheel, GameRoleFortuneWheelConfig settings)
        {
            _gameRoleFortuneWheel = gameRoleFortuneWheel;
            _settings = settings;
            _initialPosition = _camera.transform.position;
            _initialRotation = _camera.transform.rotation;
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _gameRoleFortuneWheel.OnSpinStarted += OnSpinStarted;
            _gameRoleFortuneWheel.OnSpinCompleted += OnSpinCompleted;
        }

        private void UnSubscribeFromEvents()
        {
            _gameRoleFortuneWheel.OnSpinStarted -= OnSpinStarted;
            _gameRoleFortuneWheel.OnSpinCompleted -= OnSpinCompleted;
        }

        private void OnSpinStarted(GameRoleFortuneWheelSegment segment)
        {
            if (_winShowCoroutine != null)
                StopCoroutine(_winShowCoroutine);
            ResetWinDisplay();

            _camera.enabled = true;
            _camera.gameObject.SetActive(true);

            Vector3 targetPosition = _wheelCenter.position + _settings.cameraOffset;
            _moveTween?.Kill();
            _moveTween = _camera.transform.DOMove(targetPosition, _settings.cameraRotationDuration)
                .SetEase(Ease.InOutQuad);

            Quaternion targetRotation = Quaternion.LookRotation(_wheelCenter.position - targetPosition);
            _lookAtTween?.Kill();
            _lookAtTween = _camera.transform.DORotateQuaternion(targetRotation, _settings.cameraRotationDuration)
                .SetEase(Ease.InOutQuad);
        }

        private void OnSpinCompleted(GameRoleFortuneWheelSegment segment)
        {
            _currentWinSegment = segment;
            if (_currentWinSegment != null)
                _winShowCoroutine = StartCoroutine(ShowWinSequence());
            else
                ResetCamera();
        }

        private System.Collections.IEnumerator ShowWinSequence()
        {
            // 1. Перемещаем камеру к выигрышному сегменту
            Vector3 targetPos = _currentWinSegment.transform.position + _settings.winCameraOffset;
            _moveTween?.Kill();
            _moveTween = _camera.transform.DOMove(targetPos, _settings.cameraRotationDuration)
                .SetEase(Ease.OutQuad);

            Vector3 lookTarget = _currentWinSegment.transform.position;
            Quaternion targetRot = Quaternion.LookRotation(lookTarget - targetPos);
            _lookAtTween?.Kill();
            _lookAtTween = _camera.transform.DORotateQuaternion(targetRot, _settings.cameraRotationDuration)
                .SetEase(Ease.OutQuad);

            // 2. Включаем анимацию выигрыша
            _currentWinSegment.PlayWinAnimation();

            // 3. Включаем фон
            if (_winBackground != null)
                _winBackground.SetActive(true);

            // 4. Ждём
            yield return new WaitForSeconds(_settings.resultShowDuration);

            // 5. Выключаем анимацию и фон
            _currentWinSegment.StopWinAnimation();
            if (_winBackground != null)
                _winBackground.SetActive(false);

            // 6. Возвращаем камеру и отключаем
            ResetCamera();

            _winShowCoroutine = null;
        }

        private void ResetWinDisplay()
        {
            if (_currentWinSegment != null)
            {
                _currentWinSegment.StopWinAnimation();
                _currentWinSegment = null;
            }

            if (_winBackground != null)
                _winBackground.SetActive(false);
        }

        public void ResetCamera()
        {
            _moveTween?.Kill();
            _lookAtTween?.Kill();
            _camera.transform.position = _initialPosition;
            _camera.transform.rotation = _initialRotation;
            _camera.enabled = false;
            _camera.gameObject.SetActive(false);
            ResetWinDisplay();
        }

        private void OnDestroy()
        {
            UnSubscribeFromEvents();
            _moveTween?.Kill();
            _lookAtTween?.Kill();
        }
    }
}