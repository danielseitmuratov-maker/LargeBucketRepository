using System;
using System.Collections;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Timers;
using TMPro;
using UnityEngine;

namespace _Root._Scripts.Ui.Core.Timers
{
    public class PreGameCycleTimerView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text _timerValueText;

        [Header("Animation Settings")]
        [SerializeField] private float _punchScale = 1.2f; // множитель увеличения
        [SerializeField] private float _animationDuration = 0.3f;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _warningColor = Color.yellow;
        [SerializeField] private Color _dangerColor = Color.red;
        [SerializeField] private float _warningThreshold = 5f; // при значении ниже этого - желтый
        [SerializeField] private float _dangerThreshold = 3f;  // при значении ниже этого - красный

        private IPreGameCycleTimer _timer;
        private IConfigProvider _configProvider;
        private TimerConfig _config;

        private Vector3 _initialScale;
        private Coroutine _scaleCoroutine;
        private Coroutine _colorCoroutine;

        public void Init(IPreGameCycleTimer timer, IConfigProvider configProvider)
        {
            _timer = timer;
            _configProvider = configProvider;

            SetUpValues();
            SubscribeToEvents();

            // Устанавливаем начальный текст (например, 0 или конфиг)
            if (_timer != null)
                OnTimeUpdated(_timer.GetCurrentTime());
        }
        

        private void SetUpValues()
        {
            _config = _configProvider.GetConfig<TimerConfig>(Paths.Timers.TimerConfigPath);
            _initialScale = _timerValueText.transform.localScale;
        }

        private void SubscribeToEvents()
        {
            _timer.TimeUpdated += OnTimeUpdated;
            _timer.OnCompleted += OnTimerCompleted;
        }

        private void UnsubscribeFromEvents()
        {
            _timer.TimeUpdated -= OnTimeUpdated;
            _timer.OnCompleted -= OnTimerCompleted;
        }

        private void PlayUpdateAnimation(float currentTime)
        {
            if (_scaleCoroutine != null)
                StopCoroutine(_scaleCoroutine);
            _scaleCoroutine = StartCoroutine(ScaleAnimation());

            if (_colorCoroutine != null)
                StopCoroutine(_colorCoroutine);
            Color targetColor = GetColorForTime(currentTime);
            _colorCoroutine = StartCoroutine(ColorAnimation(targetColor));
        }

        private IEnumerator ScaleAnimation()
        {
            Transform textTransform = _timerValueText.transform;
            Vector3 startScale = _initialScale;
            Vector3 targetScale = startScale * _punchScale;
            float halfDuration = _animationDuration * 0.5f;

            float elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                textTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }
            textTransform.localScale = targetScale;

            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                textTransform.localScale = Vector3.Lerp(targetScale, startScale, t);
                yield return null;
            }
            textTransform.localScale = startScale;
        }

        private IEnumerator ColorAnimation(Color targetColor)
        {
            Color startColor = _timerValueText.color;
            float elapsed = 0f;
            while (elapsed < _animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _animationDuration;
                _timerValueText.color = Color.Lerp(startColor, targetColor, t);
                yield return null;
            }
            _timerValueText.color = targetColor;
        }

        private Color GetColorForTime(float time)
        {
            if (time <= _dangerThreshold)
                return _dangerColor;
            
            else if (time <= _warningThreshold)
                return _warningColor;
            else
                return _normalColor;
        }
        
        private void OnTimerCompleted()
        {
            // Можно показать финальный текст или скрыть
            _timerValueText.text = "";
            // дополнительно можно изменить цвет на зеленый или др.
            _timerValueText.color = Color.green;
        }

        private void OnTimeUpdated(float currentTime)
        {
            int seconds = Mathf.CeilToInt(currentTime);
            _timerValueText.text = seconds.ToString();

            PlayUpdateAnimation(currentTime);
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
            
            if (_scaleCoroutine != null)
                StopCoroutine(_scaleCoroutine);
            if (_colorCoroutine != null)
                StopCoroutine(_colorCoroutine);
        }
    }
}