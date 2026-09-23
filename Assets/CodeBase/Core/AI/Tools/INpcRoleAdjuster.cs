using System.Collections.Generic;
using _Root._Scripts.Core.AI.Core;

namespace _Root._Scripts.Core.AI.Tools
{
    public interface INpcRoleAdjuster
    {
        List<NpcRoleConfigSo> AdjustRoles();
    }
}