using System.Collections.Generic;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.Chunks;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Factories
{
    public class ChunkFactory : IFactory<ChunkRoot>
    {
        private readonly IConfigProvider _configProvider;

        private List<ChunkConfig> _configs;
        private List<ChunkRoot> _prefabs;

        public ChunkFactory(IConfigProvider configProvider)
        {
            _configProvider = configProvider;

            SetUpValues();
        }

        private void SetUpValues()
        {
            _configs = _configProvider.GetConfigs<ChunkConfig>(Paths.ChunkConfigsRootPath);
            _prefabs = new List<ChunkRoot>(_configs.Count);

            for (int i = 0; i < _configs.Count; i++)
                _prefabs.Add(_configs[i].Prefab);
        }

        public ChunkRoot Create(Vector3 at, Transform parent = null)
        {
            if (_prefabs == null || _prefabs.Count == 0)
            {
                Debug.LogError("ChunkFactory.Create: no prefabs configured.");
                return null;
            }

            // По умолчанию берём первый префаб
            ChunkRoot prefab = _prefabs[0];
            ChunkRoot root = Object.Instantiate(prefab, at, prefab.transform.rotation, parent);

            InitRoot(root);
            return root;
        }

        public ChunkRoot CreateById(int id, Vector3 at, Transform parent = null)
        {
            if (_prefabs == null || _prefabs.Count == 0)
            {
                Debug.LogError("ChunkFactory.CreateById: no prefabs configured.");
                return null;
            }

            if (id < 0 || id >= _prefabs.Count)
            {
                Debug.LogError($"ChunkFactory.CreateById: id {id} is out of range [0, {_prefabs.Count - 1}].");
                return null;
            }

            ChunkRoot prefab = _prefabs[id];
            ChunkRoot root = Object.Instantiate(prefab, at, prefab.transform.rotation, parent);

            InitRoot(root);
            return root;
        }

        private void InitRoot(ChunkRoot root)
        {
            root.Init();
        }
    }
}