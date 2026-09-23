    using System;
    using System.Collections.Generic;
    using Unity.AI.Navigation;

    namespace _Root._Scripts.Infrastructure.Services.Spawners
    {
        public interface INpcSpawner<T>
        {
            event Action<T> ObjectSpawned;
            event Action<T> ObjectDeSpawned;

            List<T> Spawn(int amount ,NavMeshSurface navMeshSurface);

            void Despawn(T npc);
            void DespawnMultiple(List<T> npcs);
        }
    }