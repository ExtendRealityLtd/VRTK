// UI Canvas|UI|80010
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;

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

        protected BoxCollider canvasBoxCollider;
        protected Rigidbody canvasRigidBody;
        protected Coroutine draggablePanelCreation;
        protected const string CANVAS_DRAGGABLE_PANEL = "VRTK_UICANVAS_DRAGGABLE_PANEL";
        protected const string ACTIVATOR_FRONT_TRIGGER_GAMEOBJECT = "VRTK_UICANVAS_ACTIVATOR_FRONT_TRIGGER";

        protected virtual void OnEnable()
        {
            SetupCanvas();
        }

        protected virtual void OnDisable()
        {
            RemoveCanvas();
        }

        protected virtual void OnDestroy()
        {
            RemoveCanvas();
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            var colliderCheck = collider.GetComponentInParent<VRTK_PlayerObject>();
            var pointerCheck = collider.GetComponentInParent<VRTK_UIPointer>();
            if (pointerCheck && colliderCheck && colliderCheck.objectType == VRTK_PlayerObject.ObjectTypes.Collider)
            {
                pointerCheck.collisionClick = (clickOnPointerCollision ? true : false);
            }
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            var pointerCheck = collider.GetComponentInParent<VRTK_UIPointer>();
            if (pointerCheck)
            {
                pointerCheck.collisionClick = false;
            }
        }

        protected virtual void SetupCanvas()
        {
            var canvas = GetComponent<Canvas>();

            if (!canvas || canvas.renderMode != RenderMode.WorldSpace)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_UICanvas", "Canvas", "the same", " that is set to `Render Mode = World Space`"));
                return;
            }

            var canvasRectTransform = canvas.GetComponent<RectTransform>();
            var canvasSize = canvasRectTransform.sizeDelta;
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
                Vector2 pivot = canvasRectTransform.pivot;
                float zSize = 0.1f;
                float zScale = zSize / canvasRectTransform.localScale.z;

                canvasBoxCollider = canvas.gameObject.AddComponent<BoxCollider>();
                canvasBoxCollider.size = new Vector3(canvasSize.x, canvasSize.y, zScale);
                canvasBoxCollider.center = new Vector3(canvasSize.x / 2 - canvasSize.x * pivot.x, canvasSize.y / 2 - canvasSize.y * pivot.y, zScale / 2f);
                canvasBoxCollider.isTrigger = true;
            }

            if (!canvas.gameObject.GetComponent<Rigidbody>())
            {
                canvasRigidBody = canvas.gameObject.AddComponent<Rigidbody>();
                canvasRigidBody.isKinematic = true;
            }

            draggablePanelCreation = StartCoroutine(CreateDraggablePanel(canvas, canvasSize));
            CreateActivator(canvas, canvasSize);
        }

        protected virtual IEnumerator CreateDraggablePanel(Canvas canvas, Vector2 canvasSize)
        {
            if (canvas && !canvas.transform.FindChild(CANVAS_DRAGGABLE_PANEL))
            {
                yield return null;

                var draggablePanel = new GameObject(CANVAS_DRAGGABLE_PANEL, typeof(RectTransform));
                draggablePanel.AddComponent<LayoutElement>().ignoreLayout = true;
                draggablePanel.AddComponent<Image>().color = Color.clear;
                draggablePanel.AddComponent<EventTrigger>();
                draggablePanel.transform.SetParent(canvas.transform);
                draggablePanel.transform.localPosition = Vector3.zero;
                draggablePanel.transform.localRotation = Quaternion.identity;
                draggablePanel.transform.localScale = Vector3.one;
                draggablePanel.transform.SetAsFirstSibling();

                draggablePanel.GetComponent<RectTransform>().sizeDelta = canvasSize;
            }
        }

        protected virtual void CreateActivator(Canvas canvas, Vector2 canvasSize)
        {
            //if autoActivateWithinDistance is greater than 0 then create the front collider sub object
            if (autoActivateWithinDistance > 0f && canvas && !canvas.transform.FindChild(ACTIVATOR_FRONT_TRIGGER_GAMEOBJECT))
            {
                var canvasRectTransform = canvas.GetComponent<RectTransform>();
                Vector2 pivot = canvasRectTransform.pivot;

                var frontTrigger = new GameObject(ACTIVATOR_FRONT_TRIGGER_GAMEOBJECT);
                frontTrigger.transform.SetParent(canvas.transform);
                frontTrigger.transform.SetAsFirstSibling();
                frontTrigger.transform.localPosition = new Vector3(canvasSize.x / 2 - canvasSize.x * pivot.x, canvasSize.y / 2 - canvasSize.y * pivot.y);
                frontTrigger.transform.localRotation = Quaternion.identity;
                frontTrigger.transform.localScale = Vector3.one;

                var actualActivationDistance = autoActivateWithinDistance / canvasRectTransform.localScale.z;
                var frontTriggerBoxCollider = frontTrigger.AddComponent<BoxCollider>();
                frontTriggerBoxCollider.isTrigger = true;
                frontTriggerBoxCollider.size = new Vector3(canvasSize.x, canvasSize.y, actualActivationDistance);
                frontTriggerBoxCollider.center = new Vector3(0f, 0f, -(actualActivationDistance / 2));

                frontTrigger.AddComponent<Rigidbody>().isKinematic = true;
                frontTrigger.AddComponent<VRTK_UIPointerAutoActivator>();
                frontTrigger.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }

        protected virtual void RemoveCanvas()
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
            if (canvasBoxCollider)
            {
                Destroy(canvasBoxCollider);
            }

            if (canvasRigidBody)
            {
                Destroy(canvasRigidBody);
            }

            StopCoroutine(draggablePanelCreation);
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
    }

    public class VRTK_UIPointerAutoActivator : MonoBehaviour
    {
        protected virtual void OnTriggerEnter(Collider collider)
        {
            var colliderCheck = collider.GetComponentInParent<VRTK_PlayerObject>();
            var pointerCheck = collider.GetComponentInParent<VRTK_UIPointer>();
            if (pointerCheck && colliderCheck && colliderCheck.objectType == VRTK_PlayerObject.ObjectTypes.Collider)
            {
                pointerCheck.autoActivatingCanvas = gameObject;
            }
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            var pointerCheck = collider.GetComponentInParent<VRTK_UIPointer>();
            if (pointerCheck && pointerCheck.autoActivatingCanvas == gameObject)
            {
                pointerCheck.autoActivatingCanvas = null;
            }
        }
    }
}