// UI Pointer|UI|80020
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using System.Collections;
#if UNITY_5_5_OR_NEWER
    using System.Linq;
    using System.Reflection;
#endif

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
        /// <param name="Hold_Button">Only activates the UI Pointer when the Pointer button on the controller is pressed and held down.</param>
        /// <param name="Toggle_Button">Activates the UI Pointer on the first click of the Pointer button on the controller and it stays active until the Pointer button is clicked again.</param>
        /// <param name="Always_On">The UI Pointer is always active regardless of whether the Pointer button on the controller is pressed or not.</param>
        public enum ActivationMethods
        {
            Hold_Button,
            Toggle_Button,
            Always_On
        }

        /// <summary>
        /// Methods of when to consider a UI Click action
        /// </summary>
        /// <param name="Click_On_Button_Up">Consider a UI Click action has happened when the UI Click alias button is released.</param>
        /// <param name="Click_On_Button_Down">Consider a UI Click action has happened when the UI Click alias button is pressed.</param>
        public enum ClickMethods
        {
            Click_On_Button_Up,
            Click_On_Button_Down
        }

        [Tooltip("The controller that will be used to toggle the pointer. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_ControllerEvents controller;
        [Tooltip("A custom transform to use as the origin of the pointer. If no pointer origin transform is provided then the transform the script is attached to is used.")]
        public Transform pointerOriginTransform = null;
        [Tooltip("Determines when the UI pointer should be active.")]
        public ActivationMethods activationMode = ActivationMethods.Hold_Button;
        [Tooltip("Determines when the UI Click event action should happen.")]
        public ClickMethods clickMethod = ClickMethods.Click_On_Button_Up;
        [Tooltip("Determines whether the UI click action should be triggered when the pointer is deactivated. If the pointer is hovering over a clickable element then it will invoke the click action on that element. Note: Only works with `Click Method =  Click_On_Button_Up`")]
        public bool attemptClickOnDeactivate = false;


        [HideInInspector]
        public PointerEventData pointerEventData;
        [HideInInspector]
        public GameObject hoveringElement;
        [HideInInspector]
        public GameObject controllerRenderModel;

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

        private bool pointerClicked = false;
        private bool beamEnabledState = false;
        private bool lastPointerPressState = false;
        private bool lastPointerClickState = false;

        private EventSystem cachedEventSystem;
        private VRTK_EventSystemVRInput cachedEventSystemInput;

        private const string ACTIVATOR_FRONT_TRIGGER_GAMEOBJECT = "UIPointer_Activator_Front_Trigger";

        public virtual void OnUIPointerElementEnter(UIPointerEventArgs e)
        {
            if (UIPointerElementEnter != null)
            {
                UIPointerElementEnter(this, e);
            }
        }

        public virtual void OnUIPointerElementExit(UIPointerEventArgs e)
        {
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

        public UIPointerEventArgs SetUIPointerEvent(GameObject currentTarget, GameObject lastTarget = null)
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
        /// <returns>A custom event system input class that is used to detect input from VR pointers.</returns>
        public VRTK_EventSystemVRInput SetEventSystem(EventSystem eventSystem)
        {
            if (!eventSystem)
            {
                Debug.LogError("A VRTK_UIPointer requires an EventSystem");
                return null;
            }

            //disable existing standalone input module
            var standaloneInputModule = eventSystem.gameObject.GetComponent<StandaloneInputModule>();
            if (standaloneInputModule.enabled)
            {
                standaloneInputModule.enabled = false;
            }

            //if it doesn't already exist, add the custom event system
            var eventSystemInput = eventSystem.GetComponent<VRTK_EventSystemVRInput>();
            if (!eventSystemInput)
            {
                eventSystemInput = eventSystem.gameObject.AddComponent<VRTK_EventSystemVRInput>();
                eventSystemInput.Initialise();
            }

#if UNITY_5_5_OR_NEWER
            //if it doesn't already exist, add the custom non-pausing event system
            var nonPausingEventSystem = eventSystem.gameObject.GetComponent<VRTK_NonPausingEventSystem>();
            if (!nonPausingEventSystem && VRTK_NonPausingEventSystem.EventSystemPausesOnApplicationFocusLost(eventSystem))
            {
                eventSystem.enabled = false;
                nonPausingEventSystem = eventSystem.gameObject.AddComponent<VRTK_NonPausingEventSystem>();
                nonPausingEventSystem.CopyValuesFrom(eventSystem);
                VRTK_NonPausingEventSystem.SetEventSystemOfBaseInputModules(nonPausingEventSystem);
            }
#endif

            return eventSystemInput;
        }

        /// <summary>
        /// The RemoveEventSystem resets the Unity EventSystem back to the original state before the VRTK_EventSystemVRInput was swapped for it.
        /// </summary>
        public void RemoveEventSystem()
        {
            var eventSystem = FindObjectOfType<EventSystem>();

            if (!eventSystem)
            {
                Debug.LogError("A VRTK_UIPointer requires an EventSystem");
                return;
            }

#if UNITY_5_5_OR_NEWER
            //remove the custom non-pausing event system
            if (eventSystem is VRTK_NonPausingEventSystem)
            {
                var nonPausingEventSystem = eventSystem;
                eventSystem = eventSystem.gameObject.GetComponent<EventSystem>();

                nonPausingEventSystem.enabled = false;
                Destroy(nonPausingEventSystem);

                eventSystem.enabled = true;
                VRTK_NonPausingEventSystem.SetEventSystemOfBaseInputModules(eventSystem);
            }
#endif

            //re-enable existing standalone input module
            var standaloneInputModule = eventSystem.gameObject.GetComponent<StandaloneInputModule>();
            if (!standaloneInputModule.enabled)
            {
                standaloneInputModule.enabled = true;
            }

            //remove the custom event system
            var eventSystemInput = eventSystem.GetComponent<VRTK_EventSystemVRInput>();
            if (eventSystemInput)
            {
                Destroy(eventSystemInput);
            }
        }

        /// <summary>
        /// The PointerActive method determines if the ui pointer beam should be active based on whether the pointer alias is being held and whether the Hold Button To Use parameter is checked.
        /// </summary>
        /// <returns>Returns true if the ui pointer should be currently active.</returns>
        public bool PointerActive()
        {
            if (activationMode == ActivationMethods.Always_On || autoActivatingCanvas != null)
            {
                return true;
            }
            else if (activationMode == ActivationMethods.Hold_Button)
            {
                return controller.pointerPressed;
            }
            else
            {
                pointerClicked = false;
                if (controller.pointerPressed && !lastPointerPressState)
                {
                    pointerClicked = true;
                }
                lastPointerPressState = controller.pointerPressed;

                if (pointerClicked)
                {
                    beamEnabledState = !beamEnabledState;
                }

                return beamEnabledState;
            }
        }

        /// <summary>
        /// The ValidClick method determines if the UI Click button is in a valid state to register a click action.
        /// </summary>
        /// <param name="checkLastClick">If this is true then the last frame's state of the UI Click button is also checked to see if a valid click has happened.</param>
        /// <param name="lastClickState">This determines what the last frame's state of the UI Click button should be in for it to be a valid click.</param>
        /// <returns>Returns true if the UI Click button is in a valid state to action a click, returns false if it is not in a valid state.</returns>
        public bool ValidClick(bool checkLastClick, bool lastClickState = false)
        {
            var controllerClicked = (collisionClick ? collisionClick : controller.uiClickPressed);
            var result = (checkLastClick ? controllerClicked && lastPointerClickState == lastClickState : controllerClicked);
            lastPointerClickState = controllerClicked;

            return result;
        }

        /// <summary>
        /// The GetOriginPosition method returns the relevant transform position for the pointer based on whether the pointerOriginTransform variable is valid.
        /// </summary>
        /// <returns>A Vector3 of the pointer transform position</returns>
        public Vector3 GetOriginPosition()
        {
            return (pointerOriginTransform ? pointerOriginTransform.position : transform.position);
        }

        /// <summary>
        /// The GetOriginPosition method returns the relevant transform forward for the pointer based on whether the pointerOriginTransform variable is valid.
        /// </summary>
        /// <returns>A Vector3 of the pointer transform forward</returns>
        public Vector3 GetOriginForward()
        {
            return (pointerOriginTransform ? pointerOriginTransform.forward : transform.forward);
        }

        private void OnEnable()
        {
            pointerOriginTransform = (pointerOriginTransform == null ? VRTK_SDK_Bridge.GenerateControllerPointerOrigin() : pointerOriginTransform);

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

        private void OnDisable()
        {
            if (cachedEventSystemInput && cachedEventSystemInput.pointers.Contains(this))
            {
                cachedEventSystemInput.pointers.Remove(this);
            }
        }

        private void ConfigureEventSystem()
        {
            if (!cachedEventSystem)
            {
                cachedEventSystem = FindObjectOfType<EventSystem>();
            }

            if (!cachedEventSystemInput)
            {
                cachedEventSystemInput = SetEventSystem(cachedEventSystem);
            }

            if (cachedEventSystem && cachedEventSystemInput)
            {
                if (pointerEventData == null)
                {
                    pointerEventData = new PointerEventData(cachedEventSystem);
                }

                StartCoroutine(WaitForPointerId());

                if (!cachedEventSystemInput.pointers.Contains(this))
                {
                    cachedEventSystemInput.pointers.Add(this);
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

#if UNITY_5_5_OR_NEWER
        private sealed class VRTK_NonPausingEventSystem : EventSystem
        {
            private const BindingFlags EVENT_SYSTEM_BINDING_FLAGS_PUBLIC = BindingFlags.Instance | BindingFlags.Public;
            private const BindingFlags EVENT_SYSTEM_BINDING_FLAGS_ALL_ACCESS = EVENT_SYSTEM_BINDING_FLAGS_PUBLIC | BindingFlags.NonPublic;
            private static readonly FieldInfo BASE_INPUT_MODULE_EVENT_SYSTEM_FIELD = typeof(BaseInputModule).GetField("m_EventSystem", BindingFlags.Instance | BindingFlags.NonPublic);
            private static readonly FieldInfo[] EVENT_SYSTEM_FIELD_INFOS = typeof(EventSystem).GetFields(EVENT_SYSTEM_BINDING_FLAGS_PUBLIC);
            private static readonly PropertyInfo[] EVENT_SYSTEM_PROPERTY_INFOS = typeof(EventSystem).GetProperties(EVENT_SYSTEM_BINDING_FLAGS_PUBLIC).Except(new[] { typeof(EventSystem).GetProperty("enabled") }).ToArray();

            public static bool EventSystemPausesOnApplicationFocusLost(EventSystem eventSystem)
            {
                return eventSystem.GetType().GetMethod("OnApplicationFocus", EVENT_SYSTEM_BINDING_FLAGS_ALL_ACCESS) != null
                       && eventSystem.GetType().GetField("m_Paused", EVENT_SYSTEM_BINDING_FLAGS_ALL_ACCESS) != null;
            }

            public static void SetEventSystemOfBaseInputModules(EventSystem eventSystem)
            {
                //BaseInputModule has a private field referencing the current EventSystem
                //this field is set in BaseInputModule.OnEnable only
                //it's used in BaseInputModule.OnEnable and BaseInputModule.OnDisable to call EventSystem.UpdateModules
                //this means we could just disable and enable every enabled BaseInputModule to fix that reference
                //
                //but the StandaloneInputModule (which is added by default when adding an EventSystem in the Editor) requires EventSystem
                //which means we can't correctly destroy the old EventSystem first and then add our own one
                //we also want to leave the existing EventSystem as is, so it can be used again whenever VRTK_UIPointer.RemoveEventSystem is called
                var baseInputModules = FindObjectsOfType<BaseInputModule>();
                for (var index = 0; index < baseInputModules.Length; index++)
                {
                    BASE_INPUT_MODULE_EVENT_SYSTEM_FIELD.SetValue(baseInputModules[index], eventSystem);
                }
                eventSystem.UpdateModules();
            }

            public void CopyValuesFrom(EventSystem eventSystem)
            {
                for (var index = 0; index < EVENT_SYSTEM_FIELD_INFOS.Length; index++)
                {
                    var fieldInfo = EVENT_SYSTEM_FIELD_INFOS[index];
                    fieldInfo.SetValue(this, fieldInfo.GetValue(eventSystem));
                }

                for (var index = 0; index < EVENT_SYSTEM_PROPERTY_INFOS.Length; index++)
                {
                    var propertyInfo = EVENT_SYSTEM_PROPERTY_INFOS[index];
                    if (propertyInfo.CanWrite)
                    {
                        propertyInfo.SetValue(this, propertyInfo.GetValue(eventSystem, null), null);
                    }
                }
            }

            protected override void OnApplicationFocus(bool hasFocus)
            {
                //don't call base implementation because it will set a pause flag for this EventSystem
            }
        }
#endif
    }
}