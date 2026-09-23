using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Root._Scripts.Infrastructure.Services.Input
{
    public class JumpButtonHandler : MonoBehaviour, IJumpButton ,IPointerClickHandler
    {
        public event Action Performed;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Performed?.Invoke();
        }
    }
}