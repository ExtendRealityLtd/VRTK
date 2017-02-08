// LeapMotion System|SDK_LeapMotion|001
namespace VRTK
{
#if VRTK_DEFINE_SDK_LEAPMOTION
    using UnityEngine;
    using System.Collections.Generic;
    using System;
    using Leap.Unity;
#endif

    /// <summary>
    /// The LeapMotion Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    [SDK_Description("LeapMotion", SDK_LeapMotionDefines.ScriptingDefineSymbol)]
    public class SDK_LeapMotionController
#if VRTK_DEFINE_SDK_LEAPMOTION
        : SDK_BaseController
#else
        : SDK_FallbackController
#endif
    {
#if VRTK_DEFINE_SDK_LEAPMOTION
        private const uint leftHandControllerIndex = 0;
        private const uint rightHandControllerIndex = 1;
        private IHandModel leftHand;
        private IHandModel rightHand;
        private PinchDetector leftPinchDetector;
        private PinchDetector rightPinchDetector;
        private Rigidbody leftHandRigidBody;
        private Rigidbody rightHandRigidBody;
        private VelocityTracker leftHandVelocityTracker;
        private VelocityTracker rightHandVelocityTracker;

        private IHandModel LeftHand
        {
            get
            {
                if (rightHand == null)
                {
                    GameObject rightController = GetControllerRightHand(true);
                    rightHand = rightController.GetComponent<IHandModel>();
                }
                return rightHand;
            }
        }
        private IHandModel RightHand
        {
            get
            {
                if (rightHand == null)
                {
                    GameObject rightController = GetControllerRightHand(true);
                    rightHand = rightController.GetComponent<IHandModel>();
                }
                return rightHand;
            }
        }
        private PinchDetector LeftPinchDetector
        {
            get
            {
                if (leftPinchDetector == null)
                {
                    GameObject leftController = GetControllerLeftHand(true);
                    leftPinchDetector = leftController.GetComponent<PinchDetector>();
                }
                return leftPinchDetector;
            }
        }
        private PinchDetector RightPinchDetector
        {
            get
            {
                if (rightPinchDetector == null)
                {
                    GameObject rightController = GetControllerRightHand(true);
                    rightPinchDetector = rightController.GetComponent<PinchDetector>();
                }
                return rightPinchDetector;
            }
        }
        private Rigidbody LeftHandRigidBody
        {
            get
            {
                if(leftHandRigidBody == null)
                {
                    GameObject leftController = GetControllerLeftHand(true);
                    leftHandRigidBody = leftController.GetComponent<Rigidbody>();
                    if(leftHandRigidBody == null)
                    {
                        leftHandRigidBody = leftController.AddComponent<Rigidbody>();
                        leftHandRigidBody.useGravity = false;
                        leftHandRigidBody.isKinematic = true;
                    }
                }
                return leftHandRigidBody;
            }
        }
        private Rigidbody RightHandRigidBody
        {
            get
            {
                if (rightHandRigidBody == null)
                {
                    GameObject rightController = GetControllerRightHand(true);
                    rightHandRigidBody = rightController.GetComponent<Rigidbody>();
                    if (rightHandRigidBody == null)
                    {
                        rightHandRigidBody = rightController.AddComponent<Rigidbody>();
                        rightHandRigidBody.useGravity = false;
                        rightHandRigidBody.isKinematic = true;
                    }
                }
                return rightHandRigidBody;
            }
        }
        private VelocityTracker LeftHandVelocityTracker
        {
            get
            {
                if (leftHandVelocityTracker == null)
                {
                    GameObject leftController = GetController(ControllerHand.Left, true);
                    leftHandVelocityTracker = leftController.GetComponent<VelocityTracker>();
                    if (leftHandVelocityTracker == null)
                    {
                        leftHandVelocityTracker = leftController.AddComponent<VelocityTracker>();
                    }
                }
                return leftHandVelocityTracker;
            }
        }

        private VelocityTracker RightHandVelocityTracker
        {
            get
            {
                if (rightHandVelocityTracker == null)
                {
                    GameObject rightController = GetController(ControllerHand.Right, true);
                    rightHandVelocityTracker = rightController.GetComponent<VelocityTracker>();
                    if (rightHandVelocityTracker == null)
                    {
                        rightHandVelocityTracker = rightController.AddComponent<VelocityTracker>();
                    }
                }
                return rightHandVelocityTracker;
            }
        }

        private Vector3 currentVelocity;

        private ControllerHand LeapChiralityToControllerHand(Chirality handedness)
        {
            switch (handedness)
            {
                case Chirality.Left:
                    return ControllerHand.Left;
                case Chirality.Right:
                    return ControllerHand.Right;
                default:
                    return ControllerHand.None;
            }
        }

        private ControllerHand indexToControllerHand(uint index)
        {
            switch (index)
            {
                case leftHandControllerIndex:
                    return ControllerHand.Left;
                case rightHandControllerIndex:
                    return ControllerHand.Right;
                default:
                    return ControllerHand.None;
            }
        }

        private PinchDetector GetPinchDetectorByIndex(uint index)
        {
            switch (indexToControllerHand(index))
            {
                case ControllerHand.Left:
                    return LeftPinchDetector;
                case ControllerHand.Right:
                    return RightPinchDetector;
                default:
                    return null;
            }
        }

        private Rigidbody GetRigidBodByIndex(uint index)
        {
            switch (indexToControllerHand(index))
            {
                case ControllerHand.Left:
                    return LeftHandRigidBody;
                case ControllerHand.Right:
                    return RightHandRigidBody;
                default:
                    return null;
            }
        }

        private GameObject GetController(ControllerHand controllerHandedness, bool actual)
        {
            GameObject returnController;
            if (controllerHandedness == ControllerHand.Right)
            {
                returnController = GetSDKManagerControllerRightHand(actual);
            }
            else
            {
                returnController = GetSDKManagerControllerLeftHand(actual);
            }

            if (!returnController && actual)
            {
                IHandModel[] handModels = FindObjectsOfType<IHandModel>();
                foreach (IHandModel handModel in handModels)
                {
                    if (controllerHandedness == LeapChiralityToControllerHand(handModel.Handedness))
                    {
                        return handModel.gameObject;
                    }
                }
            }
            return returnController;
        }

        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            switch (indexToControllerHand(index))
            {
                case ControllerHand.Left:
                    return GetControllerLeftHand(actual);
                case ControllerHand.Right:
                    return GetControllerRightHand(actual);
                default:
                    return null;
            }
        }

        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            // TODO
            return "ControllerColliders/Fallback";
        }

        public override uint GetControllerIndex(GameObject controller)
        {
            bool isLeftHand = CheckActualOrScriptAliasControllerIsLeftHand(controller);
            if (isLeftHand)
            {
                return leftHandControllerIndex;
            } else
            {
                return rightHandControllerIndex;
            }
        }

        public override GameObject GetControllerLeftHand(bool actual = false)
        {
            return GetController(ControllerHand.Left, actual);
        }

        public override GameObject GetControllerModel(ControllerHand hand)
        {
            var model = GetSDKManagerControllerModelForHand(hand);
            if (!model)
            {
                model = GetController(hand, true);
            }
            return model;
        }

        public override GameObject GetControllerModel(GameObject controller)
        {
            return GetControllerModelFromController(controller);
        }

        public override Transform GetControllerOrigin(GameObject controller)
        {
            return GetActualController(controller).transform;
        }

        public override GameObject GetControllerRenderModel(GameObject controller)
        {
            // TODO
            return null;
        }

        public override GameObject GetControllerRightHand(bool actual = false)
        {
            return GetController(ControllerHand.Right, actual);
        }

        public override Vector3 GetVelocityOnIndex(uint index)
        {
            ControllerHand handedness = indexToControllerHand(index);
            VelocityTracker vT;
            switch (handedness)
            {
                case ControllerHand.Left:
                    vT = LeftHandVelocityTracker;
                    break;
                case ControllerHand.Right:
                    vT = RightHandVelocityTracker;
                    break;
                default:
                    return Vector3.zero;
            }
            // rotate velocity vector to hand rotation
            Vector3 velocity = (Quaternion.identity * Quaternion.Inverse(vT.transform.rotation)) * vT.Velocity;
            return velocity;
        }

        public override Vector3 GetAngularVelocityOnIndex(uint index)
        {
            // FIXME Rigidbody angular velocity is always 0
            Rigidbody rB = GetRigidBodByIndex(index);
            return rB.angularVelocity;
        }

        public override bool IsControllerLeftHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsLeftHand(controller);
        }

        public override bool IsControllerLeftHand(GameObject controller, bool actual)
        {
            return CheckControllerLeftHand(controller, actual);
        }

        public override bool IsControllerRightHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsRightHand(controller);
        }

        public override bool IsControllerRightHand(GameObject controller, bool actual)
        {
            return CheckControllerRightHand(controller, actual);
        }

        public override bool IsGripPressedDownOnIndex(uint index)
        {
            return GetPinchDetectorByIndex(index).DidStartPinch;
        }

        public override bool IsGripPressedOnIndex(uint index)
        {
            return GetPinchDetectorByIndex(index).IsPinching;
        }

        public override bool IsGripPressedUpOnIndex(uint index)
        {
            return GetPinchDetectorByIndex(index).DidEndPinch;
        }

        public override void ProcessUpdate(uint index, Dictionary<string, object> options)
        {
        }
        
        #region Not Applicable
        public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
        {
            return null;
        }

        public override Vector2 GetGripAxisOnIndex(uint index)
        {
            // TODO
            return Vector2.zero;
        }

        public override float GetGripHairlineDeltaOnIndex(uint index)
        {
            return 0;
        }

        public override SDK_ControllerHapticModifiers GetHapticModifiers()
        {
            return null;
        }

        public override Vector2 GetTouchpadAxisOnIndex(uint index)
        {
            return Vector2.zero;
        }

        public override Vector2 GetTriggerAxisOnIndex(uint index)
        {
            return Vector2.zero;
        }

        public override float GetTriggerHairlineDeltaOnIndex(uint index)
        {
            return 0;
        }

        public override void HapticPulseOnIndex(uint index, float strength = 0.5F)
        {
            // no haptic pulse
        }

        public override bool IsButtonOnePressedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsButtonOnePressedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsButtonOnePressedUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsButtonOneTouchedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsButtonOneTouchedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsButtonOneTouchedUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsButtonTwoPressedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsButtonTwoPressedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsButtonTwoPressedUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsButtonTwoTouchedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsButtonTwoTouchedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsButtonTwoTouchedUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsHairGripDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsHairGripUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsHairTriggerDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsHairTriggerUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsStartMenuPressedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsStartMenuPressedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsStartMenuPressedUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsStartMenuTouchedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsStartMenuTouchedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsStartMenuTouchedUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTouchpadPressedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTouchpadPressedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTouchpadPressedUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTouchpadTouchedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTouchpadTouchedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTouchpadTouchedUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsGripTouchedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsGripTouchedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsGripTouchedUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTriggerPressedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTriggerPressedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTriggerPressedUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTriggerTouchedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTriggerTouchedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTriggerTouchedUpOnIndex(uint index)
        {
            return false;
        }

        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
            // no wheel
        }

        public override Transform GenerateControllerPointerOrigin(GameObject parent)
        {
            // TODO
            throw new NotImplementedException();
        }
        #endregion Not Applicable
#endif
    }
}