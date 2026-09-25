using System;
using System.Collections;
using System.Collections.Generic;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.Chunks;
using _Root._Scripts.Core.Generators;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Pools;
using _Root._Scripts.Infrastructure.Services.WeightedRandom;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Loaders
{
    public class ChunkLoader : IAutoLoader<ChunkRoot>, IDisposable
    {
        public event Action Started;
        public event Action<ChunkRoot, int> Loaded;
        public event Action<ChunkRoot> UnLoaded;
        
        private readonly IPool<ChunkRoot> _pool;
        private readonly ISpawnPointGenerator _spawnPointGenerator;
        private readonly ICoroutineRunnerService _coroutineRunnerService;
        private readonly IWeightedRandomService _weightedRandomService;
        private readonly IConfigProvider _configProvider;

        private Coroutine _autoLoadRoutine;
        private Coroutine _autoUnloadRoutine;
    
        private List<ChunkConfig> _configs;
        private List<float> _spawnWeights;

        private readonly Queue<ChunkRoot> _loadedChunks = new Queue<ChunkRoot>();
        private const int MaxLoadedChunks = 6;

        private bool _hasSpawnedFirstChunk;

        public ChunkLoader(
            IPool<ChunkRoot> pool,
            ISpawnPointGenerator spawnPointGenerator,
            ICoroutineRunnerService coroutineRunnerService,
            IWeightedRandomService weightedRandomService,
            IConfigProvider configProvider)
        {
            _pool = pool;
            _spawnPointGenerator = spawnPointGenerator;
            _coroutineRunnerService = coroutineRunnerService;
            _weightedRandomService = weightedRandomService;
            _configProvider = configProvider;

            SetUpSpawnWeights();
        }

        private void SetUpSpawnWeights()
        {
            _configs = _configProvider.GetConfigs<ChunkConfig>(Paths.ChunkConfigsRootPath);
            _spawnWeights = new List<float>(_configs.Count);

            for (int i = 0; i < _configs.Count; i++)
                _spawnWeights.Add(_configs[i].Weight);
        }

        // Ручной спавн (если нужен)
        

        public ChunkRoot Load(int id)
        {
            Vector3 spawnPoint = _hasSpawnedFirstChunk
                ? _spawnPointGenerator.GetNextSpawnPoint()
                : _spawnPointGenerator.GetStartSpawnPoint();

            _hasSpawnedFirstChunk = true;

            ChunkRoot chunk = _pool.Get(id, spawnPoint);
            RegisterChunk(chunk);
            return chunk;
        }

        public void Unload(ChunkRoot unLoaded)
        {
            if (unLoaded == null)
                return;

            unLoaded.ActiveNextChunkHandlerEntered -= OnActivateNextChunkHandlerEntered;
            _pool.Return(unLoaded);
        }

        public void StartAutoLoad()
        {
            if (_autoLoadRoutine != null)
                _coroutineRunnerService.StopCoroutine(_autoLoadRoutine);

            _autoLoadRoutine = _coroutineRunnerService.StartCoroutine(AutoLoadLoop());
        }

        public void StartAutoUnload()
        {
            if (_autoUnloadRoutine != null)
                _coroutineRunnerService.StopCoroutine(_autoUnloadRoutine);

            _autoUnloadRoutine = _coroutineRunnerService.StartCoroutine(AutoUnloadLoop());
        }

        private IEnumerator AutoLoadLoop()
        {
            yield return new WaitForSeconds(0.15f);

            int initialChunksCount = 3;

            // 1) Первый чанк — строго на стартовой позиции
            if (!_hasSpawnedFirstChunk)
            {
                SpawnRandomChunkAtStartPoint();
            }

            // 2) Остальные стартовые чанки — на next‑позициях
            for (int i = 1; i < initialChunksCount; i++)
            {
                SpawnRandomChunkAtNextPoint();
                yield return null;
            }

            _autoLoadRoutine = null;
        }

        private void SpawnRandomChunkAtStartPoint()
        {
            Vector3 spawnPoint = _spawnPointGenerator.GetStartSpawnPoint();

            int chunkIndex = _weightedRandomService.GetWeightedRandom(0, _spawnWeights.Count - 1, _spawnWeights);
            ChunkRoot chunk = _pool.Get(chunkIndex, spawnPoint);

            _hasSpawnedFirstChunk = true;
            RegisterChunk(chunk);
        }

        private void SpawnRandomChunkAtNextPoint()
        {
            Vector3 spawnPoint = _spawnPointGenerator.GetNextSpawnPoint();

            int chunkIndex = _weightedRandomService.GetWeightedRandom(0, _spawnWeights.Count - 1, _spawnWeights);
            ChunkRoot chunk = _pool.Get(chunkIndex, spawnPoint);

            RegisterChunk(chunk);
        }

        private void RegisterChunk(ChunkRoot chunk)
        {
            if (chunk == null)
                return;

            chunk.ActiveNextChunkHandlerEntered += OnActivateNextChunkHandlerEntered;
            _loadedChunks.Enqueue(chunk);
        }

        private void OnActivateNextChunkHandlerEntered(ChunkRoot currentChunkRoot)
        {
            SpawnRandomChunkAtNextPoint();
            TryAutoUnloadOldest();
        }

        private void TryAutoUnloadOldest()
        {
            while (_loadedChunks.Count > MaxLoadedChunks)
            {
                ChunkRoot oldest = _loadedChunks.Dequeue();
                Unload(oldest);
            }
        }

        private IEnumerator AutoUnloadLoop()
        {
            while (true)
            {
                TryAutoUnloadOldest();
                yield return new WaitForSeconds(1f);
            }
        }

        public void Dispose()
        {
            if (_autoLoadRoutine != null)
            {
                _coroutineRunnerService.StopCoroutine(_autoLoadRoutine);
                _autoLoadRoutine = null;
            }

            if (_autoUnloadRoutine != null)
            {
                _coroutineRunnerService.StopCoroutine(_autoUnloadRoutine);
                _autoUnloadRoutine = null;
            }

            while (_loadedChunks.Count > 0)
            {
                ChunkRoot chunk = _loadedChunks.Dequeue();
                Unload(chunk);
            }
        }
    }
}