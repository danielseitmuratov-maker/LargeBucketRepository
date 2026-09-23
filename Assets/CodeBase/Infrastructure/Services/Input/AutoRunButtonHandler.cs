using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Root._Scripts.Infrastructure.Services.Input
{
    public class AutoRunButtonHandler : MonoBehaviour, IAutoRunButton ,IPointerClickHandler
    {
        public event Action Performed;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Performed?.Invoke();
        }
    }
}