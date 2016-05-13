//====================================================================================
//
// Purpose: Provide ability to grab an interactable object when it is being touched
//
// This script must be attached to a Controller within the [CameraRig] Prefab
//
// The SteamVR_ControllerEvents and SteamVR_InteractTouch scripts must also be
// attached to the Controller
//
// Press the default 'Trigger' button on the controller to grab an object
// Released the default 'Trigger' button on the controller to drop an object
//
//====================================================================================

using UnityEngine;
using System.Collections;

public class SteamVR_InteractGrab : MonoBehaviour
{
    public Rigidbody controllerAttachPoint = null;
    public bool hideControllerOnGrab = false;

    public event ObjectInteractEventHandler ControllerGrabInteractableObject;
    public event ObjectInteractEventHandler ControllerUngrabInteractableObject;

    Joint controllerAttachJoint;
    GameObject grabbedObject = null;

    SteamVR_InteractTouch interactTouch;
    SteamVR_TrackedObject trackedController;
    SteamVR_ControllerActions controllerActions;

    private int grabEnabledState = 0;

    public virtual void OnControllerGrabInteractableObject(ObjectInteractEventArgs e)
    {
        if (ControllerGrabInteractableObject != null)
            ControllerGrabInteractableObject(this, e);
    }

    public virtual void OnControllerUngrabInteractableObject(ObjectInteractEventArgs e)
    {
        if (ControllerUngrabInteractableObject != null)
            ControllerUngrabInteractableObject(this, e);
    }

    private void Awake()
    {
        if (GetComponent<SteamVR_InteractTouch>() == null)
        {
            Debug.LogError("SteamVR_InteractGrab is required to be attached to a SteamVR Controller that has the SteamVR_InteractTouch script attached to it");
            return;
        }

        interactTouch = GetComponent<SteamVR_InteractTouch>();
        trackedController = GetComponent<SteamVR_TrackedObject>();
        controllerActions = GetComponent<SteamVR_ControllerActions>();
    }

    private void Start()
    {
        //If no attach point has been specified then just use the tip of the controller
        if (controllerAttachPoint == null)
        {
            controllerAttachPoint = transform.GetChild(0).Find("tip").GetChild(0).GetComponent<Rigidbody>();
        }

        if (GetComponent<SteamVR_ControllerEvents>() == null)
        {
            Debug.LogError("SteamVR_InteractGrab is required to be attached to a SteamVR Controller that has the SteamVR_ControllerEvents script attached to it");
            return;
        }

        GetComponent<SteamVR_ControllerEvents>().AliasGrabOn += new ControllerClickedEventHandler(DoGrabObject);
        GetComponent<SteamVR_ControllerEvents>().AliasGrabOff += new ControllerClickedEventHandler(DoReleaseObject);
    }

    private bool IsObjectGrabbable(GameObject obj)
    {
        return (interactTouch.IsObjectInteractable(obj) && obj.GetComponent<SteamVR_InteractableObject>().isGrabbable);
    }

    private bool IsObjectHoldOnGrab(GameObject obj)
    {
        return (obj && obj.GetComponent<SteamVR_InteractableObject>() && obj.GetComponent<SteamVR_InteractableObject>().holdButtonToGrab);
    }

    private void SnapObjectToGrabToController(GameObject obj)
    {
        //Stop collisions for a moment whilst sorting out position to prevent clipping issues
        obj.GetComponent<SteamVR_InteractableObject>().PauseCollisions(0.2f);

        SteamVR_InteractableObject.GrabType grabType = obj.GetComponent<SteamVR_InteractableObject>().grabSnapType;

        if (grabType == SteamVR_InteractableObject.GrabType.Rotation_Snap)
        {
            // Identity Controller Rotation
            this.transform.eulerAngles = new Vector3(0f, 270f, 0f);
            obj.transform.eulerAngles = obj.GetComponent<SteamVR_InteractableObject>().snapToRotation;
        }

        if (grabType != SteamVR_InteractableObject.GrabType.Precision_Snap)
        {
            obj.transform.position = controllerAttachPoint.transform.position;
        }

        CreateJoint(obj);
    }

    private bool JointOnController(GameObject obj)
    {
        return (obj.GetComponent<Joint>() && obj.GetComponent<Joint>().connectedBody && obj.GetComponent<Joint>().connectedBody.gameObject.GetComponentInParent<SteamVR_InteractTouch>());
    }

    private void CreateJoint(GameObject obj)
    {
        if (obj.GetComponent<Joint>() && ! JointOnController(obj))
        {
            SpringJoint tempSpringJoint = obj.AddComponent<SpringJoint>();
            tempSpringJoint.spring = 500;
            tempSpringJoint.damper = 50;
            controllerAttachJoint = tempSpringJoint;
        }
        else
        {
            controllerAttachJoint = obj.AddComponent<FixedJoint>();
        }
        controllerAttachJoint.connectedBody = controllerAttachPoint;
    }

    private Rigidbody ReleaseGrabbedObjectFromController()
    {
        var jointGameObject = controllerAttachJoint.gameObject;
        var rigidbody = jointGameObject.GetComponent<Rigidbody>();
        Object.DestroyImmediate(controllerAttachJoint);
        controllerAttachJoint = null;

        return rigidbody;
    }

    private void ThrowReleasedObject(Rigidbody rb, uint controllerIndex)
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

    private void GrabInteractedObject()
    {
        if (controllerAttachJoint == null && grabbedObject == null && IsObjectGrabbable(interactTouch.GetTouchedObject()))
        {
            grabbedObject = interactTouch.GetTouchedObject();
            OnControllerGrabInteractableObject(interactTouch.SetControllerInteractEvent(grabbedObject));
            grabbedObject.GetComponent<SteamVR_InteractableObject>().Grabbed(this.gameObject);
            if (hideControllerOnGrab)
            {
                controllerActions.ToggleControllerModel(false);
            }

            SnapObjectToGrabToController(grabbedObject);
        }
    }

    private void UngrabInteractedObject(uint controllerIndex)
    {
        if (grabbedObject != null && controllerAttachJoint != null)
        {
            OnControllerUngrabInteractableObject(interactTouch.SetControllerInteractEvent(grabbedObject));
            grabbedObject.GetComponent<SteamVR_InteractableObject>().Ungrabbed(this.gameObject);

            Rigidbody releasedObjectRigidBody = ReleaseGrabbedObjectFromController();
            ThrowReleasedObject(releasedObjectRigidBody, controllerIndex);
            if (hideControllerOnGrab)
            {
                controllerActions.ToggleControllerModel(true);
            }
            grabbedObject.GetComponent<SteamVR_InteractableObject>().ToggleHighlight(false);
            grabbedObject = null;
        }
    }

    private void DoGrabObject(object sender, ControllerClickedEventArgs e)
    {
        if (interactTouch.GetTouchedObject() != null && interactTouch.IsObjectInteractable(interactTouch.GetTouchedObject()))
        {
            GrabInteractedObject();
            if(!IsObjectHoldOnGrab(interactTouch.GetTouchedObject()))
            {
                grabEnabledState++;
            }
        }
    }

    private void DoReleaseObject(object sender, ControllerClickedEventArgs e)
    {
        if (IsObjectHoldOnGrab(grabbedObject) || grabEnabledState >= 2)
        {
            UngrabInteractedObject(e.controllerIndex);
            grabEnabledState = 0;
        }
    }
}
