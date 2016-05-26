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
    public float hideControllerDelay = 0f;
    public float grabPrecognition = 0f;
    public bool createRigidBodyWhenNotTouching = false;

    public event ObjectInteractEventHandler ControllerGrabInteractableObject;
    public event ObjectInteractEventHandler ControllerUngrabInteractableObject;

    private GameObject controllerRigidBody;
    private Joint controllerAttachJoint;
    private GameObject grabbedObject = null;

    private SteamVR_InteractTouch interactTouch;
    private SteamVR_TrackedObject trackedController;
    private SteamVR_ControllerActions controllerActions;

    private int grabEnabledState = 0;
    private float grabPrecognitionTimer = 0f;

    private Vector3 controllerRigidBodyPosition = new Vector3(0f, -0.04f, 0f);
    private Transform lastParentController;

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

    public void ForceRelease()
    {
        if (grabbedObject && grabbedObject.GetComponent<SteamVR_InteractableObject>() && grabbedObject.GetComponent<SteamVR_InteractableObject>().AttatchIsTrackObject())
        {
            UngrabTrackedObject();
        }
        else
        {
            ReleaseObject((uint)trackedController.index, false);
        }
    }

    public void AttemptGrab()
    {
        GrabInteractedObject();
    }

    public GameObject GetGrabbedObject()
    {
        return grabbedObject;
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

        CreateRigidBodyPoint();
    }

    private void CreateRigidBodyPoint()
    {
        controllerRigidBody = new GameObject(string.Format("[{0}]_RigidBody_Holder", this.gameObject.name));
        controllerRigidBody.transform.parent = this.transform;

        SphereCollider sc = controllerRigidBody.AddComponent<SphereCollider>();
        Rigidbody rb = controllerRigidBody.AddComponent<Rigidbody>();

        sc.radius = 0.35f;
        sc.center = new Vector3(0f, 0f, 0f);

        controllerRigidBody.transform.localPosition = controllerRigidBodyPosition;
        controllerRigidBody.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);

        rb.useGravity = false;
        rb.mass = 100f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        ToggleRigidBody(false);
    }

    private void ToggleRigidBody(bool state)
    {
        state = (!createRigidBodyWhenNotTouching ? false : state);
        controllerRigidBody.GetComponent<Rigidbody>().detectCollisions = state;
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
        //Pause collisions (if allowed on object) for a moment whilst sorting out position to prevent clipping issues
        obj.GetComponent<SteamVR_InteractableObject>().PauseCollisions(0.2f);

        SteamVR_InteractableObject.GrabSnapType grabType = obj.GetComponent<SteamVR_InteractableObject>().grabSnapType;

        if (grabType == SteamVR_InteractableObject.GrabSnapType.Rotation_Snap)
        {
            // Identity Controller Rotation
            this.transform.eulerAngles = new Vector3(0f, 270f, 0f);
            obj.transform.eulerAngles = obj.GetComponent<SteamVR_InteractableObject>().snapToRotation;
        }

        if (grabType != SteamVR_InteractableObject.GrabSnapType.Precision_Snap)
        {
            obj.transform.position = controllerAttachPoint.transform.position + obj.GetComponent<SteamVR_InteractableObject>().snapToPosition;
        }

        if (obj.GetComponent<SteamVR_InteractableObject>().grabAttatchMechanic == SteamVR_InteractableObject.GrabAttatchType.Child_Of_Controller)
        {
            SetControllerAsParent(obj);
        } else
        {
            CreateJoint(obj);
        }
    }

    private void SetControllerAsParent(GameObject obj)
    {
        obj.transform.parent = this.transform;
        if (obj.GetComponent<Rigidbody>())
        {
            obj.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void CreateJoint(GameObject obj)
    {
        if (obj.GetComponent<SteamVR_InteractableObject>().grabAttatchMechanic == SteamVR_InteractableObject.GrabAttatchType.Fixed_Joint)
        {
            controllerAttachJoint = obj.AddComponent<FixedJoint>();
        }
        else if (obj.GetComponent<SteamVR_InteractableObject>().grabAttatchMechanic == SteamVR_InteractableObject.GrabAttatchType.Spring_Joint)
        {
            SpringJoint tempSpringJoint = obj.AddComponent<SpringJoint>();
            tempSpringJoint.spring = obj.GetComponent<SteamVR_InteractableObject>().springJointStrength;
            tempSpringJoint.damper = obj.GetComponent<SteamVR_InteractableObject>().springJointDamper;
            controllerAttachJoint = tempSpringJoint;
        }
        controllerAttachJoint.breakForce = obj.GetComponent<SteamVR_InteractableObject>().detatchThreshold;
        controllerAttachJoint.connectedBody = controllerAttachPoint;
    }

    private Rigidbody ReleaseGrabbedObjectFromController(bool withThrow)
    {
        if (controllerAttachJoint != null)
        {
            return ReleaseAttatchedObjectFromController(withThrow);
        } else
        {
            return ReleaseParentedObjectFromController();
        }
    }

    private Rigidbody ReleaseAttatchedObjectFromController(bool withThrow)
    {
        var jointGameObject = controllerAttachJoint.gameObject;
        var rigidbody = jointGameObject.GetComponent<Rigidbody>();
        if (withThrow)
        {
            Object.DestroyImmediate(controllerAttachJoint);
        }
        else
        {
            Object.Destroy(controllerAttachJoint);
        }
        controllerAttachJoint = null;

        return rigidbody;
    }

    private Rigidbody ReleaseParentedObjectFromController()
    {
        var rigidbody = grabbedObject.GetComponent<Rigidbody>();
        grabbedObject.transform.parent = lastParentController;
        rigidbody.isKinematic = false;
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
            InitGrabbedObject();
            SnapObjectToGrabToController(grabbedObject);
        }
    }

    private void GrabTrackedObject()
    {
        if (grabbedObject == null && IsObjectGrabbable(interactTouch.GetTouchedObject())) {
            InitGrabbedObject();
        }
    }

    private void InitGrabbedObject()
    {
        grabbedObject = interactTouch.GetTouchedObject();
        OnControllerGrabInteractableObject(interactTouch.SetControllerInteractEvent(grabbedObject));
        grabbedObject.GetComponent<SteamVR_InteractableObject>().Grabbed(this.gameObject);
        grabbedObject.GetComponent<SteamVR_InteractableObject>().ZeroVelocity();
        if (grabbedObject)
        {
            grabbedObject.GetComponent<SteamVR_InteractableObject>().ToggleHighlight(false);
        }
        if (hideControllerOnGrab)
        {
            Invoke("HideController", hideControllerDelay);
        }
    }

    private void HideController()
    {
        if(grabbedObject != null)
        {
            controllerActions.ToggleControllerModel(false, grabbedObject);
        }
    }

    private void UngrabInteractedObject(uint controllerIndex, bool withThrow)
    {
        if (grabbedObject != null)
        {
            Rigidbody releasedObjectRigidBody = ReleaseGrabbedObjectFromController(withThrow);
            if (withThrow)
            {
                ThrowReleasedObject(releasedObjectRigidBody, controllerIndex);
            }
        }
        InitUngrabbedObject();
    }

    private void UngrabTrackedObject()
    {
        if (grabbedObject != null)
        {
            InitUngrabbedObject();
        }
    }

    private void InitUngrabbedObject()
    {
        OnControllerUngrabInteractableObject(interactTouch.SetControllerInteractEvent(grabbedObject));
        if (grabbedObject != null)
        {
            grabbedObject.GetComponent<SteamVR_InteractableObject>().Ungrabbed(this.gameObject);
            grabbedObject.GetComponent<SteamVR_InteractableObject>().ToggleHighlight(false);
        }

        if (hideControllerOnGrab)
        {
            controllerActions.ToggleControllerModel(true, grabbedObject);
        }

        grabbedObject = null;
    }

    private void ReleaseObject(uint controllerIndex, bool withThrow)
    {
        UngrabInteractedObject(controllerIndex, withThrow);
        grabEnabledState = 0;
    }

    private bool IsValidGrab()
    {
        GameObject obj = interactTouch.GetTouchedObject();
        return (obj != null && interactTouch.IsObjectInteractable(obj));
    }

    private void AttemptGrabObject()
    {
        if (IsValidGrab())
        {
            if (interactTouch.GetTouchedObject().GetComponent<SteamVR_InteractableObject>().AttatchIsTrackObject())
            {
                GrabTrackedObject();
            }
            else
            {
                GrabInteractedObject();
            }

            if (!IsObjectHoldOnGrab(interactTouch.GetTouchedObject()))
            {
                grabEnabledState++;
            }
        } else
        {
            grabPrecognitionTimer = grabPrecognition;
            ToggleRigidBody(true);
        }
    }

    private void AttemptReleaseObject(uint controllerIndex)
    {
        if (IsObjectHoldOnGrab(grabbedObject) || grabEnabledState >= 2)
        {
            if (grabbedObject.GetComponent<SteamVR_InteractableObject>().AttatchIsTrackObject())
            {
                UngrabTrackedObject();
            }
            else
            {
                ReleaseObject(controllerIndex, true);
            }
        }
        ToggleRigidBody(false);
    }

    private void DoGrabObject(object sender, ControllerClickedEventArgs e)
    {
        AttemptGrabObject();
    }

    private void DoReleaseObject(object sender, ControllerClickedEventArgs e)
    {
        AttemptReleaseObject(e.controllerIndex);
    }

    private void Update()
    {
        controllerRigidBody.transform.localPosition = controllerRigidBodyPosition;
        if (grabPrecognitionTimer > 0)
        {
            grabPrecognitionTimer--;
            if(IsValidGrab())
            {
                AttemptGrabObject();
            }
        }
    }
}
