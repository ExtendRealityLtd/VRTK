﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using VRTK;

[CustomEditor(typeof(VRTK_InteractableObject), true)]
public class VRTK_InteractableObjectEditor : Editor {

	private bool viewTouch = false;
	private bool viewGrab = false;
	private bool viewUse = false;
	private bool viewCustom = false;

	public override void OnInspectorGUI()
    {
		VRTK_InteractableObject targ = (VRTK_InteractableObject)target;
		EditorGUILayout.LabelField("Base Options", EditorStyles.boldLabel);
		GUILayout.Space(10);
		GUIStyle guiStyle = EditorStyles.foldout;
		FontStyle previousStyle = guiStyle.fontStyle;
		guiStyle.fontStyle = FontStyle.Bold;
		viewTouch = EditorGUILayout.Foldout(viewTouch, "Touch Options", guiStyle);
		guiStyle.fontStyle = previousStyle;
		GUILayout.Space(2);
		if (viewTouch)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Highlight on Touch:", GUILayout.Width(115));
			targ.highlightOnTouch = EditorGUILayout.Toggle(targ.highlightOnTouch);
			if(targ.highlightOnTouch)
			{
				targ.touchHighlightColor = EditorGUILayout.ColorField(targ.touchHighlightColor);
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("Rumble on Touch");
			EditorGUI.indentLevel++;
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Strength:", GUILayout.Width(70));
			float x = EditorGUILayout.FloatField(targ.rumbleOnTouch.x, GUILayout.Width(50));
			EditorGUILayout.LabelField("Duration:", GUILayout.Width(70));
			float y = EditorGUILayout.FloatField(targ.rumbleOnTouch.y, GUILayout.Width(50));
			targ.rumbleOnTouch = new Vector2( x, y);
			GUILayout.EndHorizontal();
			EditorGUI.indentLevel--;
			targ.allowedTouchControllers = (VRTK_InteractableObject.AllowedController)EditorGUILayout.EnumPopup("Allowed Touch Controllers:", targ.allowedTouchControllers);
			targ.hideControllerOnTouch = (VRTK_InteractableObject.ControllerHideMode)EditorGUILayout.EnumPopup("Hide Controller On Touch:", targ.hideControllerOnTouch);
		}

		//Grab Layout
		GUILayout.Space(10);
		targ.isGrabbable = EditorGUILayout.Toggle("Is Grabbable:", targ.isGrabbable);
		guiStyle = EditorStyles.foldout;
		previousStyle = guiStyle.fontStyle;
		guiStyle.fontStyle = FontStyle.Bold;
		viewGrab = EditorGUILayout.Foldout(viewGrab, "Grab Options", guiStyle);
		guiStyle.fontStyle = previousStyle;
		GUILayout.Space(2);
		if(viewGrab)
		{
			targ.isDroppable = EditorGUILayout.Toggle("Is Droppable:", targ.isDroppable);
			targ.isSwappable = EditorGUILayout.Toggle("Is Swappable:", targ.isSwappable);
			targ.holdButtonToGrab = EditorGUILayout.Toggle("Hold Button To Grab:", targ.holdButtonToGrab);
			targ.grabOverrideButton = (VRTK_ControllerEvents.ButtonAlias)EditorGUILayout.EnumPopup("Grab Override Button:", targ.grabOverrideButton);
			GUILayout.Label("Rumble On Grab");
			EditorGUI.indentLevel++;
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Strength:", GUILayout.Width(70));
			float x = EditorGUILayout.FloatField(targ.rumbleOnGrab.x, GUILayout.Width(50));
			EditorGUILayout.LabelField("Duration:", GUILayout.Width(70));
			float y = EditorGUILayout.FloatField(targ.rumbleOnGrab.y, GUILayout.Width(50));
			targ.rumbleOnGrab = new Vector2( x, y);
			GUILayout.EndHorizontal();
			EditorGUI.indentLevel--;
			targ.allowedGrabControllers = (VRTK_InteractableObject.AllowedController)EditorGUILayout.EnumPopup("Allowed Controllers:", targ.allowedGrabControllers);
			targ.precisionSnap = EditorGUILayout.Toggle("Precision Snap:", targ.precisionSnap);
			if(!targ.precisionSnap)
			{
				targ.rightSnapHandle = EditorGUILayout.ObjectField("Right Snap Handle:", targ.rightSnapHandle, typeof(Transform), true) as Transform;
				targ.leftSnapHandle = EditorGUILayout.ObjectField("Left Snap Handle:", targ.leftSnapHandle, typeof(Transform), true) as Transform;
			
			}
			targ.hideControllerOnGrab = (VRTK_InteractableObject.ControllerHideMode)EditorGUILayout.EnumPopup("Hide Controller On Grab:", targ.hideControllerOnGrab);
			targ.stayGrabbedOnTeleport = EditorGUILayout.Toggle("Stay Grabbed on Teleport:", targ.stayGrabbedOnTeleport);
			targ.grabAttachMechanic = (VRTK_InteractableObject.GrabAttachType)EditorGUILayout.EnumPopup("Grab Attach Mechanic:", targ.grabAttachMechanic);
			if(targ.grabAttachMechanic == VRTK_InteractableObject.GrabAttachType.Fixed_Joint || targ.grabAttachMechanic == VRTK_InteractableObject.GrabAttachType.Spring_Joint
				|| targ.grabAttachMechanic == VRTK_InteractableObject.GrabAttachType.Track_Object || targ.grabAttachMechanic == VRTK_InteractableObject.GrabAttachType.Rotator_Track)
			{
				targ.detachThreshold = EditorGUILayout.FloatField("Detach Threshold:", targ.detachThreshold);
				if(targ.grabAttachMechanic == VRTK_InteractableObject.GrabAttachType.Spring_Joint)
				{
					targ.springJointStrength = EditorGUILayout.FloatField("Spring Joint Strength:", targ.springJointStrength);
					targ.springJointDamper = EditorGUILayout.FloatField("Spring Joint Damper:", targ.springJointDamper);
				}
			}
			targ.throwMultiplier = EditorGUILayout.FloatField("Throw Multiplier:", targ.throwMultiplier);
			targ.onGrabCollisionDelay = EditorGUILayout.FloatField("On Grab Collision Delay:", targ.onGrabCollisionDelay);
		}

		GUILayout.Space(10);
		targ.isUsable = EditorGUILayout.Toggle("Is Usable:", targ.isUsable);
		guiStyle = EditorStyles.foldout;
		previousStyle = guiStyle.fontStyle;
		guiStyle.fontStyle = FontStyle.Bold;
		viewUse = EditorGUILayout.Foldout(viewUse, "Use Options", guiStyle);
		guiStyle.fontStyle = previousStyle;
		GUILayout.Space(2);
		if(viewUse)
		{
			targ.useOnlyIfGrabbed = EditorGUILayout.Toggle("Use Only If Grabbed: ", targ.useOnlyIfGrabbed);
			targ.holdButtonToUse = EditorGUILayout.Toggle("Hold Button To Use: ", targ.holdButtonToUse);
			targ.useOverrideButton = (VRTK_ControllerEvents.ButtonAlias)EditorGUILayout.EnumPopup("Use Override Button:", targ.useOverrideButton);
			targ.pointerActivatesUseAction = EditorGUILayout.Toggle("Pointer Activates Use Action: ", targ.pointerActivatesUseAction);
			GUILayout.Label("Rumble On Use");
			EditorGUI.indentLevel++;
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Strength:", GUILayout.Width(70));
			float x = EditorGUILayout.FloatField(targ.rumbleOnUse.x, GUILayout.Width(50));
			EditorGUILayout.LabelField("Duration:", GUILayout.Width(70));
			float y = EditorGUILayout.FloatField(targ.rumbleOnUse.y, GUILayout.Width(50));
			targ.rumbleOnUse = new Vector2( x, y);
			GUILayout.EndHorizontal();
			EditorGUI.indentLevel--;
			targ.allowedUseControllers = (VRTK_InteractableObject.AllowedController)EditorGUILayout.EnumPopup("Allowed Use Controllers:", targ.allowedUseControllers);
			targ.hideControllerOnUse = (VRTK_InteractableObject.ControllerHideMode)EditorGUILayout.EnumPopup("Hide Controller On Use:", targ.hideControllerOnUse);
		}
		if(targ.GetComponent<VRTK_InteractableObject>().GetType().IsSubclassOf(typeof(VRTK_InteractableObject)))
		{
			GUILayout.Space(10);
			guiStyle = EditorStyles.foldout;
			previousStyle = guiStyle.fontStyle;
			guiStyle.fontStyle = FontStyle.Bold;
			viewCustom = EditorGUILayout.Foldout(viewCustom, "Custom Options", guiStyle);
			guiStyle.fontStyle = previousStyle;
			GUILayout.Space(2);
			if(viewCustom)
			{
				DrawPropertiesExcluding(serializedObject, new string[] {"hideControllerOnUse","allowedUseControllers","rumbleOnUse","pointerActivatesUseAction","useOverrideButton",
					"holdButtonToUse","useOnlyIfGrabbed","throwMultiplier","onGrabCollisionDelay","springJointDamper","springJointStrength","detachThreshold","grabAttachMechanic","stayGrabbedOnTeleport",
					"hideControllerOnGrab","leftSnapHandle","rightSnapHandle","precisionSnap","allowedGrabControllers","rumbleOnGrab","grabOverrideButton","holdButtonToGrab","isSwappable","isDroppable",
					"isGrabbable","hideControllerOnTouch","allowedTouchControllers","rumbleOnTouch","touchHighlightColor","highlightOnTouch", "isUsable"});
			}
		}

    }
}
