// Interact Object Appearance|Interactions|30040
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

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
        public VRTK_InteractableObject.InteractionType interactionType;
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
        /// The valid interacting object.
        /// </summary>
        public enum ValidInteractingObject
        {
            /// <summary>
            /// Any GameObject is considered a valid interacting object.
            /// </summary>
            Anything,
            /// <summary>
            /// Only a game controller is considered a valid interacting objcet.
            /// </summary>
            EitherController,
            /// <summary>
            /// Any GameObject except a game controller is considered a valid interacting object.
            /// </summary>
            NeitherController,
            /// <summary>
            /// Only the left game controller is considered a valid interacting objcet.
            /// </summary>
            LeftControllerOnly,
            /// <summary>
            /// Only the right game controller is considered a valid interacting objcet.
            /// </summary>
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

        [Header("Near Touch Appearance Settings")]

        [Tooltip("If this is checked then the `Object To Affect` will be an active GameObject when the `Object To Monitor` is near touched. If it's unchecked then it will be disabled on near touch.")]
        public bool gameObjectActiveOnNearTouch = true;
        [Tooltip("If this is checked then the `Object To Affect` will have visible renderers when the `Object To Monitor` is near touched. If it's unchecked then it will have it's renderers disabled on near touch.")]
        public bool rendererVisibleOnNearTouch = true;
        [Tooltip("The amount of time to wait before the near touch appearance settings are applied after the near touch event.")]
        public float nearTouchAppearanceDelay = 0f;
        [Tooltip("The amount of time to wait before the previous appearance settings are applied after the near untouch event.")]
        public float nearUntouchAppearanceDelay = 0f;
        [Tooltip("Determines what type of interacting object will affect the appearance of the `Object To Affect` after the near touch and near untouch event.")]
        public ValidInteractingObject validNearTouchInteractingObject = ValidInteractingObject.Anything;

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

        protected Dictionary<GameObject, bool> currentRenderStates = new Dictionary<GameObject, bool>();
        protected Dictionary<GameObject, bool> currentGameObjectStates = new Dictionary<GameObject, bool>();
        protected Dictionary<GameObject, Coroutine> affectingRoutines = new Dictionary<GameObject, Coroutine>();
        protected List<GameObject> nearTouchingObjects = new List<GameObject>();
        protected List<GameObject> touchingObjects = new List<GameObject>();

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
            currentRenderStates.Clear();
            currentGameObjectStates.Clear();
            affectingRoutines.Clear();
            nearTouchingObjects.Clear();
            touchingObjects.Clear();
            objectToMonitor = (objectToMonitor == null ? GetComponentInParent<VRTK_InteractableObject>() : objectToMonitor);

            if (objectToMonitor != null)
            {
                objectToMonitor.InteractableObjectDisabled += InteractableObjectDisabled;

                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.NearTouch, InteractableObjectNearTouched);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.NearUntouch, InteractableObjectNearUntouched);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, InteractableObjectTouched);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Untouch, InteractableObjectUntouched);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, InteractableObjectGrabbed);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Ungrab, InteractableObjectUngrabbed);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Use, InteractableObjectUsed);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Unuse, InteractableObjectUnused);
            }
            else
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_InteractObjectAppearance", "VRTK_InteractableObject", "objectToMonitor", "current or parent"));
            }

            if (objectToAffect != null)
            {
                ToggleState(objectToAffect, gameObjectActiveByDefault, rendererVisibleByDefault, VRTK_InteractableObject.InteractionType.None);
            }
        }

        protected virtual void OnDisable()
        {
            if (objectToMonitor != null)
            {
                RestoreDefaults();
                objectToMonitor.InteractableObjectDisabled -= InteractableObjectDisabled;

                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, InteractableObjectTouched);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Untouch, InteractableObjectUntouched);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, InteractableObjectGrabbed);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Ungrab, InteractableObjectUngrabbed);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Use, InteractableObjectUsed);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Unuse, InteractableObjectUnused);
            }
            CancelRoutines();
        }

        protected virtual InteractObjectAppearanceEventArgs SetPayload(GameObject affectingObject, VRTK_InteractableObject.InteractionType interactionType)
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
                for (int i = 0; i < touchingObjects.Count; i++)
                {
                    ToggleState(touchingObjects[i], gameObjectActiveByDefault, rendererVisibleByDefault, VRTK_InteractableObject.InteractionType.None);
                }

                for (int i = 0; i < nearTouchingObjects.Count; i++)
                {
                    ToggleState(nearTouchingObjects[i], gameObjectActiveByDefault, rendererVisibleByDefault, VRTK_InteractableObject.InteractionType.None);
                }
            }
        }

        protected virtual GameObject ObjectToIgnore()
        {
            return (objectToAffect == null ? objectToMonitor.gameObject : null);
        }

        protected virtual void EmitRenderEvent(GameObject objectToToggle, bool rendererShow, VRTK_InteractableObject.InteractionType interactionType)
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

        protected virtual void EmitGameObjectEvent(GameObject objectToToggle, bool gameObjectShow, VRTK_InteractableObject.InteractionType interactionType)
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

        protected virtual void ToggleState(GameObject objectToToggle, bool gameObjectShow, bool rendererShow, VRTK_InteractableObject.InteractionType interactionType)
        {
            if (objectToToggle != null)
            {
                if (!currentRenderStates.ContainsKey(objectToToggle) || currentRenderStates[objectToToggle] != rendererShow)
                {
                    VRTK_ObjectAppearance.ToggleRenderer(rendererShow, objectToToggle, ObjectToIgnore());
                    EmitRenderEvent(objectToToggle, rendererShow, interactionType);
                }

                if (!currentGameObjectStates.ContainsKey(objectToToggle) || currentGameObjectStates[objectToToggle] != gameObjectShow)
                {
                    objectToToggle.SetActive(gameObjectShow);
                    EmitGameObjectEvent(objectToToggle, gameObjectShow, interactionType);
                }

                currentRenderStates[objectToToggle] = rendererShow;
                currentGameObjectStates[objectToToggle] = gameObjectShow;
            }
        }

        protected virtual IEnumerator ToggleStateAfterTime(GameObject objectToToggle, bool gameObjectShow, bool rendererShow, float delayTime, VRTK_InteractableObject.InteractionType interactionType)
        {
            yield return new WaitForSeconds(delayTime);
            ToggleState(objectToToggle, gameObjectShow, rendererShow, interactionType);
        }

        protected virtual void CancelRoutines(GameObject currentAffectingObject = null)
        {
            if (currentAffectingObject != null)
            {
                if (affectingRoutines.ContainsKey(currentAffectingObject) && affectingRoutines[currentAffectingObject] != null)
                {
                    StopCoroutine(affectingRoutines[currentAffectingObject]);
                }
            }
            else
            {
                foreach (KeyValuePair<GameObject, Coroutine> affectingRouting in affectingRoutines)
                {
                    if (currentAffectingObject == affectingRouting.Key && affectingRouting.Value != null)
                    {
                        StopCoroutine(affectingRouting.Value);
                    }
                }
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

        protected virtual void InteractableObjectNearTouched(object sender, InteractableObjectEventArgs e)
        {
            if (IsValidInteractingObject(e.interactingObject, validNearTouchInteractingObject))
            {
                GameObject nearTouchAffectedObject = (objectToAffect == null ? GetActualController(e.interactingObject) : objectToAffect);
                CancelRoutines(nearTouchAffectedObject);
                if (!nearTouchingObjects.Contains(nearTouchAffectedObject))
                {
                    nearTouchingObjects.Add(nearTouchAffectedObject);
                }
                affectingRoutines[nearTouchAffectedObject] = StartCoroutine(ToggleStateAfterTime(nearTouchAffectedObject, gameObjectActiveOnNearTouch, rendererVisibleOnNearTouch, nearTouchAppearanceDelay, VRTK_InteractableObject.InteractionType.NearTouch));
            }
        }

        protected virtual void InteractableObjectNearUntouched(object sender, InteractableObjectEventArgs e)
        {
            if (IsValidInteractingObject(e.interactingObject, validNearTouchInteractingObject))
            {
                GameObject nearTouchAffectedObject = (objectToAffect == null ? GetActualController(e.interactingObject) : objectToAffect);
                CancelRoutines(nearTouchAffectedObject);
                if (nearTouchingObjects.Contains(nearTouchAffectedObject))
                {
                    nearTouchingObjects.Remove(nearTouchAffectedObject);
                }
                affectingRoutines[nearTouchAffectedObject] = StartCoroutine(ToggleStateAfterTime(nearTouchAffectedObject, gameObjectActiveByDefault, rendererVisibleByDefault, nearUntouchAppearanceDelay, VRTK_InteractableObject.InteractionType.NearUntouch));
            }
        }

        protected virtual void InteractableObjectTouched(object sender, InteractableObjectEventArgs e)
        {
            if (IsValidInteractingObject(e.interactingObject, validTouchInteractingObject))
            {
                GameObject touchAffectedObject = (objectToAffect == null ? GetActualController(e.interactingObject) : objectToAffect);
                CancelRoutines(touchAffectedObject);
                if (!touchingObjects.Contains(touchAffectedObject))
                {
                    touchingObjects.Add(touchAffectedObject);
                }
                affectingRoutines[touchAffectedObject] = StartCoroutine(ToggleStateAfterTime(touchAffectedObject, gameObjectActiveOnTouch, rendererVisibleOnTouch, touchAppearanceDelay, VRTK_InteractableObject.InteractionType.Touch));
            }
        }

        protected virtual void InteractableObjectUntouched(object sender, InteractableObjectEventArgs e)
        {
            if (IsValidInteractingObject(e.interactingObject, validTouchInteractingObject))
            {
                GameObject touchAffectedObject = (objectToAffect == null ? GetActualController(e.interactingObject) : objectToAffect);
                CancelRoutines(touchAffectedObject);
                if (touchingObjects.Contains(touchAffectedObject))
                {
                    touchingObjects.Remove(touchAffectedObject);
                }
                if (objectToMonitor.IsNearTouched())
                {
                    affectingRoutines[touchAffectedObject] = StartCoroutine(ToggleStateAfterTime(touchAffectedObject, gameObjectActiveOnNearTouch, rendererVisibleOnNearTouch, untouchAppearanceDelay, VRTK_InteractableObject.InteractionType.NearTouch));
                }
                else
                {
                    affectingRoutines[touchAffectedObject] = StartCoroutine(ToggleStateAfterTime(touchAffectedObject, gameObjectActiveByDefault, rendererVisibleByDefault, untouchAppearanceDelay, VRTK_InteractableObject.InteractionType.Untouch));
                }
            }
        }

        protected virtual void InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            if (IsValidInteractingObject(e.interactingObject, validGrabInteractingObject))
            {
                GameObject grabAffectedObject = (objectToAffect == null ? GetActualController(e.interactingObject) : objectToAffect);
                CancelRoutines(grabAffectedObject);
                affectingRoutines[grabAffectedObject] = StartCoroutine(ToggleStateAfterTime(grabAffectedObject, gameObjectActiveOnGrab, rendererVisibleOnGrab, grabAppearanceDelay, VRTK_InteractableObject.InteractionType.Grab));
            }
        }

        protected virtual void InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            if (IsValidInteractingObject(e.interactingObject, validGrabInteractingObject))
            {
                GameObject grabAffectedObject = (objectToAffect == null ? GetActualController(e.interactingObject) : objectToAffect);
                CancelRoutines(grabAffectedObject);
                if (objectToMonitor.IsUsing())
                {
                    affectingRoutines[grabAffectedObject] = StartCoroutine(ToggleStateAfterTime(grabAffectedObject, gameObjectActiveOnUse, rendererVisibleOnUse, ungrabAppearanceDelay, VRTK_InteractableObject.InteractionType.Ungrab));
                }
                else if (objectToMonitor.IsTouched())
                {
                    affectingRoutines[grabAffectedObject] = StartCoroutine(ToggleStateAfterTime(grabAffectedObject, gameObjectActiveOnTouch, rendererVisibleOnTouch, ungrabAppearanceDelay, VRTK_InteractableObject.InteractionType.Ungrab));
                }
                else if (objectToMonitor.IsNearTouched())
                {
                    affectingRoutines[grabAffectedObject] = StartCoroutine(ToggleStateAfterTime(grabAffectedObject, gameObjectActiveOnNearTouch, rendererVisibleOnNearTouch, ungrabAppearanceDelay, VRTK_InteractableObject.InteractionType.NearTouch));
                }
                else
                {
                    affectingRoutines[grabAffectedObject] = StartCoroutine(ToggleStateAfterTime(grabAffectedObject, gameObjectActiveByDefault, rendererVisibleByDefault, ungrabAppearanceDelay, VRTK_InteractableObject.InteractionType.Ungrab));
                }
            }
        }

        protected virtual void InteractableObjectUsed(object sender, InteractableObjectEventArgs e)
        {
            if (IsValidInteractingObject(e.interactingObject, validUseInteractingObject))
            {
                GameObject useAffectedObject = (objectToAffect == null ? GetActualController(e.interactingObject) : objectToAffect);
                CancelRoutines(useAffectedObject);
                affectingRoutines[useAffectedObject] = StartCoroutine(ToggleStateAfterTime(useAffectedObject, gameObjectActiveOnUse, rendererVisibleOnUse, useAppearanceDelay, VRTK_InteractableObject.InteractionType.Use));
            }
        }

        protected virtual void InteractableObjectUnused(object sender, InteractableObjectEventArgs e)
        {
            if (IsValidInteractingObject(e.interactingObject, validUseInteractingObject))
            {
                GameObject useAffectedObject = (objectToAffect == null ? GetActualController(e.interactingObject) : objectToAffect);
                CancelRoutines(useAffectedObject);
                if (objectToMonitor.IsGrabbed())
                {
                    affectingRoutines[useAffectedObject] = StartCoroutine(ToggleStateAfterTime(useAffectedObject, gameObjectActiveOnGrab, rendererVisibleOnGrab, unuseAppearanceDelay, VRTK_InteractableObject.InteractionType.Unuse));
                }
                else if (objectToMonitor.IsTouched())
                {
                    affectingRoutines[useAffectedObject] = StartCoroutine(ToggleStateAfterTime(useAffectedObject, gameObjectActiveOnTouch, rendererVisibleOnTouch, unuseAppearanceDelay, VRTK_InteractableObject.InteractionType.Unuse));
                }
                else if (objectToMonitor.IsNearTouched())
                {
                    affectingRoutines[useAffectedObject] = StartCoroutine(ToggleStateAfterTime(useAffectedObject, gameObjectActiveOnNearTouch, rendererVisibleOnNearTouch, unuseAppearanceDelay, VRTK_InteractableObject.InteractionType.NearTouch));
                }
                else
                {
                    affectingRoutines[useAffectedObject] = StartCoroutine(ToggleStateAfterTime(useAffectedObject, gameObjectActiveByDefault, rendererVisibleByDefault, unuseAppearanceDelay, VRTK_InteractableObject.InteractionType.Unuse));
                }
            }
        }
    }
}