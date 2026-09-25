using System.Collections;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public class CoroutineRunnerService : ICoroutineRunnerService
    {
        private readonly MonoBehaviour _monoBehaviour;

        public CoroutineRunnerService(MonoBehaviour monoBehaviour) => 
            _monoBehaviour = monoBehaviour;

        public Coroutine StartCoroutine(IEnumerator routine) => _monoBehaviour.StartCoroutine(routine);

        public void StopCoroutine(Coroutine coroutine) => 
           _monoBehaviour.StopCoroutine(coroutine);
    }
}