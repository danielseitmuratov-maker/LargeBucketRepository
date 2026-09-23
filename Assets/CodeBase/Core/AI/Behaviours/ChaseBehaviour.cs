using _Root._Scripts.Core.AI.Core;
using KinematicCharacterController.Examples;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Behaviours
{
    public class ChaseBehaviour : NpcBehaviorBase
    {
        private readonly float _stopDistance;
        private readonly float _repathDelay;

        private float _nextRepathTime;
        private Vector3 _lastTargetPosition;

        public ChaseBehaviour(float stopDistance = 1f, float repathDelay = 0.5f)
        {
            _stopDistance = Mathf.Max(0.1f, stopDistance);
            _repathDelay = Mathf.Max(0f, repathDelay);

            _nextRepathTime = 0f;
            _lastTargetPosition = Vector3.zero;
            
        }

        public override void Enter(NpcContext localContext, GlobalNpcContext globalContext)
        {
            //;
        }

        public override void Tick(NpcContext context,GlobalNpcContext globalNpcContext)
        {
            if (context == null || context.Agent == null || context.CharacterController == null || !context.Agent.enabled)
                return;

            if (!context.Agent.isOnNavMesh)
            {
                StopCharacter(context);
                return;
            }

            // Если цель пропала или ушла слишком далеко – выходим (переключение произойдёт в основном цикле)
            if (context.Target == null)
                return;

            float distanceToTarget = Vector3.Distance(context.Self.position, context.Target.position);
            if (distanceToTarget > context.DetectionRadius * 1.5f)
                return;

            // Если цель достаточно близко – останавливаемся
            if (distanceToTarget <= _stopDistance)
            {
                StopCharacter(context);
                return;
            }

            // Синхронизация позиции (т.к. KinematicCharacterMotor управляет трансформом)
            context.Agent.nextPosition = context.Self.position;

            // Перестраиваем путь, если цель сместилась или прошло время
            if (Time.time >= _nextRepathTime || Vector3.Distance(context.Target.position, _lastTargetPosition) > 1f)
            {
                SetDestinationToTarget(context);
            }

            if (context.Agent.pathPending)
            {
                StopCharacter(context);
                return;
            }

            if (!context.Agent.hasPath)
            {
                SetDestinationToTarget(context);
                return;
            }

            Vector3 steeringTarget = context.Agent.steeringTarget;
            Vector3 direction = steeringTarget - context.Self.position;
            direction = Vector3.ProjectOnPlane(direction, context.Self.up);

            if (direction.sqrMagnitude <= 0.001f)
            {
                StopCharacter(context);
                return;
            }

            direction.Normalize();

            var inputs = new AICharacterInputs
            {
                MoveVector = direction,
                LookVector = direction
            };
            context.CharacterController.SetInputs(ref inputs);
        }

        public override void Exit(NpcContext context,GlobalNpcContext globalNpcContext)
        {
            _lastTargetPosition = Vector3.zero;
            StopCharacter(context);

            if (context?.Agent != null && context.Agent.enabled && context.Agent.isOnNavMesh)
                context.Agent.ResetPath();
        }

        private void SetDestinationToTarget(NpcContext context)
        {
            if (context?.Agent == null || !context.Agent.enabled || !context.Agent.isOnNavMesh || context.Target == null)
                return;

            Vector3 targetPos = context.Target.position;
            bool success = context.Agent.SetDestination(targetPos);
            if (success)
            {
                _lastTargetPosition = targetPos;
                _nextRepathTime = Time.time + _repathDelay;
                context.Agent.isStopped = false;
            }
            else
            {
                _nextRepathTime = Time.time + _repathDelay;
                StopCharacter(context);
            }
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