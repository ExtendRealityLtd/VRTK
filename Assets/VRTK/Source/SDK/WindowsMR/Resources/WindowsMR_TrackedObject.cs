namespace VRTK
{
    using UnityEngine;
#if VRTK_DEFINE_SDK_WINDOWSMR && UNITY_2017_2_OR_NEWER
    using System.Collections;
    using UnityEngine.XR.WSA.Input;
    using VRTK.WindowsMixedReality.Utilities;
#endif
#if VRTK_DEFINE_WINDOWSMR_CONTROLLER_VISUALIZATION
    using VRTK.WindowsMixedReality;
#endif

    public class WindowsMR_TrackedObject : MonoBehaviour
    {
#if VRTK_DEFINE_SDK_WINDOWSMR && UNITY_2017_2_OR_NEWER
        private struct ButtonState
        {
            //
            // Summary:
            //     ///
            //     Normalized amount ([0, 1]) representing how much select is pressed.
            //     ///
            public float SelectPressedAmount { get; set; }
            //
            // Summary:
            //     ///
            //     Depending on the InteractionSourceType of the interaction source, this returning
            //     true could represent a number of equivalent things: main button on a blicker,
            //     air-tap on a hand, and the trigger on a motion controller.
            //     ///
            public bool SelectPressed { get; set; }
            //
            // Summary:
            //     ///
            //     Whether or not the menu button is pressed.
            //     ///
            public bool MenuPressed { get; set; }
            //
            // Summary:
            //     ///
            //     Whether the controller is grasped.
            //     ///
            public bool Grasped { get; set; }
            //
            // Summary:
            //     ///
            //     Whether or not the touchpad is touched.
            //     ///
            public bool TouchpadTouched { get; set; }
            //
            // Summary:
            //     ///
            //     Whether or not the touchpad is pressed, as if a button.
            //     ///
            public bool TouchpadPressed { get; set; }
            //
            // Summary:
            //     ///
            //     Normalized coordinates for the position of a touchpad interaction.
            //     ///
            public Vector2 TouchpadPosition { get; set; }
            //
            // Summary:
            //     ///
            //     Normalized coordinates for the position of a thumbstick.
            //     ///
            public Vector2 ThumbstickPosition { get; set; }
            //
            // Summary:
            //     ///
            //     Whether or not the thumbstick is pressed.
            //     ///
            public bool ThumbstickPressed { get; set; }
        }

        [SerializeField]
        [Tooltip("Defines the controllers hand.")]
        private InteractionSourceHandedness handedness;

        public Vector3 AngularVelocity { get { return angularVelocity; } }
        public InteractionSourceHandedness Handedness { get { return handedness; } }
        public uint Index { get { return index; } }

        private uint index = uint.MaxValue;
        private ButtonState currentButtonState;
        private ButtonState prevButtonState;
        private Vector3 angularVelocity;

        private float hairTriggerDelta = 0.1f; // amount trigger must be pulled or released to change state
        private float hairTriggerLimit;
        private bool hairTriggerState;
        private bool hairTriggerPrevState;
        private bool isDetected;

        public float GetPressAmount(InteractionSourcePressType button)
        {
            switch (button)
            {
                case InteractionSourcePressType.Select:
                    return currentButtonState.SelectPressedAmount;
            }
            return 0;
        }

        public bool GetPress(InteractionSourcePressType button)
        {
            switch (button)
            {
                case InteractionSourcePressType.Select:
                    return currentButtonState.SelectPressed;
                case InteractionSourcePressType.Grasp:
                    return currentButtonState.Grasped;
                case InteractionSourcePressType.Menu:
                    return currentButtonState.MenuPressed;
                case InteractionSourcePressType.Touchpad:
                    return currentButtonState.TouchpadPressed;
                case InteractionSourcePressType.Thumbstick:
                    return currentButtonState.ThumbstickPressed;
            }
            return false;
        }

        public bool GetPressDown(InteractionSourcePressType button)
        {
            switch (button)
            {
                case InteractionSourcePressType.Select:
                    return (prevButtonState.SelectPressed == false && currentButtonState.SelectPressed == true);
                case InteractionSourcePressType.Grasp:
                    return (prevButtonState.Grasped == false && currentButtonState.Grasped == true);
                case InteractionSourcePressType.Menu:
                    return (prevButtonState.MenuPressed == false && currentButtonState.MenuPressed == true);
                case InteractionSourcePressType.Touchpad:
                    return (prevButtonState.TouchpadPressed == false && currentButtonState.TouchpadPressed == true);
                case InteractionSourcePressType.Thumbstick:
                    return (prevButtonState.ThumbstickPressed == false && currentButtonState.ThumbstickPressed == true);
            }
            return false;
        }

        public bool GetPressUp(InteractionSourcePressType button)
        {
            switch (button)
            {
                case InteractionSourcePressType.Select:
                    return (prevButtonState.SelectPressed == true && currentButtonState.SelectPressed == false);
                case InteractionSourcePressType.Grasp:
                    return (prevButtonState.Grasped == true && currentButtonState.Grasped == false);
                case InteractionSourcePressType.Menu:
                    return (prevButtonState.MenuPressed == true && currentButtonState.MenuPressed == false);
                case InteractionSourcePressType.Touchpad:
                    return (prevButtonState.TouchpadPressed == true && currentButtonState.TouchpadPressed == false);
                case InteractionSourcePressType.Thumbstick:
                    return (prevButtonState.ThumbstickPressed == true && currentButtonState.ThumbstickPressed == false);
            }
            return false;
        }

        public bool GetTouch(InteractionSourcePressType button)
        {
            switch (button)
            {
                case InteractionSourcePressType.Touchpad:
                    return currentButtonState.TouchpadTouched;
            }
            return false;
        }

        public bool GetTouchDown(InteractionSourcePressType button)
        {
            switch (button)
            {
                case InteractionSourcePressType.Touchpad:
                    return (prevButtonState.TouchpadTouched == false && currentButtonState.TouchpadTouched == true);
            }
            return false;
        }

        public bool GetTouchUp(InteractionSourcePressType button)
        {
            switch (button)
            {
                case InteractionSourcePressType.Touchpad:
                    return (prevButtonState.TouchpadTouched == true && currentButtonState.TouchpadTouched == false);
            }
            return false;
        }

        public Vector2 GetAxis(InteractionSourcePressType button)
        {
            switch (button)
            {
                case InteractionSourcePressType.Select:
                    return new Vector2(currentButtonState.SelectPressedAmount, 0f);
                case InteractionSourcePressType.Touchpad:
                    return currentButtonState.TouchpadPosition;
                case InteractionSourcePressType.Thumbstick:
                    return currentButtonState.ThumbstickPosition;
            }
            return Vector2.zero;
        }

        public bool GetHairTrigger()
        {
            return hairTriggerState;
        }

        public bool GetHairTriggerDown()
        {
            return (hairTriggerState && !hairTriggerPrevState);
        }

        public bool GetHairTriggerUp()
        {
            return !hairTriggerState && hairTriggerPrevState;
        }

        public void StartHaptics(float intensity = 0.5f, float duration = 0.4f)
        {
            InteractionSourceState[] states = InteractionManager.GetCurrentReading();

            foreach (InteractionSourceState state in states)
            {
                if (state.source.kind == InteractionSourceKind.Controller && state.source.handedness == handedness)
                {
                    state.source.StartHaptics(intensity, duration);
                }
            }
        }

        protected virtual void OnEnable()
        {
            switch (handedness)
            {
                case InteractionSourceHandedness.Left:
                    index = 1;
                    break;

                case InteractionSourceHandedness.Right:
                    index = 2;
                    break;

                case InteractionSourceHandedness.Unknown:
                    Debug.LogError("Handedness of " + gameObject.name + " is not set.");
                    break;
            }

            InitController();
        }

        protected virtual void OnDisable()
        {
            InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;
            InteractionManager.InteractionSourceUpdated -= InteractionManager_InteractionSourceUpdated;
            InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;
        }

        protected virtual void Update()
        {
            if (isDetected)
            {
                InteractionSourceState[] states = InteractionManager.GetCurrentReading();

                foreach (InteractionSourceState state in states)
                {
                    if (state.source.kind == InteractionSourceKind.Controller && state.source.handedness == handedness)
                    {
                        // Necessary to update Select Button State in Update Loop since it causes issues with PressDown and PressUp
                        // Will be changed in a future iteration (probably VRTK 4)
                        UpdateSelectButton(state);
                        UpdateTouchpadTouch(state);
                    }
                }
            }
        }

        protected virtual void InitController()
        {
            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
            InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
            InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;

#if VRTK_DEFINE_WINDOWSMR_CONTROLLER_VISUALIZATION
            if (MotionControllerVisualizer.Instance != null)
            {
                MotionControllerVisualizer.Instance.OnControllerModelLoaded += AttachControllerModel;
            }
#endif
        }

        protected virtual void SetupController(InteractionSource source)
        {
            index = source.id;
            currentButtonState = new ButtonState();
            prevButtonState = new ButtonState();
            isDetected = true;
        }

#if VRTK_DEFINE_WINDOWSMR_CONTROLLER_VISUALIZATION
        protected virtual void AttachControllerModel(MotionControllerInfo controllerInfo)
        {
            if (controllerInfo.Handedness == Handedness)
            {
                Transform controllerTransform = controllerInfo.ControllerParent.transform;
                controllerTransform.SetParent(transform);
                controllerTransform.localPosition = Vector3.zero;
                controllerTransform.localRotation = Quaternion.identity;
                controllerTransform.localScale = Vector3.one;
            }
        }
#endif
        protected virtual void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs args)
        {
            InteractionSourceState state = args.state;
            InteractionSource source = state.source;

            if (source.kind == InteractionSourceKind.Controller && source.handedness == handedness)
            {
                SetupController(source);
            }
        }

        protected virtual void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs args)
        {
            InteractionSourceState state = args.state;
            InteractionSource source = state.source;

            if (source.kind == InteractionSourceKind.Controller && source.handedness == handedness)
            {
                index = uint.MaxValue;
                currentButtonState = new ButtonState();
                isDetected = false;
            }
        }

        protected virtual void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs args)
        {
            InteractionSourceState state = args.state;
            InteractionSource source = state.source;

            if (source.kind == InteractionSourceKind.Controller && source.handedness == handedness)
            {
                if (!isDetected)
                {
                    SetupController(source);
                }

                UpdateAxis(state);
                UpdatePose(state);
            }
        }

        protected virtual void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs args)
        {
            InteractionSourceState state = args.state;
            InteractionSource source = state.source;

            if (source.kind == InteractionSourceKind.Controller && source.handedness == handedness)
            {
                UpdateButtonState(args.pressType, state);
            }
        }

        protected virtual void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs args)
        {
            InteractionSourceState state = args.state;
            InteractionSource source = state.source;

            if (source.kind == InteractionSourceKind.Controller && source.handedness == handedness)
            {
                UpdateButtonState(args.pressType, state);
            }
        }

        protected virtual void UpdatePose(InteractionSourceState state)
        {
            UpdateAngularVelocity(state.sourcePose);
            UpdateControllerPose(state.sourcePose);
        }

        // Workaround for Select Button
        // Issue: Pressed and Released event only recognize Select once and Select is pressed when selectPressedAmount==1,
        // so on press Select State is not always true and therefore not 'pressed'.
        // Updating SelectPressed in UpdateEvent of WSA.XR causes issues because the event and Unity Update are not synched
        // and therefore VRTK's polling of GetPressDown and GetPressUp might already been overwritten.
        protected virtual void UpdateSelectButton(InteractionSourceState state)
        {
            prevButtonState.SelectPressed = currentButtonState.SelectPressed;
            currentButtonState.SelectPressed = state.selectPressed;
        }

        protected virtual void UpdateTouchpadTouch(InteractionSourceState state)
        {
            if (state.source.supportsTouchpad)
            {
                prevButtonState.TouchpadTouched = currentButtonState.TouchpadTouched;
                currentButtonState.TouchpadTouched = state.touchpadTouched;
            }
        }

        protected virtual void UpdateButtonState(InteractionSourcePressType button, InteractionSourceState state)
        {
            switch (button)
            {
                case InteractionSourcePressType.Grasp:
                    prevButtonState.Grasped = currentButtonState.Grasped;
                    currentButtonState.Grasped = state.grasped;
                    break;
                case InteractionSourcePressType.Menu:
                    prevButtonState.MenuPressed = currentButtonState.MenuPressed;
                    currentButtonState.MenuPressed = state.menuPressed;
                    break;
                case InteractionSourcePressType.Touchpad:
                    prevButtonState.TouchpadPressed = currentButtonState.TouchpadPressed;
                    currentButtonState.TouchpadPressed = state.touchpadPressed;
                    break;
                case InteractionSourcePressType.Thumbstick:
                    prevButtonState.ThumbstickPressed = currentButtonState.ThumbstickPressed;
                    currentButtonState.ThumbstickPressed = state.thumbstickPressed;
                    break;
            }

            StartCoroutine(UpdateButtonStateAfterNextFrame(button));
        }

        protected virtual IEnumerator UpdateButtonStateAfterNextFrame(InteractionSourcePressType button)
        {
            yield return new WaitForEndOfFrame();

            switch (button)
            {
                case InteractionSourcePressType.Grasp:
                    prevButtonState.Grasped = currentButtonState.Grasped;
                    break;
                case InteractionSourcePressType.Menu:
                    prevButtonState.MenuPressed = currentButtonState.MenuPressed;
                    break;
                case InteractionSourcePressType.Touchpad:
                    prevButtonState.TouchpadPressed = currentButtonState.TouchpadPressed;
                    break;
                case InteractionSourcePressType.Thumbstick:
                    prevButtonState.ThumbstickPressed = currentButtonState.ThumbstickPressed;
                    break;
            }
        }

        protected virtual void UpdateControllerPose(InteractionSourcePose pose)
        {
            Quaternion newRotation;
            if (pose.TryGetRotation(out newRotation, InteractionSourceNode.Grip))
            {
                transform.localRotation = newRotation;
            }

            Vector3 newPosition;
            if (pose.TryGetPosition(out newPosition, InteractionSourceNode.Grip))
            {
                transform.localPosition = newPosition;
            }
        }

        protected virtual void UpdateAxis(InteractionSourceState state)
        {
            InteractionSource source = state.source;

            prevButtonState.SelectPressedAmount = currentButtonState.SelectPressedAmount;
            currentButtonState.SelectPressedAmount = state.selectPressedAmount;

            UpdateHairTrigger();

            if (source.supportsTouchpad)
            {
                currentButtonState.TouchpadPosition = state.touchpadPosition;
            }

            if (source.supportsThumbstick)
            {
                currentButtonState.ThumbstickPosition = state.thumbstickPosition;
            }
        }

        protected virtual void UpdateAngularVelocity(InteractionSourcePose pose)
        {
            Vector3 newAngularVelocity;
            if (pose.TryGetAngularVelocity(out newAngularVelocity))
            {
                angularVelocity = newAngularVelocity;
            }
        }

        protected virtual void UpdateHairTrigger()
        {
            hairTriggerPrevState = hairTriggerState;
            float value = currentButtonState.SelectPressedAmount;

            if (hairTriggerState)
            {
                if (value < hairTriggerLimit - hairTriggerDelta || value <= 0.0f)
                    hairTriggerState = false;
            }
            else
            {
                if (value > hairTriggerLimit + hairTriggerDelta || value >= 1.0f)
                    hairTriggerState = true;
            }

            hairTriggerLimit = hairTriggerState ? Mathf.Max(hairTriggerLimit, value) : Mathf.Min(hairTriggerLimit, value);
        }
#endif
    }
}