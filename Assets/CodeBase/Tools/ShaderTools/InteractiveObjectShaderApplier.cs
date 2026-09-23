using UnityEngine;

namespace _Root._Scripts.Tools.ShaderTools
{
    public class InteractiveObjectShaderApplier : FullCustomShaderApplier
    {
      

        private const string DefaultHitColorProperty = "_HitColor";
        private const string DefaultHitGlowProperty = "_HitGlow";
        private const string DefaultHitBlendProperty = "_HitBlend";

        private const string DefaultBlendTextureWhiteProperty = "_BlendingTextureWhite";
        private const string DefaultBlendCutoffWhiteProperty = "_BlendingMaskCutoffWhite";
        private const string DefaultBlendSmoothnessWhiteProperty = "_BlendingMaskSmoothnessWhite";
        private const string DefaultBlendModeProperty = "_TextureBlendingMode";

    
        private const float GlowDisabledValue = 0f;
        private const float BlendingDisabledCutoff = 1f;

      
        private const string HitKeyword = "_HIT_ON";
        private const string TextureBlendingKeyword = "_TEXTURE_BLENDING_ON";
        private const string TextureSourceKeyword = "_TEXTUREBLENDINGSOURCE_TEXTURE";

        // ================= FIELDS =================

        [Header("Feature Toggles")]
        [Tooltip("Разрешить использование Hit-эффекта на этом объекте.")]
        [SerializeField] private bool _enableHit = false;

        [Tooltip("Разрешить использование Texture Blending на этом объекте.")]
        [SerializeField] private bool _enableTextureBlending = false;

        [Header("Editor Simulation")]
        [Tooltip("Симулировать Hit-эффект в редакторе.")]
        [SerializeField] private bool _simulateHit = false;

        [Tooltip("Симулировать Texture Blending в редакторе.")]
        [SerializeField] private bool _simulateTextureBlending = false;

        [SerializeField] private Color _simulateHitColor = new Color(1f, 0.3f, 0.2f, 1f);
        [SerializeField] private float _simulateHitIntensity = 1f;
        [SerializeField] private Texture2D _simulateBlendTexture;
        [Range(0f, 1f)] [SerializeField] private float _simulateBlendCutoff = 0.1f;
        [Range(0f, 1f)] [SerializeField] private float _simulateBlendSmoothness = 0.4f;
        [Range(0f, 1f)] [SerializeField] private float _simulateBlendMode = 1f;

        [Header("Glow Settings")]
        [SerializeField] private string _hitColorProperty = DefaultHitColorProperty;
        [SerializeField] private string _hitGlowProperty = DefaultHitGlowProperty;
        [SerializeField] private string _hitBlendProperty = DefaultHitBlendProperty;
        [SerializeField] private float _defaultGlowIntensity = 1f;
        [SerializeField] private float _defaultHitBlend = 0.75f;

        [Header("Texture Blending Settings")]
        [SerializeField] private string _blendTextureWhiteProperty = DefaultBlendTextureWhiteProperty;
        [SerializeField] private string _blendCutoffWhiteProperty = DefaultBlendCutoffWhiteProperty;
        [SerializeField] private string _blendSmoothnessWhiteProperty = DefaultBlendSmoothnessWhiteProperty;
        [SerializeField] private string _blendModeProperty = DefaultBlendModeProperty;

        public bool IsHitEnabled => _enableHit;
        public bool IsTextureBlendingEnabled => _enableTextureBlending;

        // ================= LIFECYCLE =================

        public override void Init()
        {
            base.Init();
            if (!_isInitialized) return;

            ValidatePropertyNames();

            // Ключевые слова включаются ОДИН РАЗ в зависимости от разрешённых фич.
            // Это не ломает батчинг: материал остаётся общим.
            SetKeyword(HitKeyword, _enableHit);
            SetKeyword(TextureBlendingKeyword, _enableTextureBlending);
            SetKeyword(TextureSourceKeyword, _enableTextureBlending);

            // Значения по умолчанию — эффекты визуально выключены
            _propertyBlock.SetFloat(GetHitGlowProperty(), GlowDisabledValue);
            _propertyBlock.SetFloat(GetBlendCutoffProperty(), BlendingDisabledCutoff);
            _renderer.SetPropertyBlock(_propertyBlock);

            Debug.Log($"[{GetType().Name}] Init: enableHit={_enableHit}, enableTextureBlending={_enableTextureBlending}, keywords set.");
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (_renderer == null) return;

            ValidatePropertyNames();

            // В редакторе ключи включаются, если включены либо фича, либо симуляция.
            bool hitOn = _enableHit || _simulateHit;
            bool blendOn = _enableTextureBlending || _simulateTextureBlending;

            SetKeyword(HitKeyword, hitOn);
            SetKeyword(TextureBlendingKeyword, blendOn);
            SetKeyword(TextureSourceKeyword, blendOn);

            ApplyEditorSimulation();
        }

