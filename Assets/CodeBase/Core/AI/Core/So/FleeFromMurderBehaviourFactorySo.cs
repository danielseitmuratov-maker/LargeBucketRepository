using _Root._Scripts.Core.AI.Behaviours;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core.So
{
    [CreateAssetMenu(
        fileName = "FleeFromMurderBehaviourFactory",
        menuName = "AI/Behaviours/Flee From Murderer")]
    public class FleeFromMurderBehaviourFactorySo : BehaviourFactorySo
    {
        [field: SerializeField] public float FleeDistance { get; private set; }
        [field: SerializeField] public float RepathDelay { get; private set; }
        

        public override IStateBehaviour Create()
        {
            return new FleeFromMurderBehaviour(Priority, FleeDistance, RepathDelay);
        }
    }
}