using UnityEngine;

namespace CodeBase.Infrastructure.Services.Factories.Npcs
{
    public interface INpcFactory<out T>
    {
        T Create(Vector3 at,Transform parent = null);
    }
}