// Touchpad Control|Locomotion|20058
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controlledGameObject">The GameObject that is going to be affected.</param>
    /// <param name="directionDevice">The device that is used for the direction.</param>
    /// <param name="axisDirection">The axis that is being affected from the touchpad.</param>
    /// <param name="axis">The value of the current touchpad touch point based across the axis direction.</param>
    /// <param name="deadzone">The value of the deadzone based across the axis direction.</param>
    /// <param name="currentlyFalling">Whether the controlled GameObject is currently falling.</param>
    /// <param name="modifierActive">Whether the modifier button is pressed.</param>
    public struct TouchpadControlEventArgs
    {
        public GameObject controlledGameObject;
        public Transform directionDevice;
        public Vector3 axisDirection;
        public float axis;
        public float deadzone;
        public bool currentlyFalling;
        public bool modifierActive;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="TouchpadControlEventArgs"/></param>
    public delegate void TouchpadControlEventHandler(object sender, TouchpadControlEventArgs e);

    /// <summary>
    /// The ability to control an object with the touchpad based on the position of the finger on the touchpad axis.
    /// </summary>
    /// <remarks>
    /// The Touchpad Control script forms the stub to allow for pre-defined actions to execute when the touchpad axis changes.
    ///
    /// This is enabled by the Touchpad Control script emitting an event each time the X axis and Y Axis on the touchpad change and the corresponding Touchpad Control Action registers with the appropriate axis event. This means that multiple Touchpad Control Actions can be triggered per axis change.
    ///
    /// This script is placed on the Script Alias of the Controller that is required to be affected by changes in the touchpad.
    ///
    /// If the controlled object is the play area and `VRTK_BodyPhysics` is also available, then additional logic is processed when the user is falling such as preventing the touchpad control from affecting a falling user.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.
    /// </example>
    [RequireComponent(typeof(VRTK_ControllerEvents))]
    public class VRTK_TouchpadControl : MonoBehaviour
    {
        /// <summary>
        /// Devices for providing direction.
        /// </summary>
        /// <param name="Headset">The headset device.</param>
        /// <param name="LeftController">The left controller device.</param>
        /// <param name="RightController">The right controller device.</param>
        /// <param name="ControlledObject">The controlled object.</param>
        public enum DirectionDevices
        {
            Headset,
            LeftController,
            RightController,
            ControlledObject
        }

        [Tooltip("An optional button that has to be engaged to allow the touchpad control to activate.")]
        public VRTK_ControllerEvents.ButtonAlias primaryActivationButton = VRTK_ControllerEvents.ButtonAlias.Touchpad_Touch;
        [Tooltip("An optional button that when engaged will activate the modifier on the touchpad control action.")]
        public VRTK_ControllerEvents.ButtonAlias actionModifierButton = VRTK_ControllerEvents.ButtonAlias.Touchpad_Press;
        [Tooltip("The direction that will be moved in is the direction of this device.")]
        public DirectionDevices deviceForDirection = DirectionDevices.Headset;
        [Tooltip("Any input on the axis will be ignored if it is within this deadzone threshold. Between `0f` and `1f`.")]
        public Vector2 axisDeadzone = new Vector2(0.2f, 0.2f);
        [Tooltip("If this is checked then whenever the touchpad axis on the attached controller is being changed, all other touchpad control scripts on other controllers will be disabled.")]
        public bool disableOtherControlsOnActive = true;
        [Tooltip("If a `VRTK_BodyPhysics` script is present and this is checked, then the touchpad control will affect the play area whilst it is falling.")]
        public bool affectOnFalling = false;
        [Tooltip("An optional game object to apply the touchpad control to. If this is blank then the PlayArea will be controlled.")]
        public GameObject controlOverrideObject;

        /// <summary>
        /// Emitted when the touchpad X Axis Changes.
        /// </summary>
        public event TouchpadControlEventHandler XAxisChanged;

        /// <summary>
        /// Emitted when the touchpad Y Axis Changes.
        /// </summary>
        public event TouchpadControlEventHandler YAxisChanged;

        private VRTK_ControllerEvents controllerEvents;
        private VRTK_BodyPhysics bodyPhysics;
        private VRTK_TouchpadControl otherTouchpadControl;
        private GameObject controlledGameObject;
        private GameObject setControlOverrideObject;
        private Transform directionDevice;
        private DirectionDevices previousDeviceForDirection;
        private Vector2 touchpadAxis;
        private Vector2 storedTouchpadAxis;
        private bool currentlyFalling = false;
        private bool modifierActive = false;
        private bool touchpadFirstChange;
        private bool otherTouchpadControlEnabledState;
        private float controlledGameObjectPreviousY = 0f;
        private float controlledGameObjectPreviousYOffset = 0.01f;

        public virtual void OnXAxisChanged(TouchpadControlEventArgs e)
        {
            if (XAxisChanged != null)
            {
                XAxisChanged(this, e);
            }
        }

        public virtual void OnYAxisChanged(TouchpadControlEventArgs e)
        {
            if (YAxisChanged != null)
            {
                YAxisChanged(this, e);
            }
        }

        protected virtual void OnEnable()
        {
            touchpadAxis = Vector2.zero;
            storedTouchpadAxis = Vector2.zero;
            controllerEvents = GetComponent<VRTK_ControllerEvents>();
            SetControlledObject();
            bodyPhysics = (!controlOverrideObject ? FindObjectOfType<VRTK_BodyPhysics>() : null);
            touchpadFirstChange = true;

            directionDevice = GetDirectionDevice();
            SetListeners(true);
            otherTouchpadControl = GetOtherControl();
        }

        protected virtual void OnDisable()
        {
            SetListeners(false);
        }

        protected virtual void Update()
        {
            if (controlOverrideObject != setControlOverrideObject)
            {
                SetControlledObject();
            }
        }

        protected virtual void FixedUpdate()
        {
            CheckDirectionDevice();
            CheckFalling();
            ModifierButtonActive();
            if (OutsideDeadzone(touchpadAxis.x, axisDeadzone.x) || touchpadAxis.x == 0f)
            {
                OnXAxisChanged(SetEventArguements(directionDevice.right, touchpadAxis.x, axisDeadzone.x));
            }

            if (OutsideDeadzone(touchpadAxis.y, axisDeadzone.y) || touchpadAxis.x == 0f)
            {
                OnYAxisChanged(SetEventArguements(directionDevice.forward, touchpadAxis.y, axisDeadzone.y));
            }
        }

        protected virtual TouchpadControlEventArgs SetEventArguements(Vector3 axisDirection, float axis, float axisDeadzone)
        {
            TouchpadControlEventArgs e;
            e.controlledGameObject = controlledGameObject;
            e.directionDevice = directionDevice;
            e.axisDirection = axisDirection;
            e.axis = axis;
            e.deadzone = axisDeadzone;
            e.currentlyFalling = currentlyFalling;
            e.modifierActive = modifierActive;

            return e;
        }

        protected virtual bool OutsideDeadzone(float axisValue, float deadzoneThreshold)
        {
            return (axisValue > deadzoneThreshold || axisValue < -deadzoneThreshold);
        }

        protected virtual VRTK_TouchpadControl GetOtherControl()
        {
            GameObject foundController = (VRTK_DeviceFinder.IsControllerLeftHand(gameObject) ? VRTK_DeviceFinder.GetControllerRightHand(false) : VRTK_DeviceFinder.GetControllerLeftHand(false));
            if (foundController)
            {
                return foundController.GetComponent<VRTK_TouchpadControl>();
            }
            return null;
        }

        protected virtual void SetControlledObject()
        {
            setControlOverrideObject = controlOverrideObject;
            controlledGameObject = (controlOverrideObject ? controlOverrideObject : VRTK_DeviceFinder.PlayAreaTransform().gameObject);
            controlledGameObjectPreviousY = controlledGameObject.transform.position.y;
        }

        protected virtual void CheckFalling()
        {
            if (bodyPhysics && bodyPhysics.IsFalling() && ObjectHeightChange())
            {
                if (!affectOnFalling)
                {
                    if (storedTouchpadAxis == Vector2.zero)
                    {
                        storedTouchpadAxis = new Vector2(touchpadAxis.x, touchpadAxis.y);
                    }
                    touchpadAxis = Vector2.zero;
                }
                currentlyFalling = true;
            }

            if (bodyPhysics && !bodyPhysics.IsFalling() && currentlyFalling)
            {
                touchpadAxis = (ValidPrimaryButton() && TouchpadTouched() ? storedTouchpadAxis : Vector2.zero);
                storedTouchpadAxis = Vector2.zero;
                currentlyFalling = false;
            }
        }

        protected virtual bool ObjectHeightChange()
        {
            bool heightChanged = ((controlledGameObjectPreviousY - controlledGameObjectPreviousYOffset) > controlledGameObject.transform.position.y);
            controlledGameObjectPreviousY = controlledGameObject.transform.position.y;
            return heightChanged;
        }

        protected virtual Transform GetDirectionDevice()
        {
            switch (deviceForDirection)
            {
                case DirectionDevices.ControlledObject:
                    return controlledGameObject.transform;
                case DirectionDevices.Headset:
                    return VRTK_DeviceFinder.HeadsetTransform();
                case DirectionDevices.LeftController:
                    return VRTK_DeviceFinder.GetControllerLeftHand(true).transform;
                case DirectionDevices.RightController:
                    return VRTK_DeviceFinder.GetControllerRightHand(true).transform;
            }

            return null;
        }

        protected virtual void CheckDirectionDevice()
        {
            if (previousDeviceForDirection != deviceForDirection)
            {
                directionDevice = GetDirectionDevice();
            }

            previousDeviceForDirection = deviceForDirection;
        }

        private void SetListeners(bool state)
        {
            if (controllerEvents)
            {
                if (state)
                {
                    controllerEvents.TouchpadAxisChanged += TouchpadAxisChanged;
                    controllerEvents.TouchpadTouchEnd += TouchpadTouchEnd;
                }
                else
                {
                    controllerEvents.TouchpadAxisChanged -= TouchpadAxisChanged;
                    controllerEvents.TouchpadTouchEnd -= TouchpadTouchEnd;
                }
            }
        }

        private bool ValidPrimaryButton()
        {
            return (controllerEvents && (primaryActivationButton == VRTK_ControllerEvents.ButtonAlias.Undefined || controllerEvents.IsButtonPressed(primaryActivationButton)));
        }

        private void ModifierButtonActive()
        {
            modifierActive = (controllerEvents && actionModifierButton != VRTK_ControllerEvents.ButtonAlias.Undefined && controllerEvents.IsButtonPressed(actionModifierButton));
        }

        private bool TouchpadTouched()
        {
            return (controllerEvents && controllerEvents.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.Touchpad_Touch));
        }

        private void TouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if (touchpadFirstChange && otherTouchpadControl && disableOtherControlsOnActive && e.touchpadAxis != Vector2.zero)
            {
                otherTouchpadControlEnabledState = otherTouchpadControl.enabled;
                otherTouchpadControl.enabled = false;
            }
            touchpadAxis = (ValidPrimaryButton() ? e.touchpadAxis : Vector2.zero);

            if (touchpadAxis != Vector2.zero)
            {
                touchpadFirstChange = false;
            }
        }

        private void TouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            if (otherTouchpadControl && disableOtherControlsOnActive)
            {
                otherTouchpadControl.enabled = otherTouchpadControlEnabledState;
            }
            touchpadAxis = Vector2.zero;
            touchpadFirstChange = true;
        }
    }
}