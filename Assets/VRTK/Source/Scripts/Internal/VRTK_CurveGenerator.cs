//====================================================================================
//
// Purpose: To generate a bezier curve between at least 4 points in space and draw
// a number of spheres across the generated curve
//
// This script is heavily based on the tutorial at: 
// http://catlikecoding.com/unity/tutorials/curves-and-splines/
//
//====================================================================================
namespace VRTK
{
    using UnityEngine;

    public static class Bezier
    {

        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * p0 +
                2f * oneMinusT * t * p1 +
                t * t * p2;
        }

        public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            return
                2f * (1f - t) * (p1 - p0) +
                2f * t * (p2 - p1);
        }

        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float OneMinusT = 1f - t;
            return
                OneMinusT * OneMinusT * OneMinusT * p0 +
                3f * OneMinusT * OneMinusT * t * p1 +
                3f * OneMinusT * t * t * p2 +
                t * t * t * p3;
        }

        public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                3f * oneMinusT * oneMinusT * (p1 - p0) +
                6f * oneMinusT * t * (p2 - p1) +
                3f * t * t * (p3 - p2);
        }
    }

    public class VRTK_CurveGenerator : MonoBehaviour
    {
        public enum BezierControlPointMode
        {
            Free,
            Aligned,
            Mirrored
        }

        protected Vector3[] points;
        protected GameObject[] items;
        protected BezierControlPointMode[] modes;
        protected bool loop;
        protected int frequency;
        protected bool customTracer;
        protected bool rescalePointerTracer;
        protected GameObject tracerLineRenderer;
        protected LineRenderer customLineRenderer;
        protected bool lineRendererAndItem;

        public virtual void Create(int setFrequency, float radius, GameObject tracer, bool rescaleTracer = false)
        {
            float circleSize = radius / 8;

            frequency = setFrequency;
            customLineRenderer = (tracer != null ? tracer.GetComponent<LineRenderer>() : null);
            lineRendererAndItem = (customLineRenderer != null && tracer.GetComponentInChildren<MeshFilter>());
            if (customLineRenderer != null)
            {
                tracerLineRenderer = Instantiate(tracer);
                tracerLineRenderer.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, name, "LineRenderer");
                for (int i = 0; i < tracerLineRenderer.transform.childCount; i++)
                {
                    Destroy(tracerLineRenderer.transform.GetChild(i).gameObject);
                }
                customLineRenderer = tracerLineRenderer.GetComponent<LineRenderer>();
#if UNITY_5_5
                customLineRenderer.numPositions = frequency;
#elif UNITY_5_6_OR_NEWER
                customLineRenderer.positionCount = frequency;
#else
                customLineRenderer.SetVertexCount(frequency);
#endif
            }

            if (customLineRenderer == null || lineRendererAndItem)
            {
                items = new GameObject[frequency];
                for (int f = 0; f < items.Length; f++)
                {
                    customTracer = true;
                    items[f] = (tracer != null ? Instantiate(tracer) : CreateSphere());
                    items[f].transform.SetParent(transform);
                    items[f].layer = LayerMask.NameToLayer("Ignore Raycast");
                    items[f].transform.localScale = new Vector3(circleSize, circleSize, circleSize);
                    if (customLineRenderer != null)
                    {
                        Destroy(items[f].GetComponent<LineRenderer>());
                    }
                }
            }

            rescalePointerTracer = rescaleTracer;
        }

        public virtual void SetPoints(Vector3[] controlPoints, Material material, Color color)
        {
            PointsInit(controlPoints);
            SetObjects(material, color);
        }

        public virtual Vector3[] GetPoints(Vector3[] controlPoints)
        {
            PointsInit(controlPoints);

            Vector3[] calculatedPoints = new Vector3[frequency];
            float stepSize = frequency * 1;
            if (Loop || stepSize == 1)
            {
                stepSize = 1f / stepSize;
            }
            else
            {
                stepSize = 1f / (stepSize - 1);
            }

            for (int f = 0; f < frequency; f++)
            {
                calculatedPoints[f] = GetPoint(f * stepSize);
            }
            return calculatedPoints;
        }

        public virtual void TogglePoints(bool state)
        {
            gameObject.SetActive(state);
            if (tracerLineRenderer != null)
            {
                tracerLineRenderer.SetActive(state);
            }
        }

        protected virtual void PointsInit(Vector3[] controlPoints)
        {
            points = controlPoints;
            modes = new BezierControlPointMode[]
            {
                BezierControlPointMode.Free,
                BezierControlPointMode.Free
            };
        }

        protected virtual GameObject CreateSphere()
        {
            customTracer = false;
            GameObject item = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            item.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, "Sphere");
            Destroy(item.GetComponent<SphereCollider>());
            item.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            item.GetComponent<MeshRenderer>().receiveShadows = false;

            return item;
        }

        protected virtual bool Loop
        {
            get
            {
                return loop;
            }
            set
            {
                loop = value;
                if (value == true)
                {
                    modes[modes.Length - 1] = modes[0];
                    SetControlPoint(0, points[0]);
                }
            }
        }

        protected virtual int ControlPointCount
        {
            get
            {
                return points.Length;
            }
        }

        protected virtual Vector3 GetControlPoint(int index)
        {
            return points[index];
        }

        protected virtual void SetControlPoint(int index, Vector3 point)
        {
            if (index % 3 == 0)
            {
                Vector3 delta = point - points[index];
                if (loop)
                {
                    if (index == 0)
                    {
                        points[1] += delta;
                        points[points.Length - 2] += delta;
                        points[points.Length - 1] = point;
                    }
                    else if (index == points.Length - 1)
                    {
                        points[0] = point;
                        points[1] += delta;
                        points[index - 1] += delta;
                    }
                    else
                    {
                        points[index - 1] += delta;
                        points[index + 1] += delta;
                    }
                }
                else
                {
                    if (index > 0)
                    {
                        points[index - 1] += delta;
                    }
                    if (index + 1 < points.Length)
                    {
                        points[index + 1] += delta;
                    }
                }
            }
            points[index] = point;
            EnforceMode(index);
        }

        protected virtual void EnforceMode(int index)
        {
            int modeIndex = (index + 1) / 3;
            BezierControlPointMode mode = modes[modeIndex];
            if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1))
            {
                return;
            }

            int middleIndex = modeIndex * 3;
            int fixedIndex, enforcedIndex;
            if (index <= middleIndex)
            {
                fixedIndex = middleIndex - 1;
                if (fixedIndex < 0)
                {
                    fixedIndex = points.Length - 2;
                }
                enforcedIndex = middleIndex + 1;
                if (enforcedIndex >= points.Length)
                {
                    enforcedIndex = 1;
                }
            }
            else
            {
                fixedIndex = middleIndex + 1;
                if (fixedIndex >= points.Length)
                {
                    fixedIndex = 1;
                }
                enforcedIndex = middleIndex - 1;
                if (enforcedIndex < 0)
                {
                    enforcedIndex = points.Length - 2;
                }
            }

            Vector3 middle = points[middleIndex];
            Vector3 enforcedTangent = middle - points[fixedIndex];
            if (mode == BezierControlPointMode.Aligned)
            {
                enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
            }
            points[enforcedIndex] = middle + enforcedTangent;
        }

        protected virtual int CurveCount
        {
            get
            {
                return (points.Length - 1) / 3;
            }
        }

        protected virtual Vector3 GetPoint(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = points.Length - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
        }

        protected virtual void SetObjects(Material material, Color color)
        {
            float stepSize = frequency * 1;
            if (Loop || stepSize == 1)
            {
                stepSize = 1f / stepSize;
            }
            else
            {
                stepSize = 1f / (stepSize - 1);
            }

            SetPointData(material, color, stepSize);
        }

        protected virtual void SetPointData(Material material, Color color, float stepSize)
        {
            for (int f = 0; f < frequency; f++)
            {
                Vector3 position = GetPoint(f * stepSize);

                if (customLineRenderer != null)
                {
                    customLineRenderer.SetPosition(f, position);
                    SetMaterial(customLineRenderer.sharedMaterial, color);
                }

                if (customLineRenderer == null || lineRendererAndItem)
                {
                    SetItemPosition(f, position, material, color, stepSize);
                }
            }
        }

        protected virtual void SetItemPosition(int currentIndex, Vector3 setPosition, Material material, Color color, float stepSize)
        {
            if (customTracer && (currentIndex == (frequency - 1)))
            {
                items[currentIndex].SetActive(false);
                return;
            }
            SetItemMaterial(items[currentIndex], material, color);
            items[currentIndex].transform.position = setPosition;

            Vector3 nextPosition = GetPoint((currentIndex + 1) * stepSize);
            Vector3 offset = nextPosition - setPosition;
            Vector3 lookPosition = offset.normalized;
            if (lookPosition != Vector3.zero)
            {
                items[currentIndex].transform.rotation = Quaternion.LookRotation(lookPosition);

                // rescale the custom tracer according to the length of the beam
                if (rescalePointerTracer)
                {
                    Vector3 scl = items[currentIndex].transform.localScale;
                    scl.z = offset.magnitude / 2f; // (assuming a center-based scaling)
                    items[currentIndex].transform.localScale = scl;
                }
            }
        }

        protected virtual void SetItemMaterial(GameObject item, Material material, Color color)
        {
            foreach (Renderer mr in item.GetComponentsInChildren<Renderer>())
            {
                if (material != null)
                {
                    mr.material = material;
                }

                SetMaterial(mr.material, color);
            }
        }

        protected virtual void SetMaterial(Material material, Color color)
        {
            if (material != null)
            {
                material.EnableKeyword("_EMISSION");

                if (material.HasProperty("_Color"))
                {
                    material.color = color;
                }

                if (material.HasProperty("_EmissionColor"))
                {
                    material.SetColor("_EmissionColor", VRTK_SharedMethods.ColorDarken(color, 50));
                }
            }
        }
    }
}