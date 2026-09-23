using System;
using _Root._Scripts.Infrastructure.Services.Saves;
using UnityEngine;
using UnityEngine.Audio;
using YG;

namespace _Root._Scripts.Infrastructure.Services.GlobalSettings
{
    public class GlobalSettingsService : IGlobalSettingsService
    {
        public event Action OnSettingsChanged;

        public float MasterVolume => Data.MasterVolume;
        public float MusicVolume => Data.MusicVolume;
        public float CameraSensitivity => Data.CameraSensitivity;
        public bool SoundEnabled => Data.SoundEnabled;
        public bool MusicEnabled => Data.MusicEnabled;

        private readonly ISaveLoadService _saveLoadService;
        private readonly AudioMixer _audioMixer;

        private const string MasterVolumeParameter = "Master";
        private const string MusicVolumeParameter = "SFX";

        private SavesYG Data => _saveLoadService.Data;

        public GlobalSettingsService(ISaveLoadService saveLoadService, AudioMixer audioMixer)
        {
            _saveLoadService = saveLoadService;
            _audioMixer = audioMixer;
        }

        public void Init()
        {
            ApplyAllSettings();
            Debug.Log($"[GlobalSettingsService] Initialized with Sound: {MasterVolume}, Music: {MusicVolume}, Sensitivity: {CameraSensitivity}");
        }

        public void SetMasterVolume(float value)
        {
            Data.MasterVolume = Mathf.Clamp01(value);
            ApplyMasterVolume();
            _saveLoadService.MarkDirty();
            OnSettingsChanged?.Invoke();
        }

        public void SetMusicVolume(float value)
        {
            Data.MusicVolume = Mathf.Clamp01(value);
            ApplyMusicVolume();
            _saveLoadService.MarkDirty();
            OnSettingsChanged?.Invoke();
        }

        public void SetCameraSensitivity(float value)
        {
            Data.CameraSensitivity = Mathf.Max(0.01f, value);
            _saveLoadService.MarkDirty();
            OnSettingsChanged?.Invoke();
        }

        public void SetSoundEnabled(bool enabled)
        {
            Data.SoundEnabled = enabled;
            ApplyMasterVolume();
            _saveLoadService.MarkDirty();
            OnSettingsChanged?.Invoke();
        }

        public void SetMusicEnabled(bool enabled)
        {
            Data.MusicEnabled = enabled;
            ApplyMusicVolume();
            _saveLoadService.MarkDirty();
            OnSettingsChanged?.Invoke();
        }

        private void ApplyAllSettings()
        {
            ApplyMasterVolume();
            ApplyMusicVolume();
        }

        private void ApplyMasterVolume()
        {
            if (_audioMixer == null)
            {
                Debug.LogWarning("[GlobalSettingsService] AudioMixer is null!");
                return;
            }

            float volume = Data.SoundEnabled ? Data.MasterVolume : 0f;
            float db = ToDb(volume);
            _audioMixer.SetFloat(MasterVolumeParameter, db);
            
            Debug.Log($"[GlobalSettingsService] Master Volume: {Data.MasterVolume} (Enabled: {Data.SoundEnabled}) -> {db:F2} dB");
        }

        private void ApplyMusicVolume()
        {
            if (_audioMixer == null)
            {
                Debug.LogWarning("[GlobalSettingsService] AudioMixer is null!");
                return;
            }

            float volume = Data.MusicEnabled ? Data.MusicVolume : 0f;
            float db = ToDb(volume);
            _audioMixer.SetFloat(MusicVolumeParameter, db);
            
            Debug.Log($"[GlobalSettingsService] Music Volume: {Data.MusicVolume} (Enabled: {Data.MusicEnabled}) -> {db:F2} dB");
        }

        private float ToDb(float value)
        {
            value = Mathf.Clamp(value, 0.0001f, 1f);
            return Mathf.Log10(value) * 20f;
        }
    }
}