using System;
using UnityEngine;

namespace CodeBase.Core.Character
{
    public interface IMovement
    {
        event Action<float> MoveSpeedChanged;
        event Action Jumped;

        Vector3 Position { get; }

        void Move(float deltaTime);
    }
}