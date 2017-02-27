// UI Pointer|UI|80020
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using System.Collections;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controllerIndex">The index of the controller that was used.</param>
    /// <param name="isActive">The state of whether the UI Pointer is currently active or not.</param>
    /// <param name="currentTarget">The current UI element that the pointer is colliding with.</param>
    /// <param name="previousTarget">The previous UI element that the pointer was colliding with.</param>
    public struct UIPointerEventArgs
    {
        public uint controllerIndex;
        public bool isActive;
        public GameObject currentTarget;
        public GameObject previousTarget;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="UIPointerEventArgs"/></param>
    public delegate void UIPointerEventHandler(object sender, UIPointerEventArgs e);

    /// <summary>
    /// The UI Pointer provides a mechanism for interacting with Unity UI elements on a world canvas. The UI Pointer can be attached to any game object the same way in which a Base Pointer can be and the UI Pointer also requires a controller to initiate the pointer activation and pointer click states.
    /// </summary>
    /// <remarks>
    /// The simplest way to use the UI Pointer is to attach the script to a game controller along with a Simple Pointer as this provides visual feedback as to where the UI ray is pointing.
    ///
    /// The UI pointer is activated via the `Pointer` alias on the `Controller Events` and the UI pointer click state is triggered via the `UI Click` alias on the `Controller Events`.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/034_Controls_InteractingWithUnityUI` uses the `VRTK_UIPointer` script on the right Controller to allow for the interaction with Unity UI elements using a Simple Pointer beam. The left Controller controls a Simple Pointer on the headset to demonstrate gaze interaction with Unity UI elements.
    /// </example>
    public class VRTK_UIPointer : MonoBehaviour
    {
        /// <summary>
        /// Methods of activation.
        /// </summary>
        /// <param name="HoldButton">Only activates the UI Pointer when the Pointer button on the controller is pressed and held down.</param>
        /// <param name="ToggleButton">Activates the UI Pointer on the first click of the Pointer button on the controller and it stays active until the Pointer button is clicked again.</param>
        /// <param name="AlwaysOn">The UI Pointer is always active regardless of whether the Pointer button on the controller is pressed or not.</param>
        public enum ActivationMethods
        {
            HoldButton,
            ToggleButton,
            AlwaysOn
        }

        /// <summary>
        /// Methods of when to consider a UI Click action
        /// </summary>
        /// <param name="ClickOnButtonUp">Consider a UI Click action has happened when the UI Click alias button is released.</param>
        /// <param name="ClickOnButtonDown">Consider a UI Click action has happened when the UI Click alias button is pressed.</param>
        public enum ClickMethods
        {
            ClickOnButtonUp,
            ClickOnButtonDown
        }

        [Header("Activation Settings")]

        [Tooltip("The button used to activate/deactivate the UI raycast for the pointer.")]
        public VRTK_ControllerEvents.ButtonAlias activationButton = VRTK_ControllerEvents.ButtonAlias.Touchpad_Press;
        [Tooltip("Determines when the UI pointer should be active.")]
        public ActivationMethods activationMode = ActivationMethods.HoldButton;

        [Header("Selection Settings")]

        [Tooltip("The button used to execute the select action at the pointer's target position.")]
        public VRTK_ControllerEvents.ButtonAlias selectionButton = VRTK_ControllerEvents.ButtonAlias.Trigger_Press;
        [Tooltip("Determines when the UI Click event action should happen.")]
        public ClickMethods clickMethod = ClickMethods.ClickOnButtonUp;
        [Tooltip("Determines whether the UI click action should be triggered when the pointer is deactivated. If the pointer is hovering over a clickable element then it will invoke the click action on that element. Note: Only works with `Click Method =  Click_On_Button_Up`")]
        public bool attemptClickOnDeactivate = false;
        [Tooltip("The amount of time the pointer can be over the same UI element before it automatically attempts to click it. 0f means no click attempt will be made.")]
        public float clickAfterHoverDuration = 0f;

        [Header("Customisation Settings")]

        [Tooltip("The controller that will be used to toggle the pointer. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_ControllerEvents controller;
        [Tooltip("A custom transform to use as the origin of the pointer. If no pointer origin transform is provided then the transform the script is attached to is used.")]
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

        private bool pointerClicked = false;
        private bool beamEnabledState = false;
        private bool lastPointerPressState = false;
        private bool lastPointerClickState = false;
        private GameObject currentTarget;

        private EventSystem cachedEventSystem;
        private VRTK_VRInputModule cachedVRInputModule;

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

        public virtual UIPointerEventArgs SetUIPointerEvent(GameObject currentTarget, GameObject lastTarget = null)
        {
            UIPointerEventArgs e;
            e.controllerIndex = VRTK_DeviceFinder.GetControllerIndex(controller.gameObject);
            e.isActive = PointerActive();
            e.currentTarget = currentTarget;
            e.previousTarget = lastTarget;
            return e;
        }

        /// <summary>
        /// The SetEventSystem method is used to set up the global Unity event system for the UI pointer. It also handles disabling the existing Standalone Input Module that exists on the EventSystem and adds a custom VRTK Event System VR Input component that is required for interacting with the UI with VR inputs.
        /// </summary>
        /// <param name="eventSystem">The global Unity event system to be used by the UI pointers.</param>
        /// <returns>A custom input module that is used to detect input from VR pointers.</returns>
        public virtual VRTK_VRInputModule SetEventSystem(EventSystem eventSystem)
        {
            if (!eventSystem)
            {
                Debug.LogError("A VRTK_UIPointer requires an EventSystem");
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
            var vrtkEventSystem = FindObjectOfType<VRTK_EventSystem>();

            if (!vrtkEventSystem)
            {
                Debug.LogError("A VRTK_UIPointer requires an EventSystem");
                return;
            }

            Destroy(vrtkEventSystem);
        }

        /// <summary>
        /// The PointerActive method determines if the ui pointer beam should be active based on whether the pointer alias is being held and whether the Hold Button To Use parameter is checked.
        /// </summary>
        /// <returns>Returns true if the ui pointer should be currently active.</returns>
        public virtual bool PointerActive()
        {
            if (activationMode == ActivationMethods.AlwaysOn || autoActivatingCanvas != null)
            {
                return true;
            }
            else if (activationMode == ActivationMethods.HoldButton)
            {
                return controller.IsButtonPressed(activationButton);
            }
            else
            {
                pointerClicked = false;
                if (controller.IsButtonPressed(activationButton) && !lastPointerPressState)
                {
                    pointerClicked = true;
                }
                lastPointerPressState = controller.IsButtonPressed(activationButton);

                if (pointerClicked)
                {
                    beamEnabledState = !beamEnabledState;
                }

                return beamEnabledState;
            }
        }

        /// <summary>
        /// The SelectionButtonActive method is used to determine if the configured selection button is currently in the active state.
        /// </summary>
        /// <returns>Returns true if the selection button is active.</returns>
        public virtual bool SelectionButtonActive()
        {
            return controller.IsButtonPressed(selectionButton);
        }

        /// <summary>
        /// The ValidClick method determines if the UI Click button is in a valid state to register a click action.
        /// </summary>
        /// <param name="checkLastClick">If this is true then the last frame's state of the UI Click button is also checked to see if a valid click has happened.</param>
        /// <param name="lastClickState">This determines what the last frame's state of the UI Click button should be in for it to be a valid click.</param>
        /// <returns>Returns true if the UI Click button is in a valid state to action a click, returns false if it is not in a valid state.</returns>
        public virtual bool ValidClick(bool checkLastClick, bool lastClickState = false)
        {
            var controllerClicked = (collisionClick ? collisionClick : SelectionButtonActive());
            var result = (checkLastClick ? controllerClicked && lastPointerClickState == lastClickState : controllerClicked);
            lastPointerClickState = controllerClicked;
            return result;
        }

        /// <summary>
        /// The GetOriginPosition method returns the relevant transform position for the pointer based on whether the pointerOriginTransform variable is valid.
        /// </summary>
        /// <returns>A Vector3 of the pointer transform position</returns>
        public virtual Vector3 GetOriginPosition()
        {
            return (pointerOriginTransform ? pointerOriginTransform.position : transform.position);
        }

        /// <summary>
        /// The GetOriginPosition method returns the relevant transform forward for the pointer based on whether the pointerOriginTransform variable is valid.
        /// </summary>
        /// <returns>A Vector3 of the pointer transform forward</returns>
        public virtual Vector3 GetOriginForward()
        {
            return (pointerOriginTransform ? pointerOriginTransform.forward : transform.forward);
        }

        protected virtual void OnEnable()
        {
            pointerOriginTransform = (pointerOriginTransform == null ? VRTK_SDK_Bridge.GenerateControllerPointerOrigin(gameObject) : pointerOriginTransform);

            if (controller == null)
            {
                controller = GetComponent<VRTK_ControllerEvents>();
            }
            ConfigureEventSystem();
            pointerClicked = false;
            lastPointerPressState = false;
            lastPointerClickState = false;
            beamEnabledState = false;
            controllerRenderModel = VRTK_SDK_Bridge.GetControllerRenderModel(controller.gameObject);
        }

        protected virtual void OnDisable()
        {
            if (cachedVRInputModule && cachedVRInputModule.pointers.Contains(this))
            {
                cachedVRInputModule.pointers.Remove(this);
            }
        }

        private void ResetHoverTimer()
        {
            hoverDurationTimer = 0f;
            canClickOnHover = false;
        }

        private void ConfigureEventSystem()
        {
            if (!cachedEventSystem)
            {
                cachedEventSystem = FindObjectOfType<EventSystem>();
            }

            if (!cachedVRInputModule)
            {
                cachedVRInputModule = SetEventSystem(cachedEventSystem);
            }

            if (cachedEventSystem && cachedVRInputModule)
            {
                if (pointerEventData == null)
                {
                    pointerEventData = new PointerEventData(cachedEventSystem);
                }

                StartCoroutine(WaitForPointerId());

                if (!cachedVRInputModule.pointers.Contains(this))
                {
                    cachedVRInputModule.pointers.Add(this);
                }
            }
        }

        private IEnumerator WaitForPointerId()
        {
            var index = (int)VRTK_SDK_Bridge.GetControllerIndex(controller.gameObject);
            while (index < 0 || index == int.MaxValue)
            {
                index = (int)VRTK_SDK_Bridge.GetControllerIndex(controller.gameObject);
                yield return null;
            }
            pointerEventData.pointerId = index;
        }
    }
}