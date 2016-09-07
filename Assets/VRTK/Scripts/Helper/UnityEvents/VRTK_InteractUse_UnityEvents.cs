﻿using UnityEngine;
using UnityEngine.Events;
using VRTK;

[RequireComponent(typeof(VRTK_InteractUse))]
public class VRTK_InteractUse_UnityEvents : MonoBehaviour
{
    private VRTK_InteractUse iu;

    [System.Serializable]
    public class UnityObjectEvent : UnityEvent<ObjectInteractEventArgs> { };
    public UnityObjectEvent OnControllerUseInteractableObject;
    public UnityObjectEvent OnControllerUnuseInteractableObject;

    private void SetInteractUse()
    {
        if (iu == null)
        {
            iu = GetComponent<VRTK_InteractUse>();
        }
    }

    private void OnEnable()
    {
        SetInteractUse();
        if (iu == null)
        {
            Debug.LogError("The VRTK_InteractUse_UnityEvents script requires to be attached to a GameObject that contains a VRTK_InteractUse script");
            return;
        }

        iu.ControllerUseInteractableObject += ControllerUseInteractableObject;
        iu.ControllerUnuseInteractableObject += ControllerUnuseInteractableObject;
    }

    private void ControllerUseInteractableObject(object o, ObjectInteractEventArgs e)
    {
        OnControllerUseInteractableObject.Invoke(e);
    }

    private void ControllerUnuseInteractableObject(object o, ObjectInteractEventArgs e)
    {
        OnControllerUnuseInteractableObject.Invoke(e);
    }

    private void OnDisable()
    {
        if (iu == null)
        {
            return;
        }

        iu.ControllerUseInteractableObject -= ControllerUseInteractableObject;
        iu.ControllerUnuseInteractableObject -= ControllerUnuseInteractableObject;
    }
}