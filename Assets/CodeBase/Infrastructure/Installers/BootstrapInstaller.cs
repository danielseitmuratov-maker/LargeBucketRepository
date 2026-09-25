using _Root._Scripts.Core.AI.Tools;
using _Root._Scripts.Core.Character;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Cursor;
using _Root._Scripts.Infrastructure.Services.Fx;
using _Root._Scripts.Infrastructure.Services.GlobalSettings;
using _Root._Scripts.Infrastructure.Services.Input;
using _Root._Scripts.Infrastructure.Services.Saves;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using _Root._Scripts.Infrastructure.Services.WeightedRandom;
using _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel;
using CodeBase.Infrastructure.Services.Input;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace CodeBase.Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private AudioMixerGroup _audioMixerGroup;

        public override void InstallBindings()
        {
            BindInfrastructureServices();
            BindInputService();
            BindMonoComponents();
        }

        private void BindMonoComponents()
        {
            Container.BindInstance(_audioMixerGroup).AsSingle();
        }

        private void BindInfrastructureServices()
        {
            Container.Bind<ICoroutineRunnerService>().To<CoroutineRunnerService>().AsSingle();
            Container.Bind<IConfigProvider>().To<ConfigProvider>().AsSingle();
            Container.Bind<IWeightedRandomService>().To<WeightedRandomService>().AsSingle();
            Container.Bind<ISaveLoadService>().To<SaveLoadService>().AsSingle();
            Container.Bind<ISfxPlayer>().To<SfxPlayer>().AsSingle();
            Container.Bind<IFxPlayer>().To<FxPlayer>().AsSingle();
            Container.Bind<ICharacterRoleAdjuster>().To<CharacterRoleAdjuster>().AsSingle();
            Container.Bind<ICursorLocker>().To<CursorLocker>();
            Container.Bind<IGlobalSettingsService>().To<GlobalSettingsService>().AsSingle();
            Container.Bind<IGameRoleFortuneWheel>().To<GameRoleFortuneWheel>().AsSingle();
            
        }
        
        private void BindInputService()
        {
            if (Application.isMobilePlatform)
                Container.Bind<IInputService>().To<MobileInputService>().AsSingle();
            else
                Container.Bind<IInputService>().To<PcInputService>().AsSingle();
        }
    }
}