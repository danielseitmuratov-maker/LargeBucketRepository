using _Root._Scripts.Configs;
using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Core.AI.Npcs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Fx;
using _Root._Scripts.Infrastructure.Services.Npc.Providers;
using _Root._Scripts.Infrastructure.Services.Npc.Registrars;
using _Root._Scripts.Infrastructure.Services.RandomPoints;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Factories.Npcs
{
    public class LobbyNpcFactory : IFactory<LobbyNpcRoot>
    {
        private readonly IConfigProvider _configProvider;

        private LobbyNpcRoot _prefab;
        private INpcBehavioursProvider _npcBehavioursProvider;
        private INpcStateMachineRegistrar _npcStateMachineRegistrar;
        private IGlobalNpcContextProvider _globalNpcContextProvider;
        private readonly IRandomNavMeshPointService _randomNavMeshPointService;
        private NpcRoleConfigSo _config;
        private ISfxPlayer _sfxPlayer;
        private IFxPlayer _fxPlayer;


        public LobbyNpcFactory(IConfigProvider configProvider, INpcBehavioursProvider npcBehavioursProvider,
            INpcStateMachineRegistrar npcStateMachineRegistrar, IGlobalNpcContextProvider globalNpcContextProvider,
            IRandomNavMeshPointService randomNavMeshPointService,ISfxPlayer sfxPlayer,IFxPlayer fxPlayer)
        {
            _configProvider = configProvider;
            _npcBehavioursProvider = npcBehavioursProvider;
            _npcStateMachineRegistrar = npcStateMachineRegistrar;
            _globalNpcContextProvider = globalNpcContextProvider;
            _randomNavMeshPointService = randomNavMeshPointService;
            _sfxPlayer = sfxPlayer;
            _fxPlayer = fxPlayer;

            SetUpConfig();
        }

        public LobbyNpcRoot Create(Vector3 at, Transform parent = null)
        {
            LobbyNpcRoot root = Object.Instantiate(_prefab, at, _prefab.transform.rotation, parent);
            InitRoot(root);

            return root;
        }

        public LobbyNpcRoot CreateById(int id, Vector3 at, Transform parent = null)
        {
            LobbyNpcRoot root = Object.Instantiate(_prefab, at, _config.VisualPrefab.Transform.rotation, parent);

            InitRoot(root);

            return root;
        }

        private void SetUpConfig()
        {
            _config = _configProvider.GetConfig<NpcRoleConfigSo>(Paths.NpcData.Roles.LobbyNpcConfigRolePath);
            _prefab = _config.VisualPrefab as LobbyNpcRoot;
        }

        private void InitRoot(LobbyNpcRoot root) =>
            root.Init(_configProvider, _npcBehavioursProvider, _npcStateMachineRegistrar, _globalNpcContextProvider,
                _randomNavMeshPointService,_sfxPlayer,_fxPlayer);
    }
}