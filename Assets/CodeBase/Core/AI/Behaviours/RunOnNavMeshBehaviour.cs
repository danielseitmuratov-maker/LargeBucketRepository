using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Core.AI.Core.Components.Animations;
using KinematicCharacterController.Examples;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Behaviours
{
    public class RunOnNavMeshBehaviour : NpcBehaviorBase
    {
        private readonly float _wanderRadius;
        private readonly float _reachedDistance;
        private readonly float _repathDelay;

        private float _nextRepathTime;
        private bool _hasDestination;

        // Поля для детекции застревания
        private float _stuckCheckTimer;
        private Vector3 _lastPosition;
        private float _stuckTimeThreshold = 1.5f;
        private float _movementThreshold = 0.1f;

        public RunOnNavMeshBehaviour(float wanderRadius, float reachedDistance, float repathDelay)
        {
            _wanderRadius = Mathf.Max(0.5f, wanderRadius);
            _reachedDistance = Mathf.Max(0.1f, reachedDistance);
            _repathDelay = Mathf.Max(0f, repathDelay);
        }

        public override void Enter(NpcContext context, GlobalNpcContext globalNpcContext)
        {
            _nextRepathTime = 0f;
            _hasDestination = false;
            _stuckCheckTimer = 0f;
            _lastPosition = context.Self.position;

            if (context?.Agent == null) return;

            context.Agent.isStopped = false;
            context.Agent.updatePosition = false;
            context.Agent.updateRotation = false;
            context.Agent.autoBraking = false;
            context.Agent.avoidancePriority = Random.Range(0, 99);
            context.Agent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;

            TrySetRandomDestination(context);
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

            // Проверка на застревание
            if (IsAgentStuck(context))
            {
                _hasDestination = false;
                _nextRepathTime = Time.time + 0.3f;
                TrySetRandomDestination(context);
                return;
            }

            if (!_hasDestination && Time.time >= _nextRepathTime)
            {
                TrySetRandomDestination(context);
                return;
            }

            if (context.Agent.pathPending || !context.Agent.hasPath)
            {
                TrySetRandomDestination(context);
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

            PlayRunAnimation(context, desiredVelocity.magnitude);

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
            // Если нет цели или достигли – не застряли
            if (!_hasDestination) return false;
            if (context.Agent.remainingDistance <= _reachedDistance) return false;
            if (context.Agent.pathPending || !context.Agent.hasPath) return false;

            float speed = context.Agent.velocity.magnitude;
            if (speed > 0.3f) // если движется – сбрасываем таймер
            {
                _stuckCheckTimer = 0f;
                _lastPosition = context.Self.position;
                return false;
            }

            // Если скорость низкая, проверяем изменение позиции
            _stuckCheckTimer += Time.deltaTime;
            if (_stuckCheckTimer >= _stuckTimeThreshold)
            {
                float dist = Vector3.Distance(context.Self.position, _lastPosition);
                if (dist < _movementThreshold)
                    return true;
                else
                {
                    // обновляем позицию и сбрасываем таймер
                    _lastPosition = context.Self.position;
                    _stuckCheckTimer = 0f;
                }
            }
            return false;
        }

        private void TrySetRandomDestination(NpcContext context)
        {
            if (context == null || context.Agent == null || context.RandomNavMeshPointService == null ||
                !context.Agent.enabled || !context.Agent.isOnNavMesh)
                return;

            if (!context.RandomNavMeshPointService.TryGetRandomPoint(context.Self.position, _wanderRadius, out Vector3 target))
                return;

            if (context.Agent.SetDestination(target))
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

        private void PlayRunAnimation(NpcContext context, float speed)
        {
            if (speed < 0.1f) speed = 1f;
            INpcAnimator animator = context.AnimationPlayer;
            if (animator != null)
            {
                animator.SetState(NpcAnimationState.Run);
                animator.PlayMovement(speed);
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