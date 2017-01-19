// SteamVR Headset|SDK_SteamVR|002
namespace VRTK
{
#if VRTK_SDK_STEAMVR
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// The SteamVR Headset SDK script provides a bridge to the SteamVR SDK.
    /// </summary>
    public class SDK_SteamVRHeadset : SDK_BaseHeadset
    {
        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(Dictionary<string, object> options)
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
#if (UNITY_5_4_OR_NEWER)
                var foundCamera = FindObjectOfType<SteamVR_Camera>();
#else
                var foundCamera = FindObjectOfType<SteamVR_GameView>();
#endif
                if (foundCamera)
                {
                    cachedHeadset = foundCamera.transform;
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
                var foundCamera = FindObjectOfType<SteamVR_Camera>();
                if (foundCamera)
                {
                    cachedHeadsetCamera = foundCamera.transform;
                }
            }
            return cachedHeadsetCamera;
        }

        /// <summary>
        /// The GetHeadsetVelocity method is used to determine the current velocity of the headset.
        /// </summary>
        /// <returns>A Vector3 containing the current velocity of the headset.</returns>
        public override Vector3 GetHeadsetVelocity()
        {
            return SteamVR_Controller.Input((int)SteamVR_TrackedObject.EIndex.Hmd).velocity;
        }

        /// <summary>
        /// The GetHeadsetAngularVelocity method is used to determine the current angular velocity of the headset.
        /// </summary>
        /// <returns>A Vector3 containing the current angular velocity of the headset.</returns>
        public override Vector3 GetHeadsetAngularVelocity()
        {
            return SteamVR_Controller.Input((int)SteamVR_TrackedObject.EIndex.Hmd).angularVelocity;
        }

        /// <summary>
        /// The HeadsetFade method is used to apply a fade to the headset camera to progressively change the colour.
        /// </summary>
        /// <param name="color">The colour to fade to.</param>
        /// <param name="duration">The amount of time the fade should take to reach the given colour.</param>
        /// <param name="fadeOverlay">Determines whether to use an overlay on the fade.</param>
        public override void HeadsetFade(Color color, float duration, bool fadeOverlay = false)
        {
            SteamVR_Fade.Start(color, duration, fadeOverlay);
        }

        /// <summary>
        /// The HasHeadsetFade method checks to see if the given game object (usually the camera) has the ability to fade the viewpoint.
        /// </summary>
        /// <param name="obj">The Transform to check to see if a camera fade is available on.</param>
        /// <returns>Returns true if the headset has fade functionality on it.</returns>
        public override bool HasHeadsetFade(Transform obj)
        {
            if (obj.GetComponentInChildren<SteamVR_Fade>())
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
            if (camera && !camera.GetComponent<SteamVR_Fade>())
            {
                camera.gameObject.AddComponent<SteamVR_Fade>();
            }
        }
    }
#else
    public class SDK_SteamVRHeadset : SDK_FallbackHeadset
    {
    }
#endif
}