using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Core.AI.Core.Components.Animations;
using KinematicCharacterController.Examples;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Behaviours
{
    public class TransformationBehaviour : NpcBehaviorBase
    {
        private readonly float _duration;
        private float _timer;

        public TransformationBehaviour(float duration = 2f)
        {
            _duration = duration;
        }

        public override void Enter(NpcContext context, GlobalNpcContext globalNpcContext)
        {
            if (context == null) return;

            context.IsTransformed = true;
            context.IsInSpecialZone = true;
            _timer = 0f;

            SwitchModel(context, true);

            StopCharacter(context);
            if (context.Agent != null) context.Agent.ResetPath();

            INpcAnimator animator = context.AnimationPlayer;
            if (animator != null)
            {
                animator.SetState(NpcAnimationState.Idle);
                animator.PlayMovement(0f);
            }

            Debug.Log($"{context.Self.name} трансформировался.");
        }

        public override void Tick(NpcContext context, GlobalNpcContext globalNpcContext)
        {
            if (context == null) return;

            _timer += Time.deltaTime;
            if (_timer >= _duration)
            {
                // Можно оставить пустым, мозг управляет переходом.
            }
        }

        public override void Exit(NpcContext context, GlobalNpcContext globalNpcContext)
        {
            if (context == null) return;

            context.IsTransformed = false;
            context.IsInSpecialZone = false;
            SwitchModel(context, false);

            INpcAnimator animator = context.AnimationPlayer;
            if (animator != null)
            {
                animator.SetState(NpcAnimationState.Idle);
                animator.PlayMovement(0f);
            }
        }

        private void SwitchModel(NpcContext context, bool transformed)
        {
            Transform normal = context.NormalModel;
            Transform transformedModel = context.TransformedModel;

            if (normal != null)
                normal.gameObject.SetActive(!transformed);
            if (transformedModel != null)
                transformedModel.gameObject.SetActive(transformed);
        }

        private void StopCharacter(NpcContext context)
        {
            if (context?.CharacterController == null)
                return;

            var inputs = new AICharacterInputs
            {
                MoveVector = Vector3.zero,
                LookVector = context.Self.forward
            };
            context.CharacterController.SetInputs(ref inputs);
        }
    }
}