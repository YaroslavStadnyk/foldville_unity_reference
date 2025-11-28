using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Core.Attributes
{
    [Conditional("UNITY_EDITOR")]
    public class ShowOnlyAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
    public class ShowOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var valueStr = prop.propertyType switch
            {
                SerializedPropertyType.String => prop.stringValue,
                SerializedPropertyType.Boolean => prop.boolValue.ToString(),
                SerializedPropertyType.Integer => prop.intValue.ToString(),
                SerializedPropertyType.Float => prop.floatValue.ToString("0.0000"),
                SerializedPropertyType.Vector2 => prop.vector2Value.ToString(),
                SerializedPropertyType.Vector3 => prop.vector3Value.ToString(),
                _ => "(not supported)"
            };

            EditorGUI.LabelField(position, label.text, valueStr);
        }
    }

#endif
}