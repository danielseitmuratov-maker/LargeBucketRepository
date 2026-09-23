using _Root._Scripts.Core.AI.Behaviours;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core.So
{
    [CreateAssetMenu(
        fileName = "AttackTargetFactorySo",
        menuName = "AI/Behaviours/Timed Cycle")]
    public class TimedCycleBehaviourSo : BehaviourFactorySo
    {
        [field: SerializeField] public float WanderRadius { get; private set; }
        [field: SerializeField] public float ReachedDistance { get; private set; }
        [field: SerializeField] public float RepathDelay { get; private set; }

        public override IStateBehaviour Create()
        {
            return new TimedCycleBehaviour(WanderRadius,ReachedDistance, RepathDelay);
        }
    }
}