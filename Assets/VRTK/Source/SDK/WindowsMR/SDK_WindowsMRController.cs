// WindowsMR Controller|SDK_WindowsMR|004
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;
#if VRTK_DEFINE_SDK_WINDOWSMR && UNITY_2017_2_OR_NEWER
    using UnityEngine.XR.WSA.Input;
#endif
#if VRTK_DEFINE_WINDOWSMR_CONTROLLER_VISUALIZATION
    using VRTK.WindowsMixedReality;
#endif

    /// <summary>
    /// The WindowsMR Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    [SDK_Description(typeof(SDK_WindowsMR))]
    public class SDK_WindowsMRController
#if VRTK_DEFINE_SDK_WINDOWSMR && UNITY_2017_2_OR_NEWER
        : SDK_BaseController
#else
        : SDK_FallbackController
#endif
    {
#if VRTK_DEFINE_SDK_WINDOWSMR && UNITY_2017_2_OR_NEWER
        protected WindowsMR_TrackedObject cachedLeftTrackedObject;
        protected WindowsMR_TrackedObject cachedRightTrackedObject;
        protected VRTK_VelocityEstimator cachedLeftVelocityEstimator;
        protected VRTK_VelocityEstimator cachedRightVelocityEstimator;

        protected Dictionary<GameObject, WindowsMR_TrackedObject> cachedTrackedObjectsByGameObject = new Dictionary<GameObject, WindowsMR_TrackedObject>();
        protected Dictionary<uint, WindowsMR_TrackedObject> cachedTrackedObjectsByIndex = new Dictionary<uint, WindowsMR_TrackedObject>();

        #region Overriden base functions
        /// <summary>
        /// The GenerateControllerPointerOrigin method can create a custom pointer origin Transform to represent the pointer position and forward.
        /// </summary>
        /// <param name="parent">The GameObject that the origin will become parent of. If it is a controller then it will also be used to determine the hand if required.</param>
        /// <returns>A generated Transform that contains the custom pointer origin.</returns>
        [System.Obsolete("GenerateControllerPointerOrigin has been deprecated and will be removed in a future version of VRTK.")]
        public override Transform GenerateControllerPointerOrigin(GameObject parent)
        {
            //TODO: Implement
            return null;
        }

        /// <summary>
        /// The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
        {
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                if (controllerReference.hand == ControllerHand.Left && cachedLeftVelocityEstimator != null)
                {
                    return cachedLeftVelocityEstimator.GetAngularVelocityEstimate();
                }
                else if (controllerReference.hand == ControllerHand.Right && cachedRightVelocityEstimator != null)
                {
                    return cachedRightVelocityEstimator.GetAngularVelocityEstimate();
                }
            }
            return Vector3.zero;
        }

        /// <summary>
        /// The GetButtonAxis method retrieves the current X/Y axis values for the given button type on the given controller reference.
        /// </summary>
        /// <param name="buttonType">The type of button to check for the axis on.</param>
        /// <param name="controllerReference">The reference to the controller to check the button axis on.</param>
        /// <returns>A Vector2 of the X/Y values of the button axis. If no axis values exist for the given button, then a Vector2.Zero is returned.</returns>
        public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);

            if (GetControllerByIndex(index, true) == null)
            {
                return Vector2.zero;
            }

            WindowsMR_TrackedObject device = GetControllerByIndex(index, true).GetComponent<WindowsMR_TrackedObject>();

            switch (buttonType)
            {
                case ButtonTypes.Trigger:
                    return device.GetAxis(InteractionSourcePressType.Select);
                case ButtonTypes.Touchpad:
                    return device.GetAxis(InteractionSourcePressType.Touchpad);
                case ButtonTypes.TouchpadTwo:
                    return device.GetAxis(InteractionSourcePressType.Thumbstick);
            }

            return Vector2.zero;
        }

        /// <summary>
        /// The GetButtonSenseAxis method retrieves the current sense axis value for the given button type on the given controller reference.
        /// </summary>
        /// <param name="buttonType">The type of button to check for the sense axis on.</param>
        /// <param name="controllerReference">The reference to the controller to check the sense axis on.</param>
        /// <returns>The current sense axis value.</returns>
        public override float GetButtonSenseAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            // TODO: Implement
            return 0f;
        }

        /// <summary>
        /// The GetButtonHairlineDelta method is used to get the difference between the current button press and the previous frame button press.
        /// </summary>
        /// <param name="buttonType">The type of button to get the hairline delta for.</param>
        /// <param name="controllerReference">The reference to the controller to get the hairline delta for.</param>
        /// <returns>The delta between the button presses.</returns>
        public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            //TODO: Implement
            return 0;
        }

        /// <summary>
        /// The GetControllerButtonState method is used to determine if the given controller button for the given press type on the given controller reference is currently taking place.
        /// </summary>
        /// <param name="buttonType">The type of button to check for the state of.</param>
        /// <param name="pressType">The button state to check for.</param>
        /// <param name="controllerReference">The reference to the controller to check the button state on.</param>
        /// <returns>Returns true if the given button is in the state of the given press type on the given controller reference.</returns>
        public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return false;
            }

            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);

            switch (buttonType)
            {
                case ButtonTypes.Trigger:
                    return IsButtonPressed(index, pressType, InteractionSourcePressType.Select);
                case ButtonTypes.TriggerHairline:
                    WindowsMR_TrackedObject device = GetControllerByIndex(index, true).GetComponent<WindowsMR_TrackedObject>();

                    if (pressType == ButtonPressTypes.PressDown)
                    {
                        return device.GetHairTriggerDown();
                    }
                    else if (pressType == ButtonPressTypes.PressUp)
                    {
                        return device.GetHairTriggerUp();
                    }
                    break;
                case ButtonTypes.Grip:
                    return IsButtonPressed(index, pressType, InteractionSourcePressType.Grasp);
                case ButtonTypes.Touchpad:
                    return IsButtonPressed(index, pressType, InteractionSourcePressType.Touchpad);
                case ButtonTypes.TouchpadTwo:
                    return IsButtonPressed(index, pressType, InteractionSourcePressType.Thumbstick);
                case ButtonTypes.ButtonTwo:
                case ButtonTypes.StartMenu:
                    return IsButtonPressed(index, pressType, InteractionSourcePressType.Menu);
            }
            return false;
        }

        /// <summary>
        /// The GetControllerByIndex method returns the GameObject of a controller with a specific index.
        /// </summary>
        /// <param name="index">The index of the controller to find.</param>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject of the controller</returns>
        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            SetTrackedControllerCaches();
            if (index < uint.MaxValue)
            {
                VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
                if (sdkManager != null)
                {
                    if (cachedLeftTrackedObject != null && (uint)cachedLeftTrackedObject.Index == index)
                    {
                        return (actual ? sdkManager.loadedSetup.actualLeftController : sdkManager.scriptAliasLeftController);
                    }

                    if (cachedRightTrackedObject != null && (uint)cachedRightTrackedObject.Index == index)
                    {
                        return (actual ? sdkManager.loadedSetup.actualRightController : sdkManager.scriptAliasRightController);
                    }
                }

                if (cachedTrackedObjectsByIndex.ContainsKey(index) && cachedTrackedObjectsByIndex[index] != null)
                {
                    return cachedTrackedObjectsByIndex[index].gameObject;
                }
            }

            return null;
        }

        /// <summary>
        /// The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.
        /// </summary>
        /// <param name="hand">The controller hand to check for</param>
        /// <returns>A path to the resource that contains the collider GameObject.</returns>
        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            //TODO: Implement
            return "ControllerColliders/Fallback";
        }

        /// <summary>
        /// The GetControllerElementPath returns the path to the game object that the given controller element for the given hand resides in.
        /// </summary>
        /// <param name="element">The controller element to look up.</param>
        /// <param name="hand">The controller hand to look up.</param>
        /// <param name="fullPath">Whether to get the initial path or the full path to the element.</param>
        /// <returns>A string containing the path to the game object that the controller element resides in.</returns>
        public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
        {
#if VRTK_DEFINE_WINDOWSMR_CONTROLLER_VISUALIZATION
            InteractionSourceHandedness handedness;
            switch (hand)
            {
                case ControllerHand.Left:
                    handedness = InteractionSourceHandedness.Left;
                    break;
                case ControllerHand.Right:
                    handedness = InteractionSourceHandedness.Right;
                    break;
                default:
                    handedness = InteractionSourceHandedness.Unknown;
                    break;
            }

            return MotionControllerVisualizer.Instance.GetPathToButton(element, handedness);
#else
            return null;
#endif
        }

        /// <summary>
        /// The GetControllerIndex method returns the index of the given controller.
        /// </summary>
        /// <param name="controller">The GameObject containing the controller.</param>
        /// <returns>The index of the given controller.</returns>
        public override uint GetControllerIndex(GameObject controller)
        {
            WindowsMR_TrackedObject trackedObject = GetTrackedObject(controller);
            return (trackedObject != null ? (uint)trackedObject.Index : uint.MaxValue);
        }

        /// <summary>
        /// The GetControllerLeftHand method returns the GameObject containing the representation of the left hand controller.
        /// </summary>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject containing the left hand controller.</returns>
        public override GameObject GetControllerLeftHand(bool actual = false)
        {
            GameObject controller = GetSDKManagerControllerLeftHand(actual);
            if (controller == null && actual)
            {
                controller = VRTK_SharedMethods.FindEvenInactiveGameObject<WindowsMR_ControllerManager>("Controller (left)");
            }
            return controller;
        }

        /// <summary>
        /// The GetControllerModel method returns the model alias for the given GameObject.
        /// </summary>
        /// <param name="controller">The GameObject to get the model alias for.</param>
        /// <returns>The GameObject that has the model alias within it.</returns>
        public override GameObject GetControllerModel(GameObject controller)
        {
            return GetControllerModelFromController(controller);
        }

        /// <summary>
        /// The GetControllerModel method returns the model alias for the given controller hand.
        /// </summary>
        /// <param name="hand">The hand enum of which controller model to retrieve.</param>
        /// <returns>The GameObject that has the model alias within it.</returns>
        public override GameObject GetControllerModel(ControllerHand hand)
        {
            GameObject model = GetSDKManagerControllerModelForHand(hand);
            if (model == null)
            {
                GameObject controller = null;
                switch (hand)
                {
                    case ControllerHand.Left:
                        controller = GetControllerLeftHand(true);
                        if (controller != null)
                        {
                            Transform modelTransform = controller.transform.Find("LeftControllerModel");
                            if (modelTransform != null)
                            {
                                model = modelTransform.gameObject;
                            }
                        }
                        break;
                    case ControllerHand.Right:
                        controller = GetControllerRightHand(true);
                        if (controller != null)
                        {
                            Transform modelTransform = controller.transform.Find("RightControllerModel");
                            if (modelTransform != null)
                            {
                                model = modelTransform.gameObject;
                            }
                        }
                        break;
                }
            }
            return model;
        }

        /// <summary>
        /// The GetControllerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)
        {
            return VRTK_SDK_Bridge.GetPlayArea();
        }

        /// <summary>
        /// The GetControllerRenderModel method gets the game object that contains the given controller's render model.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to check.</param>
        /// <returns>A GameObject containing the object that has a render model for the controller.</returns>
        public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)
        {
            return null;
        }

        /// <summary>
        /// The GetControllerRightHand method returns the GameObject containing the representation of the right hand controller.
        /// </summary>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject containing the right hand controller.</returns>
        public override GameObject GetControllerRightHand(bool actual = false)
        {
            GameObject controller = GetSDKManagerControllerRightHand(actual);
            if (controller == null && actual)
            {
                controller = VRTK_SharedMethods.FindEvenInactiveGameObject<WindowsMR_ControllerManager>("Controller (right)");
            }
            return controller;
        }

        /// <summary>
        /// The GetCurrentControllerType method returns the current used ControllerType based on the SDK and headset being used.
        /// </summary>
        /// <returns>The ControllerType based on the SDK and headset being used.</returns>
        public override ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null)
        {
            return ControllerType.WindowsMR_MotionController;
        }

        /// <summary>
        /// The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.
        /// </summary>
        /// <returns>An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.</returns>
        public override SDK_ControllerHapticModifiers GetHapticModifiers()
        {
            SDK_ControllerHapticModifiers modifiers = new SDK_ControllerHapticModifiers();
            modifiers.durationModifier = 0.4f;
            return modifiers;
        }

        /// <summary>
        /// The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)
        {
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                if (controllerReference.hand == ControllerHand.Left && cachedLeftVelocityEstimator != null)
                {
                    return cachedLeftVelocityEstimator.GetVelocityEstimate();
                }
                else if (controllerReference.hand == ControllerHand.Right && cachedRightVelocityEstimator != null)
                {
                    return cachedRightVelocityEstimator.GetVelocityEstimate();
                }
            }
            return Vector3.zero;
        }

        /// <summary>
        /// The HapticPulse/2 method is used to initiate a simple haptic pulse on the tracked object of the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to initiate the haptic pulse on.</param>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5F)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            WindowsMR_TrackedObject device = GetControllerByIndex(index).transform.parent.GetComponent<WindowsMR_TrackedObject>();
            if (device != null)
            {
                device.StartHaptics(1f, 1f);
            }
        }

        /// <summary>
        /// The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to initiate the haptic pulse on.</param>
        /// <param name="clip">The audio clip to use for the haptic pattern.</param>
        public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
        {
            return false;
        }

        /// <summary>
        /// The IsControllerLeftHand/1 method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public override bool IsControllerLeftHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsLeftHand(controller);
        }

        /// <summary>
        /// The IsControllerLeftHand/2 method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public override bool IsControllerLeftHand(GameObject controller, bool actual)
        {
            return CheckControllerLeftHand(controller, actual);
        }

        /// <summary>
        /// The IsControllerRightHand/1 method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public override bool IsControllerRightHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsRightHand(controller);
        }

        /// <summary>
        /// The IsControllerRightHand/2 method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public override bool IsControllerRightHand(GameObject controller, bool actual)
        {
            return CheckControllerRightHand(controller, actual);
        }

        /// <summary>
        /// The IsTouchpadStatic method is used to determine if the touchpad is currently not being moved.
        /// </summary>
        /// <param name="currentAxisValues"></param>
        /// <param name="previousAxisValues"></param>
        /// <param name="compareFidelity"></param>
        /// <returns>Returns true if the touchpad is not currently being touched or moved.</returns>
        public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)
        {
            return (!isTouched || VRTK_SharedMethods.Vector2ShallowCompare(currentAxisValues, previousAxisValues, compareFidelity));
        }

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
        }

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
        }

        /// <summary>
        /// The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.
        /// </summary>
        /// <param name="renderModel">The GameObject containing the controller render model.</param>
        /// <param name="state">If true and the render model has a scroll wheen then it will be displayed, if false then the scroll wheel will be hidden.</param>
        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
        }

        /// <summary>
        /// The WaitForControllerModel method determines whether the controller model for the given hand requires waiting to load in on scene start.
        /// </summary>
        /// <param name="hand">The hand to determine if the controller model will be ready for.</param>
        /// <returns>Returns true if the controller model requires loading in at runtime and therefore needs waiting for. Returns false if the controller model will be available at start.</returns>
        public override bool WaitForControllerModel(ControllerHand hand)
        {
            return true;
        }

        #endregion

        protected virtual WindowsMR_TrackedObject GetTrackedObject(GameObject controller)
        {
            SetTrackedControllerCaches();

            if (IsControllerLeftHand(controller))
            {
                return cachedLeftTrackedObject;
            }
            else if (IsControllerRightHand(controller))
            {
                return cachedRightTrackedObject;
            }

            if (controller == null)
            {
                return null;
            }

            if (cachedTrackedObjectsByGameObject.ContainsKey(controller) && cachedTrackedObjectsByGameObject[controller] != null)
            {
                return cachedTrackedObjectsByGameObject[controller];
            }
            else
            {
                WindowsMR_TrackedObject trackedObject = controller.GetComponent<WindowsMR_TrackedObject>();
                if (trackedObject != null)
                {
                    cachedTrackedObjectsByGameObject.Add(controller, trackedObject);
                    cachedTrackedObjectsByIndex.Add((uint)trackedObject.Index, trackedObject);
                }
                return trackedObject;
            }
        }

        protected virtual void SetTrackedControllerCaches(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                cachedLeftTrackedObject = null;
                cachedRightTrackedObject = null;
                cachedTrackedObjectsByGameObject.Clear();
                cachedTrackedObjectsByIndex.Clear();
            }

            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftTrackedObject == null && sdkManager.loadedSetup.actualLeftController)
                {
                    cachedLeftTrackedObject = sdkManager.loadedSetup.actualLeftController.GetComponent<WindowsMR_TrackedObject>();
                    cachedLeftVelocityEstimator = cachedLeftTrackedObject.GetComponent<VRTK_VelocityEstimator>();
                }
                if (cachedRightTrackedObject == null && sdkManager.loadedSetup.actualRightController)
                {
                    cachedRightTrackedObject = sdkManager.loadedSetup.actualRightController.GetComponent<WindowsMR_TrackedObject>();
                    cachedRightVelocityEstimator = cachedRightTrackedObject.GetComponent<VRTK_VelocityEstimator>();
                }
            }
        }

        protected virtual bool IsButtonPressed(uint index, ButtonPressTypes type, InteractionSourcePressType button)
        {
            bool actual = true;
            WindowsMR_TrackedObject device = GetControllerByIndex(index, actual).GetComponent<WindowsMR_TrackedObject>();

            switch (type)
            {
                case ButtonPressTypes.Press:
                    return device.GetPress(button);
                case ButtonPressTypes.PressDown:
                    return device.GetPressDown(button);
                case ButtonPressTypes.PressUp:
                    return device.GetPressUp(button);
                case ButtonPressTypes.Touch:
                    return device.GetTouch(button);
                case ButtonPressTypes.TouchDown:
                    return device.GetTouchDown(button);
                case ButtonPressTypes.TouchUp:
                    return device.GetTouchUp(button);
            }

            return false;
        }

        public override void OnAfterSetupLoad(VRTK_SDKSetup setup)
        {
#if VRTK_DEFINE_WINDOWSMR_CONTROLLER_VISUALIZATION
            SubscribeToControllerModelLoaded();
#endif
        }

#if VRTK_DEFINE_WINDOWSMR_CONTROLLER_VISUALIZATION
        private void SubscribeToControllerModelLoaded()
        {
            if (MotionControllerVisualizer.Instance != null)
            {
                MotionControllerVisualizer.Instance.OnControllerModelLoaded += SetControllerModelReady;
            }
        }

        private void SetControllerModelReady(MotionControllerInfo motionControllerInfo)
        {
            VRTK_ControllerReference controllerReference = null;
            ControllerHand hand = ControllerHand.None;

            switch (motionControllerInfo.Handedness)
            {
                case InteractionSourceHandedness.Left:
                    hand = ControllerHand.Left;
                    controllerReference = VRTK_ControllerReference.GetControllerReference(GetControllerLeftHand());
                    break;
                case InteractionSourceHandedness.Right:
                    hand = ControllerHand.Right;
                    controllerReference = VRTK_ControllerReference.GetControllerReference(GetControllerRightHand());
                    break;
            }

            if (hand != ControllerHand.None && controllerReference != null)
            {
                OnControllerModelReady(hand, controllerReference);
            }
        }
#endif
#endif
    }
}
