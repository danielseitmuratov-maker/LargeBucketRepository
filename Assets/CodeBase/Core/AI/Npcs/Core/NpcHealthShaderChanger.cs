using System.Collections;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Interfaces;
using _Root._Scripts.Tools.ShaderTools;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Npcs.Core
{
    public class NpcHealthShaderChanger : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InteractiveObjectShaderApplier _shaderApplier;

        private Color _damageColor;
        private float _glowIntensity;
        private float _flashDuration;

        private Texture2D _damageTexture;
        private float _blendCutoff;
        private float _blendSmoothness;
        private float _blendMode;

        private IHealth _health;
        private IConfigProvider _configProvider;
        private ICoroutineRunner _coroutineRunner;

        private Coroutine _flashCoroutine;
        private ShaderChangerConfig _config;

        public void Init(IHealth health, IConfigProvider configProvider, ICoroutineRunner coroutineRunner)
        {
            _health = health;
            _configProvider = configProvider;
            _coroutineRunner = coroutineRunner;

            GetConfig();
            SetUpValues();
            InitializeComponents();
            SubscribeToEvents();
        }

        private void GetConfig()
        {
            _config = _configProvider.GetConfig<ShaderChangerConfig>(Paths.GlobalValues.ShaderChangerConfigPath);
        }

        private void SetUpValues()
        {
            _glowIntensity = _config.NpcOnDamageGlowIntensity;
            _flashDuration = _config.NpcOnDamageFlashDuration;
            _damageColor = _config.NpcOnDamageColor;

            _damageTexture = _config.NpcOnDamageTexture;
            _blendCutoff = _config.NpcOnDamageBlendCutoff;
            _blendSmoothness = _config.NpcOnDamageBlendSmoothness;
            _blendMode = _config.NpcOnDamageBlendMode;
        }

        private void InitializeComponents()
        {
            if (_shaderApplier != null)
                _shaderApplier.Init();
        }

        private void SubscribeToEvents()
        {
            if (_health != null)
                _health.HealthChanged += OnHealthChanged;
        }

        private void UnsubscribeFromEvents()
        {
            if (_health != null)
                _health.HealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(float damage)
        {
            if (damage < 0)
                return;

            if (_flashCoroutine != null)
                _coroutineRunner.StopCoroutine(_flashCoroutine);

            _flashCoroutine = _coroutineRunner.StartCoroutine(FlashDamageEffect());
        }

        private IEnumerator FlashDamageEffect()
        {
            if (_shaderApplier == null)
                yield break;

            _shaderApplier.EnableGlow(_damageColor, _glowIntensity);

            if (_damageTexture != null)
                _shaderApplier.EnableTextureBlending(_damageTexture, _blendCutoff, _blendSmoothness, _blendMode);

            yield return new WaitForSeconds(_flashDuration);

            _shaderApplier.DisableTextureBlending();
            _shaderApplier.DisableGlow();

            _flashCoroutine = null;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();

            if (_flashCoroutine != null)
                _coroutineRunner.StopCoroutine(_flashCoroutine);

            if (_shaderApplier != null)
            {
                _shaderApplier.DisableTextureBlending();
                _shaderApplier.DisableGlow();
                _shaderApplier.ResetToOriginal();
            }
        }
    }
}