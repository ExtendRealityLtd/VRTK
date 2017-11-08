// Base Boundaries|SDK_Base|007
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Base Boundaries SDK script provides a bridge to SDK methods that deal with the play area of SDKs that support room scale play spaces.
    /// </summary>
    /// <remarks>
    /// This is an abstract class to implement the interface required by all implemented SDKs.
    /// </remarks>
    public abstract class SDK_BaseBoundaries : SDK_Base
    {
        protected Transform cachedPlayArea;

        /// <summary>
        /// The InitBoundaries method is run on start of scene and can be used to initialse anything on game start.
        /// </summary>
        public abstract void InitBoundaries();

        /// <summary>
        /// The GetPlayArea method returns the Transform of the object that is used to represent the play area in the scene.
        /// </summary>
        /// <returns>A transform of the object representing the play area in the scene.</returns>
        public abstract Transform GetPlayArea();

        /// <summary>
        /// The GetPlayAreaVertices method returns the points of the play area boundaries.
        /// </summary>
        /// <returns>A Vector3 array of the points in the scene that represent the play area boundaries.</returns>
        public abstract Vector3[] GetPlayAreaVertices();

        /// <summary>
        /// The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.
        /// </summary>
        /// <returns>The thickness of the drawn border.</returns>
        public abstract float GetPlayAreaBorderThickness();

        /// <summary>
        /// The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.
        /// </summary>
        /// <returns>Returns true if the play area size has been auto calibrated and set by external sensors.</returns>
        public abstract bool IsPlayAreaSizeCalibrated();

        /// <summary>
        /// The GetDrawAtRuntime method returns whether the given play area drawn border is being displayed.
        /// </summary>
        /// <returns>Returns true if the drawn border is being displayed.</returns>
        public abstract bool GetDrawAtRuntime();

        /// <summary>
        /// The SetDrawAtRuntime method sets whether the given play area drawn border should be displayed at runtime.
        /// </summary>
        /// <param name="value">The state of whether the drawn border should be displayed or not.</param>
        public abstract void SetDrawAtRuntime(bool value);

        protected Transform GetSDKManagerPlayArea()
        {
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null && sdkManager.loadedSetup.actualBoundaries != null)
            {
                cachedPlayArea = (sdkManager.loadedSetup.actualBoundaries ? sdkManager.loadedSetup.actualBoundaries.transform : null);
                return cachedPlayArea;
            }
            return null;
        }
    }
}