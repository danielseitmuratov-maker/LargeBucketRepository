using System;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Spawners
{
    public interface ISpawner<T>
    {
        event Action<T> ObjectSpawned;
        event Action<T> ObjectDeSpawned;

        T SpawnSingle(int enemyId, Vector3 at, float delay = 0f);

        void DespawnSingle(T despawnObject);
    }
}