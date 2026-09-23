using System;
using System.Collections.Generic;
using _Root._Scripts.Core.AI.Core;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Npc.StateMachine
{
    public class BehaviourStateMachine : IBehaviourStateMachine
    {
        private readonly Dictionary<Type, IStateBehaviour> _behaviours = new Dictionary<Type, IStateBehaviour>();
        private IStateBehaviour _currentBehaviour;
        private NpcContext _localContext;
        private GlobalNpcContext _globalContext;

        public BehaviourStateMachine(IEnumerable<IStateBehaviour> behaviours)
        {
            foreach (var behaviour in behaviours)
            {
                var type = behaviour.GetType();
                if (!_behaviours.ContainsKey(type))
                    _behaviours[type] = behaviour;
                else
                    Debug.LogWarning($"Behaviour {type.Name} already registered, skipping.");
            }
        }

        public void SetContexts(NpcContext localContext, GlobalNpcContext globalContext)
        {
            _localContext = localContext;
            _globalContext = globalContext;
        }

        public void RegisterBehaviour<T>(T behaviour) where T : IStateBehaviour
        {
            Type type = typeof(T);
            
            if (!_behaviours.ContainsKey(type))
                _behaviours[type] = behaviour;
            else
                Debug.LogWarning($"Behaviour {type.Name} already registered, skipping.");
        }

        public void Enter<T>() where T : class, IStateBehaviour
        {
            Type type = typeof(T);
            if (!_behaviours.TryGetValue(type, out var newBehaviour))
            {
                Debug.LogError($"Behaviour {type.Name} not registered in state machine.");
                return;
            }

            if (_currentBehaviour != null && _currentBehaviour.GetType() == type)
                return;

            _currentBehaviour?.Exit(_localContext, _globalContext);
            _currentBehaviour = newBehaviour;
            _currentBehaviour?.Enter(_localContext, _globalContext);
        }

        public void Tick()
        {
            _currentBehaviour?.Tick(_localContext, _globalContext);
        }
    }
}