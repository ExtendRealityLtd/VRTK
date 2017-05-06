// OculusVR Boundaries|SDK_OculusVR|005
namespace VRTK
{
#if VRTK_DEFINE_SDK_OCULUSVR
    using UnityEngine;
#endif

    /// <summary>
    /// The OculusVR Boundaries SDK script provides a bridge to the OculusVR SDK play area.
    /// </summary>
    [SDK_Description(typeof(SDK_OculusVRSystem))]
    public class SDK_OculusVRBoundaries
#if VRTK_DEFINE_SDK_OCULUSVR
        : SDK_BaseBoundaries
#else
        : SDK_FallbackBoundaries
#endif
    {
#if VRTK_DEFINE_SDK_OCULUSVR
        /// <summary>
        /// The InitBoundaries method is run on start of scene and can be used to initialse anything on game start.
        /// </summary>
        public override void InitBoundaries()
        {
#if VRTK_DEFINE_SDK_OCULUSVR_AVATAR
            GetAvatar();
#endif
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
                var ovrManager = VRTK_SharedMethods.FindEvenInactiveComponent<OVRManager>();
                if (ovrManager)
                {
                    cachedPlayArea = ovrManager.transform;
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
            var area = new OVRBoundary();
            if (area.GetConfigured())
            {
                var outerBoundary = area.GetDimensions(OVRBoundary.BoundaryType.OuterBoundary);
                var thickness = 0.1f;

                var vertices = new Vector3[8];

                vertices[0] = new Vector3(outerBoundary.x - thickness, 0f, outerBoundary.z - thickness);
                vertices[1] = new Vector3(0f + thickness, 0f, outerBoundary.z - thickness);
                vertices[2] = new Vector3(0f + thickness, 0f, 0f + thickness);
                vertices[3] = new Vector3(outerBoundary.x - thickness, 0f, 0f + thickness);

                vertices[4] = new Vector3(outerBoundary.x, 0f, outerBoundary.z);
                vertices[5] = new Vector3(0f, 0f, outerBoundary.z);
                vertices[6] = new Vector3(0f, 0f, 0f);
                vertices[7] = new Vector3(outerBoundary.x, 0f, 0f);

                return vertices;
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

#if VRTK_DEFINE_SDK_OCULUSVR_AVATAR
        private OvrAvatar avatarContainer;

        /// <summary>
        /// The GetAvatar method is used to retrieve the Oculus Avatar object if it exists in the scene. This method is only available if the Oculus Avatar package is installed.
        /// </summary>
        /// <returns>The OvrAvatar script for managing the Oculus Avatar.</returns>
        public virtual OvrAvatar GetAvatar()
        {
            if (avatarContainer == null)
            {
                avatarContainer = VRTK_SharedMethods.FindEvenInactiveComponent<OvrAvatar>();
                if (avatarContainer != null && avatarContainer.GetComponent<VRTK_TransformFollow>() == null)
                {
                    var objectFollow = avatarContainer.gameObject.AddComponent<VRTK_TransformFollow>();
                    objectFollow.gameObjectToFollow = GetPlayArea().gameObject;
                }
            }
            return avatarContainer;
        }
#endif
#endif
    }
}