using _Root._Scripts.Configs;
using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using CodeBase.Core.AI.Npcs;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure.Services.Factories.Npcs
{
    public class PeacefulNpcFactory : IPeacefulNpcFactory ,IInitializable
    {
        private readonly IInstantiator _instantiator;
        private readonly IConfigProvider _configProvider;

        private NpcRoleConfigSo _config;
        private PeacefulNpcRoot _prefab;

        public PeacefulNpcFactory(IInstantiator instantiator)
        {
            _instantiator = instantiator;
        }
        
        public void Initialize()
        {
            SetUpStartValues();
        }
        
        public PeacefulNpcRoot Create(Vector3 at, Transform parent = null)
        {
            return _instantiator.InstantiatePrefabForComponent<PeacefulNpcRoot>(_prefab, at, _prefab.transform.rotation,
                parent);
        }

        private void SetUpStartValues()
        {
            _config = _configProvider.GetConfig<NpcRoleConfigSo>(Paths.NpcData.Roles.PeacefulNpcRoleConfigPath);
            _prefab = _config.VisualPrefab as PeacefulNpcRoot;
        }
    }
}