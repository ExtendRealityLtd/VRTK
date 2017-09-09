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

        SerializedProperty clipOnTouch;
        SerializedProperty strengthOnTouch;
        SerializedProperty durationOnTouch;
        SerializedProperty intervalOnTouch;

        SerializedProperty clipOnGrab;
        SerializedProperty strengthOnGrab;
        SerializedProperty durationOnGrab;
        SerializedProperty intervalOnGrab;

        SerializedProperty clipOnUse;
        SerializedProperty strengthOnUse;
        SerializedProperty durationOnUse;
        SerializedProperty intervalOnUse;

        SerializedProperty objectToAffect;

        private void OnEnable()
        {
            clipOnNearTouch = serializedObject.FindProperty("clipOnNearTouch");
            strengthOnNearTouch = serializedObject.FindProperty("strengthOnNearTouch");
            durationOnNearTouch = serializedObject.FindProperty("durationOnNearTouch");
            intervalOnNearTouch = serializedObject.FindProperty("intervalOnNearTouch");

            clipOnTouch = serializedObject.FindProperty("clipOnTouch");
            strengthOnTouch = serializedObject.FindProperty("strengthOnTouch");
            durationOnTouch = serializedObject.FindProperty("durationOnTouch");
            intervalOnTouch = serializedObject.FindProperty("intervalOnTouch");

            clipOnGrab = serializedObject.FindProperty("clipOnGrab");
            strengthOnGrab = serializedObject.FindProperty("strengthOnGrab");
            durationOnGrab = serializedObject.FindProperty("durationOnGrab");
            intervalOnGrab = serializedObject.FindProperty("intervalOnGrab");

            clipOnUse = serializedObject.FindProperty("clipOnUse");
            strengthOnUse = serializedObject.FindProperty("strengthOnUse");
            durationOnUse = serializedObject.FindProperty("durationOnUse");
            intervalOnUse = serializedObject.FindProperty("intervalOnUse");

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

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Haptics On Touch Settings", EditorStyles.boldLabel);

            EditorGUILayout.ObjectField(clipOnTouch, typeof(AudioClip));
            if (clipOnTouch.objectReferenceValue == null)
            {
                EditorGUILayout.PropertyField(strengthOnTouch);
                EditorGUILayout.PropertyField(durationOnTouch);
                EditorGUILayout.PropertyField(intervalOnTouch);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Haptics On Grab Settings", EditorStyles.boldLabel);

            EditorGUILayout.ObjectField(clipOnGrab, typeof(AudioClip));
            if (clipOnGrab.objectReferenceValue == null)
            {
                EditorGUILayout.PropertyField(strengthOnGrab);
                EditorGUILayout.PropertyField(durationOnGrab);
                EditorGUILayout.PropertyField(intervalOnGrab);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Haptics On Use Settings", EditorStyles.boldLabel);

            EditorGUILayout.ObjectField(clipOnUse, typeof(AudioClip));
            if (clipOnUse.objectReferenceValue == null)
            {
                EditorGUILayout.PropertyField(strengthOnUse);
                EditorGUILayout.PropertyField(durationOnUse);
                EditorGUILayout.PropertyField(intervalOnUse);
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(objectToAffect);

            serializedObject.ApplyModifiedProperties();
        }
    }
}