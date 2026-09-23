using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(fileName = "DoTweenAnimationsConfigConfig", menuName = "SO/DoTweenAnimationsConfig", order = 0)]
    public class DoTweenAnimationsConfig : ScriptableObject
    {
        // npcs
        [field: SerializeField] public Vector3 GetDamagePunchScale { get; private set; }
    }
}