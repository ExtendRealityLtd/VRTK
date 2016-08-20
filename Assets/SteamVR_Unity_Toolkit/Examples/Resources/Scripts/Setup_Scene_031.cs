﻿using UnityEngine;
using VRTK;

[ExecuteInEditMode]
public class Setup_Scene_031 : MonoBehaviour
{
    private bool initalised = false;
#if UNITY_EDITOR
    private void Update()
    {
        if (!initalised)
        {
            var headset = VRTK_DeviceFinder.HeadsetTransform();
            if (!headset.GetComponent<VRTK_BezierPointer>())
            {
                var controllerManager = FindObjectOfType<SteamVR_ControllerManager>();
                var pointer = headset.gameObject.AddComponent<VRTK_BezierPointer>();

                pointer.controller = controllerManager.right.GetComponent<VRTK_ControllerEvents>();
                pointer.showPlayAreaCursor = true;
                pointer.pointerVisibility = VRTK_WorldPointer.pointerVisibilityStates.Always_Off;
                pointer.pointerLength = 7f;
                pointer.pointerDensity = 1;
                pointer.pointerCursorRadius = 0.3f;
                pointer.beamCurveOffset = 1f;
            }
            initalised = true;
        }
    }
#endif
}