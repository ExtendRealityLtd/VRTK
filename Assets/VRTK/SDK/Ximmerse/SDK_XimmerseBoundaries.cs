// Ximmerse Boundaries|SDK_Ximmerse|005
namespace VRTK
{
#if VRTK_DEFINE_SDK_XIMMERSE
    using UnityEngine;
    using Ximmerse.VR;
#endif

    /// <summary>
    /// The Ximmerse Boundaries SDK script provides a bridge to the Ximmerse SDK play area.
    /// </summary>
    [SDK_Description(typeof(SDK_XimmerseSystem))]
    [SDK_Description(typeof(SDK_XimmerseSystem), 1)]
    public class SDK_XimmerseBoundaries
#if VRTK_DEFINE_SDK_XIMMERSE
        : SDK_BaseBoundaries
#else
        : SDK_FallbackBoundaries
#endif
    {
#if VRTK_DEFINE_SDK_XIMMERSE
        /// <summary>
        /// The InitBoundaries method is run on start of scene and can be used to initialse anything on game start.
        /// </summary>
        public override void InitBoundaries()
        {
        }

        /// <summary>
        /// The GetPlayArea method returns the Transform of the object that is used to represent the play area in the scene.
        /// </summary>
        /// <returns>A transform of the object representing the play area in the scene.</returns>
        public override Transform GetPlayArea()
        {
            cachedPlayArea = GetSDKManagerPlayArea();
            if (cachedPlayArea == null)
            {
                VRContext vrContext = VRTK_SharedMethods.FindEvenInactiveComponent<VRContext>();
                if (Application.isPlaying)
                {
                    vrContext.InitVRContext();
                }
                cachedPlayArea = vrContext.transform;
            }
            return cachedPlayArea;
        }

        /// <summary>
        /// The GetPlayAreaVertices method returns the points of the play area boundaries.
        /// </summary>
        /// <returns>A Vector3 array of the points in the scene that represent the play area boundaries.</returns>
        public override Vector3[] GetPlayAreaVertices()
        {
            var area = GetPlayArea().GetComponentInChildren<PlayAreaRenderer>();
            if (area)
            {
                return area.corners;
            }
            return null;
        }

        /// <summary>
        /// The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.
        /// </summary>
        /// <returns>The thickness of the drawn border.</returns>
        public override float GetPlayAreaBorderThickness()
        {
            var area = GetPlayArea().GetComponentInChildren<PlayAreaRenderer>();
            if (area)
            {
                return area.borderThickness;
            }
            return 0f;
        }

        /// <summary>
        /// The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.
        /// </summary>
        /// <returns>Returns true if the play area size has been auto calibrated and set by external sensors.</returns>
        public override bool IsPlayAreaSizeCalibrated()
        {
            return true;
        }

        /// <summary>
        /// The GetDrawAtRuntime method returns whether the given play area drawn border is being displayed.
        /// </summary>
        /// <returns>Returns true if the drawn border is being displayed.</returns>
        public override bool GetDrawAtRuntime()
        {
            return false;
        }

        /// <summary>
        /// The SetDrawAtRuntime method sets whether the given play area drawn border should be displayed at runtime.
        /// </summary>
        /// <param name="value">The state of whether the drawn border should be displayed or not.</param>
        public override void SetDrawAtRuntime(bool value)
        {
        }
#endif
    }
}