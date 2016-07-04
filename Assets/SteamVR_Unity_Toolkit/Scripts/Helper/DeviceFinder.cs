namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Valve.VR;

    public class DeviceFinder : MonoBehaviour
    {
        public enum ControllerHand
        {
            None,
            Left,
            Right
        }

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

        public static ControllerHand GetControllerHandType(string hand)
        {
            switch(hand.ToLower())
            {
                case "left":
                    return ControllerHand.Left;
                case "right":
                    return ControllerHand.Right;
                default:
                    return ControllerHand.None;
            }
        }

        public static bool IsControllerOfHand(GameObject checkController, ControllerHand hand)
        {
            var controllerManager = GameObject.FindObjectOfType<SteamVR_ControllerManager>();

            if (hand == ControllerHand.Left && controllerManager && controllerManager.left == checkController)
            {
                return true;
            }

            if (hand == ControllerHand.Right && controllerManager && controllerManager.right == checkController)
            {
                return true;
            }

            return false;
        }

        public static Transform HeadsetTransform()
        {
#if (UNITY_5_4_OR_NEWER)
            return GameObject.FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();
#else
            return GameObject.FindObjectOfType<SteamVR_GameView>().GetComponent<Transform>();
#endif
        }

        public static Transform HeadsetCamera()
        {
            return GameObject.FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();
        }
    }
}