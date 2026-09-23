using _Root._Scripts.Core.AI.Behaviours;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core.So
{
    [CreateAssetMenu(
        fileName = "Chase",
        menuName = "AI/Behaviours/Chase")]
    public class ChaseFactorySo : BehaviourFactorySo
    {
        [field: SerializeField] public float StopDistance { get; private set; }
        [field: SerializeField] public float RepathDelay { get; private set; }
        

        public override IStateBehaviour Create()
        {
            return new ChaseBehaviour(StopDistance, RepathDelay);
        }
    }
}