using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.Base
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get => _instance;
        }
        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            gameObject.name = typeof(T).FullName;
            _instance = this as T;
        }
    }

    public class ServiceSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get => _instance;
        }
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            gameObject.name = typeof(T).FullName;
            _instance = this as T;
            DontDestroyOnLoad(this);
        }
    }
}

