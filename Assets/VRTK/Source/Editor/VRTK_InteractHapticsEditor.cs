namespace VRTK
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(VRTK_InteractHaptics))]
    public class VRTK_InteractHapticsEditor : Editor
    {
        SerializedProperty clipOnNearTouch;
        SerializedProperty strengthOnNearTouch;
        SerializedProperty durationOnNearTouch;
        SerializedProperty intervalOnNearTouch;
        SerializedProperty cancelOnNearUntouch;

        SerializedProperty clipOnTouch;
        SerializedProperty strengthOnTouch;
        SerializedProperty durationOnTouch;
        SerializedProperty intervalOnTouch;
        SerializedProperty cancelOnUntouch;

        SerializedProperty clipOnGrab;
        SerializedProperty strengthOnGrab;
        SerializedProperty durationOnGrab;
        SerializedProperty intervalOnGrab;
        SerializedProperty cancelOnUngrab;

        SerializedProperty clipOnUse;
        SerializedProperty strengthOnUse;
        SerializedProperty durationOnUse;
        SerializedProperty intervalOnUse;
        SerializedProperty cancelOnUnuse;

        SerializedProperty objectToAffect;

        private void OnEnable()
        {
            clipOnNearTouch = serializedObject.FindProperty("clipOnNearTouch");
            strengthOnNearTouch = serializedObject.FindProperty("strengthOnNearTouch");
            durationOnNearTouch = serializedObject.FindProperty("durationOnNearTouch");
            intervalOnNearTouch = serializedObject.FindProperty("intervalOnNearTouch");
            cancelOnNearUntouch = serializedObject.FindProperty("cancelOnNearUntouch");

            clipOnTouch = serializedObject.FindProperty("clipOnTouch");
            strengthOnTouch = serializedObject.FindProperty("strengthOnTouch");
            durationOnTouch = serializedObject.FindProperty("durationOnTouch");
            intervalOnTouch = serializedObject.FindProperty("intervalOnTouch");
            cancelOnUntouch = serializedObject.FindProperty("cancelOnUntouch");

            clipOnGrab = serializedObject.FindProperty("clipOnGrab");
            strengthOnGrab = serializedObject.FindProperty("strengthOnGrab");
            durationOnGrab = serializedObject.FindProperty("durationOnGrab");
            intervalOnGrab = serializedObject.FindProperty("intervalOnGrab");
            cancelOnUngrab = serializedObject.FindProperty("cancelOnUngrab");

            clipOnUse = serializedObject.FindProperty("clipOnUse");
            strengthOnUse = serializedObject.FindProperty("strengthOnUse");
            durationOnUse = serializedObject.FindProperty("durationOnUse");
            intervalOnUse = serializedObject.FindProperty("intervalOnUse");
            cancelOnUnuse = serializedObject.FindProperty("cancelOnUnuse");

            objectToAffect = serializedObject.FindProperty("objectToAffect");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Haptics On Near Touch Settings", EditorStyles.boldLabel);

            EditorGUILayout.ObjectField(clipOnNearTouch, typeof(AudioClip));
            if (clipOnNearTouch.objectReferenceValue == null)
            {
                EditorGUILayout.PropertyField(strengthOnNearTouch);
                EditorGUILayout.PropertyField(durationOnNearTouch);
                EditorGUILayout.PropertyField(intervalOnNearTouch);
            }
            EditorGUILayout.PropertyField(cancelOnNearUntouch);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Haptics On Touch Settings", EditorStyles.boldLabel);

            EditorGUILayout.ObjectField(clipOnTouch, typeof(AudioClip));
            if (clipOnTouch.objectReferenceValue == null)
            {
                EditorGUILayout.PropertyField(strengthOnTouch);
                EditorGUILayout.PropertyField(durationOnTouch);
                EditorGUILayout.PropertyField(intervalOnTouch);
            }
            EditorGUILayout.PropertyField(cancelOnUntouch);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Haptics On Grab Settings", EditorStyles.boldLabel);

            EditorGUILayout.ObjectField(clipOnGrab, typeof(AudioClip));
            if (clipOnGrab.objectReferenceValue == null)
            {
                EditorGUILayout.PropertyField(strengthOnGrab);
                EditorGUILayout.PropertyField(durationOnGrab);
                EditorGUILayout.PropertyField(intervalOnGrab);
            }
            EditorGUILayout.PropertyField(cancelOnUngrab);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Haptics On Use Settings", EditorStyles.boldLabel);

            EditorGUILayout.ObjectField(clipOnUse, typeof(AudioClip));
            if (clipOnUse.objectReferenceValue == null)
            {
                EditorGUILayout.PropertyField(strengthOnUse);
                EditorGUILayout.PropertyField(durationOnUse);
                EditorGUILayout.PropertyField(intervalOnUse);
            }
            EditorGUILayout.PropertyField(cancelOnUnuse);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(objectToAffect);

            serializedObject.ApplyModifiedProperties();
        }
    }
}