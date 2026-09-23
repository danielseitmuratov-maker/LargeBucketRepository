using _Root._Scripts.Core.AI.Core;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Behaviours
{
    public class FleeFromMurderBehaviour : NpcBehaviorBase
    {
        private readonly int _priority;
        private readonly float _fleeDistance;
        private readonly float _repathDelay;

        private float _nextRepathTime;

        public FleeFromMurderBehaviour(int priority, float fleeDistance, float repathDelay)
        {
            _priority = priority;
            _fleeDistance = fleeDistance;
            _repathDelay = repathDelay;
        }

        public override void Enter(NpcContext context,GlobalNpcContext globalNpcContext)
        {
            _nextRepathTime = 0f;
        }

        public override void Tick(NpcContext context,GlobalNpcContext globalNpcContext)
        {
            if (context.Agent == null ||
                !context.Agent.enabled ||
                !context.Agent.isOnNavMesh ||
                context.Murder == null)
            {
                return;
            }

            if (Time.time < _nextRepathTime)
                return;

            Vector3 awayDirection =
                context.Self.position - context.Murder.position;

            awayDirection.y = 0f;

            if (awayDirection.sqrMagnitude < 0.01f)
                awayDirection = Random.insideUnitSphere;

            awayDirection.Normalize();

            Vector3 fleePoint =
                context.Self.position + awayDirection * _fleeDistance;

            if (context.RandomNavMeshPointService.TryGetRandomPoint(
                    fleePoint,
                    _fleeDistance,
                    out Vector3 targetPoint))
            {
                context.Agent.SetDestination(targetPoint);
                _nextRepathTime = Time.time + _repathDelay;
            }
        }

        public override void Exit(NpcContext localContext, GlobalNpcContext globalContext)
        {
            
        }
    }
}