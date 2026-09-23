using System;
using UnityEngine;

namespace _Root._Scripts.Core.Chunks
{
    public class ChunkRoot : MonoBehaviour
    {
        public event Action<ChunkRoot> ActiveNextChunkHandlerEntered; 
        
        [SerializeField] private ActiveNextChunkHandler _activeNextChunkHandler;

        public void Init()
        {
            _activeNextChunkHandler.Entered += OnActiveNextChunkHandlerEntered;
        }

        private void OnActiveNextChunkHandlerEntered() => 
            ActiveNextChunkHandlerEntered?.Invoke(this);

        private void OnDestroy()
        {
            _activeNextChunkHandler.Entered -= OnActiveNextChunkHandlerEntered;
        }
    }
}