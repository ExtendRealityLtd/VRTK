// Interactable Object|Scripts|0160
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Highlighters;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="interactingObject">The object that is initiating the interaction (e.g. a controller).</param>
    public struct InteractableObjectEventArgs
    {
        public GameObject interactingObject;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="InteractableObjectEventArgs"/></param>
    public delegate void InteractableObjectEventHandler(object sender, InteractableObjectEventArgs e);

    /// <summary>
    /// The Interactable Object script is attached to any game object that is required to be interacted with (e.g. via the controllers).
    /// </summary>
    /// <remarks>
    /// The basis of this script is to provide a simple mechanism for identifying objects in the game world that can be grabbed or used but it is expected that this script is the base to be inherited into a script with richer functionality.
    ///
    /// The highlighting of an Interactable Object is defaulted to use the `VRTK_MaterialColorSwapHighlighter` if no other highlighter is applied to the Object.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/005_Controller_BasicObjectGrabbing` uses the `VRTK_InteractTouch` and `VRTK_InteractGrab` scripts on the controllers to show how an interactable object can be grabbed and snapped to the controller and thrown around the game world.
    ///
    /// `VRTK/Examples/013_Controller_UsingAndGrabbingMultipleObjects` shows multiple objects that can be grabbed by holding the buttons or grabbed by toggling the button click and also has objects that can have their Using state toggled to show how multiple items can be turned on at the same time.
    /// </example>
    public class VRTK_InteractableObject : MonoBehaviour
    {
        /// <summary>
        /// Types of grab attachment.
        /// </summary>
        /// <param name="Fixed_Joint">Attaches the object to the controller with a fixed joint meaning it tracks the position and rotation of the controller with perfect 1:1 tracking.</param>
        /// <param name="Spring_Joint">Attaches the object to the controller with a spring joint meaning there is some flexibility between the item and the controller force moving the item. This works well when attempting to pull an item rather than snap the item directly to the controller. It creates the illusion that the item has resistance to move it.</param>
        /// <param name="Track_Object">Doesn't attach the object to the controller via a joint, instead it ensures the object tracks the direction of the controller, which works well for items that are on hinged joints.</param>
        /// <param name="Rotator_Track">Tracks the object but instead of the object tracking the direction of the controller, a force is applied to the object to cause it to rotate. This is ideal for hinged joints on items such as wheels or doors.</param>
        /// <param name="Child_Of_Controller">Makes the object a child of the controller grabbing so it naturally tracks the position of the controller motion.</param>
        /// <param name="Climbable">Non-rigid body interactable object used to allow player climbing.</param>
        public enum GrabAttachType
        {
            Fixed_Joint,
            Spring_Joint,
            Track_Object,
            Rotator_Track,
            Child_Of_Controller,
            Climbable
        }

        /// <summary>
        /// Allowed controller type.
        /// </summary>
        /// <param name="Both">Both controllers are allowed to interact.</param>
        /// <param name="Left_Only">Only the left controller is allowed to interact.</param>
        /// <param name="Right_Only">Only the right controller is allowed to interact.</param>
        public enum AllowedController
        {
            Both,
            Left_Only,
            Right_Only
        }

        /// <summary>
        /// Hide controller state.
        /// </summary>
        /// <param name="Default">Use the hide settings from the controller.</param>
        /// <param name="OverrideHide">Hide the controller when interacting, overriding controller settings.</param>
        /// <param name="OverrideDontHide">Don't hide the controller when interacting, overriding controller settings.</param>
        public enum ControllerHideMode
        {
            Default,
            OverrideHide,
            OverrideDontHide,
        }

        [Header("Touch Interactions", order = 1)]

        [Tooltip("The object will only highlight when a controller touches it if this is checked.")]
        public bool highlightOnTouch = false;
        [Tooltip("The colour to highlight the object when it is touched. This colour will override any globally set colour (for instance on the `VRTK_InteractTouch` script).")]
        public Color touchHighlightColor = Color.clear;
        [Tooltip("The haptic feedback on the controller can be triggered upon touching the object, the `Strength` denotes the strength of the pulse, the `Duration` denotes the length of time.")]
        public Vector2 rumbleOnTouch = Vector2.zero;
        [Tooltip("Determines which controller can initiate a touch action.")]
        public AllowedController allowedTouchControllers = AllowedController.Both;
        [Tooltip("Optionally override the controller setting.")]
        public ControllerHideMode hideControllerOnTouch = ControllerHideMode.Default;

        [Header("Grab Interactions", order = 2)]

        [Tooltip("Determines if the object can be grabbed.")]
        public bool isGrabbable = false;
        [Tooltip("Determines if the object can be dropped by the controller grab button being used. If this is unchecked then it's not possible to drop the item once it's picked up using the controller button.")]
        public bool isDroppable = true;
        [Tooltip("Determines if the object can be swapped between controllers when it is picked up. If it is unchecked then the object must be dropped before it can be picked up by the other controller.")]
        public bool isSwappable = true;
        [Tooltip("If this is checked then the grab button on the controller needs to be continually held down to keep grabbing. If this is unchecked the grab button toggles the grab action with one button press to grab and another to release.")]
        public bool holdButtonToGrab = true;
        [Tooltip("If this is set to `Undefined` then the global grab alias button will grab the object, setting it to any other button will ensure the override button is used to grab this specific interactable object.")]
        public VRTK_ControllerEvents.ButtonAlias grabOverrideButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("The haptic feedback on the controller can be triggered upon grabbing the object, the `Strength` denotes the strength of the pulse, the `Duration` denotes the length of time.")]
        public Vector2 rumbleOnGrab = Vector2.zero;
        [Tooltip("Determines which controller can initiate a grab action.")]
        public AllowedController allowedGrabControllers = AllowedController.Both;
        [Tooltip("If this is checked then when the controller grabs the object, it will grab it with precision and pick it up at the particular point on the object the controller is touching.")]
        public bool precisionSnap;
        [Tooltip("A Transform provided as an empty game object which must be the child of the item being grabbed and serves as an orientation point to rotate and position the grabbed item in relation to the right handed controller. If no Right Snap Handle is provided but a Left Snap Handle is provided, then the Left Snap Handle will be used in place. If no Snap Handle is provided then the object will be grabbed at its central point. Not required for `Precision Snap`.")]
        public Transform rightSnapHandle;
        [Tooltip("A Transform provided as an empty game object which must be the child of the item being grabbed and serves as an orientation point to rotate and position the grabbed item in relation to the left handed controller. If no Left Snap Handle is provided but a Right Snap Handle is provided, then the Right Snap Handle will be used in place. If no Snap Handle is provided then the object will be grabbed at its central point. Not required for `Precision Snap`.")]
        public Transform leftSnapHandle;
        [Tooltip("Optionally override the controller setting.")]
        public ControllerHideMode hideControllerOnGrab = ControllerHideMode.Default;
        [Tooltip("If this is checked then the object will stay grabbed to the controller when a teleport occurs. If it is unchecked then the object will be released when a teleport occurs.")]
        public bool stayGrabbedOnTeleport = true;

        [Header("Grab Mechanics", order = 3)]

        [Tooltip("This determines how the grabbed item will be attached to the controller when it is grabbed.")]
        public GrabAttachType grabAttachMechanic = GrabAttachType.Fixed_Joint;
        [Tooltip("The force amount when to detach the object from the grabbed controller. If the controller tries to exert a force higher than this threshold on the object (from pulling it through another object or pushing it into another object) then the joint holding the object to the grabbing controller will break and the object will no longer be grabbed. This also works with Tracked Object grabbing but determines how far the controller is from the object before breaking the grab. Only required for `Fixed Joint`, `Spring Joint`, `Track Object` and `Rotator Track`.")]
        public float detachThreshold = 500f;
        [Tooltip("The strength of the spring holding the object to the controller. A low number will mean the spring is very loose and the object will require more force to move it, a high number will mean a tight spring meaning less force is required to move it. Only required for `Spring Joint`.")]
        public float springJointStrength = 500f;
        [Tooltip("The amount to damper the spring effect when using a Spring Joint grab mechanic. A higher number here will reduce the oscillation effect when moving jointed Interactable Objects. Only required for `Spring Joint`.")]
        public float springJointDamper = 50f;
        [Tooltip("An amount to multiply the velocity of the given object when it is thrown. This can also be used in conjunction with the Interact Grab Throw Multiplier to have certain objects be thrown even further than normal (or thrown a shorter distance if a number below 1 is entered).")]
        public float throwMultiplier = 1f;
        [Tooltip("The amount of time to delay collisions affecting the object when it is first grabbed. This is useful if a game object may get stuck inside another object when it is being grabbed.")]
        public float onGrabCollisionDelay = 0f;

        [Header("Use Interactions", order = 4)]

        [Tooltip("Determines if the object can be used.")]
        public bool isUsable = false;
        [Tooltip("If this is checked the object can be used only if it is currently being grabbed.")]
        public bool useOnlyIfGrabbed = false;
        [Tooltip("If this is checked then the use button on the controller needs to be continually held down to keep using. If this is unchecked the the use button toggles the use action with one button press to start using and another to stop using.")]
        public bool holdButtonToUse = true;
        [Tooltip("If this is set to `Undefined` then the global use alias button will use the object, setting it to any other button will ensure the override button is used to use this specific interactable object.")]
        public VRTK_ControllerEvents.ButtonAlias useOverrideButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("If this is checked then when a World Pointer beam (projected from the controller) hits the interactable object, if the object has `Hold Button To Use` unchecked then whilst the pointer is over the object it will run it's `Using` method. If `Hold Button To Use` is unchecked then the `Using` method will be run when the pointer is deactivated. The world pointer will not throw the `Destination Set` event if it is affecting an interactable object with this setting checked as this prevents unwanted teleporting from happening when using an object with a pointer.")]
        public bool pointerActivatesUseAction = false;
        [Tooltip("The haptic feedback on the controller can be triggered upon using the object, the `Strength` denotes the strength of the pulse, the `Duration` denotes the length of time.")]
        public Vector2 rumbleOnUse = Vector2.zero;
        [Tooltip("Determines which controller can initiate a use action.")]
        public AllowedController allowedUseControllers = AllowedController.Both;
        [Tooltip("Optionally override the controller setting.")]
        public ControllerHideMode hideControllerOnUse = ControllerHideMode.Default;

        /// <summary>
        /// Emitted when another object touches the current object.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectTouched;
        /// <summary>
        /// Emitted when the other object stops touching the current object.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectUntouched;
        /// <summary>
        /// Emitted when another object grabs the current object (e.g. a controller).
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectGrabbed;
        /// <summary>
        /// Emitted when the other object stops grabbing the current object.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectUngrabbed;
        /// <summary>
        /// Emitted when another object uses the current object (e.g. a controller).
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectUsed;
        /// <summary>
        /// Emitted when the other object stops using the current object.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectUnused;

        /// <summary>
        /// The current using state of the object. `0` not being used, `1` being used.
        /// </summary>
        [HideInInspector]
        public int usingState = 0;

        protected Rigidbody rb;
        protected bool autoRigidbody = false;
        protected List<GameObject> touchingObjects = new List<GameObject>();
        protected GameObject grabbingObject = null;
        protected GameObject usingObject = null;
        protected Transform grabbedSnapHandle;
        protected Transform trackPoint;
        protected bool customTrackPoint = false;
        protected Transform originalControllerAttachPoint;
        protected Transform previousParent;
        protected bool previousKinematicState;
        protected bool previousIsGrabbable;
        protected bool forcedDropped;
        protected bool forceDisabled;
        protected VRTK_BaseHighlighter objectHighlighter;
        protected bool autoHighlighter = false;

        public virtual void OnInteractableObjectTouched(InteractableObjectEventArgs e)
        {
            if (InteractableObjectTouched != null)
            {
                InteractableObjectTouched(this, e);
            }
        }

        public virtual void OnInteractableObjectUntouched(InteractableObjectEventArgs e)
        {
            if (InteractableObjectUntouched != null)
            {
                InteractableObjectUntouched(this, e);
            }
        }

        public virtual void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
        {
            if (InteractableObjectGrabbed != null)
            {
                InteractableObjectGrabbed(this, e);
            }
        }

        public virtual void OnInteractableObjectUngrabbed(InteractableObjectEventArgs e)
        {
            if (InteractableObjectUngrabbed != null)
            {
                InteractableObjectUngrabbed(this, e);
            }
        }

        public virtual void OnInteractableObjectUsed(InteractableObjectEventArgs e)
        {
            if (InteractableObjectUsed != null)
            {
                InteractableObjectUsed(this, e);
            }
        }

        public virtual void OnInteractableObjectUnused(InteractableObjectEventArgs e)
        {
            if (InteractableObjectUnused != null)
            {
                InteractableObjectUnused(this, e);
            }
        }

        public InteractableObjectEventArgs SetInteractableObjectEvent(GameObject interactingObject)
        {
            InteractableObjectEventArgs e;
            e.interactingObject = interactingObject;
            return e;
        }

        /// <summary>
        /// The CheckHideMode method is a simple service method used only by some scripts (e.g. InteractTouch InteractGrab InteractUse) to calculate the "hide controller" condition according to the default controller settings and the interactive object override method.
        /// </summary>
        /// <param name="defaultMode">The default setting of the controller. true = hide, false = don't hide.</param>
        /// <param name="overrideMode">The override setting of the object.</param>
        /// <returns>Returns `true` if the combination of `defaultMode` and `overrideMode` lead to "hide controller.</returns>
        public bool CheckHideMode(bool defaultMode, ControllerHideMode overrideMode)
        {
            switch (overrideMode)
            {
                case ControllerHideMode.OverrideDontHide:
                    return false;
                case ControllerHideMode.OverrideHide:
                    return true;
            }
            return defaultMode;
        }

        /// <summary>
        /// The IsTouched method is used to determine if the object is currently being touched.
        /// </summary>
        /// <returns>Returns `true` if the object is currently being touched.</returns>
        public bool IsTouched()
        {
            return (touchingObjects.Count > 0);
        }

        /// <summary>
        /// The IsGrabbed method is used to determine if the object is currently being grabbed.
        /// </summary>
        /// <returns>Returns `true` if the object is currently being grabbed.</returns>
        public bool IsGrabbed()
        {
            return (grabbingObject != null);
        }

        /// <summary>
        /// The IsUsing method is used to determine if the object is currently being used.
        /// </summary>
        /// <returns>Returns `true` if the object is currently being used.</returns>
        public bool IsUsing()
        {
            return (usingObject != null);
        }

        /// <summary>
        /// The StartTouching method is called automatically when the object is touched initially. It is also a virtual method to allow for overriding in inherited classes.
        /// </summary>
        /// <param name="currentTouchingObject">The game object that is currently touching this object.</param>
        public virtual void StartTouching(GameObject currentTouchingObject)
        {
            if (!touchingObjects.Contains(currentTouchingObject))
            {
                touchingObjects.Add(currentTouchingObject);
                OnInteractableObjectTouched(SetInteractableObjectEvent(currentTouchingObject));
            }
        }

        /// <summary>
        /// The StopTouching method is called automatically when the object has stopped being touched. It is also a virtual method to allow for overriding in inherited classes.
        /// </summary>
        /// <param name="previousTouchingObject">The game object that was previously touching this object.</param>
        public virtual void StopTouching(GameObject previousTouchingObject)
        {
            if (touchingObjects.Contains(previousTouchingObject))
            {
                OnInteractableObjectUntouched(SetInteractableObjectEvent(previousTouchingObject));
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(StopUsingOnControllerChange(previousTouchingObject));
                }
                touchingObjects.Remove(previousTouchingObject);
            }
        }

        /// <summary>
        /// The Grabbed method is called automatically when the object is grabbed initially. It is also a virtual method to allow for overriding in inherited classes.
        /// </summary>
        /// <param name="currentGrabbingObject">The game object that is currently grabbing this object.</param>
        public virtual void Grabbed(GameObject currentGrabbingObject)
        {
            OnInteractableObjectGrabbed(SetInteractableObjectEvent(currentGrabbingObject));
            ForceReleaseGrab();
            RemoveTrackPoint();
            grabbingObject = currentGrabbingObject;
            SetTrackPoint(grabbingObject);
            if (!isSwappable)
            {
                previousIsGrabbable = isGrabbable;
                isGrabbable = false;
            }
        }

        /// <summary>
        /// The Ungrabbed method is called automatically when the object has stopped being grabbed. It is also a virtual method to allow for overriding in inherited classes.
        /// </summary>
        /// <param name="previousGrabbingObject">The game object that was previously grabbing this object.</param>
        public virtual void Ungrabbed(GameObject previousGrabbingObject)
        {
            OnInteractableObjectUngrabbed(SetInteractableObjectEvent(previousGrabbingObject));
            RemoveTrackPoint();
            grabbedSnapHandle = null;
            grabbingObject = null;
            LoadPreviousState();
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(StopUsingOnControllerChange(previousGrabbingObject));
            }
        }

        /// <summary>
        /// The StartUsing method is called automatically when the object is used initially. It is also a virtual method to allow for overriding in inherited classes.
        /// </summary>
        /// <param name="currentUsingObject">The game object that is currently using this object.</param>
        public virtual void StartUsing(GameObject currentUsingObject)
        {
            OnInteractableObjectUsed(SetInteractableObjectEvent(currentUsingObject));
            usingObject = currentUsingObject;
        }

        /// <summary>
        /// The StopUsing method is called automatically when the object has stopped being used. It is also a virtual method to allow for overriding in inherited classes.
        /// </summary>
        /// <param name="previousUsingObject">The game object that was previously using this object.</param>
        public virtual void StopUsing(GameObject previousUsingObject)
        {
            OnInteractableObjectUnused(SetInteractableObjectEvent(previousUsingObject));
            usingObject = null;
        }

        /// <summary>
        /// The ToggleHighlight/1 method is used as a shortcut to disable highlights whilst keeping the same method signature. It should always be used with `false` and it calls ToggleHighlight/2 with a `Color.clear`.
        /// </summary>
        /// <param name="toggle">The state to determine whether to activate or deactivate the highlight. `true` will enable the highlight and `false` will remove the highlight.</param>
        public virtual void ToggleHighlight(bool toggle)
        {
            ToggleHighlight(toggle, Color.clear);
        }

        /// <summary>
        /// The ToggleHighlight/2 method is used to turn on or off the colour highlight of the object.
        /// </summary>
        /// <param name="toggle">The state to determine whether to activate or deactivate the highlight. `true` will enable the highlight and `false` will remove the highlight.</param>
        /// <param name="globalHighlightColor">The colour to use when highlighting the object.</param>
        public virtual void ToggleHighlight(bool toggle, Color globalHighlightColor)
        {
            if (highlightOnTouch)
            {
                if (toggle && !IsGrabbed() && !IsUsing())
                {
                    Color color = (touchHighlightColor != Color.clear ? touchHighlightColor : globalHighlightColor);
                    if (color != Color.clear)
                    {
                        objectHighlighter.Highlight(color);
                    }
                }
                else
                {
                    objectHighlighter.Unhighlight();
                }
            }
        }

        /// <summary>
        /// The PauseCollisions method temporarily pauses all collisions on the object at grab time by removing the object's rigidbody's ability to detect collisions. This can be useful for preventing clipping when initially grabbing an item.
        /// </summary>
        public void PauseCollisions()
        {
            if (onGrabCollisionDelay > 0f)
            {
                foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
                {
                    rb.detectCollisions = false;
                }
                Invoke("UnpauseCollisions", onGrabCollisionDelay);
            }
        }

        /// <summary>
        /// The AttachIsTrackObject method is used to determine if the object is using one of the track grab attach mechanics.
        /// </summary>
        /// <returns>Is true if the grab attach mechanic is one of the track types like `Track Object` or `Rotator Track`.</returns>
        public bool AttachIsTrackObject()
        {
            return (grabAttachMechanic == GrabAttachType.Track_Object || grabAttachMechanic == GrabAttachType.Rotator_Track);
        }

        /// <summary>
        /// The AttachIsClimbObject method is used to determine if the object is using the `Climbable` grab attach mechanics.
        /// </summary>
        /// <returns>Is true if the grab attach mechanic is `Climbable`.</returns>
        public bool AttachIsClimbObject()
        {
            return (grabAttachMechanic == GrabAttachType.Climbable);
        }

        /// <summary>
        /// The AttachIsKinematicObject method is used to determine if the object has kinematics turned on at the point of grab.
        /// </summary>
        /// <returns>Is true if the grab attach mechanic sets the object to a kinematic state on grab.</returns>
        public bool AttachIsKinematicObject()
        {
            return (grabAttachMechanic == GrabAttachType.Child_Of_Controller);
        }

        /// <summary>
        /// The AttachIsStaticObject method is used to determine if the object is using one of the static grab attach types.
        /// </summary>
        /// <returns>Is true if the grab attach mechanic is one of the static types like `Climbable`.</returns>
        public bool AttachIsStaticObject()
        {
            return AttachIsClimbObject(); // only one at the moment
        }

        /// <summary>
        /// The AttachIsUnthrowableObject method is used to determine if the object is using one of the grab types that should not be thrown when released.
        /// </summary>
        /// <returns>Is true if the grab attach mechanic is of a type that shouldn't be considered thrown when released.</returns>
        public bool AttachIsUnthrowableObject()
        {
            return (grabAttachMechanic == GrabAttachType.Rotator_Track);
        }

        /// <summary>
        /// The ZeroVelocity method resets the velocity and angular velocity to zero on the rigidbody attached to the object.
        /// </summary>
        public void ZeroVelocity()
        {
            if (rb)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        /// <summary>
        /// The SaveCurrentState method stores the existing object parent and the object's rigidbody kinematic setting.
        /// </summary>
        public void SaveCurrentState()
        {
            if (grabbingObject == null)
            {
                previousParent = transform.parent;

                if (rb)
                {
                    previousKinematicState = rb.isKinematic;
                }
            }
        }

        /// <summary>
        /// The ToggleKinematic method is used to set the object's internal rigidbody kinematic state.
        /// </summary>
        /// <param name="state">The object's rigidbody kinematic state.</param>
        public void ToggleKinematic(bool state)
        {
            if (rb)
            {
                rb.isKinematic = state;
            }
        }

        /// <summary>
        /// The GetGrabbingObject method is used to return the game object that is currently grabbing this object.
        /// </summary>
        /// <returns>The game object of what is grabbing the current object.</returns>
        public GameObject GetGrabbingObject()
        {
            return grabbingObject;
        }

        /// <summary>
        /// The IsValidInteractableController method is used to check to see if a controller is allowed to perform an interaction with this object as sometimes controllers are prohibited from grabbing or using an object depedning on the use case.
        /// </summary>
        /// <param name="actualController">The game object of the controller that is being checked.</param>
        /// <param name="controllerCheck">The value of which controller is allowed to interact with this object.</param>
        /// <returns>Is true if the interacting controller is allowed to grab the object.</returns>
        public bool IsValidInteractableController(GameObject actualController, AllowedController controllerCheck)
        {
            if (controllerCheck == AllowedController.Both)
            {
                return true;
            }

            var controllerHand = VRTK_DeviceFinder.GetControllerHandType(controllerCheck.ToString().Replace("_Only", ""));
            return (VRTK_DeviceFinder.IsControllerOfHand(actualController, controllerHand));
        }

        /// <summary>
        /// The ForceStopInteracting method forces the object to no longer be interacted with and will cause a controller to drop the object and stop touching it. This is useful if the controller is required to auto interact with another object.
        /// </summary>
        public void ForceStopInteracting()
        {
            if (gameObject.activeInHierarchy)
            {
                forceDisabled = false;
                StartCoroutine(ForceStopInteractingAtEndOfFrame());
            }

            if (!gameObject.activeInHierarchy && forceDisabled)
            {
                ForceStopAllInteractions();
                forceDisabled = false;
            }
        }

        /// <summary>
        /// The SetGrabbedSnapHandle method is used to set the snap handle of the object at runtime.
        /// </summary>
        /// <param name="handle">A transform of an object to use for the snap handle when the object is grabbed.</param>
        public void SetGrabbedSnapHandle(Transform handle)
        {
            grabbedSnapHandle = handle;
        }

        /// <summary>
        /// The RegisterTeleporters method is used to find all objects that have a teleporter script and register the object on the `OnTeleported` event. This is used internally by the object for keeping Tracked objects positions updated after teleporting.
        /// </summary>
        public void RegisterTeleporters()
        {
            StartCoroutine(RegisterTeleportersAtEndOfFrame());
        }

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
            autoRigidbody = false;
            if (!AttachIsStaticObject())
            {
                // If there is no rigid body, add one and set it to 'kinematic'.
                if (!rb)
                {
                    rb = gameObject.AddComponent<Rigidbody>();
                    rb.isKinematic = true;
                    autoRigidbody = true;
                }
                rb.maxAngularVelocity = float.MaxValue;
            }
            forcedDropped = false;
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            if (AttachIsTrackObject())
            {
                CheckBreakDistance();
            }
        }

        protected virtual void FixedUpdate()
        {
            if (trackPoint)
            {
                switch (grabAttachMechanic)
                {
                    case GrabAttachType.Rotator_Track:
                        FixedUpdateRotatorTrack();
                        break;
                    case GrabAttachType.Track_Object:
                        FixedUpdateTrackObject();
                        break;
                }
            }
        }

        protected virtual void OnEnable()
        {
            InitialiseHighlighter();
            RegisterTeleporters();
            forceDisabled = false;
            if (forcedDropped)
            {
                LoadPreviousState();
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var teleporter in VRTK_ObjectCache.registeredTeleporters)
            {
                teleporter.Teleporting -= new TeleportEventHandler(OnTeleporting);
                teleporter.Teleported -= new TeleportEventHandler(OnTeleported);
            }
            if (autoHighlighter)
            {
                Destroy(objectHighlighter);
            }
            forceDisabled = true;
            ForceStopInteracting();
        }

        protected virtual void OnJointBreak(float force)
        {
            ForceReleaseGrab();
        }

        protected virtual void LoadPreviousState()
        {
            if (gameObject.activeInHierarchy)
            {
                transform.parent = previousParent;
                forcedDropped = false;
            }
            if (rb)
            {
                rb.isKinematic = previousKinematicState;
            }
            if (!isSwappable)
            {
                isGrabbable = previousIsGrabbable;
            }
        }

        protected virtual void InitialiseHighlighter()
        {
            if (highlightOnTouch)
            {
                autoHighlighter = false;
                objectHighlighter = Utilities.GetActiveHighlighter(gameObject);
                if (objectHighlighter == null)
                {
                    autoHighlighter = true;
                    objectHighlighter = gameObject.AddComponent<VRTK_MaterialColorSwapHighlighter>();
                }
                objectHighlighter.Initialise(touchHighlightColor);
            }
        }

        private void ForceReleaseGrab()
        {
            if (grabbingObject)
            {
                grabbingObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
            }
        }

        private void UnpauseCollisions()
        {
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.detectCollisions = true;
            }
        }

        private void CheckBreakDistance()
        {
            if (trackPoint && isDroppable)
            {
                float distance = Vector3.Distance(trackPoint.position, originalControllerAttachPoint.position);
                if (distance > (detachThreshold / 1000))
                {
                    ForceReleaseGrab();
                }
            }
        }

        private void SetTrackPoint(GameObject point)
        {
            var controllerPoint = point.transform;
            var grabScript = point.GetComponent<VRTK_InteractGrab>();

            if (grabScript && grabScript.controllerAttachPoint)
            {
                controllerPoint = grabScript.controllerAttachPoint.transform;
            }

            if (AttachIsTrackObject() && precisionSnap)
            {
                trackPoint = new GameObject(string.Format("[{0}]TrackObject_PrecisionSnap_AttachPoint", gameObject.name)).transform;
                trackPoint.parent = point.transform;
                customTrackPoint = true;
                if (grabAttachMechanic == GrabAttachType.Track_Object)
                {
                    trackPoint.position = transform.position;
                    trackPoint.rotation = transform.rotation;
                }
                else
                {
                    trackPoint.position = controllerPoint.position;
                    trackPoint.rotation = controllerPoint.rotation;
                }
            }
            else
            {
                trackPoint = controllerPoint;
                customTrackPoint = false;
            }

            originalControllerAttachPoint = new GameObject(string.Format("[{0}]Original_Controller_AttachPoint", grabbingObject.name)).transform;
            originalControllerAttachPoint.parent = transform;
            originalControllerAttachPoint.position = trackPoint.position;
            originalControllerAttachPoint.rotation = trackPoint.rotation;
        }

        private void RemoveTrackPoint()
        {
            if (customTrackPoint && trackPoint)
            {
                Destroy(trackPoint.gameObject);
            }
            else
            {
                trackPoint = null;
            }
            if (originalControllerAttachPoint)
            {
                Destroy(originalControllerAttachPoint.gameObject);
            }
        }

        private void FixedUpdateRotatorTrack()
        {
            var rotateForce = trackPoint.position - originalControllerAttachPoint.position;
            rb.AddForceAtPosition(rotateForce, originalControllerAttachPoint.position, ForceMode.VelocityChange);
        }

        private void FixedUpdateTrackObject()
        {
            float maxDistanceDelta = 10f;

            Quaternion rotationDelta;
            Vector3 positionDelta;

            float angle;
            Vector3 axis;

            if (grabbedSnapHandle != null)
            {
                rotationDelta = trackPoint.rotation * Quaternion.Inverse(grabbedSnapHandle.rotation);
                positionDelta = trackPoint.position - grabbedSnapHandle.position;
            }
            else
            {
                rotationDelta = trackPoint.rotation * Quaternion.Inverse(transform.rotation);
                positionDelta = trackPoint.position - transform.position;
            }

            rotationDelta.ToAngleAxis(out angle, out axis);

            angle = (angle > 180 ? angle -= 360 : angle);

            if (angle != 0)
            {
                Vector3 angularTarget = angle * axis;
                rb.angularVelocity = Vector3.MoveTowards(rb.angularVelocity, angularTarget, maxDistanceDelta);
            }

            Vector3 velocityTarget = positionDelta / Time.fixedDeltaTime;
            rb.velocity = Vector3.MoveTowards(rb.velocity, velocityTarget, maxDistanceDelta);
        }

        private void OnTeleporting(object sender, DestinationMarkerEventArgs e)
        {
            if (!stayGrabbedOnTeleport)
            {
                ZeroVelocity();
                ForceStopAllInteractions();
            }
        }

        private void OnTeleported(object sender, DestinationMarkerEventArgs e)
        {
            if (stayGrabbedOnTeleport && AttachIsTrackObject() && trackPoint)
            {
                transform.position = grabbingObject.transform.position;
            }
        }

        private IEnumerator RegisterTeleportersAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            foreach (var teleporter in VRTK_ObjectCache.registeredTeleporters)
            {
                teleporter.Teleporting += new TeleportEventHandler(OnTeleporting);
                teleporter.Teleported += new TeleportEventHandler(OnTeleported);
            }
        }

        private IEnumerator StopUsingOnControllerChange(GameObject previousController)
        {
            yield return new WaitForEndOfFrame();
            var usingObject = previousController.GetComponent<VRTK_InteractUse>();
            if (usingObject)
            {
                if (holdButtonToUse)
                {
                    usingObject.ForceStopUsing();
                }
                else
                {
                    usingObject.ForceResetUsing();
                }
            }
        }

        private IEnumerator ForceStopInteractingAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            ForceStopAllInteractions();
        }

        private void ForceStopAllInteractions()
        {
            if (touchingObjects == null)
            {
                return;
            }

            for (int i = 0; i < touchingObjects.Count; i++)
            {
                var touchingObject = touchingObjects[i];

                if (touchingObject.activeInHierarchy || forceDisabled)
                {
                    touchingObject.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
                    forcedDropped = true;
                }
            }

            if (grabbingObject != null && (grabbingObject.activeInHierarchy || forceDisabled))
            {
                grabbingObject.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
                grabbingObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
                forcedDropped = true;
            }

            if (usingObject != null && (usingObject.activeInHierarchy || forceDisabled))
            {
                usingObject.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
                usingObject.GetComponent<VRTK_InteractUse>().ForceStopUsing();
                forcedDropped = true;
            }
        }
    }
}
