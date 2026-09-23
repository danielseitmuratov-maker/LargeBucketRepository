using System;
using System.Collections.Generic;

namespace _Root._Scripts.Core.Character
{
    public interface ICharacterRoleAdjuster
    {
        event Action<Roles.GameRole> RoleAdjusted;

        Roles.GameRole Adjust();

        List<float> GetDropChances();
    }
}