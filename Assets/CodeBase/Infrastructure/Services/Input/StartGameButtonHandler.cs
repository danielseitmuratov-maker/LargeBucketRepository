using System;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using _Root._Scripts.Ui.Core;
using UnityEngine.EventSystems;

namespace _Root._Scripts.Infrastructure.Services.Input
{
    public class StartGameButtonHandler : UiButton ,IPointerClickHandler
    {
        public event Action Performed;
        
        public void Init(IConfigProvider configProvider ,ISfxPlayer sfxPlayer)
        {
            base.Init(configProvider, sfxPlayer);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PlayClickSound();
            Performed?.Invoke();
        }
    }
}