using System;
using TMPro;
using UnityEngine;

namespace _Root._Scripts.Tools
{
    public class AnimationValidator : MonoBehaviour
    {

        [SerializeField] private Animator _animator;

        private bool _isValidate;
        private static readonly int IsAttack = Animator.StringToHash("IsAttack");

        public void Init()
        {
            _isValidate = false;
            _animator.SetTrigger(IsAttack);
        }

        private void Update()
        {
            if (_animator != null && _isValidate == false)
            {
                for (int i = 0; i < _animator.parameters.Length; i++)
                {
                    string name = _animator.parameters[i].name;
                    Debug.Log($"{_animator.avatar.name} {name}");
                }

                AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
                
                _isValidate = true;
            }
        }
    }
}