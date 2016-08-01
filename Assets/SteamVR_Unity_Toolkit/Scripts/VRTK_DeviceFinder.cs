namespace VRTK
{
    using UnityEngine;
    using Valve.VR;

    public class VRTK_DeviceFinder : MonoBehaviour
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

        public static uint GetControllerIndex(GameObject controller)
        {
            var obj = controller.GetComponent<SteamVR_TrackedObject>();
            if (obj)
            {
                return (uint)obj.index;
            }
            return 0;
        }

        public static SteamVR_TrackedObject TrackedObjectByIndex(uint controllerIndex)
        {
            foreach (SteamVR_TrackedObject trackedObject in FindObjectsOfType<SteamVR_TrackedObject>())
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

        public static ControllerHand GetControllerHand(GameObject controller)
        {
            var controllerManager = FindObjectOfType<SteamVR_ControllerManager>();

            if (controllerManager && controller == controllerManager.left)
            {
                return ControllerHand.Left;
            }

            if (controllerManager && controller == controllerManager.right)
            {
                return ControllerHand.Right;
            }

            return ControllerHand.None;
        }

        public static bool IsControllerOfHand(GameObject checkController, ControllerHand hand)
        {
            var controllerManager = FindObjectOfType<SteamVR_ControllerManager>();

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
            return FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();
#else
            return FindObjectOfType<SteamVR_GameView>().GetComponent<Transform>();
#endif
        }

        public static Transform HeadsetCamera()
        {
            return FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();
        }
    }
}