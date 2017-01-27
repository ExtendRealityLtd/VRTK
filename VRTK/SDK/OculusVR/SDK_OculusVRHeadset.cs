// OculusVR Headset|SDK_OculusVR|002
namespace VRTK
{
#if VRTK_SDK_OCULUSVR
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// The OculusVR Headset SDK script provides a bridge to the OculusVR SDK.
    /// </summary>
    public class SDK_OculusVRHeadset : SDK_BaseHeadset
    {
        private Quaternion previousHeadsetRotation;
        private Quaternion currentHeadsetRotation;

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(Dictionary<string, object> options)
        {
            var device = GetHeadset();
            previousHeadsetRotation = currentHeadsetRotation;
            currentHeadsetRotation = device.transform.rotation;
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
                var ovrManager = FindObjectOfType<OVRManager>();
                if (ovrManager)
                {
                    cachedHeadset = ovrManager.transform.FindChild("TrackingSpace/CenterEyeAnchor");
                }
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
        /// The GetHeadsetVelocity method is used to determine the current velocity of the headset.
        /// </summary>
        /// <returns>A Vector3 containing the current velocity of the headset.</returns>
        public override Vector3 GetHeadsetVelocity()
        {
            return OVRManager.isHmdPresent ? OVRPlugin.GetEyeVelocity(OVRPlugin.Eye.Left).ToOVRPose().position : Vector3.zero;
        }

        /// <summary>
        /// The GetHeadsetAngularVelocity method is used to determine the current angular velocity of the headset.
        /// </summary>
        /// <returns>A Vector3 containing the current angular velocity of the headset.</returns>
        public override Vector3 GetHeadsetAngularVelocity()
        {
            var deltaRotation = currentHeadsetRotation * Quaternion.Inverse(previousHeadsetRotation);
            return new Vector3(Mathf.DeltaAngle(0, deltaRotation.eulerAngles.x), Mathf.DeltaAngle(0, deltaRotation.eulerAngles.y), Mathf.DeltaAngle(0, deltaRotation.eulerAngles.z));
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
    }
#else
    public class SDK_OculusVRHeadset : SDK_FallbackHeadset
    {
    }
#endif
}