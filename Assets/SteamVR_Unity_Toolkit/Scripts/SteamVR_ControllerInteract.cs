//====================================================================================
//
// Purpose: Provide ability to interact with interactable objects in the game world
//
// This script must be attached to a Controller within the [CameraRig] Prefab
//
// The SteamVR_ControllerEvents script must also be attached to the Controller
//
// For an object to be grabbale it must contain the SteamVR_InteractableObject script
// and have the isGrabbable flag set to true.
//
// Press the default 'Trigger' button on the controller to grab the object
// Released the default 'Trigger' button on the controller to release the object
//
//====================================================================================

using UnityEngine;
using System.Collections;

public struct ControllerInteractEventArgs
{
    public uint controllerIndex;
    public GameObject target;
}

public delegate void ControllerInteractEventHandler(object sender, ControllerInteractEventArgs e);

public class SteamVR_ControllerInteract : MonoBehaviour {
    public Rigidbody controllerAttachPoint = null;
    public bool hideControllerOnTouch = false;
    public bool hideControllerOnGrab = false;
    public bool hideControllerOnUse = false;
    public Color globalTouchHighlightColor = Color.clear;

    public event ControllerInteractEventHandler ControllerTouchInteractableObject;
    public event ControllerInteractEventHandler ControllerUntouchInteractableObject;
    public event ControllerInteractEventHandler ControllerGrabInteractableObject;
    public event ControllerInteractEventHandler ControllerUngrabInteractableObject;
    public event ControllerInteractEventHandler ControllerUseInteractableObject;
    public event ControllerInteractEventHandler ControllerUnuseInteractableObject;

    private FixedJoint controllerAttachJoint;
    private GameObject touchedObject = null;
    private GameObject grabbedObject = null;
    private GameObject usingObject = null;

    private SteamVR_TrackedObject trackedController;
    private bool controllerVisible = true;

    public virtual void OnControllerTouchInteractableObject(ControllerInteractEventArgs e)
    {
        if (ControllerTouchInteractableObject != null)
            ControllerTouchInteractableObject(this, e);
    }

    public virtual void OnControllerUntouchInteractableObject(ControllerInteractEventArgs e)
    {
        if (ControllerUntouchInteractableObject != null)
            ControllerUntouchInteractableObject(this, e);
    }

    public virtual void OnControllerGrabInteractableObject(ControllerInteractEventArgs e)
    {
        if (ControllerGrabInteractableObject != null)
            ControllerGrabInteractableObject(this, e);
    }

    public virtual void OnControllerUngrabInteractableObject(ControllerInteractEventArgs e)
    {
        if (ControllerUngrabInteractableObject != null)
            ControllerUngrabInteractableObject(this, e);
    }

    public virtual void OnControllerUseInteractableObject(ControllerInteractEventArgs e)
    {
        if (ControllerUseInteractableObject != null)
            ControllerUseInteractableObject(this, e);
    }

    public virtual void OnControllerUnuseInteractableObject(ControllerInteractEventArgs e)
    {
        if (ControllerUnuseInteractableObject != null)
            ControllerUnuseInteractableObject(this, e);
    }

    ControllerInteractEventArgs SetControllerInteractEvent(GameObject target)
    {
        ControllerInteractEventArgs e;
        e.controllerIndex = (uint)trackedController.index;
        e.target = target;
        return e;
    }

    void Awake()
    {
        trackedController = GetComponent<SteamVR_TrackedObject>();
    }

    void Start () {
        if (GetComponent<SteamVR_ControllerEvents>() == null)
        {
            Debug.LogError("SteamVR_ControllerInteract is required to be attached to a SteamVR Controller that has the SteamVR_ControllerEvents script attached to it");
            return;
        }

        //If no attach point has been specified then just use the tip of the controller
        if (controllerAttachPoint == null)
        {
            controllerAttachPoint = transform.GetChild(0).Find("tip").GetChild(0).GetComponent<Rigidbody>();
        }

        //Create trigger box collider for controller
        BoxCollider collider = this.gameObject.AddComponent<BoxCollider>();
        collider.size = new Vector3(0.1f, 0.08f, 0.2f);
        collider.center = new Vector3(0f, -0.035f, -0.055f);
        collider.isTrigger = true;

        GetComponent<SteamVR_ControllerEvents>().AliasInteractOn += new ControllerClickedEventHandler(DoInteractObject);
        GetComponent<SteamVR_ControllerEvents>().AliasInteractOff += new ControllerClickedEventHandler(DoStopInteractObject);
    }

    void ToggleControllerModel(bool on)
    {
        foreach(MeshRenderer renderer in this.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = on;
        }
        controllerVisible = on;
    }

    bool IsObjectInteractable(GameObject obj)
    {
        return (obj.GetComponent<SteamVR_InteractableObject>());
    }

