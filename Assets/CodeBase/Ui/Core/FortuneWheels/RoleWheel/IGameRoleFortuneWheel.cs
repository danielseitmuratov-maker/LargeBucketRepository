using System;
using System.Collections.Generic;
using _Root._Scripts.Core.Roles;
using UnityEngine;

namespace _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel
{
    public interface IGameRoleFortuneWheel
    {
        public IReadOnlyList<GameRoleFortuneWheelSegment> Segments { get; }

        event Action<GameRoleFortuneWheelSegment> OnSpinStarted;
        event Action<GameRoleFortuneWheelSegment> OnSpinCompleted;
        event Action<IReadOnlyList<GameRoleFortuneWheelSegment>> OnSegmentsRebuilt;


        void Spin(float duration, Transform parent = null);
        GameRole GiveOutRole();
    }
}