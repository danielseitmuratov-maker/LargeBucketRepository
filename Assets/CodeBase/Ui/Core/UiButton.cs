using System;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using UnityEngine;

namespace _Root._Scripts.Ui.Core
{
    public abstract class UiButton : MonoBehaviour, IButton
    {
        public event Action Performed;

        private IConfigProvider _configProvider;
        
        private ISfxPlayer _sfxPlayer;
        
        private SoundsConfig _sfxConfig;


        public void Init(IConfigProvider configProvider ,ISfxPlayer sfxPlayer)
        {
            _configProvider = configProvider;
            _sfxPlayer = sfxPlayer;

            _sfxConfig = _configProvider.GetConfig<SoundsConfig>(Paths.GlobalValues.SoundsConfigPath);
        }

        public virtual void PlayClickSound()
        {
            _sfxPlayer.PlaySfx(_sfxConfig.UIButtonClickSound);
        }
    }
}