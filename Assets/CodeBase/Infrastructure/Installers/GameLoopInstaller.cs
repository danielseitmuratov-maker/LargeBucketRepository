using _Root._Scripts.Core.AI.Npcs;
using _Root._Scripts.Core.AI.Tools;
using _Root._Scripts.Infrastructure.Services.Factories.Npcs;
using _Root._Scripts.Infrastructure.Services.Npc.Providers;
using _Root._Scripts.Infrastructure.Services.Npc.Registrars;
using _Root._Scripts.Infrastructure.Services.RandomPoints;
using _Root._Scripts.Infrastructure.Services.Timers;
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
            BindFactories();
            BindLoaders();
            BindHandleServices();
        }

        private void BindHandleServices()
        {
            throw new System.NotImplementedException();
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
        
        private void BindFactories()
        {
        }
        
        private void BindLoaders()
        {
        }
        
         private void BindHandleServices()
         {
         }
    }
}