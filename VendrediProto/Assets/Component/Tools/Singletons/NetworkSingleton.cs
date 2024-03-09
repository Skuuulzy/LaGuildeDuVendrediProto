using Unity.Netcode;
using UnityEngine;

namespace VComponent.Tools.Singletons
{
    /// <summary>
    /// A simple singleton implementation.
    /// </summary>
    public class NetworkSingleton<T> : NetworkBehaviour where T : Component
    {
        private static T _instance;

        public static bool HasInstance => _instance != null;
        public static T TryGetInstance() => HasInstance ? _instance : null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                    if (_instance == null)
                    {
                        Debug.LogError($"There was no instance of of {typeof(T).Name} and yet someone try to call it. An new instance has been set but singletons should always be present.");
                        var go = new GameObject(typeof(T).Name + " Auto-Generated");
                        _instance = go.AddComponent<T>();
                        go.AddComponent<NetworkObject>();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying) return;

            _instance = this as T;
        }
    }
}