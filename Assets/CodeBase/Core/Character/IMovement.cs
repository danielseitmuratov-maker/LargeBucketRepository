using System;
using KinematicCharacterController.Examples;
using UnityEngine;

namespace _Root._Scripts.Core.Character
{
    public interface IMovement
    {
        event Action<float> MoveSpeedChanged;
        event Action Jumped;

        Vector3 Position { get; }

        void Init(
            ExampleCharacterController characterController,
            ExampleCharacterCamera camera);

        void Move(float deltaTime);
    }
    
    
}