using System;
using System.Collections.Generic;
using System.Linq;
using _Root._Scripts.Core.Character;
using _Root._Scripts.Core.Roles;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.AI.Core;

namespace _Root._Scripts.Core.AI.Tools
{
    public class NpcRoleAdjuster : IDisposable, INpcRoleAdjuster
    {
        private readonly IConfigProvider _configProvider;
        private readonly ICharacterRoleAdjuster _characterRoleAdjuster;

        private readonly List<NpcRoleConfigSo> _allRoleConfigs;
        private readonly List<NpcRoleConfigSo> _gameRoleConfigs;

        private GameRole _playerRole;
        private bool _playerRoleSelected;

        public NpcRoleAdjuster(IConfigProvider configProvider, ICharacterRoleAdjuster characterRoleAdjuster)
        {
            _configProvider = configProvider;
            _characterRoleAdjuster = characterRoleAdjuster;

            _allRoleConfigs = _configProvider.GetConfigs<NpcRoleConfigSo>(Paths.NpcData.NpcRoleConfigsRootPath);

            _gameRoleConfigs = new List<NpcRoleConfigSo>();
            
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _characterRoleAdjuster.RoleAdjusted += OnCharacterRoleAdjusted;
        }

        private void UnsubscribeFromEvents()
        {
            _characterRoleAdjuster.RoleAdjusted -= OnCharacterRoleAdjusted;
        }

        public List<NpcRoleConfigSo> AdjustRoles()
        {
            if (_allRoleConfigs == null || _allRoleConfigs.Count == 0)
                return new List<NpcRoleConfigSo>();

            if (!_playerRoleSelected)
                return new List<NpcRoleConfigSo>(_allRoleConfigs);

            if (_playerRole == GameRole.Peaceful)
                return new List<NpcRoleConfigSo>(_allRoleConfigs);

            return new List<NpcRoleConfigSo>(
                _allRoleConfigs
                    .Where(role => role != null && role.GameRole != _playerRole));
        }

        private void OnCharacterRoleAdjusted(GameRole role)
        {
            _playerRole = role;
            _playerRoleSelected = true;

            _gameRoleConfigs.Clear();

            _gameRoleConfigs
                .AddRange(_allRoleConfigs
                    .Where(config => config != null && config.GameRole != role));
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();
        }
    }
}