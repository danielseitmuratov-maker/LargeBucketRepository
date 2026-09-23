using UnityEngine;

namespace _Root._Scripts.Core.AI.Core
{
    public class GlobalNpcContext
    {
        public bool IsPrepareTimeOut { get; set; }
        public float TimeOfHiding { get; set; }
        public int MurderCount { get; set; }
        public float TimeRemaining { get; set; }
        public bool IsSheriffAlive { get; set; }
        public Vector3 SheriffDeathPosition { get; set; }
    }
}