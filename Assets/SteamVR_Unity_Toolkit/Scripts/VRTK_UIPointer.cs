namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    public struct UIPointerEventArgs
    {
        public uint controllerIndex;
        public GameObject currentTarget;
        public GameObject previousTarget;
    }

    public delegate void UIPointerEventHandler(object sender, UIPointerEventArgs e);

    public class VRTK_UIPointer : MonoBehaviour
    {
        public enum ActivationMethods
        {
            Hold_Button,
            Toggle_Button,
            Always_On
        }
        public VRTK_ControllerEvents controller;
        public string ignoreCanvasWithTagOrClass;
        public ActivationMethods activationMode = ActivationMethods.Hold_Button;

        [HideInInspector]
        public PointerEventData pointerEventData;
        [HideInInspector]
        public GameObject hoveringElement;
        [HideInInspector]
        public SteamVR_RenderModel controllerRenderModel;

        public event UIPointerEventHandler UIPointerElementEnter;
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
            }
        }

        public UIPointerEventArgs SetUIPointerEvent(GameObject currentTarget, GameObject lastTarget = null)
        {
            UIPointerEventArgs e;
            e.controllerIndex = VRTK_DeviceFinder.GetControllerIndex(controller.gameObject);
            e.currentTarget = currentTarget;
            e.previousTarget = lastTarget;
            return e;
        }

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

        public void SetWorldCanvas(Canvas canvas)
        {
            if (canvas.renderMode != RenderMode.WorldSpace || canvas.gameObject.tag == ignoreCanvasWithTagOrClass || canvas.GetComponent(ignoreCanvasWithTagOrClass) != null)
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

        public bool PointerActive()
        {
            if(activationMode == ActivationMethods.Always_On)
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

                if(pointerClicked)
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
            controllerRenderModel = (controller.GetComponent<SteamVR_RenderModel>() ? controller.GetComponent<SteamVR_RenderModel>() : controller.GetComponentInChildren<SteamVR_RenderModel>());
        }

        private void ConfigureEventSystem()
        {
            var eventSystem = FindObjectOfType<EventSystem>();
            var eventSystemInput = SetEventSystem(eventSystem);

            pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.pointerId = (int)controller.gameObject.GetComponent<SteamVR_TrackedObject>().index + 1000;
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
