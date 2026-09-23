using _Root._Scripts.Core.AI.Behaviours;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core.So
{
    [CreateAssetMenu(
        fileName = "FreezeAfterShootFactorySo",
        menuName = "AI/Behaviours/Freeze After Shoot")]
    public class FreezeAfterShootFactorySo : BehaviourFactorySo
    {
        public override IStateBehaviour Create()
        {
            return new FreezeAfterShootBehaviour();
        }
    }
}