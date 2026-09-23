using System;
using UnityEngine;

namespace _Root._Scripts.Tools.ShaderTools
{
    [Serializable]
    public class ShaderPropertySetting
    {
        public string propertyName;
        public PropertyType type;
        public Color colorValue = Color.white;
        public float floatValue = 1f;
        public Texture textureValue;
        public Vector4 vectorValue = Vector4.zero;
    }
}