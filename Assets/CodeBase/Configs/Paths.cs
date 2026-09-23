namespace _Root._Scripts.Configs
{
    public class Paths
    {
        public class Camera
        {
            public const string CameraConfigPath = "Configs/Camera/CameraConfig";
        }

        public class FortuneWheel
        {
            public const string CharacterGameRoleFortuneWheelConfigPath = "Configs/FortuneWheel/CharacterGameRoleFortuneWheelConfig";
        }

        public class NpcData
        {
            public const string NpcSpawnerConfigPath = "Configs/Npc/Spawners/NpcSpawnerConfig";
            public const string NpcRoleConfigsRootPath = "Configs/Npc/Roles";
            
            public class Roles
            {
                public const string PeacefulNpcRoleConfigPath = "Configs/Npc/Roles/Peaceful";
                public const string LobbyNpcConfigRolePath = "Configs/Npc/Roles/LobbyNpc";
            }
        }

        public class GlobalValues
        {
            public const string LoadingScreenConfigPath = "Configs/GameLogic/UI/Screen/LoadingScreenConfig";

            public const string GameMapConfigPath = "Configs/GameLogic/Map/GameMapConfig";
            public const string GameLobbyConfigPath = "Configs/GameLogic/Map/GameLobbyConfig";
            public const string MainUiFxConfigPath = "Configs/GameLogic/UI/MainUiFxConfig";
            public const string GameLogicConfigPath = "Configs/GameLogic/GameLogicConfig";
            public const string SoundsConfigPath = "Configs/SFX/MainSoundsConfig";
            public const string ParticleConfigPath = "Configs/VFX/MainParticleEffectConfig";
            public const string DOTweenAnimationsConfigPath = "Configs/Tweens/DoTweenAnimationsConfig";
            public const string CharacterConfigPath = "Configs/Character/CharacterConfig";
            public const string ShaderChangerConfigPath = "Configs/Shaders/ShaderChangerConfig";
        }

        public const string ChunkConfigsRootPath = "Configs/Chunks";
        public const string FarmZoneConfigsRootPath = "Configs/FarmZones";
        public const string EnemyMovementConfigPath = "Configs/Enemies/Movement/BasicEnemyMovementConfig";
        public const string EnemyRootConfigsPath = "Configs/Enemies";

        public const string HostileLocationConfigsRootPath = "Configs/Locations/Hostiles";
        public const string PieceLocationConfigsRootPath = "Configs/Locations/Pieces";
        public const string LocationRulesConfigPath = "Configs/Locations/Rules/LocationRulesConfig";

        public class Timers
        {
            public const string TimerConfigPath = "Configs/Timers/MainTimerConfig";
        }
    }
}