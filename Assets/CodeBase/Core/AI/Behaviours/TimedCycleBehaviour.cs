using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Core.AI.Core.Components.Animations;
using KinematicCharacterController.Examples;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Behaviours
{
    /// <summary>
    /// Поведение, которое управляет полным циклом:
    /// - патрулирование (бег по карте) до истечения времени подготовки,
    /// - бег в точку укрытия,
    /// - трансформация и стояние в укрытии до окончания времени укрытия,
    /// - обратная трансформация и возврат к патрулированию.
    /// </summary>
    public class TimedCycleBehaviour : NpcBehaviorBase
    {
        private enum Phase
        {
            Patrol,
            GoToHiding,
            Hiding
        }

        private readonly float _wanderRadius;
        private readonly float _reachedDistance;
        private readonly float _repathDelay;

        private Phase _currentPhase;
        private float _nextRepathTime;
        private bool _hasDestination;
        private float _hidingTimer;

        // Для патрулирования
        private float _patrolNextRepathTime;
        private bool _patrolHasDestination;

        public TimedCycleBehaviour(float wanderRadius, float reachedDistance = 0.5f, float repathDelay = 1f)
        {
            _wanderRadius = Mathf.Max(0.5f, wanderRadius);
            _reachedDistance = Mathf.Max(0.1f, reachedDistance);
            _repathDelay = Mathf.Max(0f, repathDelay);
        }

        public override void Enter(NpcContext context, GlobalNpcContext globalNpcContext)
        {
            _currentPhase = Phase.Patrol;
            _patrolHasDestination = false;
            _patrolNextRepathTime = 0f;
            _hidingTimer = 0f;

            if (context?.Agent != null)
            {
                context.Agent.isStopped = false;
                context.Agent.updatePosition = false;
                context.Agent.updateRotation = false;
                // Настройки избегания
                context.Agent.autoBraking = false;
                context.Agent.avoidancePriority = Random.Range(0, 99);
                context.Agent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            }

            SwitchToPatrol(context);
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

            // Синхронизация позиции агента с трансформом
            context.Agent.nextPosition = context.Self.position;

            switch (_currentPhase)
            {
                case Phase.Patrol:
                    TickPatrol(context);
                    if (globalNpcContext.IsPrepareTimeOut)
                        SwitchToGoToHiding(context);
                    break;

                case Phase.GoToHiding:
                    TickGoToHiding(context);
                    if (IsHidingPointReached(context))
                        SwitchToHiding(context);
                    break;

                case Phase.Hiding:
                    TickHiding(context, globalNpcContext);
                    break;
            }
        }

        public override void Exit(NpcContext context, GlobalNpcContext globalNpcContext)
        {
            _patrolHasDestination = false;
            _hasDestination = false;
            StopCharacter(context);

            if (context?.Agent != null && context.Agent.enabled && context.Agent.isOnNavMesh)
                context.Agent.ResetPath();

            if (context != null)
            {
                context.IsTransformed = false;
                SwitchModel(context, false);
            }
        }

        // ========== Патрулирование ==========
        private void TickPatrol(NpcContext context)
        {
            if (!_patrolHasDestination && Time.time >= _patrolNextRepathTime)
            {
                TrySetRandomPatrolDestination(context);
                return;
            }

            if (context.Agent.pathPending || !context.Agent.hasPath)
            {
                TrySetRandomPatrolDestination(context);
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

        private void TrySetRandomPatrolDestination(NpcContext context)
        {
            if (context == null || context.Agent == null || context.RandomNavMeshPointService == null ||
                !context.Agent.enabled || !context.Agent.isOnNavMesh)
                return;

            if (!context.RandomNavMeshPointService.TryGetRandomPoint(context.Self.position, _wanderRadius, out Vector3 target))
                return;

            if (context.Agent.SetDestination(target))
            {
                _patrolHasDestination = true;
                _patrolNextRepathTime = Time.time + _repathDelay;
                context.Agent.isStopped = false;
            }
            else
            {
                _patrolHasDestination = false;
                _patrolNextRepathTime = Time.time + _repathDelay;
                StopCharacter(context);
            }
        }

        // ========== Движение к укрытию ==========
        private void SwitchToGoToHiding(NpcContext context)
        {
            _currentPhase = Phase.GoToHiding;
            _hasDestination = false;
            _nextRepathTime = 0f;
            if (context != null)
                TrySetHidingDestination(context);
        }

        private void TickGoToHiding(NpcContext context)
        {
            if (!_hasDestination && Time.time >= _nextRepathTime)
            {
                TrySetHidingDestination(context);
                return;
            }

            if (context.Agent.pathPending || !context.Agent.hasPath)
            {
                TrySetHidingDestination(context);
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

        private void TrySetHidingDestination(NpcContext context)
        {
            if (context?.Agent == null || !context.Agent.enabled || !context.Agent.isOnNavMesh)
                return;

            if (context.Agent.SetDestination(context.HidingPoint))
            {
                _hasDestination = true;
                _nextRepathTime = Time.time + _repathDelay;
                context.Agent.isStopped = false;
            }
            else
            {
                _hasDestination = false;
                _nextRepathTime = Time.time + _repathDelay;
                StopCharacter(context);
            }
        }

        private bool IsHidingPointReached(NpcContext context)
        {
            return context.Agent.remainingDistance <= _reachedDistance;
        }

        // ========== Укрытие ==========
        private void SwitchToHiding(NpcContext context)
        {
            _currentPhase = Phase.Hiding;
            _hidingTimer = 0f;

            context.IsTransformed = true;
            context.IsInSpecialZone = true;
            SwitchModel(context, true);

            StopCharacter(context);
            context.Agent.ResetPath();

            INpcAnimator animator = context.AnimationPlayer;
            if (animator != null)
            {
                animator.SetState(NpcAnimationState.Idle);
                animator.PlayMovement(0f);
            }

            Debug.Log($"{context.Self.name} трансформировался и стоит в укрытии.");
        }

        private void TickHiding(NpcContext context, GlobalNpcContext globalNpcContext)
        {
            _hidingTimer += Time.deltaTime;
            if (_hidingTimer >= context.PersonSimulationTimeAmount)
            {
                context.IsTransformed = false;
                context.IsInSpecialZone = false;
                SwitchModel(context, false);

                _currentPhase = Phase.Patrol;
                _patrolHasDestination = false;
                _patrolNextRepathTime = 0f;
                SwitchToPatrol(context);

                Debug.Log($"{context.Self.name} вернулся к патрулированию.");
            }
        }

        // ========== Общие методы ==========
        private void SwitchToPatrol(NpcContext context)
        {
            _currentPhase = Phase.Patrol;
            _patrolHasDestination = false;
            _patrolNextRepathTime = 0f;

            if (context?.Agent != null)
            {
                context.Agent.isStopped = false;
                TrySetRandomPatrolDestination(context);
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

        private void SwitchModel(NpcContext context, bool transformed)
        {
            Transform normal = context.NormalModel;
            Transform transformedModel = context.TransformedModel;

            if (normal != null)
                normal.gameObject.SetActive(!transformed);
            if (transformedModel != null)
                transformedModel.gameObject.SetActive(transformed);
        }
    }
}