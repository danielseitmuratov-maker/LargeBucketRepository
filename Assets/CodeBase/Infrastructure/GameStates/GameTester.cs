using _Root._Scripts.Core.GameMap;
using _Root._Scripts.Infrastructure.Services.Loaders;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.GameStates
{

    public class GameTester : MonoBehaviour
    {
        private ILoader<GameMapRoot> _gameMapLoader;

        public void Init(ILoader<GameMapRoot> gameMapLoader)
        {
            _gameMapLoader = gameMapLoader;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O)) 
                _gameMapLoader.Load(0);
        }
    }
}