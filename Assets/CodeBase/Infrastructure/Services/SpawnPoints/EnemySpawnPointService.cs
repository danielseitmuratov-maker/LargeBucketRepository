using System.Collections.Generic;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.SpawnPoints
{
    public class EnemySpawnPointService : IEnemySpawnPointService
    {
        public List<Vector3> GenerateSpawnPoints(EnemyFormationType formationType, EnemySpawnFormationParams p)
        {
            switch (formationType)
            {
                case EnemyFormationType.Circle:
                    return GenerateCircleFormation(p.Center, p.Radius, p.Count);

                case EnemyFormationType.Noise:
                    return GenerateNoiseFormation(p.Center, p.NoiseAreaSize, p.NoiseScale, p.Count);

                case EnemyFormationType.Hexagon:
                    return GenerateHexagonFormation(p.Center, p.HexCellRadius, p.HexRingCount);

                default:
                    return new List<Vector3>();
            }
        }

        public List<Vector3> GenerateCircleFormation(Vector3 center, float radius, int count)
        {
            var points = new List<Vector3>(count);

            if (count <= 0 || radius <= 0f)
                return points;

            float angleStep = 360f / count;

            for (int i = 0; i < count; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                points.Add(new Vector3(center.x + x, center.y, center.z + z));
            }

            return points;
        }

        public List<Vector3> GenerateNoiseFormation(Vector3 center, Vector2 areaSize, float noiseScale, int count)
        {
            var points = new List<Vector3>(count);

            if (count <= 0)
                return points;

            if (noiseScale <= 0f)
                noiseScale = 1f;

            float halfX = areaSize.x * 0.5f;
            float halfZ = areaSize.y * 0.5f;

            for (int i = 0; i < count; i++)
            {
                float sampleX = i * noiseScale;
                float sampleZ = (i + 1000) * noiseScale;

                float noiseX = Mathf.PerlinNoise(sampleX, 0f) * 2f - 1f;
                float noiseZ = Mathf.PerlinNoise(0f, sampleZ) * 2f - 1f;

                float x = Mathf.Lerp(-halfX, halfX, (noiseX + 1f) * 0.5f);
                float z = Mathf.Lerp(-halfZ, halfZ, (noiseZ + 1f) * 0.5f);

                points.Add(new Vector3(center.x + x, center.y, center.z + z));
            }

            return points;
        }

        public List<Vector3> GenerateHexagonFormation(Vector3 center, float cellRadius, int ringCount)
        {
            var points = new List<Vector3>();

            if (cellRadius <= 0f || ringCount <= 0)
                return points;

            points.Add(center);

            for (int r = 1; r <= ringCount; r++)
            {
                GenerateHexRing(center, cellRadius, r, points);
            }

            return points;
        }

        private void GenerateHexRing(Vector3 center, float cellRadius, int ringIndex, List<Vector3> points)
        {
            float w = cellRadius * 2f;
            float h = Mathf.Sqrt(3f) * cellRadius;

            Vector3[] directions =
            {
                new Vector3(w * 0.75f, 0f,  h * 0.5f),
                new Vector3(0f,         0f,  h),
                new Vector3(-w * 0.75f, 0f,  h * 0.5f),
                new Vector3(-w * 0.75f, 0f, -h * 0.5f),
                new Vector3(0f,         0f, -h),
                new Vector3(w * 0.75f,  0f, -h * 0.5f)
            };

            Vector3 pos = center + directions[4] * ringIndex;

            for (int side = 0; side < 6; side++)
            {
                for (int step = 0; step < ringIndex; step++)
                {
                    pos += directions[side];
                    points.Add(pos);
                }
            }
        }
    }
}