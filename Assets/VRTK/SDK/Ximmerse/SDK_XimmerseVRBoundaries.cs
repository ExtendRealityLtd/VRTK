// XimmerseVR Boundaries|SDK_XimmerseVR|005
namespace VRTK
{
#if VRTK_DEFINE_SDK_XIMMERSEVR
    using UnityEngine;
#endif

    /// <summary>
    /// The XimmerseVR Boundaries SDK script provides a bridge to the XimmerseVR SDK play area.
    /// </summary>
    [SDK_Description(typeof(SDK_XimmerseVRSystem))]
    public class SDK_XimmerseVRBoundaries
#if VRTK_DEFINE_SDK_XIMMERSEVR
        : SDK_BaseBoundaries
#else
        : SDK_FallbackBoundaries
#endif
    {
#if VRTK_DEFINE_SDK_XIMMERSEVR
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
                cachedPlayArea = Ximmerse.VR.VRContext.main.transform;
            }
            return cachedPlayArea;
        }

        /// <summary>
        /// The GetPlayAreaVertices method returns the points of the play area boundaries.
        /// </summary>
        /// <param name="playArea">The GameObject containing the play area representation.</param>
        /// <returns>A Vector3 array of the points in the scene that represent the play area boundaries.</returns>
        public override Vector3[] GetPlayAreaVertices(GameObject playArea)
        {
            var area = playArea.GetComponentInChildren<PlayAreaRenderer>();
            if (area)
            {
                return area.corners;
            }
            return null;
        }

        /// <summary>
        /// The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.
        /// </summary>
        /// <param name="playArea">The GameObject containing the play area representation.</param>
        /// <returns>The thickness of the drawn border.</returns>
        public override float GetPlayAreaBorderThickness(GameObject playArea)
        {
            var area = playArea.GetComponentInChildren<PlayAreaRenderer>();
            if (area)
            {
                return area.borderThickness;
            }
            return 0f;
        }

        /// <summary>
        /// The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.
        /// </summary>
        /// <param name="playArea">The GameObject containing the play area representation.</param>
        /// <returns>Returns true if the play area size has been auto calibrated and set by external sensors.</returns>
        public override bool IsPlayAreaSizeCalibrated(GameObject playArea)
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