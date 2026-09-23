using _Root._Scripts.Core.Character.CharacterGameRoles.States.Murder;
using _Root._Scripts.Infrastructure.Services.Fx;
using _Root._Scripts.Infrastructure.Services.Input;

namespace _Root._Scripts.Core.Character.CharacterGameRoles.States
{
    public class CharacterMurderGameRoleState : ICharacterGameRoleState
    {
        private readonly IRoleDispatcher _murderRoleDispatcher;

        private readonly IFxPlayer _fxPlayer;
        private readonly IInputService _inputService;

        public CharacterMurderGameRoleState(IRoleDispatcher murderRoleDispatcher)
        {
            _murderRoleDispatcher = murderRoleDispatcher;
        }

        public void Exit()
        {
            
        }

        public void Enter()
        {
        }

        public void Tick() => 
            _murderRoleDispatcher?.Tick();
    }
}