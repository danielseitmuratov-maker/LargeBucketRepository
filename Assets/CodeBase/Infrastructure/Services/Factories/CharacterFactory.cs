using _Root._Scripts.Configs;
using _Root._Scripts.Core.Character;
using _Root._Scripts.Core.Character.CharacterGameRoles.States.Murder;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Fx;
using _Root._Scripts.Infrastructure.Services.Input;
using _Root._Scripts.Infrastructure.Services.Saves;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Factories
{
    public class CharacterFactory : IFactory<CharacterRoot>
    {
        private readonly IConfigProvider _configProvider;
        private readonly IInputService _inputService;
        private readonly ISaveLoadService _saveLoadService;
        private readonly ISfxPlayer _sfxPlayer;

        private CharacterConfig _config;
        private CharacterRoot _prefab;
        private ICoroutineRunner _coroutineRunner;
        private IRoleDispatcher _roleDispatcher;
        private IGameRoleFortuneWheel _gameRoleFortuneWheel;
        private IFxPlayer _fxPlayer;

        public CharacterFactory(IConfigProvider configProvider, IInputService inputService,
            ISaveLoadService saveLoadService, ISfxPlayer sfxPlayer, ICoroutineRunner coroutineRunner,
            IGameRoleFortuneWheel gameRoleFortuneWheel, IFxPlayer fxPlayer)
        {
            _configProvider = configProvider;
            _inputService = inputService;
            _saveLoadService = saveLoadService;
            _sfxPlayer = sfxPlayer;
            _coroutineRunner = coroutineRunner;
            _gameRoleFortuneWheel = gameRoleFortuneWheel;
            _fxPlayer = fxPlayer;

            SetUpValues();
        }

        public CharacterRoot Create(Vector3 at, Transform parent = null)
        {
            CharacterRoot characterRoot = Object.Instantiate(_prefab, at, _prefab.transform.rotation);
            InitRoot(characterRoot);
            return characterRoot;
        }


        public CharacterRoot CreateById(int id, Vector3 at, Transform parent = null)
        {
            CharacterRoot characterRoot = Object.Instantiate(_prefab, at, _prefab.transform.rotation);
            InitRoot(characterRoot);
            return characterRoot;
        }

        private void InitRoot(CharacterRoot characterRoot)
        {
            characterRoot.Init(_inputService, _saveLoadService, _configProvider, _sfxPlayer, _coroutineRunner,
                _gameRoleFortuneWheel, _fxPlayer);
        }

        private void SetUpValues()
        {
            _config = _configProvider.GetConfig<CharacterConfig>(Paths.GlobalValues.CharacterConfigPath);
            _prefab = _config.Prefab;
        }
    }
}