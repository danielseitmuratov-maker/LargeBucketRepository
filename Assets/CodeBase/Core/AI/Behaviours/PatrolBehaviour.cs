using _Root._Scripts.Core.AI.Core;
using KinematicCharacterController.Examples;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Behaviours
{
    public class PatrolBehaviour : NpcBehaviorBase
    {
        private readonly int _priority;
        private readonly float _wanderRadius;
        private readonly float _reachedDistance;
        private readonly float _repathDelay;

        private float _nextRepathTime;
        private bool _hasDestination;

        public PatrolBehaviour(int priority, float wanderRadius, float reachedDistance = 0.5f, float repathDelay = 1f)
        {
            _priority = priority;
            _wanderRadius = Mathf.Max(0.5f, wanderRadius);
            _reachedDistance = Mathf.Max(0.1f, reachedDistance);
            _repathDelay = Mathf.Max(0f, repathDelay);
        }
        

        public override void Enter(NpcContext context,GlobalNpcContext globalNpcContext)
        {
            _nextRepathTime = 0f;
            _hasDestination = false;

            if (context?.Agent != null)
            {
                context.Agent.isStopped = false;
                context.Agent.updatePosition = false;
                context.Agent.updateRotation = false;
                TrySetRandomDestination(context);
            }
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

            context.Agent.nextPosition = context.Self.position;

            if (!_hasDestination && Time.time >= _nextRepathTime)
            {
                TrySetRandomDestination(context);
                return;
            }

            if (context.Agent.pathPending)
            {
                StopCharacter(context);
                return;
            }

            if (!context.Agent.hasPath)
            {
                TrySetRandomDestination(context);
                return;
            }

            Vector3 steeringTarget = context.Agent.steeringTarget;
            Vector3 direction = steeringTarget - context.Self.position;
            direction = Vector3.ProjectOnPlane(direction, context.Self.up);

            bool reached = context.Agent.remainingDistance <= _reachedDistance;
            if (reached)
            {
                TrySetRandomDestination(context);
                return;
            }

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
            _hasDestination = false;
            StopCharacter(context);

            if (context?.Agent != null && context.Agent.enabled && context.Agent.isOnNavMesh)
                context.Agent.ResetPath();
        }

        private void TrySetRandomDestination(NpcContext context)
        {
            if (context == null || context.Agent == null || context.RandomNavMeshPointService == null ||
                !context.Agent.enabled || !context.Agent.isOnNavMesh)
                return;

            if (!context.RandomNavMeshPointService.TryGetRandomPoint(context.Self.position, _wanderRadius, out Vector3 targetPoint))
                return;

            bool success = context.Agent.SetDestination(targetPoint);
            if (success)
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