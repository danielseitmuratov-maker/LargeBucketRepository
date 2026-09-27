using _Root._Scripts.Core.AI.Npcs;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.Factories.Npcs
{
    public interface ILobbyNpcFactory
    {
        LobbyNpcRoot Create(Vector3 at, Transform parent);
    }
}