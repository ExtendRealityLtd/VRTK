// UI Pointer|Scripts|0060
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

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
    /// The UI Pointer provides a mechanism for interacting with Unity UI elements on a world canvas. The UI Pointer can be attached to any game object the same way in which a World Pointer can be and the UI Pointer also requires a controller to initiate the pointer activation and pointer click states.
    /// </summary>
    /// <remarks>
    /// It's possible to prevent a world canvas from being interactable with a UI Pointer by setting a tag or applying a class to the canvas and then entering the tag or class name for the UI Pointer to ignore on the UI Pointer inspector parameters.
    ///
    /// The simplest way to use the UI Pointer is to attach the script to a game controller within the `[CameraRig]` along with a Simple Pointer as this provides visual feedback as to where the UI ray is pointing.
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

        [Tooltip("The controller that will be used to toggle the pointer. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_ControllerEvents controller;
        [Tooltip("Determines when the UI pointer should be active.")]
        public ActivationMethods activationMode = ActivationMethods.Hold_Button;
        [Tooltip("Determines whether the UI click action should be triggered when the pointer is deactivated. If the pointer is hovering over a clickable element then it will invoke the click action on that element.")]
        public bool attemptClickOnDeactivate = false;
        [Tooltip("A string that specifies a canvas Tag or the name of a Script attached to a canvas and denotes that any world canvases that contain this tag or script will be ignored by the UI Pointer.")]
        public string ignoreCanvasWithTagOrClass;
        [Tooltip("A specified VRTK_TagOrScriptPolicyList to use to determine whether any world canvases will be acted upon by the UI Pointer. If a list is provided then the 'Ignore Canvas With Tag Or Class' parameter will be ignored.")]
        public VRTK_TagOrScriptPolicyList canvasTagOrScriptListPolicy;

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

        private bool pointerClicked = false;
        private bool beamEnabledState = false;
        private bool lastPointerPressState = false;

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

            return eventSystemInput;
        }

        /// <summary>
        /// The SetWorldCanvas method is used to initialise a `WorldSpace` canvas for use with the UI Pointer. This method is called automatically on start for all editor created canvases but would need to be manually called if a canvas was generated at runtime.
        /// </summary>
        /// <param name="canvas">The canvas object to initialise for use with the UI pointers. Must be of type `WorldSpace`.</param>
        public void SetWorldCanvas(Canvas canvas)
        {
            if (canvas.renderMode != RenderMode.WorldSpace || Utilities.TagOrScriptCheck(canvas.gameObject, canvasTagOrScriptListPolicy, ignoreCanvasWithTagOrClass))
            {
                return;
            }

            //copy public params then disable existing graphic raycaster
            var defaultRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
            var customRaycaster = canvas.gameObject.GetComponent<VRTK_UIGraphicRaycaster>();
            //if it doesn't already exist, add the custom raycaster
            if (!customRaycaster)
            {
                customRaycaster = canvas.gameObject.AddComponent<VRTK_UIGraphicRaycaster>();
            }

            if (defaultRaycaster && defaultRaycaster.enabled)
            {
                customRaycaster.ignoreReversedGraphics = defaultRaycaster.ignoreReversedGraphics;
                customRaycaster.blockingObjects = defaultRaycaster.blockingObjects;
                defaultRaycaster.enabled = false;
            }

            //add a box collider and background image to ensure the rays always hit
            var canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
            if (!canvas.gameObject.GetComponent<BoxCollider>())
            {
                var canvasBoxCollider = canvas.gameObject.AddComponent<BoxCollider>();
                canvasBoxCollider.size = new Vector3(canvasSize.x, canvasSize.y, 10f);
                canvasBoxCollider.center = new Vector3(0f, 0f, 5f);
            }

            if (!canvas.gameObject.GetComponent<Image>())
            {
                canvas.gameObject.AddComponent<Image>().color = Color.clear;
            }
        }

        /// <summary>
        /// The PointerActive method determines if the ui pointer beam should be active based on whether the pointer alias is being held and whether the Hold Button To Use parameter is checked.
        /// </summary>
        /// <returns>Returns true if the ui pointer should be currently active.</returns>
        public bool PointerActive()
        {
            if (activationMode == ActivationMethods.Always_On)
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

        private void Start()
        {
            if (controller == null)
            {
                controller = GetComponent<VRTK_ControllerEvents>();
            }
            ConfigureEventSystem();
            ConfigureWorldCanvases();
            pointerClicked = false;
            lastPointerPressState = false;
            beamEnabledState = false;
            controllerRenderModel = VRTK_SDK_Bridge.GetControllerRenderModel(controller.gameObject);
        }

        private void ConfigureEventSystem()
        {
            var eventSystem = FindObjectOfType<EventSystem>();
            var eventSystemInput = SetEventSystem(eventSystem);

            pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.pointerId = (int)VRTK_SDK_Bridge.GetIndexOfTrackedObject(controller.gameObject) + 1000;
            eventSystemInput.pointers.Add(this);
        }

        private void ConfigureWorldCanvases()
        {
            foreach (var canvas in FindObjectsOfType<Canvas>())
            {
                SetWorldCanvas(canvas);
            }
        }
    }
}
