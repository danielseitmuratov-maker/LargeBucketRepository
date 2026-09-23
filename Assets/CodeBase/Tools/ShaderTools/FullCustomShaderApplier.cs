using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Root._Scripts.Tools.ShaderTools
{
    public class FullCustomShaderApplier : MonoBehaviour
    {
        protected const string BaseMapProperty = "_BaseMap";
        protected const string BumpMapProperty = "_BumpMap";

        [Header("Default Textures")]
        [SerializeField] protected Texture2D _albedo;
        [SerializeField] protected Texture2D _normalMap;

        [Header("Main Color")]
        [SerializeField] protected string _mainColorPropertyName = "_Color";
        [SerializeField] protected Color _mainColor = Color.white;

        [Header("Additional Properties")]
        [SerializeField] protected List<ShaderPropertySetting> _propertySettings = new List<ShaderPropertySetting>();

        protected MaterialPropertyBlock _propertyBlock;
        protected Renderer _renderer;
        protected bool _isInitialized = false;


        public virtual void Init()
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer == null)
            {
                Debug.LogError($"[{GetType().Name}] Renderer не найден на {gameObject.name}", this);
                return;
            }

            _propertyBlock = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(_propertyBlock);

            ApplyAllProperties();
            _isInitialized = true;
        }

        protected virtual void OnEnable()
        {
            if (!_isInitialized && _renderer != null)
                ApplyAllProperties();
        }

        protected virtual void OnValidate()
        {
            if (_renderer == null) _renderer = GetComponent<Renderer>();
            if (_propertyBlock == null) _propertyBlock = new MaterialPropertyBlock();
            if (_renderer == null) return;

            ApplyAllProperties();
        }


        protected virtual void ApplyAllProperties()
        {
            if (_renderer == null || _propertyBlock == null) return;

            if (_albedo != null)
                _propertyBlock.SetTexture(BaseMapProperty, _albedo);
            if (_normalMap != null)
                _propertyBlock.SetTexture(BumpMapProperty, _normalMap);

            _propertyBlock.SetColor(_mainColorPropertyName, _mainColor);

            ApplyPropertySettings();

            _renderer.SetPropertyBlock(_propertyBlock);
        }

        private void ApplyPropertySettings()
        {
            if (_propertySettings == null) return;

            foreach (var setting in _propertySettings)
            {
                if (setting == null || string.IsNullOrWhiteSpace(setting.propertyName)) continue;

                switch (setting.type)
                {
                    case PropertyType.Color:
                        _propertyBlock.SetColor(setting.propertyName, setting.colorValue);
                        break;
                    case PropertyType.Float:
                        _propertyBlock.SetFloat(setting.propertyName, setting.floatValue);
                        break;
                    case PropertyType.Texture:
                        if (setting.textureValue != null)
                            _propertyBlock.SetTexture(setting.propertyName, setting.textureValue);
                        break;
                    case PropertyType.Vector:
                        _propertyBlock.SetVector(setting.propertyName, setting.vectorValue);
                        break;
                }
            }
        }


        public void UpdateAlbedo(Texture2D newAlbedo)
        {
            if (newAlbedo == null) return;
            _albedo = newAlbedo;
            _propertyBlock.SetTexture(BaseMapProperty, _albedo);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        public void UpdateNormalMap(Texture2D newNormalMap)
        {
            if (newNormalMap == null) return;
            _normalMap = newNormalMap;
            _propertyBlock.SetTexture(BumpMapProperty, _normalMap);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        public void UpdateMainColor(Color newColor)
        {
            _mainColor = newColor;
            if (_renderer == null || _propertyBlock == null) return;
            _propertyBlock.SetColor(_mainColorPropertyName, _mainColor);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        public void UpdateColor(string propertyName, Color color) =>
            UpdateProperty(propertyName, block => block.SetColor(propertyName, color));

        public void UpdateFloat(string propertyName, float value) =>
            UpdateProperty(propertyName, block => block.SetFloat(propertyName, value));

        public void UpdateTexture(string propertyName, Texture texture)
        {
            if (texture == null) return;
            UpdateProperty(propertyName, block => block.SetTexture(propertyName, texture));
        }

        public void UpdateVector(string propertyName, Vector4 value) =>
            UpdateProperty(propertyName, block => block.SetVector(propertyName, value));

        public void SetProperty(string propertyName, Color value) => UpdateColor(propertyName, value);
        public void SetProperty(string propertyName, float value) => UpdateFloat(propertyName, value);
        public void SetProperty(string propertyName, Texture value) => UpdateTexture(propertyName, value);
        public void SetProperty(string propertyName, Vector4 value) => UpdateVector(propertyName, value);

        private void UpdateProperty(string propertyName, Action<MaterialPropertyBlock> setAction)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) return;
            if (_renderer == null || _propertyBlock == null) return;

            setAction(_propertyBlock);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        public void RefreshAllProperties() => ApplyAllProperties();
    }
}