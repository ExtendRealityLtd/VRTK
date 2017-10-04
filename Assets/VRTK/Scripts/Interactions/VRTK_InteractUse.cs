// Interact Use|Interactions|30070
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Interact Use script is attached to a Controller object and requires the `VRTK_ControllerEvents` script to be attached as it uses this for listening to the controller button events for using and stop using interactable game objects.
    /// </summary>
    /// <remarks>
    /// It listens for the `AliasUseOn` and `AliasUseOff` events to determine when an object should be used and should stop using.
    ///
    /// The Controller object also requires the `VRTK_InteractTouch` script to be attached to it as this is used to determine when an interactable object is being touched. Only valid touched objects can be used.
    ///
    /// An object can be used if the Controller touches a game object which contains the `VRTK_InteractableObject` script and has the flag `isUsable` set to `true`.
    ///
    /// If a valid interactable object is usable then pressing the set `Use` button on the Controller (default is `Trigger`) will call the `StartUsing` method on the touched interactable object.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/006_Controller_UsingADoor` simulates using a door object to open and close it. It also has a cube on the floor that can be grabbed to show how interactable objects can be usable or grabbable.
    ///
    /// `VRTK/Examples/008_Controller_UsingAGrabbedObject` shows that objects can be grabbed with one button and used with another (e.g. firing a gun).
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/VRTK_InteractUse")]
    public class VRTK_InteractUse : MonoBehaviour
    {
        [Header("Use Settings")]

        [Tooltip("The button used to use/unuse a touched object.")]
        public VRTK_ControllerEvents.ButtonAlias useButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;

        [Header("Custom Settings")]

        [Tooltip("The controller to listen for the events on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_ControllerEvents controllerEvents;
        [Tooltip("The Interact Touch to listen for touches on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_InteractTouch interactTouch;
        [Tooltip("The Interact Grab to listen for grab actions on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_InteractGrab interactGrab;

        /// <summary>
        /// Emitted when the use toggle alias button is pressed.
        /// </summary>
        public event ControllerInteractionEventHandler UseButtonPressed;
        /// <summary>
        /// Emitted when the use toggle alias button is released.
        /// </summary>
        public event ControllerInteractionEventHandler UseButtonReleased;

        /// <summary>
        /// Emitted when a use of a valid object is started.
        /// </summary>
        public event ObjectInteractEventHandler ControllerStartUseInteractableObject;
        /// <summary>
        /// Emitted when a valid object starts being used.
        /// </summary>
        public event ObjectInteractEventHandler ControllerUseInteractableObject;
        /// <summary>
        /// Emitted when a unuse of a valid object is started.
        /// </summary>
        public event ObjectInteractEventHandler ControllerStartUnuseInteractableObject;
        /// <summary>
        /// Emitted when a valid object stops being used.
        /// </summary>
        public event ObjectInteractEventHandler ControllerUnuseInteractableObject;

        protected VRTK_ControllerEvents.ButtonAlias subscribedUseButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        protected VRTK_ControllerEvents.ButtonAlias savedUseButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        protected bool usePressed;
        protected VRTK_ControllerReference controllerReference
        {
            get
            {
                return VRTK_ControllerReference.GetControllerReference((interactTouch != null ? interactTouch.gameObject : null));
            }
        }

        protected GameObject usingObject = null;

        public virtual void OnControllerStartUseInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerStartUseInteractableObject != null)
            {
                ControllerStartUseInteractableObject(this, e);
            }
        }

        public virtual void OnControllerUseInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerUseInteractableObject != null)
            {
                ControllerUseInteractableObject(this, e);
            }
        }

        public virtual void OnControllerStartUnuseInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerStartUnuseInteractableObject != null)
            {
                ControllerStartUnuseInteractableObject(this, e);
            }
        }

        public virtual void OnControllerUnuseInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerUnuseInteractableObject != null)
            {
                ControllerUnuseInteractableObject(this, e);
            }
        }

        public virtual void OnUseButtonPressed(ControllerInteractionEventArgs e)
        {
            if (UseButtonPressed != null)
            {
                UseButtonPressed(this, e);
            }
        }

        public virtual void OnUseButtonReleased(ControllerInteractionEventArgs e)
        {
            if (UseButtonReleased != null)
            {
                UseButtonReleased(this, e);
            }
        }

        /// <summary>
        /// The IsUsebuttonPressed method determines whether the current use alias button is being pressed down.
        /// </summary>
        /// <returns>Returns true if the use alias button is being held down.</returns>
        public virtual bool IsUseButtonPressed()
        {
            return usePressed;
        }

        /// <summary>
        /// The GetUsingObject method returns the current object being used by the controller.
        /// </summary>
        /// <returns>The game object of what is currently being used by this controller.</returns>
        public virtual GameObject GetUsingObject()
        {
            return usingObject;
        }

        /// <summary>
        /// The ForceStopUsing method will force the controller to stop using the currently touched object and will also stop the object's using action.
        /// </summary>
        public virtual void ForceStopUsing()
        {
            if (usingObject != null)
            {
                StopUsing();
            }
        }

        /// <summary>
        /// The ForceResetUsing will force the controller to stop using the currently touched object but the object will continue with it's existing using action.
        /// </summary>
        public virtual void ForceResetUsing()
        {
            if (usingObject != null)
            {
                UnuseInteractedObject(false);
            }
        }

        /// <summary>
        /// The AttemptUse method will attempt to use the currently touched object without needing to press the use button on the controller.
        /// </summary>
        public virtual void AttemptUse()
        {
            AttemptUseObject();
        }

        protected virtual void OnEnable()
        {
            controllerEvents = (controllerEvents != null ? controllerEvents : GetComponentInParent<VRTK_ControllerEvents>());
            interactTouch = (interactTouch != null ? interactTouch : GetComponentInParent<VRTK_InteractTouch>());
            interactGrab = (interactGrab != null ? interactGrab : GetComponentInParent<VRTK_InteractGrab>());

            if (interactTouch == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_InteractUse", "VRTK_InteractTouch", "interactTouch", "the same or parent"));
            }

            ManageUseListener(true);
            ManageInteractTouchListener(true);
        }

        protected virtual void OnDisable()
        {
            ForceResetUsing();
            ManageUseListener(false);
            ManageInteractTouchListener(false);
        }

        protected virtual void Update()
        {
            ManageUseListener(true);
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
                if (touchedObjectScript != null && touchedObjectScript.useOverrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    savedUseButton = subscribedUseButton;
                    useButton = touchedObjectScript.useOverrideButton;
                    ManageUseListener(true);
                }
            }
        }

        protected virtual void ControllerUntouchInteractableObject(object sender, ObjectInteractEventArgs e)
        {
            if (e.target != null)
            {
                VRTK_InteractableObject touchedObjectScript = e.target.GetComponent<VRTK_InteractableObject>();
                if (touchedObjectScript != null && !touchedObjectScript.IsUsing() && savedUseButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    useButton = savedUseButton;
                    savedUseButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
                    ManageUseListener(true);
                }
            }
        }

        protected virtual void ManageUseListener(bool state)
        {
            if (controllerEvents != null && subscribedUseButton != VRTK_ControllerEvents.ButtonAlias.Undefined && (!state || useButton != subscribedUseButton))
            {
                controllerEvents.UnsubscribeToButtonAliasEvent(subscribedUseButton, true, DoStartUseObject);
                controllerEvents.UnsubscribeToButtonAliasEvent(subscribedUseButton, false, DoStopUseObject);
                subscribedUseButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
            }

            if (controllerEvents != null && state && useButton != VRTK_ControllerEvents.ButtonAlias.Undefined && useButton != subscribedUseButton)
            {
                controllerEvents.SubscribeToButtonAliasEvent(useButton, true, DoStartUseObject);
                controllerEvents.SubscribeToButtonAliasEvent(useButton, false, DoStopUseObject);
                subscribedUseButton = useButton;
            }
        }

        protected virtual bool IsObjectUsable(GameObject obj)
        {
            VRTK_InteractableObject objScript = (obj != null ? obj.GetComponent<VRTK_InteractableObject>() : null);
            return (obj != null && interactTouch != null && interactTouch.IsObjectInteractable(obj) && objScript != null && objScript.isUsable);
        }

        protected virtual bool IsObjectHoldOnUse(GameObject obj)
        {
            if (obj != null)
            {
                VRTK_InteractableObject objScript = obj.GetComponent<VRTK_InteractableObject>();
                return (objScript != null && objScript.holdButtonToUse);
            }
            return false;
        }

        protected virtual int GetObjectUsingState(GameObject obj)
        {
            if (obj != null)
            {
                VRTK_InteractableObject objScript = obj.GetComponent<VRTK_InteractableObject>();
                if (objScript != null)
                {
                    return objScript.usingState;
                }
            }
            return 0;
        }

        protected virtual void SetObjectUsingState(GameObject obj, int value)
        {
            if (obj != null)
            {
                VRTK_InteractableObject objScript = obj.GetComponent<VRTK_InteractableObject>();
                if (objScript != null)
                {
                    objScript.usingState = value;
                }
            }
        }

        protected virtual void AttemptHaptics()
        {
            if (usingObject != null)
            {
                VRTK_InteractHaptics doHaptics = usingObject.GetComponentInParent<VRTK_InteractHaptics>();
                if (doHaptics != null)
                {
                    doHaptics.HapticsOnUse(controllerReference);
                }
            }
        }

        protected virtual void ToggleControllerVisibility(bool visible)
        {
            if (usingObject != null)
            {
                VRTK_InteractControllerAppearance[] controllerAppearanceScript = usingObject.GetComponentsInParent<VRTK_InteractControllerAppearance>(true);
                if (controllerAppearanceScript.Length > 0)
                {
                    controllerAppearanceScript[0].ToggleControllerOnUse(visible, controllerReference.model, usingObject);
                }
            }
        }

        protected virtual void UseInteractedObject(GameObject touchedObject)
        {
            if ((usingObject == null || usingObject != touchedObject) && IsObjectUsable(touchedObject) && interactTouch != null)
            {
                usingObject = touchedObject;
                OnControllerStartUseInteractableObject(interactTouch.SetControllerInteractEvent(usingObject));
                VRTK_InteractableObject usingObjectScript = (usingObject != null ? usingObject.GetComponent<VRTK_InteractableObject>() : null);

                if (usingObjectScript != null)
                {
                    if (!usingObjectScript.IsValidInteractableController(gameObject, usingObjectScript.allowedUseControllers))
                    {
                        usingObject = null;
                        return;
                    }

                    usingObjectScript.StartUsing(this);
                    ToggleControllerVisibility(false);
                    AttemptHaptics();
                    OnControllerUseInteractableObject(interactTouch.SetControllerInteractEvent(usingObject));
                }
            }
        }

        protected virtual void UnuseInteractedObject(bool completeStop)
        {
            if (usingObject != null && interactTouch != null)
            {
                OnControllerStartUnuseInteractableObject(interactTouch.SetControllerInteractEvent(usingObject));
                VRTK_InteractableObject usingObjectCheck = usingObject.GetComponent<VRTK_InteractableObject>();
                if (usingObjectCheck != null && completeStop)
                {
                    usingObjectCheck.StopUsing(this);
                }
                ToggleControllerVisibility(true);
                OnControllerUnuseInteractableObject(interactTouch.SetControllerInteractEvent(usingObject));
                usingObject = null;
            }
        }

        protected virtual GameObject GetFromGrab()
        {
            if (interactGrab != null)
            {
                return interactGrab.GetGrabbedObject();
            }
            return null;
        }

        protected virtual void StopUsing()
        {
            SetObjectUsingState(usingObject, 0);
            UnuseInteractedObject(true);
        }

        protected virtual void AttemptUseObject()
        {
            GameObject touchedObject = (interactTouch != null ? interactTouch.GetTouchedObject() : null);
            if (touchedObject == null)
            {
                touchedObject = GetFromGrab();
            }

            if (touchedObject != null && interactTouch != null && interactTouch.IsObjectInteractable(touchedObject))
            {
                VRTK_InteractableObject interactableObjectScript = touchedObject.GetComponent<VRTK_InteractableObject>();

                if (interactableObjectScript != null && interactableObjectScript.useOnlyIfGrabbed && !interactableObjectScript.IsGrabbed())
                {
                    return;
                }

                UseInteractedObject(touchedObject);
                if (usingObject != null && !IsObjectHoldOnUse(usingObject))
                {
                    SetObjectUsingState(usingObject, GetObjectUsingState(usingObject) + 1);
                }
            }
        }

        protected virtual void DoStartUseObject(object sender, ControllerInteractionEventArgs e)
        {
            OnUseButtonPressed(controllerEvents.SetControllerEvent(ref usePressed, true));
            AttemptUseObject();
        }

        protected virtual void DoStopUseObject(object sender, ControllerInteractionEventArgs e)
        {
            if (IsObjectHoldOnUse(usingObject) || GetObjectUsingState(usingObject) >= 2)
            {
                StopUsing();
            }
            OnUseButtonReleased(controllerEvents.SetControllerEvent(ref usePressed, false));
        }
    }
}