        // ================= KEYWORDS =================

        private void SetKeyword(string keyword, bool enabled)
        {
            if (_renderer == null) return;

            // Используем sharedMaterial, чтобы не создавать инстанс.
            // Если у объекта свой материал-инстанс — будет изменён только он.
            Material mat = _renderer.sharedMaterial;
            if (mat == null) return;

            if (enabled) mat.EnableKeyword(keyword);
            else mat.DisableKeyword(keyword);
        }

        // ================= EDITOR SIMULATION =================

        private void ApplyEditorSimulation()
        {
            if (_renderer == null || _propertyBlock == null) return;

            ApplySimulatedHit();
            ApplySimulatedBlending();

            _renderer.SetPropertyBlock(_propertyBlock);
        }

        private void ApplySimulatedHit()
        {
            if (_simulateHit)
            {
                _propertyBlock.SetColor(GetHitColorProperty(), _simulateHitColor);
                _propertyBlock.SetFloat(GetHitGlowProperty(), _simulateHitIntensity);
                _propertyBlock.SetFloat(GetHitBlendProperty(), _defaultHitBlend);
            }
            else
            {
                _propertyBlock.SetFloat(GetHitGlowProperty(), GlowDisabledValue);
            }
        }

        private void ApplySimulatedBlending()
        {
            if (_simulateTextureBlending && _simulateBlendTexture != null)
            {
                _propertyBlock.SetTexture(GetBlendTextureProperty(), _simulateBlendTexture);
                _propertyBlock.SetFloat(GetBlendCutoffProperty(), _simulateBlendCutoff);
                _propertyBlock.SetFloat(GetBlendSmoothnessProperty(), _simulateBlendSmoothness);
                _propertyBlock.SetFloat(GetBlendModeProperty(), _simulateBlendMode);
            }
            else
            {
                _propertyBlock.SetFloat(GetBlendCutoffProperty(), BlendingDisabledCutoff);
            }
        }

        // ================= VALIDATION =================

        private void ValidatePropertyNames()
        {
            if (_enableHit)
            {
                _hitColorProperty = EnsureName(_hitColorProperty, DefaultHitColorProperty, "HitColor");
                _hitGlowProperty = EnsureName(_hitGlowProperty, DefaultHitGlowProperty, "HitGlow");
                _hitBlendProperty = EnsureName(_hitBlendProperty, DefaultHitBlendProperty, "HitBlend");
            }

            if (_enableTextureBlending)
            {
                _blendTextureWhiteProperty = EnsureName(_blendTextureWhiteProperty, DefaultBlendTextureWhiteProperty, "BlendTexture");
                _blendCutoffWhiteProperty = EnsureName(_blendCutoffWhiteProperty, DefaultBlendCutoffWhiteProperty, "BlendCutoff");
                _blendSmoothnessWhiteProperty = EnsureName(_blendSmoothnessWhiteProperty, DefaultBlendSmoothnessWhiteProperty, "BlendSmoothness");
                _blendModeProperty = EnsureName(_blendModeProperty, DefaultBlendModeProperty, "BlendMode");
            }
        }

        private string EnsureName(string value, string fallback, string label)
        {
            if (!string.IsNullOrWhiteSpace(value)) return value;
            Debug.LogWarning($"[{GetType().Name}] Поле '{label}' пустое на {gameObject.name}. Используется {fallback}");
            return fallback;
        }

        private string GetHitColorProperty() => string.IsNullOrWhiteSpace(_hitColorProperty) ? DefaultHitColorProperty : _hitColorProperty;
        private string GetHitGlowProperty() => string.IsNullOrWhiteSpace(_hitGlowProperty) ? DefaultHitGlowProperty : _hitGlowProperty;
        private string GetHitBlendProperty() => string.IsNullOrWhiteSpace(_hitBlendProperty) ? DefaultHitBlendProperty : _hitBlendProperty;

