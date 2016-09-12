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
                if (pointer.gameObject.activeInHierarchy && pointer.enabled)
                {
                    List<RaycastResult> results = new List<RaycastResult>();
                    if (pointer.PointerActive())
                    {
                        results = CheckRaycasts(pointer);
                    }
                    //Process events
                    Hover(pointer, results);
                    Click(pointer, results);
                    Drag(pointer, results);
                    Scroll(pointer, results);
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
                if (pointer.pointerEventData.pointerEnter && hoveredObject && CheckTransformTree(hoveredObject.transform, pointer.pointerEventData.pointerEnter.transform))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ShouldIgnoreElement(GameObject obj, string ignoreCanvasWithTagOrClass, VRTK_TagOrScriptPolicyList canvasTagOrScriptListPolicy)
        {
            var canvas = obj.GetComponentInParent<Canvas>();
            if (!canvas)
            {
                return false;
            }

            return (Utilities.TagOrScriptCheck(canvas.gameObject, canvasTagOrScriptListPolicy, ignoreCanvasWithTagOrClass));
        }

        private void Hover(VRTK_UIPointer pointer, List<RaycastResult> results)
        {
            if (pointer.pointerEventData.pointerEnter)
            {
                if (ShouldIgnoreElement(pointer.pointerEventData.pointerEnter, pointer.ignoreCanvasWithTagOrClass, pointer.canvasTagOrScriptListPolicy))
                {
                    return;
                }

                if (NoValidCollision(pointer, results))
                {
                    ExecuteEvents.ExecuteHierarchy(pointer.pointerEventData.pointerEnter, pointer.pointerEventData, ExecuteEvents.pointerExitHandler);
                    pointer.pointerEventData.hovered.Remove(pointer.pointerEventData.pointerEnter);
                    pointer.pointerEventData.pointerEnter = null;
                }
            }
            else
            {
                foreach (var result in results)
                {
                    if (ShouldIgnoreElement(result.gameObject, pointer.ignoreCanvasWithTagOrClass, pointer.canvasTagOrScriptListPolicy))
                    {
                        continue;
                    }

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

                        pointer.OnUIPointerElementEnter(pointer.SetUIPointerEvent(target, pointer.hoveringElement));
                        pointer.hoveringElement = target;
                        pointer.pointerEventData.pointerCurrentRaycast = result;
                        pointer.pointerEventData.pointerEnter = target;
                        pointer.pointerEventData.hovered.Add(pointer.pointerEventData.pointerEnter);
                        break;
                    }
                    else
                    {
                        if (result.gameObject != pointer.hoveringElement)
                        {
                            pointer.OnUIPointerElementEnter(pointer.SetUIPointerEvent(result.gameObject, pointer.hoveringElement));
                        }
                        pointer.hoveringElement = result.gameObject;
                    }
                }

                if (pointer.hoveringElement && results.Count == 0)
                {
                    pointer.OnUIPointerElementExit(pointer.SetUIPointerEvent(null, pointer.hoveringElement));
                    pointer.hoveringElement = null;
                }
            }
        }

        private void Click(VRTK_UIPointer pointer, List<RaycastResult> results)
        {
            pointer.pointerEventData.eligibleForClick = pointer.controller.uiClickPressed;

            if (pointer.pointerEventData.pointerPress)
            {
                if (ShouldIgnoreElement(pointer.pointerEventData.pointerPress, pointer.ignoreCanvasWithTagOrClass, pointer.canvasTagOrScriptListPolicy))
                {
                    return;
                }

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
                    if (ShouldIgnoreElement(result.gameObject, pointer.ignoreCanvasWithTagOrClass, pointer.canvasTagOrScriptListPolicy))
                    {
                        continue;
                    }

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
            pointer.pointerEventData.dragging = pointer.controller.uiClickPressed && pointer.pointerEventData.delta != Vector2.zero;

            if (pointer.pointerEventData.pointerDrag)
            {
                if (ShouldIgnoreElement(pointer.pointerEventData.pointerDrag, pointer.ignoreCanvasWithTagOrClass, pointer.canvasTagOrScriptListPolicy))
                {
                    return;
                }

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
                    if (ShouldIgnoreElement(result.gameObject, pointer.ignoreCanvasWithTagOrClass, pointer.canvasTagOrScriptListPolicy))
                    {
                        continue;
                    }

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

        private void Scroll(VRTK_UIPointer pointer, List<RaycastResult> results)
        {
            pointer.pointerEventData.scrollDelta = pointer.controller.GetTouchpadAxis();
            var scrollWheelVisible = false;
            foreach (RaycastResult result in results)
            {
                if (pointer.pointerEventData.scrollDelta != Vector2.zero)
                {
                    var target = ExecuteEvents.ExecuteHierarchy(result.gameObject, pointer.pointerEventData, ExecuteEvents.scrollHandler);
                    if (target)
                    {
                        scrollWheelVisible = true;
                    }
                }
            }

            if (pointer.controllerRenderModel)
            {
                VRTK_SDK_Bridge.SetControllerRenderModelWheel(pointer.controllerRenderModel, scrollWheelVisible);
            }
        }
    }
}