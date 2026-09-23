using _Root._Scripts.Core.AI.Behaviours;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Core.So
{
    [CreateAssetMenu(
        fileName = "HealFactorySo",
        menuName = "AI/Behaviours/Heal")]
    public class HealFactorySo : BehaviourFactorySo
    {
        [field: SerializeField] public float HealRadius { get; private set; }
        [field: SerializeField] public float HealAmount { get; private set; }
        [field: SerializeField] public float HealCooldown { get; private set; }


        public override IStateBehaviour Create()
        {
            return new HealBehaviour(Priority,HealRadius,HealAmount,HealCooldown);
        }
    }
}