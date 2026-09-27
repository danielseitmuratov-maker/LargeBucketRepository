using _Root._Scripts.Configs;
using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Core.AI.Npcs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure.Services.Factories.Npcs
{
    public class LobbyNpcFactory : ILobbyNpcFactory
    {
        private readonly IInstantiator _instantiator;
        private readonly IConfigProvider _configProvider;

        public LobbyNpcFactory(IInstantiator instantiator, IConfigProvider configProvider)
        {
            _instantiator = instantiator;
            _configProvider = configProvider;
        }

        public LobbyNpcRoot Create(Vector3 at, Transform parent = null)
        {
            var prefab =
                _configProvider.GetConfig<NpcRoleConfigSo>(Paths.NpcData.Roles.LobbyNpcConfigRolePath)
                    .VisualPrefab as LobbyNpcRoot;
            var gameObject = _instantiator.InstantiatePrefab(prefab, at, prefab.transform.rotation, parent);

            return gameObject.GetComponent<LobbyNpcRoot>();
        }
    }
}