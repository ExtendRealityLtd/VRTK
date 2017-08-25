// Interact Object Appearance|Interactions|30040
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="affectingObject">The object that is being affected.</param>
    /// <param name="monitoringObject">The interactable object that is being monitored.</param>
    /// <param name="interactionType">The type of interaction initiating the event.</param>
    public struct InteractObjectAppearanceEventArgs
    {
        public GameObject affectingObject;
        public GameObject objectToIgnore;
        public VRTK_InteractableObject monitoringObject;
        public VRTK_InteractObjectAppearance.InteractionType interactionType;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="InteractObjectAppearanceEventArgs"/></param>
    public delegate void InteractObjectAppearanceEventHandler(object sender, InteractObjectAppearanceEventArgs e);

    /// <summary>
    /// The Interact Object Appearance script is used to determine whether the `Object To Affect` should be visible or hidden by default or on touch, grab or use.
    /// </summary>
    /// <remarks>
    /// The `Object To Affect` can be the object that is causing the interaction (touch/grab/use) and is usually the controller. So this script can be used to hide the controller on interaction.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/008_Controller_UsingAGrabbedObject` shows that the controller can be hidden when touching, grabbing and using an object.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/VRTK_InteractObjectAppearance")]
    public class VRTK_InteractObjectAppearance : MonoBehaviour
    {
        /// <summary>
        /// The interaction type.
        /// </summary>
        /// <param name="None">No interaction has affected the object appearance.</param>
        /// <param name="Touch">The touch interaction has affected the object appearance.</param>
        /// <param name="Untouch">The untouch interaction has affected the object appearance.</param>
        /// <param name="Grab">The grab interaction has affected the object appearance.</param>
        /// <param name="Ungrab">The ungrab interaction has affected the object appearance.</param>
        /// <param name="Use">The use interaction has affected the object appearance.</param>
        /// <param name="Unuse">The unuse interaction has affected the object appearance.</param>
        public enum InteractionType
        {
            None,
            Touch,
            Untouch,
            Grab,
            Ungrab,
            Use,
            Unuse
        }

        /// <summary>
        /// The valid interacting object.
        /// </summary>
        /// <param name="Anything">Any GameObject is considered a valid interacting object.</param>
        /// <param name="EitherController">Only a game controller is considered a valid interacting objcet.</param>
        /// <param name="NeitherController">Any GameObject except a game controller is considered a valid interacting object.</param>
        /// <param name="LeftControllerOnly">Only the left game controller is considered a valid interacting objcet.</param>
        /// <param name="RightControllerOnly">Only the right game controller is considered a valid interacting objcet.</param>
        public enum ValidInteractingObject
        {
            Anything,
            EitherController,
            NeitherController,
            LeftControllerOnly,
            RightControllerOnly
        }

        [Header("General Settings")]

        [Tooltip("The GameObject to affect the appearance of. If this is null then then the interacting object will be used (usually the controller).")]
        public GameObject objectToAffect;
        [SerializeField]
        [Tooltip("The Interactable Object to monitor for the touch/grab/use event.")]
        protected VRTK_InteractableObject objectToMonitor;

        [Header("Default Appearance Settings")]

        [Tooltip("If this is checked then the `Object To Affect` will be an active GameObject when the script is enabled. If it's unchecked then it will be disabled. This only takes effect if `Affect Interacting Object` is unticked.")]
        public bool gameObjectActiveByDefault = true;
        [Tooltip("If this is checked then the `Object To Affect` will have visible renderers when the script is enabled. If it's unchecked then it will have it's renderers disabled. This only takes effect if `Affect Interacting Object` is unticked.")]
        public bool rendererVisibleByDefault = true;

        [Header("Touch Appearance Settings")]

        [Tooltip("If this is checked then the `Object To Affect` will be an active GameObject when the `Object To Monitor` is touched. If it's unchecked then it will be disabled on touch.")]
        public bool gameObjectActiveOnTouch = true;
        [Tooltip("If this is checked then the `Object To Affect` will have visible renderers when the `Object To Monitor` is touched. If it's unchecked then it will have it's renderers disabled on touch.")]
        public bool rendererVisibleOnTouch = true;
        [Tooltip("The amount of time to wait before the touch appearance settings are applied after the touch event.")]
        public float touchAppearanceDelay = 0f;
        [Tooltip("The amount of time to wait before the previous appearance settings are applied after the untouch event.")]
        public float untouchAppearanceDelay = 0f;
        [Tooltip("Determines what type of interacting object will affect the appearance of the `Object To Affect` after the touch/untouch event.")]
        public ValidInteractingObject validTouchInteractingObject = ValidInteractingObject.Anything;

        [Header("Grab Appearance Settings")]

        [Tooltip("If this is checked then the `Object To Affect` will be an active GameObject when the `Object To Monitor` is grabbed. If it's unchecked then it will be disabled on grab.")]
        public bool gameObjectActiveOnGrab = true;
        [Tooltip("If this is checked then the `Object To Affect` will have visible renderers when the `Object To Monitor` is grabbed. If it's unchecked then it will have it's renderers disabled on grab.")]
        public bool rendererVisibleOnGrab = true;
        [Tooltip("The amount of time to wait before the grab appearance settings are applied after the grab event.")]
        public float grabAppearanceDelay = 0f;
        [Tooltip("The amount of time to wait before the previous appearance settings are applied after the ungrab event.")]
        public float ungrabAppearanceDelay = 0f;
        [Tooltip("Determines what type of interacting object will affect the appearance of the `Object To Affect` after the grab/ungrab event.")]
        public ValidInteractingObject validGrabInteractingObject = ValidInteractingObject.Anything;

        [Header("Use Appearance Settings")]

        [Tooltip("If this is checked then the `Object To Affect` will be an active GameObject when the `Object To Monitor` is used. If it's unchecked then it will be disabled on use.")]
        public bool gameObjectActiveOnUse = true;
        [Tooltip("If this is checked then the `Object To Affect` will have visible renderers when the `Object To Monitor` is used. If it's unchecked then it will have it's renderers disabled on use.")]
        public bool rendererVisibleOnUse = true;
        [Tooltip("The amount of time to wait before the use appearance settings are applied after the use event.")]
        public float useAppearanceDelay = 0f;
        [Tooltip("The amount of time to wait before the previous appearance settings are applied after the unuse event.")]
        public float unuseAppearanceDelay = 0f;
        [Tooltip("Determines what type of interacting object will affect the appearance of the `Object To Affect` after the use/unuse event.")]
        public ValidInteractingObject validUseInteractingObject = ValidInteractingObject.Anything;

        /// <summary>
        /// Emitted when the GameObject on the `Object To Affect` is enabled.
        /// </summary>
        public event InteractObjectAppearanceEventHandler GameObjectEnabled;
        /// <summary>
        /// Emitted when the GameObject on the `Object To Affect` is disabled.
        /// </summary>
        public event InteractObjectAppearanceEventHandler GameObjectDisabled;
        /// <summary>
        /// Emitted when the Renderers on the `Object To Affect` are enabled.
        /// </summary>
        public event InteractObjectAppearanceEventHandler RenderersEnabled;
        /// <summary>
        /// Emitted when the Renderers on the `Object To Affect` are disabled.
        /// </summary>
        public event InteractObjectAppearanceEventHandler RenderersDisabled;

        protected GameObject touchAffectedObject;
        protected GameObject grabAffectedObject;
        protected GameObject useAffectedObject;
        protected Coroutine touchRoutine;
        protected Coroutine grabRoutine;
        protected Coroutine useRoutine;
        protected bool currentRenderState;
        protected bool currentGameObjectState;
        protected bool currentStatesSet;

        public virtual void OnGameObjectEnabled(InteractObjectAppearanceEventArgs e)
        {
            if (GameObjectEnabled != null)
            {
                GameObjectEnabled(this, e);
            }
        }

        public virtual void OnGameObjectDisabled(InteractObjectAppearanceEventArgs e)
        {
            if (GameObjectDisabled != null)
            {
                GameObjectDisabled(this, e);
            }
        }

        public virtual void OnRenderersEnabled(InteractObjectAppearanceEventArgs e)
        {
            if (RenderersEnabled != null)
            {
                RenderersEnabled(this, e);
            }
        }

        public virtual void OnRenderersDisabled(InteractObjectAppearanceEventArgs e)
        {
            if (RenderersDisabled != null)
            {
                RenderersDisabled(this, e);
            }
        }

        protected virtual void OnEnable()
        {
            currentStatesSet = false;
            objectToMonitor = (objectToMonitor == null ? GetComponentInParent<VRTK_InteractableObject>() : objectToMonitor);

            if (objectToMonitor != null)
            {
                objectToMonitor.InteractableObjectDisabled += InteractableObjectDisabled;
                objectToMonitor.InteractableObjectTouched += InteractableObjectTouched;
                objectToMonitor.InteractableObjectUntouched += InteractableObjectUntouched;
                objectToMonitor.InteractableObjectGrabbed += InteractableObjectGrabbed;
                objectToMonitor.InteractableObjectUngrabbed += InteractableObjectUngrabbed;
                objectToMonitor.InteractableObjectUsed += InteractableObjectUsed;
                objectToMonitor.InteractableObjectUnused += InteractableObjectUnused;
            }
            else
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_InteractObjectAppearance", "VRTK_InteractableObject", "objectToMonitor", "current or parent"));
            }

            if (objectToAffect != null)
            {
                ToggleState(objectToAffect, gameObjectActiveByDefault, rendererVisibleByDefault, InteractionType.None);
            }
        }

        protected virtual void OnDisable()
        {
            if (objectToMonitor != null)
            {
                RestoreDefaults();
                objectToMonitor.InteractableObjectDisabled -= InteractableObjectDisabled;
                objectToMonitor.InteractableObjectTouched -= InteractableObjectTouched;
                objectToMonitor.InteractableObjectUntouched -= InteractableObjectUntouched;
                objectToMonitor.InteractableObjectGrabbed -= InteractableObjectGrabbed;
                objectToMonitor.InteractableObjectUngrabbed -= InteractableObjectUngrabbed;
                objectToMonitor.InteractableObjectUsed -= InteractableObjectUsed;
                objectToMonitor.InteractableObjectUnused -= InteractableObjectUnused;
            }
            CancelAllRoutines();
        }

        protected virtual InteractObjectAppearanceEventArgs SetPayload(GameObject affectingObject, InteractionType interactionType)
        {
            InteractObjectAppearanceEventArgs e;
            e.affectingObject = affectingObject;
            e.monitoringObject = objectToMonitor;
            e.objectToIgnore = ObjectToIgnore();
            e.interactionType = interactionType;
            return e;
        }

        protected virtual void RestoreDefaults()
        {
            if (objectToMonitor != null && objectToMonitor.IsTouched())
            {
                ToggleState(touchAffectedObject, gameObjectActiveByDefault, rendererVisibleByDefault, InteractionType.None);
            }
        }

        protected virtual GameObject ObjectToIgnore()
        {
            return (objectToAffect == null ? objectToMonitor.gameObject : null);
        }

        protected virtual void EmitRenderEvent(GameObject objectToToggle, bool rendererShow, InteractionType interactionType)
        {
            if (rendererShow)
            {
                OnRenderersEnabled(SetPayload(objectToToggle, interactionType));
            }
            else
            {
                OnRenderersDisabled(SetPayload(objectToToggle, interactionType));
            }
        }

        protected virtual void EmitGameObjectEvent(GameObject objectToToggle, bool gameObjectShow, InteractionType interactionType)
        {
            if (gameObjectShow)
            {
                OnGameObjectEnabled(SetPayload(objectToToggle, interactionType));
            }
            else
            {
                OnGameObjectDisabled(SetPayload(objectToToggle, interactionType));
            }
        }

        protected virtual void ToggleState(GameObject objectToToggle, bool gameObjectShow, bool rendererShow, InteractionType interactionType)
        {
            if (objectToToggle != null)
            {
                if (!currentStatesSet || currentRenderState != rendererShow)
                {
                    VRTK_ObjectAppearance.ToggleRenderer(rendererShow, objectToToggle, ObjectToIgnore());
                    EmitRenderEvent(objectToToggle, rendererShow, interactionType);
                }

                if (!currentStatesSet || currentGameObjectState != gameObjectShow)
                {
                    objectToToggle.SetActive(gameObjectShow);
                    EmitGameObjectEvent(objectToToggle, gameObjectShow, interactionType);
                }

                currentRenderState = rendererShow;
                currentGameObjectState = gameObjectShow;
                currentStatesSet = true;
            }
        }

        protected virtual IEnumerator ToggleStateAfterTime(GameObject objectToToggle, bool gameObjectShow, bool rendererShow, float delayTime, InteractionType interactionType)
        {
            yield return new WaitForSeconds(delayTime);
            ToggleState(objectToToggle, gameObjectShow, rendererShow, interactionType);
        }

        protected virtual void CancelAllRoutines()
        {
            CancelTouchRoutine();
            CancelGrabRoutine();
            CancelUseRoutine();
        }

        protected virtual void CancelTouchRoutine()
        {
            if (touchRoutine != null)
            {
                StopCoroutine(touchRoutine);
                touchRoutine = null;
            }
        }

        protected virtual void CancelGrabRoutine()
        {
            if (grabRoutine != null)
            {
                StopCoroutine(grabRoutine);
                grabRoutine = null;
            }
        }

        protected virtual void CancelUseRoutine()
        {
            if (useRoutine != null)
            {
                StopCoroutine(useRoutine);
                useRoutine = null;
            }
        }

        protected virtual GameObject GetActualController(GameObject givenObject)
        {
            VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(givenObject);
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                return controllerReference.actual;
            }
            else
            {
                return givenObject;
            }
        }

        protected virtual void InteractableObjectDisabled(object sender, InteractableObjectEventArgs e)
        {
            if (objectToMonitor != null && !objectToMonitor.gameObject.activeInHierarchy)
            {
                RestoreDefaults();
            }
        }

        protected virtual bool IsValidInteractingObject(GameObject givenObject, ValidInteractingObject givenInteractingObjectValidType)
        {
            if (gameObject.activeInHierarchy)
            {
                VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(givenObject);
                switch (givenInteractingObjectValidType)
                {
                    case ValidInteractingObject.Anything:
                        return true;
                    case ValidInteractingObject.EitherController:
                        return VRTK_ControllerReference.IsValid(controllerReference);
                    case ValidInteractingObject.NeitherController:
                        return !VRTK_ControllerReference.IsValid(controllerReference);
                    case ValidInteractingObject.LeftControllerOnly:
                        return (VRTK_ControllerReference.IsValid(controllerReference) && controllerReference.hand == SDK_BaseController.ControllerHand.Left);
                    case ValidInteractingObject.RightControllerOnly:
                        return (VRTK_ControllerReference.IsValid(controllerReference) && controllerReference.hand == SDK_BaseController.ControllerHand.Right);
                }
            }
            return false;
        }

        protected virtual void InteractableObjectTouched(object sender, InteractableObjectEventArgs e)
        {
            if (IsValidInteractingObject(e.interactingObject, validTouchInteractingObject))
            {
                CancelAllRoutines();
                touchAffectedObject = (objectToAffect == null ? GetActualController(e.interactingObject) : objectToAffect);
                touchRoutine = StartCoroutine(ToggleStateAfterTime(touchAffectedObject, gameObjectActiveOnTouch, rendererVisibleOnTouch, touchAppearanceDelay, InteractionType.Touch));
            }
        }

        protected virtual void InteractableObjectUntouched(object sender, InteractableObjectEventArgs e)
        {
            if (IsValidInteractingObject(e.interactingObject, validTouchInteractingObject))
            {
                CancelAllRoutines();
                touchRoutine = StartCoroutine(ToggleStateAfterTime(touchAffectedObject, gameObjectActiveByDefault, rendererVisibleByDefault, untouchAppearanceDelay, InteractionType.Untouch));
                touchAffectedObject = null;
            }
        }

        protected virtual void InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            if (IsValidInteractingObject(e.interactingObject, validGrabInteractingObject))
            {
                CancelAllRoutines();
                grabAffectedObject = (objectToAffect == null ? GetActualController(e.interactingObject) : objectToAffect);
                grabRoutine = StartCoroutine(ToggleStateAfterTime(grabAffectedObject, gameObjectActiveOnGrab, rendererVisibleOnGrab, grabAppearanceDelay, InteractionType.Grab));
            }
        }

        protected virtual void InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            if (IsValidInteractingObject(e.interactingObject, validGrabInteractingObject))
            {
                CancelAllRoutines();
                if (objectToMonitor.IsUsing())
                {
                    grabRoutine = StartCoroutine(ToggleStateAfterTime(grabAffectedObject, gameObjectActiveOnUse, rendererVisibleOnUse, ungrabAppearanceDelay, InteractionType.Ungrab));
                }
                else if (objectToMonitor.IsTouched())
                {
                    grabRoutine = StartCoroutine(ToggleStateAfterTime(grabAffectedObject, gameObjectActiveOnTouch, rendererVisibleOnTouch, ungrabAppearanceDelay, InteractionType.Ungrab));
                }
                else
                {
                    grabRoutine = StartCoroutine(ToggleStateAfterTime(grabAffectedObject, gameObjectActiveByDefault, rendererVisibleByDefault, ungrabAppearanceDelay, InteractionType.Ungrab));
                }
                grabAffectedObject = null;
            }
        }

        protected virtual void InteractableObjectUsed(object sender, InteractableObjectEventArgs e)
        {
            if (IsValidInteractingObject(e.interactingObject, validUseInteractingObject))
            {
                CancelAllRoutines();
                useAffectedObject = (objectToAffect == null ? GetActualController(e.interactingObject) : objectToAffect);
                useRoutine = StartCoroutine(ToggleStateAfterTime(useAffectedObject, gameObjectActiveOnUse, rendererVisibleOnUse, useAppearanceDelay, InteractionType.Use));
            }
        }

        protected virtual void InteractableObjectUnused(object sender, InteractableObjectEventArgs e)
        {
            if (IsValidInteractingObject(e.interactingObject, validUseInteractingObject))
            {
                CancelAllRoutines();
                if (objectToMonitor.IsGrabbed())
                {
                    useRoutine = StartCoroutine(ToggleStateAfterTime(useAffectedObject, gameObjectActiveOnGrab, rendererVisibleOnGrab, unuseAppearanceDelay, InteractionType.Unuse));
                }
                else if (objectToMonitor.IsTouched())
                {
                    useRoutine = StartCoroutine(ToggleStateAfterTime(useAffectedObject, gameObjectActiveOnTouch, rendererVisibleOnTouch, unuseAppearanceDelay, InteractionType.Unuse));
                }
                else
                {
                    useRoutine = StartCoroutine(ToggleStateAfterTime(useAffectedObject, gameObjectActiveByDefault, rendererVisibleByDefault, unuseAppearanceDelay, InteractionType.Unuse));
                }
                useAffectedObject = null;
            }
        }
    }
}