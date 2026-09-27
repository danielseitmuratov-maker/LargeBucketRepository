using _Root._Scripts.Configs;
using _Root._Scripts.Core.Character;
using _Root._Scripts.Core.Character.CharacterGameRoles.States.Murder;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Fx;
using _Root._Scripts.Infrastructure.Services.Input;
using _Root._Scripts.Infrastructure.Services.Saves;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel;
using CodeBase.Core.Character;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure.Services.Factories
{
    public class CharacterFactory : ICharacterFactory
    {
        private readonly IInstantiator _instantiator;
        private readonly IConfigProvider _configProvider;

        public CharacterFactory(IInstantiator instantiator,[CanBeNull] IConfigProvider configProvider)
        {
            _instantiator = instantiator;
            _configProvider = configProvider;
        }

        public CharacterRoot Create(Vector3 at,Transform parent)
        {
            var prefab = _configProvider.GetConfig<CharacterConfig>(Paths.GlobalValues.CharacterConfigPath).Prefab;
            var gameObject =  _instantiator.InstantiatePrefab(prefab,at,prefab.transform.rotation,parent);
            return gameObject.GetComponent<CharacterRoot>();
        }
    }
}