using _Root._Scripts.Core.Character;
using _Root._Scripts.Core.Character.CharacterGameRoles.States.Murder;
using _Root._Scripts.Core.Character.Handlers;
using _Root._Scripts.Infrastructure.Services.Sfx.Character;
using CodeBase.Core.Character;
using CodeBase.Infrastructure.Services.Interfaces;
using CodeBase.Infrastructure.Services.Teleporters;
using KinematicCharacterController;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure.Installers.GameEntities
{
    public class CharacterInstaller : MonoInstaller
    {
        [SerializeField] private Transform _transform;
        [SerializeField] KinematicCharacterMotor _motor;
        [SerializeField] private Animator _animator;

        public override void InstallBindings()
        {
            BindRootComponent();
            BindComponents();
            BindHandlers();
        }

        private void BindRootComponent()
        {
            Container
                .BindInterfacesAndSelfTo<CharacterRoot>()
                .FromComponentOnRoot()
                .AsSingle()
                .NonLazy();
        }


        private void BindComponents()
        {
            Container.Bind<ICharacterHealth>().To<CharacterHealth>().AsSingle();
            Container.Bind<ICharacterMovement>().To<CharacterMovement>().AsSingle();

            Container.Bind<ICharacterAttack>()
                .To<CharacterAttack>()
                .AsSingle()
                .WithArguments(_transform);

            Container
                .Bind<ITeleporter>()
                .To<СharacterTeleporter>()
                .AsSingle()
                .WithArguments(_motor.transform);
        }
        
        
        private void BindHandlers()
        {
            Container
                .BindInterfacesTo<CharacterSfxPlayer>()
                .AsSingle()
                .WithArguments(_transform);


            Container
                .BindInterfacesAndSelfTo<CharacterAnimator>()
                .AsSingle()
                .WithArguments(_animator);

            Container.BindInterfacesAndSelfTo<CharacterSoundHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterEffectsHandler>().AsSingle();
        }
    }
}