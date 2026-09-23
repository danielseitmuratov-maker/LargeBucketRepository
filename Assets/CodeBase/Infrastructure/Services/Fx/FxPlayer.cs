using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Fx
{
    public class FxPlayer : IFxPlayer
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly Transform _poolParent;
        private readonly Queue<ParticleSystem> _pool = new Queue<ParticleSystem>();
        private readonly HashSet<ParticleSystem> _activeSystems = new HashSet<ParticleSystem>(); 

        private readonly Dictionary<ParticleSystem, ParticleSystem> _prefabMap = new Dictionary<ParticleSystem, ParticleSystem>();

        private readonly WaitForSeconds _checkInterval = new WaitForSeconds(0.1f);

        public FxPlayer(ICoroutineRunner coroutineRunner, Transform poolParent = null)
        {
            _coroutineRunner = coroutineRunner;
            _poolParent = poolParent ? poolParent : new GameObject("FxPool").transform;
        }


        public void PlayFx(ParticleSystem prefab) => 
            PlayFx(prefab, Vector3.zero, Quaternion.identity, null);

        public void PlayFx(ParticleSystem prefab, Vector3 position) => 
            PlayFx(prefab, position, Quaternion.identity, null);

        public void PlayFx(ParticleSystem prefab, Vector3 position, Quaternion rotation) => 
            PlayFx(prefab, position, rotation, null);

        public void PlayFx(ParticleSystem prefab, Transform parent) => 
            PlayFx(prefab, Vector3.zero, Quaternion.identity, parent);

        public void PlayFx(ParticleSystem prefab, Transform parent, Vector3 localPosition) => 
            PlayFx(prefab, localPosition, Quaternion.identity, parent);

        public void PlayFx(ParticleSystem prefab, Transform parent, Vector3 localPosition, Quaternion localRotation) => 
            PlayFx(prefab, localPosition, localRotation, parent);


        private void PlayFx(ParticleSystem prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (prefab == null) return;

            ParticleSystem system = GetFromPoolOrCreate(prefab);
            if (system == null) return;

            Transform systemTransform = system.transform;
            if (parent != null)
            {
                systemTransform.SetParent(parent, false);
                systemTransform.localPosition = position;
                systemTransform.localRotation = rotation;
            }
            else
            {
                systemTransform.SetParent(null, false);
                systemTransform.position = position;
                systemTransform.rotation = rotation;
            }

            system.gameObject.SetActive(true);
            system.Play();

            _activeSystems.Add(system);

            _coroutineRunner.StartCoroutine(AutoReturnRoutine(system));
        }


        public void PlayFxByRange(List<ParticleSystem> particleSystems, float delay = 0f)
        {
            PlayFxByRange(particleSystems, Vector3.zero, delay);
        }

        public void PlayFxByRange(List<ParticleSystem> particleSystems, Vector3 position, float delay = 0f)
        {
            if (particleSystems == null || particleSystems.Count == 0)
                return;

            foreach (var prefab in particleSystems)
            {
                if (prefab == null) continue;
                _coroutineRunner.StartCoroutine(PlayWithDelay(prefab, position, delay));
            }
        }

        private IEnumerator PlayWithDelay(ParticleSystem prefab, Vector3 position, float delay)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            PlayFx(prefab, position);
        }
        

        private ParticleSystem GetFromPoolOrCreate(ParticleSystem prefab)
        {
            if (_pool.Count > 0)
            {
                ParticleSystem system = _pool.Dequeue();
                if (system != null)
                {
                    system.transform.SetParent(null);
                    system.gameObject.SetActive(false);
                    system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    return system;
                }
            }

            ParticleSystem newSystem = Object.Instantiate(prefab, _poolParent);
            newSystem.gameObject.SetActive(false);
            _prefabMap[newSystem] = prefab;
            return newSystem;
        }

        private IEnumerator AutoReturnRoutine(ParticleSystem system)
        {
            while (system != null && system.isPlaying)
                yield return _checkInterval;

            if (system == null) yield break;

            if (_activeSystems.Contains(system))
            {
                StopFx(system);
            }
        }

        public void StopFx(ParticleSystem system)
        {
            if (system == null) 
                return;
            if (!_activeSystems.Contains(system)) 
                return;

            system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            system.gameObject.SetActive(false);

            _activeSystems.Remove(system);

            if (_prefabMap.ContainsKey(system))
            {
                system.transform.SetParent(_poolParent);
                _pool.Enqueue(system);
            }
            else
            {
                Object.Destroy(system.gameObject);
            }
        }

        public void ReturnAllToPool()
        {
            foreach (ParticleSystem system in new List<ParticleSystem>(_activeSystems)) 
                StopFx(system);
        }
    }
}