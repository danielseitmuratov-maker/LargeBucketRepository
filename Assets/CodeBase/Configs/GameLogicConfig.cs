using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(menuName = "SO/GameLogicConfig", fileName = "GameLogicConfig", order = 0)]
    public class GameLogicConfig : ScriptableObject
    {
        [field: SerializeField] public int LobbyNpcsAmount { get; private set; }
        [field: SerializeField] public int NpcPerRound { get; private set; }
        [field: SerializeField] public float SpinGameRoleDuration { get; private set; }

        [field: SerializeField] public float GameTeleportDelay { get; private set; }


        [field: SerializeField] public Vector3 GameMapSpawnPointPosition { get; private set; }
        [field: SerializeField] public Vector3 GameLobbySpawnPointPosition { get; private set; }
        
        
        /// время которое требуется нпс для того чтобы просто побегать по карте перед тем как начать бежать к точке для пряиток
        [field: SerializeField] public float GameLoopNpcPrepareTime { get; private set; }
    }
}