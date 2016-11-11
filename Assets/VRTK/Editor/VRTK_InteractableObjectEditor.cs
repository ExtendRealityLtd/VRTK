namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;

    [CustomEditor(typeof(VRTK_InteractableObject), true), CanEditMultipleObjects]
    public class VRTK_InteractableObjectEditor : Editor
    {
        private bool viewTouch = true;
        private bool viewGrab = false;
        private bool viewUse = false;
        private bool viewCustom = true;
        private bool isGrabbableLastState = false;
        private bool isUsableLastState = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            VRTK_InteractableObject targ = (VRTK_InteractableObject)target;
            GUILayout.Space(10);
            GUIStyle guiStyle = EditorStyles.foldout;
            FontStyle previousStyle = guiStyle.fontStyle;
            guiStyle.fontStyle = FontStyle.Bold;
            viewTouch = EditorGUILayout.Foldout(viewTouch, "Touch Options", guiStyle);
            guiStyle.fontStyle = previousStyle;
            GUILayout.Space(2);
            if (viewTouch)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("touchHighlightColor"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("allowedTouchControllers"));
                EditorGUI.indentLevel--;
            }

            //Grab Layout
            GUILayout.Space(10);
            targ.isGrabbable = EditorGUILayout.Toggle(VRTK_EditorUtilities.BuildGUIContent<VRTK_InteractableObject>("isGrabbable"), targ.isGrabbable);
            if (targ.isGrabbable != isGrabbableLastState && targ.isGrabbable)
            {
                viewGrab = true;
            }
            isGrabbableLastState = targ.isGrabbable;
            if (targ.isGrabbable)
            {
                guiStyle = EditorStyles.foldout;
                previousStyle = guiStyle.fontStyle;
                guiStyle.fontStyle = FontStyle.Bold;
                viewGrab = EditorGUILayout.Foldout(viewGrab, "Grab Options", guiStyle);
                guiStyle.fontStyle = previousStyle;
                GUILayout.Space(2);
                if (viewGrab)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("holdButtonToGrab"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("stayGrabbedOnTeleport"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("validDrop"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("grabOverrideButton"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("allowedGrabControllers"));
                    targ.secondaryGrabAction = (VRTK_InteractableObject.SecondaryControllerActions)EditorGUILayout.EnumPopup(VRTK_EditorUtilities.BuildGUIContent<VRTK_InteractableObject>("secondaryGrabAction"), targ.secondaryGrabAction);
                    if (targ.secondaryGrabAction == VRTK_InteractableObject.SecondaryControllerActions.Custom_Action)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("customSecondaryAction"));
                    }
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("grabAttachMechanicScript"));
                    EditorGUI.indentLevel--;
                }
            }

            GUILayout.Space(10);
            targ.isUsable = EditorGUILayout.Toggle(VRTK_EditorUtilities.BuildGUIContent<VRTK_InteractableObject>("isUsable"), targ.isUsable);
            if (targ.isUsable != isUsableLastState && targ.isUsable)
            {
                viewUse = true;
            }

            isUsableLastState = targ.isUsable;
            if (targ.isUsable)
            {
                guiStyle = EditorStyles.foldout;
                previousStyle = guiStyle.fontStyle;
                guiStyle.fontStyle = FontStyle.Bold;
                viewUse = EditorGUILayout.Foldout(viewUse, "Use Options", guiStyle);
                guiStyle.fontStyle = previousStyle;
                GUILayout.Space(2);
                if (viewUse)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("useOnlyIfGrabbed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("holdButtonToUse"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("pointerActivatesUseAction"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("useOverrideButton"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("allowedUseControllers"));
                    EditorGUI.indentLevel--;
                }
            }

            if (targ.GetComponent<VRTK_InteractableObject>().GetType().IsSubclassOf(typeof(VRTK_InteractableObject)))
            {
                GUILayout.Space(10);
                guiStyle = EditorStyles.foldout;
                previousStyle = guiStyle.fontStyle;
                guiStyle.fontStyle = FontStyle.Bold;
                viewCustom = EditorGUILayout.Foldout(viewCustom, "Custom Options", guiStyle);
                guiStyle.fontStyle = previousStyle;
                GUILayout.Space(2);
                if (viewCustom)
                {
                    List<string> excludedProperties = new List<string>();
                    foreach (var ioProperty in typeof(VRTK_InteractableObject).GetFields())
                    {
                        excludedProperties.Add(ioProperty.Name);
                    }

                    DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}