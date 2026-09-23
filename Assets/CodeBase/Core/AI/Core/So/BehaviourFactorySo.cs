using _Root._Scripts.Core.AI.Behaviours;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core.So
{
    public abstract class BehaviourFactorySo : ScriptableObject
    {
        [Header("Common")]
        
        public int Priority => _priority;

        [SerializeField] private int _priority;

        public abstract IStateBehaviour Create();
    }
}