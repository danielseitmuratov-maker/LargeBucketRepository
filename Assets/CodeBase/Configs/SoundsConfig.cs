using System.Collections.Generic;
using _Root._Scripts.Core.Character;
using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(menuName = "SO/SoundsConfig", fileName = "SoundsConfig", order = 0)]
    public class SoundsConfig : ScriptableObject , IMovementConfig
    {
        // character
        
        [field: SerializeField] public AudioClip Win { get; private set; }
        [field: SerializeField] public AudioClip OnDamaged { get; private set; }
        [field: SerializeField] public AudioClip Death { get; private set; }
        [field: SerializeField] public AudioClip Swoosh { get; private set; }
        [field: SerializeField] public AudioClip BaseAttack { get; private set; }
        [field: SerializeField] public AudioClip Teleport { get; private set; }
        [field: SerializeField] public List<AudioClip> RunSounds { get; private set; }
        [field: SerializeField] public AudioClip Jump { get; private set; }
        
        // fortune wheel
        [field: SerializeField] public AudioClip FortuneWheelSpinStartedSond { get; private set; }
        [field: SerializeField] public AudioClip FortuneWheelSpinLoopSound { get; private set; }
        [field: SerializeField] public AudioClip FortuneWheelSpinCompletedSound { get; private set; }
        //ui         
        [field: SerializeField] public AudioClip UIButtonClickSound { get; private set; }
        [field: SerializeField] public AudioClip UIButtonEnterSound { get; private set; }

        // timer
        [field: SerializeField] public AudioClip TimerTickSound { get; private set; }
        [field: SerializeField] public AudioClip TimerAlarmingCountDownSound { get; private set; }
        
        // npc
        [field: SerializeField] public AudioClip NpcGetDamageSound { get; private set; }
        [field: SerializeField] public AudioClip NpcDiedSound { get; private set; }
    }
    
}