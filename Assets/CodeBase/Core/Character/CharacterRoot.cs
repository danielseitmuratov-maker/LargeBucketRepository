using _Root._Scripts.Core.Character;
using CodeBase.Infrastructure.Services.Teleporters;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine;
using Zenject;

namespace CodeBase.Core.Character
{
    public class CharacterRoot : MonoBehaviour, IInitializable
    {
        private KinematicCharacterMotor _motor;
        private ExampleCharacterCamera _camera;
        private Transform _cameraFollowPoint;
        private IMovement _movement;
        private ITeleporter _teleporter;


        [Inject]
        public void Construct(ExampleCharacterCamera camera, Transform cameraFollowPoint, KinematicCharacterMotor motor,
            IMovement movement,ITeleporter teleporter)
        {
            _motor = motor;
            _camera = camera;
            _cameraFollowPoint = cameraFollowPoint;
            _movement = movement;
            _teleporter = teleporter;
        }

        public void Initialize()
        {
            SetUpController();
        }

        private void SetUpController()
        {
            _camera.SetFollowTransform(_cameraFollowPoint);
        }

        private void Update()
        {
            if (_movement == null)
                return;

            _movement.Move(Time.deltaTime);
        }

        public void TeleportSelfWithDelay(Vector3 to, float delay = 0f) =>
            _motor.SetPosition(_teleporter.Teleport(to, delay));
    }
}