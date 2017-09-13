// UI Drop Zone|UI|80040
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// Specifies a Unity UI Element as being a valid drop zone location for a UI Draggable element.
    /// </summary>
    /// <remarks>
    ///   > It's appropriate to use a Panel UI element as a drop zone with a layout group applied so new children dropped into the drop zone automatically align.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_UIDropZone` script on the Unity UI element that is to become the drop zone.
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
            if (eventData.pointerDrag != null)
            {
                VRTK_UIDraggableItem dragItem = eventData.pointerDrag.GetComponent<VRTK_UIDraggableItem>();
                if (dragItem != null && dragItem.restrictToDropZone)
                {
                    dragItem.validDropZone = gameObject;
                    droppableItem = dragItem;
                }
            }
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (droppableItem != null)
            {
                droppableItem.validDropZone = null;
            }
            droppableItem = null;
        }
    }
}