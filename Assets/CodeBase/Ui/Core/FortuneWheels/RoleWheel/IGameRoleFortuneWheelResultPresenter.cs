using UnityEngine;

namespace _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel
{
    public interface IGameRoleFortuneWheelResultPresenter
    {
        void ShowResult(GameRoleFortuneWheelSegment winningSegment, Color backgroundColor);
        void Hide();
    }
}