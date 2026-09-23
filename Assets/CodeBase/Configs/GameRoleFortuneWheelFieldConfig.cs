using _Root._Scripts.Core.Roles;
using _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel;
using UnityEngine;

namespace _Root._Scripts.Configs
{
    [CreateAssetMenu(fileName = "GameRoleFortuneWheelFieldConfig", menuName = "SO/GameRoleFortuneWheelFieldConfig", order = 0)]
    public class GameRoleFortuneWheelFieldConfig : ScriptableObject    
    {
        [field: SerializeField] public GameRole GameRole { get; private set; }
        [field: SerializeField] public GameRoleFortuneWheelSegment Prefab { get; private set; }
        
        [field: SerializeField] public Color BackGroundColor { get; private set; }
    }
}