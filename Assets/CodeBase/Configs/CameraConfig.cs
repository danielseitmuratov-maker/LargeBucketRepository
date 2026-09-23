using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(menuName = "SO/CameraConfig", fileName = "CameraConfig", order = 0)]
    public class CameraConfig : ScriptableObject
    {
        [field: SerializeField] public float ZoomSpeed { get; private set; }
        [field: SerializeField] public float ZoomLerpSpeed { get; private set; }
        [field: SerializeField] public float MinZoomDistance { get; private set; } 
        [field: SerializeField] public float MaxZoomDistance { get; private set; }
        [field: SerializeField] public float FocusZoomDistance { get; private set; }


        [field: SerializeField] public float RotationSpeed { get; private set; }
        [field: SerializeField] public int HorizontalRotationRange { get; private set; }
        [field: SerializeField] public int VerticalRotationRange { get; private set; }
    }



    namespace _Root._Scripts.Configs
    {
    }
}