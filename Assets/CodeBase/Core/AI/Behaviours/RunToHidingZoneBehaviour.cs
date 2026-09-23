using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Core.AI.Core.Components.Animations;
using KinematicCharacterController.Examples;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Behaviours
{
    public class RunToHidingZoneBehaviour : NpcBehaviorBase
    {
        private readonly float _reachedDistance;
        private readonly float _repathDelay;

        private bool _hasDestination;
        private float _nextRepathTime;
        private Vector3 _currentTarget;

        // Поля для детекции застревания
        private float _stuckCheckTimer;
        private Vector3 _lastPosition;
        private float _stuckTimeThreshold = 1.5f;
        private float _movementThreshold = 0.1f;

        public RunToHidingZoneBehaviour(float reachedDistance = 0.5f, float repathDelay = 1f)
        {
            _reachedDistance = Mathf.Max(0.1f, reachedDistance);
            _repathDelay = Mathf.Max(0f, repathDelay);
        }

        public override void Enter(NpcContext context, GlobalNpcContext globalNpcContext)
        {
            _hasDestination = false;
            _nextRepathTime = 0f;
            _currentTarget = context.HidingPoint;
            _stuckCheckTimer = 0f;
            _lastPosition = context.Self.position;

            if (context?.Agent != null)
            {
                context.Agent.isStopped = false;
                context.Agent.updatePosition = false;
                context.Agent.updateRotation = false;
                context.Agent.autoBraking = false;
                context.Agent.avoidancePriority = Random.Range(0, 99);
                context.Agent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                TrySetDestination(context);
            }
        }

        public override void Tick(NpcContext context, GlobalNpcContext globalNpcContext)
        {
            if (context == null || context.Agent == null || context.CharacterController == null || !context.Agent.enabled)
                return;

            if (!context.Agent.isOnNavMesh)
            {
                StopCharacter(context);
                return;
            }

            context.Agent.nextPosition = context.Self.position;

            // ======== ЯВНАЯ ПРОВЕРКА ДОСТИЖЕНИЯ ========
            float distanceToTarget = Vector3.Distance(context.Self.position, _currentTarget);
            if (distanceToTarget <= _reachedDistance)
            {
                // Достигли цели – останавливаемся
                StopCharacter(context);
                _hasDestination = false;
                return;
            }

            if (context.HidingPoint != _currentTarget)
            {
                _currentTarget = context.HidingPoint;
                TrySetDestination(context);
                return;
            }

            // Проверка на застревание
            if (IsAgentStuck(context))
            {
                _hasDestination = false;
                _nextRepathTime = Time.time + 0.3f;
                TrySetDestination(context);
                return;
            }

            if (!_hasDestination && Time.time >= _nextRepathTime)
            {
                TrySetDestination(context);
                return;
            }

            if (context.Agent.pathPending || !context.Agent.hasPath)
            {
                TrySetDestination(context);
                return;
            }

            Vector3 desiredVelocity = Vector3.ProjectOnPlane(context.Agent.desiredVelocity, context.Self.up);
            if (desiredVelocity.sqrMagnitude < 0.01f)
            {
                StopCharacter(context);
                return;
            }

            float maxSpeed = context.Agent.speed;
            if (desiredVelocity.magnitude > maxSpeed)
                desiredVelocity = desiredVelocity.normalized * maxSpeed;

            INpcAnimator animator = context.AnimationPlayer;
            if (animator != null)
            {
                animator.SetState(NpcAnimationState.Run);
                animator.PlayMovement(desiredVelocity.magnitude);
            }

            var inputs = new AICharacterInputs
            {
                MoveVector = desiredVelocity.normalized,
                LookVector = desiredVelocity.normalized
            };
            context.CharacterController.SetInputs(ref inputs);
        }

        public override void Exit(NpcContext context, GlobalNpcContext globalNpcContext)
        {
            _hasDestination = false;
            _stuckCheckTimer = 0f;
            StopCharacter(context);

            if (context?.Agent != null && context.Agent.enabled && context.Agent.isOnNavMesh)
                context.Agent.ResetPath();
        }

        private bool IsAgentStuck(NpcContext context)
        {
            if (!_hasDestination) return false;
            if (context.Agent.remainingDistance <= _reachedDistance) return false;
            if (context.Agent.pathPending || !context.Agent.hasPath) return false;

            float speed = context.Agent.velocity.magnitude;
            if (speed > 0.3f)
            {
                _stuckCheckTimer = 0f;
                _lastPosition = context.Self.position;
                return false;
            }

            _stuckCheckTimer += Time.deltaTime;
            if (_stuckCheckTimer >= _stuckTimeThreshold)
            {
                float dist = Vector3.Distance(context.Self.position, _lastPosition);
                if (dist < _movementThreshold)
                    return true;
                else
                {
                    _lastPosition = context.Self.position;
                    _stuckCheckTimer = 0f;
                }
            }
            return false;
        }

        private void TrySetDestination(NpcContext context)
        {
            if (context?.Agent == null || !context.Agent.enabled || !context.Agent.isOnNavMesh)
                return;

            if (context.Agent.SetDestination(_currentTarget))
            {
                _hasDestination = true;
                _nextRepathTime = Time.time + _repathDelay;
                context.Agent.isStopped = false;
                _stuckCheckTimer = 0f;
                _lastPosition = context.Self.position;
            }
            else
            {
                _hasDestination = false;
                _nextRepathTime = Time.time + _repathDelay;
                StopCharacter(context);
            }
        }

        private void StopCharacter(NpcContext context)
        {
            if (context?.CharacterController == null)
                return;

            INpcAnimator animator = context.AnimationPlayer;
            if (animator != null)
            {
                animator.SetState(NpcAnimationState.Idle);
                animator.PlayMovement(0f);
            }

            var inputs = new AICharacterInputs
            {
                MoveVector = Vector3.zero,
                LookVector = context.Self.forward
            };
            context.CharacterController.SetInputs(ref inputs);
        }
    }
}