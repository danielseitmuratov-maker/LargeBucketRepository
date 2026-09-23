using System.Collections.Generic;
using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(menuName = "SO/GameRoleFortuneWheelConfig", fileName = "GameRoleFortuneWheelConfig", order = 0)]
    public class  GameRoleFortuneWheelConfig : ScriptableObject
    {
        [field: SerializeField] public List<GameRoleFortuneWheelFieldConfig> FieldsConfigs { get; private set; }
        
        
        [Header("Camera")]
        public Vector3 cameraOffset = new Vector3(0, 2, -5);
        public Vector3 winCameraOffset = new Vector3(0, 0.5f, -2);
        public float cameraRotationDuration = 0.5f;
        public float resultShowDuration = 2.5f;

        [Header("Colors")]
        public Color[] roleColors;

        [Header("Spin Animation")]
        public float spinDuration = 4f;
        public float extraRotations = 5f;
        public AnimationCurve spinCurve;
        
        [Header("Result Display")]
        public Vector3 resultBackgroundOffset; // смещение фона относительно выигранного сегмента

        [Header("Segments")]
        public float segmentRadius = 5f;
    }
}