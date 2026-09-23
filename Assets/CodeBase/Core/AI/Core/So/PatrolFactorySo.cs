using _Root._Scripts.Core.AI.Behaviours;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core.So
{
    [CreateAssetMenu(
        fileName = "PatrolFactorySo",
        menuName = "AI/Behaviours/Patrol")]
    public class PatrolFactorySo : BehaviourFactorySo
    {
        [field: SerializeField] public float ReachedDistance { get; private set; }
        [field: SerializeField] public float RepathDelay { get; private set; }


        public override IStateBehaviour Create()
        {
            return new RunToHidingZoneBehaviour(ReachedDistance,
                RepathDelay);
        }
    }
}