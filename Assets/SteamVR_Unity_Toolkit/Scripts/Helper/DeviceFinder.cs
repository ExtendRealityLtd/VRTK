namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Valve.VR;

    public class DeviceFinder : MonoBehaviour
    {
        //Seconds to keep trying to initialise
        public static float initTries = 15f;

        public static SteamVR_TrackedObject ControllerByIndex(uint index)
        {
            var system = OpenVR.System;
            if (system != null && system.GetTrackedDeviceClass(index) == ETrackedDeviceClass.Controller)
            {
                return TrackedObjectByIndex(index);
            }
            return null;
        }

        public static SteamVR_TrackedObject TrackedObjectByIndex(uint controllerIndex)
        {
            foreach (SteamVR_TrackedObject trackedObject in GameObject.FindObjectsOfType<SteamVR_TrackedObject>())
            {
                if ((uint)trackedObject.index == controllerIndex)
                {
                    return trackedObject;
                }
            }
            return null;
        }

        public static Transform HeadsetTransform()
        {
#if (UNITY_5_4_OR_NEWER)
            return GameObject.FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();
#else
            return GameObject.FindObjectOfType<SteamVR_GameView>().GetComponent<Transform>();
#endif
        }
    }
}