using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public static class Coroutines
    {
        public static IEnumerator WaitForSeconds(float time, UnityAction onFinish)
        {
            yield return new WaitForSeconds(time);
            onFinish?.Invoke();
        }

        public static IEnumerator WaitUntil(Func<bool> predict, UnityAction onFinish)
        {
            yield return new WaitUntil(predict);
            onFinish?.Invoke();
        }
    }
}