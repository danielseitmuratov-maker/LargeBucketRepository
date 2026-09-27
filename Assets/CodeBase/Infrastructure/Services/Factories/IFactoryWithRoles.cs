using _Root._Scripts.Core.Roles;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.Factories
{
    public interface IFactoryWithRoles<T>
    {
        T Create(Vector3 at, Transform parent = null);
        T CreateById(int id, Vector3 at, Transform parent = null);
        T CreateWithRole(Vector3 at, GameRole role, Transform parent = null);
    }
}