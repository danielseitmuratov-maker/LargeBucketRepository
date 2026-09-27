using _Root._Scripts.Infrastructure;

namespace CodeBase.Core.Character.CharacterGameRoles
{
    public interface ICharacterGameRoleState : IExitableState
    {
        void Enter();
        void Tick();
    }
}