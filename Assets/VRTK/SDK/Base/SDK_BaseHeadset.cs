// Base Headset|SDK_Base|005
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// The Base Headset SDK script provides a bridge to SDK methods that deal with the VR Headset.
    /// </summary>
    /// <remarks>
    /// This is an abstract class to implement the interface required by all implemented SDKs.
    /// </remarks>
    public abstract class SDK_BaseHeadset : SDK_Base
    {
        protected Transform cachedHeadset;
        protected Transform cachedHeadsetCamera;

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public abstract void ProcessUpdate(Dictionary<string, object> options);

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public abstract void ProcessFixedUpdate(Dictionary<string, object> options);

        /// <summary>
        /// The GetHeadset method returns the Transform of the object that is used to represent the headset in the scene.
        /// </summary>
        /// <returns>A transform of the object representing the headset in the scene.</returns>
        public abstract Transform GetHeadset();

        /// <summary>
        /// The GetHeadsetCamera method returns the Transform of the object that is used to hold the headset camera in the scene.
        /// </summary>
        /// <returns>A transform of the object holding the headset camera in the scene.</returns>
        public abstract Transform GetHeadsetCamera();

        /// <summary>
        /// The GetHeadsetVelocity method is used to determine the current velocity of the headset.
        /// </summary>
        /// <returns>A Vector3 containing the current velocity of the headset.</returns>
        public abstract Vector3 GetHeadsetVelocity();

        /// <summary>
        /// The GetHeadsetAngularVelocity method is used to determine the current angular velocity of the headset.
        /// </summary>
        /// <returns>A Vector3 containing the current angular velocity of the headset.</returns>
        public abstract Vector3 GetHeadsetAngularVelocity();

        /// <summary>
        /// The HeadsetFade method is used to apply a fade to the headset camera to progressively change the colour.
        /// </summary>
        /// <param name="color">The colour to fade to.</param>
        /// <param name="duration">The amount of time the fade should take to reach the given colour.</param>
        /// <param name="fadeOverlay">Determines whether to use an overlay on the fade.</param>
        public abstract void HeadsetFade(Color color, float duration, bool fadeOverlay = false);

        /// <summary>
        /// The HasHeadsetFade method checks to see if the given game object (usually the camera) has the ability to fade the viewpoint.
        /// </summary>
        /// <param name="obj">The Transform to check to see if a camera fade is available on.</param>
        /// <returns>Returns true if the headset has fade functionality on it.</returns>
        public abstract bool HasHeadsetFade(Transform obj);

        /// <summary>
        /// The AddHeadsetFade method attempts to add the fade functionality to the game object with the camera on it.
        /// </summary>
        /// <param name="camera">The Transform to with the camera on to add the fade functionality to.</param>
        public abstract void AddHeadsetFade(Transform camera);

        protected Transform GetSDKManagerHeadset()
        {
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null && sdkManager.loadedSetup.actualHeadset != null)
            {
                cachedHeadset = (sdkManager.loadedSetup.actualHeadset ? sdkManager.loadedSetup.actualHeadset.transform : null);
                return cachedHeadset;
            }
            return null;
        }
    }
}