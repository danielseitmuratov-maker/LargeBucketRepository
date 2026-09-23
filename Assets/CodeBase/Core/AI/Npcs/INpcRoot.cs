using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Fx;
using _Root._Scripts.Infrastructure.Services.Npc.Providers;
using _Root._Scripts.Infrastructure.Services.Npc.Registrars;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Npcs
{
    public interface INpcRoot
    {
        public void Init(IConfigProvider configProvider, INpcBehavioursProvider npcBehavioursProvider,
            INpcStateMachineRegistrar registrar, IGlobalNpcContextProvider globalNpcContextProvider,
            ISfxPlayer sfxPlayer, IFxPlayer fxPlayer);

        Transform Transform { get; }
    }
}