using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Core.Attributes
{
    [Conditional("UNITY_EDITOR")]
    public class NamedArrayAttribute : PropertyAttribute
    {
        public readonly string[] Names;

        public NamedArrayAttribute(params string[] names)
        {
            Names = names;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(NamedArrayAttribute))]
    public class NamedArrayDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            try
            {
                var index = int.Parse(prop.propertyPath.Split('[', ']')[1]);
                var names = ((NamedArrayAttribute)attribute).Names;
                EditorGUI.ObjectField(position, prop, new GUIContent(names[index]));
            }
            catch
            {
                EditorGUI.ObjectField(position, prop, label);
            }
        }
    }

#endif
}