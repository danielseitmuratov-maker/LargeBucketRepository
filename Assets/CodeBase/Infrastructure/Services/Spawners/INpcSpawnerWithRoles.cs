using System;
using System.Collections.Generic;
using _Root._Scripts.Core.AI.Npcs;
using _Root._Scripts.Core.Roles;
using Unity.AI.Navigation;

namespace _Root._Scripts.Infrastructure.Services.Spawners
{
    public interface INpcSpawnerWithRoles
    {
        event Action<NpcRoot> ObjectSpawned;
        event Action<NpcRoot> ObjectDeSpawned;

        List<NpcRoot> Spawn(int amount, GameRole characterRole, NavMeshSurface navMeshSurface);
        List<NpcRoot> Spawn(int amount, NavMeshSurface navMeshSurface); // опционально
        void Despawn(NpcRoot npc);
        void DespawnMultiple(List<NpcRoot> npcs);
    }
}