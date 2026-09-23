 using _Root._Scripts.Core.Chunks;
using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(menuName = "SO/ChunkConfig", fileName = "ChunkConfig", order = 0)]
    public class ChunkConfig : ScriptableObject
    {
        [field: SerializeField] public ChunkRoot Prefab { get; private set; }

        [field: SerializeField] public float Weight { get; private set; }
    }
}

 
