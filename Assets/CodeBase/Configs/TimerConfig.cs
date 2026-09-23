using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(fileName = "TimerConfig", menuName = "SO/TimerConfig", order = 0)]
    public class TimerConfig : ScriptableObject
    {
        [field: SerializeField] public float StandardPreGameCycleTime { get; private set; }
        [field: SerializeField] public float StandardGameCycleTime { get; private set; }
    }
}