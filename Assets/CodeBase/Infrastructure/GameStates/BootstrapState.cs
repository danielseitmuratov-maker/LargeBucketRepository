using System.Collections.Generic;
using _Root._Scripts.Core.AI.Npcs;
using _Root._Scripts.Core.AI.Tools;
using _Root._Scripts.Core.Character;
using _Root._Scripts.Core.GameMap;
using _Root._Scripts.Core.Lobby;
using _Root._Scripts.Core.Roles;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Cursor;
using _Root._Scripts.Infrastructure.Services.Factories;
using _Root._Scripts.Infrastructure.Services.Factories.Npcs;
using _Root._Scripts.Infrastructure.Services.Fx;
using _Root._Scripts.Infrastructure.Services.GlobalSettings;
using _Root._Scripts.Infrastructure.Services.Input;
using _Root._Scripts.Infrastructure.Services.Loaders;
using _Root._Scripts.Infrastructure.Services.Npc.Providers;
using _Root._Scripts.Infrastructure.Services.Npc.Registrars;
using _Root._Scripts.Infrastructure.Services.RandomPoints;
using _Root._Scripts.Infrastructure.Services.Saves;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using _Root._Scripts.Infrastructure.Services.Sfx.Timer;
using _Root._Scripts.Infrastructure.Services.Spawners;
using _Root._Scripts.Infrastructure.Services.Timers;
using _Root._Scripts.Infrastructure.Services.WeightedRandom;
using _Root._Scripts.Ui;
using _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel;
using CodeBase.Infrastructure.Services.Input;
using UnityEngine;
using UnityEngine.Audio;
using YG;

namespace _Root._Scripts.Infrastructure.GameStates
{
    public class BootstrapState : IState
    {
        private readonly StateMachine _stateMachine;

        private ICoroutineRunner _coroutineRunner;

        private readonly MainHudHandler _mainHudHandler;
        private readonly GameRoleFortuneWheelRoot _gameRoleFortuneWheelRoot;


        private readonly IJumpButton _jumpButton;
        private readonly IAttackButton _attackButton;
        private readonly IAutoRunButton _autoRunButton;
        private readonly IAutoAttackButton _autoAttackButton;
        private readonly AudioMixer _audioMixer;
        private AudioMixerGroup _audioMixerGroup;
        private GameTester _gameTester;


        public BootstrapState(StateMachine stateMachine, ICoroutineRunner coroutineRunner, IJumpButton jumpButton,
            IAttackButton attackButton,
            IAutoRunButton autoRunButton, IAutoAttackButton autoAttackButton, AudioMixer audioMixer,
            AudioMixerGroup audioMixerGroup, MainHudHandler mainHudHandler,
            GameRoleFortuneWheelRoot gameRoleFortuneWheelRoot,
            GameTester gameTester)
        {
            _stateMachine = stateMachine;
            _coroutineRunner = coroutineRunner;
            _jumpButton = jumpButton;
            _attackButton = attackButton;
            _autoRunButton = autoRunButton;
            _autoAttackButton = autoAttackButton;
            _audioMixer = audioMixer;
            _audioMixerGroup = audioMixerGroup;
            _mainHudHandler = mainHudHandler;
            _gameRoleFortuneWheelRoot = gameRoleFortuneWheelRoot;
            _gameTester = gameTester;
            _coroutineRunner = coroutineRunner;

            RegisterServices();
        }

        public void Enter()
        {
            _stateMachine.Enter<MainMenuState>();
        }

        public void Exit()
        {
            Debug.Log("bootstrap exit");
        }

