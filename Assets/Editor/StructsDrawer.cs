using UnityEditor;
using UnityEngine;
using HeroLeft.Interfaces;
using System;
#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(SafeFloat))]
public class SafeFloatDrawer : PropertyDrawer {
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var firstLineRect = new Rect(
            x: rect.x,
            y: rect.y,
            width: rect.width,
            height: EditorGUIUtility.singleLineHeight
            );
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("value"), label);

        EditorGUI.indentLevel = indent;
    }
}

[CustomPropertyDrawer(typeof(SafeInt))]
public class SafeIntDrawer : PropertyDrawer {

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var firstLineRect = new Rect(
            x: rect.x,
            y: rect.y,
            width: rect.width,
            height: EditorGUIUtility.singleLineHeight
            );
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("value"), label);

        EditorGUI.indentLevel = indent;
    }
}
[CustomPropertyDrawer(typeof(HeroLeft.Interfaces.RangeAttribute))]
public class RangeSafeDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // First get the attribute since it contains the range for the slider
        HeroLeft.Interfaces.RangeAttribute range = attribute as HeroLeft.Interfaces.RangeAttribute;

        // Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
        if (property.propertyType == SerializedPropertyType.Float)
            EditorGUI.Slider(position, property, range.min, range.max, label);
        else if (property.propertyType == SerializedPropertyType.Integer)
            EditorGUI.IntSlider(position, property, Convert.ToInt32(range.min), Convert.ToInt32(range.max), label);
        else if (property.type == "SafeInt")
            EditorGUI.IntSlider(position, property.FindPropertyRelative("value"), Convert.ToInt32(range.min), Convert.ToInt32(range.max), label);
        else
            EditorGUI.LabelField(position, label.text, property.type.ToString());
    }
}
#endif