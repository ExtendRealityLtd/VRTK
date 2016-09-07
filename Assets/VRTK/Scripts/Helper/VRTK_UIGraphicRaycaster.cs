namespace VRTK
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class VRTK_UIGraphicRaycaster : GraphicRaycaster
    {
        public GameObject raycastSource;

        private struct VRGraphic
        {
            public Graphic graphic;
            public float distance;
            public Vector3 position;
            public Vector2 pointerPosition;
        }

        private Canvas m_Canvas;
        private Vector2 lastKnownPosition;
        private const float UI_CONTROL_OFFSET = 0.00001f;

        [NonSerialized]
        private List<VRGraphic> m_RaycastResults = new List<VRGraphic>();
        [NonSerialized]
        private static readonly List<VRGraphic> s_SortedGraphics = new List<VRGraphic>();

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (canvas == null)
            {
                return;
            }

            m_RaycastResults.Clear();
            var ray = new Ray(eventData.pointerCurrentRaycast.worldPosition, eventData.pointerCurrentRaycast.worldNormal);
            Raycast(canvas, eventCamera, ray, m_RaycastResults);
            SetNearestRaycast(ref eventData, resultAppendList);
        }

        private void SetNearestRaycast(ref PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            RaycastResult? nearestRaycast = null;
            for (var index = 0; index < m_RaycastResults.Count; index++)
            {
                RaycastResult castResult = new RaycastResult();
                castResult.gameObject = m_RaycastResults[index].graphic.gameObject;
                castResult.module = this;
                castResult.distance = m_RaycastResults[index].distance;
                castResult.screenPosition = m_RaycastResults[index].pointerPosition;
                castResult.worldPosition = m_RaycastResults[index].position;
                castResult.index = resultAppendList.Count;
                castResult.depth = m_RaycastResults[index].graphic.depth;
                castResult.sortingLayer = canvas.sortingLayerID;
                castResult.sortingOrder = canvas.sortingOrder;
                if (!nearestRaycast.HasValue || castResult.distance < nearestRaycast.Value.distance)
                {
                    nearestRaycast = castResult;
                }
                resultAppendList.Add(castResult);
            }

            if (nearestRaycast.HasValue)
            {
                eventData.position = nearestRaycast.Value.screenPosition;
                eventData.delta = eventData.position - lastKnownPosition;
                lastKnownPosition = eventData.position;
                eventData.pointerCurrentRaycast = nearestRaycast.Value;
            }
        }

        private float GetHitDistance(Ray ray)
        {
            var hitDistance = float.MaxValue;

            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay && blockingObjects != BlockingObjects.None)
            {
                var maxDistance = Vector3.Distance(ray.origin, canvas.transform.position);

                if (blockingObjects == BlockingObjects.ThreeD || blockingObjects == BlockingObjects.All)
                {
                    RaycastHit hit;
                    Physics.Raycast(ray, out hit, maxDistance);
                    if (hit.collider && !hit.collider.GetComponent<VRTK_PlayerObject>())
                    {
                        hitDistance = hit.distance;
                    }
                }

                if (blockingObjects == BlockingObjects.TwoD || blockingObjects == BlockingObjects.All)
                {
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, maxDistance);

                    if (hit.collider != null && !hit.collider.GetComponent<VRTK_PlayerObject>())
                    {
                        hitDistance = hit.fraction * maxDistance;
                    }
                }
            }
            return hitDistance;
        }

        private void Raycast(Canvas canvas, Camera eventCamera, Ray ray, List<VRGraphic> results)
        {
            var hitDistance = GetHitDistance(ray);
            var canvasGraphics = GraphicRegistry.GetGraphicsForCanvas(canvas);
            for (int i = 0; i < canvasGraphics.Count; ++i)
            {
                var graphic = canvasGraphics[i];

                if (graphic.depth == -1 || !graphic.raycastTarget)
                {
                    continue;
                }

                var graphicTransform = graphic.transform;
                Vector3 graphicFormward = graphicTransform.forward;
                float distance = (Vector3.Dot(graphicFormward, graphicTransform.position - ray.origin) / Vector3.Dot(graphicFormward, ray.direction));

                if (distance < 0)
                {
                    continue;
                }

                if ((distance - UI_CONTROL_OFFSET) > hitDistance)
                {
                    continue;
                }

                Vector3 position = ray.GetPoint(distance);
                Vector2 pointerPosition = eventCamera.WorldToScreenPoint(position);

                if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pointerPosition, eventCamera))
                {
                    continue;
                }

                if (graphic.Raycast(pointerPosition, eventCamera))
                {
                    var vrGraphic = new VRGraphic();
                    vrGraphic.graphic = graphic;
                    vrGraphic.position = position;
                    vrGraphic.distance = distance;
                    vrGraphic.pointerPosition = pointerPosition;
                    s_SortedGraphics.Add(vrGraphic);
                }
            }

            s_SortedGraphics.Sort((g1, g2) => g2.graphic.depth.CompareTo(g1.graphic.depth));
            for (int i = 0; i < s_SortedGraphics.Count; ++i)
            {
                results.Add(s_SortedGraphics[i]);
            }

            s_SortedGraphics.Clear();
        }

        private Canvas canvas
        {
            get
            {
                if (m_Canvas != null)
                {
                    return m_Canvas;
                }

                m_Canvas = gameObject.GetComponent<Canvas>();
                return m_Canvas;
            }
        }
    }
}