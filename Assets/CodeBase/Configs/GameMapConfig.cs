using _Root._Scripts.Core.GameMap;
using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(fileName = "GameMapConfig", menuName = "SO/GameMapConfig", order = 0)]
    public class GameMapConfig : ScriptableObject
    {
        [field: SerializeField] public GameMapRoot Prefab { get; private set; }
    }
    
}