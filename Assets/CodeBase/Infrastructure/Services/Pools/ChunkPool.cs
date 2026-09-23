using System;
using System.Collections.Generic;
using _Root._Scripts.Core.Chunks;
using _Root._Scripts.Infrastructure.Services.Factories;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Pools
{
    public class ChunkPool : IPool<ChunkRoot>
    {
        public event Action<ChunkRoot> ObjectGotFromPool;
        
        public int ProducedObjectsAmount => _chunks.Count;

        private readonly Stack<ChunkRoot> _chunks = new Stack<ChunkRoot>();
        private readonly IFactory<ChunkRoot> _factory;

        public ChunkPool(IFactory<ChunkRoot> factory)
        {
            _factory = factory;
        }
        

        public ChunkRoot Get(int id, Vector3 spawnPosition)
        {
            ChunkRoot chunk;

            if (_chunks.Count > 0)
            {
                chunk = _chunks.Pop();

                if (chunk == null)
                {
                    chunk = _factory.CreateById(id, spawnPosition);
                }
                else
                {
                    Transform t = chunk.transform;
                    t.position = spawnPosition;
                    t.rotation = Quaternion.identity;
                    t.gameObject.SetActive(true);
                }
            }
            else
            {
                chunk = _factory.CreateById(id, spawnPosition);
            }

            ObjectGotFromPool?.Invoke(chunk);
            return chunk;
        }

        public void Prewarm(int count, int id, Vector3 spawnPosition, Transform parent = null)
        {
            for (int i = 0; i < count; i++)
            {
                ChunkRoot chunk = _factory.CreateById(id, spawnPosition, parent);
                if (chunk == null)
                    continue;

                chunk.gameObject.SetActive(false);
                _chunks.Push(chunk);
            }
        }

        public void Return(ChunkRoot item)
        {
            if (item == null)
                return;

            item.gameObject.SetActive(false);
            _chunks.Push(item);
        }

        public void Clear()
        {
            while (_chunks.Count > 0)
            {
                ChunkRoot chunk = _chunks.Pop();
                if (chunk != null)
                    UnityEngine.Object.Destroy(chunk.gameObject);
            }
        }
    }
}