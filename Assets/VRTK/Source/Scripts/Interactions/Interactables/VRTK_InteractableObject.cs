// Interactable Object|Interactables|35010
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using GrabAttachMechanics;
    using SecondaryControllerGrabActions;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="interactingObject">The GameObject that is initiating the interaction (e.g. a controller).</param>
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
    /// Determines if the GameObject can be interacted with.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `Collider` - A Unity Collider to determine when an interaction has occured. Can be a compound collider set in child GameObjects.
    ///
    /// **Optional Components:**
    ///  * `Rigidbody` - A Unity Rigidbody to allow the GameObject to be affected by the Unity Physics System (not required for Climbable Grab Attach Types).
    ///  * `VRTK_BaseGrabAttach` - A Grab Attach mechanic for determining how the Interactable Object is grabbed by the primary interacting object.
    ///  * `VRTK_BaseGrabAction` - A Grab Action mechanic for determining how to manipulate the Interactable Object when grabbed by the secondary interacting object.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_InteractableObject` script onto the GameObject that is to be interactable.
    ///  * Alternatively, select the GameObject and use the `Window -> VRTK -> Setup Interactable Object` panel to set up quickly.
    ///  * The optional Highlighter used by the Interactable Object will be selected in the following order:
    ///    * The provided Base Highlighter in the `Object Highlighter` parameter.
    ///    * If the above is not provided, then the first active Base Highlighter found on the Interactable Object GameObject will be used.
    ///    * If the above is not found, then a Material Color Swap Highlighter will be created on the Interactable Object GameObject at runtime.
    ///
    /// **Script Dependencies:**
    ///  * Interactions
    ///    * To near touch an Interactable Object the Interact NearTouch script is required on a controller Script Alias GameObject.
    ///    * To touch an Interactable Object the Interact NearTouch script is required on a controller Script Alias GameObject.
    ///    * To grab an Interactable Object the Interact Grab script is required on a controller Script Alias GameObject.
    ///    * To use an Interactable Object the Interact Use script is required on a controller Script Alias GameObject.
    ///  * Highlighting
    ///    * To highlight an Interactable Object on a given interaction then a valid Interact Object Highlighter script must be associated with the Interactable Object.
    ///  * Appearance
    ///    * To affect the appearance of an Interactable Object then a valid Interact Object Appearance script must be associated with the Interactable Object.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/005_Controller_BasicObjectGrabbing` uses the `VRTK_InteractTouch` and `VRTK_InteractGrab` scripts on the controllers to show how an interactable object can be grabbed and snapped to the controller and thrown around the game world.
    ///
    /// `VRTK/Examples/013_Controller_UsingAndGrabbingMultipleObjects` shows multiple objects that can be grabbed by holding the buttons or grabbed by toggling the button click and also has objects that can have their Using state toggled to show how multiple items can be turned on at the same time.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/VRTK_InteractableObject")]
    public class VRTK_InteractableObject : MonoBehaviour
    {
        /// <summary>
        /// The interaction type.
        /// </summary>
        public enum InteractionType
        {
            /// <summary>
            /// No interaction is affecting the object.
            /// </summary>
            None,
            /// <summary>
            /// The near touch interaction is affecting the object.
            /// </summary>
            NearTouch,
            /// <summary>
            /// The near untouch interaction stopped affecting the object
            /// </summary>
            NearUntouch,
            /// <summary>
            /// The touch interaction is affecting the object.
            /// </summary>
            Touch,
            /// <summary>
            /// The untouch interaction stopped affecting the object
            /// </summary>
            Untouch,
            /// <summary>
            /// The grab interaction is affecting the object.
            /// </summary>
            Grab,
            /// <summary>
            /// The ungrab interaction stopped affecting the object
            /// </summary>
            Ungrab,
            /// <summary>
            /// The use interaction is affecting the object.
            /// </summary>
            Use,
            /// <summary>
            /// The unuse interaction stopped affecting the object
            /// </summary>
            Unuse
        }

        /// <summary>
        /// Allowed controller type.
        /// </summary>
        public enum AllowedController
        {
            /// <summary>
            /// Both controllers are allowed to interact.
            /// </summary>
            Both,
            /// <summary>
            /// Only the left controller is allowed to interact.
            /// </summary>
            LeftOnly,
            /// <summary>
            /// Only the right controller is allowed to interact.
            /// </summary>
            RightOnly
        }

        /// <summary>
        /// The types of valid situations that the object can be released from grab.
        /// </summary>
        public enum ValidDropTypes
        {
            /// <summary>
            /// The object cannot be dropped via the controller.
            /// </summary>
            NoDrop,
            /// <summary>
            /// The object can be dropped anywhere in the scene via the controller.
            /// </summary>
            DropAnywhere,
            /// <summary>
            /// The object can only be dropped when it is hovering over a valid snap drop zone.
            /// </summary>
            DropValidSnapDropZone
        }

        [Header("General Settings")]

        [Tooltip("If this is checked then the Interactable Object component will be disabled when the Interactable Object is not being interacted with.")]
        public bool disableWhenIdle = true;

        [Header("Near Touch Settings")]

        [Tooltip("Determines which controller can initiate a near touch action.")]
        public AllowedController allowedNearTouchControllers = AllowedController.Both;

        [Header("Touch Settings")]

        [Tooltip("Determines which controller can initiate a touch action.")]
        public AllowedController allowedTouchControllers = AllowedController.Both;
        [Tooltip("An array of colliders on the GameObject to ignore when being touched.")]
        public Collider[] ignoredColliders;

        [Header("Grab Settings")]

        [Tooltip("Determines if the Interactable Object can be grabbed.")]
        public bool isGrabbable = false;
        [Tooltip("If this is checked then the grab button on the controller needs to be continually held down to keep grabbing. If this is unchecked the grab button toggles the grab action with one button press to grab and another to release.")]
        public bool holdButtonToGrab = true;
        [Tooltip("If this is checked then the Interactable Object will stay grabbed to the controller when a teleport occurs. If it is unchecked then the Interactable Object will be released when a teleport occurs.")]
        public bool stayGrabbedOnTeleport = true;
        [Tooltip("Determines in what situation the Interactable Object can be dropped by the controller grab button.")]
        public ValidDropTypes validDrop = ValidDropTypes.DropAnywhere;
        [Tooltip("Setting to a button will ensure the override button is used to grab this specific Interactable Object. Setting to `Undefined` will mean the `Grab Button` on the Interact Grab script will grab the object.")]
        public VRTK_ControllerEvents.ButtonAlias grabOverrideButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("Determines which controller can initiate a grab action.")]
        public AllowedController allowedGrabControllers = AllowedController.Both;
        [Tooltip("This determines how the grabbed Interactable Object will be attached to the controller when it is grabbed. If one isn't provided then the first Grab Attach script on the GameObject will be used, if one is not found and the object is grabbable then a Fixed Joint Grab Attach script will be created at runtime.")]
        public VRTK_BaseGrabAttach grabAttachMechanicScript;
        [Tooltip("The script to utilise when processing the secondary controller action on a secondary grab attempt. If one isn't provided then the first Secondary Controller Grab Action script on the GameObject will be used, if one is not found then no action will be taken on secondary grab.")]
        public VRTK_BaseGrabAction secondaryGrabActionScript;

        [Header("Use Settings")]

        [Tooltip("Determines if the Interactable Object can be used.")]
        public bool isUsable = false;
        [Tooltip("If this is checked then the use button on the controller needs to be continually held down to keep using. If this is unchecked the the use button toggles the use action with one button press to start using and another to stop using.")]
        public bool holdButtonToUse = true;
        [Tooltip("If this is checked the Interactable Object can be used only if it is currently being grabbed.")]
        public bool useOnlyIfGrabbed = false;
        [Tooltip("If this is checked then when a Pointer collides with the Interactable Object it will activate it's use action. If the the `Hold Button To Use` parameter is unchecked then whilst the Pointer is collising with the Interactable Object it will run the `Using` method. If `Hold Button To Use` is unchecked then the `Using` method will be run when the Pointer is deactivated. The Pointer will not emit the `Destination Set` event if it is affecting an Interactable Object with this setting checked as this prevents unwanted teleporting from happening when using an Interactable Object with a pointer.")]
        public bool pointerActivatesUseAction = false;
        [Tooltip("Setting to a button will ensure the override button is used to use this specific Interactable Object. Setting to `Undefined` will mean the `Use Button` on the Interact Use script will use the object.")]
        public VRTK_ControllerEvents.ButtonAlias useOverrideButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("Determines which controller can initiate a use action.")]
        public AllowedController allowedUseControllers = AllowedController.Both;

        [Header("Obsolete Settings")]

        [System.Obsolete("`VRTK_InteractableObject.objectHighlighter` has been replaced with `VRTK_InteractObjectHighlighter.objectHighlighter`. This parameter will be removed in a future version of VRTK.")]
        [ObsoleteInspector]
        public Highlighters.VRTK_BaseHighlighter objectHighlighter;
        [System.Obsolete("`VRTK_InteractableObject.touchHighlightColor` has been replaced with `VRTK_InteractObjectHighlighter.touchHighlight`. This parameter will be removed in a future version of VRTK.")]
        [ObsoleteInspector]
        public Color touchHighlightColor = Color.clear;

        protected Rigidbody interactableRigidbody;
        protected HashSet<GameObject> currentIgnoredColliders = new HashSet<GameObject>();
        protected HashSet<GameObject> hoveredSnapObjects = new HashSet<GameObject>();
        protected HashSet<GameObject> nearTouchingObjects = new HashSet<GameObject>();
        protected HashSet<GameObject> touchingObjects = new HashSet<GameObject>();
        protected List<GameObject> grabbingObjects = new List<GameObject>();
        protected VRTK_InteractUse usingObject = null;
        protected Transform trackPoint;
        protected bool customTrackPoint = false;
        protected Transform primaryControllerAttachPoint;
        protected Transform secondaryControllerAttachPoint;
        protected Transform previousParent;
        protected bool previousKinematicState;
        protected bool previousIsGrabbable;
        protected bool forcedDropped;
        protected bool forceDisabled;
        protected bool hoveredOverSnapDropZone = false;
        protected bool snappedInSnapDropZone = false;
        protected VRTK_SnapDropZone storedSnapDropZone;
        protected Vector3 previousLocalScale = Vector3.zero;
        protected bool startDisabled = false;

        /// <summary>
        /// Emitted when the Interactable Object script is enabled;
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectEnabled;
        /// <summary>
        /// Emitted when the Interactable Object script is disabled;
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectDisabled;
        /// <summary>
        /// Emitted when another interacting object near touches the current Interactable Object.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectNearTouched;
        /// <summary>
        /// Emitted when the other interacting object stops near touching the current Interactable Object.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectNearUntouched;
        /// <summary>
        /// Emitted when another interacting object touches the current Interactable Object.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectTouched;
        /// <summary>
        /// Emitted when the other interacting object stops touching the current Interactable Object.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectUntouched;
        /// <summary>
        /// Emitted when another interacting object grabs the current Interactable Object.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectGrabbed;
        /// <summary>
        /// Emitted when the other interacting object stops grabbing the current Interactable Object.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectUngrabbed;
        /// <summary>
        /// Emitted when another interacting object uses the current Interactable Object.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectUsed;
        /// <summary>
        /// Emitted when the other interacting object stops using the current Interactable Object.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectUnused;
        /// <summary>
        /// Emitted when the Interactable Object enters a Snap Drop Zone.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectEnteredSnapDropZone;
        /// <summary>
        /// Emitted when the Interactable Object exists a Snap Drop Zone.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectExitedSnapDropZone;
        /// <summary>
        /// Emitted when the Interactable Object gets snapped to a Snap Drop Zone.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectSnappedToDropZone;
        /// <summary>
        /// Emitted when the Interactable Object gets unsnapped from a Snap Drop Zone.
        /// </summary>
        public event InteractableObjectEventHandler InteractableObjectUnsnappedFromDropZone;

        /// <summary>
        /// The current using state of the Interactable Object. `0` not being used, `1` being used.
        /// </summary>
        [HideInInspector]
        public int usingState = 0;

        /// <summary>
        /// isKinematic is a pass through to the `isKinematic` getter/setter on the Interactable Object's Rigidbody component.
        /// </summary>
        public bool isKinematic
        {
            get
            {
                if (interactableRigidbody != null)
                {
                    return interactableRigidbody.isKinematic;
                }
                return true;
            }
            set
            {
                if (interactableRigidbody != null)
                {
                    interactableRigidbody.isKinematic = value;
                }
            }
        }

        public virtual void OnInteractableObjectEnabled(InteractableObjectEventArgs e)
        {
            if (InteractableObjectEnabled != null)
            {
                InteractableObjectEnabled(this, e);
            }
        }

        public virtual void OnInteractableObjectDisabled(InteractableObjectEventArgs e)
        {
            if (InteractableObjectDisabled != null)
            {
                InteractableObjectDisabled(this, e);
            }
        }

        public virtual void OnInteractableObjectNearTouched(InteractableObjectEventArgs e)
        {
            if (InteractableObjectNearTouched != null)
            {
                InteractableObjectNearTouched(this, e);
            }
        }

        public virtual void OnInteractableObjectNearUntouched(InteractableObjectEventArgs e)
        {
            if (InteractableObjectNearUntouched != null)
            {
                InteractableObjectNearUntouched(this, e);
            }
        }

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
        /// The IsNearTouched method is used to determine if the Interactable Object is currently being near touched.
        /// </summary>
        /// <returns>Returns `true` if the Interactable Object is currently being near touched.</returns>
        public virtual bool IsNearTouched()
        {
            return (!IsTouched() && nearTouchingObjects.Count > 0);
        }

        /// <summary>
        /// The IsTouched method is used to determine if the Interactable Object is currently being touched.
        /// </summary>
        /// <returns>Returns `true` if the Interactable Object is currently being touched.</returns>
        public virtual bool IsTouched()
        {
            return (touchingObjects.Count > 0);
        }

        /// <summary>
        /// The IsGrabbed method is used to determine if the Interactable Object is currently being grabbed.
        /// </summary>
        /// <param name="grabbedBy">An optional GameObject to check if the Interactable Object is grabbed by that specific GameObject. Defaults to `null`</param>
        /// <returns>Returns `true` if the Interactable Object is currently being grabbed.</returns>
        public virtual bool IsGrabbed(GameObject grabbedBy = null)
        {
            if (grabbingObjects.Count > 0 && grabbedBy != null)
            {
                return (grabbingObjects.Contains(grabbedBy));
            }
            return (grabbingObjects.Count > 0);
        }

        /// <summary>
        /// The IsUsing method is used to determine if the Interactable Object is currently being used.
        /// </summary>
        /// <param name="usedBy">An optional GameObject to check if the Interactable Object is used by that specific GameObject. Defaults to `null`</param>
        /// <returns>Returns `true` if the Interactable Object is currently being used.</returns>
        public virtual bool IsUsing(GameObject usedBy = null)
        {
            if (usingObject != null && usedBy != null)
            {
                return (usingObject.gameObject == usedBy);
            }
            return (usingObject != null);
        }

        /// <summary>
        /// The StartNearTouching method is called automatically when the Interactable Object is initially nearly touched.
        /// </summary>
        /// <param name="currentNearTouchingObject">The interacting object that is currently nearly touching this Interactable Object.</param>
        public virtual void StartNearTouching(VRTK_InteractNearTouch currentNearTouchingObject = null)
        {
            GameObject currentNearTouchingGameObject = (currentNearTouchingObject != null ? currentNearTouchingObject.gameObject : null);
            if (currentNearTouchingGameObject != null)
            {
                if (nearTouchingObjects.Add(currentNearTouchingGameObject))
                {
                    ToggleEnableState(true);
                    OnInteractableObjectNearTouched(SetInteractableObjectEvent(currentNearTouchingGameObject));
                }
            }
        }

        /// <summary>
        /// The StopNearTouching method is called automatically when the Interactable Object has stopped being nearly touched.
        /// </summary>
        /// <param name="previousNearTouchingObject">The interacting object that was previously nearly touching this Interactable Object.</param>
        public virtual void StopNearTouching(VRTK_InteractNearTouch previousNearTouchingObject = null)
        {
            GameObject previousNearTouchingGameObject = (previousNearTouchingObject != null ? previousNearTouchingObject.gameObject : null);
            if (previousNearTouchingGameObject != null && nearTouchingObjects.Remove(previousNearTouchingGameObject))
            {
                OnInteractableObjectNearUntouched(SetInteractableObjectEvent(previousNearTouchingGameObject));
            }
        }

        /// <summary>
        /// The StartTouching method is called automatically when the Interactable Object is touched initially.
        /// </summary>
        /// <param name="currentTouchingObject">The interacting object that is currently touching this Interactable Object.</param>
        public virtual void StartTouching(VRTK_InteractTouch currentTouchingObject = null)
        {
            GameObject currentTouchingGameObject = (currentTouchingObject != null ? currentTouchingObject.gameObject : null);
            if (currentTouchingGameObject != null)
            {
                IgnoreColliders(currentTouchingGameObject);
                if (touchingObjects.Add(currentTouchingGameObject))
                {
                    ToggleEnableState(true);
                    OnInteractableObjectTouched(SetInteractableObjectEvent(currentTouchingGameObject));
                }
            }
        }

        /// <summary>
        /// The StopTouching method is called automatically when the Interactable Object has stopped being touched.
        /// </summary>
        /// <param name="previousTouchingObject">The interacting object that was previously touching this Interactable Object.</param>
        public virtual void StopTouching(VRTK_InteractTouch previousTouchingObject = null)
        {
            GameObject previousTouchingGameObject = (previousTouchingObject != null ? previousTouchingObject.gameObject : null);
            if (previousTouchingGameObject != null && touchingObjects.Remove(previousTouchingGameObject))
            {
                ResetUseState(previousTouchingGameObject);
                OnInteractableObjectUntouched(SetInteractableObjectEvent(previousTouchingGameObject));
            }
        }

        /// <summary>
        /// The Grabbed method is called automatically when the Interactable Object is grabbed initially.
        /// </summary>
        /// <param name="currentGrabbingObject">The interacting object that is currently grabbing this Interactable Object.</param>
        public virtual void Grabbed(VRTK_InteractGrab currentGrabbingObject = null)
        {
            GameObject currentGrabbingGameObject = (currentGrabbingObject != null ? currentGrabbingObject.gameObject : null);
            ToggleEnableState(true);
            if (!IsGrabbed() || IsSwappable())
            {
                PrimaryControllerGrab(currentGrabbingGameObject);
            }
            else
            {
                SecondaryControllerGrab(currentGrabbingGameObject);
            }
            OnInteractableObjectGrabbed(SetInteractableObjectEvent(currentGrabbingGameObject));
        }

        /// <summary>
        /// The Ungrabbed method is called automatically when the Interactable Object has stopped being grabbed.
        /// </summary>
        /// <param name="previousGrabbingObject">The interacting object that was previously grabbing this Interactable Object.</param>
        public virtual void Ungrabbed(VRTK_InteractGrab previousGrabbingObject = null)
        {
            GameObject previousGrabbingGameObject = (previousGrabbingObject != null ? previousGrabbingObject.gameObject : null);
            GameObject secondaryGrabbingObject = GetSecondaryGrabbingObject();
            if (secondaryGrabbingObject == null || secondaryGrabbingObject != previousGrabbingGameObject)
            {
                SecondaryControllerUngrab(secondaryGrabbingObject);
                PrimaryControllerUngrab(previousGrabbingGameObject, secondaryGrabbingObject);
            }
            else
            {
                SecondaryControllerUngrab(previousGrabbingGameObject);
            }
            OnInteractableObjectUngrabbed(SetInteractableObjectEvent(previousGrabbingGameObject));
        }

        /// <summary>
        /// The StartUsing method is called automatically when the Interactable Object is used initially.
        /// </summary>
        /// <param name="currentUsingObject">The interacting object that is currently using this Interactable Object.</param>
        public virtual void StartUsing(VRTK_InteractUse currentUsingObject = null)
        {
            GameObject currentUsingGameObject = (currentUsingObject != null ? currentUsingObject.gameObject : null);
            ToggleEnableState(true);
            if (IsUsing() && !IsUsing(currentUsingGameObject))
            {
                ResetUsingObject();
            }
            OnInteractableObjectUsed(SetInteractableObjectEvent(currentUsingGameObject));
            usingObject = currentUsingObject;
        }

        /// <summary>
        /// The StopUsing method is called automatically when the Interactable Object has stopped being used.
        /// </summary>
        /// <param name="previousUsingObject">The interacting object that was previously using this Interactable Object.</param>
        /// <param name="resetUsingObjectState">Resets the using object state to reset it's using action.</param>
        public virtual void StopUsing(VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true)
        {
            GameObject previousUsingGameObject = (previousUsingObject != null ? previousUsingObject.gameObject : null);
            OnInteractableObjectUnused(SetInteractableObjectEvent(previousUsingGameObject));
            if (resetUsingObjectState)
            {
                ResetUsingObject();
            }
            usingState = 0;
            usingObject = null;
        }

        /// <summary>
        /// The ToggleHighlight method is used to turn on or off the highlight of the Interactable Object.
        /// </summary>
        /// <param name="toggle">The state to determine whether to activate or deactivate the highlight. `true` will enable the highlight and `false` will remove the highlight.</param>
        [System.Obsolete("`VRTK_InteractableObject.ToggleHighlight` has been replaced with `VRTK_InteractableObject.Highlight` and `VRTK_InteractableObject.Unhighlight`. This method will be removed in a future version of VRTK.")]
        public virtual void ToggleHighlight(bool toggle, Color? highlightColor = null)
        {
            if (toggle)
            {
                Highlight((highlightColor != null ? (Color)highlightColor : Color.clear));
            }
            else
            {
                Unhighlight();
            }
        }

        /// <summary>
        /// The Highlight method turns on the highlighter attached to the Interactable Object with the given Color.
        /// </summary>
        /// <param name="highlightColor">The colour to apply to the highlighter.</param>
        [System.Obsolete("`VRTK_InteractableObject.Highlight` has been replaced with `VRTK_InteractObjectHighlighter.Highlight`. This method will be removed in a future version of VRTK.")]
        public virtual void Highlight(Color highlightColor)
        {
            VRTK_InteractObjectHighlighter interactObjectHighlighter = GetComponentInChildren<VRTK_InteractObjectHighlighter>();
            if (interactObjectHighlighter != null)
            {
                interactObjectHighlighter.Highlight(highlightColor);
            }
        }

        /// <summary>
        /// The Unhighlight method turns off the highlighter attached to the Interactable Object.
        /// </summary>
        [System.Obsolete("`VRTK_InteractableObject.Unhighlight` has been replaced with `VRTK_InteractObjectHighlighter.Unhighlight`. This method will be removed in a future version of VRTK.")]
        public virtual void Unhighlight()
        {
            VRTK_InteractObjectHighlighter interactObjectHighlighter = GetComponentInChildren<VRTK_InteractObjectHighlighter>();
            if (interactObjectHighlighter != null)
            {
                interactObjectHighlighter.Unhighlight();
            }
        }

        /// <summary>
        /// The ResetHighlighter method is used to reset the currently attached highlighter.
        /// </summary>
        [System.Obsolete("`VRTK_InteractableObject.ResetHighlighter` has been replaced with `VRTK_InteractObjectHighlighter.ResetHighlighter`. This method will be removed in a future version of VRTK.")]
        public virtual void ResetHighlighter()
        {
            VRTK_InteractObjectHighlighter interactObjectHighlighter = GetComponentInChildren<VRTK_InteractObjectHighlighter>();
            if (interactObjectHighlighter != null)
            {
                interactObjectHighlighter.ResetHighlighter();
            }
        }

        /// <summary>
        /// The PauseCollisions method temporarily pauses all collisions on the Interactable Object at grab time by removing the Interactable Object's Rigidbody's ability to detect collisions.
        /// </summary>
        /// <param name="delay">The time in seconds to pause the collisions for.</param>
        public virtual void PauseCollisions(float delay)
        {
            if (delay > 0f)
            {
                Rigidbody[] childRigidbodies = GetComponentsInChildren<Rigidbody>();
                for (int i = 0; i < childRigidbodies.Length; i++)
                {
                    childRigidbodies[i].detectCollisions = false;
                }
                Invoke("UnpauseCollisions", delay);
            }
        }

        /// <summary>
        /// The ZeroVelocity method resets the velocity and angular velocity to zero on the Rigidbody attached to the Interactable Object.
        /// </summary>
        public virtual void ZeroVelocity()
        {
            if (interactableRigidbody != null)
            {
                interactableRigidbody.velocity = Vector3.zero;
                interactableRigidbody.angularVelocity = Vector3.zero;
            }
        }

        /// <summary>
        /// The SaveCurrentState method stores the existing Interactable Object parent and the Rigidbody kinematic setting.
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

                if (interactableRigidbody != null)
                {
                    previousKinematicState = interactableRigidbody.isKinematic;
                }
            }
        }

        /// <summary>
        /// The GetNearTouchingObjects method is used to return the collecetion of valid GameObjects that are currently nearly touching this Interactable Object.
        /// </summary>
        /// <returns>A list of GameObject of that are currently nearly touching the current Interactable Object.</returns>
        public virtual List<GameObject> GetNearTouchingObjects()
        {
            return new List<GameObject>(nearTouchingObjects);
        }

        /// <summary>
        /// The GetTouchingObjects method is used to return the collecetion of valid GameObjects that are currently touching this Interactable Object.
        /// </summary>
        /// <returns>A list of GameObject of that are currently touching the current Interactable Object.</returns>
        public virtual List<GameObject> GetTouchingObjects()
        {
            return new List<GameObject>(touchingObjects);
        }

        /// <summary>
        /// The GetGrabbingObject method is used to return the GameObject that is currently grabbing this Interactable Object.
        /// </summary>
        /// <returns>The GameObject of what is grabbing the current Interactable Object.</returns>
        public virtual GameObject GetGrabbingObject()
        {
            return (IsGrabbed() ? grabbingObjects[0] : null);
        }

        /// <summary>
        /// The GetSecondaryGrabbingObject method is used to return the GameObject that is currently being used to influence this Interactable Object whilst it is being grabbed by a secondary influencing.
        /// </summary>
        /// <returns>The GameObject of the secondary influencing object of the current grabbed Interactable Object.</returns>
        public virtual GameObject GetSecondaryGrabbingObject()
        {
            return (grabbingObjects.Count > 1 ? grabbingObjects[1] : null);
        }

        /// <summary>
        /// The GetUsingObject method is used to return the GameObject that is currently using this Interactable Object.
        /// </summary>
        /// <returns>The GameObject of what is using the current Interactable Object.</returns>
        public virtual GameObject GetUsingObject()
        {
            return usingObject.gameObject;
        }

        /// <summary>
        /// The GetUsingScript method is used to return the Interact Use component that is currently using this Interactable Object.
        /// </summary>
        /// <returns>The Interact Use script of the interacting object that is using the current Interactable Object.</returns>
        public virtual VRTK_InteractUse GetUsingScript()
        {
            return usingObject;
        }

        /// <summary>
        /// The IsValidInteractableController method is used to check to see if a controller is allowed to perform an interaction with this Interactable Object as sometimes controllers are prohibited from grabbing or using an Interactable Object depedning on the use case.
        /// </summary>
        /// <param name="actualController">The GameObject of the controller that is being checked.</param>
        /// <param name="controllerCheck">The value of which controller is allowed to interact with this object.</param>
        /// <returns>Returns `true` if the interacting controller is allowed to grab the Interactable Object.</returns>
        public virtual bool IsValidInteractableController(GameObject actualController, AllowedController controllerCheck)
        {
            if (controllerCheck == AllowedController.Both)
            {
                return true;
            }

            SDK_BaseController.ControllerHand controllerHand = VRTK_DeviceFinder.GetControllerHandType(controllerCheck.ToString().Replace("Only", ""));
            return (VRTK_DeviceFinder.IsControllerOfHand(actualController, controllerHand));
        }

        /// <summary>
        /// The ForceStopInteracting method forces the Interactable Object to no longer be interacted with and will cause an interacting object to drop the Interactable Object and stop touching it.
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
        /// The ForceStopSecondaryGrabInteraction method forces the Interactable Object to no longer be influenced by the second controller grabbing it.
        /// </summary>
        public virtual void ForceStopSecondaryGrabInteraction()
        {
            GameObject grabbingObject = GetSecondaryGrabbingObject();
            if (grabbingObject != null)
            {
                grabbingObject.GetComponentInChildren<VRTK_InteractGrab>().ForceRelease();
            }
        }

        /// <summary>
        /// The RegisterTeleporters method is used to find all GameObjects that have a teleporter script and register the Interactable Object on the `OnTeleported` event.
        /// </summary>
        public virtual void RegisterTeleporters()
        {
            StartCoroutine(RegisterTeleportersAtEndOfFrame());
        }

        /// <summary>
        /// The UnregisterTeleporters method is used to unregister all teleporter events that are active on this Interactable Object.
        /// </summary>
        public virtual void UnregisterTeleporters()
        {
            for (int i = 0; i < VRTK_ObjectCache.registeredTeleporters.Count; i++)
            {
                VRTK_BasicTeleport teleporter = VRTK_ObjectCache.registeredTeleporters[i];
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
        /// The ToggleSnapDropZone method is used to set the state of whether the Interactable Object is in a Snap Drop Zone or not.
        /// </summary>
        /// <param name="snapDropZone">The Snap Drop Zone object that is being interacted with.</param>
        /// <param name="state">The state of whether the Interactable Object is fixed in or removed from the Snap Drop Zone. `true` denotes the Interactable Object is snapped to the Snap Drop Zone and `false` denotes it has been removed from the Snap Drop Zone.</param>
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
                if (interactableRigidbody != null)
                {
                    interactableRigidbody.WakeUp();
                }
                ResetDropSnapType();
                OnInteractableObjectUnsnappedFromDropZone(SetInteractableObjectEvent(snapDropZone.gameObject));
            }
        }

        /// <summary>
        /// The IsInSnapDropZone method determines whether the Interactable Object is currently snapped to a Snap Drop Zone.
        /// </summary>
        /// <returns>Returns `true` if the Interactable Object is currently snapped in a Snap Drop Zone, returns `false` if it is not.</returns>
        public virtual bool IsInSnapDropZone()
        {
            return snappedInSnapDropZone;
        }

        /// <summary>
        /// The SetSnapDropZoneHover method sets whether the Interactable Object is currently being hovered over a valid Snap Drop Zone.
        /// </summary>
        /// <param name="snapDropZone">The Snap Drop Zone that is being interacted with.</param>
        /// <param name="state">The state of whether the Interactable Object is being hovered or not.</param>
        public virtual void SetSnapDropZoneHover(VRTK_SnapDropZone snapDropZone, bool state)
        {
            if (state)
            {
                if (hoveredSnapObjects.Add(snapDropZone.gameObject))
                {
                    OnInteractableObjectEnteredSnapDropZone(SetInteractableObjectEvent(snapDropZone.gameObject));
                }
            }
            else
            {
                if (hoveredSnapObjects.Remove(snapDropZone.gameObject))
                {
                    OnInteractableObjectExitedSnapDropZone(SetInteractableObjectEvent(snapDropZone.gameObject));
                }
            }
            hoveredOverSnapDropZone = (hoveredSnapObjects.Count > 0);
        }

        /// <summary>
        /// The GetStoredSnapDropZone method returns the Snap Drop Zone that the Interactable Object is currently snapped to.
        /// </summary>
        /// <returns>The SnapDropZone that the Interactable Object is currently snapped to.</returns>
        public virtual VRTK_SnapDropZone GetStoredSnapDropZone()
        {
            return storedSnapDropZone;
        }

        /// <summary>
        /// The IsHoveredOverSnapDropZone method returns whether the Interactable Object is currently hovering over a Snap Drop Zone.
        /// </summary>
        /// <returns>Returns `true` if the Interactable Object is currently hovering over a Snap Drop Zone.</returns>
        public virtual bool IsHoveredOverSnapDropZone()
        {
            return hoveredOverSnapDropZone;
        }

        /// <summary>
        /// The IsDroppable method returns whether the Interactable Object can be dropped or not in it's current situation.
        /// </summary>
        /// <returns>Returns `true` if the Interactable Object can currently be dropped, returns `false` if it is not currently possible to drop.</returns>
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
        /// The IsSwappable method returns whether the Interactable Object can be grabbed with one interacting object and then swapped to another interacting object by grabbing with the secondary grab action.
        /// </summary>
        /// <returns>Returns `true` if the Interactable Object can be grabbed by a secondary interacting object whilst already being grabbed and the Interactable Object will swap controllers. Returns `false` if the Interactable Object cannot be swapped.</returns>
        public virtual bool IsSwappable()
        {
            return (secondaryGrabActionScript != null ? secondaryGrabActionScript.IsSwappable() : false);
        }

        /// <summary>
        /// The PerformSecondaryAction method returns whether the Interactable Object has a Secondary Grab Action that can be performed when grabbing the object with a secondary interacting object.
        /// </summary>
        /// <returns>Returns `true` if the Interactable Object has a Secondary Grab Action, returns `false` if it has no Secondary Grab Action or is swappable.</returns>
        public virtual bool PerformSecondaryAction()
        {
            return (GetGrabbingObject() != null && GetSecondaryGrabbingObject() == null && secondaryGrabActionScript != null ? secondaryGrabActionScript.IsActionable() : false);
        }

        /// <summary>
        /// The ResetIgnoredColliders method is used to clear any stored ignored colliders in case the `Ignored Colliders` array parameter is changed at runtime. This needs to be called manually if changes are made at runtime.
        /// </summary>
        public virtual void ResetIgnoredColliders()
        {
            //Go through all the existing set up ignored colliders and reset their collision state
            foreach (GameObject currentIgnoredCollider in new HashSet<GameObject>(currentIgnoredColliders))
            {
                if (currentIgnoredCollider != null)
                {
                    Collider[] touchingColliders = currentIgnoredCollider.GetComponentsInChildren<Collider>();
                    if (ignoredColliders != null)
                    {
                        for (int i = 0; i < ignoredColliders.Length; i++)
                        {
                            for (int j = 0; j < touchingColliders.Length; j++)
                            {
                                Physics.IgnoreCollision(touchingColliders[j], ignoredColliders[i], false);
                            }
                        }
                    }
                }
            }
            currentIgnoredColliders.Clear();
        }

        /// <summary>
        /// The SubscribeToInteractionEvent method subscribes a given method callback for the given Interaction Type.
        /// </summary>
        /// <param name="givenType">The Interaction Type to register the events for.</param>
        /// <param name="methodCallback">The method to execute when the Interaction Type is initiated.</param>
        public virtual void SubscribeToInteractionEvent(InteractionType givenType, InteractableObjectEventHandler methodCallback)
        {
            ManageInteractionEvent(givenType, true, methodCallback);
        }

        /// <summary>
        /// The UnsubscribeFromInteractionEvent method unsubscribes a previous event subscription for the given Interaction Type.
        /// </summary>
        /// <param name="givenType">The Interaction Type that the previous event subscription was under.</param>
        /// <param name="methodCallback">The method that was being executed when the Interaction Type was initiated.</param>
        public virtual void UnsubscribeFromInteractionEvent(InteractionType givenType, InteractableObjectEventHandler methodCallback)
        {
            ManageInteractionEvent(givenType, false, methodCallback);
        }

        /// <summary>
        /// The GetPrimaryAttachPoint returns the Transform that determines where the primary grabbing object is grabbing the Interactable Object at.
        /// </summary>
        /// <returns>A Transform that denotes where the primary grabbing object is grabbing the Interactable Object at.</returns>
        public virtual Transform GetPrimaryAttachPoint()
        {
            return primaryControllerAttachPoint;
        }

        /// <summary>
        /// The GetSecondaryAttachPoint returns the Transform that determines where the secondary grabbing object is grabbing the Interactable Object at.
        /// </summary>
        /// <returns>A Transform that denotes where the secondary grabbing object is grabbing the Interactable Object at.</returns>
        public virtual Transform GetSecondaryAttachPoint()
        {
            return secondaryControllerAttachPoint;
        }

        protected virtual void Awake()
        {
            interactableRigidbody = GetComponent<Rigidbody>();
            if (interactableRigidbody != null)
            {
                interactableRigidbody.maxAngularVelocity = float.MaxValue;
            }

            if (disableWhenIdle && enabled && IsIdle())
            {
                startDisabled = true;
                enabled = false;
            }

            ///[Obsolete]
#pragma warning disable 0618
            if (touchHighlightColor != Color.clear && !GetComponent<VRTK_InteractObjectHighlighter>())
            {
                VRTK_InteractObjectHighlighter autoGenInteractHighlighter = gameObject.AddComponent<VRTK_InteractObjectHighlighter>();
                autoGenInteractHighlighter.touchHighlight = touchHighlightColor;
                autoGenInteractHighlighter.objectHighlighter = (objectHighlighter == null ? Highlighters.VRTK_BaseHighlighter.GetActiveHighlighter(gameObject) : objectHighlighter);
            }
#pragma warning restore 0618
        }

        protected virtual void OnEnable()
        {
            RegisterTeleporters();
            forceDisabled = false;
            if (forcedDropped)
            {
                LoadPreviousState();
            }
            forcedDropped = false;
            startDisabled = false;
            OnInteractableObjectEnabled(SetInteractableObjectEvent(null));
        }

        protected virtual void OnDisable()
        {
            UnregisterTeleporters();
            if (!startDisabled)
            {
                forceDisabled = true;
                ForceStopInteracting();
            }
            OnInteractableObjectDisabled(SetInteractableObjectEvent(null));
        }

        protected virtual void FixedUpdate()
        {
            if (trackPoint != null && grabAttachMechanicScript != null)
            {
                grabAttachMechanicScript.ProcessFixedUpdate();
            }

            if (secondaryGrabActionScript != null)
            {
                secondaryGrabActionScript.ProcessFixedUpdate();
            }
        }

        protected virtual void Update()
        {
            AttemptSetGrabMechanic();
            AttemptSetSecondaryGrabAction();

            if (trackPoint != null && grabAttachMechanicScript != null)
            {
                grabAttachMechanicScript.ProcessUpdate();
            }

            if (secondaryGrabActionScript != null)
            {
                secondaryGrabActionScript.ProcessUpdate();
            }
        }

        /// <summary>
        /// determines if this object is currently idle
        /// used to determine whether or not the script
        /// can be disabled for now
        /// </summary>
        /// <returns>whether or not the script is currently idle</returns>
        protected virtual bool IsIdle()
        {
            return !IsNearTouched() && !IsTouched() && !IsGrabbed() && !IsUsing();
        }

        protected virtual void LateUpdate()
        {
            if (disableWhenIdle && IsIdle())
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
            if (interactableRigidbody != null)
            {
                interactableRigidbody.isKinematic = previousKinematicState;
            }
            if (!IsSwappable())
            {
                isGrabbable = previousIsGrabbable;
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
                VRTK_BaseGrabAttach setGrabMechanic = GetComponent<VRTK_BaseGrabAttach>();
                if (setGrabMechanic == null)
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
            GameObject grabbingObject = GetGrabbingObject();
            if (grabbingObject != null)
            {
                grabbingObject.GetComponentInChildren<VRTK_InteractGrab>().ForceRelease();
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
            VRTK_SharedMethods.AddListValue(grabbingObjects, currentGrabbingObject, true);
            SetTrackPoint(currentGrabbingObject);
            if (!IsSwappable())
            {
                previousIsGrabbable = isGrabbable;
                isGrabbable = false;
            }
        }

        protected virtual void SecondaryControllerGrab(GameObject currentGrabbingObject)
        {
            if (VRTK_SharedMethods.AddListValue(grabbingObjects, currentGrabbingObject, true))
            {
                secondaryControllerAttachPoint = CreateAttachPoint(currentGrabbingObject.name, "Secondary", currentGrabbingObject.transform);

                if (secondaryGrabActionScript != null)
                {
                    secondaryGrabActionScript.Initialise(this, GetGrabbingObject().GetComponentInChildren<VRTK_InteractGrab>(), GetSecondaryGrabbingObject().GetComponentInChildren<VRTK_InteractGrab>(), primaryControllerAttachPoint, secondaryControllerAttachPoint);
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
                previousSecondaryGrabbingObject.GetComponentInChildren<VRTK_InteractGrab>().ForceRelease();
            }
            LoadPreviousState();
        }

        protected virtual void SecondaryControllerUngrab(GameObject previousGrabbingObject)
        {
            if (grabbingObjects.Remove(previousGrabbingObject))
            {
                Destroy(secondaryControllerAttachPoint.gameObject);
                secondaryControllerAttachPoint = null;
                if (secondaryGrabActionScript != null)
                {
                    secondaryGrabActionScript.ResetAction();
                }
            }
        }

        protected virtual void UnpauseCollisions()
        {
            Rigidbody[] childRigidbodies = GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < childRigidbodies.Length; i++)
            {
                childRigidbodies[i].detectCollisions = true;
            }
        }

        protected virtual void SetTrackPoint(GameObject currentGrabbingObject)
        {
            AddTrackPoint(currentGrabbingObject);
            primaryControllerAttachPoint = CreateAttachPoint(GetGrabbingObject().name, "Original", trackPoint);

            if (grabAttachMechanicScript != null)
            {
                grabAttachMechanicScript.SetTrackPoint(trackPoint);
                grabAttachMechanicScript.SetInitialAttachPoint(primaryControllerAttachPoint);
            }
        }

        protected virtual Transform CreateAttachPoint(string namePrefix, string nameSuffix, Transform origin)
        {
            Transform attachPoint = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, namePrefix, nameSuffix, "Controller", "AttachPoint")).transform;
            attachPoint.SetParent(transform);
            attachPoint.position = origin.position;
            attachPoint.rotation = origin.rotation;
            attachPoint.localScale = Vector3.one;
            return attachPoint;
        }

        protected virtual void AddTrackPoint(GameObject currentGrabbingObject)
        {
            VRTK_InteractGrab grabScript = currentGrabbingObject.GetComponentInChildren<VRTK_InteractGrab>();
            Transform controllerPoint = ((grabScript && grabScript.controllerAttachPoint) ? grabScript.controllerAttachPoint.transform : currentGrabbingObject.transform);

            if (grabAttachMechanicScript != null)
            {
                trackPoint = grabAttachMechanicScript.CreateTrackPoint(controllerPoint, gameObject, currentGrabbingObject, ref customTrackPoint);
            }
        }

        protected virtual void RemoveTrackPoint()
        {
            if (customTrackPoint && trackPoint != null)
            {
                Destroy(trackPoint.gameObject);
            }
            else
            {
                trackPoint = null;
            }

            if (primaryControllerAttachPoint != null)
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
            if (grabAttachMechanicScript != null && grabAttachMechanicScript.IsTracked() && stayGrabbedOnTeleport && trackPoint != null)
            {
                GameObject actualController = VRTK_DeviceFinder.GetActualController(GetGrabbingObject());
                transform.position = (actualController ? actualController.transform.position : transform.position);
            }
        }

        protected virtual IEnumerator RegisterTeleportersAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < VRTK_ObjectCache.registeredTeleporters.Count; i++)
            {
                VRTK_BasicTeleport teleporter = VRTK_ObjectCache.registeredTeleporters[i];
                teleporter.Teleporting += new TeleportEventHandler(OnTeleporting);
                teleporter.Teleported += new TeleportEventHandler(OnTeleported);
            }
        }

        protected virtual void ResetUseState(GameObject checkObject)
        {
            if (checkObject != null)
            {
                VRTK_InteractUse usingObjectCheck = checkObject.GetComponentInChildren<VRTK_InteractUse>();
                if (usingObjectCheck != null)
                {
                    if (holdButtonToUse)
                    {
                        usingObjectCheck.ForceStopUsing();
                    }
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
            foreach (GameObject touchingObject in new HashSet<GameObject>(touchingObjects))
            {
                if (touchingObject.activeInHierarchy || forceDisabled)
                {
                    touchingObject.GetComponentInChildren<VRTK_InteractTouch>().ForceStopTouching();
                }
            }
        }

        protected virtual void StopGrabbingInteractions()
        {
            GameObject grabbingObject = GetGrabbingObject();

            if (grabbingObject != null && (grabbingObject.activeInHierarchy || forceDisabled))
            {
                VRTK_InteractGrab grabbingObjectScript = grabbingObject.GetComponentInChildren<VRTK_InteractGrab>();
                if (grabbingObjectScript != null && grabbingObjectScript.interactTouch != null)
                {
                    grabbingObjectScript.interactTouch.ForceStopTouching();
                    grabbingObjectScript.ForceRelease();
                    forcedDropped = true;
                }
            }
        }

        protected virtual void StopUsingInteractions()
        {
            if (usingObject != null && usingObject.interactTouch != null && (usingObject.gameObject.activeInHierarchy || forceDisabled))
            {
                usingObject.interactTouch.ForceStopTouching();
                usingObject.ForceStopUsing();
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
                    Joint snapDropZoneJoint = storedSnapDropZone.GetComponent<Joint>();
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
            if (usingObject != null)
            {
                usingObject.ForceResetUsing();
            }
        }

        protected virtual void ManageInteractionEvent(InteractionType givenType, bool state, InteractableObjectEventHandler methodCallback)
        {
            switch (givenType)
            {
                case InteractionType.NearTouch:
                    ManageNearTouchSubscriptions(state, methodCallback);
                    break;
                case InteractionType.Touch:
                    ManageTouchSubscriptions(state, methodCallback);
                    break;
                case InteractionType.Grab:
                    ManageGrabSubscriptions(state, methodCallback);
                    break;
                case InteractionType.Use:
                    ManageUseSubscriptions(state, methodCallback);
                    break;
                case InteractionType.NearUntouch:
                    ManageNearUntouchSubscriptions(state, methodCallback);
                    break;
                case InteractionType.Untouch:
                    ManageUntouchSubscriptions(state, methodCallback);
                    break;
                case InteractionType.Ungrab:
                    ManageUngrabSubscriptions(state, methodCallback);
                    break;
                case InteractionType.Unuse:
                    ManageUnuseSubscriptions(state, methodCallback);
                    break;
            }
        }

        protected virtual void ManageNearTouchSubscriptions(bool register, InteractableObjectEventHandler methodCallback)
        {
            if (!register)
            {
                InteractableObjectNearTouched -= methodCallback;
            }

            if (register)
            {
                InteractableObjectNearTouched += methodCallback;
            }
        }

        protected virtual void ManageTouchSubscriptions(bool register, InteractableObjectEventHandler methodCallback)
        {
            if (!register)
            {
                InteractableObjectTouched -= methodCallback;
            }

            if (register)
            {
                InteractableObjectTouched += methodCallback;
            }
        }

        protected virtual void ManageGrabSubscriptions(bool register, InteractableObjectEventHandler methodCallback)
        {
            if (!register)
            {
                InteractableObjectGrabbed -= methodCallback;
            }

            if (register)
            {
                InteractableObjectGrabbed += methodCallback;
            }
        }

        protected virtual void ManageUseSubscriptions(bool register, InteractableObjectEventHandler methodCallback)
        {
            if (!register)
            {
                InteractableObjectUsed -= methodCallback;
            }

            if (register)
            {
                InteractableObjectUsed += methodCallback;
            }
        }

        protected virtual void ManageNearUntouchSubscriptions(bool register, InteractableObjectEventHandler methodCallback)
        {
            if (!register)
            {
                InteractableObjectNearUntouched -= methodCallback;
            }

            if (register)
            {
                InteractableObjectNearUntouched += methodCallback;
            }
        }

        protected virtual void ManageUntouchSubscriptions(bool register, InteractableObjectEventHandler methodCallback)
        {
            if (!register)
            {
                InteractableObjectUntouched -= methodCallback;
            }

            if (register)
            {
                InteractableObjectUntouched += methodCallback;
            }
        }

        protected virtual void ManageUngrabSubscriptions(bool register, InteractableObjectEventHandler methodCallback)
        {
            if (!register)
            {
                InteractableObjectUngrabbed -= methodCallback;
            }

            if (register)
            {
                InteractableObjectUngrabbed += methodCallback;
            }
        }

        protected virtual void ManageUnuseSubscriptions(bool register, InteractableObjectEventHandler methodCallback)
        {
            if (!register)
            {
                InteractableObjectUnused -= methodCallback;
            }

            if (register)
            {
                InteractableObjectUnused += methodCallback;
            }
        }
    }
}