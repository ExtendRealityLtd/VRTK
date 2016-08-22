namespace VRTK
{
    using UnityEngine;

    public class VRTK_DeviceFinder : MonoBehaviour
    {
        public enum ControllerHand
        {
            None,
            Left,
            Right
        }

        public static bool TrackedIndexIsController(uint index)
        {
            return VRTK_SDK_Bridge.TrackedIndexIsController(index);
        }

        public static uint GetControllerIndex(GameObject controller)
        {
            return VRTK_SDK_Bridge.GetIndexOfTrackedObject(controller);
        }

        public static GameObject TrackedObjectByIndex(uint index)
        {
            return VRTK_SDK_Bridge.GetTrackedObjectByIndex(index);
        }

        public static Transform TrackedObjectOrigin(GameObject obj)
        {
            return VRTK_SDK_Bridge.GetTrackedObjectOrigin(obj);
        }

        public static ControllerHand GetControllerHandType(string hand)
        {
            switch (hand.ToLower())
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
            if (VRTK_SDK_Bridge.IsControllerLeftHand(controller))
            {
                return ControllerHand.Left;
            }
            else if (VRTK_SDK_Bridge.IsControllerRightHand(controller))
            {
                return ControllerHand.Right;
            }
            else
            {
                return ControllerHand.None;
            }
        }

        public static bool IsControllerOfHand(GameObject checkController, ControllerHand hand)
        {
            if (hand == ControllerHand.Left && VRTK_SDK_Bridge.IsControllerLeftHand(checkController))
            {
                return true;
            }

            if (hand == ControllerHand.Right && VRTK_SDK_Bridge.IsControllerRightHand(checkController))
            {
                return true;
            }

            return false;
        }

        public static Transform HeadsetTransform()
        {
            return VRTK_SDK_Bridge.GetHeadset();
        }

        public static Transform HeadsetCamera()
        {
            return VRTK_SDK_Bridge.GetHeadsetCamera();
        }

        public static Transform PlayAreaTransform()
        {
            return VRTK_SDK_Bridge.GetPlayArea();
        }
    }
}