using System;
using System.Collections;
using KinematicCharacterController;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Teleporters
{
    public class СharacterTeleporter : ITeleporter, IDisposable
    {
        private readonly Transform _target;
        private readonly ICoroutineRunner _coroutineRunner;

        private Coroutine _delayRoutine;
        private KinematicCharacterMotor _motor;

        public СharacterTeleporter(Transform target, ICoroutineRunner coroutineRunner)
        {
            _target = target;
            _coroutineRunner = coroutineRunner;
        }

        public Vector3 Teleport(Vector3 to, float delay = 0f)
        {
            if (_target == null)
                return Vector3.zero;

            if (delay <= 0f || _coroutineRunner == null)
            {
                _target.position = to;
                return _target.position;
            }

            if (_delayRoutine != null)
                _coroutineRunner.StopCoroutine(_delayRoutine);

            _delayRoutine = _coroutineRunner.StartCoroutine(TeleportWithDelay(to, delay));

            return _target.position;
        }

        public Vector3 Teleport(Vector3 to)
        {
            return Teleport(to, 0f);
        }

        private IEnumerator TeleportWithDelay(Vector3 to, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_target != null) 
                _target.position = to;

            _delayRoutine = null;
        }
        

        public void Dispose()
        {
            if (_delayRoutine != null && _coroutineRunner != null)
            {
                _coroutineRunner.StopCoroutine(_delayRoutine);
                _delayRoutine = null;
            }
        }
    }
}