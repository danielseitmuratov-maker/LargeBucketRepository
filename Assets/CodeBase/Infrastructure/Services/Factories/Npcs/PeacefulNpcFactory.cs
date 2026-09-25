using _Root._Scripts.Configs;
using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Core.AI.Npcs;
using _Root._Scripts.Core.GameMap;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Fx;
using _Root._Scripts.Infrastructure.Services.Npc.Providers;
using _Root._Scripts.Infrastructure.Services.Npc.Registrars;
using _Root._Scripts.Infrastructure.Services.RandomPoints;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Factories.Npcs
{
    public class PeacefulNpcFactory : IFactory<PeacefulNpcRoot>
    {
        private readonly INpcBehavioursProvider _npcBehavioursProvider;
        private readonly INpcStateMachineRegistrar _registrar;
        private readonly IGlobalNpcContextProvider _globalNpcContextProvider;
        private readonly ICoroutineRunnerService _coroutineRunnerService;
        private readonly IRandomNavMeshPointService _randomNavMeshPointService;
        private readonly IConfigProvider _configProvider;

        private NpcRoleConfigSo _config;
        private PeacefulNpcRoot _prefab;
        private IGameMapSpecialPointsProvider _gameMapSpecialPointsProvider;
        private ISfxPlayer _sfxPlayer;
        private IFxPlayer _fxPlayer;

        public PeacefulNpcFactory(INpcBehavioursProvider npcBehavioursProvider, INpcStateMachineRegistrar registrar,
            IGlobalNpcContextProvider globalNpcContextProvider, ICoroutineRunnerService coroutineRunnerService,
            IRandomNavMeshPointService randomNavMeshPointService, IConfigProvider configProvider,
            IGameMapSpecialPointsProvider gameMapSpecialPointsProvider,ISfxPlayer sfxPlayer,IFxPlayer fxPlayer)
        {
            _npcBehavioursProvider = npcBehavioursProvider;
            _registrar = registrar;
            _globalNpcContextProvider = globalNpcContextProvider;
            _coroutineRunnerService = coroutineRunnerService;
            _randomNavMeshPointService = randomNavMeshPointService;
            _configProvider = configProvider;
            _gameMapSpecialPointsProvider = gameMapSpecialPointsProvider;
            _sfxPlayer = sfxPlayer;
            _fxPlayer = fxPlayer;

            SetUpStartValues();
        }

        public PeacefulNpcRoot Create(Vector3 at, Transform parent = null)
        {
            PeacefulNpcRoot root = Object.Instantiate(_prefab, at, _prefab.Transform.rotation, parent);
            InitRoot(root);
            return root;
        }

        private void InitRoot(PeacefulNpcRoot root)
        {
            root. Init(_configProvider, _npcBehavioursProvider, _registrar, _globalNpcContextProvider,
                _randomNavMeshPointService, _coroutineRunnerService, _gameMapSpecialPointsProvider,_sfxPlayer,_fxPlayer);
        }


        public PeacefulNpcRoot CreateById(int id, Vector3 at, Transform parent = null)
        {
            PeacefulNpcRoot root = Object.Instantiate(_prefab, at, _prefab.Transform.rotation, parent);
            InitRoot(root);
            return root;
        }

        private void SetUpStartValues()
        {
            _config = _configProvider.GetConfig<NpcRoleConfigSo>(Paths.NpcData.Roles.PeacefulNpcRoleConfigPath);
            _prefab = _config.VisualPrefab as PeacefulNpcRoot;
        }
    }
}