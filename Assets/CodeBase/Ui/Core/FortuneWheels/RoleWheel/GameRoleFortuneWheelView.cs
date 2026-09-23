using System.Collections;
using System.Collections.Generic;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using UnityEngine;

namespace _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel
{
    public class GameRoleFortuneWheelView : MonoBehaviour
    {
        public Transform SegmentsContainer => _segmentsContainer;

        [SerializeField] private Transform _wheelRoot;
        [SerializeField] private Transform _segmentsContainer;
        [SerializeField] private float _spinSpeed = 500f;
        [SerializeField] private AnimationCurve _slowDownCurve;

        private List<GameRoleFortuneWheelSegment> _segments;

        private IGameRoleFortuneWheel _fortuneWheel;
        private IConfigProvider _configProvider;
        
        private Coroutine _spinCoroutine;
        private bool _isSpinning;

        private int _segmentCount;
        private float _anglePerSegment;
        private GameRoleFortuneWheelConfig _config;


        public void Init(IGameRoleFortuneWheel fortuneWheel,IConfigProvider configProvider)
        {
            _fortuneWheel = fortuneWheel;
            _configProvider = configProvider;

            GetConfigs();
            SubscribeToEvents();
        }

        private void GetConfigs()
        {
            _config = _configProvider.GetConfig<GameRoleFortuneWheelConfig>(Paths.FortuneWheel
                .CharacterGameRoleFortuneWheelConfigPath);
        }

        private void SubscribeToEvents()
        {
            _fortuneWheel.OnSpinStarted += OnSpinStarted;
            _fortuneWheel.OnSpinCompleted += OnSpinCompleted;
        }
        
        private void UnsubscribeFromEvents()
        {
            _fortuneWheel.OnSpinStarted -= OnSpinStarted;
            _fortuneWheel.OnSpinCompleted -= OnSpinCompleted;
        }
        

        public void RebuildSegments(List<GameRoleFortuneWheelSegment> newSegments)
        {
            if (_segmentsContainer == null)
            {
                Debug.LogError("GameRoleFortuneWheelView: _segmentsContainer не назначен!");
                return;
            }

            foreach (Transform child in _segmentsContainer)
                Destroy(child.gameObject);

            _segments = newSegments;
            _segmentCount = _segments.Count;
            _anglePerSegment = 360f / _segmentCount;

            ArrangeSegmentsOnWheel();
            Debug.Log($"Перестроено {_segments.Count} сегментов.");
        }

        private void ArrangeSegmentsOnWheel()
        {
            float radius = 5f;
            for (int i = 0; i < _segments.Count; i++)
            {
                float angle = i * _anglePerSegment * Mathf.Deg2Rad;
                Vector3 localPos = new Vector3(
                    Mathf.Sin(angle) * radius,
                    Mathf.Cos(angle) * radius,
                    0f
                );
                _segments[i].transform.SetParent(_segmentsContainer, false);
                _segments[i].transform.localPosition = localPos;
                _segments[i].transform.localRotation = Quaternion.Euler(0, 0, -i * _anglePerSegment);
            }
        }

        private void OnSpinStarted(GameRoleFortuneWheelSegment targetSegment)
        {
            if (_isSpinning) return;
            _isSpinning = true;

            int targetIndex = GetSegmentIndex(targetSegment);
            if (targetIndex < 0)
            {
                Debug.LogError("Не удалось определить индекс выигравшего сегмента");
                return;
            }

            _spinCoroutine = StartCoroutine(SpinAnimation(targetIndex));
        }

        private void OnSpinCompleted(GameRoleFortuneWheelSegment obj)
        {
            if (_spinCoroutine != null)
            {
                StopCoroutine(_spinCoroutine);
                _spinCoroutine = null;
            }

            _isSpinning = false;
            HighlightSegment(obj);
        }

        private IEnumerator SpinAnimation(int targetIndex)
        {
            float targetAngle = -(targetIndex * _anglePerSegment + _anglePerSegment / 2f) + 90f;
            float extraRotations = _config.extraRotations;
            float totalRotation = targetAngle + extraRotations * 360f;

            float startAngle = _wheelRoot.eulerAngles.z;
            float endAngle = startAngle + totalRotation;
            float duration = _config.spinDuration;
            float elapsed = 0f;

            if (_slowDownCurve != null && _slowDownCurve.keys.Length > 0)
            {
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / duration;
                    float curveValue = _slowDownCurve.Evaluate(t);
                    float currentAngle = Mathf.Lerp(startAngle, endAngle, curveValue);
                    _wheelRoot.rotation = Quaternion.Euler(0f, 0f, currentAngle);
                    yield return null;
                }
            }
            else
            {
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float progress = elapsed / duration;
                    
                    float speedMultiplier = 1f - progress;
                    
                    float currentSpeed = _spinSpeed * speedMultiplier;
                    
                    _wheelRoot.Rotate(0f, 0f, currentSpeed * Time.deltaTime);
                    
                    yield return null;
                }
                float finalAngle = endAngle % 360f;
                
                _wheelRoot.rotation = Quaternion.Euler(0f, 0f, finalAngle);
            }

            _wheelRoot.rotation = Quaternion.Euler(0f, 0f, endAngle);
        }

        private int GetSegmentIndex(GameRoleFortuneWheelSegment segment) =>
            _segments.IndexOf(segment);

        private void HighlightSegment(GameRoleFortuneWheelSegment segment)
        {
            if (segment == null) return;
            segment.Highlight();
        }

        private void OnDestroy()
        {
            if (_fortuneWheel != null)
            {
               UnsubscribeFromEvents();
            }
        }
    }
}