using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Fx;
using _Root._Scripts.Infrastructure.Services.Npc.Providers;
using _Root._Scripts.Infrastructure.Services.Npc.Registrars;
using _Root._Scripts.Infrastructure.Services.Sfx.Base;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Npcs
{
    public abstract class NpcRoot : MonoBehaviour, INpcRoot
    {
        protected IConfigProvider ConfigProvider;

        protected INpcBehavioursProvider NpcBehavioursProvider;
        protected INpcStateMachineRegistrar Registrar;
        protected IGlobalNpcContextProvider GlobalNpcContextProvider;
        protected ISfxPlayer SfxPlayer;
        protected IFxPlayer FxPlayer;

        public virtual void Init(IConfigProvider configProvider, INpcBehavioursProvider npcBehavioursProvider,
            INpcStateMachineRegistrar registrar, IGlobalNpcContextProvider globalNpcContextProvider,
            ISfxPlayer sfxPlayer, IFxPlayer fxPlayer)
        {
            ConfigProvider = configProvider;
            NpcBehavioursProvider = npcBehavioursProvider;
            Registrar = registrar;
            GlobalNpcContextProvider = globalNpcContextProvider;
            SfxPlayer = sfxPlayer;
            FxPlayer = fxPlayer;
        }

        public abstract Transform Transform { get; }
    }
}