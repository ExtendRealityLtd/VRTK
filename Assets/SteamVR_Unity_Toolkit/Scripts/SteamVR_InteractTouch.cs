//====================================================================================
//
// Purpose: Provide basic touch detection of controller to interactable objects
//
// This script must be attached to a Controller within the [CameraRig] Prefab
//
//====================================================================================

using UnityEngine;
using System.Collections;

public struct ObjectInteractEventArgs
{
    public uint controllerIndex;
    public GameObject target;
}

public delegate void ObjectInteractEventHandler(object sender, ObjectInteractEventArgs e);

public class SteamVR_InteractTouch : MonoBehaviour {

    public bool hideControllerOnTouch = false;
    public float hideControllerDelay = 0f;
    public Color globalTouchHighlightColor = Color.clear;

    public event ObjectInteractEventHandler ControllerTouchInteractableObject;
    public event ObjectInteractEventHandler ControllerUntouchInteractableObject;

    GameObject touchedObject = null;    
    SteamVR_TrackedObject trackedController;
    SteamVR_ControllerActions controllerActions;

    public virtual void OnControllerTouchInteractableObject(ObjectInteractEventArgs e)
    {
        if (ControllerTouchInteractableObject != null)
            ControllerTouchInteractableObject(this, e);
    }

    public virtual void OnControllerUntouchInteractableObject(ObjectInteractEventArgs e)
    {
        if (ControllerUntouchInteractableObject != null)
            ControllerUntouchInteractableObject(this, e);
    }

    public ObjectInteractEventArgs SetControllerInteractEvent(GameObject target)
    {
        ObjectInteractEventArgs e;
        e.controllerIndex = (uint)trackedController.index;
        e.target = target;
        return e;
    }

    public GameObject GetTouchedObject()
    {
        return touchedObject;
    }

    public bool IsObjectInteractable(GameObject obj)
    {
        return (obj && obj.GetComponent<SteamVR_InteractableObject>());
    }

    private void Awake()
    {
        trackedController = GetComponent<SteamVR_TrackedObject>();
        controllerActions = GetComponent<SteamVR_ControllerActions>();
    }

    private void Start()
    {
        if (GetComponent<SteamVR_ControllerActions>() == null)
        {
            Debug.LogError("SteamVR_InteractTouch is required to be attached to a SteamVR Controller that has the SteamVR_ControllerActions script attached to it");
            return;
        }

        //Create trigger box collider for controller
        SphereCollider collider = this.gameObject.AddComponent<SphereCollider>();
        collider.radius = 0.05f;
        collider.center = new Vector3(0f, -0.035f, -0.01f);
        collider.isTrigger = true;
    }

    private void OnTriggerStay(Collider collider)
    {
        if (touchedObject == null && IsObjectInteractable(collider.gameObject))
        {
            touchedObject = collider.gameObject;
            OnControllerTouchInteractableObject(SetControllerInteractEvent(touchedObject));
            touchedObject.GetComponent<SteamVR_InteractableObject>().ToggleHighlight(true, globalTouchHighlightColor);
            touchedObject.GetComponent<SteamVR_InteractableObject>().StartTouching(this.gameObject);
            if (controllerActions.IsControllerVisible() && hideControllerOnTouch)
            {
                Invoke("HideController", hideControllerDelay);
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (IsObjectInteractable(collider.gameObject))
        {
            OnControllerUntouchInteractableObject(SetControllerInteractEvent(collider.gameObject));
            collider.GetComponent<SteamVR_InteractableObject>().ToggleHighlight(false);
            collider.GetComponent<SteamVR_InteractableObject>().StopTouching(this.gameObject);
        }
        touchedObject = null;
        if (hideControllerOnTouch)
        {
            controllerActions.ToggleControllerModel(true);
        }
    }

    private void HideController()
    {
        if (touchedObject != null)
        {
            controllerActions.ToggleControllerModel(false);
        }
    }
}
