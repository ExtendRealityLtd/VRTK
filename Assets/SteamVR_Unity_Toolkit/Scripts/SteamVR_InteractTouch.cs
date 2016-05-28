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

    public void ForceTouch(GameObject obj)
    {
        if (obj.GetComponent<Collider>())
        {
            OnTriggerStay(obj.GetComponent<Collider>());
        }
    }

    public GameObject GetTouchedObject()
    {
        return touchedObject;
    }

    public bool IsObjectInteractable(GameObject obj)
    {
        return (obj && (obj.GetComponent<SteamVR_InteractableObject>() || obj.GetComponentInParent<SteamVR_InteractableObject>()));
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
        collider.radius = 0.06f;
        collider.center = new Vector3(0f, -0.04f, 0f);
        collider.isTrigger = true;
    }

    private void OnTriggerStay(Collider collider)
    {
        if (touchedObject == null && IsObjectInteractable(collider.gameObject))
        {
            if (collider.gameObject.GetComponent<SteamVR_InteractableObject>())
            {
                touchedObject = collider.gameObject;
            }
            else
            {
                touchedObject = collider.gameObject.GetComponentInParent<SteamVR_InteractableObject>().gameObject;
            }

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
            GameObject untouched;
            if (collider.gameObject.GetComponent<SteamVR_InteractableObject>())
            {
                untouched = collider.gameObject;
            }
            else
            {
                untouched = collider.gameObject.GetComponentInParent<SteamVR_InteractableObject>().gameObject;
            }

            OnControllerUntouchInteractableObject(SetControllerInteractEvent(untouched.gameObject));
            untouched.GetComponent<SteamVR_InteractableObject>().ToggleHighlight(false);
            untouched.GetComponent<SteamVR_InteractableObject>().StopTouching(this.gameObject);
        }

        if (hideControllerOnTouch)
        {
            controllerActions.ToggleControllerModel(true, touchedObject);
        }
        touchedObject = null;
    }

    private void HideController()
    {
        if (touchedObject != null)
        {
            controllerActions.ToggleControllerModel(false, touchedObject);
        }
    }
}
