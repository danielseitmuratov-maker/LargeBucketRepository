using UnityEngine;

namespace _Root._Scripts.Tools.ShaderTools
{
    public class AllIn1ShaderController : MonoBehaviour
    {
        [Header("Основные настройки")]
        [SerializeField] private Material material; // Если не назначен, будет взят с Renderer

        [Header("Параметры для изменения")]
        [SerializeField] private Color mainColor = Color.white;
        [SerializeField] private float alpha = 1f;
        [SerializeField] private Texture2D mainTexture;

        private Renderer objectRenderer;
        private MaterialPropertyBlock propertyBlock;

        private bool _isInitialized;

        public void Init()
        {
            // Если материал не назначен вручную, пытаемся получить его с Renderer
            if (material == null)
            {
                objectRenderer = GetComponent<Renderer>();
                if (objectRenderer != null)
                    material = objectRenderer.material;
            }

            if (material == null)
            {
                Debug.LogError("Material не найден! Назначьте его в инспекторе или добавьте Renderer на объект.");
                return;
            }

            // Создаём PropertyBlock для изолированных изменений (рекомендуется)
            propertyBlock = new MaterialPropertyBlock();
            objectRenderer?.GetPropertyBlock(propertyBlock);

            ApplyAllProperties();

            _isInitialized = true;
        }

        private void OnEnable()
        {
            if (!_isInitialized)
            {
                ApplyAllProperties();
            }
        }

        /// <summary>
        /// Применяет все заданные свойства к материалу.
        /// </summary>
        public void ApplyAllProperties()
        {
            if (material == null) return;

            // Основные свойства
            SetColor("_Color", mainColor);
            SetFloat("_Alpha", alpha);
            if (mainTexture != null)
                SetTexture("_MainTex", mainTexture);

            // Применяем PropertyBlock, если он используется
            if (objectRenderer != null && propertyBlock != null)
                objectRenderer.SetPropertyBlock(propertyBlock);
        }

        // === Методы для изменения отдельных свойств ===

        public void SetColor(string propertyName, Color color)
        {
            if (material == null) return;
            if (propertyBlock != null)
                propertyBlock.SetColor(propertyName, color);
            else
                material.SetColor(propertyName, color);
        }

        public void SetFloat(string propertyName, float value)
        {
            if (material == null) return;
            if (propertyBlock != null)
                propertyBlock.SetFloat(propertyName, value);
            else
                material.SetFloat(propertyName, value);
        }

        public void SetTexture(string propertyName, Texture texture)
        {
            if (material == null) return;
            if (propertyBlock != null)
                propertyBlock.SetTexture(propertyName, texture);
            else
                material.SetTexture(propertyName, texture);
        }

        public void SetVector(string propertyName, Vector4 vector)
        {
            if (material == null) return;
            if (propertyBlock != null)
                propertyBlock.SetVector(propertyName, vector);
            else
                material.SetVector(propertyName, vector);
        }

        // === Примеры быстрого доступа к популярным свойствам ===

        public void SetMainColor(Color color) => SetColor("_Color", color);
        public void SetAlpha(float value) => SetFloat("_Alpha", value);
        public void SetMainTexture(Texture2D texture) => SetTexture("_MainTex", texture);

        // === Дополнительные свойства (эффекты) ===

        public void SetEmissionColor(Color color) => SetColor("_EmissionColor", color);
        public void SetEmissionStrength(float strength) => SetFloat("_EmissionStrength", strength);
        public void SetHueShift(float hue) => SetFloat("_HueShift", hue);
        public void SetSaturation(float saturation) => SetFloat("_Saturation", saturation);
        public void SetBrightness(float brightness) => SetFloat("_Brightness", brightness);
        public void SetContrast(float contrast) => SetFloat("_Contrast", contrast);
        public void SetGreyscale(float greyscale) => SetFloat("_Greyscale", greyscale);
        public void SetPosterize(float posterize) => SetFloat("_Posterize", posterize);
        public void SetRimPower(float power) => SetFloat("_RimPower", power);
        public void SetRimColor(Color color) => SetColor("_RimColor", color);
        public void SetNormalMap(Texture2D normalMap) => SetTexture("_NormalMap", normalMap);
    }
}