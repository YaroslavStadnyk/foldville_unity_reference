using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Ordinaries
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    SetupInstance();
                }

                return _instance;
            }
        }

        [BoxGroup("Singleton")] [SerializeField] private bool dontDestroyOnLoad = true;
        [BoxGroup("Singleton")] [SerializeField] private bool detachFromParents = true;

        public virtual void Awake()
        {
            RemoveDuplicates();

            if (_instance == this)
            {
                SetupProperties();
            }
        }

        private static void SetupInstance()
        {
            _instance = FindObjectOfType<T>();
            if (_instance != null)
            {
                return;
            }

            var gameObject = new GameObject(typeof(T).Name);
            _instance = gameObject.AddComponent<T>();

            Debug.LogWarning(typeof(T) + " instance created automatically.");
        }

        private void SetupProperties()
        {
            if (detachFromParents && transform.parent != null)
            {
                transform.parent = null;
            }

            if (dontDestroyOnLoad && gameObject.scene.name != "DontDestroyOnLoad")
            {
                DontDestroyOnLoad(transform.parent == null ? gameObject : transform.parent.gameObject);
            }
        }

        private void RemoveDuplicates()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}