using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.SpawnPoints
{
    public struct EnemySpawnFormationParams
    {
        public Vector3 Center;
        public int Count;

        // Circle
        public float Radius;

        // Noise
        public float NoiseScale;
        public Vector2 NoiseAreaSize;

        // Hexagon
        public float HexCellRadius;
        public int HexRingCount;
    }
}