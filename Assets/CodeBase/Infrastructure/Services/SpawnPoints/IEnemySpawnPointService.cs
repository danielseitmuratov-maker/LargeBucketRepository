using System.Collections.Generic;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.SpawnPoints
{
    public interface IEnemySpawnPointService
    {
        List<Vector3> GenerateSpawnPoints(EnemyFormationType formationType, EnemySpawnFormationParams formationParams);

        List<Vector3> GenerateCircleFormation(Vector3 center, float radius, int count);

        List<Vector3> GenerateNoiseFormation(Vector3 center, Vector2 areaSize, float noiseScale, int count);

        List<Vector3> GenerateHexagonFormation(Vector3 center, float cellRadius, int ringCount);
    }
}