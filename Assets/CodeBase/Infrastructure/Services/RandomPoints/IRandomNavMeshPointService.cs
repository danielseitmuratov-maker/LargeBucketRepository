using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.RandomPoints
{
    public interface IRandomNavMeshPointService
    {
        bool HasValidTriangulation { get; }

        void RebuildTriangulation();

        bool TryGetRandomPoint(
            out Vector3 point);

        bool TryGetRandomPoint(
            Vector3 center,
            float radius,
            out Vector3 point,
            int attempts = 10);

        bool TryGetRandomPointAtHeight(
            float height,
            out Vector3 point);
    }
}