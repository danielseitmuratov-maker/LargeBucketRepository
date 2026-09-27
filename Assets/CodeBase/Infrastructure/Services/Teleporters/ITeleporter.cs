using UnityEngine;

namespace CodeBase.Infrastructure.Services.Teleporters
{
    public interface ITeleporter
    {
        Vector3 Teleport(Vector3 to, float delay = 0f);
        Vector3 Teleport(Vector3 to);
    }
}