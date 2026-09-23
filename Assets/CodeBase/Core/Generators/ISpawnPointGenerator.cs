using UnityEngine;

namespace _Root._Scripts.Core.Generators
{
    public interface ISpawnPointGenerator
    {
        Vector3 GetStartSpawnPoint();
        Vector3 GetNextSpawnPoint();

        void Reset();
    }
}