    bool IsObjectGrabbable(GameObject obj)
    {
        return (IsObjectInteractable(obj) && obj.GetComponent<SteamVR_InteractableObject>().isGrabbable);
    }

    bool IsObjectUsable(GameObject obj)
    {
        return (IsObjectInteractable(obj) && obj.GetComponent<SteamVR_InteractableObject>().isUsable);
    }

    void SnapObjectToGrabToController(GameObject obj)
    {
        obj.transform.position = controllerAttachPoint.transform.position;
        controllerAttachJoint = obj.AddComponent<FixedJoint>();
        controllerAttachJoint.connectedBody = controllerAttachPoint;
    }

    Rigidbody ReleaseGrabbedObjectFromController()
    {
        var jointGameObject = controllerAttachJoint.gameObject;
        var rigidbody = jointGameObject.GetComponent<Rigidbody>();
        Object.DestroyImmediate(controllerAttachJoint);
        controllerAttachJoint = null;

        return rigidbody;
    }

    void ThrowReleasedObject(Rigidbody rb, uint controllerIndex)
    {
        var origin = trackedController.origin ? trackedController.origin : trackedController.transform.parent;
        var device = SteamVR_Controller.Input((int)controllerIndex);
        if (origin != null)
        {
            rb.velocity = origin.TransformVector(device.velocity);
            rb.angularVelocity = origin.TransformVector(device.angularVelocity);
        }
        else
        {
            rb.velocity = device.velocity;
            rb.angularVelocity = device.angularVelocity;
        }
        rb.maxAngularVelocity = rb.angularVelocity.magnitude;
    }

    void GrabInteractedObject()
    {
        if (controllerAttachJoint == null && grabbedObject == null && IsObjectGrabbable(touchedObject))
        {
            grabbedObject = touchedObject;
            OnControllerGrabInteractableObject(SetControllerInteractEvent(grabbedObject));
            grabbedObject.GetComponent<SteamVR_InteractableObject>().Grabbed(this.gameObject);            
            if (hideControllerOnGrab)
            {
                ToggleControllerModel(false);
            }

            SnapObjectToGrabToController(grabbedObject);
        }
    }

    void UngrabInteractedObject(uint controllerIndex)
    {
        if (grabbedObject != null && controllerAttachJoint != null)
        {
            OnControllerUngrabInteractableObject(SetControllerInteractEvent(grabbedObject));
            grabbedObject.GetComponent<SteamVR_InteractableObject>().Ungrabbed(this.gameObject);

            Rigidbody releasedObjectRigidBody = ReleaseGrabbedObjectFromController();
            ThrowReleasedObject(releasedObjectRigidBody, controllerIndex);
            if (hideControllerOnGrab)
            {
                ToggleControllerModel(true);
            }
            grabbedObject = null;
        }
    }

    void UseInteractedObject()
    {
        if (usingObject == null && IsObjectUsable(touchedObject)) {
            usingObject = touchedObject;
            OnControllerUseInteractableObject(SetControllerInteractEvent(usingObject));
            usingObject.GetComponent<SteamVR_InteractableObject>().StartUsing(this.gameObject);
            if (hideControllerOnUse)
            {
                ToggleControllerModel(false);
            }
        }
    }

    void UnuseInteractedObject()
    {
        if (usingObject != null)
        {
            OnControllerUnuseInteractableObject(SetControllerInteractEvent(usingObject));
            usingObject.GetComponent<SteamVR_InteractableObject>().StopUsing(this.gameObject);
            if (hideControllerOnUse)
            {
                ToggleControllerModel(true);
            }
            usingObject = null;
        }
    }

    void DoInteractObject(object sender, ControllerClickedEventArgs e)
    {
        if (touchedObject != null && IsObjectInteractable(touchedObject) )
        {
            GrabInteractedObject();
            UseInteractedObject();
        }
    }

    void DoStopInteractObject(object sender, ControllerClickedEventArgs e)
    {
        UngrabInteractedObject(e.controllerIndex);
        UnuseInteractedObject();
    }

    void OnTriggerStay(Collider collider)
    {
        if (touchedObject == null && IsObjectInteractable(collider.gameObject))
        {            
            touchedObject = collider.gameObject;
            OnControllerTouchInteractableObject(SetControllerInteractEvent(touchedObject));
            touchedObject.GetComponent<SteamVR_InteractableObject>().ToggleHighlight(true, globalTouchHighlightColor);
            if (controllerVisible && hideControllerOnTouch)
            {
                ToggleControllerModel(false);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<SteamVR_InteractableObject>())
        {
            OnControllerUntouchInteractableObject(SetControllerInteractEvent(collider.gameObject));
            collider.GetComponent<SteamVR_InteractableObject>().ToggleHighlight(false);
        }
        touchedObject = null;
        if (hideControllerOnTouch)
        {
            ToggleControllerModel(true);
        }
    }
}
