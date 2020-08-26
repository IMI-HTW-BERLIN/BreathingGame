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
    }
}