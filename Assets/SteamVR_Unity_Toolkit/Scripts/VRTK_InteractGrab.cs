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
        private bool updatedHideControllerOnGrab = false;

        private SteamVR_TrackedObject trackedController;
        private VRTK_InteractTouch interactTouch;
        private VRTK_ControllerActions controllerActions;
        private VRTK_ControllerEvents controllerEvents;

        private int grabEnabledState = 0;
        private float grabPrecognitionTimer = 0f;

        public virtual void OnControllerGrabInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerGrabInteractableObject != null)
            {
                ControllerGrabInteractableObject(this, e);
            }
        }

        public virtual void OnControllerUngrabInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerUngrabInteractableObject != null)
            {
                ControllerUngrabInteractableObject(this, e);
            }
        }

        public void ForceRelease()
        {
            if (grabbedObject != null && grabbedObject.GetComponent<VRTK_InteractableObject>() && grabbedObject.GetComponent<VRTK_InteractableObject>().AttachIsTrackObject())
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
            controllerEvents = GetComponent<VRTK_ControllerEvents>();
        }

        private void OnEnable()
        {
            if (GetComponent<VRTK_ControllerEvents>() == null)
            {
                Debug.LogError("VRTK_InteractGrab is required to be attached to a SteamVR Controller that has the VRTK_ControllerEvents script attached to it");
                return;
            }

            GetComponent<VRTK_ControllerEvents>().AliasGrabOn += new ControllerInteractionEventHandler(DoGrabObject);
            GetComponent<VRTK_ControllerEvents>().AliasGrabOff += new ControllerInteractionEventHandler(DoReleaseObject);

            SetControllerAttachPoint();
        }

        private void OnDisable()
        {
            ForceRelease();
            GetComponent<VRTK_ControllerEvents>().AliasGrabOn -= new ControllerInteractionEventHandler(DoGrabObject);
            GetComponent<VRTK_ControllerEvents>().AliasGrabOff -= new ControllerInteractionEventHandler(DoReleaseObject);
        }

        private void SetControllerAttachPoint()
        {
            //If no attach point has been specified then just use the tip of the controller
            if (controllerAttachPoint == null)
            {
                //attempt to find the attach point on the controller
                var defaultAttachPoint = transform.Find("Model/tip/attach");
                if (defaultAttachPoint != null)
                {
                    controllerAttachPoint = defaultAttachPoint.GetComponent<Rigidbody>();

                    if (controllerAttachPoint == null)
                    {
                        var autoGenRB = defaultAttachPoint.gameObject.AddComponent<Rigidbody>();
                        autoGenRB.isKinematic = true;
                        controllerAttachPoint = autoGenRB;
                    }
                }
            }
        }

        private bool IsObjectGrabbable(GameObject obj)
        {
            return (interactTouch.IsObjectInteractable(obj) && obj.GetComponent<VRTK_InteractableObject>().isGrabbable);
        }

        private bool IsObjectHoldOnGrab(GameObject obj)
        {
            return (obj && obj.GetComponent<VRTK_InteractableObject>() && obj.GetComponent<VRTK_InteractableObject>().holdButtonToGrab);
        }

        private Transform GetSnapHandle(VRTK_InteractableObject objectScript)
        {
            if (objectScript.rightSnapHandle == null && objectScript.leftSnapHandle != null)
            {
                objectScript.rightSnapHandle = objectScript.leftSnapHandle;
            }

            if (objectScript.leftSnapHandle == null && objectScript.rightSnapHandle != null)
            {
                objectScript.leftSnapHandle = objectScript.rightSnapHandle;
            }

            if (VRTK_DeviceFinder.IsControllerOfHand(gameObject, VRTK_DeviceFinder.ControllerHand.Right))
            {
                return objectScript.rightSnapHandle;
            }

            if (VRTK_DeviceFinder.IsControllerOfHand(gameObject, VRTK_DeviceFinder.ControllerHand.Left))
            {
                return objectScript.leftSnapHandle;
            }

            return null;
        }

        private void SetSnappedObjectPosition(GameObject obj)
        {
            var objectScript = obj.GetComponent<VRTK_InteractableObject>();

            if (objectScript.rightSnapHandle == null && objectScript.leftSnapHandle == null)
            {
                obj.transform.position = controllerAttachPoint.transform.position;
            }
            else
            {
                var snapHandle = GetSnapHandle(objectScript);
                objectScript.SetGrabbedSnapHandle(snapHandle);

                obj.transform.rotation = controllerAttachPoint.transform.rotation * Quaternion.Euler(snapHandle.transform.localEulerAngles);
                obj.transform.position = controllerAttachPoint.transform.position - (snapHandle.transform.position - obj.transform.position);
            }
        }

        private void SnapObjectToGrabToController(GameObject obj)
        {
            var objectScript = obj.GetComponent<VRTK_InteractableObject>();

            if (!objectScript.precisionSnap)
            {
                SetSnappedObjectPosition(obj);
            }

            if (objectScript.grabAttachMechanic == VRTK_InteractableObject.GrabAttachType.Child_Of_Controller)
            {
                obj.transform.parent = controllerAttachPoint.transform;
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
                if (objectScript.precisionSnap)
                {
                    tempSpringJoint.anchor = obj.transform.InverseTransformPoint(controllerAttachPoint.position);
                }
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
                DestroyImmediate(controllerAttachJoint);
            }
            else
            {
                Destroy(controllerAttachJoint);
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

        private bool GrabInteractedObject()
        {
            if (controllerAttachJoint == null && grabbedObject == null && IsObjectGrabbable(interactTouch.GetTouchedObject()))
            {
                InitGrabbedObject();
                if (grabbedObject)
                {
                    SnapObjectToGrabToController(grabbedObject);
                    return true;
                }
            }
            return false;
        }

        private bool GrabTrackedObject()
        {
            if (grabbedObject == null && IsObjectGrabbable(interactTouch.GetTouchedObject()))
            {
                InitGrabbedObject();
                if (grabbedObject)
                {
                    var objectScript = grabbedObject.GetComponent<VRTK_InteractableObject>();
                    objectScript.SetGrabbedSnapHandle(GetSnapHandle(objectScript));
                    return true;
                }
            }
            return false;
        }

        private bool GrabClimbObject()
        {
            if (grabbedObject == null && IsObjectGrabbable(interactTouch.GetTouchedObject()))
            {
                InitGrabbedObject();
                if (grabbedObject)
                {
                    return true;
                }
            }
            return false;
        }

        private void InitGrabbedObject()
        {
            grabbedObject = interactTouch.GetTouchedObject();
            if (grabbedObject)
            {
                var grabbedObjectScript = grabbedObject.GetComponent<VRTK_InteractableObject>();

                if (!grabbedObjectScript.IsValidInteractableController(gameObject, grabbedObjectScript.allowedGrabControllers))
                {
                    grabbedObject = null;
                    return;
                }

                OnControllerGrabInteractableObject(interactTouch.SetControllerInteractEvent(grabbedObject));

                grabbedObjectScript.SaveCurrentState();
                grabbedObjectScript.Grabbed(gameObject);
                grabbedObjectScript.ZeroVelocity();
                grabbedObjectScript.ToggleHighlight(false);
                grabbedObjectScript.ToggleKinematic(false);

                //Pause collisions (if allowed on object) for a moment whilst sorting out position to prevent clipping issues
                grabbedObjectScript.PauseCollisions();

                if (grabbedObjectScript.grabAttachMechanic == VRTK_InteractableObject.GrabAttachType.Child_Of_Controller)
                {
                    grabbedObjectScript.ToggleKinematic(true);
                }
                updatedHideControllerOnGrab = grabbedObjectScript.CheckHideMode(hideControllerOnGrab, grabbedObjectScript.hideControllerOnGrab);
            }

            if (updatedHideControllerOnGrab)
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

        private void UngrabClimbObject()
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
                grabbedObject.GetComponent<VRTK_InteractableObject>().Ungrabbed(gameObject);
                grabbedObject.GetComponent<VRTK_InteractableObject>().ToggleHighlight(false);
            }

            if (updatedHideControllerOnGrab)
            {
                controllerActions.ToggleControllerModel(true, grabbedObject);
            }

            grabEnabledState = 0;
            grabbedObject = null;
        }

        private void ReleaseObject(uint controllerIndex, bool withThrow)
        {
            UngrabInteractedObject(controllerIndex, withThrow);
        }

        private GameObject GetGrabbableObject()
        {
            GameObject obj = interactTouch.GetTouchedObject();
            if (obj != null && interactTouch.IsObjectInteractable(obj))
            {
                return obj;
            }
            return grabbedObject;
        }

        private void IncrementGrabState()
        {
            if (!IsObjectHoldOnGrab(interactTouch.GetTouchedObject()))
            {
                grabEnabledState++;
            }
        }

        private void AttemptGrabObject()
        {
            var objectToGrab = GetGrabbableObject();
            if (objectToGrab != null)
            {
                IncrementGrabState();
                var initialGrabAttempt = false;

                if (objectToGrab.GetComponent<VRTK_InteractableObject>().AttachIsTrackObject())
                {
                    initialGrabAttempt = GrabTrackedObject();
                }
                else if (objectToGrab.GetComponent<VRTK_InteractableObject>().AttachIsClimbObject())
                {
                    initialGrabAttempt = GrabClimbObject();
                }
                else
                {
                    initialGrabAttempt = GrabInteractedObject();
                }

                if (grabbedObject && initialGrabAttempt)
                {
                    var rumbleAmount = grabbedObject.GetComponent<VRTK_InteractableObject>().rumbleOnGrab;
                    if (!rumbleAmount.Equals(Vector2.zero))
                    {
                        controllerActions.TriggerHapticPulse((ushort)rumbleAmount.y, rumbleAmount.x, 0.05f);
                    }
                }
            }
            else
            {
                grabPrecognitionTimer = Time.time + grabPrecognition;
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
                else if (grabbedObject.GetComponent<VRTK_InteractableObject>().AttachIsClimbObject())
                {
                    UngrabClimbObject();
                }
                else
                {
                    ReleaseObject(controllerIndex, true);
                }
            }
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
            if (controllerAttachPoint == null)
            {
                SetControllerAttachPoint();
            }

            if (createRigidBodyWhenNotTouching && grabbedObject == null)
            {
                if (interactTouch.IsRigidBodyActive() != controllerEvents.grabPressed)
                {
                    interactTouch.ToggleControllerRigidBody(controllerEvents.grabPressed);
                }
            }

            if (grabPrecognitionTimer >= Time.time)
            {
                if (GetGrabbableObject() != null)
                {
                    AttemptGrabObject();
                    if (GetGrabbedObject() != null)
                    {
                        grabPrecognitionTimer = 0f;
                    }
                }
            }
        }
    }
}