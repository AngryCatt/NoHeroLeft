using HeroLeft.BattleLogic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(PassiveSkill))]
public class PassiveSkillDrawer : PropertyDrawer {

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var firstLineRect = new Rect(
            x: rect.x,
            y: rect.y,
            width: rect.width,
            height: EditorGUIUtility.singleLineHeight
            );

        EditorGUI.PropertyField(rect, property.FindPropertyRelative("IsPassiveSkill"), label);
        if (property.FindPropertyRelative("IsPassiveSkill").boolValue == true) {
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("HaveChanse"), new GUIContent("HaveChanse"));
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("chanse"), new GUIContent("Chanse"));
        }

        EditorGUI.indentLevel = indent;
    }

}
#endif