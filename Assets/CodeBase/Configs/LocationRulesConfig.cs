using _Root._Scripts.Core.Character;
using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(menuName = "SO/LocationRulesConfig", fileName = "LocationRulesConfig", order = 0)]
    public class LocationRulesConfig : ScriptableObject , IMovementConfig
    {
        [field: SerializeField] public Vector3 LocationsSpawnPoint { get; private set; }
        [field: SerializeField] public float MaxProtectedObjectHealth { get; private set; }
    }
}