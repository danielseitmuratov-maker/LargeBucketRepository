using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Sfx.Character
{
    public class CharacterSfxPlayer : ICharacterSfxPlayer
    {
        private readonly ISfxPlayer _sfxPlayer;
        private readonly IConfigProvider _configProvider;
        private readonly Transform _characterTransform;

        private SoundsConfig _config;

        private float _randomPitch;
        private float _randomVolume;

        private int _lastRunSoundIndex = -1;

        public CharacterSfxPlayer(
            ISfxPlayer sfxPlayer,
            IConfigProvider configProvider,
            Transform characterTransform)
        {
            _sfxPlayer = sfxPlayer;
            _configProvider = configProvider;
            _characterTransform = characterTransform;

            _config = _configProvider.GetConfig<SoundsConfig>(
                Paths.GlobalValues.SoundsConfigPath);

            if (_config == null)
            {
                Debug.LogError(
                    "[CharacterSfxPlayer] SoundsConfig не найден.");
            }
        }

        public void PlayTeleport() => 
            PlaySound(_config.Teleport);

        public void PlaySwoosh() => 
            PlaySound(_config.Swoosh);

        public void PlayBaseAttack() => 
            PlaySound(_config.BaseAttack);

        public void PlayWin() => 
            PlaySound(_config.Win);

        public void PlayDeath() => 
            PlaySound(_config.Death);

        public void PlayOnDamaged() => 
            PlaySound(_config.OnDamaged);
        
        public void PlayJump() => 
            PlaySound(_config.Jump);

        public void PlayRun()
        {
            if (_config == null ||
                _config.RunSounds == null ||
                _config.RunSounds.Count == 0)
            {
                Debug.LogWarning(
                    "[CharacterSfxPlayer] Список RunSounds пуст.");
                return;
            }

            int soundIndex = GetRandomRunSoundIndex();
            AudioClip selectedClip = _config.RunSounds[soundIndex];

            if (selectedClip == null)
                return;

            SetRandomValues();

            _sfxPlayer.PlaySfx(
                selectedClip,
                _characterTransform.position,
                _randomPitch,
                false,
                _randomVolume);
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning(
                    "[CharacterSfxPlayer] Попытка проиграть пустой AudioClip.");
                return;
            }

            SetRandomValues();

            _sfxPlayer.PlaySfx(
                clip,
                _characterTransform.position,
                _randomPitch,
                false,
                _randomVolume);
        }

        private int GetRandomRunSoundIndex()
        {
            int count = _config.RunSounds.Count;

            if (count == 1)
            {
                _lastRunSoundIndex = 0;
                return 0;
            }

            int randomIndex;

            do
            {
                randomIndex = Random.Range(0, count);
            }
            while (randomIndex == _lastRunSoundIndex);

            _lastRunSoundIndex = randomIndex;
            return randomIndex;
        }

        private void SetRandomValues()
        {
            _randomPitch = Random.Range(0.95f, 1.2f);
            _randomVolume = Random.Range(0.56f, 1f);
        }
    }
}