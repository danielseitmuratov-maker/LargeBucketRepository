using System.Collections.Generic;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Fx
{
    public interface IFxPlayer
    {
        void PlayFx(ParticleSystem prefab);
        void PlayFx(ParticleSystem prefab, Vector3 position);
        void PlayFx(ParticleSystem prefab, Vector3 position, Quaternion rotation);
        void PlayFx(ParticleSystem prefab, Transform parent);
        void PlayFx(ParticleSystem prefab, Transform parent, Vector3 localPosition);
        void PlayFx(ParticleSystem prefab, Transform parent, Vector3 localPosition, Quaternion localRotation);
        void PlayFxByRange(List<ParticleSystem> particleSystems, float delay = 0f);
        void PlayFxByRange(List<ParticleSystem> particleSystems, Vector3 position, float delay = 0f);
        void StopFx(ParticleSystem system);
        void ReturnAllToPool();
    }
}