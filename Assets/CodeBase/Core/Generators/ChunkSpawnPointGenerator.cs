using System.Collections.Generic;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using UnityEngine;

namespace _Root._Scripts.Core.Generators
{
    public class ChunkSpawnPointGenerator : ISpawnPointGenerator
    {
        private readonly IConfigProvider _configProvider;

        private List<ChunkConfig> _configs;

        private Vector3 _currentSpawnPoint;

        // Здесь уже храним длину чанка в МИРОВЫХ координатах
        private List<float> _chunkLengths;

        public ChunkSpawnPointGenerator(IConfigProvider configProvider)
        {
            _configProvider = configProvider;

          // GetLengths();

            _currentSpawnPoint = Vector3.zero;
        }

        private void GetLengths()
        {
            _configs = _configProvider.GetConfigs<ChunkConfig>(Paths.ChunkConfigsRootPath);

            _chunkLengths = new List<float>(_configs.Count);

            for (int i = 0; i < _configs.Count; i++)
            {
                var prefab = _configs[i].Prefab;

                float lengthZ = 0f;

                // Пытаемся взять Renderer.bounds.size.z (это уже world space)
                Renderer renderer = prefab.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    lengthZ = renderer.bounds.size.z;
                }
                else
                {
                    // Фоллбек: считаем по mesh.bounds.size * lossyScale
                    var meshFilter = prefab.GetComponentInChildren<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        Vector3 meshSize = meshFilter.sharedMesh.bounds.size;
                        Vector3 worldScale = prefab.transform.lossyScale; // глобальный масштаб[web:435][web:438][web:439]
                        Vector3 worldSize = Vector3.Scale(meshSize, worldScale);
                        lengthZ = worldSize.z;
                    }
                    else
                    {
                        Debug.LogWarning($"[ChunkSpawnPointGenerator] Prefab {prefab.name} has no Renderer or MeshFilter. Using default length 1.");
                        lengthZ = 1f;
                    }
                }

                _chunkLengths.Add(lengthZ);
            }
        }

        public Vector3 GetStartSpawnPoint()
        {
            return _currentSpawnPoint;
        }

        public Vector3 GetNextSpawnPoint()
        {
            if (_chunkLengths == null || _chunkLengths.Count == 0)
            {
                Debug.LogWarning("[ChunkSpawnPointGenerator] No chunk lengths configured. Using default step 1.");
                _currentSpawnPoint += new Vector3(0, 0, 1f);
                return _currentSpawnPoint;
            }

            int index = Random.Range(0, _chunkLengths.Count); // maxExclusive
            float length = _chunkLengths[index];

            Vector3 nextPositionOffset = new Vector3(0, 0, length);

            _currentSpawnPoint += nextPositionOffset;

            return _currentSpawnPoint;
        }

        public void Reset()
        {
            _currentSpawnPoint = Vector3.zero;
        }
    }
}