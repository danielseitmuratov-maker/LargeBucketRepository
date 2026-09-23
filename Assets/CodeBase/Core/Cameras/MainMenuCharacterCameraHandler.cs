using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Input;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using UnityEngine;

namespace _Root._Scripts.Cameras
{
    public class MainMenuCharacterCameraHandler : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        [SerializeField] private StartGameButtonHandler _startGameButtonHandler;

        public void Init( IConfigProvider configProvider,
            ISfxPlayer sfxPlayer)
        {

            _startGameButtonHandler.Init(configProvider, sfxPlayer);

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _startGameButtonHandler.Performed += OnGameStarted;
        }

        private void UnsubscribeFromEvents()
        {
            _startGameButtonHandler.Performed -= OnGameStarted;
        }

        private void OnGameStarted()
        {
            _camera.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    }
}