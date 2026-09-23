using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using UnityEngine;
using UnityEngine.AI;

namespace _Root._Scripts.Infrastructure.Services.RandomPoints
{
    public class RandomNavMeshPointService :
        IRandomNavMeshPointService
    {
        private readonly IConfigProvider _configProvider;
        private readonly int _areaMask;
        private readonly int _sampleAttempts;
        private readonly float _maxHeightDifference;

        private NavMeshTriangulation _triangulation;
        private float[] _triangleAreas;
        private float _totalArea;
        private NpcSpawnerConfig _config;

        public bool HasValidTriangulation =>
            _totalArea > 0f &&
            _triangleAreas != null &&
            _triangleAreas.Length > 0 &&
            _triangulation.vertices != null &&
            _triangulation.vertices.Length > 0 &&
            _triangulation.indices != null &&
            _triangulation.indices.Length >= 3;


        public RandomNavMeshPointService(IConfigProvider configProvider)
        {
            _configProvider = configProvider;

            _config = _configProvider.GetConfig<NpcSpawnerConfig>(Paths.NpcData.NpcSpawnerConfigPath);
            
            _areaMask = _config.AreaMask;
            _sampleAttempts = Mathf.Max(1, _config.SampleAttempts);
            _maxHeightDifference =
                Mathf.Max(0.01f, _config.MaxHeightDifference);

            RebuildTriangulation();
        }

        public void RebuildTriangulation()
        {
            _triangulation =
                NavMesh.CalculateTriangulation();

            if (_triangulation.vertices == null ||
                _triangulation.indices == null ||
                _triangulation.indices.Length < 3)
            {
                _triangleAreas = null;
                _totalArea = 0f;
                return;
            }

            int triangleCount =
                _triangulation.indices.Length / 3;

            _triangleAreas =
                new float[triangleCount];

            _totalArea = 0f;

            for (int i = 0; i < triangleCount; i++)
            {
                int indexA =
                    _triangulation.indices[i * 3];

                int indexB =
                    _triangulation.indices[i * 3 + 1];

                int indexC =
                    _triangulation.indices[i * 3 + 2];

                Vector3 a =
                    _triangulation.vertices[indexA];

                Vector3 b =
                    _triangulation.vertices[indexB];

                Vector3 c =
                    _triangulation.vertices[indexC];

                float area =
                    Vector3.Cross(b - a, c - a).magnitude * 0.5f;

                _triangleAreas[i] = area;
                _totalArea += area;
            }
        }

        public bool TryGetRandomPoint(
            out Vector3 point)
        {
            point = default;

            if (!HasValidTriangulation)
            {
                Debug.LogWarning(
                    "[RandomNavMeshPointService] " +
                    "NavMesh triangulation пуста.");

                return false;
            }

            int triangleIndex =
                GetRandomTriangleIndex();

            int indexA =
                _triangulation.indices[triangleIndex * 3];

            int indexB =
                _triangulation.indices[triangleIndex * 3 + 1];

            int indexC =
                _triangulation.indices[triangleIndex * 3 + 2];

            Vector3 a =
                _triangulation.vertices[indexA];

            Vector3 b =
                _triangulation.vertices[indexB];

            Vector3 c =
                _triangulation.vertices[indexC];

            point =
                GetRandomPointInsideTriangle(a, b, c);

            return true;
        }

        public bool TryGetRandomPointAtHeight(
            float height,
            out Vector3 point)
        {
            point = default;

            if (!HasValidTriangulation)
                return false;

            for (int i = 0; i < _sampleAttempts; i++)
            {
                if (!TryGetRandomPoint(out Vector3 randomPoint))
                {
                    continue;
                }

                Vector3 sourcePoint = new Vector3(randomPoint.x,height, randomPoint.z);

                if (NavMesh.SamplePosition(sourcePoint, out NavMeshHit hit, _maxHeightDifference, _areaMask))
                {
                    point = hit.position;
                    return true;
                }
            }

            // Временный fallback:
            // если фиксированная высота не подходит,
            // возвращаем настоящую точку с высотой NavMesh.
            Debug.LogWarning(
                "[RandomNavMeshPointService] " +
                "Фиксированная высота не подошла. " +
                "Используется реальная высота NavMesh.");

            return TryGetRandomPoint(out point);
        }

        public bool TryGetRandomPoint(
            Vector3 center,
            float radius,
            out Vector3 point,
            int attempts = 10)
        {
            point = default;

            if (radius <= 0f)
                return false;

            if (_areaMask == 0)
                return false;

            attempts = Mathf.Max(1, attempts);

            for (int i = 0; i < attempts; i++)
            {
                Vector3 randomOffset =
                    Random.insideUnitSphere * radius;

                Vector3 sourcePoint =
                    center + randomOffset;

                if (NavMesh.SamplePosition(sourcePoint, out NavMeshHit hit, radius, _areaMask))
                {
                    point = hit.position;
                    return true;
                }
            }

            return false;
        }

        private int GetRandomTriangleIndex()
        {
            float randomValue =
                Random.Range(0f, _totalArea);

            for (int i = 0; i < _triangleAreas.Length; i++)
            {
                randomValue -= _triangleAreas[i];

                if (randomValue <= 0f)
                    return i;
            }

            return _triangleAreas.Length - 1;
        }

        private Vector3 GetRandomPointInsideTriangle(
            Vector3 a,
            Vector3 b,
            Vector3 c)
        {
            float u = Random.value;
            float v = Random.value;

            if (u + v > 1f)
            {
                u = 1f - u;
                v = 1f - v;
            }

            return a + u * (b - a) + v * (c - a);
        }
    }
}