using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(fileName = "SO/MainUiFxConfigConfig", menuName = "MainUiFxConfig", order = 0)]
    public class MainUiFxConfig : ScriptableObject
    {
        [field: SerializeField] public ParticleSystem ClickEffect { get; private set; }
        [field: SerializeField] public ParticleSystem FortuneWheelWinEffect { get; private set; }
        
    }
}