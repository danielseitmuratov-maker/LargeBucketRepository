using _Root._Scripts.Core.Character;
using _Root._Scripts.Core.Character.CharacterGameRoles.States.Murder;
using _Root._Scripts.Infrastructure.Services.Interfaces;
using CodeBase.Infrastructure.Services.Teleporters;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure.Installers.GameEntities
{
    public class CharacterInstaller : MonoInstaller
    {
        [SerializeField] private Transform _transform;
        [SerializeField] KinematicCharacterMotor _motor;

        public override void InstallBindings()
        {
            BindKCC();
            BindComponents();
        }

        private void BindComponents()
        {
            Container
                .Bind<IHealth>()
                .WithId("Character")
                .To<CharacterHealth>()
                .AsSingle();

            Container.Bind<IMovement>()
                .WithId("Character")
                .To<CharacterMovement>()
                .AsSingle();

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

        private void BindKCC()
        {
            Container.BindInterfacesAndSelfTo<KinematicCharacterMotor>().FromInstance(_motor).AsSingle();
            Container.BindInterfacesAndSelfTo<ExampleCharacterController>().AsSingle();
            Container.BindInterfacesAndSelfTo<ExampleCharacterCamera>().AsSingle();
        }
    }
}