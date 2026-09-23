using System;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Pools
{
    public interface IPool<T> where T : new()
    {
        event Action<T> ObjectGotFromPool;

        int ProducedObjectsAmount { get; }

        T Get(int id, Vector3 spawnPosition);
        void Prewarm(int count, int rarity, Vector3 spawnPosition,Transform parent  = null);
        
        void Return(T item);
        void Clear();
    }
}