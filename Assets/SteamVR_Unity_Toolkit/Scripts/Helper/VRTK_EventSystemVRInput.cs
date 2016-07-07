namespace VRTK
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using System.Collections.Generic;

    public class VRTK_EventSystemVRInput : PointerInputModule
    {
        public List<VRTK_UIPointer> pointers;

        public void Initialise()
        {
            pointers = new List<VRTK_UIPointer>();
        }

        public override void Process()
        {
            foreach (var pointer in pointers)
            {
                if (pointer.gameObject.activeInHierarchy)
                {
                    List<RaycastResult> results = new List<RaycastResult>();
                    if (pointer.controller.pointerPressed)
                    {
                        results = CheckRaycasts(pointer);
                    }
                    //Process events
                    Hover(pointer, results);
                    Click(pointer, results);
                    Drag(pointer, results);
                }
            }
        }

        private List<RaycastResult> CheckRaycasts(VRTK_UIPointer pointer)
        {
            var raycastResult = new RaycastResult();
            raycastResult.worldPosition = pointer.transform.position;
            raycastResult.worldNormal = pointer.transform.forward;

            pointer.pointerEventData.pointerCurrentRaycast = raycastResult;

            List<RaycastResult> raycasts = new List<RaycastResult>();
            eventSystem.RaycastAll(pointer.pointerEventData, raycasts);
            return raycasts;
        }

        private bool CheckTransformTree(Transform target, Transform source)
        {
            if (target == null)
            {
                return false;
            }

            if (target.Equals(source))
            {
                return true;
            }

            return CheckTransformTree(target.transform.parent, source);
        }

        private bool NoValidCollision(VRTK_UIPointer pointer, List<RaycastResult> results)
        {
            return (results.Count == 0 || !CheckTransformTree(results[0].gameObject.transform, pointer.pointerEventData.pointerEnter.transform));
        }

        private bool IsHovering(VRTK_UIPointer pointer)
        {
            foreach (var hoveredObject in pointer.pointerEventData.hovered)
            {
                if (pointer.pointerEventData.pointerEnter && CheckTransformTree(hoveredObject.transform, pointer.pointerEventData.pointerEnter.transform))
                {
                    return true;
                }
            }
            return false;
        }

        private void Hover(VRTK_UIPointer pointer, List<RaycastResult> results)
        {
            if (pointer.pointerEventData.pointerEnter)
            {
                if (NoValidCollision(pointer, results))
                {
                    ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerEnter, pointer.pointerEventData, ExecuteEvents.pointerExitHandler);
                    pointer.pointerEventData.pointerEnter = null;
                }
            }
            else
            {
                foreach (var result in results)
                {
                    var target = ExecuteEvents.ExecuteHierarchy(result.gameObject, pointer.pointerEventData, ExecuteEvents.pointerEnterHandler);
                    if (target != null)
                    {
                        var selectable = target.GetComponent<Selectable>();
                        if (selectable)
                        {
                            var noNavigation = new Navigation();
                            noNavigation.mode = Navigation.Mode.None;
                            selectable.navigation = noNavigation;
                        }

                        pointer.pointerEventData.pointerCurrentRaycast = result;
                        pointer.pointerEventData.pointerEnter = target;
                        break;
                    }
                }
            }
        }

        private void Click(VRTK_UIPointer pointer, List<RaycastResult> results)
        {
            pointer.pointerEventData.eligibleForClick = pointer.controller.usePressed;

            if (pointer.pointerEventData.pointerPress)
            {
                if (pointer.pointerEventData.eligibleForClick)
                {
                    if (!IsHovering(pointer))
                    {
                        ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerPress, pointer.pointerEventData, ExecuteEvents.pointerUpHandler);
                        pointer.pointerEventData.pointerPress = null;
                    }
                }
                else
                {
                    ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerPress, pointer.pointerEventData, ExecuteEvents.pointerClickHandler);
                    ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerPress, pointer.pointerEventData, ExecuteEvents.pointerUpHandler);
                    pointer.pointerEventData.pointerPress = null;
                }
            }
            else if (pointer.pointerEventData.eligibleForClick)
            {
                foreach (var result in results)
                {
                    var target = ExecuteEvents.ExecuteHierarchy(result.gameObject, pointer.pointerEventData, ExecuteEvents.pointerDownHandler);
                    if (target != null)
                    {
                        pointer.pointerEventData.pressPosition = pointer.pointerEventData.position;
                        pointer.pointerEventData.pointerPressRaycast = result;
                        pointer.pointerEventData.pointerPress = target;
                        break;
                    }
                }
            }
        }

        private void Drag(VRTK_UIPointer pointer, List<RaycastResult> results)
        {
            pointer.pointerEventData.dragging = pointer.controller.usePressed && pointer.pointerEventData.delta != Vector2.zero;

            if (pointer.pointerEventData.pointerDrag)
            {
                if (pointer.pointerEventData.dragging)
                {
                    if (IsHovering(pointer))
                    {
                        ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerDrag, pointer.pointerEventData, ExecuteEvents.dragHandler);
                    }
                }
                else
                {
                    ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerDrag, pointer.pointerEventData, ExecuteEvents.dragHandler);
                    ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerDrag, pointer.pointerEventData, ExecuteEvents.endDragHandler);
                    foreach (RaycastResult raycast in results)
                    {
                        ExecuteEvents.ExecuteHierarchy(raycast.gameObject, pointer.pointerEventData, ExecuteEvents.dropHandler);
                    }
                    pointer.pointerEventData.pointerDrag = null;
                }
            }
            else if (pointer.pointerEventData.dragging)
            {
                foreach (var result in results)
                {
                    ExecuteEvents.ExecuteHierarchy(result.gameObject, pointer.pointerEventData, ExecuteEvents.initializePotentialDrag);
                    ExecuteEvents.ExecuteHierarchy(result.gameObject, pointer.pointerEventData, ExecuteEvents.beginDragHandler);
                    var target = ExecuteEvents.ExecuteHierarchy(result.gameObject, pointer.pointerEventData, ExecuteEvents.dragHandler);
                    if (target != null)
                    {
                        pointer.pointerEventData.pointerDrag = target;
                        break;
                    }
                }
            }
        }
    }
}