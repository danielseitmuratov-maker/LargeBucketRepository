using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(fileName = "ShaderChangerConfig", menuName = "SO/ShaderChangerConfig", order = 0)]
    public class ShaderChangerConfig : ScriptableObject
    {
        // npc on damage
        [field: SerializeField] public Texture2D NpcOnDamageTexture { get; private set; }
        [field: SerializeField] public float NpcOnDamageBlendCutoff { get; private set; }
        [field: SerializeField] public float NpcOnDamageBlendSmoothness { get; private set; }
        [field: SerializeField] public float NpcOnDamageBlendMode { get; private set; }
        [field: SerializeField] public float NpcOnDamageGlowIntensity { get; private set; }
        [field: SerializeField] public float NpcOnDamageFlashDuration { get; private set; }
        [field: SerializeField] public Color NpcOnDamageColor { get; private set; }
    }
}