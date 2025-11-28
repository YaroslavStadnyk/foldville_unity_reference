using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Core.Configs
{
    public abstract class BaseConfig<T> : SerializedScriptableObject where T : ScriptableObject
    {
        private static readonly string Name = typeof(T).Name;
        private static readonly string Path = typeof(T).Assembly.GetName().Name.Replace('.', '/') + "/";

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = LoadInstance(Path, Name);
                    if (_instance == null)
                    {
                        _instance = CreateInstance<T>();
                        SaveInstance(_instance, Path, Name);
                    }
                }

                return _instance;
            }
        }

#if UNITY_EDITOR
        private const string AssetPath = "Assets/Resources/";
#endif

        private static T LoadInstance(string path, string name)
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<T>(AssetPath + path + name + ".asset");
#else
            return Resources.Load<T>(path + name);
#endif
        }

        private static void SaveInstance(T instance, string path, string name)
        {
#if UNITY_EDITOR
            if (!Directory.Exists(AssetPath + path))
            {
                Directory.CreateDirectory(AssetPath + path);
            }

            AssetDatabase.CreateAsset(instance, AssetPath + path + name + ".asset");
            AssetDatabase.Refresh();
#endif
        }

#if UNITY_EDITOR
        // [MenuItem("Category/Select Config")]
        protected static void SelectInstanceInEditor()
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = Instance;
        }
#endif
    }
}