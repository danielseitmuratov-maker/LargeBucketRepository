using _Root._Scripts.Core.AI.Tools;
using _Root._Scripts.Infrastructure.Services.Npc.Providers;
using _Root._Scripts.Infrastructure.Services.Npc.Registrars;
using _Root._Scripts.Infrastructure.Services.RandomPoints;
using _Root._Scripts.Infrastructure.Services.Sfx.Timer;
using _Root._Scripts.Infrastructure.Services.Timers;
using CodeBase.Infrastructure.Services.Factories;
using CodeBase.Infrastructure.Services.Factories.Npcs;
using CodeBase.Ui.Core.Timers;
using Zenject;

namespace CodeBase.Infrastructure.Installers
{
    public class GameLoopInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindGameLogicServices();
            BindNpcServices();
            BindTimers();
            BindTimerComponents();
            BindFactories();
            BindLoaders();
            BindHandleServices();
        }

        private void BindGameLogicServices()
        {
            Container.Bind<IRandomNavMeshPointService>().To<IRandomNavMeshPointService>().AsSingle();
        }

        private void BindNpcServices()
        {
            Container.Bind<INpcRoleAdjuster>().To<NpcRoleAdjuster>().AsSingle();
            Container.Bind<INpcBehavioursProvider>().To<NpcBehavioursProvider>().AsSingle();
            Container.Bind<INpcStateMachineRegistrar>().To<NpcStateMachineRegistrar>().AsSingle();
            Container.Bind<IGlobalNpcContextProvider>().To<GlobalNpcContextProvider>().AsSingle();
        }

        private void BindTimers()
        {
            Container.Bind<IPreGameCycleTimer>().To<PreGameCycleTimer>().AsSingle();
            Container.Bind<IGameLoopTimer>().To<GameLoopTimer>().AsSingle();
        }
        
        private void BindTimerComponents()
        {
            Container
                .BindInterfacesAndSelfTo<PreGameCycleTimerRoot>()
                .FromComponentOnRoot()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<ITimerSfxPlayer>().To<TimerSfxPlayer>().AsSingle();
        }
        
        private void BindFactories()
        {
            Container.Bind<ICharacterFactory>().To<CharacterFactory>().AsSingle();

            BindNpcFactories();
        }

        private void BindNpcFactories()
        {
            Container.Bind<ILobbyNpcFactory>().To<LobbyNpcFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<PeacefulNpcFactory>().AsSingle();
        }

        private void BindLoaders()
        {
        }
        
         private void BindHandleServices()
         {
             
         }
    }
}