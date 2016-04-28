//====================================================================================
//
// Purpose: Provide ability to use an interactable object when it is being touched
//
// This script must be attached to a Controller within the [CameraRig] Prefab
//
// The SteamVR_ControllerEvents and SteamVR_InteractTouch scripts must also be
// attached to the Controller
//
// Press the default 'Trigger' button on the controller to use an object
// Released the default 'Trigger' button on the controller to stop using an object
//
//====================================================================================

using UnityEngine;
using System.Collections;

public class SteamVR_InteractUse : MonoBehaviour
{
    public bool hideControllerOnUse = false;    

    public event ObjectInteractEventHandler ControllerUseInteractableObject;
    public event ObjectInteractEventHandler ControllerUnuseInteractableObject;

    GameObject usingObject = null;
    SteamVR_InteractTouch interactTouch;
    SteamVR_TrackedObject trackedController;

    public virtual void OnControllerUseInteractableObject(ObjectInteractEventArgs e)
    {
        if (ControllerUseInteractableObject != null)
            ControllerUseInteractableObject(this, e);
    }

    public virtual void OnControllerUnuseInteractableObject(ObjectInteractEventArgs e)
    {
        if (ControllerUnuseInteractableObject != null)
            ControllerUnuseInteractableObject(this, e);
    }

    void Awake()
    {
        if (GetComponent<SteamVR_InteractTouch>() == null)
        {
            Debug.LogError("SteamVR_InteractUse is required to be attached to a SteamVR Controller that has the SteamVR_InteractTouch script attached to it");
            return;
        }

        interactTouch = GetComponent<SteamVR_InteractTouch>();
        trackedController = GetComponent<SteamVR_TrackedObject>();
    }

    void Start()
    {
        if (GetComponent<SteamVR_ControllerEvents>() == null)
        {
            Debug.LogError("SteamVR_InteractUse is required to be attached to a SteamVR Controller that has the SteamVR_ControllerEvents script attached to it");
            return;
        }

        GetComponent<SteamVR_ControllerEvents>().AliasUseOn += new ControllerClickedEventHandler(DoStartUseObject);
        GetComponent<SteamVR_ControllerEvents>().AliasUseOff += new ControllerClickedEventHandler(DoStopUseObject);
    }

    bool IsObjectUsable(GameObject obj)
    {
        return (interactTouch.IsObjectInteractable(obj) && obj.GetComponent<SteamVR_InteractableObject>().isUsable);
    }

    void UseInteractedObject()
    {
        if (usingObject == null && IsObjectUsable(interactTouch.GetTouchedObject()))
        {
            usingObject = interactTouch.GetTouchedObject();
            OnControllerUseInteractableObject(interactTouch.SetControllerInteractEvent(usingObject));
            usingObject.GetComponent<SteamVR_InteractableObject>().StartUsing(this.gameObject);
            if (hideControllerOnUse)
            {
                trackedController.ToggleControllerModel(false);
            }
        }
    }

    void UnuseInteractedObject()
    {
        if (usingObject != null)
        {
            OnControllerUnuseInteractableObject(interactTouch.SetControllerInteractEvent(usingObject));
            usingObject.GetComponent<SteamVR_InteractableObject>().StopUsing(this.gameObject);
            if (hideControllerOnUse)
            {
                trackedController.ToggleControllerModel(true);
            }
            usingObject = null;
        }
    }

    void DoStartUseObject(object sender, ControllerClickedEventArgs e)
    {
        if (interactTouch.GetTouchedObject() != null && interactTouch.IsObjectInteractable(interactTouch.GetTouchedObject()))
        {
            UseInteractedObject();
        }
    }

    void DoStopUseObject(object sender, ControllerClickedEventArgs e)
    {
        UnuseInteractedObject();
    }
}
