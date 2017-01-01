// Fallback Boundaries|SDK_Fallback|004
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Base Boundaries SDK script provides a bridge to SDK methods that deal with the play area of SDKs that support room scale play spaces.
    /// </summary>
    /// <remarks>
    /// This is the fallback class that will just return default values.
    /// </remarks>
    public class SDK_FallbackBoundaries : SDK_BaseBoundaries
    {
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
            return null;
        }

        /// <summary>
        /// The GetPlayAreaVertices method returns the points of the play area boundaries.
        /// </summary>
        /// <param name="playArea">The GameObject containing the play area representation.</param>
        /// <returns>A Vector3 array of the points in the scene that represent the play area boundaries.</returns>
        public override Vector3[] GetPlayAreaVertices(GameObject playArea)
        {
            return new Vector3[8]
            {
                Vector3.zero,
                Vector3.zero,
                Vector3.zero,
                Vector3.zero,
                Vector3.zero,
                Vector3.zero,
                Vector3.zero,
                Vector3.zero
            };
        }

        /// <summary>
        /// The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.
        /// </summary>
        /// <param name="playArea">The GameObject containing the play area representation.</param>
        /// <returns>The thickness of the drawn border.</returns>
        public override float GetPlayAreaBorderThickness(GameObject playArea)
        {
            return 0f;
        }

        /// <summary>
        /// The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.
        /// </summary>
        /// <param name="playArea">The GameObject containing the play area representation.</param>
        /// <returns>Returns true if the play area size has been auto calibrated and set by external sensors.</returns>
        public override bool IsPlayAreaSizeCalibrated(GameObject playArea)
        {
            return false;
        }

        private void Awake()
        {
            Debug.LogError("Fallback Boundaries SDK is being used. Have you selected a valid Boundaries SDK in the SDK Manager? If you are unsure, then click the GameObject with the `VRTK_SDKManager` script attached to it in Edit Mode and select a Boundaries SDK from the dropdown.");
        }
    }
}