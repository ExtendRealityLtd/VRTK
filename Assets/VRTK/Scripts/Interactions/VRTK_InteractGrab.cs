// Interact Grab|Interactions|30060
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

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
    [AddComponentMenu("VRTK/Scripts/Interactions/VRTK_InteractGrab")]
    public class VRTK_InteractGrab : MonoBehaviour
    {
        [Header("Grab Settings")]

        [Tooltip("The button used to grab/release a touched object.")]
        public VRTK_ControllerEvents.ButtonAlias grabButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
        [Tooltip("An amount of time between when the grab button is pressed to when the controller is touching something to grab it. For example, if an object is falling at a fast rate, then it is very hard to press the grab button in time to catch the object due to human reaction times. A higher number here will mean the grab button can be pressed before the controller touches the object and when the collision takes place, if the grab button is still being held down then the grab action will be successful.")]
        public float grabPrecognition = 0f;
        [Tooltip("An amount to multiply the velocity of any objects being thrown. This can be useful when scaling up the play area to simulate being able to throw items further.")]
        public float throwMultiplier = 1f;
        [Tooltip("If this is checked and the controller is not touching an Interactable Object when the grab button is pressed then a rigid body is added to the controller to allow the controller to push other rigid body objects around.")]
        public bool createRigidBodyWhenNotTouching = false;

        [Header("Custom Settings")]

        [Tooltip("The rigidbody point on the controller model to snap the grabbed object to. If blank it will be set to the SDK default.")]
        public Rigidbody controllerAttachPoint = null;
        [Tooltip("The controller to listen for the events on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_ControllerEvents controllerEvents;
        [Tooltip("The Interact Touch to listen for touches on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_InteractTouch interactTouch;

        /// <summary>
        /// Emitted when the grab button is pressed.
        /// </summary>
        public event ControllerInteractionEventHandler GrabButtonPressed;
        /// <summary>
        /// Emitted when the grab button is released.
        /// </summary>
        public event ControllerInteractionEventHandler GrabButtonReleased;

        /// <summary>
        /// Emitted when a grab of a valid object is started.
        /// </summary>
        public event ObjectInteractEventHandler ControllerStartGrabInteractableObject;
        /// <summary>
        /// Emitted when a valid object is grabbed.
        /// </summary>
        public event ObjectInteractEventHandler ControllerGrabInteractableObject;
        /// <summary>
        /// Emitted when a ungrab of a valid object is started.
        /// </summary>
        public event ObjectInteractEventHandler ControllerStartUngrabInteractableObject;
        /// <summary>
        /// Emitted when a valid object is released from being grabbed.
        /// </summary>
        public event ObjectInteractEventHandler ControllerUngrabInteractableObject;

        protected VRTK_ControllerEvents.ButtonAlias subscribedGrabButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        protected VRTK_ControllerEvents.ButtonAlias savedGrabButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        protected bool grabPressed;

        protected GameObject grabbedObject = null;
        protected bool influencingGrabbedObject = false;
        protected int grabEnabledState = 0;
        protected float grabPrecognitionTimer = 0f;
        protected GameObject undroppableGrabbedObject;
        protected Rigidbody originalControllerAttachPoint;
        protected Coroutine attemptSetCurrentControllerAttachPoint;
        protected VRTK_ControllerReference controllerReference
        {
            get
            {
                return VRTK_ControllerReference.GetControllerReference((interactTouch != null ? interactTouch.gameObject : null));
            }
        }

        public virtual void OnControllerStartGrabInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerStartGrabInteractableObject != null)
            {
                ControllerStartGrabInteractableObject(this, e);
            }
        }

        public virtual void OnControllerGrabInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerGrabInteractableObject != null)
            {
                ControllerGrabInteractableObject(this, e);
            }
        }

        public virtual void OnControllerStartUngrabInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerStartUngrabInteractableObject != null)
            {
                ControllerStartUngrabInteractableObject(this, e);
            }
        }

        public virtual void OnControllerUngrabInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerUngrabInteractableObject != null)
            {
                ControllerUngrabInteractableObject(this, e);
            }
        }

        public virtual void OnGrabButtonPressed(ControllerInteractionEventArgs e)
        {
            if (GrabButtonPressed != null)
            {
                GrabButtonPressed(this, e);
            }
        }

        public virtual void OnGrabButtonReleased(ControllerInteractionEventArgs e)
        {
            if (GrabButtonReleased != null)
            {
                GrabButtonReleased(this, e);
            }
        }

        /// <summary>
        /// The IsGrabButtonPressed method determines whether the current grab alias button is being pressed down.
        /// </summary>
        /// <returns>Returns true if the grab alias button is being held down.</returns>
        public virtual bool IsGrabButtonPressed()
        {
            return grabPressed;
        }

        /// <summary>
        /// The ForceRelease method will force the controller to stop grabbing the currently grabbed object.
        /// </summary>
        /// <param name="applyGrabbingObjectVelocity">If this is true then upon releasing the object any velocity on the grabbing object will be applied to the object to essentiall throw it. Defaults to `false`.</param>
        public virtual void ForceRelease(bool applyGrabbingObjectVelocity = false)
        {
            InitUngrabbedObject(applyGrabbingObjectVelocity);
        }

        /// <summary>
        /// The AttemptGrab method will attempt to grab the currently touched object without needing to press the grab button on the controller.
        /// </summary>
        public virtual void AttemptGrab()
        {
            AttemptGrabObject();
        }

        /// <summary>
        /// The GetGrabbedObject method returns the current object being grabbed by the controller.
        /// </summary>
        /// <returns>The game object of what is currently being grabbed by this controller.</returns>
        public virtual GameObject GetGrabbedObject()
        {
            return grabbedObject;
        }

        protected virtual void Awake()
        {
            originalControllerAttachPoint = controllerAttachPoint;
            controllerEvents = (controllerEvents != null ? controllerEvents : GetComponentInParent<VRTK_ControllerEvents>());
            interactTouch = (interactTouch != null ? interactTouch : GetComponentInParent<VRTK_InteractTouch>());
            if (interactTouch == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_InteractGrab", "VRTK_InteractTouch", "interactTouch", "the same or parent"));
            }

            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            RegrabUndroppableObject();
            ManageGrabListener(true);
            ManageInteractTouchListener(true);
            if (controllerEvents != null)
            {
                controllerEvents.ControllerIndexChanged += ControllerIndexChanged;
            }
            SetControllerAttachPoint();
        }

        protected virtual void OnDisable()
        {
            if (attemptSetCurrentControllerAttachPoint != null)
            {
                StopCoroutine(attemptSetCurrentControllerAttachPoint);
                attemptSetCurrentControllerAttachPoint = null;
            }
            SetUndroppableObject();
            ForceRelease();
            ManageGrabListener(false);
            ManageInteractTouchListener(false);
            if (controllerEvents != null)
            {
                controllerEvents.ControllerIndexChanged -= ControllerIndexChanged;
            }
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Update()
        {
            ManageGrabListener(true);
            CheckControllerAttachPointSet();
            CreateNonTouchingRigidbody();
            CheckPrecognitionGrab();
        }

        protected virtual void ControllerIndexChanged(object sender, ControllerInteractionEventArgs e)
        {
            SetControllerAttachPoint();
        }

        protected virtual void ManageInteractTouchListener(bool state)
        {
            if (interactTouch != null && !state)
            {
                interactTouch.ControllerTouchInteractableObject -= ControllerTouchInteractableObject;
                interactTouch.ControllerUntouchInteractableObject -= ControllerUntouchInteractableObject;
            }

            if (interactTouch != null && state)
            {
                interactTouch.ControllerTouchInteractableObject += ControllerTouchInteractableObject;
                interactTouch.ControllerUntouchInteractableObject += ControllerUntouchInteractableObject;
            }
        }

        protected virtual void ControllerTouchInteractableObject(object sender, ObjectInteractEventArgs e)
        {
            if (e.target != null)
            {
                VRTK_InteractableObject touchedObjectScript = e.target.GetComponent<VRTK_InteractableObject>();
                if (touchedObjectScript != null && touchedObjectScript.grabOverrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    savedGrabButton = subscribedGrabButton;
                    grabButton = touchedObjectScript.grabOverrideButton;
                }
            }
        }

        protected virtual void ControllerUntouchInteractableObject(object sender, ObjectInteractEventArgs e)
        {
            if (e.target != null)
            {
                VRTK_InteractableObject touchedObjectScript = e.target.GetComponent<VRTK_InteractableObject>();
                if (!touchedObjectScript.IsGrabbed() && savedGrabButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    grabButton = savedGrabButton;
                    savedGrabButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
                }
            }
        }

        protected virtual void ManageGrabListener(bool state)
        {
            if (controllerEvents != null && subscribedGrabButton != VRTK_ControllerEvents.ButtonAlias.Undefined && (!state || !grabButton.Equals(subscribedGrabButton)))
            {
                controllerEvents.UnsubscribeToButtonAliasEvent(subscribedGrabButton, true, DoGrabObject);
                controllerEvents.UnsubscribeToButtonAliasEvent(subscribedGrabButton, false, DoReleaseObject);
                subscribedGrabButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
            }

            if (controllerEvents != null && state && grabButton != VRTK_ControllerEvents.ButtonAlias.Undefined && !grabButton.Equals(subscribedGrabButton))
            {
                controllerEvents.SubscribeToButtonAliasEvent(grabButton, true, DoGrabObject);
                controllerEvents.SubscribeToButtonAliasEvent(grabButton, false, DoReleaseObject);
                subscribedGrabButton = grabButton;
            }
        }

        protected virtual void RegrabUndroppableObject()
        {
            if (undroppableGrabbedObject != null)
            {
                VRTK_InteractableObject undroppableGrabbedObjectScript = undroppableGrabbedObject.GetComponent<VRTK_InteractableObject>();
                if (interactTouch != null && undroppableGrabbedObjectScript != null && !undroppableGrabbedObjectScript.IsGrabbed())
                {
                    undroppableGrabbedObject.SetActive(true);
                    interactTouch.ForceTouch(undroppableGrabbedObject);
                    AttemptGrab();
                }
            }
            else
            {
                undroppableGrabbedObject = null;
            }
        }

        protected virtual void SetUndroppableObject()
        {
            if (undroppableGrabbedObject != null)
            {
                VRTK_InteractableObject undroppableGrabbedObjectScript = undroppableGrabbedObject.GetComponent<VRTK_InteractableObject>();
                if (undroppableGrabbedObjectScript != null && undroppableGrabbedObjectScript.IsDroppable())
                {
                    undroppableGrabbedObject = null;
                }
                else
                {
                    undroppableGrabbedObject.SetActive(false);
                }
            }
        }

        protected virtual void SetControllerAttachPoint()
        {
            //If no attach point has been specified then just use the tip of the controller
            if (controllerReference.model != null && originalControllerAttachPoint == null)
            {
                //attempt to find the attach point on the controller
                SDK_BaseController.ControllerHand handType = VRTK_DeviceFinder.GetControllerHand(interactTouch.gameObject);
                string elementPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.AttachPoint, handType);
                attemptSetCurrentControllerAttachPoint = StartCoroutine(SetCurrentControllerAttachPoint(elementPath, 10, 0.1f));
            }
        }

        protected virtual IEnumerator SetCurrentControllerAttachPoint(string searchPath, int attempts, float delay)
        {
            WaitForSeconds delayInstruction = new WaitForSeconds(delay);
            Transform defaultAttachPoint = controllerReference.model.transform.Find(searchPath);

            while (defaultAttachPoint == null && attempts > 0)
            {
                defaultAttachPoint = controllerReference.model.transform.Find(searchPath);
                attempts--;
                yield return delayInstruction;
            }

            if (defaultAttachPoint != null)
            {
                controllerAttachPoint = defaultAttachPoint.GetComponent<Rigidbody>();

                if (controllerAttachPoint == null)
                {
                    Rigidbody autoGenRB = defaultAttachPoint.gameObject.AddComponent<Rigidbody>();
                    autoGenRB.isKinematic = true;
                    controllerAttachPoint = autoGenRB;
                }
            }
        }

        protected virtual bool IsObjectGrabbable(GameObject obj)
        {
            VRTK_InteractableObject objScript = obj.GetComponent<VRTK_InteractableObject>();
            return (interactTouch != null && interactTouch.IsObjectInteractable(obj) && objScript != null && (objScript.isGrabbable || objScript.PerformSecondaryAction()));
        }

        protected virtual bool IsObjectHoldOnGrab(GameObject obj)
        {
            if (obj != null)
            {
                VRTK_InteractableObject objScript = obj.GetComponent<VRTK_InteractableObject>();
                return (objScript != null && objScript.holdButtonToGrab);
            }
            return false;
        }

        protected virtual void ChooseGrabSequence(VRTK_InteractableObject grabbedObjectScript)
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

        protected virtual void ToggleControllerVisibility(bool visible)
        {
            if (grabbedObject != null)
            {
                VRTK_InteractControllerAppearance[] controllerAppearanceScript = grabbedObject.GetComponentsInParent<VRTK_InteractControllerAppearance>(true);
                if (controllerAppearanceScript.Length > 0)
                {
                    controllerAppearanceScript[0].ToggleControllerOnGrab(visible, controllerReference.model, grabbedObject);
                }
            }
            else if (visible)
            {
                VRTK_ObjectAppearance.SetRendererVisible(controllerReference.model, grabbedObject);
            }
        }

        protected virtual void InitGrabbedObject()
        {
            grabbedObject = (interactTouch != null ? interactTouch.GetTouchedObject() : null);
            if (grabbedObject != null)
            {
                OnControllerStartGrabInteractableObject(interactTouch.SetControllerInteractEvent(grabbedObject));
                VRTK_InteractableObject grabbedObjectScript = grabbedObject.GetComponent<VRTK_InteractableObject>();
                ChooseGrabSequence(grabbedObjectScript);
                ToggleControllerVisibility(false);
                OnControllerGrabInteractableObject(interactTouch.SetControllerInteractEvent(grabbedObject));
            }
        }

        protected virtual void InitPrimaryGrab(VRTK_InteractableObject currentGrabbedObject)
        {
            if (!currentGrabbedObject.IsValidInteractableController(gameObject, currentGrabbedObject.allowedGrabControllers))
            {
                grabbedObject = null;
                if (interactTouch != null && currentGrabbedObject.IsGrabbed(gameObject))
                {
                    interactTouch.ForceStopTouching();
                }
                return;
            }

            influencingGrabbedObject = false;
            currentGrabbedObject.SaveCurrentState();
            currentGrabbedObject.Grabbed(this);
            currentGrabbedObject.ZeroVelocity();
            currentGrabbedObject.ToggleHighlight(false);
            currentGrabbedObject.isKinematic = false;
        }

        protected virtual void InitSecondaryGrab(VRTK_InteractableObject currentGrabbedObject)
        {
            if (!currentGrabbedObject.IsValidInteractableController(gameObject, currentGrabbedObject.allowedGrabControllers))
            {
                grabbedObject = null;
                influencingGrabbedObject = false;
                currentGrabbedObject.Ungrabbed(this);
                return;
            }

            influencingGrabbedObject = true;
            currentGrabbedObject.Grabbed(this);
        }

        protected virtual void CheckInfluencingObjectOnRelease()
        {
            if (!influencingGrabbedObject && interactTouch != null)
            {
                interactTouch.ForceStopTouching();
                ToggleControllerVisibility(true);
            }
            influencingGrabbedObject = false;
        }

        protected virtual void InitUngrabbedObject(bool applyGrabbingObjectVelocity)
        {
            if (grabbedObject != null && interactTouch != null)
            {
                OnControllerStartUngrabInteractableObject(interactTouch.SetControllerInteractEvent(grabbedObject));
                VRTK_InteractableObject grabbedObjectScript = grabbedObject.GetComponent<VRTK_InteractableObject>();
                if (grabbedObjectScript != null)
                {
                    if (!influencingGrabbedObject)
                    {
                        grabbedObjectScript.grabAttachMechanicScript.StopGrab(applyGrabbingObjectVelocity);
                    }
                    grabbedObjectScript.Ungrabbed(this);
                    grabbedObjectScript.ToggleHighlight(false);
                    ToggleControllerVisibility(true);

                    OnControllerUngrabInteractableObject(interactTouch.SetControllerInteractEvent(grabbedObject));
                }
            }

            CheckInfluencingObjectOnRelease();

            grabEnabledState = 0;
            grabbedObject = null;
        }

        protected virtual GameObject GetGrabbableObject()
        {
            GameObject obj = (interactTouch != null ? interactTouch.GetTouchedObject() : null);
            if (obj != null && interactTouch.IsObjectInteractable(obj))
            {
                return obj;
            }
            return grabbedObject;
        }

        protected virtual void IncrementGrabState()
        {
            if (interactTouch != null && !IsObjectHoldOnGrab(interactTouch.GetTouchedObject()))
            {
                grabEnabledState++;
            }
        }

        protected virtual GameObject GetUndroppableObject()
        {
            if (grabbedObject != null)
            {
                VRTK_InteractableObject grabbedObjectScript = grabbedObject.GetComponent<VRTK_InteractableObject>();
                return (grabbedObjectScript != null && !grabbedObjectScript.IsDroppable() ? grabbedObject : null);
            }
            return null;
        }

        protected virtual void AttemptHaptics(bool initialGrabAttempt)
        {
            if (grabbedObject != null && initialGrabAttempt)
            {
                VRTK_InteractHaptics doHaptics = grabbedObject.GetComponentInParent<VRTK_InteractHaptics>();
                if (doHaptics != null)
                {
                    doHaptics.HapticsOnGrab(controllerReference);
                }
            }
        }

        protected virtual void AttemptGrabObject()
        {
            GameObject objectToGrab = GetGrabbableObject();
            if (objectToGrab != null)
            {
                PerformGrabAttempt(objectToGrab);
            }
            else
            {
                grabPrecognitionTimer = Time.time + grabPrecognition;
            }
        }

        protected virtual void PerformGrabAttempt(GameObject objectToGrab)
        {
            IncrementGrabState();
            bool initialGrabAttempt = IsValidGrabAttempt(objectToGrab);
            undroppableGrabbedObject = GetUndroppableObject();
            AttemptHaptics(initialGrabAttempt);
        }

        protected virtual bool ScriptValidGrab(VRTK_InteractableObject objectToGrabScript)
        {
            return (objectToGrabScript != null && objectToGrabScript.grabAttachMechanicScript != null && objectToGrabScript.grabAttachMechanicScript.ValidGrab(controllerAttachPoint));
        }

        protected virtual bool IsValidGrabAttempt(GameObject objectToGrab)
        {
            bool initialGrabAttempt = false;
            VRTK_InteractableObject objectToGrabScript = (objectToGrab != null ? objectToGrab.GetComponent<VRTK_InteractableObject>() : null);
            if (grabbedObject == null && interactTouch != null && IsObjectGrabbable(interactTouch.GetTouchedObject()) && ScriptValidGrab(objectToGrabScript))
            {
                InitGrabbedObject();
                if (!influencingGrabbedObject)
                {
                    initialGrabAttempt = objectToGrabScript.grabAttachMechanicScript.StartGrab(gameObject, grabbedObject, controllerAttachPoint);
                }
            }
            return initialGrabAttempt;
        }

        protected virtual bool CanRelease()
        {
            if (grabbedObject != null)
            {
                VRTK_InteractableObject objectToGrabScript = grabbedObject.GetComponent<VRTK_InteractableObject>();
                return (objectToGrabScript != null && objectToGrabScript.IsDroppable());
            }
            return false;
        }

        protected virtual void AttemptReleaseObject()
        {
            if (CanRelease() && (IsObjectHoldOnGrab(grabbedObject) || grabEnabledState >= 2))
            {
                InitUngrabbedObject(true);
            }
        }

        protected virtual void DoGrabObject(object sender, ControllerInteractionEventArgs e)
        {
            OnGrabButtonPressed(controllerEvents.SetControllerEvent(ref grabPressed, true));
            AttemptGrabObject();
        }

        protected virtual void DoReleaseObject(object sender, ControllerInteractionEventArgs e)
        {
            AttemptReleaseObject();
            OnGrabButtonReleased(controllerEvents.SetControllerEvent(ref grabPressed, false));
        }

        protected virtual void CheckControllerAttachPointSet()
        {
            if (controllerAttachPoint == null)
            {
                SetControllerAttachPoint();
            }
        }

        protected virtual void CreateNonTouchingRigidbody()
        {
            if (createRigidBodyWhenNotTouching && grabbedObject == null && interactTouch != null)
            {
                if (!interactTouch.IsRigidBodyForcedActive() && interactTouch.IsRigidBodyActive() != grabPressed)
                {
                    interactTouch.ToggleControllerRigidBody(grabPressed);
                }
            }
        }

        protected virtual void CheckPrecognitionGrab()
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