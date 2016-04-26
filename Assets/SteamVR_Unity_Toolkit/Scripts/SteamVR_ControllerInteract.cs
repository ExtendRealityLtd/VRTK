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

public class SteamVR_ControllerInteract : MonoBehaviour {
    public Rigidbody controllerAttachPoint = null;  

    private FixedJoint controllerAttachJoint;
    private GameObject objectToGrab = null;
    private GameObject grabbedObject = null;

    private SteamVR_TrackedObject trackedController;

    void Awake()
    {
        trackedController = GetComponent<SteamVR_TrackedObject>();
    }

    // Use this for initialization
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

        GetComponent<SteamVR_ControllerEvents>().AliasInteractOn += new ControllerClickedEventHandler(DoGrabObject);
        GetComponent<SteamVR_ControllerEvents>().AliasInteractOff += new ControllerClickedEventHandler(DoReleaseObject);
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

    void DoGrabObject(object sender, ControllerClickedEventArgs e)
    {
        if (objectToGrab != null && grabbedObject == null)
        {
            if (controllerAttachJoint == null)
            {
                grabbedObject = objectToGrab;
                grabbedObject.GetComponent<SteamVR_InteractableObject>().OnGrab(gameObject);
                SnapObjectToGrabToController(grabbedObject);
            }
        }
    }

    void DoReleaseObject(object sender, ControllerClickedEventArgs e)
    {
        if (grabbedObject != null && controllerAttachJoint != null)
        {
            grabbedObject.GetComponent<SteamVR_InteractableObject>().OnUngrab(gameObject);
            grabbedObject = null;
            Rigidbody releasedObjectRigidBody = ReleaseGrabbedObjectFromController();
            ThrowReleasedObject(releasedObjectRigidBody, e.controllerIndex);
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.GetComponent<SteamVR_InteractableObject>() && collider.GetComponent<SteamVR_InteractableObject>().isGrabbable && grabbedObject == null)
        {            
            collider.GetComponent<SteamVR_InteractableObject>().ToggleHighlight(true);
            objectToGrab = collider.gameObject;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<SteamVR_InteractableObject>())
        {
            collider.GetComponent<SteamVR_InteractableObject>().ToggleHighlight(false);
        }
        objectToGrab = null;
    }
}
