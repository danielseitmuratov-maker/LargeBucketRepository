using System;
using _Root._Scripts.Core.Character;
using UnityEngine;

namespace _Root._Scripts.Core.Chunks
{
    [RequireComponent(typeof(BoxCollider))]
    public class ActiveNextChunkHandler : MonoBehaviour
    {
        public event Action Entered;
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CharacterRoot characterRoot))
            {
                Entered?.Invoke();
            }
        }
    }
}