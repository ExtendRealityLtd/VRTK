﻿// Base Boundaries|SDK_Base|007
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
        /// <param name="playArea">The GameObject containing the play area representation.</param>
        /// <returns>A Vector3 array of the points in the scene that represent the play area boundaries.</returns>
        public abstract Vector3[] GetPlayAreaVertices(GameObject playArea);

        /// <summary>
        /// The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.
        /// </summary>
        /// <param name="playArea">The GameObject containing the play area representation.</param>
        /// <returns>The thickness of the drawn border.</returns>
        public abstract float GetPlayAreaBorderThickness(GameObject playArea);

        /// <summary>
        /// The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.
        /// </summary>
        /// <param name="playArea">The GameObject containing the play area representation.</param>
        /// <returns>Returns true if the play area size has been auto calibrated and set by external sensors.</returns>
        public abstract bool IsPlayAreaSizeCalibrated(GameObject playArea);

        protected Transform GetSDKManagerPlayArea()
        {
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null && sdkManager.actualBoundaries != null)
            {
                cachedPlayArea = (sdkManager.actualBoundaries ? sdkManager.actualBoundaries.transform : null);
                return cachedPlayArea;
            }
            return null;
        }
    }
}