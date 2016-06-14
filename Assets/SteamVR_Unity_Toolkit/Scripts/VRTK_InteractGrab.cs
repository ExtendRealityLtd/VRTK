//====================================================================================
//
// Purpose: Provide ability to grab an interactable object when it is being touched
//
// This script must be attached to a Controller within the [CameraRig] Prefab
//
// The VRTK_ControllerEvents and VRTK_InteractTouch scripts must also be
// attached to the Controller
//
// Press the default 'Trigger' button on the controller to grab an object
// Released the default 'Trigger' button on the controller to drop an object
//
//====================================================================================
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    [RequireComponent(typeof(VRTK_InteractTouch)), RequireComponent(typeof(VRTK_ControllerEvents))]
    public class VRTK_InteractGrab : MonoBehaviour
    {
        public Rigidbody controllerAttachPoint = null;
        public bool hideControllerOnGrab = false;
        public float hideControllerDelay = 0f;
        public float grabPrecognition = 0f;
        public float throwMultiplier = 1f;
        public bool createRigidBodyWhenNotTouching = false;

        public event ObjectInteractEventHandler ControllerGrabInteractableObject;
        public event ObjectInteractEventHandler ControllerUngrabInteractableObject;

        private Joint controllerAttachJoint;
        private GameObject grabbedObject = null;

        private SteamVR_TrackedObject trackedController;
        private VRTK_InteractTouch interactTouch;
        private VRTK_ControllerActions controllerActions;

        private int grabEnabledState = 0;
        private float grabPrecognitionTimer = 0f;

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
            if (grabbedObject && grabbedObject.GetComponent<VRTK_InteractableObject>() && grabbedObject.GetComponent<VRTK_InteractableObject>().AttachIsTrackObject())
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
            AttemptGrabObject();
        }

        public GameObject GetGrabbedObject()
        {
            return grabbedObject;
        }

        private void Awake()
        {
            if (GetComponent<VRTK_InteractTouch>() == null)
            {
                Debug.LogError("VRTK_InteractGrab is required to be attached to a SteamVR Controller that has the VRTK_InteractTouch script attached to it");
                return;
            }

            interactTouch = GetComponent<VRTK_InteractTouch>();
            trackedController = GetComponent<SteamVR_TrackedObject>();
            controllerActions = GetComponent<VRTK_ControllerActions>();
        }

        private void Start()
        {
            //If no attach point has been specified then just use the tip of the controller
            if (controllerAttachPoint == null)
            {
                controllerAttachPoint = transform.GetChild(0).Find("tip").GetChild(0).GetComponent<Rigidbody>();
            }

            if (GetComponent<VRTK_ControllerEvents>() == null)
            {
                Debug.LogError("VRTK_InteractGrab is required to be attached to a SteamVR Controller that has the VRTK_ControllerEvents script attached to it");
                return;
            }

            GetComponent<VRTK_ControllerEvents>().AliasGrabOn += new ControllerInteractionEventHandler(DoGrabObject);
            GetComponent<VRTK_ControllerEvents>().AliasGrabOff += new ControllerInteractionEventHandler(DoReleaseObject);
        }

        private bool IsObjectGrabbable(GameObject obj)
        {
            return (interactTouch.IsObjectInteractable(obj) && obj.GetComponent<VRTK_InteractableObject>().isGrabbable);
        }

        private bool IsObjectHoldOnGrab(GameObject obj)
        {
            return (obj && obj.GetComponent<VRTK_InteractableObject>() && obj.GetComponent<VRTK_InteractableObject>().holdButtonToGrab);
        }

        private void SnapObjectToGrabToController(GameObject obj)
        {
            //Pause collisions (if allowed on object) for a moment whilst sorting out position to prevent clipping issues
            var objectScript = obj.GetComponent<VRTK_InteractableObject>();

            objectScript.PauseCollisions();

            VRTK_InteractableObject.GrabSnapType grabType = objectScript.grabSnapType;

            if (grabType == VRTK_InteractableObject.GrabSnapType.Rotation_Snap)
            {
                // Identity Controller Rotation
                this.transform.eulerAngles = new Vector3(0f, 270f, 0f);
                obj.transform.eulerAngles = objectScript.snapToRotation;
            }

            if (grabType != VRTK_InteractableObject.GrabSnapType.Precision_Snap)
            {
                obj.transform.position = controllerAttachPoint.transform.position + objectScript.snapToPosition;
            }

            if (grabType == VRTK_InteractableObject.GrabSnapType.Handle_Snap && objectScript.snapHandle != null)
            {
                // Identity Controller Rotation
                this.transform.eulerAngles = new Vector3(0f, 270f, 0f);
                obj.transform.eulerAngles = objectScript.snapHandle.transform.localEulerAngles;

                Vector3 snapHandleDelta = objectScript.snapHandle.transform.position - obj.transform.position;
                obj.transform.position = controllerAttachPoint.transform.position - snapHandleDelta;
            }

            if (objectScript.grabAttachMechanic == VRTK_InteractableObject.GrabAttachType.Child_Of_Controller)
            {
                obj.transform.parent = this.transform;
            }
            else
            {
                CreateJoint(obj);
            }
        }

        private void CreateJoint(GameObject obj)
        {
            var objectScript = obj.GetComponent<VRTK_InteractableObject>();

            if (objectScript.grabAttachMechanic == VRTK_InteractableObject.GrabAttachType.Fixed_Joint)
            {
                controllerAttachJoint = obj.AddComponent<FixedJoint>();
            }
            else if (objectScript.grabAttachMechanic == VRTK_InteractableObject.GrabAttachType.Spring_Joint)
            {
                SpringJoint tempSpringJoint = obj.AddComponent<SpringJoint>();
                tempSpringJoint.spring = objectScript.springJointStrength;
                tempSpringJoint.damper = objectScript.springJointDamper;
                controllerAttachJoint = tempSpringJoint;
            }
            controllerAttachJoint.breakForce = objectScript.detachThreshold;
            controllerAttachJoint.connectedBody = controllerAttachPoint;
        }

        private Rigidbody ReleaseGrabbedObjectFromController(bool withThrow)
        {
            if (controllerAttachJoint != null)
            {
                return ReleaseAttachedObjectFromController(withThrow);
            }
            else
            {
                return ReleaseParentedObjectFromController();
            }
        }

        private Rigidbody ReleaseAttachedObjectFromController(bool withThrow)
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
            return rigidbody;
        }

        private void ThrowReleasedObject(Rigidbody rb, uint controllerIndex, float objectThrowMultiplier)
        {
            var origin = trackedController.origin ? trackedController.origin : trackedController.transform.parent;
            var device = SteamVR_Controller.Input((int)controllerIndex);
            if (origin != null)
            {
                rb.velocity = origin.TransformDirection(device.velocity) * (throwMultiplier * objectThrowMultiplier);
                rb.angularVelocity = origin.TransformDirection(device.angularVelocity);
            }
            else
            {
                rb.velocity = device.velocity * (throwMultiplier * objectThrowMultiplier);
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
            if (grabbedObject == null && IsObjectGrabbable(interactTouch.GetTouchedObject()))
            {
                InitGrabbedObject();
            }
        }

        private void InitGrabbedObject()
        {
            grabbedObject = interactTouch.GetTouchedObject();
            if (grabbedObject)
            {
                var grabbedObjectScript = grabbedObject.GetComponent<VRTK_InteractableObject>();

                OnControllerGrabInteractableObject(interactTouch.SetControllerInteractEvent(grabbedObject));

                grabbedObjectScript.SaveCurrentState();
                grabbedObjectScript.Grabbed(this.gameObject);
                grabbedObjectScript.ZeroVelocity();
                grabbedObjectScript.ToggleHighlight(false);
                grabbedObjectScript.ToggleKinematic(false);

                if (grabbedObjectScript.grabAttachMechanic == VRTK_InteractableObject.GrabAttachType.Child_Of_Controller)
                {
                    grabbedObjectScript.ToggleKinematic(true);
                }
            }

            if (hideControllerOnGrab)
            {
                Invoke("HideController", hideControllerDelay);
            }
        }

        private void HideController()
        {
            if (grabbedObject != null)
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
                    ThrowReleasedObject(releasedObjectRigidBody, controllerIndex, grabbedObject.GetComponent<VRTK_InteractableObject>().throwMultiplier);
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
                grabbedObject.GetComponent<VRTK_InteractableObject>().Ungrabbed(this.gameObject);
                grabbedObject.GetComponent<VRTK_InteractableObject>().ToggleHighlight(false);
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
                if (interactTouch.GetTouchedObject().GetComponent<VRTK_InteractableObject>().AttachIsTrackObject())
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

                if (grabbedObject)
                {
                    var rumbleAmount = grabbedObject.GetComponent<VRTK_InteractableObject>().rumbleOnGrab;
                    if (!rumbleAmount.Equals(Vector2.zero))
                    {
                        controllerActions.TriggerHapticPulse((int)rumbleAmount.x, (ushort)rumbleAmount.y);
                    }
                }
            }
            else
            {
                grabPrecognitionTimer = grabPrecognition;
                if (createRigidBodyWhenNotTouching)
                {
                    interactTouch.ToggleControllerRigidBody(true);
                }
            }
        }

        private bool CanRelease()
        {
            return (grabbedObject && grabbedObject.GetComponent<VRTK_InteractableObject>().isDroppable);
        }

        private void AttemptReleaseObject(uint controllerIndex)
        {
            if (CanRelease() && (IsObjectHoldOnGrab(grabbedObject) || grabEnabledState >= 2))
            {
                if (grabbedObject.GetComponent<VRTK_InteractableObject>().AttachIsTrackObject())
                {
                    UngrabTrackedObject();
                }
                else
                {
                    ReleaseObject(controllerIndex, true);
                }
            }
            interactTouch.ToggleControllerRigidBody(false);
        }

        private void DoGrabObject(object sender, ControllerInteractionEventArgs e)
        {
            AttemptGrabObject();
        }

        private void DoReleaseObject(object sender, ControllerInteractionEventArgs e)
        {
            AttemptReleaseObject(e.controllerIndex);
        }

        private void Update()
        {
            if (grabPrecognitionTimer > 0)
            {
                grabPrecognitionTimer--;
                if (IsValidGrab())
                {
                    AttemptGrabObject();
                }
            }
        }
    }
}