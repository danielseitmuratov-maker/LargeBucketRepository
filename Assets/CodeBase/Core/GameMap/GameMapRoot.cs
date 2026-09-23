using System.Collections.Generic;
using _Root._Scripts.Core.GameMap.SpecialZones;
using _Root._Scripts.Tools.ShaderTools;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace _Root._Scripts.Core.GameMap
{
    public class GameMapRoot : MonoBehaviour
    {
        [field: SerializeField] public List<HidingZone> HidingZones { get; private set; }
        [field: SerializeField] public List<Transform> TreasuresSpawnPoints { get; private set; }
        [field: SerializeField] public NavMeshSurface MeshSurface { get; private set; }
        [field: SerializeField] public List<FullCustomShaderApplier> FullCustomShaderAppliers{ get; private set; }


        public void Init()
        {
            for (int i = 0; i < FullCustomShaderAppliers.Count; i++) 
                FullCustomShaderAppliers[i].Init();
        }
        
        public void DeInitialize()
        {
        }
    }
}