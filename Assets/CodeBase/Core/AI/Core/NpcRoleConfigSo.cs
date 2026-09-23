using System.Collections.Generic;
using _Root._Scripts.Core.AI.Core.So;
using _Root._Scripts.Core.AI.Npcs;
using _Root._Scripts.Core.Roles;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core
{
    [CreateAssetMenu(menuName = "AI/NPC Role Config")]
    public class NpcRoleConfigSo : ScriptableObject
    {
        [field: SerializeField] public NpcRoot VisualPrefab { get; private set; }
        [field: SerializeField] public GameRole GameRole { get; private set; }

        [field: SerializeField] public Gradient HealthShaderGradient { get; private set; }

        [field: SerializeField] public float MaxHealth { get; private set; }
        [field: SerializeField] public string GameRoleNameRu { get; private set; }
        [field: SerializeField] public string GameRoleNameEng { get; private set; }
        
        [field: SerializeField] public LayerMask LayerMask { get; private set; }

        [field: SerializeField] public List<BehaviourFactorySo> Behaviours { get; private set; }
    }
}