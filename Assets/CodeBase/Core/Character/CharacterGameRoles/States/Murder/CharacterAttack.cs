using System;
using _Root._Scripts.Configs;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Interfaces;
using UnityEngine;

namespace _Root._Scripts.Core.Character.CharacterGameRoles.States.Murder
{
    public class CharacterAttack : ICharacterAttack
    {
        public event Action<Vector3> OnAttacked;

        private readonly Transform _characterTransform;
        private readonly IConfigProvider _configProvider;
        private CharacterConfig _config;

        private Collider[] _attackedColliders;
        private int _maxAttackCollidersAmount;
        private float _attackRadius;
        private LayerMask _attackTargetLayerMask;
        private float _basicDamage;

        public CharacterAttack(Transform characterTransform, IConfigProvider configProvider)
        {
            _characterTransform = characterTransform;
            _configProvider = configProvider;

            GetConfigs();
            SetUpValues();
        }

        public void Attack()
        {
            int targetAmount = Physics.OverlapSphereNonAlloc(_characterTransform.position, _attackRadius,
                _attackedColliders,
                _attackTargetLayerMask);

            for (int i = 0; i < targetAmount; i++)
            {
                if (_attackedColliders[i].TryGetComponent(out IHealth health))
                {
                    health.TakeDamage(_basicDamage);
                    OnAttacked?.Invoke(_attackedColliders[i].transform.position);
                }
            }
        }

        private void GetConfigs() =>
            _config = _configProvider.GetConfig<CharacterConfig>(Paths.GlobalValues.CharacterConfigPath);

        private void SetUpValues()
        {
            _attackRadius = _config.AttackRaidus;
            _maxAttackCollidersAmount = _config.MaxAttackCollidersAmount;
            _attackedColliders = new Collider[_maxAttackCollidersAmount];
            _attackTargetLayerMask = _config.AttackTargetLayerMask;

            _basicDamage = _config.BasicDamage;
        }
    }
}