// Simulator Boundaries|SDK_Simulator|004

using UnityEngine;

namespace VRTK
{
    /// <summary>
    ///     The PlayStation Boundaries SDK script provides  functions for the Frustum area boundaries.
    /// </summary>
    [SDK_Description(typeof(SDK_PlayStationVRSystem))]
    public class SDK_PlayStationVRBoundaries : SDK_BaseBoundaries
    {
        private Transform area;
        private SDK_PlayStationFrustum frustum;
        private Color hideColor;
        private Color showcolor;

        /// <summary>
        ///     The InitBoundaries method is run on start of scene and can be used to initialse anything on game start.
        /// </summary>
        public override void InitBoundaries()
        {
            frustum = FindObjectOfType<SDK_PlayStationFrustum>();
#if UNITY_PS4
            frustum.Register();
#endif
        }

        /// <summary>
        ///     The GetPlayArea method returns the Transform of the object that is used to represent the play area in the scene.
        /// </summary>
        /// <returns>A transform of the object representing the play area in the scene.</returns>
        public override Transform GetPlayArea()
        {
            if (area == null)
            {
                GameObject simPlayer = SDK_PlayStationVRInput.FindInScene();
                if (simPlayer)
                {
                    area = simPlayer.transform;
                }
            }
            return area;
        }

        /// <summary>
        ///     The GetPlayAreaVertices method returns the points of the play area boundaries.
        /// </summary>
        /// <param name="playArea">The GameObject containing the play area representation.</param>
        /// <returns>A Vector3 array of the points in the scene that represent the play area boundaries.</returns>
        public override Vector3[] GetPlayAreaVertices(GameObject playArea)
        {
            return area ? frustum.Bounds : null;
        }

        /// <summary>
        ///     The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.
        /// </summary>
        /// <param name="playArea">The GameObject containing the play area representation.</param>
        /// <returns>The thickness of the drawn border.</returns>
        public override float GetPlayAreaBorderThickness(GameObject playArea)
        {
            return 0.1f;
        }

        /// <summary>
        ///     The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external
        ///     sensors.
        /// </summary>
        /// <param name="playArea">The GameObject containing the play area representation.</param>
        /// <returns>Returns true if the play area size has been auto calibrated and set by external sensors.</returns>
        public override bool IsPlayAreaSizeCalibrated(GameObject playArea)
        {
            return true;
        }

        /// <summary>
        ///     The GetDrawAtRuntime method returns whether the given play area drawn border is being displayed.
        /// </summary>
        /// <returns>Returns true if the drawn border is being displayed.</returns>
        public override bool GetDrawAtRuntime()
        {
            return frustum.ShowFrustum;
        }

        /// <summary>
        ///     The SetDrawAtRuntime method sets whether the given play area drawn border should be displayed at runtime.
        /// </summary>
        /// <param name="value">The state of whether the drawn border should be displayed or not.</param>
        public override void SetDrawAtRuntime(bool value)
        {
            frustum.ToggleFrustum(value);
        }
    }
}