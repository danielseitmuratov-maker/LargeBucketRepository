using System;
using _Root._Scripts.Configs;
using _Root._Scripts.Core.Roles;
using _Root._Scripts.Tools.ShaderTools;
using DG.Tweening;
using UnityEngine;

namespace _Root._Scripts.Ui.Core.FortuneWheels.RoleWheel
{
    public class GameRoleFortuneWheelSegment : MonoBehaviour
    {
        public event Action Highlighted;

        [SerializeField] private FullCustomShaderApplier _fullCustomShaderApplier;
        [SerializeField] private Animator _animator; // если есть аниматор

        public GameRole GameRole => _gameRole;
        private GameRole _gameRole;
        private GameRoleFortuneWheelFieldConfig _config;

        public void Init(GameRoleFortuneWheelFieldConfig config)
        {
            _config = config;
            _gameRole = config.GameRole;
            _fullCustomShaderApplier.Init();
        }

        public void Highlight()
        {
            Highlighted?.Invoke();
        }

        public void PlayWinAnimation()
        {
            if (_animator != null)
                _animator.SetTrigger("Win");
            else
                Debug.Log($"Segment {name} Win animation (no Animator)");
        }

        public void StopWinAnimation()
        {
            if (_animator != null)
                _animator.ResetTrigger("Win");
        }
    }
}