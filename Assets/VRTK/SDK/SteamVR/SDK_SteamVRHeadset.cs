// SteamVR Headset|SDK|002
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The SteamVR Headset SDK script provides a bridge to the SteamVR SDK.
    /// </summary>
    public class SDK_SteamVRHeadset : SDK_BaseHeadset
    {
#if VRTK_SDK_HEADSET_STEAMVR
        /// <summary>
        /// The GetHeadset method returns the Transform of the object that is used to represent the headset in the scene.
        /// </summary>
        /// <returns>A transform of the object representing the headset in the scene.</returns>
        public override Transform GetHeadset()
        {
            if (cachedHeadset == null)
            {
#if (UNITY_5_4_OR_NEWER)
                cachedHeadset = FindObjectOfType<SteamVR_Camera>().transform;
#else
                cachedHeadset = FindObjectOfType<SteamVR_GameView>().transform;
#endif
            }
            return cachedHeadset;
        }

        /// <summary>
        /// The GetHeadsetCamera/0 method returns the Transform of the object that is used to hold the headset camera in the scene.
        /// </summary>
        /// <returns>A transform of the object holding the headset camera in the scene.</returns>
        public override Transform GetHeadsetCamera()
        {
            if (cachedHeadsetCamera == null)
            {
                cachedHeadsetCamera = FindObjectOfType<SteamVR_Camera>().transform;
            }
            return cachedHeadsetCamera;
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
            if (camera && !camera.gameObject.GetComponent<SteamVR_Fade>())
            {
                camera.gameObject.AddComponent<SteamVR_Fade>();
            }
        }
#endif
    }
}