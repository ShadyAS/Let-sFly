using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Surface)), CanEditMultipleObjects()]
public class AeroSurfaceEditor : Editor
{
    SerializedProperty config;
    SerializedProperty isControlSurface;
    SerializedProperty inputType;
    SerializedProperty inputMultiplyer;
    Surface surface;

    private void OnEnable()
    {
        config = serializedObject.FindProperty("config");
        isControlSurface = serializedObject.FindProperty("IsControlSurface");
        inputType = serializedObject.FindProperty("InputType");
        inputMultiplyer = serializedObject.FindProperty("InputMultiplyer");
        surface = target as Surface;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(config);
        EditorGUILayout.PropertyField(isControlSurface);
        if (surface.IsControlSurface)
        {
            EditorGUILayout.PropertyField(inputType);
            EditorGUILayout.PropertyField(inputMultiplyer);
        }
        serializedObject.ApplyModifiedProperties();
    }
}