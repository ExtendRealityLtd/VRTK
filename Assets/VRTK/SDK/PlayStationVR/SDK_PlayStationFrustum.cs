using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

namespace VRTK
{
    /// <summary>
    /// Creates the bounds for the player. 
    /// </summary>
    public partial class SDK_PlayStationFrustum : MonoBehaviour
    {
        private Color cachedShowColor, cachedHideColor;
        private Vector3 camAcceleration;
        public float fadeSpeed = 3f;
        public Renderer[] frustumRenderers;
        public Transform frustumTransform;
        private Vector3 hmdPositionRaw;
        private Quaternion hmdRotationUnity, hmdRotationRaw;
        public float safeDistance = 0.1f;
        public Color showColor, hideColor;

        public Vector3[] Bounds
        {
            get { return BoundsCalculation(); }
        }

        public bool ShowFrustum { get; set; }

        private void Awake()
        {
            cachedShowColor = showColor;
            cachedHideColor = hideColor;
        }

        private IEnumerator Start()
        {
            foreach (Renderer fR in frustumRenderers)
            {
                fR.material.color = hideColor;
            }


#if UNITY_PS4
            yield return new WaitUntil(() => VRSettings.enabled);

            UpdateFrustumTransform();
#endif
            yield return null;
        }

        private void Update()
        {
#if UNITY_PS4
            UpdateFrustumTracking();
#endif
        }

        private Vector3[] BoundsCalculation()
        {
            List<Vector3> bounds = new List<Vector3>();
            for (int i = 0; i < frustumRenderers.Length; i++)
            {
                Bounds bBox = CalculateBoundingBox(frustumRenderers[i].gameObject);
                Vector3 xz = new Vector3(bBox.min.x, bBox.min.y, bBox.center.z);
                Vector3 xzMax = new Vector3(bBox.max.x, bBox.center.y, bBox.max.z);
                Vector3 xzMin = new Vector3(bBox.max.x, bBox.center.y, -bBox.max.z);
                Vector3 backMax = new Vector3(bBox.max.x, bBox.max.y, bBox.center.z);

                if (!bounds.Contains(frustumTransform.position))
                {
                    bounds.Add(frustumTransform.position);
                }

                if (!bounds.Contains(bBox.center))
                {
                    bounds.Add(bBox.center);
                }

                if (!bounds.Contains(frustumRenderers[i].transform.position))
                {
                    bounds.Add(frustumRenderers[i].transform.position);
                }

                if (!bounds.Contains(xz))
                {
                    bounds.Add(xz);
                }

                if (!bounds.Contains(xzMax))
                {
                    bounds.Add(xzMax);
                }

                if (!bounds.Contains(xzMin))
                {
                    bounds.Add(xzMin);
                }

                if (!bounds.Contains(backMax))
                {
                    bounds.Add(backMax);
                }
            }
            return bounds.ToArray();
        }

        private Bounds CalculateBoundingBox(GameObject gameObjectWithMesh)
        {
            Mesh mesh = null;
            MeshFilter meshFilter = gameObjectWithMesh.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                mesh = meshFilter.mesh;
            }
            else
            {
                SkinnedMeshRenderer skinnedMeshRenderer = gameObjectWithMesh.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                {
                    mesh = skinnedMeshRenderer.sharedMesh;
                }
            }
            if (mesh == null)
            {
                Debug.LogError("CalculateBoundingBox: no mesh found on the given object");
                return new Bounds(gameObjectWithMesh.transform.position, Vector3.one);
            }
            Vector3[] vertices = mesh.vertices;
            if (vertices.Length <= 0)
            {
                Debug.LogError("CalculateBoundingBox: mesh doesn't have vertices");
                return new Bounds(gameObjectWithMesh.transform.position, Vector3.one);
            }
            Vector3 max;
            Vector3 min = max = gameObjectWithMesh.transform.TransformPoint(vertices[0]);
            for (int i = 1; i < vertices.Length; i++)
            {
                Vector3 v = gameObjectWithMesh.transform.TransformPoint(vertices[i]);
                for (int n = 0; n < 3; n++)
                {
                    if (v[n] > max[n])
                    {
                        max[n] = v[n];
                    }
                    if (v[n] < min[n])
                    {
                        min[n] = v[n];
                    }
                }
            }
            Bounds b = new Bounds();
            b.SetMinMax(min, max);
            return b;
        }

        private void UpdateFrustumDisplay()
        {
            foreach (Renderer fR in frustumRenderers)
            {
                if (ShowFrustum)
                {
                    fR.material.color = Color.Lerp(fR.material.color, showColor, Time.deltaTime * fadeSpeed);
                }
                else
                {
                    fR.material.color = Color.Lerp(fR.material.color, hideColor, Time.deltaTime * fadeSpeed * 2);
                }
            }
        }

        public void TurnOnFrustum()
        {
            showColor = cachedShowColor;
            hideColor = cachedHideColor;
        }

        public void ToggleFrustum(bool on)
        {
            if (on)
            {
                TurnOnFrustum();
            }
            else
            {
                TurnOffFrustum();
            }
        }

        public void TurnOffFrustum()
        {
            showColor = Color.clear;
            hideColor = Color.clear;
        }
    }
}