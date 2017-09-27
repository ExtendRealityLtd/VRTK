// Button Control|Locomotion|20080
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Provides the ability to control a GameObject's position based the press of a controller button linked to a specific axis direction.
    /// </summary>
    /// <remarks>
    ///   > This script forms the stub of emitting the axis X and Y changes that are then digested by the corresponding Object Control Actions that are listening for the relevant event.
    ///
    /// **Required Components:**
    ///  * `VRTK_ControllerEvents` - The Controller Events script to listen for button presses events on.
    ///
    /// **Optional Components:**
    ///  * `VRTK_BodyPhysics` - The Body Physics script to utilise to determine if falling is occuring.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_ButtonControl` script on either:
    ///    * The GameObject with the Controller Events script.
    ///    * Any other scene GameObject and provide a valid `VRTK_ControllerEvents` component to the `Controller` parameter of this script.
    ///  * Place a corresponding Object Control Action for the Button Control script to notify of axis changes. Without a corresponding Object Control Action, the Button Control script will do nothing.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_ButtonControl")]
    public class VRTK_ButtonControl : VRTK_ObjectControl
    {
        [Header("Button Control Settings")]

        [Tooltip("The button to set the y axis to +1.")]
        public VRTK_ControllerEvents.ButtonAlias forwardButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
        [Tooltip("The button to set the y axis to -1.")]
        public VRTK_ControllerEvents.ButtonAlias backwardButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("The button to set the x axis to -1.")]
        public VRTK_ControllerEvents.ButtonAlias leftButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("The button to set the x axis to +1.")]
        public VRTK_ControllerEvents.ButtonAlias rightButton = VRTK_ControllerEvents.ButtonAlias.Undefined;

        protected bool forwardPressed;
        protected bool backwardPressed;
        protected bool leftPressed;
        protected bool rightPressed;

        protected VRTK_ControllerEvents.ButtonAlias subscribedForwardButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        protected VRTK_ControllerEvents.ButtonAlias subscribedBackwardButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        protected VRTK_ControllerEvents.ButtonAlias subscribedLeftButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        protected VRTK_ControllerEvents.ButtonAlias subscribedRightButton = VRTK_ControllerEvents.ButtonAlias.Undefined;

        protected Vector2 axisDeadzone = Vector2.zero;

        protected override void Update()
        {
            base.Update();

            if (forwardButton != subscribedForwardButton || backwardButton != subscribedBackwardButton || leftButton != subscribedLeftButton || rightButton != subscribedRightButton)
            {
                SetListeners(true);
            }
        }
        protected override void ControlFixedUpdate()
        {
            float xAxis = (leftPressed ? -1f : (rightPressed ? 1f : 0f));
            float yAxis = (forwardPressed ? 1f : (backwardPressed ? -1f : 0f));
            currentAxis = new Vector2(xAxis, yAxis);
            if (currentAxis.x != 0f)
            {
                OnXAxisChanged(SetEventArguements(directionDevice.right, currentAxis.x, axisDeadzone.x));
            }

            if (currentAxis.y != 0f)
            {
                OnYAxisChanged(SetEventArguements(directionDevice.forward, currentAxis.y, axisDeadzone.y));
            }

        }

        protected override VRTK_ObjectControl GetOtherControl()
        {
            GameObject foundController = (VRTK_DeviceFinder.IsControllerLeftHand(gameObject) ? VRTK_DeviceFinder.GetControllerRightHand(false) : VRTK_DeviceFinder.GetControllerLeftHand(false));
            if (foundController)
            {
                return foundController.GetComponentInChildren<VRTK_ButtonControl>();
            }
            return null;
        }

        protected override void SetListeners(bool state)
        {
            SetDirectionListener(state, forwardButton, ref subscribedForwardButton, ForwardButtonPressed, ForwardButtonReleased);
            SetDirectionListener(state, backwardButton, ref subscribedBackwardButton, BackwardButtonPressed, BackwardButtonReleased);
            SetDirectionListener(state, leftButton, ref subscribedLeftButton, LeftButtonPressed, LeftButtonReleased);
            SetDirectionListener(state, rightButton, ref subscribedRightButton, RightButtonPressed, RightButtonReleased);
        }

        protected override bool IsInAction()
        {
            return (forwardPressed || backwardPressed || leftPressed || rightPressed);
        }

        protected virtual void SetDirectionListener(bool state, VRTK_ControllerEvents.ButtonAlias directionButton, ref VRTK_ControllerEvents.ButtonAlias subscribedDirectionButton, ControllerInteractionEventHandler pressCallback, ControllerInteractionEventHandler releaseCallback)
        {
            if (controllerEvents)
            {
                if (subscribedDirectionButton != VRTK_ControllerEvents.ButtonAlias.Undefined && (!state || directionButton == VRTK_ControllerEvents.ButtonAlias.Undefined || directionButton != subscribedDirectionButton))
                {
                    controllerEvents.UnsubscribeToButtonAliasEvent(subscribedDirectionButton, true, pressCallback);
                    controllerEvents.UnsubscribeToButtonAliasEvent(subscribedDirectionButton, false, releaseCallback);
                    subscribedDirectionButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
                }

                if (state && directionButton != VRTK_ControllerEvents.ButtonAlias.Undefined && directionButton != subscribedDirectionButton)
                {
                    controllerEvents.SubscribeToButtonAliasEvent(directionButton, true, pressCallback);
                    controllerEvents.SubscribeToButtonAliasEvent(directionButton, false, releaseCallback);
                    subscribedDirectionButton = directionButton;
                }
            }
        }

        protected virtual void ForwardButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            forwardPressed = true;
            backwardPressed = false;
        }

        protected virtual void ForwardButtonReleased(object sender, ControllerInteractionEventArgs e)
        {
            forwardPressed = false;
        }

        protected virtual void BackwardButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            backwardPressed = true;
            forwardPressed = false;
        }

        protected virtual void BackwardButtonReleased(object sender, ControllerInteractionEventArgs e)
        {
            backwardPressed = false;
        }

        protected virtual void LeftButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            leftPressed = true;
            rightPressed = false;
        }

        protected virtual void LeftButtonReleased(object sender, ControllerInteractionEventArgs e)
        {
            leftPressed = false;
        }

        protected virtual void RightButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            rightPressed = true;
            leftPressed = false;
        }

        protected virtual void RightButtonReleased(object sender, ControllerInteractionEventArgs e)
        {
            rightPressed = false;
        }
    }
}