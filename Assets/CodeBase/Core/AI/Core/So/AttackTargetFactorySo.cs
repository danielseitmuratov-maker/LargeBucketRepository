using _Root._Scripts.Core.AI.Behaviours;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core.So
{
    [CreateAssetMenu(
        fileName = "AttackTargetFactorySo",
        menuName = "AI/Behaviours/Attack Target")]
    public class AttackTargetFactorySo : BehaviourFactorySo
    {
        public override IStateBehaviour Create()
        {
            return new AttackTargetBehaviour();
        }
    }
}
    
