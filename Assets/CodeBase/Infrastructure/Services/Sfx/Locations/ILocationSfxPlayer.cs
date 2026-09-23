namespace _Root._Scripts.Infrastructure.Services.Sfx.Locations
{
    public interface ILocationSfxPlayer
    {
        void PlayLoaded();
        void PlayLowCharacterHp();
        void PlayNewLocationOpened();
        void PlayUnloaded();
    }
}