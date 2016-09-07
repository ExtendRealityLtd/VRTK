﻿using UnityEngine;
using UnityEngine.Events;
using VRTK;

[RequireComponent(typeof(VRTK_InteractTouch))]
public class VRTK_InteractTouch_UnityEvents : MonoBehaviour
{
    private VRTK_InteractTouch it;

    [System.Serializable]
    public class UnityObjectEvent : UnityEvent<ObjectInteractEventArgs> { };
    public UnityObjectEvent OnControllerTouchInteractableObject;
    public UnityObjectEvent OnControllerUntouchInteractableObject;

    private void SetInteractTouch()
    {
        if (it == null)
        {
            it = GetComponent<VRTK_InteractTouch>();
        }
    }

    private void OnEnable()
    {
        SetInteractTouch();
        if (it == null)
        {
            Debug.LogError("The VRTK_InteractTouch_UnityEvents script requires to be attached to a GameObject that contains a VRTK_InteractTouch script");
            return;
        }

        it.ControllerTouchInteractableObject += ControllerTouchInteractableObject;
        it.ControllerUntouchInteractableObject += ControllerUntouchInteractableObject;
    }

    private void ControllerTouchInteractableObject(object o, ObjectInteractEventArgs e)
    {
        OnControllerTouchInteractableObject.Invoke(e);
    }

    private void ControllerUntouchInteractableObject(object o, ObjectInteractEventArgs e)
    {
        OnControllerUntouchInteractableObject.Invoke(e);
    }

    private void OnDisable()
    {
        if (it == null)
        {
            return;
        }

        it.ControllerTouchInteractableObject -= ControllerTouchInteractableObject;
        it.ControllerUntouchInteractableObject -= ControllerUntouchInteractableObject;
    }
}