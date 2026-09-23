using System;
using _Root._Scripts.Core.Character;
using UnityEngine;

namespace _Root._Scripts.Core.Chunks.FarmZones
{
    public class FarmEntranceHandler : MonoBehaviour
    {
        public event Action<FarmZoneRoot> OnEntered;

        private FarmZoneRoot _root;

        public void Init(FarmZoneRoot root)
        {
            _root = root;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CharacterRoot root))
            {
                OnEntered?.Invoke(_root);
            }
        }
    }
}