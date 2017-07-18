// Touchpad Control|Locomotion|20070
namespace VRTK
{
    using UnityEngine;
    /// <summary>
    /// The ability to control an object with the touchpad based on the position of the finger on the touchpad axis.
    /// </summary>
    /// <remarks>
    /// The Touchpad Control script forms the stub to allow for pre-defined actions to execute when the touchpad axis changes.
    ///
    /// This is enabled by the Touchpad Control script emitting an event each time the X axis and Y Axis on the touchpad change and the corresponding Object Control Action registers with the appropriate axis event. This means that multiple Object Control Actions can be triggered per axis change.
    ///
    /// This script is placed on the Script Alias of the Controller that is required to be affected by changes in the touchpad.
    ///
    /// If the controlled object is the play area and `VRTK_BodyPhysics` is also available, then additional logic is processed when the user is falling such as preventing the touchpad control from affecting a falling user.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_TouchpadControl")]
    public class VRTK_TouchpadControl : VRTK_ObjectControl
    {
        [Header("Touchpad Control Settings")]

        [Tooltip("An optional button that has to be engaged to allow the touchpad control to activate.")]
        public VRTK_ControllerEvents.ButtonAlias primaryActivationButton = VRTK_ControllerEvents.ButtonAlias.TouchpadTouch;
        [Tooltip("An optional button that when engaged will activate the modifier on the touchpad control action.")]
        public VRTK_ControllerEvents.ButtonAlias actionModifierButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;
        [Tooltip("Any input on the axis will be ignored if it is within this deadzone threshold. Between `0f` and `1f`.")]
        public Vector2 axisDeadzone = new Vector2(0.2f, 0.2f);

        protected bool touchpadFirstChange;
        protected bool otherTouchpadControlEnabledState;

        protected override void OnEnable()
        {
            base.OnEnable();
            touchpadFirstChange = true;
        }

        protected override void ControlFixedUpdate()
        {
            ModifierButtonActive();
            if (OutsideDeadzone(currentAxis.x, axisDeadzone.x) || currentAxis.x == 0f)
            {
                OnXAxisChanged(SetEventArguements(directionDevice.right, currentAxis.x, axisDeadzone.x));
            }

            if (OutsideDeadzone(currentAxis.y, axisDeadzone.y) || currentAxis.y == 0f)
            {
                OnYAxisChanged(SetEventArguements(directionDevice.forward, currentAxis.y, axisDeadzone.y));
            }
        }

        protected override VRTK_ObjectControl GetOtherControl()
        {
            GameObject foundController = (VRTK_DeviceFinder.IsControllerLeftHand(gameObject) ? VRTK_DeviceFinder.GetControllerRightHand(false) : VRTK_DeviceFinder.GetControllerLeftHand(false));
            if (foundController)
            {
                return foundController.GetComponent<VRTK_TouchpadControl>();
            }
            return null;
        }

        protected override void SetListeners(bool state)
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

        protected override bool IsInAction()
        {
            return (ValidPrimaryButton() && TouchpadTouched());
        }

        protected virtual bool OutsideDeadzone(float axisValue, float deadzoneThreshold)
        {
            return (axisValue > deadzoneThreshold || axisValue < -deadzoneThreshold);
        }

        protected virtual bool ValidPrimaryButton()
        {
            return (controllerEvents && (primaryActivationButton == VRTK_ControllerEvents.ButtonAlias.Undefined || controllerEvents.IsButtonPressed(primaryActivationButton)));
        }

        protected virtual void ModifierButtonActive()
        {
            modifierActive = (controllerEvents && actionModifierButton != VRTK_ControllerEvents.ButtonAlias.Undefined && controllerEvents.IsButtonPressed(actionModifierButton));
        }

        protected virtual bool TouchpadTouched()
        {
            return (controllerEvents && controllerEvents.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.TouchpadTouch));
        }

        protected virtual void TouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if (touchpadFirstChange && otherObjectControl && disableOtherControlsOnActive && e.touchpadAxis != Vector2.zero)
            {
                otherTouchpadControlEnabledState = otherObjectControl.enabled;
                otherObjectControl.enabled = false;
            }
            currentAxis = (ValidPrimaryButton() ? e.touchpadAxis : Vector2.zero);

            if (currentAxis != Vector2.zero)
            {
                touchpadFirstChange = false;
            }
        }

        protected virtual void TouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            if (otherObjectControl && disableOtherControlsOnActive)
            {
                otherObjectControl.enabled = otherTouchpadControlEnabledState;
            }
            currentAxis = Vector2.zero;
            touchpadFirstChange = true;
        }
    }
}