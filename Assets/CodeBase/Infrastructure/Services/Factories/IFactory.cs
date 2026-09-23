using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Factories
{
    public interface IFactory<out T>
    {
        T Create(Vector3 at, Transform parent = null);
        T CreateById(int id, Vector3 at, Transform parent = null);
    }
}