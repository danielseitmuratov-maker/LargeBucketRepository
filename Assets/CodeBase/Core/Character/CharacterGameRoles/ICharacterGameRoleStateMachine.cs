namespace _Root._Scripts.Core.Character.CharacterGameRoles
{
    public interface ICharacterGameRoleStateMachine
    {
        void Enter<TState>() where TState : class, ICharacterGameRoleState;

        
        void Tick();
    }
}