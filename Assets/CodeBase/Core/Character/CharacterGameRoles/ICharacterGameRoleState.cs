using _Root._Scripts.Infrastructure;

namespace _Root._Scripts.Core.Character.CharacterGameRoles
{
    public interface ICharacterGameRoleState : IExitableState
    {
        void Enter();
        void Tick();
    }
}