using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.GlobalSettings;
using _Root._Scripts.Infrastructure.Services.Saves;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YG;
using Random = UnityEngine.Random;

namespace _Root._Scripts.Ui.Core.GlobalSettings
{
    public class GlobalSettingsHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform _currentRoot;

        [Header("UI References")]
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _cameraSensitivitySlider;

        private IGlobalSettingsService _globalSettingsService;
        private ISfxPlayer _sfxPlayer;
        private SoundsConfig _soundsConfig;
        private ISaveLoadService _saveLoadService;

        private SavesYG _data;

        private bool _isInitialized;
        private bool _isOpen;
        private Tweener _currentTween;

        public void Init(
            IGlobalSettingsService globalSettingsService,
            ISfxPlayer sfxPlayer,
            IConfigProvider configProvider,
            ISaveLoadService saveLoadService)
        {
            _globalSettingsService = globalSettingsService;
            _sfxPlayer = sfxPlayer;
            _saveLoadService = saveLoadService;
            _data = _saveLoadService.Data;

            _soundsConfig = configProvider.GetConfig<SoundsConfig>(Paths.GlobalValues.SoundsConfigPath);

            CloseImmediate();

            _globalSettingsService.Init();

            ApplySavedSettingsToService();

            InitializeSliders();

            _isInitialized = true;
        }
        
        private void ApplySavedSettingsToService()
        {
            if (_data == null || _globalSettingsService == null)
                return;

            if (_data.MusicVolume > 0f)
                _globalSettingsService.SetMasterVolume(_data.MusicVolume);

            if (_data.MusicVolume > 0f)
                _globalSettingsService.SetMusicVolume(_data.MusicVolume);

            if (_data.CameraSensitivity > 0f)
                _globalSettingsService.SetCameraSensitivity(_data.CameraSensitivity);
        }

        private void InitializeSliders()
        {
            if (_masterVolumeSlider != null)
            {
                _masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
                _masterVolumeSlider.minValue = 0f;
                _masterVolumeSlider.maxValue = 1f;
                _masterVolumeSlider.value = _globalSettingsService.MasterVolume;
                _masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            }


            if (_cameraSensitivitySlider != null)
            {
                _cameraSensitivitySlider.onValueChanged.RemoveListener(OnCameraSensitivityChanged);
                _cameraSensitivitySlider.minValue = 0f;
                _cameraSensitivitySlider.maxValue = 1f;

                float normalizedValue = Mathf.InverseLerp(0.01f, 1f, _globalSettingsService.CameraSensitivity);
                _cameraSensitivitySlider.value = normalizedValue;

                _cameraSensitivitySlider.onValueChanged.AddListener(OnCameraSensitivityChanged);
            }
        }

        #region Slider Callbacks

        private void OnMasterVolumeChanged(float value)
        {
            if (_globalSettingsService == null || _data == null || _saveLoadService == null)
                return;

            _globalSettingsService.SetMasterVolume(value);

            _data.MasterVolume = value;
            _saveLoadService.Save();
        }

        private void OnMusicVolumeChanged(float value)
        {
            if (_globalSettingsService == null || _data == null || _saveLoadService == null)
                return;

            _globalSettingsService.SetMusicVolume(value);

            _data.MusicVolume = value;
            _saveLoadService.Save();
        }

        private void OnCameraSensitivityChanged(float normalizedValue)
        {
            if (_globalSettingsService == null || _data == null || _saveLoadService == null)
                return;

            float actualValue = Mathf.Lerp(0.01f, 1f, normalizedValue);
            _globalSettingsService.SetCameraSensitivity(actualValue);

            _data.CameraSensitivity = actualValue;
            _saveLoadService.Save();
        }

        #endregion

        #region Open/Close

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isOpen)
                Close();
            else
                Open();

            PlayClickSound();
        }

        private void Open()
        {
            _isOpen = true;
            _rectTransform.gameObject.SetActive(true);
        }

        private void Close()
        {
            _isOpen = false;
            _rectTransform.gameObject.SetActive(false);
        }

        private void CloseImmediate()
        {
            _isOpen = false;
            _rectTransform.gameObject.SetActive(false);
        }

        #endregion

        private void PlayClickSound()
        {
            if (_sfxPlayer == null || _soundsConfig == null || _soundsConfig.UIButtonClickSound == null)
                return;

            float pitch = Random.Range(0.95f, 1.1f);
            _sfxPlayer.PlaySfx(_soundsConfig.UIButtonClickSound, transform.position, pitch);
        }

        private void OnDisable()
        {
            if (!_isInitialized)
                return;

            _currentTween?.Kill();

            if (_masterVolumeSlider != null)
                _masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            
            if (_cameraSensitivitySlider != null)
                _cameraSensitivitySlider.onValueChanged.RemoveListener(OnCameraSensitivityChanged);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _currentRoot.DOScale(1.1f, 0.1f).SetEase(Ease.OutElastic);
            _sfxPlayer.PlaySfx(_soundsConfig.UIButtonEnterSound, Random.Range(0.9f, 1.1f));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale = Vector3.one;
        }
    }
}