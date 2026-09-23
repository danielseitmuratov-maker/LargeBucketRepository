using System;
using System.Collections.Generic;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.AI.Npcs;
using _Root._Scripts.Core.Roles;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Factories;
using _Root._Scripts.Infrastructure.Services.RandomPoints;
using Unity.AI.Navigation;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Spawners
{
    public class NpcSpawnerWithRoles : INpcSpawnerWithRoles, IDisposable
    {
        private readonly Dictionary<GameRole, IFactory<NpcRoot>> _factories;
        private readonly IRandomNavMeshPointService _randomPointService;
        private readonly IConfigProvider _configProvider;
        private readonly NpcSpawnerConfig _config;

        private NavMeshSurface _meshSurface;
        private readonly List<NpcRoot> _spawnedNpcs = new List<NpcRoot>();

        public event Action<NpcRoot> ObjectSpawned;
        public event Action<NpcRoot> ObjectDeSpawned;

        public NpcSpawnerWithRoles(Dictionary<GameRole, IFactory<NpcRoot>> factories, IRandomNavMeshPointService randomPointService,
            IConfigProvider configProvider)
        {
            _factories = factories;
            _randomPointService = randomPointService;
            _configProvider = configProvider;
            _config = _configProvider.GetConfig<NpcSpawnerConfig>(Paths.NpcData.NpcSpawnerConfigPath);
        }

        public List<NpcRoot> Spawn(int amount, NavMeshSurface navMeshSurface)
        {
            return SpawnInternal(amount, GameRole.Peaceful, navMeshSurface, useRoles: false);
        }

        public List<NpcRoot> Spawn(int amount, GameRole characterRole, NavMeshSurface navMeshSurface)
        {
            return SpawnInternal(amount, characterRole, navMeshSurface, useRoles: true);
        }

        private List<NpcRoot> SpawnInternal(int amount, GameRole characterRole, NavMeshSurface navMeshSurface, bool useRoles)
        {
            var result = new List<NpcRoot>();

            if (amount <= 0 || _randomPointService == null || _config == null)
            {
                Debug.LogError("[NpcSpawnerWithRoles] Некорректные параметры.");
                return result;
            }

            PrepareNavMesh(navMeshSurface);
            if (!_randomPointService.HasValidTriangulation)
            {
                Debug.LogError("[NpcSpawnerWithRoles] NavMesh не построен.");
                return result;
            }

            var rolesToSpawn = new List<GameRole>();

            if (useRoles)
            {
                var nonPeaceful = new List<GameRole> { GameRole.Murder, GameRole.Sheriff, GameRole.Doctor };
                
                if (characterRole != GameRole.Peaceful)
                    rolesToSpawn.Add(characterRole);
                
                foreach (var role in nonPeaceful)
                    if (!rolesToSpawn.Contains(role) && role != characterRole)
                        rolesToSpawn.Add(role);
            }

            if (rolesToSpawn.Count == 0)
                rolesToSpawn.Add(GameRole.Peaceful);

            int totalToSpawn = amount;

            foreach (var role in rolesToSpawn)
            {
                if (role == GameRole.Peaceful) continue; // мирных оставляем на потом
                if (totalToSpawn <= 0) break;

                if (!_factories.TryGetValue(role, out var factory))
                {
                    Debug.LogWarning($"Нет фабрики для роли {role}, пропускаем.");
                    continue;
                }

                if (!_randomPointService.TryGetRandomPointAtHeight(_config.SpawnHeight, out Vector3 spawnPoint))
                {
                    Debug.LogWarning($"[NpcSpawnerWithRoles] Не удалось найти точку для роли {role}.");
                    continue;
                }

                NpcRoot npc = factory.Create(spawnPoint);
                if (npc != null)
                {
                    _spawnedNpcs.Add(npc);
                    result.Add(npc);
                    ObjectSpawned?.Invoke(npc);
                    totalToSpawn--;
                }
            }

            // Оставшиеся – мирные NPC
            if (totalToSpawn > 0 && _factories.TryGetValue(GameRole.Peaceful, out var peacefulFactory))
            {
                for (int i = 0; i < totalToSpawn; i++)
                {
                    if (!_randomPointService.TryGetRandomPointAtHeight(_config.SpawnHeight, out Vector3 spawnPoint))
                    {
                        Debug.LogWarning($"[NpcSpawnerWithRoles] Не удалось найти точку для мирного NPC, попытка {i + 1}/{totalToSpawn}");
                        continue;
                    }

                    NpcRoot npc = peacefulFactory.Create(spawnPoint);
                    if (npc != null)
                    {
                        _spawnedNpcs.Add(npc);
                        result.Add(npc);
                        ObjectSpawned?.Invoke(npc);
                    }
                }
            }

            Debug.Log($"[NpcSpawnerWithRoles] Создано {result.Count}/{amount} NPC.");
            return result;
        }

        public void Despawn(NpcRoot npc)
        {
            if (npc == null) return;
            if (_spawnedNpcs.Remove(npc))
                ObjectDeSpawned?.Invoke(npc);
            UnityEngine.Object.Destroy(npc.gameObject);
        }

        public void DespawnMultiple(List<NpcRoot> npcs)
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
                Debug.LogError("[NpcSpawnerWithRoles] NavMesh поверхность отсутствует.");
                return;
            }

            _randomPointService.RebuildTriangulation();
        }

        public void Dispose()
        {
            // Очистка при необходимости
        }
    }
}