using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(menuName = "SO/EffectConfig", fileName = "EffectConfig", order = 0)]
    public class VfxConfig : ScriptableObject
    {

        // character
        [field: SerializeField] public ParticleSystem RunEffect { get; private set; }

        [field: SerializeField] public ParticleSystem JumpEffect { get; private set; }
        [field: SerializeField] public ParticleSystem CharacterMurderAttackEffect { get; private set; }
        [field: SerializeField] public ParticleSystem CharacterAttackOutlineLoopEffect { get; private set; }

        // fortune wheel 
        [field: SerializeField] public ParticleSystem FortuneWheelSpinStartedParticle { get; private set; }
        [field: SerializeField] public ParticleSystem FortuneWheelSpinCompletedParticle { get; private set; }

        // etc 

        [field: SerializeField] public float FortuneWheelSpinDestroingDelay { get; private set; }

        //ui

        [field: SerializeField] public ParticleSystem UIButtonOnClickParticle { get; private set; }

        // npcs
        [field: SerializeField] public ParticleSystem OnNpcDiedEffect { get; private set; }
        [field: SerializeField] public ParticleSystem OnNpcHealthChangedEffect { get; private set; }

        [field: SerializeField] public float NpcOnDiedDestroyDelay { get; private set; }
        [field: SerializeField] public float NpcOnHealthChangedDestroyDelay { get; private set; }
    }
}