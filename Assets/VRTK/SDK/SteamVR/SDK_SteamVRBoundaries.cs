// SteamVR Boundaries|SDK_SteamVR|004
namespace VRTK
{
#if VRTK_SDK_STEAMVR
    using UnityEngine;

    /// <summary>
    /// The SteamVR Boundaries SDK script provides a bridge to the SteamVR SDK play area.
    /// </summary>
    public class SDK_SteamVRBoundaries : SDK_BaseBoundaries
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
            cachedPlayArea = GetSDKManagerPlayArea();
            if (cachedPlayArea == null)
            {
                var steamVRPlayArea = FindObjectOfType<SteamVR_PlayArea>();
                if (steamVRPlayArea)
                {
                    cachedPlayArea = steamVRPlayArea.transform;
                }
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
            var area = playArea.GetComponent<SteamVR_PlayArea>();
            if (area)
            {
                return area.vertices;
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
            var area = playArea.GetComponent<SteamVR_PlayArea>();
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
            var area = playArea.GetComponent<SteamVR_PlayArea>();
            return (area.size == SteamVR_PlayArea.Size.Calibrated);
        }
    }
#else
    public class SDK_SteamVRBoundaries : SDK_FallbackBoundaries
    {
    }
#endif
}