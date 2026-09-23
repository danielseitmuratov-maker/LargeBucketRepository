using System;
using UnityEngine;

namespace _Root._Scripts.Core.Character.CharacterGameRoles.States.Murder
{
    public interface ICharacterAttack
    {
        event Action<Vector3> OnAttacked;

        void Attack();
    }
}