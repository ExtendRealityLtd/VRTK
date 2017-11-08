﻿// Touchpad Control|Locomotion|20070
namespace VRTK
{
    using UnityEngine;
    /// <summary>
    /// Provides the ability to control a GameObject's position based on the position of the controller touchpad axis.
    /// </summary>
    /// <remarks>
    ///   > This script forms the stub of emitting the touchpad axis X and Y changes that are then digested by the corresponding Object Control Actions that are listening for the relevant event.
    ///
    /// **Required Components:**
    ///  * `VRTK_ControllerEvents` - The Controller Events script to listen for the touchpad events on.
    ///
    /// **Optional Components:**
    ///  * `VRTK_BodyPhysics` - The Body Physics script to utilise to determine if falling is occuring.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_TouchpadControl` script on either:
    ///    * The GameObject with the Controller Events script.
    ///    * Any other scene GameObject and provide a valid `VRTK_ControllerEvents` component to the `Controller` parameter of this script.
    ///  * Place a corresponding Object Control Action for the Touchpad Control script to notify of touchpad changes. Without a corresponding Object Control Action, the Touchpad Control script will do nothing.
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
        [Tooltip("A deadzone threshold on the touchpad that will ignore input if the touch position is within the specified deadzone. Between `0f` and `1f`.")]
        public Vector2 axisDeadzone = new Vector2(0.2f, 0.2f);

        protected bool touchpadFirstChange;
        protected bool otherTouchpadControlEnabledState;
        protected bool otherTouchpadControlEnabledStateSet;

        protected override void OnEnable()
        {
            base.OnEnable();
            touchpadFirstChange = true;
            otherTouchpadControlEnabledStateSet = false;
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
                return foundController.GetComponentInChildren<VRTK_TouchpadControl>();
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
            if (touchpadFirstChange && otherObjectControl != null && disableOtherControlsOnActive && e.touchpadAxis != Vector2.zero)
            {
                otherTouchpadControlEnabledState = otherObjectControl.enabled;
                otherTouchpadControlEnabledStateSet = true;
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
            if (otherTouchpadControlEnabledStateSet && otherObjectControl != null && disableOtherControlsOnActive)
            {
                otherObjectControl.enabled = otherTouchpadControlEnabledState;
            }
            currentAxis = Vector2.zero;
            touchpadFirstChange = true;
        }
    }
}