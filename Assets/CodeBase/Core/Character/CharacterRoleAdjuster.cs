using System;
using System.Collections.Generic;
using _Root._Scripts.Core.Roles;
using _Root._Scripts.Infrastructure.Services.Saves;
using _Root._Scripts.Infrastructure.Services.WeightedRandom;
using UnityEngine;
using YG;

namespace _Root._Scripts.Core.Character
{
    public class CharacterRoleAdjuster : ICharacterRoleAdjuster
    {
        public event Action<GameRole> RoleAdjusted;

        private readonly ISaveLoadService _saveLoadService;
        private readonly IWeightedRandomService _weightedRandomService;

        private readonly SavesYG _data;
        private readonly List<float> _dropChances;

        public CharacterRoleAdjuster(ISaveLoadService saveLoadService, IWeightedRandomService weightedRandomService)
        {
            _saveLoadService = saveLoadService;
            _weightedRandomService = weightedRandomService;
            _data = _saveLoadService.Data;

            _dropChances = new List<float>
            {
                _data.PeacfullRoleDropChance,
                _data.MurderRoleDropChance,
                _data.SheriffRoleDropChance,
                _data.DoctorRoleDropChance
            };
        }

        public List<float> GetDropChances()
        {
            if (_dropChances.Count > 0 && _dropChances != null)
                return _dropChances;

            Debug.LogError($"character role adjuster/ trouble with drop chances");
            return new List<float>();
        }

        public GameRole Adjust()
        {
            if (_data.IsFirstGameLoop)
            {
               // _data.IsFirstGameLoop = false;
                return AdjustFirstGameRole();
            }
            
            return AdjustInteractiveRole();
        }
        
        private GameRole AdjustFirstGameRole()
        {
            RoleAdjusted?.Invoke(GameRole.Murder);

            _data.IsFirstGameLoop = false;

            return GameRole.Murder;
        }

        private GameRole AdjustInteractiveRole()
        {
            if (_weightedRandomService == null)
            {
                RoleAdjusted?.Invoke(GameRole.Peaceful);
                return GameRole.Peaceful;
            }

            int roleIndex = _weightedRandomService.GetWeightedRandom(
                0,
                _dropChances.Count - 1,
                _dropChances);

            GameRole selectedRole = ConvertIndexToRole(roleIndex);

            RoleAdjusted?.Invoke(selectedRole);

            return selectedRole;
        }

        private GameRole ConvertIndexToRole(int index)
        {
            switch (index)
            {
                case 0:
                    return GameRole.Peaceful;

                case 1:
                    return GameRole.Murder;

                case 2:
                    return GameRole.Sheriff;

                case 3:
                    return GameRole.Doctor;

                default:
                    return GameRole.Peaceful;
            }
        }
    }
}