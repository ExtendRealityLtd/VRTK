// WindowsMR Headset|SDK_WindowsMR|003
namespace VRTK
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The WindowsMR Headset SDK script provides a bridge to the WindowsMR XR.
    /// </summary>
    [SDK_Description(typeof(SDK_WindowsMR))]
    public class SDK_WindowsMRHeadset
#if VRTK_DEFINE_SDK_WINDOWSMR && UNITY_2017_2_OR_NEWER
        : SDK_BaseHeadset
#else
        : SDK_FallbackHeadset
#endif
    {
#if VRTK_DEFINE_SDK_WINDOWSMR && UNITY_2017_2_OR_NEWER
        protected Vector3 currentHeadsetPosition;
        protected Vector3 previousHeadsetPosition;
        protected Vector3 currentHeadsetVelocity;

        protected Quaternion currentHeadsetRotation;
        protected Quaternion previousHeadsetRotation;

        #region Overriden base functions
        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public override void ProcessFixedUpdate(Dictionary<string, object> options)
        {
            UpdateVelocity();
            UpdateRotation();
        }

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(Dictionary<string, object> options)
        {
            UpdateVelocity();
            UpdateRotation();
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
                WindowsMR_Camera foundCamera = VRTK_SharedMethods.FindEvenInactiveComponent<WindowsMR_Camera>(true);

                if (foundCamera != null)
                {
                    cachedHeadset = foundCamera.transform;
                }
            }

            return cachedHeadset;
        }

        /// <summary>
        /// The GetHeadsetAngularVelocity method is used to determine the current angular velocity of the headset.
        /// </summary>
        /// <returns>A Vector3 containing the current angular velocity of the headset.</returns>
        public override Vector3 GetHeadsetAngularVelocity()
        {
            Quaternion deltaRotation = currentHeadsetRotation * Quaternion.Inverse(previousHeadsetRotation);
            return new Vector3(Mathf.DeltaAngle(0, deltaRotation.eulerAngles.x), Mathf.DeltaAngle(0, deltaRotation.eulerAngles.y), Mathf.DeltaAngle(0, deltaRotation.eulerAngles.z));
        }

        /// <summary>
        /// The GetHeadsetCamera method returns the Transform of the object that is used to hold the headset camera in the scene.
        /// </summary>
        /// <returns>A transform of the object holding the headset camera in the scene.</returns>
        public override Transform GetHeadsetCamera()
        {
            // For Immersive MR the camera is the same as the headset.
            return GetHeadset();
        }

        /// <summary>
        /// The GetHeadsetVelocity method is used to determine the current velocity of the headset.
        /// </summary>
        /// <returns>A Vector3 containing the current velocity of the headset.</returns>
        public override Vector3 GetHeadsetVelocity()
        {
            UpdateVelocity();
            return currentHeadsetVelocity;
        }

        /// <summary>
        /// The HasHeadsetFade method checks to see if the given game object (usually the camera) has the ability to fade the viewpoint.
        /// </summary>
        /// <param name="obj">The Transform to check to see if a camera fade is available on.</param>
        /// <returns>Returns true if the headset has fade functionality on it.</returns>
        public override bool HasHeadsetFade(Transform obj)
        {
            if (obj.GetComponentInChildren<VRTK_ScreenFade>() != null)
            {
                return true;
            }
            return false;
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
        /// The AddHeadsetFade method attempts to add the fade functionality to the game object with the camera on it.
        /// </summary>
        /// <param name="camera">The Transform to with the camera on to add the fade functionality to.</param>
        public override void AddHeadsetFade(Transform camera)
        {
            if (camera != null && camera.GetComponent<VRTK_ScreenFade>() == null)
            {
                camera.gameObject.AddComponent<VRTK_ScreenFade>();
            }
        }

        /// <summary>
        /// The GetHeadsetType method returns a string representing the type of headset connected.
        /// </summary>
        /// <returns>The string of the headset connected.</returns>
        public override string GetHeadsetType()
        {
            return CleanPropertyString("windowsmixedreality");
        }
        #endregion

        /// <summary>
        /// Update rotation values of headset.
        /// </summary>
        protected virtual void UpdateRotation()
        {
            previousHeadsetRotation = currentHeadsetRotation;
            currentHeadsetRotation = GetHeadset().transform.rotation;
        }

        /// <summary>
        /// Update velocity values of headset.
        /// </summary>
        protected virtual void UpdateVelocity()
        {
            previousHeadsetPosition = currentHeadsetPosition;
            currentHeadsetPosition = GetHeadset().transform.position;
            currentHeadsetVelocity = (previousHeadsetPosition - currentHeadsetPosition) / Time.deltaTime;
        }
#endif
    }
}
