// UI Drop Zone|UI|80040
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// A UI Drop Zone is applied to any UI element that is to be considered a valid parent for any UI Draggable element to be dropped into it.
    /// </summary>
    /// <remarks>
    /// It's usually appropriate to use a Panel UI element as a drop zone with a layout group applied so new children dropped into the drop zone automatically align.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/034_Controls_InteractingWithUnityUI` demonstrates a collection of UI Drop Zones.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/UI/VRTK_UIDropZone")]
    public class VRTK_UIDropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected VRTK_UIDraggableItem droppableItem;

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag)
            {
                var dragItem = eventData.pointerDrag.GetComponent<VRTK_UIDraggableItem>();
                if (dragItem && dragItem.restrictToDropZone)
                {
                    dragItem.validDropZone = gameObject;
                    droppableItem = dragItem;
                }
            }
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (droppableItem)
            {
                droppableItem.validDropZone = null;
            }
            droppableItem = null;
        }
    }
}