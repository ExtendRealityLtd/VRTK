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
    public float hideControllerDelay = 0f;

    public event ObjectInteractEventHandler ControllerUseInteractableObject;
    public event ObjectInteractEventHandler ControllerUnuseInteractableObject;

    GameObject usingObject = null;
    SteamVR_InteractTouch interactTouch;
    SteamVR_TrackedObject trackedController;
    SteamVR_ControllerActions controllerActions;

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

    private void Awake()
    {
        if (GetComponent<SteamVR_InteractTouch>() == null)
        {
            Debug.LogError("SteamVR_InteractUse is required to be attached to a SteamVR Controller that has the SteamVR_InteractTouch script attached to it");
            return;
        }

        interactTouch = GetComponent<SteamVR_InteractTouch>();
        trackedController = GetComponent<SteamVR_TrackedObject>();
        controllerActions = GetComponent<SteamVR_ControllerActions>();
    }

    private void Start()
    {
        if (GetComponent<SteamVR_ControllerEvents>() == null)
        {
            Debug.LogError("SteamVR_InteractUse is required to be attached to a SteamVR Controller that has the SteamVR_ControllerEvents script attached to it");
            return;
        }

        GetComponent<SteamVR_ControllerEvents>().AliasUseOn += new ControllerClickedEventHandler(DoStartUseObject);
        GetComponent<SteamVR_ControllerEvents>().AliasUseOff += new ControllerClickedEventHandler(DoStopUseObject);
    }

    private bool IsObjectUsable(GameObject obj)
    {
        return (interactTouch.IsObjectInteractable(obj) && obj.GetComponent<SteamVR_InteractableObject>().isUsable);
    }

    private bool IsObjectHoldOnUse(GameObject obj)
    {
        return (obj && obj.GetComponent<SteamVR_InteractableObject>() && obj.GetComponent<SteamVR_InteractableObject>().holdButtonToUse);
    }

    private int GetObjectUsingState(GameObject obj)
    {
        if (obj && obj.GetComponent<SteamVR_InteractableObject>())
        {
            return obj.GetComponent<SteamVR_InteractableObject>().UsingState;
        }
        return 0;
    }

    private void SetObjectUsingState(GameObject obj, int value)
    {
        if (obj && obj.GetComponent<SteamVR_InteractableObject>())
        {
            obj.GetComponent<SteamVR_InteractableObject>().UsingState = value;
        }
    }

    private void UseInteractedObject(GameObject touchedObject)
    {
        if ((usingObject == null || usingObject != touchedObject) && IsObjectUsable(touchedObject))
        {
            usingObject = touchedObject;
            OnControllerUseInteractableObject(interactTouch.SetControllerInteractEvent(usingObject));
            usingObject.GetComponent<SteamVR_InteractableObject>().StartUsing(this.gameObject);
            if (hideControllerOnUse)
            {
                Invoke("HideController", hideControllerDelay);
            }
            usingObject.GetComponent<SteamVR_InteractableObject>().ToggleHighlight(false);
        }
    }

    private void HideController()
    {
        if(usingObject != null)
        {
            controllerActions.ToggleControllerModel(false);
        }
    }

    private void UnuseInteractedObject()
    {
        if (usingObject != null)
        {
            OnControllerUnuseInteractableObject(interactTouch.SetControllerInteractEvent(usingObject));
            usingObject.GetComponent<SteamVR_InteractableObject>().StopUsing(this.gameObject);
            if (hideControllerOnUse)
            {
                controllerActions.ToggleControllerModel(true);
            }
            usingObject.GetComponent<SteamVR_InteractableObject>().ToggleHighlight(false);
            usingObject = null;
        }
    }

    private void DoStartUseObject(object sender, ControllerClickedEventArgs e)
    {
        GameObject touchedObject = interactTouch.GetTouchedObject();
        if (touchedObject != null && interactTouch.IsObjectInteractable(touchedObject))
        {
            UseInteractedObject(touchedObject);
            if (!IsObjectHoldOnUse(usingObject))
            {
                SetObjectUsingState(usingObject, GetObjectUsingState(usingObject) + 1);
            }
        }
    }

    private void DoStopUseObject(object sender, ControllerClickedEventArgs e)
    {
        if (IsObjectHoldOnUse(usingObject) || GetObjectUsingState(usingObject) >= 2)
        {
            SetObjectUsingState(usingObject, 0);
            UnuseInteractedObject();
        }
    }
}
