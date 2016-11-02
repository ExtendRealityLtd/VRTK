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
        private VRTK_InteractableObject.GrabAttachType[] hasDetachThreshold = new VRTK_InteractableObject.GrabAttachType[]
        {
            VRTK_InteractableObject.GrabAttachType.Fixed_Joint,
            VRTK_InteractableObject.GrabAttachType.Spring_Joint,
            VRTK_InteractableObject.GrabAttachType.Track_Object,
            VRTK_InteractableObject.GrabAttachType.Rotator_Track
        };

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

                targ.highlightOnTouch = EditorGUILayout.Toggle(VRTK_EditorUtilities.BuildGUIContent<VRTK_InteractableObject>("highlightOnTouch"), targ.highlightOnTouch);
                if (targ.highlightOnTouch)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("touchHighlightColor"));
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(VRTK_EditorUtilities.BuildGUIContent<VRTK_InteractableObject>("rumbleOnTouch"));
                EditorGUI.indentLevel--;
                GUILayout.Label("Strength", GUILayout.MinWidth(49f));
                float y = EditorGUILayout.FloatField(targ.rumbleOnTouch.y, GUILayout.MinWidth(10f));
                GUILayout.Label("Duration", GUILayout.MinWidth(50f));
                float x = EditorGUILayout.FloatField(targ.rumbleOnTouch.x, GUILayout.MinWidth(10f));
                targ.rumbleOnTouch = new Vector2(x, y);
                EditorGUI.indentLevel++;
                GUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("allowedTouchControllers"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("hideControllerOnTouch"));

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

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("validDrop"));
                    targ.secondaryGrabAction = (VRTK_InteractableObject.SecondaryControllerActions)EditorGUILayout.EnumPopup(VRTK_EditorUtilities.BuildGUIContent<VRTK_InteractableObject>("secondaryGrabAction"), targ.secondaryGrabAction);
                    if(targ.secondaryGrabAction == VRTK_InteractableObject.SecondaryControllerActions.Custom_Action)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("customSecondaryAction"));
                    }

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("holdButtonToGrab"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("grabOverrideButton"));

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(VRTK_EditorUtilities.BuildGUIContent<VRTK_InteractableObject>("rumbleOnGrab"));
                    EditorGUI.indentLevel--;
                    GUILayout.Label("Strength", GUILayout.MinWidth(49f));
                    float y = EditorGUILayout.FloatField(targ.rumbleOnGrab.y, GUILayout.MinWidth(10f));
                    GUILayout.Label("Duration", GUILayout.MinWidth(50f));
                    float x = EditorGUILayout.FloatField(targ.rumbleOnGrab.x, GUILayout.MinWidth(10f));
                    targ.rumbleOnGrab = new Vector2(x, y);
                    EditorGUI.indentLevel++;
                    GUILayout.EndHorizontal();

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("allowedGrabControllers"));
                    targ.precisionSnap = EditorGUILayout.Toggle(VRTK_EditorUtilities.BuildGUIContent<VRTK_InteractableObject>("precisionSnap", "Precision Grab(Snap)"), targ.precisionSnap);
                    if (!targ.precisionSnap)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightSnapHandle"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftSnapHandle"));

                    }
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("hideControllerOnGrab"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("stayGrabbedOnTeleport"));

                    targ.grabAttachMechanic = (VRTK_InteractableObject.GrabAttachType)EditorGUILayout.EnumPopup(VRTK_EditorUtilities.BuildGUIContent<VRTK_InteractableObject>("grabAttachMechanic"), targ.grabAttachMechanic);
                    if (Array.IndexOf(hasDetachThreshold, targ.grabAttachMechanic) >= 0)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("detachThreshold"));
                        if (targ.grabAttachMechanic == VRTK_InteractableObject.GrabAttachType.Spring_Joint)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("springJointStrength"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("springJointDamper"));
                        }
                    }
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("throwMultiplier"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("onGrabCollisionDelay"));

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
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("useOverrideButton"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("pointerActivatesUseAction"));

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(VRTK_EditorUtilities.BuildGUIContent<VRTK_InteractableObject>("rumbleOnUse"));
                    EditorGUI.indentLevel--;
                    GUILayout.Label("Strength", GUILayout.MinWidth(49f));
                    float y = EditorGUILayout.FloatField(targ.rumbleOnUse.y, GUILayout.MinWidth(10f));
                    GUILayout.Label("Duration", GUILayout.MinWidth(50f));
                    float x = EditorGUILayout.FloatField(targ.rumbleOnUse.x, GUILayout.MinWidth(10f));
                    targ.rumbleOnUse = new Vector2(x, y);
                    EditorGUI.indentLevel++;
                    GUILayout.EndHorizontal();

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("allowedUseControllers"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("hideControllerOnUse"));

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