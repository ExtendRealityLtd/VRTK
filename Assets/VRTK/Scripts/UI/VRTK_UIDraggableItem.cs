// UI Draggable Item|UI|80030
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// The UI Draggable item will make any UI element draggable on the canvas.
    /// </summary>
    /// <remarks>
    /// If a UI Draggable item is set to `Restrict To Drop Zone = true` then the UI Draggable item must be a child of an element that has the VRTK_UIDropZone script applied to it to ensure it starts in a valid drop zone.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/034_Controls_InteractingWithUnityUI` demonstrates a collection of UI elements that are draggable
    /// </example>
    [RequireComponent(typeof(CanvasGroup))]
    public class VRTK_UIDraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Tooltip("If checked then the UI element can only be dropped in valid a VRTK_UIDropZone object and must start as a child of a VRTK_UIDropZone object. If unchecked then the UI element can be dropped anywhere on the canvas.")]
        public bool restrictToDropZone = false;
        [Tooltip("If checked then the UI element can only be dropped on the original parent canvas. If unchecked the UI element can be dropped on any valid VRTK_UICanvas.")]
        public bool restrictToOriginalCanvas = false;
        [Tooltip("The offset to bring the UI element forward when it is being dragged.")]
        public float forwardOffset = 0.1f;

        /// <summary>
        /// The current valid drop zone the dragged element is hovering over.
        /// </summary>
        [HideInInspector]
        public GameObject validDropZone;

        protected RectTransform dragTransform;
        protected Vector3 startPosition;
        protected Quaternion startRotation;
        protected GameObject startDropZone;
        protected Transform startParent;
        protected Canvas startCanvas;
        protected CanvasGroup canvasGroup;

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
            startParent = transform.parent;
            startCanvas = GetComponentInParent<Canvas>();
            canvasGroup.blocksRaycasts = false;

            if (restrictToDropZone)
            {
                startDropZone = GetComponentInParent<VRTK_UIDropZone>().gameObject;
                validDropZone = startDropZone;
            }

            SetDragPosition(eventData);
            var pointer = GetPointer(eventData);
            if (pointer)
            {
                pointer.OnUIPointerElementDragStart(pointer.SetUIPointerEvent(pointer.pointerEventData.pointerPressRaycast, gameObject));
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            SetDragPosition(eventData);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;
            dragTransform = null;
            transform.position += (transform.forward * forwardOffset);
            var validDragEnd = true;
            if (restrictToDropZone)
            {
                if (validDropZone && validDropZone != startDropZone)
                {
                    transform.SetParent(validDropZone.transform);
                }
                else
                {
                    ResetElement();
                    validDragEnd = false;
                }
            }

            var destinationCanvas = (eventData.pointerEnter ? eventData.pointerEnter.GetComponentInParent<Canvas>() : null);
            if (restrictToOriginalCanvas)
            {
                if (destinationCanvas && destinationCanvas != startCanvas)
                {
                    ResetElement();
                    validDragEnd = false;
                }
            }

            if (destinationCanvas == null)
            {
                //We've been dropped off of a canvas
                ResetElement();
                validDragEnd = false;
            }

            if (validDragEnd)
            {
                var pointer = GetPointer(eventData);
                if (pointer)
                {
                    pointer.OnUIPointerElementDragEnd(pointer.SetUIPointerEvent(pointer.pointerEventData.pointerPressRaycast, gameObject));
                }
            }

            validDropZone = null;
            startParent = null;
            startCanvas = null;
        }

        protected virtual void OnEnable()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (restrictToDropZone && !GetComponentInParent<VRTK_UIDropZone>())
            {
                enabled = false;
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_UIDraggableItem", "VRTK_UIDropZone", "the parent", " if `freeDrop = false`"));
            }
        }

        protected virtual VRTK_UIPointer GetPointer(PointerEventData eventData)
        {
            var controller = VRTK_DeviceFinder.GetControllerByIndex((uint)eventData.pointerId, false);
            return (controller ? controller.GetComponent<VRTK_UIPointer>() : null);
        }

        protected virtual void SetDragPosition(PointerEventData eventData)
        {
            if (eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null)
            {
                dragTransform = eventData.pointerEnter.transform as RectTransform;
            }

            Vector3 pointerPosition;
            if (dragTransform && RectTransformUtility.ScreenPointToWorldPointInRectangle(dragTransform, eventData.position, eventData.pressEventCamera, out pointerPosition))
            {
                transform.position = pointerPosition - (transform.forward * forwardOffset);
                transform.rotation = dragTransform.rotation;
            }
        }

        protected virtual void ResetElement()
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
            transform.SetParent(startParent);
        }
    }
}