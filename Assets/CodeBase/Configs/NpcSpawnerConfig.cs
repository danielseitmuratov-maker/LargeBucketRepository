using UnityEngine;
using UnityEngine.AI;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(
        fileName = "NpcSpawnerConfig",
        menuName = "Configs/NPC/NPC Spawner Config")]
    public class NpcSpawnerConfig : ScriptableObject
    {
        public float SpawnHeight => _spawnHeight;

        public float MaxHeightDifference =>
            Mathf.Max(0.01f, _maxHeightDifference);

        public int SampleAttempts =>
            Mathf.Max(1, _sampleAttempts);

        public int AreaMask => _areaMask;
        
        [Header("Spawn Height")]
        [SerializeField] private float _spawnHeight;

        [Tooltip(
            "Максимальное вертикальное расстояние, " +
            "на котором ищется NavMesh.")]
        [SerializeField] private float _maxHeightDifference = 0.5f;

        [Header("Search")]
        [SerializeField] private int _sampleAttempts = 20;

        [SerializeField] private int _areaMask = NavMesh.AllAreas;
    }
}