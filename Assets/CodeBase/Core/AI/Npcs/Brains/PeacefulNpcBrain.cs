using System;
using System.Collections.Generic;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.AI.Behaviours;
using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Infrastructure.Services.ConfigProviding;
using _Root._Scripts.Infrastructure.Services.Npc.Providers;
using _Root._Scripts.Infrastructure.Services.Npc.Registrars;
using _Root._Scripts.Infrastructure.Services.Npc.StateMachine;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Npcs.Brains
{
    public class PeacefulNpcBrain : INpcBrain, IDisposable
    {
        private readonly IConfigProvider _configProvider;
        private readonly INpcBehavioursProvider _npcBehavioursProvider;
        private readonly INpcStateMachineRegistrar _stateMachineRegistrar;
        private readonly IGlobalNpcContextProvider _globalNpcContextProvider;
        private readonly NpcContext _localContext;

        private NpcRoleConfigSo _roleConfig;
        private List<IStateBehaviour> _behaviors = new List<IStateBehaviour>();
        private IBehaviourStateMachine _stateMachine;
        private bool _isInitialized;

        // Таймер для фазы укрытия
        private float _hidingTimer;

        public PeacefulNpcBrain(IConfigProvider configProvider,
            INpcBehavioursProvider npcBehavioursProvider,
            INpcStateMachineRegistrar stateMachineRegistrar,
            IGlobalNpcContextProvider globalNpcContextProvider,
            NpcContext localContext)
        {
            _configProvider = configProvider;
            _npcBehavioursProvider = npcBehavioursProvider;
            _stateMachineRegistrar = stateMachineRegistrar;
            _globalNpcContextProvider = globalNpcContextProvider;
            _localContext = localContext;

            SetUpRoleConfig();
            SetUpBehaviours();
            SetUpStateMachine();
            SetUpStartBehaviour();

            _isInitialized = true;
        }

        private void SetUpStartBehaviour()
        {
            _stateMachine.Enter<RunOnNavMeshBehaviour>();
            _hidingTimer = 0f;
        }

        private void SetUpRoleConfig() =>
            _roleConfig = _configProvider.GetConfig<NpcRoleConfigSo>(Paths.NpcData.Roles.PeacefulNpcRoleConfigPath);

        private void SetUpBehaviours() =>
            _behaviors = GetBehavioursByRole(_roleConfig);

        private void SetUpStateMachine()
        {
            GlobalNpcContext globalContext = _globalNpcContextProvider.GetGlobalContext();

            _stateMachine = _stateMachineRegistrar.CreateStateMachine(_behaviors);
            _stateMachine.SetContexts(_localContext, globalContext);
        }

        private List<IStateBehaviour> GetBehavioursByRole(NpcRoleConfigSo roleConfig) =>
            _npcBehavioursProvider.CreateBehavioursForRole(roleConfig);

        public void Tick()
        {
            if (!_isInitialized) return;

            _stateMachine.Tick();

            GlobalNpcContext globalContext = _globalNpcContextProvider.GetGlobalContext();


            if (globalContext.IsPrepareTimeOut && !_localContext.IsInSpecialZone)
            {
                RunToSpecialZone();
                return;
            }

            if (_localContext.IsInSpecialZone && !_localContext.IsTransformed && IsHidingPointReached())
            {
                Transform();
                return;
            }

            if (_localContext.IsTransformed && _localContext.IsInSpecialZone)
            {
                _hidingTimer += Time.deltaTime;
                if (_hidingTimer >= _localContext.PersonSimulationTimeAmount)
                {
                    UnTransform();
                    _hidingTimer = 0f;
                }

                return;
            }

            if (MurderNearby())
                FleeFromMurder();
        }


        private void RunToSpecialZone()
        {
            _stateMachine.Enter<RunToHidingZoneBehaviour>();
            Debug.Log($"{_localContext.Self.name} бежит к укрытию.");
        }

        private void Transform()
        {
            _stateMachine.Enter<TransformationBehaviour>();
            Debug.Log($"{_localContext.Self.name} трансформируется.");
        }

        private void UnTransform()
        {
            _localContext.IsTransformed = false;
            _localContext.IsInSpecialZone = false;
            SwitchModel(false);

            _stateMachine.Enter<RunOnNavMeshBehaviour>();
            Debug.Log($"{_localContext.Self.name} вернулся к патрулированию.");
        }

        private void FleeFromMurder()
        {
            _stateMachine.Enter<FleeFromMurderBehaviour>();
        }

        private bool MurderNearby()
        {
            if (_localContext.Murder == null)
                return false;
            float distance = Vector3.Distance(_localContext.Self.position, _localContext.Murder.position);

            return distance < _localContext.FleeRadius;
        }

        private bool IsHidingPointReached()
        {
            if (_localContext.Agent == null)
                return false;
            return Vector3.Distance(_localContext.Self.position, _localContext.HidingPoint) <=
                   _localContext.WanderPointReachedDistance;
        }

        private void SwitchModel(bool transformed)
        {
            Transform normal = _localContext.NormalModel;
            Transform transformedModel = _localContext.TransformedModel;

            if (normal != null)
                normal.gameObject.SetActive(!transformed);

            if (transformedModel != null)
                transformedModel.gameObject.SetActive(transformed);
        }

        public void Dispose()
        {
        }
    }
}