// Interactable Object|Interactions|30030
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Highlighters;
    using GrabAttachMechanics;
    using SecondaryControllerGrabActions;

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
        /// Allowed controller type.
        /// </summary>
        /// <param name="Both">Both controllers are allowed to interact.</param>
        /// <param name="LeftOnly">Only the left controller is allowed to interact.</param>
        /// <param name="RightOnly">Only the right controller is allowed to interact.</param>
        public enum AllowedController
        {
            Both,
            LeftOnly,
            RightOnly
        }

        /// <summary>
        /// The types of valid situations that the object can be released from grab.
        /// </summary>
        /// <param name="NoDrop">The object cannot be dropped via the controller</param>
        /// <param name="DropAnywhere">The object can be dropped anywhere in the scene via the controller.</param>
        /// <param name="DropValidSnapDropZone">The object can only be dropped when it is hovering over a valid snap drop zone.</param>
        public enum ValidDropTypes
        {
            NoDrop,
            DropAnywhere,
            DropValidSnapDropZone
        }

        [Tooltip("If this is checked then the interactable object script will be disabled when the object is not being interacted with. This will eliminate the potential number of calls the interactable objects make each frame.")]
        public bool disableWhenIdle = true;

        [Header("Touch Options", order = 1)]

        [Tooltip("The colour to highlight the object when it is touched. This colour will override any globally set colour (for instance on the `VRTK_InteractTouch` script).")]
        public Color touchHighlightColor = Color.clear;
        [Tooltip("Determines which controller can initiate a touch action.")]
        public AllowedController allowedTouchControllers = AllowedController.Both;
        [Tooltip("An array of colliders on the object to ignore when being touched.")]
        public Collider[] ignoredColliders;

        [Header("Grab Options", order = 2)]

        [Tooltip("Determines if the object can be grabbed.")]
        public bool isGrabbable = false;
        [Tooltip("If this is checked then the grab button on the controller needs to be continually held down to keep grabbing. If this is unchecked the grab button toggles the grab action with one button press to grab and another to release.")]
        public bool holdButtonToGrab = true;
        [Tooltip("If this is checked then the object will stay grabbed to the controller when a teleport occurs. If it is unchecked then the object will be released when a teleport occurs.")]
        public bool stayGrabbedOnTeleport = true;
        [Tooltip("Determines in what situation the object can be dropped by the controller grab button.")]
        public ValidDropTypes validDrop = ValidDropTypes.DropAnywhere;
        [Tooltip("If this is set to `Undefined` then the global grab alias button will grab the object, setting it to any other button will ensure the override button is used to grab this specific interactable object.")]
        public VRTK_ControllerEvents.ButtonAlias grabOverrideButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("Determines which controller can initiate a grab action.")]
        public AllowedController allowedGrabControllers = AllowedController.Both;
        [Tooltip("This determines how the grabbed item will be attached to the controller when it is grabbed. If one isn't provided then the first Grab Attach script on the GameObject will be used, if one is not found and the object is grabbable then a Fixed Joint Grab Attach script will be created at runtime.")]
        public VRTK_BaseGrabAttach grabAttachMechanicScript;
        [Tooltip("The script to utilise when processing the secondary controller action on a secondary grab attempt. If one isn't provided then the first Secondary Controller Grab Action script on the GameObject will be used, if one is not found then no action will be taken on secondary grab.")]
        public VRTK_BaseGrabAction secondaryGrabActionScript;

        [Header("Use Options", order = 3)]

        [Tooltip("Determines if the object can be used.")]
        public bool isUsable = false;
        [Tooltip("If this is checked then the use button on the controller needs to be continually held down to keep using. If this is unchecked the the use button toggles the use action with one button press to start using and another to stop using.")]
        public bool holdButtonToUse = true;
        [Tooltip("If this is checked the object can be used only if it is currently being grabbed.")]
        public bool useOnlyIfGrabbed = false;
        [Tooltip("If this is checked then when a Base Pointer beam (projected from the controller) hits the interactable object, if the object has `Hold Button To Use` unchecked then whilst the pointer is over the object it will run it's `Using` method. If `Hold Button To Use` is unchecked then the `Using` method will be run when the pointer is deactivated. The world pointer will not throw the `Destination Set` event if it is affecting an interactable object with this setting checked as this prevents unwanted teleporting from happening when using an object with a pointer.")]
        public bool pointerActivatesUseAction = false;
        [Tooltip("If this is set to `Undefined` then the global use alias button will use the object, setting it to any other button will ensure the override button is used to use this specific interactable object.")]
        public VRTK_ControllerEvents.ButtonAlias useOverrideButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("Determines which controller can initiate a use action.")]
        public AllowedController allowedUseControllers = AllowedController.Both;

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
        /// Emitted when the object enters a snap drop zone.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectEnteredSnapDropZone;
        /// <summary>
        /// Emitted when the object exists a snap drop zone.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectExitedSnapDropZone;
        /// <summary>
        /// Emitted when the object gets snapped to a drop zone.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectSnappedToDropZone;
        /// <summary>
        /// Emitted when the object gets unsnapped from a drop zone.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectUnsnappedFromDropZone;

        /// <summary>
        /// The current using state of the object. `0` not being used, `1` being used.
        /// </summary>
        [HideInInspector]
        public int usingState = 0;

        /// <summary>
        /// isKinematic is a pass through to the `isKinematic` getter/setter on the object's rigidbody component.
        /// </summary>
        public bool isKinematic
        {
            get
            {
                if (interactableRigidbody)
                {
                    return interactableRigidbody.isKinematic;
                }
                return true;
            }
            set
            {
                if (interactableRigidbody)
                {
                    interactableRigidbody.isKinematic = value;
                }
            }
        }

        protected Rigidbody interactableRigidbody;
        protected List<GameObject> touchingObjects = new List<GameObject>();
        protected List<GameObject> grabbingObjects = new List<GameObject>();
        protected List<GameObject> hoveredSnapObjects = new List<GameObject>();
        protected GameObject usingObject = null;
        protected Transform trackPoint;
        protected bool customTrackPoint = false;
        protected Transform primaryControllerAttachPoint;
        protected Transform secondaryControllerAttachPoint;
        protected Transform previousParent;
        protected bool previousKinematicState;
        protected bool previousIsGrabbable;
        protected bool forcedDropped;
        protected bool forceDisabled;
        protected VRTK_BaseHighlighter objectHighlighter;
        protected bool autoHighlighter = false;
        protected bool hoveredOverSnapDropZone = false;
        protected bool snappedInSnapDropZone = false;
        protected VRTK_SnapDropZone storedSnapDropZone;
        protected Vector3 previousLocalScale = Vector3.zero;
        protected List<GameObject> currentIgnoredColliders = new List<GameObject>();
        protected bool startDisabled = false;

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

        public virtual void OnInteractableObjectEnteredSnapDropZone(InteractableObjectEventArgs e)
        {
            if (InteractableObjectEnteredSnapDropZone != null)
            {
                InteractableObjectEnteredSnapDropZone(this, e);
            }
        }

        public virtual void OnInteractableObjectExitedSnapDropZone(InteractableObjectEventArgs e)
        {
            if (InteractableObjectExitedSnapDropZone != null)
            {
                InteractableObjectExitedSnapDropZone(this, e);
            }
        }

        public virtual void OnInteractableObjectSnappedToDropZone(InteractableObjectEventArgs e)
        {
            if (InteractableObjectSnappedToDropZone != null)
            {
                InteractableObjectSnappedToDropZone(this, e);
            }
        }

        public virtual void OnInteractableObjectUnsnappedFromDropZone(InteractableObjectEventArgs e)
        {
            if (InteractableObjectUnsnappedFromDropZone != null)
            {
                InteractableObjectUnsnappedFromDropZone(this, e);
            }
        }

        public InteractableObjectEventArgs SetInteractableObjectEvent(GameObject interactingObject)
        {
            InteractableObjectEventArgs e;
            e.interactingObject = interactingObject;
            return e;
        }

        /// <summary>
        /// The IsTouched method is used to determine if the object is currently being touched.
        /// </summary>
        /// <returns>Returns `true` if the object is currently being touched.</returns>
        public virtual bool IsTouched()
        {
            return (touchingObjects.Count > 0);
        }

        /// <summary>
        /// The IsGrabbed method is used to determine if the object is currently being grabbed.
        /// </summary>
        /// <param name="grabbedBy">An optional GameObject to check if the Interactable Object is grabbed by that specific GameObject. Defaults to `null`</param>
        /// <returns>Returns `true` if the object is currently being grabbed.</returns>
        public virtual bool IsGrabbed(GameObject grabbedBy = null)
        {
            if (grabbingObjects.Count > 0 && grabbedBy != null)
            {
                return (grabbingObjects.Contains(grabbedBy));
            }
            return (grabbingObjects.Count > 0);
        }

        /// <summary>
        /// The IsUsing method is used to determine if the object is currently being used.
        /// </summary>
        /// <param name="usedBy">An optional GameObject to check if the Interactable Object is used by that specific GameObject. Defaults to `null`</param>
        /// <returns>Returns `true` if the object is currently being used.</returns>
        public virtual bool IsUsing(GameObject usedBy = null)
        {
            if (usingObject && usedBy != null)
            {
                return (usingObject == usedBy);
            }
            return (usingObject != null);
        }

        /// <summary>
        /// The StartTouching method is called automatically when the object is touched initially. It is also a virtual method to allow for overriding in inherited classes.
        /// </summary>
        /// <param name="currentTouchingObject">The game object that is currently touching this object.</param>
        public virtual void StartTouching(GameObject currentTouchingObject)
        {
            IgnoreColliders(currentTouchingObject);
            if (!touchingObjects.Contains(currentTouchingObject))
            {
                ToggleEnableState(true);
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
                ResetUseState(previousTouchingObject);
                OnInteractableObjectUntouched(SetInteractableObjectEvent(previousTouchingObject));
                touchingObjects.Remove(previousTouchingObject);
            }
        }

        /// <summary>
        /// The Grabbed method is called automatically when the object is grabbed initially. It is also a virtual method to allow for overriding in inherited classes.
        /// </summary>
        /// <param name="currentGrabbingObject">The game object that is currently grabbing this object.</param>
        public virtual void Grabbed(GameObject currentGrabbingObject)
        {
            ToggleEnableState(true);
            if (!IsGrabbed() || IsSwappable())
            {
                PrimaryControllerGrab(currentGrabbingObject);
            }
            else
            {
                SecondaryControllerGrab(currentGrabbingObject);
            }
            OnInteractableObjectGrabbed(SetInteractableObjectEvent(currentGrabbingObject));
        }

        /// <summary>
        /// The Ungrabbed method is called automatically when the object has stopped being grabbed. It is also a virtual method to allow for overriding in inherited classes.
        /// </summary>
        /// <param name="previousGrabbingObject">The game object that was previously grabbing this object.</param>
        public virtual void Ungrabbed(GameObject previousGrabbingObject)
        {
            GameObject secondaryGrabbingObject = GetSecondaryGrabbingObject();
            if (!secondaryGrabbingObject || secondaryGrabbingObject != previousGrabbingObject)
            {
                SecondaryControllerUngrab(secondaryGrabbingObject);
                PrimaryControllerUngrab(previousGrabbingObject, secondaryGrabbingObject);
            }
            else
            {
                SecondaryControllerUngrab(previousGrabbingObject);
            }
            OnInteractableObjectUngrabbed(SetInteractableObjectEvent(previousGrabbingObject));
        }

        /// <summary>
        /// The StartUsing method is called automatically when the object is used initially. It is also a virtual method to allow for overriding in inherited classes.
        /// </summary>
        /// <param name="currentUsingObject">The game object that is currently using this object.</param>
        public virtual void StartUsing(GameObject currentUsingObject)
        {
            ToggleEnableState(true);
            if (IsUsing() && !IsUsing(currentUsingObject))
            {
                ResetUsingObject();
            }
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
            ResetUsingObject();
            usingState = 0;
            usingObject = null;
        }

        /// <summary>
        /// The ToggleHighlight method is used to turn on or off the colour highlight of the object.
        /// </summary>
        /// <param name="toggle">The state to determine whether to activate or deactivate the highlight. `true` will enable the highlight and `false` will remove the highlight.</param>
        public virtual void ToggleHighlight(bool toggle)
        {
            InitialiseHighlighter();

            if (touchHighlightColor != Color.clear && objectHighlighter)
            {
                if (toggle && !IsGrabbed())
                {
                    objectHighlighter.Highlight(touchHighlightColor);
                }
                else
                {
                    objectHighlighter.Unhighlight();
                }
            }
        }

        /// <summary>
        /// The ResetHighlighter method is used to reset the currently attached highlighter.
        /// </summary>
        public virtual void ResetHighlighter()
        {
            if (objectHighlighter)
            {
                objectHighlighter.ResetHighlighter();
            }
        }

        /// <summary>
        /// The PauseCollisions method temporarily pauses all collisions on the object at grab time by removing the object's rigidbody's ability to detect collisions. This can be useful for preventing clipping when initially grabbing an item.
        /// </summary>
        /// <param name="delay">The amount of time to pause the collisions for.</param>
        public virtual void PauseCollisions(float delay)
        {
            if (delay > 0f)
            {
                foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
                {
                    rb.detectCollisions = false;
                }
                Invoke("UnpauseCollisions", delay);
            }
        }

        /// <summary>
        /// The ZeroVelocity method resets the velocity and angular velocity to zero on the rigidbody attached to the object.
        /// </summary>
        public virtual void ZeroVelocity()
        {
            if (interactableRigidbody)
            {
                interactableRigidbody.velocity = Vector3.zero;
                interactableRigidbody.angularVelocity = Vector3.zero;
            }
        }

        /// <summary>
        /// The SaveCurrentState method stores the existing object parent and the object's rigidbody kinematic setting.
        /// </summary>
        public virtual void SaveCurrentState()
        {
            if (!IsGrabbed() && !snappedInSnapDropZone)
            {
                previousParent = transform.parent;
                if (!IsSwappable())
                {
                    previousIsGrabbable = isGrabbable;
                }

                if (interactableRigidbody)
                {
                    previousKinematicState = interactableRigidbody.isKinematic;
                }
            }
        }

        /// <summary>
        /// The GetTouchingObjects method is used to return the collecetion of valid game objects that are currently touching this object.
        /// </summary>
        /// <returns>A list of game object of that are currently touching the current object.</returns>
        public virtual List<GameObject> GetTouchingObjects()
        {
            return touchingObjects;
        }

        /// <summary>
        /// The GetGrabbingObject method is used to return the game object that is currently grabbing this object.
        /// </summary>
        /// <returns>The game object of what is grabbing the current object.</returns>
        public virtual GameObject GetGrabbingObject()
        {
            return (IsGrabbed() ? grabbingObjects[0] : null);
        }

        /// <summary>
        /// The GetSecondaryGrabbingObject method is used to return the game object that is currently being used to influence this object whilst it is being grabbed by a secondary controller.
        /// </summary>
        /// <returns>The game object of the secondary controller influencing the current grabbed object.</returns>
        public virtual GameObject GetSecondaryGrabbingObject()
        {
            return (grabbingObjects.Count > 1 ? grabbingObjects[1] : null);
        }

        /// <summary>
        /// The GetUsingObject method is used to return the game object that is currently using this object.
        /// </summary>
        /// <returns>The game object of what is using the current object.</returns>
        public virtual GameObject GetUsingObject()
        {
            return usingObject;
        }

        /// <summary>
        /// The IsValidInteractableController method is used to check to see if a controller is allowed to perform an interaction with this object as sometimes controllers are prohibited from grabbing or using an object depedning on the use case.
        /// </summary>
        /// <param name="actualController">The game object of the controller that is being checked.</param>
        /// <param name="controllerCheck">The value of which controller is allowed to interact with this object.</param>
        /// <returns>Is true if the interacting controller is allowed to grab the object.</returns>
        public virtual bool IsValidInteractableController(GameObject actualController, AllowedController controllerCheck)
        {
            if (controllerCheck == AllowedController.Both)
            {
                return true;
            }

            var controllerHand = VRTK_DeviceFinder.GetControllerHandType(controllerCheck.ToString().Replace("Only", ""));
            return (VRTK_DeviceFinder.IsControllerOfHand(actualController, controllerHand));
        }

        /// <summary>
        /// The ForceStopInteracting method forces the object to no longer be interacted with and will cause a controller to drop the object and stop touching it. This is useful if the controller is required to auto interact with another object.
        /// </summary>
        public virtual void ForceStopInteracting()
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
        /// The ForceStopSecondaryGrabInteraction method forces the object to no longer be influenced by the second controller grabbing it.
        /// </summary>
        public virtual void ForceStopSecondaryGrabInteraction()
        {
            var grabbingObject = GetSecondaryGrabbingObject();
            if (grabbingObject)
            {
                grabbingObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
            }
        }

        /// <summary>
        /// The RegisterTeleporters method is used to find all objects that have a teleporter script and register the object on the `OnTeleported` event. This is used internally by the object for keeping Tracked objects positions updated after teleporting.
        /// </summary>
        public virtual void RegisterTeleporters()
        {
            StartCoroutine(RegisterTeleportersAtEndOfFrame());
        }

        /// <summary>
        /// The UnregisterTeleporters method is used to unregister all teleporter events that are active on this object.
        /// </summary>
        public virtual void UnregisterTeleporters()
        {
            foreach (var teleporter in VRTK_ObjectCache.registeredTeleporters)
            {
                teleporter.Teleporting -= new TeleportEventHandler(OnTeleporting);
                teleporter.Teleported -= new TeleportEventHandler(OnTeleported);
            }
        }

        /// <summary>
        /// the StoreLocalScale method saves the current transform local scale values.
        /// </summary>
        public virtual void StoreLocalScale()
        {
            previousLocalScale = transform.localScale;
        }

        /// <summary>
        /// The ToggleSnapDropZone method is used to set the state of whether the interactable object is in a Snap Drop Zone or not.
        /// </summary>
        /// <param name="snapDropZone">The Snap Drop Zone object that is being interacted with.</param>
        /// <param name="state">The state of whether the interactable object is fixed in or removed from the Snap Drop Zone. True denotes the interactable object is fixed to the Snap Drop Zone and false denotes it has been removed from the Snap Drop Zone.</param>
        public virtual void ToggleSnapDropZone(VRTK_SnapDropZone snapDropZone, bool state)
        {
            snappedInSnapDropZone = state;
            if (state)
            {
                storedSnapDropZone = snapDropZone;
                OnInteractableObjectSnappedToDropZone(SetInteractableObjectEvent(snapDropZone.gameObject));
            }
            else
            {
                ResetDropSnapType();
                OnInteractableObjectUnsnappedFromDropZone(SetInteractableObjectEvent(snapDropZone.gameObject));
            }
        }

        /// <summary>
        /// The IsInSnapDropZone method determines whether the interactable object is currently snapped to a drop zone.
        /// </summary>
        /// <returns>Returns true if the interactable object is currently snapped in a drop zone and returns false if it is not.</returns>
        public virtual bool IsInSnapDropZone()
        {
            return snappedInSnapDropZone;
        }

        /// <summary>
        /// The SetSnapDropZoneHover method sets whether the interactable object is currently being hovered over a valid Snap Drop Zone.
        /// </summary>
        /// <param name="snapDropZone">The Snap Drop Zone object that is being interacted with.</param>
        /// <param name="state">The state of whether the object is being hovered or not.</param>
        public virtual void SetSnapDropZoneHover(VRTK_SnapDropZone snapDropZone, bool state)
        {
            if (state)
            {
                if (!hoveredSnapObjects.Contains(snapDropZone.gameObject))
                {
                    hoveredSnapObjects.Add(snapDropZone.gameObject);
                    OnInteractableObjectEnteredSnapDropZone(SetInteractableObjectEvent(snapDropZone.gameObject));
                }
            }
            else
            {
                if (hoveredSnapObjects.Contains(snapDropZone.gameObject))
                {
                    hoveredSnapObjects.Remove(snapDropZone.gameObject);
                    OnInteractableObjectExitedSnapDropZone(SetInteractableObjectEvent(snapDropZone.gameObject));
                }
            }
            hoveredOverSnapDropZone = hoveredSnapObjects.Count > 0;
        }

        /// <summary>
        /// The GetStoredSnapDropZone method returns the snap drop zone that the interactable object is currently snapped to.
        /// </summary>
        /// <returns>The SnapDropZone that the interactable object is currently snapped to.</returns>
        public virtual VRTK_SnapDropZone GetStoredSnapDropZone()
        {
            return storedSnapDropZone;
        }

        /// <summary>
        /// The IsDroppable method returns whether the object can be dropped or not in it's current situation.
        /// </summary>
        /// <returns>Returns true if the object can currently be dropped and returns false if it is not currently possible to drop.</returns>
        public virtual bool IsDroppable()
        {
            switch (validDrop)
            {
                case ValidDropTypes.NoDrop:
                    return false;
                case ValidDropTypes.DropAnywhere:
                    return true;
                case ValidDropTypes.DropValidSnapDropZone:
                    return hoveredOverSnapDropZone;
            }
            return false;
        }

        /// <summary>
        /// The IsSwappable method returns whether the object can be grabbed with one controller and then swapped to another controller by grabbing with the secondary controller.
        /// </summary>
        /// <returns>Returns true if the object can be grabbed by a secondary controller whilst already being grabbed and the object will swap controllers. Returns false if the object cannot be swapped.</returns>
        public virtual bool IsSwappable()
        {
            return (secondaryGrabActionScript ? secondaryGrabActionScript.IsSwappable() : false);
        }

        /// <summary>
        /// The PerformSecondaryAction method returns whether the object has a secondary action that can be performed when grabbing the object with a secondary controller.
        /// </summary>
        /// <returns>Returns true if the obejct has a secondary action, returns false if it has no secondary action or is swappable.</returns>
        public virtual bool PerformSecondaryAction()
        {
            return (!GetSecondaryGrabbingObject() && secondaryGrabActionScript ? secondaryGrabActionScript.IsActionable() : false);
        }

        /// <summary>
        /// The ResetIgnoredColliders method is used to clear any stored ignored colliders in case the `Ignored Colliders` array parameter is changed at runtime. This needs to be called manually if changes are made at runtime.
        /// </summary>
        public virtual void ResetIgnoredColliders()
        {
            currentIgnoredColliders.Clear();
        }

        protected virtual void Awake()
        {
            interactableRigidbody = GetComponent<Rigidbody>();
            if (interactableRigidbody)
            {
                interactableRigidbody.maxAngularVelocity = float.MaxValue;
            }

            if (disableWhenIdle && enabled)
            {
                startDisabled = true;
                enabled = false;
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
            forcedDropped = false;
            startDisabled = false;
        }

        protected virtual void OnDisable()
        {
            UnregisterTeleporters();

            if (autoHighlighter)
            {
                Destroy(objectHighlighter);
                objectHighlighter = null;
            }

            if (!startDisabled)
            {
                forceDisabled = true;
                ForceStopInteracting();
            }
        }

        protected virtual void FixedUpdate()
        {
            if (trackPoint && grabAttachMechanicScript)
            {
                grabAttachMechanicScript.ProcessFixedUpdate();
            }

            if (secondaryGrabActionScript)
            {
                secondaryGrabActionScript.ProcessFixedUpdate();
            }
        }

        protected virtual void Update()
        {
            AttemptSetGrabMechanic();
            AttemptSetSecondaryGrabAction();

            if (trackPoint && grabAttachMechanicScript)
            {
                grabAttachMechanicScript.ProcessUpdate();
            }

            if (secondaryGrabActionScript)
            {
                secondaryGrabActionScript.ProcessUpdate();
            }
        }

        protected virtual void LateUpdate()
        {
            if (disableWhenIdle && !IsTouched() && !IsGrabbed() && !IsUsing())
            {
                ToggleEnableState(false);
            }
        }

        protected virtual void LoadPreviousState()
        {
            if (gameObject.activeInHierarchy)
            {
                transform.SetParent(previousParent);
                forcedDropped = false;
            }
            if (interactableRigidbody)
            {
                interactableRigidbody.isKinematic = previousKinematicState;
            }
            if (!IsSwappable())
            {
                isGrabbable = previousIsGrabbable;
            }
        }

        protected virtual void InitialiseHighlighter()
        {
            if (touchHighlightColor != Color.clear && !objectHighlighter)
            {
                autoHighlighter = false;
                objectHighlighter = VRTK_BaseHighlighter.GetActiveHighlighter(gameObject);
                if (objectHighlighter == null)
                {
                    autoHighlighter = true;
                    objectHighlighter = gameObject.AddComponent<VRTK_MaterialColorSwapHighlighter>();
                }
                objectHighlighter.Initialise(touchHighlightColor);
            }
        }

        protected virtual void IgnoreColliders(GameObject touchingObject)
        {
            if (ignoredColliders != null && !currentIgnoredColliders.Contains(touchingObject))
            {
                bool objectIgnored = false;
                Collider[] touchingColliders = touchingObject.GetComponentsInChildren<Collider>();
                for (int i = 0; i < ignoredColliders.Length; i++)
                {
                    for (int j = 0; j < touchingColliders.Length; j++)
                    {
                        Physics.IgnoreCollision(touchingColliders[j], ignoredColliders[i]);
                        objectIgnored = true;
                    }
                }

                if (objectIgnored)
                {
                    currentIgnoredColliders.Add(touchingObject);
                }
            }
        }

        protected virtual void ToggleEnableState(bool state)
        {
            if (disableWhenIdle)
            {
                enabled = state;
            }
        }

        protected virtual void AttemptSetGrabMechanic()
        {
            if (isGrabbable && grabAttachMechanicScript == null)
            {
                var setGrabMechanic = GetComponent<VRTK_BaseGrabAttach>();
                if (!setGrabMechanic)
                {
                    setGrabMechanic = gameObject.AddComponent<VRTK_FixedJointGrabAttach>();
                }
                grabAttachMechanicScript = setGrabMechanic;
            }
        }

        protected virtual void AttemptSetSecondaryGrabAction()
        {
            if (isGrabbable && secondaryGrabActionScript == null)
            {
                secondaryGrabActionScript = GetComponent<VRTK_BaseGrabAction>();
            }
        }

        protected virtual void ForceReleaseGrab()
        {
            var grabbingObject = GetGrabbingObject();
            if (grabbingObject)
            {
                grabbingObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
            }
        }

        protected virtual void PrimaryControllerGrab(GameObject currentGrabbingObject)
        {
            if (snappedInSnapDropZone)
            {
                ToggleSnapDropZone(storedSnapDropZone, false);
            }
            ForceReleaseGrab();
            RemoveTrackPoint();
            grabbingObjects.Add(currentGrabbingObject);
            SetTrackPoint(currentGrabbingObject);
            if (!IsSwappable())
            {
                previousIsGrabbable = isGrabbable;
                isGrabbable = false;
            }
        }

        protected virtual void SecondaryControllerGrab(GameObject currentGrabbingObject)
        {
            if (!grabbingObjects.Contains(currentGrabbingObject))
            {
                grabbingObjects.Add(currentGrabbingObject);
                secondaryControllerAttachPoint = CreateAttachPoint(currentGrabbingObject.name, "Secondary", currentGrabbingObject.transform);

                if (secondaryGrabActionScript)
                {
                    secondaryGrabActionScript.Initialise(this, GetGrabbingObject().GetComponent<VRTK_InteractGrab>(), GetSecondaryGrabbingObject().GetComponent<VRTK_InteractGrab>(), primaryControllerAttachPoint, secondaryControllerAttachPoint);
                }
            }
        }

        protected virtual void PrimaryControllerUngrab(GameObject previousGrabbingObject, GameObject previousSecondaryGrabbingObject)
        {
            UnpauseCollisions();
            RemoveTrackPoint();
            ResetUseState(previousGrabbingObject);
            grabbingObjects.Clear();
            if (secondaryGrabActionScript != null && previousSecondaryGrabbingObject != null)
            {
                secondaryGrabActionScript.OnDropAction();
                previousSecondaryGrabbingObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
            }
            LoadPreviousState();
        }

        protected virtual void SecondaryControllerUngrab(GameObject previousGrabbingObject)
        {
            if (grabbingObjects.Contains(previousGrabbingObject))
            {
                grabbingObjects.Remove(previousGrabbingObject);
                Destroy(secondaryControllerAttachPoint.gameObject);
                secondaryControllerAttachPoint = null;
                if (secondaryGrabActionScript)
                {
                    secondaryGrabActionScript.ResetAction();
                }
            }
        }

        protected virtual void UnpauseCollisions()
        {
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.detectCollisions = true;
            }
        }

        protected virtual void SetTrackPoint(GameObject currentGrabbingObject)
        {
            AddTrackPoint(currentGrabbingObject);
            primaryControllerAttachPoint = CreateAttachPoint(GetGrabbingObject().name, "Original", trackPoint);

            if (grabAttachMechanicScript)
            {
                grabAttachMechanicScript.SetTrackPoint(trackPoint);
                grabAttachMechanicScript.SetInitialAttachPoint(primaryControllerAttachPoint);
            }
        }

        protected virtual Transform CreateAttachPoint(string namePrefix, string nameSuffix, Transform origin)
        {
            var attachPoint = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, namePrefix, nameSuffix, "Controller", "AttachPoint")).transform;
            attachPoint.parent = transform;
            attachPoint.position = origin.position;
            attachPoint.rotation = origin.rotation;
            return attachPoint;
        }

        protected virtual void AddTrackPoint(GameObject currentGrabbingObject)
        {
            var grabScript = currentGrabbingObject.GetComponent<VRTK_InteractGrab>();
            var controllerPoint = ((grabScript && grabScript.controllerAttachPoint) ? grabScript.controllerAttachPoint.transform : currentGrabbingObject.transform);

            if (grabAttachMechanicScript)
            {
                trackPoint = grabAttachMechanicScript.CreateTrackPoint(controllerPoint, gameObject, currentGrabbingObject, ref customTrackPoint);
            }
        }

        protected virtual void RemoveTrackPoint()
        {
            if (customTrackPoint && trackPoint)
            {
                Destroy(trackPoint.gameObject);
            }
            else
            {
                trackPoint = null;
            }
            if (primaryControllerAttachPoint)
            {
                Destroy(primaryControllerAttachPoint.gameObject);
            }
        }

        protected virtual void OnTeleporting(object sender, DestinationMarkerEventArgs e)
        {
            if (!stayGrabbedOnTeleport)
            {
                ZeroVelocity();
                ForceStopAllInteractions();
            }
        }

        protected virtual void OnTeleported(object sender, DestinationMarkerEventArgs e)
        {
            if (grabAttachMechanicScript && grabAttachMechanicScript.IsTracked() && stayGrabbedOnTeleport && trackPoint)
            {
                var actualController = VRTK_DeviceFinder.GetActualController(GetGrabbingObject());
                transform.position = (actualController ? actualController.transform.position : transform.position);
            }
        }

        protected virtual IEnumerator RegisterTeleportersAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            foreach (var teleporter in VRTK_ObjectCache.registeredTeleporters)
            {
                teleporter.Teleporting += new TeleportEventHandler(OnTeleporting);
                teleporter.Teleported += new TeleportEventHandler(OnTeleported);
            }
        }

        protected virtual void ResetUseState(GameObject checkObject)
        {
            var usingObjectCheck = checkObject.GetComponent<VRTK_InteractUse>();
            if (usingObjectCheck)
            {
                if (holdButtonToUse)
                {
                    usingObjectCheck.ForceStopUsing();
                }
            }
        }

        protected virtual IEnumerator ForceStopInteractingAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            ForceStopAllInteractions();
        }

        protected virtual void ForceStopAllInteractions()
        {
            if (touchingObjects == null)
            {
                return;
            }

            StopTouchingInteractions();
            StopGrabbingInteractions();
            StopUsingInteractions();
        }

        protected virtual void StopTouchingInteractions()
        {
            for (int i = 0; i < touchingObjects.Count; i++)
            {
                var touchingObject = touchingObjects[i];

                if (touchingObject.activeInHierarchy || forceDisabled)
                {
                    touchingObject.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
                }
            }
        }

        protected virtual void StopGrabbingInteractions()
        {
            var grabbingObject = GetGrabbingObject();

            if (grabbingObject != null && (grabbingObject.activeInHierarchy || forceDisabled))
            {
                grabbingObject.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
                grabbingObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
                forcedDropped = true;
            }
        }

        protected virtual void StopUsingInteractions()
        {
            if (usingObject != null && (usingObject.activeInHierarchy || forceDisabled))
            {
                usingObject.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
                usingObject.GetComponent<VRTK_InteractUse>().ForceStopUsing();
            }
        }

        protected virtual void ResetDropSnapType()
        {
            switch (storedSnapDropZone.snapType)
            {
                case VRTK_SnapDropZone.SnapTypes.UseKinematic:
                case VRTK_SnapDropZone.SnapTypes.UseParenting:
                    LoadPreviousState();
                    break;
                case VRTK_SnapDropZone.SnapTypes.UseJoint:
                    var snapDropZoneJoint = storedSnapDropZone.GetComponent<Joint>();
                    if (snapDropZoneJoint)
                    {
                        snapDropZoneJoint.connectedBody = null;
                    }
                    break;
            }

            if (!previousLocalScale.Equals(Vector3.zero))
            {
                transform.localScale = previousLocalScale;
            }

            storedSnapDropZone.OnObjectUnsnappedFromDropZone(storedSnapDropZone.SetSnapDropZoneEvent(gameObject));
            storedSnapDropZone = null;
        }

        protected virtual void ResetUsingObject()
        {
            if (usingObject)
            {
                var usingObjectScript = usingObject.GetComponent<VRTK_InteractUse>();
                if (usingObjectScript)
                {
                    usingObjectScript.ForceResetUsing();
                }
            }
        }
    }
}