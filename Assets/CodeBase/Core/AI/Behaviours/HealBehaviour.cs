using _Root._Scripts.Core.AI.Core;
using _Root._Scripts.Infrastructure.Services.Interfaces;
using KinematicCharacterController.Examples;
using UnityEngine;

namespace _Root._Scripts.Core.AI.Behaviours
{
    public class HealBehaviour : NpcBehaviorBase
    {
        private readonly int _priority;
        private readonly float _healRadius;
        private readonly float _healAmount;
        private readonly float _cooldown;
        private float _lastHealTime;

        public HealBehaviour(int priority, float healRadius = 5f, float healAmount = 20f, float cooldown = 3f)
        {
            _priority = priority;
            _healRadius = healRadius;
            _healAmount = healAmount;
            _cooldown = cooldown;
        }

        public override void Enter(NpcContext context,GlobalNpcContext globalNpcContext)
        {
            // Найти ближайшего раненого и установить как цель
            Transform wounded = FindWoundedAlly(context);
            if (wounded != null)
            {
                context.Target = wounded;
                // Двигаемся к нему
                context.Agent.SetDestination(wounded.position);
            }
        }

        public override void Tick(NpcContext context,GlobalNpcContext globalNpcContext)
        {
            if (context == null || context.Agent == null || context.CharacterController == null)
                return;

            if (context.Target == null)
                return;

            float distance = Vector3.Distance(context.Self.position, context.Target.position);

            // Если цель рядом – лечим
            if (distance <= _healRadius && Time.time >= _lastHealTime + _cooldown)
            {
                PerformHeal(context);
                _lastHealTime = Time.time;
            }
            else if (distance > _healRadius)
            {
                // Идём к цели
                context.Agent.SetDestination(context.Target.position);
                // Двигаем персонажа
                Vector3 direction = (context.Target.position - context.Self.position).normalized;
                var inputs = new AICharacterInputs
                {
                    MoveVector = direction,
                    LookVector = direction
                };
                context.CharacterController.SetInputs(ref inputs);
            }
            else
            {
                // Стоим на месте
                StopCharacter(context);
            }
        }

        public override void Exit(NpcContext context,GlobalNpcContext globalNpcContext)
        {
            if (context?.Agent != null)
                context.Agent.ResetPath();

            context.Target = null;
            StopCharacter(context);
        }

        private bool IsWoundedAllyNearby(NpcContext context)
        {
            Collider[] hits = Physics.OverlapSphere(context.Self.position, _healRadius, context.PlayerMask);
            foreach (var hit in hits)
            {
                var health = hit.GetComponent<IHealth>();
                if (health != null && health.CurrentHealth < health.MaxHealth * 0.7f)
                    return true;
            }
            return false;
        }

        private Transform FindWoundedAlly(NpcContext context)
        {
            Collider[] hits = Physics.OverlapSphere(context.Self.position, _healRadius * 2f, context.PlayerMask);
            Transform closest = null;
            float minDist = float.MaxValue;

            foreach (var hit in hits)
            {
                var health = hit.GetComponent<IHealth>();
                if (health != null && health.CurrentHealth < health.MaxHealth * 0.7f)
                {
                    float dist = Vector3.Distance(context.Self.position, hit.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = hit.transform;
                    }
                }
            }
            return closest;
        }

        private void PerformHeal(NpcContext context)
        {
            var health = context.Target.GetComponent<IHealth>();
            if (health != null)
            {
                health.Heal(_healAmount);
                Debug.Log($"{context.Self.name} вылечил {context.Target.name} на {_healAmount} HP.");
                // Запустить анимацию лечения
                context.AnimationPlayer?.PlayHeal();
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