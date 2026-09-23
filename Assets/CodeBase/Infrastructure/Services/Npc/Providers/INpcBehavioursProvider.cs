using System.Collections.Generic;
using _Root._Scripts.Core.AI.Behaviours;
using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;

namespace _Root._Scripts.Infrastructure.Services.Npc.Providers
{
    public interface INpcBehavioursProvider
    {
        List<IStateBehaviour> CreateBehavioursForRole(NpcRoleConfigSo roleConfig);
    }
}