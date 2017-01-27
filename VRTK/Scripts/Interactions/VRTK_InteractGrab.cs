// Interact Grab|Interactions|30050
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Interact Grab script is attached to a Controller object and requires the `VRTK_ControllerEvents` script to be attached as it uses this for listening to the controller button events for grabbing and releasing interactable game objects.
    /// </summary>
    /// <remarks>
    /// It listens for the `AliasGrabOn` and `AliasGrabOff` events to determine when an object should be grabbed and should be released.
    ///
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
        [Tooltip("The rigidbody point on the controller model to snap the grabbed object to. If blank it will be set to the SDK default.")]
        public Rigidbody controllerAttachPoint = null;
        [Tooltip("An amount of time between when the grab button is pressed to when the controller is touching something to grab it. For example, if an object is falling at a fast rate, then it is very hard to press the grab button in time to catch the object due to human reaction times. A higher number here will mean the grab button can be pressed before the controller touches the object and when the collision takes place, if the grab button is still being held down then the grab action will be successful.")]
        public float grabPrecognition = 0f;
        [Tooltip("An amount to multiply the velocity of any objects being thrown. This can be useful when scaling up the play area to simulate being able to throw items further.")]
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

        private GameObject grabbedObject = null;
        private bool influencingGrabbedObject = false;
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
        /// <param name="applyGrabbingObjectVelocity">If this is true then upon releasing the object any velocity on the grabbing object will be applied to the object to essentiall throw it. Defaults to `false`.</param>
        public void ForceRelease(bool applyGrabbingObjectVelocity = false)
        {
            InitUngrabbedObject(applyGrabbingObjectVelocity);
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

        protected virtual void Awake()
        {
            interactTouch = GetComponent<VRTK_InteractTouch>();
            controllerActions = GetComponent<VRTK_ControllerActions>();
            controllerEvents = GetComponent<VRTK_ControllerEvents>();
        }

        protected virtual void OnEnable()
        {
            RegrabUndroppableObject();

            controllerEvents.AliasGrabOn += new ControllerInteractionEventHandler(DoGrabObject);
            controllerEvents.AliasGrabOff += new ControllerInteractionEventHandler(DoReleaseObject);

            SetControllerAttachPoint();
        }

        protected virtual void OnDisable()
        {
            SetUndroppableObject();
            ForceRelease();
            controllerEvents.AliasGrabOn -= new ControllerInteractionEventHandler(DoGrabObject);
            controllerEvents.AliasGrabOff -= new ControllerInteractionEventHandler(DoReleaseObject);
        }

        protected virtual void Update()
        {
            CheckControllerAttachPointSet();
            CreateNonTouchingRigidbody();
            CheckPrecognitionGrab();
        }

        private void RegrabUndroppableObject()
        {
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
        }

        private void SetUndroppableObject()
        {
            if (undroppableGrabbedObject)
            {
                if (undroppableGrabbedObject.GetComponent<VRTK_InteractableObject>().IsDroppable())
                {
                    undroppableGrabbedObject = null;
                }
                else
                {
                    undroppableGrabbedObject.SetActive(false);
                }
            }
        }

        private void SetControllerAttachPoint()
        {
            var modelController = VRTK_DeviceFinder.GetModelAliasController(gameObject);
            //If no attach point has been specified then just use the tip of the controller
            if (modelController && controllerAttachPoint == null)
            {
                //attempt to find the attach point on the controller
                var defaultAttachPoint = modelController.transform.Find(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.AttachPoint, VRTK_DeviceFinder.GetControllerHand(gameObject)));
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
            var objScript = obj.GetComponent<VRTK_InteractableObject>();
            return (interactTouch.IsObjectInteractable(obj) && objScript && (objScript.isGrabbable || objScript.PerformSecondaryAction()));
        }

        private bool IsObjectHoldOnGrab(GameObject obj)
        {
            if (obj)
            {
                var objScript = obj.GetComponent<VRTK_InteractableObject>();
                return (objScript && objScript.holdButtonToGrab);
            }
            return false;
        }

        private void ChooseGrabSequence(VRTK_InteractableObject grabbedObjectScript)
        {
            if (!grabbedObjectScript.IsGrabbed() || grabbedObjectScript.IsSwappable())
            {
                InitPrimaryGrab(grabbedObjectScript);
            }
            else
            {
                InitSecondaryGrab(grabbedObjectScript);
            }
        }

        private void ToggleControllerVisibility(bool visible)
        {
            if (grabbedObject)
            {
                var controllerAppearanceScript = grabbedObject.GetComponentInParent<VRTK_InteractControllerAppearance>();
                if (controllerAppearanceScript)
                {
                    controllerAppearanceScript.ToggleControllerOnGrab(visible, controllerActions, grabbedObject);
                }
            }
            else if (visible)
            {
                controllerActions.ToggleControllerModel(true, grabbedObject);
            }
        }

        private void InitGrabbedObject()
        {
            grabbedObject = interactTouch.GetTouchedObject();
            if (grabbedObject)
            {
                var grabbedObjectScript = grabbedObject.GetComponent<VRTK_InteractableObject>();
                ChooseGrabSequence(grabbedObjectScript);
                ToggleControllerVisibility(false);
                OnControllerGrabInteractableObject(interactTouch.SetControllerInteractEvent(grabbedObject));
            }
        }

        private void InitPrimaryGrab(VRTK_InteractableObject currentGrabbedObject)
        {
            var grabbingObject = gameObject;

            if (!currentGrabbedObject.IsValidInteractableController(gameObject, currentGrabbedObject.allowedGrabControllers))
            {
                grabbedObject = null;
                if (currentGrabbedObject.IsGrabbed(grabbingObject))
                {
                    interactTouch.ForceStopTouching();
                }
                return;
            }

            influencingGrabbedObject = false;
            currentGrabbedObject.SaveCurrentState();
            currentGrabbedObject.Grabbed(grabbingObject);
            currentGrabbedObject.ZeroVelocity();
            currentGrabbedObject.ToggleHighlight(false);
            currentGrabbedObject.isKinematic = false;
        }

        private void InitSecondaryGrab(VRTK_InteractableObject currentGrabbedObject)
        {
            var grabbingObject = gameObject;

            if (!currentGrabbedObject.IsValidInteractableController(gameObject, currentGrabbedObject.allowedGrabControllers))
            {
                grabbedObject = null;
                influencingGrabbedObject = false;
                currentGrabbedObject.Ungrabbed(grabbingObject);
                return;
            }

            influencingGrabbedObject = true;
            currentGrabbedObject.Grabbed(grabbingObject);
        }

        private void CheckInfluencingObjectOnRelease()
        {
            if (!influencingGrabbedObject)
            {
                interactTouch.ForceStopTouching();
            }
            influencingGrabbedObject = false;
        }

        private void InitUngrabbedObject(bool applyGrabbingObjectVelocity)
        {
            var grabbingObject = gameObject;

            if (grabbedObject != null)
            {
                var grabbedObjectScript = grabbedObject.GetComponent<VRTK_InteractableObject>();
                if (!influencingGrabbedObject)
                {
                    grabbedObjectScript.grabAttachMechanicScript.StopGrab(applyGrabbingObjectVelocity);
                }
                grabbedObjectScript.Ungrabbed(grabbingObject);
                grabbedObjectScript.ToggleHighlight(false);
            }

            CheckInfluencingObjectOnRelease();
            ToggleControllerVisibility(true);
            OnControllerUngrabInteractableObject(interactTouch.SetControllerInteractEvent(grabbedObject));

            grabEnabledState = 0;
            grabbedObject = null;
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

        private GameObject GetUndroppableObject()
        {
            if (grabbedObject)
            {
                var grabbedObjectScript = grabbedObject.GetComponent<VRTK_InteractableObject>();
                return (grabbedObjectScript && !grabbedObjectScript.IsDroppable() ? grabbedObject : null);
            }
            return null;
        }

        private void AttemptHaptics(bool initialGrabAttempt)
        {
            if (grabbedObject && initialGrabAttempt)
            {
                var doHaptics = grabbedObject.GetComponentInParent<VRTK_InteractHaptics>();
                if (doHaptics)
                {
                    doHaptics.HapticsOnGrab(controllerActions);
                }
            }
        }

        private void AttemptGrabObject()
        {
            var objectToGrab = GetGrabbableObject();
            if (objectToGrab != null)
            {
                PerformGrabAttempt(objectToGrab);
            }
            else
            {
                grabPrecognitionTimer = Time.time + grabPrecognition;
            }
        }

        private void PerformGrabAttempt(GameObject objectToGrab)
        {
            IncrementGrabState();
            var initialGrabAttempt = IsValidGrabAttempt(objectToGrab);
            undroppableGrabbedObject = GetUndroppableObject();
            AttemptHaptics(initialGrabAttempt);
        }

        private bool IsValidGrabAttempt(GameObject objectToGrab)
        {
            var grabbingObject = gameObject;
            var initialGrabAttempt = false;
            var objectToGrabScript = objectToGrab.GetComponent<VRTK_InteractableObject>();
            if (grabbedObject == null && IsObjectGrabbable(interactTouch.GetTouchedObject()) && objectToGrabScript.grabAttachMechanicScript.ValidGrab(controllerAttachPoint))
            {
                InitGrabbedObject();
                if (!influencingGrabbedObject)
                {
                    initialGrabAttempt = objectToGrabScript.grabAttachMechanicScript.StartGrab(grabbingObject, grabbedObject, controllerAttachPoint);
                }
            }
            return initialGrabAttempt;
        }

        private bool CanRelease()
        {
            return (grabbedObject && grabbedObject.GetComponent<VRTK_InteractableObject>().IsDroppable());
        }

        private void AttemptReleaseObject()
        {
            if (CanRelease() && (IsObjectHoldOnGrab(grabbedObject) || grabEnabledState >= 2))
            {
                InitUngrabbedObject(true);
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

        private void CheckControllerAttachPointSet()
        {
            if (controllerAttachPoint == null)
            {
                SetControllerAttachPoint();
            }
        }

        private void CreateNonTouchingRigidbody()
        {
            if (createRigidBodyWhenNotTouching && grabbedObject == null)
            {
                if (!interactTouch.IsRigidBodyForcedActive() && interactTouch.IsRigidBodyActive() != controllerEvents.grabPressed)
                {
                    interactTouch.ToggleControllerRigidBody(controllerEvents.grabPressed);
                }
            }
        }

        private void CheckPrecognitionGrab()
        {
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