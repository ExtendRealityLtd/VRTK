﻿namespace VRTK
{
    using UnityEngine;
    using UnityEngine.XR;

    /// <summary>
    /// Camera script for the main camera for Immersive Mixed Reality. 
    /// </summary>
    [RequireComponent (typeof(Camera))]
    public class WindowsMR_Camera : MonoBehaviour
    {
        [SerializeField]
        [Tooltip ("Force the Tracking Space Type to be RoomScale (normal VR experiences). If false, Stationary will be forced (e.g. video experiences.")]
        private bool forceRoomScaleTracking = true;

        public bool ForceRoomScaleTracking
        {
            get { return forceRoomScaleTracking; }
            set { forceRoomScaleTracking = value; }
        }

        /// <summary>
        /// Name of the Windows Mixed Reality Device as listed in XRSettings.
        /// </summary>
        private const string DEVICE_NAME = "WindowsMR";
        
        void Awake()
        {
            if (CheckForMixedRealitySupport())
            {
                SetupMRCamera();
            }
        }

        private void Update()
        {
            if (XRDevice.GetTrackingSpaceType() != TrackingSpaceType.RoomScale && forceRoomScaleTracking)
            {
                XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
            }

            if (XRDevice.GetTrackingSpaceType() != TrackingSpaceType.Stationary && !forceRoomScaleTracking)
            {
                XRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);
            }
            
        }

        /// <summary>
        /// Check if the Mixed (Virtual) Reality Settings are properly set.
        /// </summary>
        /// <returns>Are the settings set.</returns>
        private bool CheckForMixedRealitySupport()
        {
            if(XRSettings.enabled == false)
            {
                Debug.LogError("XRSettings are not enabled. Enable in PlayerSettings. Do not forget to add Windows Mixed Reality to Virtual Reality SDKs.");
                return false;
            }
            else
            {
                foreach(string device in XRSettings.supportedDevices)
                {
                    if(device.Equals("WindowsMR"))
                    {
                        return true;
                    }
                }
                Debug.LogError("Windows Mixed Reality is not supported in XRSettings, add in PlayerSettings.");
            }

            return false;
        }

        /// <summary>
        /// Setup the MR camera properly.
        /// </summary>
        private void SetupMRCamera()
        {
            Camera camera = GetComponent<Camera>();
            if (camera.tag != "MainCamera")
            {
                camera.tag = "MainCamera";
            }

            camera.nearClipPlane = 0.01f;

            if(camera.stereoTargetEye != StereoTargetEyeMask.Both)
            {
                Debug.LogError("Target eye of main camera is not set to both. Are you sure you want to render only one eye?");
            }
        }
    }
}