using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Core.Attributes
{
    [Conditional("UNITY_EDITOR")]
    public class ScenePathAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ScenePathAttribute))]
    public class SceneNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                var sceneObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
                if (sceneObject == null && !string.IsNullOrWhiteSpace(property.stringValue))
                {
                    sceneObject = GetBuildSettingsSceneObject(property.stringValue);
                    if (sceneObject == null && !string.IsNullOrWhiteSpace(property.stringValue))
                    {
                        Debug.LogError($"Scene {property.stringValue} in {property.propertyPath} not found.");
                    }
                }

                var scene = (SceneAsset)EditorGUI.ObjectField(position, label, sceneObject, typeof(SceneAsset), true);
                property.stringValue = AssetDatabase.GetAssetPath(scene);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use [Scene] with strings.");
            }
        }

        private static SceneAsset GetBuildSettingsSceneObject(string sceneName)
        {
            foreach (var buildScene in EditorBuildSettings.scenes)
            {
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(buildScene.path);
                if (sceneAsset != null && sceneAsset.name == sceneName)
                {
                    return sceneAsset;
                }
            }

            return null;
        }
    }

#endif
}