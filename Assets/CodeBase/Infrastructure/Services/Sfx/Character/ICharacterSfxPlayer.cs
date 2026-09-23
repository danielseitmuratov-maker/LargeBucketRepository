namespace _Root._Scripts.Infrastructure.Services.Sfx.Character
{
    public interface ICharacterSfxPlayer
    {
        void PlayTeleport();
        void PlaySwoosh();
        void PlayBaseAttack();
        void PlayWin();
        void PlayDeath();
        void PlayOnDamaged();

        void PlayRun();
        void PlayJump();
    }
}