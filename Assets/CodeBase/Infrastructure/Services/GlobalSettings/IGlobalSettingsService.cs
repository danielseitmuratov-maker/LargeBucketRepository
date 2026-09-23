using System;

namespace _Root._Scripts.Infrastructure.Services.GlobalSettings
{
    public interface IGlobalSettingsService
    {
        event Action OnSettingsChanged;

        float MasterVolume { get; }
        float MusicVolume { get; }
        float CameraSensitivity { get; }
        bool SoundEnabled { get; }
        bool MusicEnabled { get; }

        void Init();
        void SetMasterVolume(float value);
        void SetMusicVolume(float value);
        void SetCameraSensitivity(float value);
        void SetSoundEnabled(bool enabled);
        void SetMusicEnabled(bool enabled);
    }
}