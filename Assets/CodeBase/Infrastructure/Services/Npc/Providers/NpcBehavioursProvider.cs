using System.Collections.Generic;
using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Core.AI.Core.So;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;

namespace _Root._Scripts.Infrastructure.Services.Npc.Providers
{
    public class NpcBehavioursProvider : INpcBehavioursProvider
    {
        public List<IStateBehaviour> CreateBehavioursForRole(NpcRoleConfigSo roleConfig)
        {
            List<IStateBehaviour> behaviours = new List<IStateBehaviour>();
            
            if (ConfigIsNotValid(roleConfig)) 
                return behaviours;

            for (int i = 0; i < roleConfig.Behaviours.Count; i++)
            {
                BehaviourFactorySo factory = GetFactoryFromConfigByIndex(roleConfig, i);
                
                if (FactoryIsNull(factory)) 
                    continue;

                CreateBehaviour(factory, behaviours);
            }

            return behaviours;
        }

        private bool ConfigIsNotValid(NpcRoleConfigSo roleConfig) => 
            roleConfig != null && roleConfig.Behaviours == null;

        private static void CreateBehaviour(BehaviourFactorySo factory, List<IStateBehaviour> behaviours)
        {
            IStateBehaviour behaviour = factory.Create();
            if (behaviour != null)
                behaviours.Add(behaviour);
        }

        private bool FactoryIsNull(BehaviourFactorySo factory)
        {
            if (factory == null)
                return true;
            
            return false;
        }

        private BehaviourFactorySo GetFactoryFromConfigByIndex(NpcRoleConfigSo roleConfig, int index)
        {
            if (IndexIsValid(roleConfig, index))
            {
                BehaviourFactorySo factory = roleConfig.Behaviours[index];
                return factory;
            }

            return null;
        }

        private bool IndexIsValid(NpcRoleConfigSo roleConfig, int index) => 
            index < roleConfig.Behaviours.Count && !float.IsNaN(index);
    }
}