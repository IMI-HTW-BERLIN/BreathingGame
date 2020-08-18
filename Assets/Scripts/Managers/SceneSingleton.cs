using System;
using UnityEngine;

namespace Managers
{
    public abstract class SceneSingleton<T> : MonoBehaviour where T : class
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    throw new NullReferenceException($"The {typeof(T).Name} is not in this Scene.");
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null)
                Destroy(gameObject);
            else
                _instance = this as T;
        }
    }
}