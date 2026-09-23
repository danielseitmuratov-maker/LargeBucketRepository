using System.Collections.Generic;
using UnityEngine;

namespace _Root._Scripts.Core.GameMap
{
    public interface IGameMapSpecialPointsProvider
    {
        Vector3 GetRandomHidingPoint();
        Vector3 GetRandomTreaseSpawnPoint();
        List<Vector3> GetHidingPoints();
        List<Vector3> GetTreasuresSpawnPoints();
    }
}