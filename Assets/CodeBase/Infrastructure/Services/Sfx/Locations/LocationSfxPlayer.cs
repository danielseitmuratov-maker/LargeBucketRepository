using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;

namespace _Root._Scripts.Infrastructure.Services.Sfx.Locations
{
    public class LocationSfxPlayer : ILocationSfxPlayer
    {
        private readonly ISfxPlayer _sfxPlayer;
        private readonly IConfigProvider _configProvider;

        public LocationSfxPlayer(ISfxPlayer sfxPlayer,IConfigProvider configProvider)
        {
            _sfxPlayer = sfxPlayer;
            _configProvider = configProvider;
        }
        
        public void PlayLoaded()
        {
        }

        public void PlayLowCharacterHp()
        {
        }

        public void PlayNewLocationOpened()
        {
        }

        public void PlayUnloaded()
        {
        }
    }
}