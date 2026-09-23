// NpcContext.cs
using _Root._Scripts.Core.AI.Core.Components;
using _Root._Scripts.Core.AI.Core.Components.Animations;
using _Root._Scripts.Infrastructure.Services.RandomPoints;
using KinematicCharacterController.Examples;
using UnityEngine;
using UnityEngine.AI;

namespace _Root._Scripts.Core.AI.Core
{
    public class NpcContext
    {
        public Transform Self;
        public NavMeshAgent Agent;
        public ExampleCharacterController CharacterController;
        public INpcAnimator AnimationPlayer;
        public Animator Animator;
        public NpcView View;

        public Transform Target;
        public Transform Murder;

        public Vector3 LastKnownTargetPosition;

        public float DetectionRadius;
        public float ShootRadius;
        public float FleeRadius;

        public float WanderRadius;
        public float WanderPointReachedDistance;

        public bool IsTransformed;
        public bool HasWeapon;

        public LayerMask PlayerMask;
        public LayerMask MurderMask;

        public IRandomNavMeshPointService RandomNavMeshPointService;

        public Vector3 HidingPoint;
        public float PersonSimulationTimeAmount;
        public bool IsInSpecialZone;

        public float HidingTimer;
        public bool IsPreparePhaseComplete; 

        public Transform NormalModel;
        public Transform TransformedModel;
    }
}