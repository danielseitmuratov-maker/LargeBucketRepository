using _Root._Scripts.Core.AI.Behaviours;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core.So
{
    [CreateAssetMenu(
        fileName = "RunOnNavMeshBehaviourFactory",
        menuName = "AI/Behaviours/Run On NavMesh")]
    public class RunOnNavMeshBehaviourFactorySo : BehaviourFactorySo
    {
        [Header("Movement")]
        [field: SerializeField] public float WanderRadius { get; private set; }

        [field: SerializeField] public float ReachedDistance { get; private set; }
        [field: SerializeField] public float RepathDelay { get; private set; }
        [field: SerializeField] public float JumpInterval { get; private set; }
        [field: SerializeField] public float JumpHeight { get; private set; }


        public override IStateBehaviour Create()
        {
            return new RunOnNavMeshBehaviour(WanderRadius, ReachedDistance, RepathDelay);
        }
    }
}