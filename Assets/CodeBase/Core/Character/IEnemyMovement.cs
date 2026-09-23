using System;
using UnityEngine;

namespace _Root._Scripts.Core.Character
{
    public interface IEnemyMovement : IMovement
    {
        event Action TargetReached;
        
        void SetDefaultMoveTarget(Transform defaultTarget);
        void RestoreDefaultTarget();
        
        void ChangeTarget(Transform newTarget);
    }
}