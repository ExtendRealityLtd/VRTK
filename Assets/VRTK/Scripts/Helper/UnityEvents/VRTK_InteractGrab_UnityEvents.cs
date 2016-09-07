﻿using UnityEngine;
using UnityEngine.Events;
using VRTK;

[RequireComponent(typeof(VRTK_InteractGrab))]
public class VRTK_InteractGrab_UnityEvents : MonoBehaviour
{
    private VRTK_InteractGrab ig;

    [System.Serializable]
    public class UnityObjectEvent : UnityEvent<ObjectInteractEventArgs> { };
    public UnityObjectEvent OnControllerGrabInteractableObject;
    public UnityObjectEvent OnControllerUngrabInteractableObject;

    private void SetInteractGrab()
    {
        if (ig == null)
        {
            ig = GetComponent<VRTK_InteractGrab>();
        }
    }

    private void OnEnable()
    {
        SetInteractGrab();
        if (ig == null)
        {
            Debug.LogError("The VRTK_InteractGrab_UnityEvents script requires to be attached to a GameObject that contains a VRTK_InteractGrab script");
            return;
        }

        ig.ControllerGrabInteractableObject += ControllerGrabInteractableObject;
        ig.ControllerUngrabInteractableObject += ControllerUngrabInteractableObject;
    }

    private void ControllerGrabInteractableObject(object o, ObjectInteractEventArgs e)
    {
        OnControllerGrabInteractableObject.Invoke(e);
    }

    private void ControllerUngrabInteractableObject(object o, ObjectInteractEventArgs e)
    {
        OnControllerUngrabInteractableObject.Invoke(e);
    }

    private void OnDisable()
    {
        if (ig == null)
        {
            return;
        }

        ig.ControllerGrabInteractableObject -= ControllerGrabInteractableObject;
        ig.ControllerUngrabInteractableObject -= ControllerUngrabInteractableObject;
    }
}