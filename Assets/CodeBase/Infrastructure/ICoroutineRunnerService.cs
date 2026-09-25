using System.Collections;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public interface ICoroutineRunnerService
    {
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine coroutine);
    }
}