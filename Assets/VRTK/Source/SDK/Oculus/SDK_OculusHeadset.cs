// Oculus Headset|SDK_Oculus|003
namespace VRTK
{
#if VRTK_DEFINE_SDK_OCULUS
    using UnityEngine;
    using System.Collections.Generic;
#endif

    /// <summary>
    /// The Oculus Headset SDK script provides a bridge to the Oculus SDK.
    /// </summary>
    [SDK_Description(typeof(SDK_OculusSystem))]
    [SDK_Description(typeof(SDK_OculusSystem), 1)]
    public class SDK_OculusHeadset
#if VRTK_DEFINE_SDK_OCULUS
        : SDK_BaseHeadset
#else
        : SDK_FallbackHeadset
#endif
    {
#if VRTK_DEFINE_SDK_OCULUS
        protected VRTK_VelocityEstimator cachedHeadsetVelocityEstimator;

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(Dictionary<string, object> options)
        {
        }

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public override void ProcessFixedUpdate(Dictionary<string, object> options)
        {
        }

        /// <summary>
        /// The GetHeadset method returns the Transform of the object that is used to represent the headset in the scene.
        /// </summary>
        /// <returns>A transform of the object representing the headset in the scene.</returns>
        public override Transform GetHeadset()
        {
            cachedHeadset = GetSDKManagerHeadset();
            if (cachedHeadset == null)
            {
                cachedHeadset = VRTK_SharedMethods.FindEvenInactiveGameObject<OVRCameraRig>("TrackingSpace/CenterEyeAnchor", true).transform;
            }
            return cachedHeadset;
        }

        /// <summary>
        /// The GetHeadsetCamera method returns the Transform of the object that is used to hold the headset camera in the scene.
        /// </summary>
        /// <returns>A transform of the object holding the headset camera in the scene.</returns>
        public override Transform GetHeadsetCamera()
        {
            cachedHeadsetCamera = GetSDKManagerHeadset();
            if (cachedHeadsetCamera == null)
            {
                cachedHeadsetCamera = GetHeadset();
            }
            return cachedHeadsetCamera;
        }

        /// <summary>
        /// The GetHeadsetType method returns a string representing the type of headset connected.
        /// </summary>
        /// <returns>The string of the headset connected.</returns>
        public override string GetHeadsetType()
        {
            switch (OVRPlugin.GetSystemHeadsetType())
            {
                case OVRPlugin.SystemHeadset.Rift_CV1:
                    return CleanPropertyString("oculusrift");
                case OVRPlugin.SystemHeadset.GearVR_R320:
                case OVRPlugin.SystemHeadset.GearVR_R321:
                case OVRPlugin.SystemHeadset.GearVR_R322:
                case OVRPlugin.SystemHeadset.GearVR_R323:
                    return CleanPropertyString("oculusgearvr");
                case OVRPlugin.SystemHeadset.Rift_DK1:
                    return CleanPropertyString("oculusriftdk1");
                case OVRPlugin.SystemHeadset.Rift_DK2:
                    return CleanPropertyString("oculusriftdk2");
            }
            return CleanPropertyString("");
        }

        /// <summary>
        /// The GetHeadsetVelocity method is used to determine the current velocity of the headset.
        /// </summary>
        /// <returns>A Vector3 containing the current velocity of the headset.</returns>
        public override Vector3 GetHeadsetVelocity()
        {
#if VRTK_DEFINE_OCULUS_UTILITIES_1_11_0_OR_OLDER
            return OVRManager.isHmdPresent ? OVRPlugin.GetEyeVelocity(OVRPlugin.Eye.Left).ToOVRPose().position : Vector3.zero;
#elif VRTK_DEFINE_OCULUS_UTILITIES_1_12_0_OR_NEWER
            return OVRManager.isHmdPresent ? OVRPlugin.GetNodeVelocity(OVRPlugin.Node.EyeCenter, OVRPlugin.Step.Render).FromFlippedZVector3f() : Vector3.zero;
#else
            return Vector3.zero;
#endif
        }

        /// <summary>
        /// The GetHeadsetAngularVelocity method is used to determine the current angular velocity of the headset.
        /// </summary>
        /// <returns>A Vector3 containing the current angular velocity of the headset.</returns>
        public override Vector3 GetHeadsetAngularVelocity()
        {
            SetHeadsetCaches();
            return cachedHeadsetVelocityEstimator.GetAngularVelocityEstimate();
        }

        /// <summary>
        /// The HeadsetFade method is used to apply a fade to the headset camera to progressively change the colour.
        /// </summary>
        /// <param name="color">The colour to fade to.</param>
        /// <param name="duration">The amount of time the fade should take to reach the given colour.</param>
        /// <param name="fadeOverlay">Determines whether to use an overlay on the fade.</param>
        public override void HeadsetFade(Color color, float duration, bool fadeOverlay = false)
        {
            VRTK_ScreenFade.Start(color, duration);
        }

        /// <summary>
        /// The HasHeadsetFade method checks to see if the given game object (usually the camera) has the ability to fade the viewpoint.
        /// </summary>
        /// <param name="obj">The Transform to check to see if a camera fade is available on.</param>
        /// <returns>Returns true if the headset has fade functionality on it.</returns>
        public override bool HasHeadsetFade(Transform obj)
        {
            if (obj.GetComponentInChildren<VRTK_ScreenFade>())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// The AddHeadsetFade method attempts to add the fade functionality to the game object with the camera on it.
        /// </summary>
        /// <param name="camera">The Transform to with the camera on to add the fade functionality to.</param>
        public override void AddHeadsetFade(Transform camera)
        {
            if (camera && !camera.GetComponent<VRTK_ScreenFade>())
            {
                camera.gameObject.AddComponent<VRTK_ScreenFade>();
            }
        }

        protected virtual void SetHeadsetCaches()
        {
            Transform currentHeadset = GetHeadset();
            if (cachedHeadsetVelocityEstimator == null && currentHeadset != null)
            {
                cachedHeadsetVelocityEstimator = (currentHeadset.GetComponent<VRTK_VelocityEstimator>() != null ? currentHeadset.GetComponent<VRTK_VelocityEstimator>() : currentHeadset.gameObject.AddComponent<VRTK_VelocityEstimator>());
            }
        }
#endif
    }
}