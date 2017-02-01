// Touchpad Control|Locomotion|20058
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The ability to control an object with the touchpad based on the position of the finger on the touchpad axis.
    /// </summary>
    /// <remarks>
    /// The Touchpad Control script forms the stub to allow for pre-defined actions to execute when the touchpad axis changes.
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
        [Tooltip("The action to perform when the X axis changes.")]
        public VRTK_BaseTouchpadControlAction xAxisActionScript;
        [Tooltip("The action to perform when the Y axis changes.")]
        public VRTK_BaseTouchpadControlAction yAxisActionScript;
        [Tooltip("If this is checked then whenever the touchpad axis on the attached controller is being changed, all other touchpad control scripts on other controllers will be disabled.")]
        public bool disableOtherControlsOnActive = true;
        [Tooltip("If a `VRTK_BodyPhysics` script is present and this is checked, then the touchpad control will affect the play area whilst it is falling.")]
        public bool affectOnFalling = false;
        [Tooltip("An optional game object to apply the touchpad control to. If this is blank then the PlayArea will be controlled.")]
        public GameObject controlOverrideObject;

        private VRTK_ControllerEvents controllerEvents;
        private VRTK_BodyPhysics bodyPhysics;
        private VRTK_TouchpadControl otherTouchpadControl;
        private GameObject controlledGameObject;
        private GameObject setControlOverrideObject;
        private Transform directionDevice;
        private DirectionDevices previousDeviceForDirection;
        private Vector2 touchpadAxis;
        private bool currentlyFalling = false;
        private bool modifierActive = false;
        private bool touchpadFirstChange;
        private bool otherTouchpadControlEnabledState;

        protected virtual void OnEnable()
        {
            touchpadAxis = Vector2.zero;
            controllerEvents = GetComponent<VRTK_ControllerEvents>();
            SetControlledObject();
            bodyPhysics = (!controlOverrideObject ? FindObjectOfType<VRTK_BodyPhysics>() : null);
            touchpadFirstChange = true;

            CheckSetupControlAction();
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
            if (xAxisActionScript && xAxisActionScript.enabled)
            {
                xAxisActionScript.ProcessFixedUpdate(controlledGameObject, directionDevice, directionDevice.right, touchpadAxis.x, axisDeadzone.x, currentlyFalling, modifierActive);
            }

            if (yAxisActionScript && yAxisActionScript.enabled)
            {
                yAxisActionScript.ProcessFixedUpdate(controlledGameObject, directionDevice, directionDevice.forward, touchpadAxis.y, axisDeadzone.y, currentlyFalling, modifierActive);
            }
        }

        protected virtual void CheckSetupControlAction()
        {
            if (xAxisActionScript && yAxisActionScript && xAxisActionScript.GetInstanceID() == yAxisActionScript.GetInstanceID())
            {
                Debug.LogError("The `X Axis Action Script` and `Y Axis Action Script` cannot be the same script instance. Create seperate scripts for each axis action.");
                return;
            }
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
        }

        protected virtual void CheckFalling()
        {
            if (bodyPhysics && bodyPhysics.IsFalling())
            {
                if (!affectOnFalling)
                {
                    touchpadAxis = Vector2.zero;
                }
                currentlyFalling = true;
            }

            if (bodyPhysics && !bodyPhysics.IsFalling() && currentlyFalling)
            {
                touchpadAxis = Vector2.zero;
                currentlyFalling = false;
            }
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