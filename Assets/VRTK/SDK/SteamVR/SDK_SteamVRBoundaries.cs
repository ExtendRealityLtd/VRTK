// SteamVR Boundaries|SDK_SteamVR|005
namespace VRTK
{
#if VRTK_DEFINE_SDK_STEAMVR
    using UnityEngine;
#endif

    /// <summary>
    /// The SteamVR Boundaries SDK script provides a bridge to the SteamVR SDK play area.
    /// </summary>
    [SDK_Description(typeof(SDK_SteamVRSystem))]
    public class SDK_SteamVRBoundaries
#if VRTK_DEFINE_SDK_STEAMVR
        : SDK_BaseBoundaries
#else
        : SDK_FallbackBoundaries
#endif
    {
#if VRTK_DEFINE_SDK_STEAMVR
        protected SteamVR_PlayArea cachedSteamVRPlayArea;

        /// <summary>
        /// The InitBoundaries method is run on start of scene and can be used to initialse anything on game start.
        /// </summary>
        public override void InitBoundaries()
        {
#if UNITY_5_6_OR_NEWER
            Transform headsetCamera = VRTK_DeviceFinder.HeadsetCamera();
            if (headsetCamera != null && headsetCamera.GetComponent<SteamVR_UpdatePoses>() == null)
            {
                headsetCamera.gameObject.AddComponent<SteamVR_UpdatePoses>();
            }
#endif
            SteamVR_PlayArea area = GetCachedSteamVRPlayArea();
            if (area != null)
            {
                area.BuildMesh();
            }
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
                var steamVRPlayArea = VRTK_SharedMethods.FindEvenInactiveComponent<SteamVR_PlayArea>();
                if (steamVRPlayArea != null)
                {
                    cachedSteamVRPlayArea = steamVRPlayArea;
                    cachedPlayArea = steamVRPlayArea.transform;
                }
            }
            return cachedPlayArea;
        }

        /// <summary>
        /// The GetPlayAreaVertices method returns the points of the play area boundaries.
        /// </summary>
        /// <returns>A Vector3 array of the points in the scene that represent the play area boundaries.</returns>
        public override Vector3[] GetPlayAreaVertices()
        {
            SteamVR_PlayArea area = GetCachedSteamVRPlayArea();
            if (area != null)
            {
                return ProcessVertices(area.vertices);
            }
            return null;
        }

        /// <summary>
        /// The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.
        /// </summary>
        /// <returns>The thickness of the drawn border.</returns>
        public override float GetPlayAreaBorderThickness()
        {
            SteamVR_PlayArea area = GetCachedSteamVRPlayArea();
            if (area != null)
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
            SteamVR_PlayArea area = GetCachedSteamVRPlayArea();
            return (area != null && area.size == SteamVR_PlayArea.Size.Calibrated);
        }

        /// <summary>
        /// The GetDrawAtRuntime method returns whether the given play area drawn border is being displayed.
        /// </summary>
        /// <returns>Returns true if the drawn border is being displayed.</returns>
        public override bool GetDrawAtRuntime()
        {
            SteamVR_PlayArea area = GetCachedSteamVRPlayArea();
            return (area != null ? area.drawInGame : false);
        }

        /// <summary>
        /// The SetDrawAtRuntime method sets whether the given play area drawn border should be displayed at runtime.
        /// </summary>
        /// <param name="value">The state of whether the drawn border should be displayed or not.</param>
        public override void SetDrawAtRuntime(bool value)
        {
            SteamVR_PlayArea area = GetCachedSteamVRPlayArea();
            if (area != null)
            {
                area.drawInGame = value;
                area.enabled = true;
            }
        }

        protected virtual SteamVR_PlayArea GetCachedSteamVRPlayArea()
        {
            if (cachedSteamVRPlayArea == null)
            {
                Transform checkPlayArea = GetPlayArea();
                if (checkPlayArea != null)
                {
                    cachedSteamVRPlayArea = checkPlayArea.GetComponent<SteamVR_PlayArea>();
                }
            }

            return cachedSteamVRPlayArea;
        }

        protected virtual Vector3[] ProcessVertices(Vector3[] vertices)
        {
            //If there aren't enough vertices or the play area is calibrated then just return
            if (vertices.Length < 8 || IsPlayAreaSizeCalibrated())
            {
                return vertices;
            }

            //Go through the existing vertices and swap them around so they're in the correct expected location
            Vector3[] modifiedVertices = new Vector3[8];
            int[] verticeIndexes = new int[] { 3, 0, 1, 2, 7, 4, 5, 6 };
            for (int i = 0; i < modifiedVertices.Length; i++)
            {
                modifiedVertices[i] = vertices[verticeIndexes[i]];
            }
            return modifiedVertices;
        }
#endif
    }
}