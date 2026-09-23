using _Root._Scripts.Core.Character;
using _Root._Scripts.Core.Locations.Hostile;
using _Root._Scripts.Infrastructure.Services.SpawnPoints;
using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(menuName = "SO/HostileLocationConfig", fileName = "HostileLocationConfig", order = 0)]
    public class HostileLocationConfig : ScriptableObject , IMovementConfig
    {
        [field: SerializeField] public int LocationId { get; private set; }
        [field: SerializeField] public int EnemyId { get; private set; }
        [field: SerializeField] public int MaxEnemiesOnScene { get; private set; }

        [field: SerializeField] public float SpawnInterval { get; private set; }
        [field: SerializeField] public int SpawnPerWave { get; private set; }

        // spawn
        
        [field: SerializeField] public Vector3 SpawnCenter { get; private set; }
        [field: SerializeField] public float SpawnRadius  { get; private set; }
        [field: SerializeField] public float NoiseScale  { get; private set; }
        [field: SerializeField] public EnemyFormationType FormationType  { get; private set; }
        
    }
}
