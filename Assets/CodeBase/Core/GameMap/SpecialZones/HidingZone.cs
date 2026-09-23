using UnityEngine;
using UnityEngine.AI;

namespace _Root._Scripts.Core.GameMap.SpecialZones
{
    public class HidingZone : MonoBehaviour
    {
        [Header("Zone Settings")]
        [SerializeField] private float _searchRadius = 10f;
        [SerializeField] private int _maxAttempts = 30;

        public Vector3 GetRandomPointOnNavMesh()
        {
            Vector3 center = transform.position;
            
            for (int i = 0; i < _maxAttempts; i++)
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * _searchRadius;
                randomPoint.y = center.y; 
                
                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, _searchRadius, NavMesh.AllAreas))
                    return hit.position;
            }
            Debug.LogWarning($"Не удалось найти точку на NavMesh в зоне {name}");
            return center;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _searchRadius);
        }
    }
}