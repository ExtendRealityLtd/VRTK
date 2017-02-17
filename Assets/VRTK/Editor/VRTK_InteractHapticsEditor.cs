namespace VRTK
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(VRTK_InteractHaptics))]
    public class VRTK_InteractHapticsEditor : Editor
    {
        SerializedProperty audioClipTouch;
        SerializedProperty strengthOnTouch;
        SerializedProperty durationOnTouch;
        SerializedProperty intervalOnTouch;

        SerializedProperty audioClipGrab;
        SerializedProperty strengthOnGrab;
        SerializedProperty durationOnGrab;
        SerializedProperty intervalOnGrab;

        SerializedProperty audioClipUse;
        SerializedProperty strengthOnUse;
        SerializedProperty durationOnUse;
        SerializedProperty intervalOnUse;


        private void OnEnable()
        {
            audioClipTouch = serializedObject.FindProperty("audioClipTouch");
            strengthOnTouch = serializedObject.FindProperty("strengthOnTouch");
            durationOnTouch = serializedObject.FindProperty("durationOnTouch");
            intervalOnTouch = serializedObject.FindProperty("intervalOnTouch");

            audioClipGrab = serializedObject.FindProperty("audioClipGrab");
            strengthOnGrab = serializedObject.FindProperty("strengthOnGrab");
            durationOnGrab = serializedObject.FindProperty("durationOnGrab");
            intervalOnGrab = serializedObject.FindProperty("intervalOnGrab");

            audioClipUse = serializedObject.FindProperty("audioClipUse");
            strengthOnUse = serializedObject.FindProperty("strengthOnUse");
            durationOnUse = serializedObject.FindProperty("durationOnUse");
            intervalOnUse = serializedObject.FindProperty("intervalOnUse");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.ObjectField(audioClipTouch, typeof(AudioClip));
            EditorGUILayout.PropertyField(strengthOnTouch);
            if(audioClipTouch.objectReferenceValue == null)
            {
                EditorGUILayout.PropertyField(durationOnTouch);
                EditorGUILayout.PropertyField(intervalOnTouch);
            }

            EditorGUILayout.ObjectField(audioClipGrab, typeof(AudioClip));
            EditorGUILayout.PropertyField(strengthOnGrab);
            if(audioClipGrab.objectReferenceValue == null)
            {
                EditorGUILayout.PropertyField(durationOnGrab);
                EditorGUILayout.PropertyField(intervalOnGrab);
            }

            EditorGUILayout.ObjectField(audioClipUse, typeof(AudioClip));
            EditorGUILayout.PropertyField(strengthOnUse);
            if(audioClipUse.objectReferenceValue == null)
            {
                EditorGUILayout.PropertyField(durationOnUse);
                EditorGUILayout.PropertyField(intervalOnUse);
            }

            serializedObject.ApplyModifiedProperties();
        }
	}
}