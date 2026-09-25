using System.Collections.Generic;
using _Root._Scripts.Core.AI.Npcs;
using _Root._Scripts.Core.AI.Tools;
using _Root._Scripts.Core.Character;
using _Root._Scripts.Core.GameMap;
using _Root._Scripts.Core.Lobby;
using _Root._Scripts.Core.Roles;
using _Root._Scripts.Infrastructure;
using _Root._Scripts.Infrastructure.GameStates;
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

namespace CodeBase.Infrastructure.GameStates
{
    public class BootstrapState : IState
    {
        private readonly StateMachine _stateMachine;

        public BootstrapState(StateMachine stateMachine)
        {
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
         //   IConfigProvider configProvider = new ConfigProvider();
        //    G.Register(configProvider);

        //    IWeightedRandomService weightedRandomService = new WeightedRandomService();
        //    G.Register(weightedRandomService);

        //    ISaveLoadService saveLoadService = new SaveLoadService(_coroutineRunner);
        //    G.Register(saveLoadService);

         //   IInputService inputService =
        //        RegisterInputService(_jumpButton, _attackButton, _autoAttackButton, _autoRunButton);
        //    G.Register(inputService);

        //    ISfxPlayer sfxPlayer = new SfxPlayer(_audioMixerGroup, _coroutineRunnerService, 100);
        //    G.Register(sfxPlayer);

       //     IFxPlayer fxPlayer = new FxPlayer(_coroutineRunnerService);
       //     G.Register(fxPlayer);

         //   ICharacterRoleAdjuster characterRoleAdjuster =
          //      new CharacterRoleAdjuster(saveLoadService, weightedRandomService);
         //   G.Register(characterRoleAdjuster);

          //  ICursorLocker cursorLocker = new CursorLocker();
          //  G.Register(cursorLocker);

         //   IGlobalSettingsService globalSettingsService = new GlobalSettingsService(saveLoadService, _audioMixer);
         //   G.Register(globalSettingsService);

           // IGameRoleFortuneWheel gameRoleFortuneWheel =
        //        new GameRoleFortuneWheel(configProvider, characterRoleAdjuster, _coroutineRunnerService);
        //    G.Register(gameRoleFortuneWheel);

         //   INpcRoleAdjuster npcRoleAdjuster = new NpcRoleAdjuster(configProvider, characterRoleAdjuster);
        //    G.Register(npcRoleAdjuster);

        //    IRandomNavMeshPointService randomNavMeshPointService = new IRandomNavMeshPointService(configProvider);
          //  G.Register(randomNavMeshPointService);

        //    INpcBehavioursProvider npcBehavioursProvider = new NpcBehavioursProvider();
        //    G.Register(npcBehavioursProvider);

           // INpcStateMachineRegistrar npcStateMachineRegistrar = new NpcStateMachineRegistrar();
         //   G.Register(npcStateMachineRegistrar);

          //  IPreGameCycleTimer preGameCycleTimer = new PreGameCycleTimer(configProvider, _coroutineRunnerService);
         //   G.Register(preGameCycleTimer);

          //  IGameLoopTimer gameLoopTimer = new GameLoopTimer(configProvider, _coroutineRunnerService);
          //  G.Register(gameLoopTimer);

          //  IGlobalNpcContextProvider globalNpcContextProvider =
         //       new GlobalNpcContextProvider(gameLoopTimer, configProvider, _coroutineRunnerService);
        //    G.Register(globalNpcContextProvider);

       //     IFactory<LobbyNpcRoot> lobbyNpcFactory = new LobbyNpcFactory(configProvider, npcBehavioursProvider,
      //          npcStateMachineRegistrar, globalNpcContextProvider, randomNavMeshPointService,sfxPlayer,fxPlayer);
       //     G.Register(lobbyNpcFactory);

       //     ILoader<GameMapRoot> gameMapLoader = new GameMapLoader(configProvider);
       //     G.Register(gameMapLoader);

        //    ITimerSfxPlayer timerSfxPlayer = new TimerSfxPlayer(sfxPlayer, configProvider);
       //     G.Register(timerSfxPlayer);

       //     ILoader<GameLobbyRoot> gameLobbyLoader =
       //         new GameLobbyLoader(configProvider, preGameCycleTimer, timerSfxPlayer);
        //    G.Register(gameLobbyLoader);

        //    INpcSpawner<LobbyNpcRoot> lobbyNpcSpawner =
        //        new LobbyNpcSpawner(lobbyNpcFactory, randomNavMeshPointService, configProvider);
       //     G.Register(lobbyNpcSpawner);

        //   IFactory<CharacterRoot> characterFactory =
        //       new CharacterFactory(configProvider, inputService, saveLoadService, sfxPlayer, _coroutineRunnerService,
        //           gameRoleFortuneWheel, fxPlayer);
        //   G.Register(characterFactory);

        //   IGameMapSpecialPointsProvider gameMapSpecialPointsProvider =
        //       new GameMapSpecialPointsProvider(gameMapLoader);
        //   G.Register(gameMapSpecialPointsProvider);

        //   IFactory<PeacefulNpcRoot> peacefulNpcFactory = new PeacefulNpcFactory(npcBehavioursProvider,
        //       npcStateMachineRegistrar, globalNpcContextProvider, _coroutineRunnerService, randomNavMeshPointService,
        //       configProvider, gameMapSpecialPointsProvider,sfxPlayer,fxPlayer);
        //   G.Register(peacefulNpcFactory);

        //   //pzdsh polnui broui 
        //   Dictionary<GameRole, IFactory<NpcRoot>> npcFactories = new Dictionary<GameRole, IFactory<NpcRoot>>()
        //   {
        //       {GameRole.Peaceful, peacefulNpcFactory},
        //   };

        //   INpcSpawnerWithRoles gameLoopSpawnerWithRoles =
        //       new NpcSpawnerWithRoles(npcFactories, randomNavMeshPointService, configProvider);
        //   G.Register(gameLoopSpawnerWithRoles);

        //   _mainHudHandler.Init(configProvider, _coroutineRunnerService, gameMapLoader, sfxPlayer);

        //   _gameRoleFortuneWheelRoot.Init(configProvider, characterRoleAdjuster, _coroutineRunnerService, sfxPlayer,
        //       fxPlayer, gameRoleFortuneWheel);

        //   _gameTester.Init(gameMapLoader);
        }
    }
}s