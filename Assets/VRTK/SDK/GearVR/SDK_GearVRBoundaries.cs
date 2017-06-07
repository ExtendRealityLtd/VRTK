// GearVR Boundaries|SDK_GearVR|004
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Base Boundaries SDK script provides a bridge to SDK methods that deal with the play area of SDKs that support room scale play spaces.
    /// </summary>
    /// <remarks>
    /// This is the fallback class that will just return default values.
    /// </remarks>
    [SDK_Description(typeof(SDK_GearVRSystem))]
    public class SDK_GearVRBoundaries : SDK_BaseBoundaries
    {
        private Vector3[] cachedBoundaryVertices;

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
                var mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    var mainCameraTransform = mainCamera.transform;
                    if (mainCameraTransform.parent != null)
                    {
                        cachedPlayArea = mainCameraTransform.parent;
                    }
                    else
                    {
#if UNITY_EDITOR
                        Debug.LogWarning("In order to allow teleporting to work, the main camera must be under a parent GameObject that can be moved!");
#endif
                        cachedPlayArea = mainCameraTransform;
                    }
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
            if (cachedBoundaryVertices == null)
            {
                float thickness = 0.1f;
                Vector3 outerBoundary = new Vector3(1f, 0f, 1f);

                cachedBoundaryVertices = new Vector3[8]
                {
                    new Vector3(outerBoundary.x - thickness, 0f, outerBoundary.z - thickness),
                    new Vector3(0f + thickness, 0f, outerBoundary.z - thickness),
                    new Vector3(0f + thickness, 0f, 0f + thickness),
                    new Vector3(outerBoundary.x - thickness, 0f, 0f + thickness),

                    new Vector3(outerBoundary.x, 0f, outerBoundary.z),
                    new Vector3(0f, 0f, outerBoundary.z),
                    new Vector3(0f, 0f, 0f),
                    new Vector3(outerBoundary.x, 0f, 0f)
                };
            }

            return cachedBoundaryVertices;
        }

        /// <summary>
        /// The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.
        /// </summary>
        /// <param name="playArea">The GameObject containing the play area representation.</param>
        /// <returns>The thickness of the drawn border.</returns>
        public override float GetPlayAreaBorderThickness(GameObject playArea)
        {
            return 0.1f;
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
    }
}