        private string GetBlendTextureProperty() => string.IsNullOrWhiteSpace(_blendTextureWhiteProperty) ? DefaultBlendTextureWhiteProperty : _blendTextureWhiteProperty;
        private string GetBlendCutoffProperty() => string.IsNullOrWhiteSpace(_blendCutoffWhiteProperty) ? DefaultBlendCutoffWhiteProperty : _blendCutoffWhiteProperty;
        private string GetBlendSmoothnessProperty() => string.IsNullOrWhiteSpace(_blendSmoothnessWhiteProperty) ? DefaultBlendSmoothnessWhiteProperty : _blendSmoothnessWhiteProperty;
        private string GetBlendModeProperty() => string.IsNullOrWhiteSpace(_blendModeProperty) ? DefaultBlendModeProperty : _blendModeProperty;

        // ================= GLOW (Hit) =================

        public void EnableGlow(Color glowColor, float intensity = -1f)
        {
            if (!_enableHit)
            {
                Debug.LogWarning($"[{GetType().Name}] Hit отключён (enableHit = false) на {gameObject.name}");
                return;
            }

            if (_renderer == null || _propertyBlock == null) return;

            if (intensity <= 0f)
                intensity = _defaultGlowIntensity;

            // Ключ уже включён в Init (потому что _enableHit = true). Управляем значениями.
            _propertyBlock.SetColor(GetHitColorProperty(), glowColor);
            _propertyBlock.SetFloat(GetHitGlowProperty(), intensity);
            _propertyBlock.SetFloat(GetHitBlendProperty(), _defaultHitBlend);
            _renderer.SetPropertyBlock(_propertyBlock);

            Debug.Log($"[{GetType().Name}] EnableGlow on {gameObject.name}: color={glowColor}, intensity={intensity}");
        }

        public void DisableGlow()
        {
            if (!_enableHit) return;
            if (_renderer == null || _propertyBlock == null) return;

            _propertyBlock.SetFloat(GetHitGlowProperty(), GlowDisabledValue);
            _renderer.SetPropertyBlock(_propertyBlock);

            Debug.Log($"[{GetType().Name}] DisableGlow on {gameObject.name}");
        }

        // ================= TEXTURE BLENDING =================

        public void EnableTextureBlending(Texture2D blendTexture, float cutoff = 0.1f,
            float smoothness = 0.4f, float blendingMode = 1f)
        {
            if (!_enableTextureBlending)
            {
                Debug.LogWarning($"[{GetType().Name}] Texture Blending отключён (enableTextureBlending = false) на {gameObject.name}");
                return;
            }

            if (_renderer == null || _propertyBlock == null || blendTexture == null) return;

            // Ключи уже включены в Init. Управляем значениями.
            _propertyBlock.SetTexture(GetBlendTextureProperty(), blendTexture);
            _propertyBlock.SetFloat(GetBlendCutoffProperty(), cutoff);
            _propertyBlock.SetFloat(GetBlendSmoothnessProperty(), smoothness);
            _propertyBlock.SetFloat(GetBlendModeProperty(), blendingMode);
            _renderer.SetPropertyBlock(_propertyBlock);

            Debug.Log($"[{GetType().Name}] EnableTextureBlending on {gameObject.name}: cutoff={cutoff}, texture={blendTexture.name}");
        }

        public void DisableTextureBlending()
        {
            if (!_enableTextureBlending) return;
            if (_renderer == null || _propertyBlock == null) return;

            _propertyBlock.SetFloat(GetBlendCutoffProperty(), BlendingDisabledCutoff);
            _renderer.SetPropertyBlock(_propertyBlock);

            Debug.Log($"[{GetType().Name}] DisableTextureBlending on {gameObject.name}");
        }

        // ================= RESET =================

        public void ResetToOriginal()
        {
            if (_renderer == null || _propertyBlock == null) return;

            if (_enableHit)
                _propertyBlock.SetFloat(GetHitGlowProperty(), GlowDisabledValue);

            if (_enableTextureBlending)
                _propertyBlock.SetFloat(GetBlendCutoffProperty(), BlendingDisabledCutoff);

            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}