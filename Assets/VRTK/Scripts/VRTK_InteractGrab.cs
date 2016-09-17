// Interact Grab|Scripts|0180
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Interact Grab script is attached to a Controller object within the `[CameraRig]` prefab and the Controller object requires the `VRTK_ControllerEvents` script to be attached as it uses this for listening to the controller button events for grabbing and releasing interactable game objects. It listens for the `AliasGrabOn` and `AliasGrabOff` events to determine when an object should be grabbed and should be released.
    /// </summary>
    /// <remarks>
    /// The Controller object also requires the `VRTK_InteractTouch` script to be attached to it as this is used to determine when an interactable object is being touched. Only valid touched objects can be grabbed.
    ///
    /// An object can be grabbed if the Controller touches a game object which contains the `VRTK_InteractableObject` script and has the flag `isGrabbable` set to `true`.
    ///
    /// If a valid interactable object is grabbable then pressing the set `Grab` button on the Controller (default is `Grip`) will grab and snap the object to the controller and will not release it until the `Grab` button is released.
    ///
    /// When the Controller `Grab` button is released, if the interactable game object is grabbable then it will be propelled in the direction and at the velocity the controller was at, which can simulate object throwing.
    ///
    /// The interactable objects require a collider to activate the trigger and a rigidbody to pick them up and move them around the game world.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/005_Controller/BasicObjectGrabbing` demonstrates the grabbing of interactable objects that have the `VRTK_InteractableObject` script attached to them. The objects can be picked up and thrown around.
    ///
    /// `VRTK/Examples/013_Controller_UsingAndGrabbingMultipleObjects` demonstrates that each controller can grab and use objects independently and objects can also be toggled to their use state simultaneously.
    ///
    /// `VRTK/Examples/014_Controller_SnappingObjectsOnGrab` demonstrates the different mechanisms for snapping a grabbed object to the controller.
    /// </example>
    [RequireComponent(typeof(VRTK_InteractTouch)), RequireComponent(typeof(VRTK_ControllerEvents))]
    public class VRTK_InteractGrab : MonoBehaviour
    {
        [Tooltip("The rigidbody point on the controller model to snap the grabbed object to (defaults to the tip).")]
        public Rigidbody controllerAttachPoint = null;
        [Tooltip("Hides the controller model when a valid grab occurs.")]
        public bool hideControllerOnGrab = false;
        [Tooltip("The amount of seconds to wait before hiding the controller on grab.")]
        public float hideControllerDelay = 0f;
        [Tooltip("An amount of time between when the grab button is pressed to when the controller is touching something to grab it. For example, if an object is falling at a fast rate, then it is very hard to press the grab button in time to catch the object due to human reaction times. A higher number here will mean the grab button can be pressed before the controller touches the object and when the collision takes place, if the grab button is still being held down then the grab action will be successful.")]
        public float grabPrecognition = 0f;
        [Tooltip("An amount to multiply the velocity of any objects being thrown. This can be useful when scaling up the `[CameraRig]` to simulate being able to throw items further.")]
        public float throwMultiplier = 1f;
        [Tooltip("If this is checked and the controller is not touching an Interactable Object when the grab button is pressed then a rigid body is added to the controller to allow the controller to push other rigid body objects around.")]
        public bool createRigidBodyWhenNotTouching = false;

        /// <summary>
        /// Emitted when a valid object is grabbed.
        /// </summary>
        public event ObjectInteractEventHandler ControllerGrabInteractableObject;
        /// <summary>
        /// Emitted when a valid object is released from being grabbed.
        /// </summary>
        public event ObjectInteractEventHandler ControllerUngrabInteractableObject;

        private Joint controllerAttachJoint;
        private GameObject grabbedObject = null;
        private bool updatedHideControllerOnGrab = false;
        private VRTK_InteractTouch interactTouch;
        private VRTK_ControllerActions controllerActions;
        private VRTK_ControllerEvents controllerEvents;
        private int grabEnabledState = 0;
        private float grabPrecognitionTimer = 0f;
        private GameObject undroppableGrabbedObject;

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

        /// <summary>
        /// The ForceRelease method will force the controller to stop grabbing the currently grabbed object.
        /// </summary>
        public void ForceRelease()
        {
            if (grabbedObject != null && grabbedObject.GetComponent<VRTK_InteractableObject>() && grabbedObject.GetComponent<VRTK_InteractableObject>().AttachIsUnthrowableObject())
            {
                UngrabTrackedObject();
            }
            else
            {
                ReleaseObject(false);
            }
        }

        /// <summary>
        /// The AttemptGrab method will attempt to grab the currently touched object without needing to press the grab button on the controller.
        /// </summary>
        public void AttemptGrab()
        {
            AttemptGrabObject();
        }

        /// <summary>
        /// The GetGrabbedObject method returns the current object being grabbed by the controller.
        /// </summary>
        /// <returns>The game object of what is currently being grabbed by this controller.</returns>
        public GameObject GetGrabbedObject()
        {
            return grabbedObject;
        }

        private void Awake()
        {
            if (GetComponent<VRTK_InteractTouch>() == null)
            {
                Debug.LogError("VRTK_InteractGrab is required to be attached to a Controller that has the VRTK_InteractTouch script attached to it");
                return;
            }

            interactTouch = GetComponent<VRTK_InteractTouch>();
            controllerActions = GetComponent<VRTK_ControllerActions>();
            controllerEvents = GetComponent<VRTK_ControllerEvents>();
        }

        private void OnEnable()
        {
            if (GetComponent<VRTK_ControllerEvents>() == null)
            {
                Debug.LogError("VRTK_InteractGrab is required to be attached to a Controller that has the VRTK_ControllerEvents script attached to it");
                return;
            }

            if (undroppableGrabbedObject && !undroppableGrabbedObject.GetComponent<VRTK_InteractableObject>().IsGrabbed())
            {
                undroppableGrabbedObject.SetActive(true);
                interactTouch.ForceTouch(undroppableGrabbedObject);
                AttemptGrab();
            }
            else
            {
                undroppableGrabbedObject = null;
            }

            GetComponent<VRTK_ControllerEvents>().AliasGrabOn += new ControllerInteractionEventHandler(DoGrabObject);
            GetComponent<VRTK_ControllerEvents>().AliasGrabOff += new ControllerInteractionEventHandler(DoReleaseObject);

            SetControllerAttachPoint();
        }

        private void OnDisable()
        {
            if (undroppableGrabbedObject)
            {
                if (undroppableGrabbedObject.GetComponent<VRTK_InteractableObject>().isDroppable)
                {
                    undroppableGrabbedObject = null;
                }
                else
                {
                    undroppableGrabbedObject.SetActive(false);
                }
            }
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
                var defaultAttachPoint = transform.Find(VRTK_SDK_Bridge.defaultAttachPointPath);
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
            controllerAttachJoint.breakForce = (objectScript.isDroppable ? objectScript.detachThreshold : Mathf.Infinity);
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

        private void ThrowReleasedObject(Rigidbody rb, float objectThrowMultiplier)
        {
            var origin = VRTK_DeviceFinder.TrackedObjectOrigin(gameObject);

            var velocity = controllerEvents.GetVelocity();
            var angularVelocity = controllerEvents.GetAngularVelocity();

            if (origin != null)
            {
                rb.velocity = origin.TransformDirection(velocity) * (throwMultiplier * objectThrowMultiplier);
                rb.angularVelocity = origin.TransformDirection(angularVelocity);
            }
            else
            {
                rb.velocity = velocity * (throwMultiplier * objectThrowMultiplier);
                rb.angularVelocity = angularVelocity;
            }
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
                    interactTouch.ForceStopTouching();
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

        private void UngrabInteractedObject(bool withThrow)
        {
            if (grabbedObject != null)
            {
                Rigidbody releasedObjectRigidBody = ReleaseGrabbedObjectFromController(withThrow);
                if (withThrow)
                {
                    ThrowReleasedObject(releasedObjectRigidBody, grabbedObject.GetComponent<VRTK_InteractableObject>().throwMultiplier);
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
            interactTouch.ForceStopTouching();
        }

        private void ReleaseObject(bool withThrow)
        {
            UngrabInteractedObject(withThrow);
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

                undroppableGrabbedObject = (grabbedObject && grabbedObject.GetComponent<VRTK_InteractableObject>() && !grabbedObject.GetComponent<VRTK_InteractableObject>().isDroppable ? grabbedObject : null);

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

        private void AttemptReleaseObject()
        {
            if (CanRelease() && (IsObjectHoldOnGrab(grabbedObject) || grabEnabledState >= 2))
            {
                if (grabbedObject.GetComponent<VRTK_InteractableObject>().AttachIsUnthrowableObject())
                {
                    UngrabTrackedObject();
                }
                else if (grabbedObject.GetComponent<VRTK_InteractableObject>().AttachIsClimbObject())
                {
                    UngrabClimbObject();
                }
                else
                {
                    ReleaseObject(true);
                }
            }
        }

        private void DoGrabObject(object sender, ControllerInteractionEventArgs e)
        {
            AttemptGrabObject();
        }

        private void DoReleaseObject(object sender, ControllerInteractionEventArgs e)
        {
            AttemptReleaseObject();
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