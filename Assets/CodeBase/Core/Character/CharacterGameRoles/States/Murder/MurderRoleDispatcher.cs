using System;
using System.Collections;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.Roles;
using _Root._Scripts.Infrastructure;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Fx;
using _Root._Scripts.Infrastructure.Services.Input;
using _Root._Scripts.Infrastructure.Services.Sfx.Character;
using UnityEngine;

namespace _Root._Scripts.Core.Character.CharacterGameRoles.States.Murder
{
    public class MurderRoleDispatcher : IDisposable, IRoleDispatcher
    {
        private readonly IFxPlayer _fxPlayer;
        private readonly ICharacterSfxPlayer _sfxPlayer;
        private readonly IConfigProvider _configProvider;
        private readonly Transform _characterTransform;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly ICharacterAttack _characterAttack;
        private readonly IInputService _inputService;

        private VfxConfig _fxConfig;
        private SoundsConfig _sfxConfig;

        private Coroutine _attackRoutine;
        private bool _isAttacking;

        private GameRole _currentGameRole;

        public MurderRoleDispatcher(GameRole gameRole, IFxPlayer fxPlayer,
            ICharacterSfxPlayer sfxPlayer, IConfigProvider configProvider, Transform characterTransform,
            ICoroutineRunner coroutineRunner, ICharacterAttack characterAttack,IInputService inputService)
        {
            _currentGameRole = gameRole;
            _fxPlayer = fxPlayer;
            _sfxPlayer = sfxPlayer;
            _configProvider = configProvider;
            _characterTransform = characterTransform;
            _coroutineRunner = coroutineRunner;
            _characterAttack = characterAttack;
            _inputService = inputService;

            GetConfigs();
            SpawnOutlineFx();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _inputService.AttackPerformed += OnAttackPerformed;
        }


        private void UnsubscribeFromEvents()
        {
            _inputService.AttackPerformed -= OnAttackPerformed;
        }


        public void Tick()
        {
            // ticki ticki tiuuuuu
        }

        private void OnAttackPerformed()
        {
            if (_isAttacking)
                return;

            _isAttacking = true;


            _characterAttack.Attack();
            Debug.Log("murder attack performed");
            PlayAttackFx();
            PlayAttackSfx();
        }

        private void PlayAttackSfx() =>
            _sfxPlayer.PlayBaseAttack();

        private void PlayAttackFx()
        {
            _attackRoutine = _coroutineRunner.StartCoroutine(AttackFxCoroutine());
        }

        private IEnumerator AttackFxCoroutine()
        {
            ParticleSystem attackEffect = _fxConfig.CharacterMurderAttackEffect;

            _fxPlayer.PlayFx(attackEffect, _characterTransform, _characterTransform.position);

            yield return new WaitForSeconds(0.45f);
            _fxPlayer.StopFx(attackEffect);

            _isAttacking = false;
            _attackRoutine = null;
        }

        private void GetConfigs() =>
            _fxConfig = _configProvider.GetConfig<VfxConfig>(Paths.GlobalValues.ParticleConfigPath);

        private void SpawnOutlineFx() =>
            _fxPlayer.PlayFx(_fxConfig.CharacterAttackOutlineLoopEffect, _characterTransform.position);

        public void Dispose()
        {
            UnsubscribeFromEvents();

            if (_coroutineRunner != null && _attackRoutine != null)
            {
                _coroutineRunner.StopCoroutine(_attackRoutine);
                _attackRoutine = null;
            }

            _isAttacking = false;
        }
    }
}