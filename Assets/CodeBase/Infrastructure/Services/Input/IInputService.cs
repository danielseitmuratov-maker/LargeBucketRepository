using System;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Input
{
    public interface IInputService
    {
        public event Action<Vector2> SwipeDirectionChanged;
        public event Action JumpPerformed;
        public event Action AttackPerformed;
        public event Action AutoRunToggled;
        public event Action AutoAttackToggled;

        public Vector2 SwipeDirection { get;}
        public Vector2 MoveAxis { get; }

        Vector2 ReadMovement();
        Vector2 ReadLook();
        Vector2 ReadZoom();
        Vector2 ReadCameraRotation();
    }
}