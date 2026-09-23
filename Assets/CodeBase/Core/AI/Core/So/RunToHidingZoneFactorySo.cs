using _Root._Scripts.Core.AI.Behaviours;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core.So
{
    [CreateAssetMenu(
        fileName = "RunToHidingZoneBehaviourFactory",
        menuName = "AI/Behaviours/Run To Hiding Zone")]
    public class RunToHidingZoneFactorySo : BehaviourFactorySo
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