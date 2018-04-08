// WindowsMR Boundaries|SDK_WindowsMR|005
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;
#if UNITY_2017_2_OR_NEWER
    using UnityEngine.XR;
    using UnityEngine.Experimental.XR;
#if VRTK_DEFINE_SDK_WINDOWSMR
    using UnityEngine.XR.WSA;
#endif
#else
    using UnityEngine.VR;
    using XRDevice = UnityEngine.VR.VRDevice;
#endif

    /// <summary>
    /// The WindowsMR Boundaries SDK script provides a bridge to the Windows Mixed Reality SDK play area.
    /// </summary>
    [SDK_Description(typeof(SDK_WindowsMR))]
    public class SDK_WindowsMRBoundaries
#if VRTK_DEFINE_SDK_WINDOWSMR && UNITY_2017_2_OR_NEWER
        : SDK_BaseBoundaries
#else
        : SDK_FallbackBoundaries
#endif
    {
#if VRTK_DEFINE_SDK_WINDOWSMR && UNITY_2017_2_OR_NEWER
        /// <summary>
        /// The GetDrawAtRuntime method returns whether the given play area drawn border is being displayed.
        /// </summary>
        /// <returns>Returns true if the drawn border is being displayed.</returns>
        public override bool GetDrawAtRuntime()
        {
            // TODO: Implement
            return false;
        }

        /// <summary>
        /// The GetPlayArea method returns the Transform of the object that is used to represent the play area in the scene.
        /// </summary>
        /// <returns>A transform of the object representing the play area in the scene.</returns>
        public override Transform GetPlayArea()
        {
            if (cachedPlayArea == null)
            {
                Transform headsetCamera = VRTK_DeviceFinder.HeadsetCamera();
                if (headsetCamera != null)
                {
                    cachedPlayArea = headsetCamera.transform;
                }
            }

            if (cachedPlayArea != null && cachedPlayArea.parent != null)
            {
                cachedPlayArea = cachedPlayArea.parent;
            }

            return cachedPlayArea;
        }

        /// <summary>
        /// The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.
        /// </summary>
        /// <returns>The thickness of the drawn border.</returns>
        public override float GetPlayAreaBorderThickness()
        {
            // TODO: Implement - Needed?
            return 0.1f;
        }

        /// <summary>
        /// The GetPlayAreaVertices method returns the points of the play area boundaries.
        /// </summary>
        /// <returns>A Vector3 array of the points in the scene that represent the play area boundaries.</returns>
        public override Vector3[] GetPlayAreaVertices()
        {
            List<Vector3> boundaryGeometry = new List<Vector3>(0);
#if UNITY_2017_2_OR_NEWER
            if (Boundary.TryGetGeometry(boundaryGeometry))
            {
                if (boundaryGeometry.Count > 0)
                {
                    foreach (Vector3 point in boundaryGeometry)
                    {
                        return boundaryGeometry.ToArray();
                    }
                }
                else
                {
                    Debug.LogWarning("Boundary has no points");
                }
            }
#endif
            return null;
        }

        /// <summary>
        /// The InitBoundaries method is run on start of scene and can be used to initialse anything on game start.
        /// </summary>
        public override void InitBoundaries()
        {
            bool isDisplayOpaque = false;
#if UNITY_2017_2_OR_NEWER
            isDisplayOpaque = HolographicSettings.IsDisplayOpaque;
#endif
            if (isDisplayOpaque)
            {
                // Defaulting coordinate system to RoomScale in immersive headsets.
                // This puts the origin 0,0,0 on the floor if a floor has been established during RunSetup via MixedRealityPortal
                XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
            }
            else
            {
                // Defaulting coordinate system to Stationary for HoloLens.
                // This puts the origin 0,0,0 at the first place where the user started the application.
                XRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);
            }

            Transform headsetCamera = VRTK_DeviceFinder.HeadsetCamera();
            if (headsetCamera != null)
            {
                cachedPlayArea = headsetCamera.transform;
            }
        }

        /// <summary>
        /// The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.
        /// </summary>
        /// <returns>Returns true if the play area size has been auto calibrated and set by external sensors.</returns>
        public override bool IsPlayAreaSizeCalibrated()
        {
            // TODO: Implement
            return false;
        }

        /// <summary>
        /// The SetDrawAtRuntime method sets whether the given play area drawn border should be displayed at runtime.
        /// </summary>
        /// <param name="value">The state of whether the drawn border should be displayed or not.</param>
        public override void SetDrawAtRuntime(bool value)
        {
            // TODO: Implement
        }
#endif
    }
}
