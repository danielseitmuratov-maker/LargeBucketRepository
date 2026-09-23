using _Root._Scripts.Core.Character;
using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(menuName = "SO/CharacterConfig", fileName = "CharacterConfig", order = 0)]
    public class CharacterConfig : ScriptableObject , IMovementConfig
    {
        [field: SerializeField] public float RunSoundInterval { get; private set; }
        [field: SerializeField] public CharacterRoot Prefab { get; private set; }
        [field: SerializeField] public float BasicDamage { get; private set; }
        
        // attack data 
        [field: SerializeField] public float AttackRaidus { get; private set; }
        [field: SerializeField] public int MaxAttackCollidersAmount { get; private set; }
        [field: SerializeField] public LayerMask AttackTargetLayerMask { get; private set; }
    }
}
