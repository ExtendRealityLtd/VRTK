// UI Draggable Item|UI|80030
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="target">The target the item is dragged onto.</param>
    public struct UIDraggableItemEventArgs
    {
        public GameObject target;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="UIDraggableItemEventArgs"/></param>
    public delegate void UIDraggableItemEventHandler(object sender, UIDraggableItemEventArgs e);

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
    [AddComponentMenu("VRTK/Scripts/UI/VRTK_UIDraggableItem")]
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

        /// <summary>
        /// Emitted when the draggable item is successfully dropped.
        /// </summary>
        public event UIDraggableItemEventHandler DraggableItemDropped;
        /// <summary>
        /// Emitted when the draggable item is reset.
        /// </summary>
        public event UIDraggableItemEventHandler DraggableItemReset;

        protected RectTransform dragTransform;
        protected Vector3 startPosition;
        protected Quaternion startRotation;
        protected GameObject startDropZone;
        protected Transform startParent;
        protected Canvas startCanvas;
        protected CanvasGroup canvasGroup;

        public virtual void OnDraggableItemDropped(UIDraggableItemEventArgs e)
        {
            if (DraggableItemDropped != null)
            {
                DraggableItemDropped(this, e);
            }
        }

        public virtual void OnDraggableItemReset(UIDraggableItemEventArgs e)
        {
            if (DraggableItemReset != null)
            {
                DraggableItemReset(this, e);
            }
        }

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
            VRTK_UIPointer pointer = GetPointer(eventData);
            if (pointer != null)
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
            bool validDragEnd = true;
            if (restrictToDropZone)
            {
                if (validDropZone != null && validDropZone != startDropZone)
                {
                    transform.SetParent(validDropZone.transform);
                }
                else
                {
                    ResetElement();
                    validDragEnd = false;
                }
            }

            Canvas destinationCanvas = (eventData.pointerEnter != null ? eventData.pointerEnter.GetComponentInParent<Canvas>() : null);
            if (restrictToOriginalCanvas)
            {
                if (destinationCanvas != null && destinationCanvas != startCanvas)
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
                VRTK_UIPointer pointer = GetPointer(eventData);
                if (pointer != null)
                {
                    pointer.OnUIPointerElementDragEnd(pointer.SetUIPointerEvent(pointer.pointerEventData.pointerPressRaycast, gameObject));
                }
                OnDraggableItemDropped(SetEventPayload(validDropZone));
            }

            validDropZone = null;
            startParent = null;
            startCanvas = null;
        }

        protected virtual void OnEnable()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (restrictToDropZone && GetComponentInParent<VRTK_UIDropZone>() == null)
            {
                enabled = false;
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_UIDraggableItem", "VRTK_UIDropZone", "the parent", " if `freeDrop = false`"));
            }
        }

        protected virtual VRTK_UIPointer GetPointer(PointerEventData eventData)
        {
            GameObject controller = VRTK_DeviceFinder.GetControllerByIndex((uint)eventData.pointerId, false);
            return (controller != null ? controller.GetComponent<VRTK_UIPointer>() : null);
        }

        protected virtual void SetDragPosition(PointerEventData eventData)
        {
            if (eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null)
            {
                dragTransform = eventData.pointerEnter.transform as RectTransform;
            }

            Vector3 pointerPosition;
            if (dragTransform != null && RectTransformUtility.ScreenPointToWorldPointInRectangle(dragTransform, eventData.position, eventData.pressEventCamera, out pointerPosition))
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
            OnDraggableItemReset(SetEventPayload(startParent.gameObject));
        }

        protected virtual UIDraggableItemEventArgs SetEventPayload(GameObject target)
        {
            UIDraggableItemEventArgs e;
            e.target = target;
            return e;
        }
    }
}