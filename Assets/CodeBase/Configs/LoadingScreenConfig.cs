using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(fileName = "LoadingScreenConfig", menuName = "SO/LoadingScreenConfig", order = 0)]
    public class LoadingScreenConfig : ScriptableObject
    {
        [field: SerializeField] public float AnimationDuration { get; private set; }
        [field: SerializeField] public Sprite LoadingRotateSprite { get; private set; }
        [field: SerializeField] public Sprite LoadingScreenSprite { get; private set; } }
}