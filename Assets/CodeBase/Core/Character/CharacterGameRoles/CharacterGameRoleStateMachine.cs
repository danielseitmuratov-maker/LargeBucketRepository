using System;
using System.Collections.Generic;

namespace _Root._Scripts.Core.Character.CharacterGameRoles
{
    public class CharacterGameRoleStateMachine : ICharacterGameRoleStateMachine
    {
        private Dictionary<Type, ICharacterGameRoleState> _states;
        private ICharacterGameRoleState _currentState;

        public CharacterGameRoleStateMachine()
        {
            _states = new Dictionary<Type, ICharacterGameRoleState>()
            {
             //  [typeof(CharacterMurderGameRoleState)] = new CharacterMurderGameRoleState(),
               
            };
        }

        public void Enter<TState>() where TState : class, ICharacterGameRoleState
        {
            TState state = ChangeState<TState>();

            state.Enter();
        }

        public void Tick() => 
            _currentState?.Tick();

        private TState ChangeState<TState>() where TState : class, ICharacterGameRoleState
        {
            _currentState?.Exit();

            TState state = GetState<TState>();
            _currentState = state;

            return state;
        }

        private TState GetState<TState>() where TState : class, ICharacterGameRoleState
        {
            return (TState) _states[typeof(TState)];
        }
    }
}