namespace YG
{
    public partial class SavesYG
    {
        //cart
        public float CurrentCharacterStartMoveSpeed = 5f;

        public float MurderRoleDropChance = 10;
        public float SheriffRoleDropChance = 15;
        public float DoctorRoleDropChance = 25;
        public float PeacfullRoleDropChance = 50;

        public float MasterVolume = 0.5f;
        public float MusicVolume = 0.5f;
        public float CameraSensitivity = 0.5f;
        public bool SoundEnabled = true;
        public bool MusicEnabled = true;


        public bool IsFirstGameSession = YG2.isFirstGameSession;
        public bool IsFirstGameLoop = true;
    }
}