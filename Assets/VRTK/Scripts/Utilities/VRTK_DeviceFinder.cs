// Device Finder|Utilities|90020
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.VR;

    /// <summary>
    /// The Device Finder offers a collection of static methods that can be called to find common game devices such as the headset or controllers, or used to determine key information about the connected devices.
    /// </summary>
    public static class VRTK_DeviceFinder
    {
        /// <summary>
        /// Possible devices.
        /// </summary>
        /// <param name="Headset">The headset.</param>
        /// <param name="LeftController">The left hand controller.</param>
        /// <param name="RightController">The right hand controller.</param>
        public enum Devices
        {
            Headset,
            LeftController,
            RightController,
        }

        /// <summary>
        /// Possible headsets
        /// </summary>
        /// <param name="Unknown">An unknown headset.</param>
        /// <param name="OculusRift">A summary of all Oculus Rift headset versions.</param>
        /// <param name="OculusRiftCV1">A specific version of the Oculus Rift headset, the Consumer Version 1.</param>
        /// <param name="Vive">A summary of all HTC Vive headset versions.</param>
        /// <param name="ViveMV">A specific version of the HTC Vive headset, the first consumer version.</param>
        public enum Headsets
        {
            Unknown,
            OculusRift,
            OculusRiftCV1,
            Vive,
            ViveMV
        }

        /// <summary>
        /// The GetControllerIndex method is used to find the index of a given controller object.
        /// </summary>
        /// <param name="controller">The controller object to get the index of a controller.</param>
        /// <returns>The index of the given controller.</returns>
        public static uint GetControllerIndex(GameObject controller)
        {
            return VRTK_SDK_Bridge.GetControllerIndex(controller);
        }

        /// <summary>
        /// The GetControllerByIndex method is used to find a controller based on it's unique index.
        /// </summary>
        /// <param name="index">The index of the actual controller to find.</param>
        /// <param name="getActual">An optional parameter that if true will return the game object that the SDK controller is attached to.</param>
        /// <returns>The actual controller GameObject that matches the given index.</returns>
        public static GameObject GetControllerByIndex(uint index, bool getActual)
        {
            return VRTK_SDK_Bridge.GetControllerByIndex(index, getActual);
        }

        /// <summary>
        /// The GetControllerOrigin method is used to find the controller's origin.
        /// </summary>
        /// <param name="controller">The GameObject to get the origin for.</param>
        /// <returns>The transform of the controller origin or if an origin is not set then the transform parent.</returns>
        public static Transform GetControllerOrigin(GameObject controller)
        {
            return VRTK_SDK_Bridge.GetControllerOrigin(controller);
        }

        /// <summary>
        /// The DeviceTransform method returns the transform for a given Devices enum.
        /// </summary>
        /// <param name="device">The Devices enum to get the transform for.</param>
        /// <returns>The transform for the given Devices enum.</returns>
        public static Transform DeviceTransform(Devices device)
        {
            switch (device)
            {
                case Devices.Headset:
                    return HeadsetTransform();
                case Devices.LeftController:
                    return GetControllerLeftHand().transform;
                case Devices.RightController:
                    return GetControllerRightHand().transform;
            }
            return null;
        }

        /// <summary>
        /// The GetControllerHandType method is used for getting the enum representation of ControllerHand from a given string.
        /// </summary>
        /// <param name="hand">The string representation of the hand to retrieve the type of. `left` or `right`.</param>
        /// <returns>A ControllerHand representing either the Left or Right hand.</returns>
        public static SDK_BaseController.ControllerHand GetControllerHandType(string hand)
        {
            switch (hand.ToLower())
            {
                case "left":
                    return SDK_BaseController.ControllerHand.Left;
                case "right":
                    return SDK_BaseController.ControllerHand.Right;
                default:
                    return SDK_BaseController.ControllerHand.None;
            }
        }

        /// <summary>
        /// The GetControllerHand method is used for getting the enum representation of ControllerHand for the given controller game object.
        /// </summary>
        /// <param name="controller">The controller game object to check the hand of.</param>
        /// <returns>A ControllerHand representing either the Left or Right hand.</returns>
        public static SDK_BaseController.ControllerHand GetControllerHand(GameObject controller)
        {
            if (VRTK_SDK_Bridge.IsControllerLeftHand(controller))
            {
                return SDK_BaseController.ControllerHand.Left;
            }
            else if (VRTK_SDK_Bridge.IsControllerRightHand(controller))
            {
                return SDK_BaseController.ControllerHand.Right;
            }
            else
            {
                return SDK_BaseController.ControllerHand.None;
            }
        }

        /// <summary>
        /// The GetControllerLeftHand method retrieves the game object for the left hand controller.
        /// </summary>
        /// <param name="getActual">An optional parameter that if true will return the game object that the SDK controller is attached to.</param>
        /// <returns>The left hand controller.</returns>
        public static GameObject GetControllerLeftHand(bool getActual = false)
        {
            return VRTK_SDK_Bridge.GetControllerLeftHand(getActual);
        }

        /// <summary>
        /// The GetControllerRightHand method retrieves the game object for the right hand controller.
        /// </summary>
        /// <param name="getActual">An optional parameter that if true will return the game object that the SDK controller is attached to.</param>
        /// <returns>The right hand controller.</returns>
        public static GameObject GetControllerRightHand(bool getActual = false)
        {
            return VRTK_SDK_Bridge.GetControllerRightHand(getActual);
        }

        /// <summary>
        /// The IsControllerOfHand method is used to check if a given controller game object is of the hand type provided.
        /// </summary>
        /// <param name="checkController">The actual controller object that is being checked.</param>
        /// <param name="hand">The representation of a hand to check if the given controller matches.</param>
        /// <returns>Is true if the given controller matches the given hand.</returns>
        public static bool IsControllerOfHand(GameObject checkController, SDK_BaseController.ControllerHand hand)
        {
            switch (hand)
            {
                case SDK_BaseController.ControllerHand.Left:
                    return (IsControllerLeftHand(checkController));
                case SDK_BaseController.ControllerHand.Right:
                    return (IsControllerRightHand(checkController));
            }

            return false;
        }

        /// <summary>
        /// The IsControllerLeftHand method is used to check if a given controller game object is the left handed controller.
        /// </summary>
        /// <param name="checkController">The controller object that is being checked.</param>
        /// <returns>Is true if the given controller is the left controller.</returns>
        public static bool IsControllerLeftHand(GameObject checkController)
        {
            return VRTK_SDK_Bridge.IsControllerLeftHand(checkController);
        }

        /// <summary>
        /// The IsControllerRightHand method is used to check if a given controller game object is the right handed controller.
        /// </summary>
        /// <param name="checkController">The controller object that is being checked.</param>
        /// <returns>Is true if the given controller is the right controller.</returns>
        public static bool IsControllerRightHand(GameObject checkController)
        {
            return VRTK_SDK_Bridge.IsControllerRightHand(checkController);
        }

        /// <summary>
        /// The GetActualController method will attempt to get the actual SDK controller object.
        /// </summary>
        /// <param name="givenController">The GameObject of the controller.</param>
        /// <returns>The GameObject that is the actual controller.</returns>
        public static GameObject GetActualController(GameObject givenController)
        {
            if (VRTK_SDK_Bridge.IsControllerLeftHand(givenController, true) || VRTK_SDK_Bridge.IsControllerRightHand(givenController, true))
            {
                return givenController;
            }

            if (VRTK_SDK_Bridge.IsControllerLeftHand(givenController, false))
            {
                return VRTK_SDK_Bridge.GetControllerLeftHand(true);
            }

            if (VRTK_SDK_Bridge.IsControllerRightHand(givenController, false))
            {
                return VRTK_SDK_Bridge.GetControllerRightHand(true);
            }

            return null;
        }

        /// <summary>
        /// The GetScriptAliasController method will attempt to get the object that contains the scripts for the controller.
        /// </summary>
        /// <param name="givenController">The GameObject of the controller.</param>
        /// <returns>The GameObject that is the alias controller containing the scripts.</returns>
        public static GameObject GetScriptAliasController(GameObject givenController)
        {
            if (VRTK_SDK_Bridge.IsControllerLeftHand(givenController, false) || VRTK_SDK_Bridge.IsControllerRightHand(givenController, false))
            {
                return givenController;
            }

            if (VRTK_SDK_Bridge.IsControllerLeftHand(givenController, true))
            {
                return VRTK_SDK_Bridge.GetControllerLeftHand(false);
            }

            if (VRTK_SDK_Bridge.IsControllerRightHand(givenController, true))
            {
                return VRTK_SDK_Bridge.GetControllerRightHand(false);
            }

            return null;
        }

        /// <summary>
        /// The GetModelAliasController method will attempt to get the object that contains the model for the controller.
        /// </summary>
        /// <param name="givenController">The GameObject of the controller.</param>
        /// <returns>The GameObject that is the alias controller containing the controller model.</returns>
        public static GameObject GetModelAliasController(GameObject givenController)
        {
            return VRTK_SDK_Bridge.GetControllerModel(givenController);
        }

        /// <summary>
        /// The GetModelAliasControllerHand method will return the hand that the given model alias GameObject is for.
        /// </summary>
        /// <param name="givenObject">The GameObject that may represent a model alias.</param>
        /// <returns>The enum of the ControllerHand that the given GameObject may represent.</returns>
        public static SDK_BaseController.ControllerHand GetModelAliasControllerHand(GameObject givenObject)
        {
            if (GetModelAliasController(GetControllerLeftHand()) == givenObject)
            {
                return SDK_BaseController.ControllerHand.Left;
            }
            else if (GetModelAliasController(GetControllerRightHand()) == givenObject)
            {
                return SDK_BaseController.ControllerHand.Right;
            }
            return SDK_BaseController.ControllerHand.None;
        }

        /// <summary>
        /// The GetControllerVelocity method is used for getting the current velocity of the physical game controller. This can be useful to determine the speed at which the controller is being swung or the direction it is being moved in.
        /// </summary>
        /// <param name="givenController">The GameObject of the controller.</param>
        /// <returns>A 3 dimensional vector containing the current real world physical controller velocity.</returns>
        public static Vector3 GetControllerVelocity(GameObject givenController)
        {
            var controllerIndex = GetControllerIndex(givenController);
            return VRTK_SDK_Bridge.GetVelocityOnIndex(controllerIndex);
        }

        /// <summary>
        /// The GetControllerAngularVelocity method is used for getting the current rotational velocity of the physical game controller. This can be useful for determining which way the controller is being rotated and at what speed the rotation is occurring.
        /// </summary>
        /// <param name="givenController">The GameObject of the controller.</param>
        /// <returns>A 3 dimensional vector containing the current real world physical controller angular (rotational) velocity.</returns>
        public static Vector3 GetControllerAngularVelocity(GameObject givenController)
        {
            var controllerIndex = GetControllerIndex(givenController);
            return VRTK_SDK_Bridge.GetAngularVelocityOnIndex(controllerIndex);
        }

        /// <summary>
        /// The GetHeadsetVelocity method is used to determine the current velocity of the headset.
        /// </summary>
        /// <returns>A Vector3 containing the current velocity of the headset.</returns>
        public static Vector3 GetHeadsetVelocity()
        {
            return VRTK_SDK_Bridge.GetHeadsetVelocity();
        }

        /// <summary>
        /// The GetHeadsetAngularVelocity method is used to determine the current angular velocity of the headset.
        /// </summary>
        /// <returns>A Vector3 containing the current angular velocity of the headset.</returns>
        public static Vector3 GetHeadsetAngularVelocity()
        {
            return VRTK_SDK_Bridge.GetHeadsetAngularVelocity();
        }

        /// <summary>
        /// The HeadsetTransform method is used to retrieve the transform for the VR Headset in the scene. It can be useful to determine the position of the user's head in the game world.
        /// </summary>
        /// <returns>The transform of the VR Headset component.</returns>
        public static Transform HeadsetTransform()
        {
            return VRTK_SDK_Bridge.GetHeadset();
        }

        /// <summary>
        /// The HeadsetCamera method is used to retrieve the transform for the VR Camera in the scene.
        /// </summary>
        /// <returns>The transform of the VR Camera component.</returns>
        public static Transform HeadsetCamera()
        {
            return VRTK_SDK_Bridge.GetHeadsetCamera();
        }

        /// <summary>
        /// The GetHeadsetType method returns the type of headset connected to the computer.
        /// </summary>
        /// <param name="summary">If this is true, then the generic name for the headset is returned not including the version type (e.g. OculusRift will be returned for DK2 and CV1).</param>
        /// <returns>The Headset type that is connected.</returns>
        public static Headsets GetHeadsetType(bool summary = false)
        {
            Headsets returnValue = Headsets.Unknown;
            string checkValue = VRDevice.model;
            switch (checkValue)
            {
                case "Oculus Rift CV1":
                    returnValue = (summary ? Headsets.OculusRift : Headsets.OculusRiftCV1);
                    break;
                case "Vive MV":
                    returnValue = (summary ? Headsets.Vive : Headsets.ViveMV);
                    break;
            }
            return returnValue;
        }

        /// <summary>
        /// The PlayAreaTransform method is used to retrieve the transform for the play area in the scene.
        /// </summary>
        /// <returns>The transform of the VR Play Area component.</returns>
        public static Transform PlayAreaTransform()
        {
            return VRTK_SDK_Bridge.GetPlayArea();
        }
    }
}