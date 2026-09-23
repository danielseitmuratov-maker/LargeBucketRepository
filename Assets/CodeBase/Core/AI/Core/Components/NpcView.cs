using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Root._Scripts.Core.AI.Core.Components
{
    public class NpcView : MonoBehaviour
    {
        public bool IsInitialized => _isInitialized;

        [Header("UI")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _roleText;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TMP_Text _healthText;

        private NpcRoleConfigSo _roleConfigSo;

        private bool _isInitialized;

        public void Init(NpcRoleConfigSo roleConfigSo)
        {
            if (_isInitialized) 
                return;

            _roleConfigSo = roleConfigSo;

            SetUpStartValues();

            _isInitialized = true;
        }

        private void SetUpStartValues()
        {
            _roleText.text = _roleConfigSo.GameRoleNameRu;
        }

        public void Show()
        {
            if (_canvasGroup == null) 
                return;
            
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            if (_canvasGroup == null)
                return;
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public void SetVisible(bool visible)
        {
            if (visible) 
                Show();
            else
                Hide();
        }

        private void OnDestroy()
        {
            _isInitialized = false;
        }
        
    }
}