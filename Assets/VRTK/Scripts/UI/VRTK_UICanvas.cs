// UI Canvas|Scripts|0063
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    /// <summary>
    /// The UI Canvas is used to denote which World Canvases are interactable by a UI Pointer.
    /// </summary>
    /// <remarks>
    /// When the script is enabled it will disable the `Graphic Raycaster` on the canvas and create a custom `UI Graphics Raycaster` and the Blocking Objects and Blocking Mask settings are copied over from the `Graphic Raycaster`.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/034_Controls_InteractingWithUnityUI` uses the `VRTK_UICanvas` script on two of the canvases to show how the UI Pointer can interact with them.
    /// </example>
    public class VRTK_UICanvas : MonoBehaviour
    {
        [Tooltip("Determines if a UI Click action should happen when a UI Pointer game object collides with this canvas.")]
        public bool clickOnPointerCollision = false;
        [Tooltip("Determines if a UI Pointer will be auto activated if a UI Pointer game object comes within the given distance of this canvas. If a value of `0` is given then no auto activation will occur.")]
        public float autoActivateWithinDistance = 0f;

        private const string CANVAS_DRAGGABLE_PANEL = "VRTK_UICANVAS_DRAGGABLE_PANEL";
        private const string ACTIVATOR_FRONT_TRIGGER_GAMEOBJECT = "VRTK_UICANVAS_ACTIVATOR_FRONT_TRIGGER";

        private void OnEnable()
        {
            SetupCanvas();
        }

        private void OnDisable()
        {
            RemoveCanvas();
        }

        private void OnDestroy()
        {
            RemoveCanvas();
        }

        private void SetupCanvas()
        {
            var canvas = GetComponent<Canvas>();

            if (!canvas || canvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.LogError("A VRTK_UICanvas requires to be placed on a Canvas that is set to `Render Mode = World Space`.");
                return;
            }

            var canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
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
            if (!canvas.gameObject.GetComponent<BoxCollider>())
            {
                var canvasBoxCollider = canvas.gameObject.AddComponent<BoxCollider>();
                canvasBoxCollider.size = new Vector3(canvasSize.x, canvasSize.y, 10f);
                canvasBoxCollider.center = new Vector3(0f, 0f, 5f);
                canvasBoxCollider.isTrigger = true;
            }

            var canvasRigidBody = canvas.gameObject.GetComponent<Rigidbody>();
            if (!canvasRigidBody)
            {
                canvas.gameObject.AddComponent<Rigidbody>().isKinematic = true;
            }

            CreateDraggablePanel(canvas, canvasSize);
            CreateActivator(canvas, canvasSize);
        }

        private void CreateDraggablePanel(Canvas canvas, Vector2 canvasSize)
        {
            if (canvas && !canvas.transform.FindChild(CANVAS_DRAGGABLE_PANEL))
            {
                var draggablePanel = new GameObject(CANVAS_DRAGGABLE_PANEL);
                draggablePanel.transform.SetParent(canvas.transform);
                draggablePanel.transform.localPosition = Vector3.zero;
                draggablePanel.transform.localRotation = Quaternion.identity;
                draggablePanel.transform.localScale = Vector3.one;
                draggablePanel.transform.SetAsFirstSibling();
                draggablePanel.AddComponent<RectTransform>();
                draggablePanel.AddComponent<Image>().color = Color.clear;
                draggablePanel.AddComponent<EventTrigger>();

                draggablePanel.GetComponent<RectTransform>().sizeDelta = canvasSize;
            }
        }

        private void CreateActivator(Canvas canvas, Vector2 canvasSize)
        {
            //if autoActivateWithinDistance is greater than 0 then create the front collider sub object
            if (autoActivateWithinDistance > 0f && canvas && !canvas.transform.FindChild(ACTIVATOR_FRONT_TRIGGER_GAMEOBJECT))
            {
                var frontTrigger = new GameObject(ACTIVATOR_FRONT_TRIGGER_GAMEOBJECT);
                frontTrigger.transform.SetParent(canvas.transform);
                frontTrigger.transform.SetAsFirstSibling();
                frontTrigger.transform.localPosition = Vector3.zero;
                frontTrigger.transform.localRotation = Quaternion.identity;
                frontTrigger.transform.localScale = Vector3.one;

                var actualActivationDistance = autoActivateWithinDistance * 10f;
                var frontTriggerBoxCollider = frontTrigger.AddComponent<BoxCollider>();
                frontTriggerBoxCollider.isTrigger = true;
                frontTriggerBoxCollider.size = new Vector3(canvasSize.x, canvasSize.y, actualActivationDistance);
                frontTriggerBoxCollider.center = new Vector3(0f, 0f, -(actualActivationDistance / 2));

                frontTrigger.AddComponent<Rigidbody>().isKinematic = true;
                frontTrigger.AddComponent<VRTK_UIPointerAutoActivator>();
                frontTrigger.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }

        private void RemoveCanvas()
        {
            var canvas = GetComponent<Canvas>();

            if (!canvas)
            {
                return;
            }

            var defaultRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
            var customRaycaster = canvas.gameObject.GetComponent<VRTK_UIGraphicRaycaster>();
            //if a custom raycaster exists then remove it
            if (customRaycaster)
            {
                Destroy(customRaycaster);
            }

            //If the default raycaster is disabled, then re-enable it
            if (defaultRaycaster && !defaultRaycaster.enabled)
            {
                defaultRaycaster.enabled = true;
            }

            //Check if there is a collider and remove it if there is
            var canvasBoxCollider = canvas.gameObject.GetComponent<BoxCollider>();
            if (canvasBoxCollider)
            {
                Destroy(canvasBoxCollider);
            }

            var canvasRigidBody = canvas.gameObject.GetComponent<Rigidbody>();
            if (canvasRigidBody)
            {
                Destroy(canvasRigidBody);
            }

            var draggablePanel = canvas.transform.FindChild(CANVAS_DRAGGABLE_PANEL);
            if (draggablePanel)
            {
                Destroy(draggablePanel.gameObject);
            }

            var frontTrigger = canvas.transform.FindChild(ACTIVATOR_FRONT_TRIGGER_GAMEOBJECT);
            if (frontTrigger)
            {
                Destroy(frontTrigger.gameObject);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            var colliderCheck = collider.GetComponentInParent<VRTK_PlayerObject>();
            var pointerCheck = collider.GetComponentInParent<VRTK_UIPointer>();
            if (pointerCheck && colliderCheck && colliderCheck.objectType == VRTK_PlayerObject.ObjectTypes.Collider)
            {
                pointerCheck.collisionClick = (clickOnPointerCollision ? true : false);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            var pointerCheck = collider.GetComponentInParent<VRTK_UIPointer>();
            if (pointerCheck)
            {
                pointerCheck.collisionClick = false;
            }
        }
    }

    public class VRTK_UIPointerAutoActivator : MonoBehaviour
    {
        private void OnTriggerEnter(Collider collider)
        {
            var colliderCheck = collider.GetComponentInParent<VRTK_PlayerObject>();
            var pointerCheck = collider.GetComponentInParent<VRTK_UIPointer>();
            if (pointerCheck && colliderCheck && colliderCheck.objectType == VRTK_PlayerObject.ObjectTypes.Collider)
            {
                pointerCheck.autoActivatingCanvas = gameObject;
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            var pointerCheck = collider.GetComponentInParent<VRTK_UIPointer>();
            if (pointerCheck && pointerCheck.autoActivatingCanvas == gameObject)
            {
                pointerCheck.autoActivatingCanvas = null;
            }
        }
    }
}