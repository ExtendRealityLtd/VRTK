// Oculus Boundaries|SDK_Oculus|005
namespace VRTK
{
#if VRTK_DEFINE_SDK_OCULUS
    using UnityEngine;
#endif

    /// <summary>
    /// The Oculus Boundaries SDK script provides a bridge to the Oculus SDK play area.
    /// </summary>
    [SDK_Description(typeof(SDK_OculusSystem))]
    [SDK_Description(typeof(SDK_OculusSystem), 1)]
    public class SDK_OculusBoundaries
#if VRTK_DEFINE_SDK_OCULUS
        : SDK_BaseBoundaries
#else
        : SDK_FallbackBoundaries
#endif
    {
#if VRTK_DEFINE_SDK_OCULUS
        /// <summary>
        /// The InitBoundaries method is run on start of scene and can be used to initialse anything on game start.
        /// </summary>
        public override void InitBoundaries()
        {
#if VRTK_DEFINE_SDK_OCULUS_AVATAR
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
                OVRManager ovrManager = VRTK_SharedMethods.FindEvenInactiveComponent<OVRManager>(true);
                if (ovrManager != null)
                {
                    cachedPlayArea = ovrManager.transform;
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
            OVRBoundary area = new OVRBoundary();
            if (area.GetConfigured())
            {
                Vector3 outerBoundary = area.GetDimensions(OVRBoundary.BoundaryType.OuterBoundary);
                float thickness = 0.1f;

                Vector3[] vertices = new Vector3[8];

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
        /// <returns>The thickness of the drawn border.</returns>
        public override float GetPlayAreaBorderThickness()
        {
            return 0.1f;
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

#if VRTK_DEFINE_SDK_OCULUS_AVATAR
        private OvrAvatar avatarContainer;

        /// <summary>
        /// The GetAvatar method is used to retrieve the Oculus Avatar object if it exists in the scene. This method is only available if the Oculus Avatar package is installed.
        /// </summary>
        /// <returns>The OvrAvatar script for managing the Oculus Avatar.</returns>
        public virtual OvrAvatar GetAvatar()
        {
            if (avatarContainer == null)
            {
                avatarContainer = VRTK_SharedMethods.FindEvenInactiveComponent<OvrAvatar>(true);
                if (avatarContainer != null && avatarContainer.GetComponent<VRTK_TransformFollow>() == null)
                {
                    VRTK_TransformFollow objectFollow = avatarContainer.gameObject.AddComponent<VRTK_TransformFollow>();
                    objectFollow.gameObjectToFollow = GetPlayArea().gameObject;
                }
            }
            return avatarContainer;
        }
#endif
#endif
    }
}