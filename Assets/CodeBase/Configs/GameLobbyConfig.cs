using _Root._Scripts.Core.Lobby;
using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(fileName = "GameLobbyConfig", menuName = "SO/GameLobbyConfig", order = 0)]
    public class GameLobbyConfig : ScriptableObject
    {
        [field: SerializeField] public GameLobbyRoot Prefab { get; private set; }
    }
}