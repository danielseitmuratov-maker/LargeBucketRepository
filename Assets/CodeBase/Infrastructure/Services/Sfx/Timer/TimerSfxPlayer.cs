using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Sfx.Timer
{
    public class TimerSfxPlayer : ITimerSfxPlayer
    {
        private readonly ISfxPlayer _sfxPlayer;
        private readonly IConfigProvider _configProvider;
        
        private SoundsConfig _config;

        public TimerSfxPlayer(ISfxPlayer sfxPlayer,IConfigProvider configProvider)
        {
            _sfxPlayer = sfxPlayer;
            _configProvider = configProvider;

            _config = _configProvider.GetConfig<SoundsConfig>(Paths.GlobalValues.SoundsConfigPath);
        }
        
        public void PlayTick() => 
            _sfxPlayer.PlaySfx(_config.TimerTickSound,Random.Range(0.95f,1.2f));

        public void PlayAlarmingCountDown() => 
            _sfxPlayer.PlaySfx(_config.TimerAlarmingCountDownSound);
    }
}