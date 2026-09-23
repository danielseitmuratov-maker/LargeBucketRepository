using System;
using System.Collections.Generic;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.AI.Npcs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Factories;
using _Root._Scripts.Infrastructure.Services.RandomPoints;
using Unity.AI.Navigation;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Spawners
{
    public class LobbyNpcSpawner : INpcSpawner<LobbyNpcRoot>, IDisposable
    {
        private NavMeshSurface _meshSurface;
        private readonly IFactory<LobbyNpcRoot> _factory;
        private readonly IRandomNavMeshPointService _randomPointService;
        private readonly IConfigProvider _configProvider;
        private readonly NpcSpawnerConfig _config;

        private readonly List<LobbyNpcRoot> _spawnedNpcs = new List<LobbyNpcRoot>();

        public event Action<LobbyNpcRoot> ObjectSpawned;
        public event Action<LobbyNpcRoot> ObjectDeSpawned;

        public LobbyNpcSpawner(IFactory<LobbyNpcRoot> factory, IRandomNavMeshPointService randomPointService,
            IConfigProvider configProvider)
        {
            _factory = factory;
            _randomPointService = randomPointService;
            _configProvider = configProvider;
            _config = _configProvider.GetConfig<NpcSpawnerConfig>(Paths.NpcData.NpcSpawnerConfigPath);
        }


        public List<LobbyNpcRoot> Spawn(int amount, NavMeshSurface navMeshSurface)
        {
            var result = new List<LobbyNpcRoot>();

            if (amount <= 0 || _factory == null || _randomPointService == null || _config == null)
            {
                Debug.LogError("[LobbyNpcSpawner] Некорректные параметры или зависимости не инициализированы.");
                return result;
            }

            PrepareNavMesh(navMeshSurface);

            if (!_randomPointService.HasValidTriangulation)
            {
                Debug.LogError("[LobbyNpcSpawner] NavMesh не построен.");
                return result;
            }

            for (int i = 0; i < amount; i++)
            {
                if (!_randomPointService.TryGetRandomPointAtHeight(_config.SpawnHeight, out Vector3 spawnPoint))
                {
                    Debug.LogWarning($"[LobbyNpcSpawner] Не удалось найти точку на NavMesh, попытка {i + 1}/{amount}");
                    continue;
                }

                LobbyNpcRoot npc = _factory.Create(spawnPoint);
                if (npc == null)
                {
                    Debug.LogError("[LobbyNpcSpawner] Factory вернула null.");
                    continue;
                }

                _spawnedNpcs.Add(npc);
                result.Add(npc);
                ObjectSpawned?.Invoke(npc);
            }

            Debug.Log($"[LobbyNpcSpawner] Создано {result.Count}/{amount} NPC на переданной NavMesh поверхности.");
            return result;
        }


        public void Despawn(LobbyNpcRoot npc)
        {
            if (npc == null) return;

            if (_spawnedNpcs.Remove(npc))
                ObjectDeSpawned?.Invoke(npc);

            UnityEngine.Object.Destroy(npc.gameObject);
        }

        public void DespawnMultiple(List<LobbyNpcRoot> npcs)
        {
            if (npcs == null) return;

            foreach (var npc in npcs)
                Despawn(npc);

            npcs.Clear();
        }

        private void PrepareNavMesh(NavMeshSurface meshSurface)
        {
            if (_randomPointService.HasValidTriangulation && _meshSurface == meshSurface)
                return;

            _meshSurface = meshSurface;
            if (_meshSurface == null)
            {
                Debug.LogError("[LobbyNpcSpawner] Невозможно построить NavMesh: поверхность отсутствует.");
                return;
            }

            _randomPointService.RebuildTriangulation();
        }

        public void Dispose()
        {
        }
    }
}