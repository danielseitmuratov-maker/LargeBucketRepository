using System.Collections.Generic;
using _Root._Scripts.Core.GameMap.SpecialZones;
using Unity.AI.Navigation;
using UnityEngine;

namespace _Root._Scripts.Core.GameMap
{
    public class GameMapRoot : MonoBehaviour
    {
        [field: SerializeField] public List<HidingZone> HidingZones { get; private set; }
        [field: SerializeField] public List<Transform> TreasuresSpawnPoints { get; private set; }
        [field: SerializeField] public NavMeshSurface MeshSurface { get; private set; }
        
        
        public void DeInitialize()
        {
        }
    }
}