        private void RegisterServices()
        {
            IConfigProvider configProvider = new ConfigProvider();
            G.Register(configProvider);

            IWeightedRandomService weightedRandomService = new WeightedRandomService();
            G.Register(weightedRandomService);

            ISaveLoadService saveLoadService = new SaveLoadService(_coroutineRunner);
            G.Register(saveLoadService);

            IInputService inputService =
                RegisterInputService(_jumpButton, _attackButton, _autoAttackButton, _autoRunButton);
            G.Register(inputService);

            ISfxPlayer sfxPlayer = new SfxPlayer(_audioMixerGroup, _coroutineRunner, 100);
            G.Register(sfxPlayer);

            IFxPlayer fxPlayer = new FxPlayer(_coroutineRunner);
            G.Register(fxPlayer);

            ICharacterRoleAdjuster characterRoleAdjuster =
                new CharacterRoleAdjuster(saveLoadService, weightedRandomService);
            G.Register(characterRoleAdjuster);

            ICursorLocker cursorLocker = new CursorLocker();
            G.Register(cursorLocker);

            IGlobalSettingsService globalSettingsService = new GlobalSettingsService(saveLoadService, _audioMixer);
            G.Register(globalSettingsService);

            IGameRoleFortuneWheel gameRoleFortuneWheel =
                new GameRoleFortuneWheel(configProvider, characterRoleAdjuster, _coroutineRunner);
            G.Register(gameRoleFortuneWheel);

            INpcRoleAdjuster npcRoleAdjuster = new NpcRoleAdjuster(configProvider, characterRoleAdjuster);
            G.Register(npcRoleAdjuster);

            IRandomNavMeshPointService randomNavMeshPointService = new RandomNavMeshPointService(configProvider);
            G.Register(randomNavMeshPointService);

            INpcBehavioursProvider npcBehavioursProvider = new NpcBehavioursProvider();
            G.Register(npcBehavioursProvider);

            INpcStateMachineRegistrar npcStateMachineRegistrar = new NpcStateMachineRegistrar();
            G.Register(npcStateMachineRegistrar);

            IPreGameCycleTimer preGameCycleTimer = new PreGameCycleTimer(configProvider, _coroutineRunner);
            G.Register(preGameCycleTimer);

            IGameLoopTimer gameLoopTimer = new GameLoopTimer(configProvider, _coroutineRunner);
            G.Register(gameLoopTimer);

            IGlobalNpcContextProvider globalNpcContextProvider =
                new GlobalNpcContextProvider(gameLoopTimer, configProvider, _coroutineRunner);
            G.Register(globalNpcContextProvider);

            IFactory<LobbyNpcRoot> lobbyNpcFactory = new LobbyNpcFactory(configProvider, npcBehavioursProvider,
                npcStateMachineRegistrar, globalNpcContextProvider, randomNavMeshPointService,sfxPlayer,fxPlayer);
            G.Register(lobbyNpcFactory);

            ILoader<GameMapRoot> gameMapLoader = new GameMapLoader(configProvider);
            G.Register(gameMapLoader);

            ITimerSfxPlayer timerSfxPlayer = new TimerSfxPlayer(sfxPlayer, configProvider);
            G.Register(timerSfxPlayer);

            ILoader<GameLobbyRoot> gameLobbyLoader =
                new GameLobbyLoader(configProvider, preGameCycleTimer, timerSfxPlayer);
            G.Register(gameLobbyLoader);

            INpcSpawner<LobbyNpcRoot> lobbyNpcSpawner =
                new LobbyNpcSpawner(lobbyNpcFactory, randomNavMeshPointService, configProvider);
            G.Register(lobbyNpcSpawner);

            IFactory<CharacterRoot> characterFactory =
                new CharacterFactory(configProvider, inputService, saveLoadService, sfxPlayer, _coroutineRunner,
                    gameRoleFortuneWheel, fxPlayer);
            G.Register(characterFactory);

            IGameMapSpecialPointsProvider gameMapSpecialPointsProvider =
                new GameMapSpecialPointsProvider(gameMapLoader);
            G.Register(gameMapSpecialPointsProvider);

            IFactory<PeacefulNpcRoot> peacefulNpcFactory = new PeacefulNpcFactory(npcBehavioursProvider,
                npcStateMachineRegistrar, globalNpcContextProvider, _coroutineRunner, randomNavMeshPointService,
                configProvider, gameMapSpecialPointsProvider,sfxPlayer,fxPlayer);
            G.Register(peacefulNpcFactory);

            //pzdsh polnui broui 
            Dictionary<GameRole, IFactory<NpcRoot>> npcFactories = new Dictionary<GameRole, IFactory<NpcRoot>>()
            {
                {GameRole.Peaceful, peacefulNpcFactory},
            };

            INpcSpawnerWithRoles gameLoopSpawnerWithRoles =
                new NpcSpawnerWithRoles(npcFactories, randomNavMeshPointService, configProvider);
            G.Register(gameLoopSpawnerWithRoles);

            _mainHudHandler.Init(configProvider, _coroutineRunner, gameMapLoader, sfxPlayer);

            _gameRoleFortuneWheelRoot.Init(configProvider, characterRoleAdjuster, _coroutineRunner, sfxPlayer,
                fxPlayer, gameRoleFortuneWheel);


            //tests here 

            _gameTester.Init(gameMapLoader);

            //  ISpawnPointGenerator spawnPointGenerator = new ChunkSpawnPointGenerator(configProvider);
            //  G.Register(spawnPointGenerator);

            //  ILoader<ChunkRoot> chunkLoader = new ChunkLoader(chunkPool, spawnPointGenerator, _coroutineRunner,
            //      weightedRandomService, configProvider);
            //  G.Register(chunkLoader);

            ////   IWallet wallet = new Wallet(saveLoadService);
            // //  G.Register(wallet);

            //   IAdsService adsService = new AdsService();
            //   G.Register(adsService);

            //   ISoundPitcher soundPitcher = new SoundPitcher();
            //   G.Register(soundPitcher);

            //   ICursorLocker cursorLocker = new CursorLocker();
            //   G.Register(cursorLocker);

            //   IMoneyFormatter moneyFormatter = new MoneyFormatter();
            //   G.Register(moneyFormatter);
            //   
            //   IBillboardService billBoardService = new BillboardService();
            //   G.Register(billBoardService);

            //   IShortAnimationService shortAnimationService = new ShortAnimationService();
            //   G.Register(shortAnimationService);

            //   IWeightedRandomService weightedRandomService = new WeightedRandomService();
            //   G.Register(weightedRandomService);

            //   ITotalPlayTimeUpdateService totalPlayTimeUpdateService = new TotalPlayTimeUpdateService(saveLoadService);
            //   G.Register(totalPlayTimeUpdateService);

            //   ISfxPlayer sfxPlayer = new SfxPlayer(_sfxAudioMixerGroup, _coroutineRunner, 40);
            //   G.Register(sfxPlayer);

            //   IGlobalSettingsService globalSettingsService = new GlobalSettingsService(saveLoadService, _audioMixer);
            //   G.Register(globalSettingsService);

            //   IRebirthService rebirthService = new RebirthService(configProvider, wallet, saveLoadService);
            //   G.Register(rebirthService);

            //   IUpgradeService upgradeService = new UpgradeService(wallet, saveLoadService, configProvider);
            //   G.Register(upgradeService);

            //   IFactory<EggRoot> eggFactory = new EggFactory(configProvider, sfxPlayer, shortAnimationService);
            //   G.Register(eggFactory);

            //   List<EggConfig> eggConfigs = configProvider.GetConfigs<EggConfig>(ConfigsPath.EggConfigsRootPath);

            //   Dictionary<EggRarity, EggPool> pools = new Dictionary<EggRarity, EggPool>();
            //   Dictionary<EggRarity, EggConfig> configsByRarity = new Dictionary<EggRarity, EggConfig>();
            //   InitEggPools(eggConfigs, configsByRarity, eggFactory, rebirthService, pools);
            //   IEggPoolsByRarity eggPoolsByRarity = new EggPoolsByRarity(pools, configsByRarity);
            //   G.Register(eggPoolsByRarity);

            //   IFactory<AnimalRoot> animalFactory =
            //       new AnimalFactory(configProvider, rebirthService, upgradeService, sfxPlayer, shortAnimationService,
            //           eggPoolsByRarity, moneyFormatter);
            //   G.Register(animalFactory);

            //   ISpawner<AnimalRoot> animalSpawner = new AnimalSpawner(_coroutineRunner, configProvider, animalFactory,
            //       rebirthService, saveLoadService, sfxPlayer);
            //   G.Register(animalSpawner);

            //   IFactory<PetRoot> petFactory = new PetFactory(configProvider);
            //   G.Register(petFactory);

            //   IPetInventoryService petInventoryService = new PetInventoryService(configProvider, saveLoadService);
            //   G.Register(petInventoryService);

            //   IPetService petService = new PetService(configProvider, saveLoadService);
            //   G.Register(petService);

            //   IFactory<PetChest> petChestFactory = new PetChestFactory(configProvider);
            //   G.Register(petChestFactory);

            //   IPetDropAnimator petDropAnimator = new PetDropAnimator(configProvider, _coroutineRunner, petChestFactory,
            //       petFactory, sfxPlayer, saveLoadService);
            //   G.Register(petDropAnimator);

            //   IFactory<WheatCollectible> wheatCollectibleFactory = new WheatCollectibleFactory(configProvider, sfxPlayer);
            //   G.Register(wheatCollectibleFactory);

            //   IPool<WheatCollectible> wheatCollectiblePool =
            //       new WheatCollectiblePool(wheatCollectibleFactory, configProvider);
            //   G.Register(wheatCollectiblePool);

            //   IFactory<WheatRoot> wheatFactory =
            //       new WheatFactory(configProvider, weightedRandomService, sfxPlayer, wheatCollectiblePool,
            //           saveLoadService,billBoardService);
            //   G.Register(wheatFactory);

            //   IBuildingsService buildingsService = new BuildingsService(saveLoadService, wallet);
            //   G.Register(buildingsService);

            //   IFactory<BonusRoot> bonusFactory = new BonusFactory(configProvider, adsService, sfxPlayer);
            //   G.Register(bonusFactory);

            //   IBonusApplierService bonusApplierService =
            //       new BonusApplierService(saveLoadService, _coroutineRunner, configProvider);
            //   G.Register(bonusApplierService);

            //   IPool<LevelRoot> levelPool = new LevelPool(configProvider, soundPitcher, moneyFormatter, wallet,
            //       animalSpawner, sfxPlayer, rebirthService, upgradeService, saveLoadService, _startLevel,
            //       petInventoryService, petDropAnimator, petChestFactory, wheatFactory, buildingsService, bonusFactory,
            //       bonusApplierService,billBoardService);
            //   G.Register(levelPool);

            //  ILevelLoader levelLoader = new LevelLoader(levelPool);
            //  G.Register(levelLoader);

            //  IClueDrawer clueDrawer = new ClueDrawer(configProvider, _lineRenderer);
            //  G.Register(clueDrawer);

            //  IPetFormationService petFormationService = new PetFormationService();
            //  G.Register(petFormationService);

            //  IPetSlotHandleService petSlotHandleService = new PetSlotHandleService();
            //  G.Register(petSlotHandleService);

            //_mainHudHandler.Init(globalSettingsService, rebirthService, configProvider, moneyFormatter, adsService,
            //    sfxPlayer, saveLoadService, petInventoryService, petService, wallet, totalPlayTimeUpdateService);

            //_characterRoot.Init(configProvider, cursorLocker, moneyFormatter, clueDrawer, animalSpawner,
            //    rebirthService, upgradeService, wallet, globalSettingsService, sfxPlayer, eggPoolsByRarity,
            //    saveLoadService, petService, petDropAnimator, petFactory, petInventoryService,
            ///     petSlotHandleService, wheatCollectiblePool, totalPlayTimeUpdateService);
        }


        private IInputService RegisterInputService(IJumpButton jumpButton, IAttackButton attackButton,
            IAutoAttackButton autoAttackButton, IAutoRunButton autoRunButton)
        {
            if (YG2.envir.isMobile)
                return new MobileInputService(jumpButton, attackButton, autoRunButton, autoAttackButton);
            if (YG2.envir.isDesktop)
                return new PcInputService(autoRunButton, autoAttackButton);

            return null;
        }
    }
}