// UI Pointer|UI|80020
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using System.Collections.Generic;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controllerReference">The reference to the controller that was used.</param>
    /// <param name="isActive">The state of whether the UI Pointer is currently active or not.</param>
    /// <param name="currentTarget">The current UI element that the pointer is colliding with.</param>
    /// <param name="previousTarget">The previous UI element that the pointer was colliding with.</param>
    /// <param name="raycastResult">The raw raycast result of the UI ray collision.</param>
    public struct UIPointerEventArgs
    {
        public VRTK_ControllerReference controllerReference;
        public bool isActive;
        public GameObject currentTarget;
        public GameObject previousTarget;
        public RaycastResult raycastResult;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="UIPointerEventArgs"/></param>
    public delegate void UIPointerEventHandler(object sender, UIPointerEventArgs e);

    /// <summary>
    /// Provides the ability to interact with UICanvas elements and the contained Unity UI elements within.
    /// </summary>
    /// <remarks>
    /// **Optional Components:**
    ///  * `VRTK_ControllerEvents` - The events component to listen for the button presses on. This must be applied on the same GameObject as this script if one is not provided via the `Controller` parameter.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_UIPointer` script on either:
    ///    * The controller script alias GameObject of the controller to emit the UIPointer from (e.g. Right Controller Script Alias).
    ///    * Any other scene GameObject and provide a valid `Transform` component to the `Pointer Origin Transform` parameter of this script. This does not have to be a controller and can be any GameObject that will emit the UIPointer.
    ///
    /// **Script Dependencies:**
    ///  * A UI Canvas attached to a Unity World UI Canvas.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/034_Controls_InteractingWithUnityUI` uses the `VRTK_UIPointer` script on the right Controller to allow for the interaction with Unity UI elements using a Simple Pointer beam. The left Controller controls a Simple Pointer on the headset to demonstrate gaze interaction with Unity UI elements.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/UI/VRTK_UIPointer")]
    public class VRTK_UIPointer : MonoBehaviour
    {
        /// <summary>
        /// Methods of activation.
        /// </summary>
        public enum ActivationMethods
        {
            /// <summary>
            /// Only activates the UI Pointer when the Pointer button on the controller is pressed and held down.
            /// </summary>
            HoldButton,
            /// <summary>
            /// Activates the UI Pointer on the first click of the Pointer button on the controller and it stays active until the Pointer button is clicked again.
            /// </summary>
            ToggleButton,
            /// <summary>
            /// The UI Pointer is always active regardless of whether the Pointer button on the controller is pressed or not.
            /// </summary>
            AlwaysOn
        }

        /// <summary>
        /// Methods of when to consider a UI Click action
        /// </summary>
        public enum ClickMethods
        {
            /// <summary>
            /// Consider a UI Click action has happened when the UI Click alias button is released.
            /// </summary>
            ClickOnButtonUp,
            /// <summary>
            /// Consider a UI Click action has happened when the UI Click alias button is pressed.
            /// </summary>
            ClickOnButtonDown
        }

        [Header("Activation Settings")]

        [Tooltip("The button used to activate/deactivate the UI raycast for the pointer.")]
        public VRTK_ControllerEvents.ButtonAlias activationButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;
        [Tooltip("Determines when the UI pointer should be active.")]
        public ActivationMethods activationMode = ActivationMethods.HoldButton;

        [Header("Selection Settings")]

        [Tooltip("The button used to execute the select action at the pointer's target position.")]
        public VRTK_ControllerEvents.ButtonAlias selectionButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
        [Tooltip("Determines when the UI Click event action should happen.")]
        public ClickMethods clickMethod = ClickMethods.ClickOnButtonUp;
        [Tooltip("Determines whether the UI click action should be triggered when the pointer is deactivated. If the pointer is hovering over a clickable element then it will invoke the click action on that element. Note: Only works with `Click Method =  Click_On_Button_Up`")]
        public bool attemptClickOnDeactivate = false;
        [Tooltip("The amount of time the pointer can be over the same UI element before it automatically attempts to click it. 0f means no click attempt will be made.")]
        public float clickAfterHoverDuration = 0f;

        [Header("Customisation Settings")]

        [Tooltip("The maximum length the UI Raycast will reach.")]
        public float maximumLength = float.PositiveInfinity;
        [Tooltip("An optional GameObject that determines what the pointer is to be attached to. If this is left blank then the GameObject the script is on will be used.")]
        public GameObject attachedTo;
        [Tooltip("The Controller Events that will be used to toggle the pointer. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_ControllerEvents controllerEvents;
        [Tooltip("A custom transform to use as the origin of the pointer. If no pointer origin transform is provided then the transform the script is attached to is used.")]
        public Transform customOrigin = null;

        [Header("Obsolete Settings")]

        [System.Obsolete("`VRTK_UIPointer.controller` has been replaced with `VRTK_UIPointer.controllerEvents`. This parameter will be removed in a future version of VRTK.")]
        [ObsoleteInspector]
        public VRTK_ControllerEvents controller;
        [System.Obsolete("`VRTK_UIPointer.pointerOriginTransform` has been replaced with `VRTK_UIPointer.customOrigin`. This parameter will be removed in a future version of VRTK.")]
        [ObsoleteInspector]
        public Transform pointerOriginTransform = null;

        [HideInInspector]
        public PointerEventData pointerEventData;
        [HideInInspector]
        public GameObject hoveringElement;
        [HideInInspector]
        public GameObject controllerRenderModel;
        [HideInInspector]
        public float hoverDurationTimer = 0f;
        [HideInInspector]
        public bool canClickOnHover = false;

        /// <summary>
        /// The GameObject of the front trigger activator of the canvas currently being activated by this pointer.
        /// </summary>
        [HideInInspector]
        public GameObject autoActivatingCanvas = null;
        /// <summary>
        /// Determines if the UI Pointer has collided with a valid canvas that has collision click turned on.
        /// </summary>
        [HideInInspector]
        public bool collisionClick = false;

        /// <summary>
        /// Emitted when the UI activation button is pressed.
        /// </summary>
        public event ControllerInteractionEventHandler ActivationButtonPressed;
        /// <summary>
        /// Emitted when the UI activation button is released.
        /// </summary>
        public event ControllerInteractionEventHandler ActivationButtonReleased;
        /// <summary>
        /// Emitted when the UI selection button is pressed.
        /// </summary>
        public event ControllerInteractionEventHandler SelectionButtonPressed;
        /// <summary>
        /// Emitted when the UI selection button is released.
        /// </summary>
        public event ControllerInteractionEventHandler SelectionButtonReleased;

        /// <summary>
        /// Emitted when the UI Pointer is colliding with a valid UI element.
        /// </summary>
        public event UIPointerEventHandler UIPointerElementEnter;
        /// <summary>
        /// Emitted when the UI Pointer is no longer colliding with any valid UI elements.
        /// </summary>
        public event UIPointerEventHandler UIPointerElementExit;
        /// <summary>
        /// Emitted when the UI Pointer has clicked the currently collided UI element.
        /// </summary>
        public event UIPointerEventHandler UIPointerElementClick;
        /// <summary>
        /// Emitted when the UI Pointer begins dragging a valid UI element.
        /// </summary>
        public event UIPointerEventHandler UIPointerElementDragStart;
        /// <summary>
        /// Emitted when the UI Pointer stops dragging a valid UI element.
        /// </summary>
        public event UIPointerEventHandler UIPointerElementDragEnd;

        protected static Dictionary<int, float> pointerLengths = new Dictionary<int, float>();
        protected bool pointerClicked = false;
        protected bool beamEnabledState = false;
        protected bool lastPointerPressState = false;
        protected bool lastPointerClickState = false;
        protected GameObject currentTarget;

        protected SDK_BaseController.ControllerHand cachedAttachedHand = SDK_BaseController.ControllerHand.None;
        protected Transform cachedPointerAttachPoint = null;
        protected EventSystem cachedEventSystem;
        protected VRTK_VRInputModule cachedVRInputModule;

        /// <summary>
        /// The GetPointerLength method retrieves the maximum UI Pointer length for the given pointer ID.
        /// </summary>
        /// <param name="pointerId">The pointer ID for the UI Pointer to recieve the length for.</param>
        /// <returns>The maximum length the UI Pointer will cast to.</returns>
        public static float GetPointerLength(int pointerId)
        {
            return VRTK_SharedMethods.GetDictionaryValue(pointerLengths, pointerId, float.MaxValue);
        }

        public virtual void OnUIPointerElementEnter(UIPointerEventArgs e)
        {
            if (e.currentTarget != currentTarget)
            {
                ResetHoverTimer();
            }

            if (clickAfterHoverDuration > 0f && hoverDurationTimer <= 0f)
            {
                canClickOnHover = true;
                hoverDurationTimer = clickAfterHoverDuration;
            }

            currentTarget = e.currentTarget;
            if (UIPointerElementEnter != null)
            {
                UIPointerElementEnter(this, e);
            }
        }

        public virtual void OnUIPointerElementExit(UIPointerEventArgs e)
        {
            if (e.previousTarget == currentTarget)
            {
                ResetHoverTimer();
            }
            if (UIPointerElementExit != null)
            {
                UIPointerElementExit(this, e);

                if (attemptClickOnDeactivate && !e.isActive && e.previousTarget)
                {
                    pointerEventData.pointerPress = e.previousTarget;
                }
            }
        }

        public virtual void OnUIPointerElementClick(UIPointerEventArgs e)
        {
            if (e.currentTarget == currentTarget)
            {
                ResetHoverTimer();
            }

            if (UIPointerElementClick != null)
            {
                UIPointerElementClick(this, e);
            }
        }

        public virtual void OnUIPointerElementDragStart(UIPointerEventArgs e)
        {
            if (UIPointerElementDragStart != null)
            {
                UIPointerElementDragStart(this, e);
            }
        }

        public virtual void OnUIPointerElementDragEnd(UIPointerEventArgs e)
        {
            if (UIPointerElementDragEnd != null)
            {
                UIPointerElementDragEnd(this, e);
            }
        }

        public virtual void OnActivationButtonPressed(ControllerInteractionEventArgs e)
        {
            if (ActivationButtonPressed != null)
            {
                ActivationButtonPressed(this, e);
            }
        }

        public virtual void OnActivationButtonReleased(ControllerInteractionEventArgs e)
        {
            if (ActivationButtonReleased != null)
            {
                ActivationButtonReleased(this, e);
            }
        }

        public virtual void OnSelectionButtonPressed(ControllerInteractionEventArgs e)
        {
            if (SelectionButtonPressed != null)
            {
                SelectionButtonPressed(this, e);
            }
        }

        public virtual void OnSelectionButtonReleased(ControllerInteractionEventArgs e)
        {
            if (SelectionButtonReleased != null)
            {
                SelectionButtonReleased(this, e);
            }
        }

        public virtual UIPointerEventArgs SetUIPointerEvent(RaycastResult currentRaycastResult, GameObject currentTarget, GameObject lastTarget = null)
        {
            UIPointerEventArgs e;
            e.controllerReference = GetControllerReference();
            e.isActive = PointerActive();
            e.currentTarget = currentTarget;
            e.previousTarget = lastTarget;
            e.raycastResult = currentRaycastResult;
            return e;
        }

        /// <summary>
        /// The SetEventSystem method is used to set up the global Unity event system for the UI pointer. It also handles disabling the existing Standalone Input Module that exists on the EventSystem and adds a custom VRTK Event System VR Input component that is required for interacting with the UI with VR inputs.
        /// </summary>
        /// <param name="eventSystem">The global Unity event system to be used by the UI pointers.</param>
        /// <returns>A custom input module that is used to detect input from VR pointers.</returns>
        public virtual VRTK_VRInputModule SetEventSystem(EventSystem eventSystem)
        {
            if (eventSystem == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "VRTK_UIPointer", "EventSystem"));
                return null;
            }

            if (!(eventSystem is VRTK_EventSystem))
            {
                eventSystem = eventSystem.gameObject.AddComponent<VRTK_EventSystem>();
            }

            return eventSystem.GetComponent<VRTK_VRInputModule>();
        }

        /// <summary>
        /// The RemoveEventSystem resets the Unity EventSystem back to the original state before the VRTK_VRInputModule was swapped for it.
        /// </summary>
        public virtual void RemoveEventSystem()
        {
            VRTK_EventSystem vrtkEventSystem = FindObjectOfType<VRTK_EventSystem>();

            if (vrtkEventSystem == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "VRTK_UIPointer", "EventSystem"));
                return;
            }

            Destroy(vrtkEventSystem);
        }

        /// <summary>
        /// The PointerActive method determines if the ui pointer beam should be active based on whether the pointer alias is being held and whether the Hold Button To Use parameter is checked.
        /// </summary>
        /// <returns>Returns `true` if the ui pointer should be currently active.</returns>
        public virtual bool PointerActive()
        {
            if (activationMode == ActivationMethods.AlwaysOn || autoActivatingCanvas != null)
            {
                return true;
            }
            else if (activationMode == ActivationMethods.HoldButton)
            {
                return IsActivationButtonPressed();
            }
            else
            {
                pointerClicked = false;
                if (IsActivationButtonPressed() && !lastPointerPressState)
                {
                    pointerClicked = true;
                }
                lastPointerPressState = (controllerEvents != null ? controllerEvents.IsButtonPressed(activationButton) : false);

                if (pointerClicked)
                {
                    beamEnabledState = !beamEnabledState;
                }

                return beamEnabledState;
            }
        }

        /// <summary>
        /// The IsActivationButtonPressed method is used to determine if the configured activation button is currently in the active state.
        /// </summary>
        /// <returns>Returns `true` if the activation button is active.</returns>
        public virtual bool IsActivationButtonPressed()
        {
            return (controllerEvents != null ? controllerEvents.IsButtonPressed(activationButton) : false);
        }

        /// <summary>
        /// The IsSelectionButtonPressed method is used to determine if the configured selection button is currently in the active state.
        /// </summary>
        /// <returns>Returns `true` if the selection button is active.</returns>
        public virtual bool IsSelectionButtonPressed()
        {
            return (controllerEvents != null ? controllerEvents.IsButtonPressed(selectionButton) : false);
        }

        /// <summary>
        /// The ValidClick method determines if the UI Click button is in a valid state to register a click action.
        /// </summary>
        /// <param name="checkLastClick">If this is true then the last frame's state of the UI Click button is also checked to see if a valid click has happened.</param>
        /// <param name="lastClickState">This determines what the last frame's state of the UI Click button should be in for it to be a valid click.</param>
        /// <returns>Returns `true` if the UI Click button is in a valid state to action a click, returns `false` if it is not in a valid state.</returns>
        public virtual bool ValidClick(bool checkLastClick, bool lastClickState = false)
        {
            bool controllerClicked = (collisionClick ? collisionClick : IsSelectionButtonPressed());
            bool result = (checkLastClick ? controllerClicked && lastPointerClickState == lastClickState : controllerClicked);
            lastPointerClickState = controllerClicked;
            return result;
        }

        /// <summary>
        /// The GetOriginPosition method returns the relevant transform position for the pointer based on whether the pointerOriginTransform variable is valid.
        /// </summary>
        /// <returns>A Vector3 of the pointer transform position</returns>
        public virtual Vector3 GetOriginPosition()
        {
            return (customOrigin != null ? customOrigin : GetPointerOriginTransform()).position;
        }

        /// <summary>
        /// The GetOriginPosition method returns the relevant transform forward for the pointer based on whether the pointerOriginTransform variable is valid.
        /// </summary>
        /// <returns>A Vector3 of the pointer transform forward</returns>
        public virtual Vector3 GetOriginForward()
        {
            return (customOrigin != null ? customOrigin : GetPointerOriginTransform()).forward;
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.AttemptAddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
#pragma warning disable 0618
            controllerEvents = (controller != null && controllerEvents == null ? controller : controllerEvents);
            customOrigin = (pointerOriginTransform != null && customOrigin == null ? pointerOriginTransform : customOrigin);
#pragma warning restore 0618
            attachedTo = (attachedTo == null ? gameObject : attachedTo);
            controllerEvents = (controllerEvents != null ? controllerEvents : GetComponentInParent<VRTK_ControllerEvents>());
            ConfigureEventSystem();
            pointerClicked = false;
            lastPointerPressState = false;
            lastPointerClickState = false;
            beamEnabledState = false;

            if (controllerEvents != null)
            {
                controllerEvents.SubscribeToButtonAliasEvent(activationButton, true, DoActivationButtonPressed);
                controllerEvents.SubscribeToButtonAliasEvent(activationButton, false, DoActivationButtonReleased);
                controllerEvents.SubscribeToButtonAliasEvent(selectionButton, true, DoSelectionButtonPressed);
                controllerEvents.SubscribeToButtonAliasEvent(selectionButton, false, DoSelectionButtonReleased);
            }
        }

        protected virtual void OnDisable()
        {
            if (cachedVRInputModule && cachedVRInputModule.pointers.Contains(this))
            {
                cachedVRInputModule.pointers.Remove(this);
            }

            if (controllerEvents != null)
            {
                controllerEvents.UnsubscribeToButtonAliasEvent(activationButton, true, DoActivationButtonPressed);
                controllerEvents.UnsubscribeToButtonAliasEvent(activationButton, false, DoActivationButtonReleased);
                controllerEvents.UnsubscribeToButtonAliasEvent(selectionButton, true, DoSelectionButtonPressed);
                controllerEvents.UnsubscribeToButtonAliasEvent(selectionButton, false, DoSelectionButtonReleased);
            }
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.AttemptRemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void LateUpdate()
        {
            if (controllerEvents != null)
            {
                pointerEventData.pointerId = (int)VRTK_ControllerReference.GetRealIndex(GetControllerReference());
                VRTK_SharedMethods.AddDictionaryValue(pointerLengths, pointerEventData.pointerId, maximumLength, true);
            }
            if (controllerRenderModel == null && VRTK_ControllerReference.IsValid(GetControllerReference()))
            {
                controllerRenderModel = VRTK_SDK_Bridge.GetControllerRenderModel(GetControllerReference());
            }
        }

        protected virtual void DoActivationButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            OnActivationButtonPressed(controllerEvents.SetControllerEvent());
        }

        protected virtual void DoActivationButtonReleased(object sender, ControllerInteractionEventArgs e)
        {
            OnActivationButtonReleased(controllerEvents.SetControllerEvent());
        }

        protected virtual void DoSelectionButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            OnSelectionButtonPressed(controllerEvents.SetControllerEvent());
        }

        protected virtual void DoSelectionButtonReleased(object sender, ControllerInteractionEventArgs e)
        {
            OnSelectionButtonReleased(controllerEvents.SetControllerEvent());
        }

        protected virtual VRTK_ControllerReference GetControllerReference(GameObject reference = null)
        {
            reference = (reference == null && controllerEvents != null ? controllerEvents.gameObject : reference);
            return VRTK_ControllerReference.GetControllerReference(reference);
        }

        protected virtual Transform GetPointerOriginTransform()
        {
            VRTK_ControllerReference controllerReference = GetControllerReference(attachedTo);
            if (VRTK_ControllerReference.IsValid(controllerReference) && (cachedAttachedHand != controllerReference.hand || cachedPointerAttachPoint == null))
            {
                cachedPointerAttachPoint = controllerReference.model.transform.Find(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.AttachPoint, controllerReference.hand));
                cachedAttachedHand = controllerReference.hand;
            }
            return (cachedPointerAttachPoint != null ? cachedPointerAttachPoint : transform);
        }

        protected virtual void ResetHoverTimer()
        {
            hoverDurationTimer = 0f;
            canClickOnHover = false;
        }

        protected virtual void ConfigureEventSystem()
        {
            if (cachedEventSystem == null)
            {
                cachedEventSystem = FindObjectOfType<EventSystem>();
            }

            if (cachedVRInputModule == null)
            {
                cachedVRInputModule = SetEventSystem(cachedEventSystem);
            }

            if (cachedEventSystem != null && cachedVRInputModule != null)
            {
                if (pointerEventData == null)
                {
                    pointerEventData = new PointerEventData(cachedEventSystem);
                }

                if (!cachedVRInputModule.pointers.Contains(this))
                {
                    cachedVRInputModule.pointers.Add(this);
                }
            }
        }
    }
}