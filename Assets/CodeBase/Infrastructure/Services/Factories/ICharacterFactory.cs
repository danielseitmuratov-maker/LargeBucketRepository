using _Root._Scripts.Core.Character;
using CodeBase.Core.Character;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.Factories
{
    public interface ICharacterFactory
    {
        public CharacterRoot Create(Vector3 at,Transform parent);
    }
}