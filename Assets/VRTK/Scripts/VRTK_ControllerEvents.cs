// Controller Events|Scripts|0010
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controllerIndex">The index of the controller that was used.</param>
    /// <param name="buttonPressure">The amount of pressure being applied to the button pressed. `0f` to `1f`.</param>
    /// <param name="touchpadAxis">The position the touchpad is touched at. `(0,0)` to `(1,1)`.</param>
    /// <param name="touchpadAngle">The rotational position the touchpad is being touched at, 0 being top, 180 being bottom and all other angles accordingly. `0f` to `360f`.</param>
    public struct ControllerInteractionEventArgs
    {
        public uint controllerIndex;
        public float buttonPressure;
        public Vector2 touchpadAxis;
        public float touchpadAngle;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="ControllerInteractionEventArgs"/></param>
    public delegate void ControllerInteractionEventHandler(object sender, ControllerInteractionEventArgs e);

    /// <summary>
    /// The Controller Events script deals with events that the game controller is sending out.
    /// </summary>
    /// <remarks>
    /// The Controller Events script is attached to a Controller object within the `[CameraRig]` prefab and provides event listeners for every button press on the controller (excluding the System Menu button as this cannot be overridden and is always used by Steam).
    ///
    /// When a controller button is pressed, the script emits an event to denote that the button has been pressed which allows other scripts to listen for this event without needing to implement any controller logic. When a controller button is released, the script also emits an event denoting that the button has been released.
    ///
    /// The script also has a public boolean pressed state for the buttons to allow the script to be queried by other scripts to check if a button is being held down.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/002_Controller_Events` shows how the events are utilised and listened to. The accompanying example script can be viewed in `VRTK/Examples/Resources/Scripts/VRTK_ControllerEvents_ListenerExample.cs`.
    /// </example>
    public class VRTK_ControllerEvents : MonoBehaviour
    {
        /// <summary>
        /// Button types
        /// </summary>
        /// <param name="Trigger_Hairline">The trigger is squeezed past the current hairline threshold.</param>
        /// <param name="Trigger_Touch">The trigger is squeezed a small amount.</param>
        /// <param name="Trigger_Press">The trigger is squeezed about half way in.</param>
        /// <param name="Trigger_Click">The trigger is squeezed all the way until it clicks.</param>
        /// <param name="Grip">The grip button is pressed.</param>
        /// <param name="Touchpad_Touch">The touchpad is touched (without pressing down to click).</param>
        /// <param name="Touchpad_Press">The touchpad is pressed (to the point of hearing a click).</param>
        /// <param name="Application_Menu">The application menu button is pressed.</param>
        /// <param name="Undefined">No button specified</param>
        public enum ButtonAlias
        {
            Trigger_Hairline,
            Trigger_Touch,
            Trigger_Press,
            Trigger_Click,
            Grip,
            Touchpad_Touch,
            Touchpad_Press,
            Application_Menu,
            Undefined
        }

        [Tooltip("The button to use for the action of turning a laser pointer on / off.")]
        public ButtonAlias pointerToggleButton = ButtonAlias.Touchpad_Press;
        [Tooltip("The button to use for the action of setting a destination marker from the cursor position of the pointer.")]
        public ButtonAlias pointerSetButton = ButtonAlias.Touchpad_Press;
        [Tooltip("The button to use for the action of grabbing game objects.")]
        public ButtonAlias grabToggleButton = ButtonAlias.Grip;
        [Tooltip("The button to use for the action of using game objects.")]
        public ButtonAlias useToggleButton = ButtonAlias.Trigger_Click;
        [Tooltip("The button to use for the action of clicking a UI element.")]
        public ButtonAlias uiClickButton = ButtonAlias.Trigger_Click;
        [Tooltip("The button to use for the action of bringing up an in-game menu.")]
        public ButtonAlias menuToggleButton = ButtonAlias.Application_Menu;
        [Tooltip("The amount of fidelity in the changes on the axis, which is defaulted to 1. Any number higher than 2 will probably give too sensitive results.")]
        public int axisFidelity = 1;
        [Tooltip("The level on the trigger axis to reach before a click is registered.")]
        public float triggerClickThreshold = 1f;

        /// <summary>
        /// This will be true if the trigger is squeezed about half way in.
        /// </summary>
        [HideInInspector]
        public bool triggerPressed = false;
        /// <summary>
        /// This will be true if the trigger is squeezed a small amount.
        /// </summary>
        [HideInInspector]
        public bool triggerTouched = false;
        /// <summary>
        /// This will be true if the trigger is squeezed a small amount more from any previous squeeze on the trigger.
        /// </summary>
        [HideInInspector]
        public bool triggerHairlinePressed = false;
        /// <summary>
        /// This will be true if the trigger is squeezed all the way until it clicks.
        /// </summary>
        [HideInInspector]
        public bool triggerClicked = false;
        /// <summary>
        /// This will be true if the trigger has been squeezed more or less.
        /// </summary>
        [HideInInspector]
        public bool triggerAxisChanged = false;
        /// <summary>
        /// This will be true if the application menu is held down.
        /// </summary>
        [HideInInspector]
        public bool applicationMenuPressed = false;
        /// <summary>
        /// This will be true if the touchpad is held down.
        /// </summary>
        [HideInInspector]
        public bool touchpadPressed = false;
        /// <summary>
        /// This will be true if the touchpad is being touched.
        /// </summary>
        [HideInInspector]
        public bool touchpadTouched = false;
        /// <summary>
        /// This will be true if the touchpad touch position has changed.
        /// </summary>
        [HideInInspector]
        public bool touchpadAxisChanged = false;
        /// <summary>
        /// This will be true if the grip is held down.
        /// </summary>

        [HideInInspector]
        public bool gripPressed = false;
        /// <summary>
        /// This will be true if the button aliased to the pointer is held down.
        /// </summary>
        [HideInInspector]
        public bool pointerPressed = false;
        /// <summary>
        /// This will be true if the button aliased to the grab is held down.
        /// </summary>
        [HideInInspector]
        public bool grabPressed = false;
        /// <summary>
        /// This will be true if the button aliased to the use is held down.
        /// </summary>
        [HideInInspector]
        public bool usePressed = false;
        /// <summary>
        /// This will be true if the button aliased to the UI click is held down.
        /// </summary>
        [HideInInspector]
        public bool uiClickPressed = false;
        /// <summary>
        /// This will be true if the button aliased to the application menu is held down.
        /// </summary>
        [HideInInspector]
        public bool menuPressed = false;

        /// <summary>
        /// Emitted when the trigger is squeezed about half way in.
        /// </summary>
        public event ControllerInteractionEventHandler TriggerPressed;
        /// <summary>
        /// Emitted when the trigger is released under half way.
        /// </summary>
        public event ControllerInteractionEventHandler TriggerReleased;

        /// <summary>
        /// Emitted when the trigger is squeezed a small amount.
        /// </summary>
        public event ControllerInteractionEventHandler TriggerTouchStart;
        /// <summary>
        /// Emitted when the trigger is no longer being squeezed at all.
        /// </summary>
        public event ControllerInteractionEventHandler TriggerTouchEnd;

        /// <summary>
        /// Emitted when the trigger is squeezed past the current hairline threshold.
        /// </summary>
        public event ControllerInteractionEventHandler TriggerHairlineStart;
        /// <summary>
        /// Emitted when the trigger is released past the current hairline threshold.
        /// </summary>
        public event ControllerInteractionEventHandler TriggerHairlineEnd;

        /// <summary>
        /// Emitted when the trigger is squeezed all the way until it clicks.
        /// </summary>
        public event ControllerInteractionEventHandler TriggerClicked;
        /// <summary>
        /// Emitted when the trigger is no longer being held all the way down.
        /// </summary>
        public event ControllerInteractionEventHandler TriggerUnclicked;

        /// <summary>
        /// Emitted when the amount of squeeze on the trigger changes.
        /// </summary>
        public event ControllerInteractionEventHandler TriggerAxisChanged;

        /// <summary>
        /// Emitted when the application menu button is pressed.
        /// </summary>
        public event ControllerInteractionEventHandler ApplicationMenuPressed;
        /// <summary>
        /// Emitted when the application menu button is released.
        /// </summary>
        public event ControllerInteractionEventHandler ApplicationMenuReleased;

        /// <summary>
        /// Emitted when the grip button is pressed.
        /// </summary>
        public event ControllerInteractionEventHandler GripPressed;
        /// <summary>
        /// Emitted when the grip button is released.
        /// </summary>
        public event ControllerInteractionEventHandler GripReleased;

        /// <summary>
        /// Emitted when the touchpad is pressed (to the point of hearing a click).
        /// </summary>
        public event ControllerInteractionEventHandler TouchpadPressed;
        /// <summary>
        /// Emitted when the touchpad has been released after a pressed state.
        /// </summary>
        public event ControllerInteractionEventHandler TouchpadReleased;

        /// <summary>
        /// Emitted when the touchpad is touched (without pressing down to click).
        /// </summary>
        public event ControllerInteractionEventHandler TouchpadTouchStart;
        /// <summary>
        /// Emitted when the touchpad is no longer being touched.
        /// </summary>
        public event ControllerInteractionEventHandler TouchpadTouchEnd;

        /// <summary>
        /// Emitted when the touchpad is being touched in a different location.
        /// </summary>
        public event ControllerInteractionEventHandler TouchpadAxisChanged;

        /// <summary>
        /// Emitted when the pointer toggle alias button is pressed.
        /// </summary>
        public event ControllerInteractionEventHandler AliasPointerOn;
        /// <summary>
        /// Emitted when the pointer toggle alias button is released.
        /// </summary>
        public event ControllerInteractionEventHandler AliasPointerOff;
        /// <summary>
        /// Emitted when the pointer set alias button is released.
        /// </summary>
        public event ControllerInteractionEventHandler AliasPointerSet;

        /// <summary>
        /// Emitted when the grab toggle alias button is pressed.
        /// </summary>
        public event ControllerInteractionEventHandler AliasGrabOn;
        /// <summary>
        /// Emitted when the grab toggle alias button is released.
        /// </summary>
        public event ControllerInteractionEventHandler AliasGrabOff;

        /// <summary>
        /// Emitted when the use toggle alias button is pressed.
        /// </summary>
        public event ControllerInteractionEventHandler AliasUseOn;
        /// <summary>
        /// Emitted when the use toggle alias button is released.
        /// </summary>
        public event ControllerInteractionEventHandler AliasUseOff;

        /// <summary>
        /// Emitted when the menu toggle alias button is pressed.
        /// </summary>
        public event ControllerInteractionEventHandler AliasMenuOn;
        /// <summary>
        /// Emitted when the menu toggle alias button is released.
        /// </summary>
        public event ControllerInteractionEventHandler AliasMenuOff;

        /// <summary>
        /// Emitted when the UI click alias button is pressed.
        /// </summary>
        public event ControllerInteractionEventHandler AliasUIClickOn;
        /// <summary>
        /// Emitted when the UI click alias button is released.
        /// </summary>
        public event ControllerInteractionEventHandler AliasUIClickOff;

        /// <summary>
        /// Emitted when the controller is enabled.
        /// </summary>
        public event ControllerInteractionEventHandler ControllerEnabled;
        /// <summary>
        /// Emitted when the controller is disabled.
        /// </summary>
        public event ControllerInteractionEventHandler ControllerDisabled;

        private uint controllerIndex;
        private Vector2 touchpadAxis = Vector2.zero;
        private Vector2 triggerAxis = Vector2.zero;
        private float hairTriggerDelta;
        private Vector3 controllerVelocity = Vector3.zero;
        private Vector3 controllerAngularVelocity = Vector3.zero;

        public virtual void OnTriggerPressed(ControllerInteractionEventArgs e)
        {
            if (TriggerPressed != null)
            {
                TriggerPressed(this, e);
            }
        }

        public virtual void OnTriggerReleased(ControllerInteractionEventArgs e)
        {
            if (TriggerReleased != null)
            {
                TriggerReleased(this, e);
            }
        }

        public virtual void OnTriggerTouchStart(ControllerInteractionEventArgs e)
        {
            if (TriggerTouchStart != null)
            {
                TriggerTouchStart(this, e);
            }
        }

        public virtual void OnTriggerTouchEnd(ControllerInteractionEventArgs e)
        {
            if (TriggerTouchEnd != null)
            {
                TriggerTouchEnd(this, e);
            }
        }

        public virtual void OnTriggerHairlineStart(ControllerInteractionEventArgs e)
        {
            if (TriggerHairlineStart != null)
            {
                TriggerHairlineStart(this, e);
            }
        }

        public virtual void OnTriggerHairlineEnd(ControllerInteractionEventArgs e)
        {
            if (TriggerHairlineEnd != null)
            {
                TriggerHairlineEnd(this, e);
            }
        }

        public virtual void OnTriggerClicked(ControllerInteractionEventArgs e)
        {
            if (TriggerClicked != null)
            {
                TriggerClicked(this, e);
            }
        }

        public virtual void OnTriggerUnclicked(ControllerInteractionEventArgs e)
        {
            if (TriggerUnclicked != null)
            {
                TriggerUnclicked(this, e);
            }
        }

        public virtual void OnTriggerAxisChanged(ControllerInteractionEventArgs e)
        {
            if (TriggerAxisChanged != null)
            {
                TriggerAxisChanged(this, e);
            }
        }

        public virtual void OnApplicationMenuPressed(ControllerInteractionEventArgs e)
        {
            if (ApplicationMenuPressed != null)
            {
                ApplicationMenuPressed(this, e);
            }
        }

        public virtual void OnApplicationMenuReleased(ControllerInteractionEventArgs e)
        {
            if (ApplicationMenuReleased != null)
            {
                ApplicationMenuReleased(this, e);
            }
        }

        public virtual void OnGripPressed(ControllerInteractionEventArgs e)
        {
            if (GripPressed != null)
            {
                GripPressed(this, e);
            }
        }

        public virtual void OnGripReleased(ControllerInteractionEventArgs e)
        {
            if (GripReleased != null)
            {
                GripReleased(this, e);
            }
        }

        public virtual void OnTouchpadPressed(ControllerInteractionEventArgs e)
        {
            if (TouchpadPressed != null)
            {
                TouchpadPressed(this, e);
            }
        }

        public virtual void OnTouchpadReleased(ControllerInteractionEventArgs e)
        {
            if (TouchpadReleased != null)
            {
                TouchpadReleased(this, e);
            }
        }

        public virtual void OnTouchpadTouchStart(ControllerInteractionEventArgs e)
        {
            if (TouchpadTouchStart != null)
            {
                TouchpadTouchStart(this, e);
            }
        }

        public virtual void OnTouchpadTouchEnd(ControllerInteractionEventArgs e)
        {
            if (TouchpadTouchEnd != null)
            {
                TouchpadTouchEnd(this, e);
            }
        }

        public virtual void OnTouchpadAxisChanged(ControllerInteractionEventArgs e)
        {
            if (TouchpadAxisChanged != null)
            {
                TouchpadAxisChanged(this, e);
            }
        }

        public virtual void OnAliasPointerOn(ControllerInteractionEventArgs e)
        {
            if (AliasPointerOn != null)
            {
                AliasPointerOn(this, e);
            }
        }

        public virtual void OnAliasPointerOff(ControllerInteractionEventArgs e)
        {
            if (AliasPointerOff != null)
            {
                AliasPointerOff(this, e);
            }
        }

        public virtual void OnAliasPointerSet(ControllerInteractionEventArgs e)
        {
            if (AliasPointerSet != null)
            {
                AliasPointerSet(this, e);
            }
        }

        public virtual void OnAliasGrabOn(ControllerInteractionEventArgs e)
        {
            if (AliasGrabOn != null)
            {
                AliasGrabOn(this, e);
            }
        }

        public virtual void OnAliasGrabOff(ControllerInteractionEventArgs e)
        {
            if (AliasGrabOff != null)
            {
                AliasGrabOff(this, e);
            }
        }

        public virtual void OnAliasUseOn(ControllerInteractionEventArgs e)
        {
            if (AliasUseOn != null)
            {
                AliasUseOn(this, e);
            }
        }

        public virtual void OnAliasUseOff(ControllerInteractionEventArgs e)
        {
            if (AliasUseOff != null)
            {
                AliasUseOff(this, e);
            }
        }

        public virtual void OnAliasUIClickOn(ControllerInteractionEventArgs e)
        {
            if (AliasUIClickOn != null)
            {
                AliasUIClickOn(this, e);
            }
        }

        public virtual void OnAliasUIClickOff(ControllerInteractionEventArgs e)
        {
            if (AliasUIClickOff != null)
            {
                AliasUIClickOff(this, e);
            }
        }

        public virtual void OnAliasMenuOn(ControllerInteractionEventArgs e)
        {
            if (AliasMenuOn != null)
            {
                AliasMenuOn(this, e);
            }
        }

        public virtual void OnAliasMenuOff(ControllerInteractionEventArgs e)
        {
            if (AliasMenuOff != null)
            {
                AliasMenuOff(this, e);
            }
        }

        public virtual void OnControllerEnabled(ControllerInteractionEventArgs e)
        {
            if (ControllerEnabled != null)
            {
                ControllerEnabled(this, e);
            }
        }

        public virtual void OnControllerDisabled(ControllerInteractionEventArgs e)
        {
            if (ControllerDisabled != null)
            {
                ControllerDisabled(this, e);
            }
        }

        /// <summary>
        /// The GetVelocity method is useful for getting the current velocity of the physical game controller. This can be useful to determine the speed at which the controller is being swung or the direction it is being moved in.
        /// </summary>
        /// <returns>A 3 dimensional vector containing the current real world physical controller velocity.</returns>
        public Vector3 GetVelocity()
        {
            SetVelocity();
            return controllerVelocity;
        }

        /// <summary>
        /// The GetAngularVelocity method is useful for getting the current rotational velocity of the physical game controller. This can be useful for determining which way the controller is being rotated and at what speed the rotation is occurring.
        /// </summary>
        /// <returns>A 3 dimensional vector containing the current real world physical controller angular (rotational) velocity.</returns>
        public Vector3 GetAngularVelocity()
        {
            SetVelocity();
            return controllerAngularVelocity;
        }

        /// <summary>
        /// The GetTouchpadAxis method returns the coordinates of where the touchpad is being touched and can be used for directional input via the touchpad. The `x` value is the horizontal touch plane and the `y` value is the vertical touch plane.
        /// </summary>
        /// <returns>A 2 dimensional vector containing the x and y position of where the touchpad is being touched. `(0,0)` to `(1,1)`.</returns>
        public Vector2 GetTouchpadAxis()
        {
            return touchpadAxis;
        }

        /// <summary>
        /// The GetTouchpadAxisAngle method returns the angle of where the touchpad is currently being touched with the top of the touchpad being 0 degrees and the bottom of the touchpad being 180 degrees.
        /// </summary>
        /// <returns>A float representing the angle of where the touchpad is being touched. `0f` to `360f`.</returns>
        public float GetTouchpadAxisAngle()
        {
            return CalculateTouchpadAxisAngle(touchpadAxis);
        }

        /// <summary>
        /// The GetTriggerAxis method returns a float that represents how much the trigger is being squeezed. This can be useful for using the trigger axis to perform high fidelity tasks or only activating the trigger press once it has exceeded a given press threshold.
        /// </summary>
        /// <returns>A float representing the amount of squeeze that is being applied to the trigger. `0f` to `1f`.</returns>
        public float GetTriggerAxis()
        {
            return triggerAxis.x;
        }

        /// <summary>
        /// The GetHairTriggerDelta method returns a float representing the difference in how much the trigger is being pressed in relation to the hairline threshold start.
        /// </summary>
        /// <returns>A float representing the difference in the trigger pressure from the hairline threshold start to current position.</returns>
        public float GetHairTriggerDelta()
        {
            return hairTriggerDelta;
        }

        /// <summary>
        /// The AnyButtonPressed method returns true if any of the controller buttons are being pressed and this can be useful to determine if an action can be taken whilst the user is using the controller.
        /// </summary>
        /// <returns>Is true if any of the controller buttons are currently being pressed.</returns>
        public bool AnyButtonPressed()
        {
            return (triggerPressed || gripPressed || touchpadPressed || applicationMenuPressed);
        }

        /// <summary>
        /// The IsButtonPressed method takes a given button alias and returns a boolean whether that given button is currently being pressed or not.
        /// </summary>
        /// <param name="button">The button to check if it's being pressed.</param>
        /// <returns>Is true if the button is being pressed.</returns>
        public bool IsButtonPressed(ButtonAlias button)
        {
            switch (button)
            {
                case ButtonAlias.Trigger_Hairline:
                    return triggerHairlinePressed;
                case ButtonAlias.Trigger_Touch:
                    return triggerTouched;
                case ButtonAlias.Trigger_Press:
                    return triggerPressed;
                case ButtonAlias.Trigger_Click:
                    return triggerClicked;
                case ButtonAlias.Grip:
                    return grabPressed;
                case ButtonAlias.Touchpad_Touch:
                    return touchpadTouched;
                case ButtonAlias.Touchpad_Press:
                    return touchpadPressed;
                case ButtonAlias.Application_Menu:
                    return applicationMenuPressed;
            }
            return false;
        }

        private ControllerInteractionEventArgs SetButtonEvent(ref bool buttonBool, bool value, float buttonPressure)
        {
            buttonBool = value;
            ControllerInteractionEventArgs e;
            e.controllerIndex = controllerIndex;
            e.buttonPressure = buttonPressure;
            e.touchpadAxis = VRTK_SDK_Bridge.GetTouchpadAxisOnIndex(controllerIndex);
            e.touchpadAngle = CalculateTouchpadAxisAngle(e.touchpadAxis);
            return e;
        }

        private void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        private void OnEnable()
        {
            Invoke("EnableEvents", 0f);
        }

        private void OnDisable()
        {
            Invoke("DisableEvents", 0f);
        }

        private float CalculateTouchpadAxisAngle(Vector2 axis)
        {
            float angle = Mathf.Atan2(axis.y, axis.x) * Mathf.Rad2Deg;
            angle = 90.0f - angle;
            if (angle < 0)
            {
                angle += 360.0f;
            }
            return angle;
        }

        private void EmitAlias(ButtonAlias type, bool touchDown, float buttonPressure, ref bool buttonBool)
        {
            if (pointerToggleButton == type)
            {
                if (touchDown)
                {
                    pointerPressed = true;
                    OnAliasPointerOn(SetButtonEvent(ref buttonBool, true, buttonPressure));
                }
                else
                {
                    pointerPressed = false;
                    OnAliasPointerOff(SetButtonEvent(ref buttonBool, false, buttonPressure));
                }
            }

            if (pointerSetButton == type)
            {
                if (!touchDown)
                {
                    OnAliasPointerSet(SetButtonEvent(ref buttonBool, false, buttonPressure));
                }
            }

            if (grabToggleButton == type)
            {
                if (touchDown)
                {
                    grabPressed = true;
                    OnAliasGrabOn(SetButtonEvent(ref buttonBool, true, buttonPressure));
                }
                else
                {
                    grabPressed = false;
                    OnAliasGrabOff(SetButtonEvent(ref buttonBool, false, buttonPressure));
                }
            }

            if (useToggleButton == type)
            {
                if (touchDown)
                {
                    usePressed = true;
                    OnAliasUseOn(SetButtonEvent(ref buttonBool, true, buttonPressure));
                }
                else
                {
                    usePressed = false;
                    OnAliasUseOff(SetButtonEvent(ref buttonBool, false, buttonPressure));
                }
            }

            if (uiClickButton == type)
            {
                if (touchDown)
                {
                    uiClickPressed = true;
                    OnAliasUIClickOn(SetButtonEvent(ref buttonBool, true, buttonPressure));
                }
                else
                {
                    uiClickPressed = false;
                    OnAliasUIClickOff(SetButtonEvent(ref buttonBool, false, buttonPressure));
                }
            }

            if (menuToggleButton == type)
            {
                if (touchDown)
                {
                    menuPressed = true;
                    OnAliasMenuOn(SetButtonEvent(ref buttonBool, true, buttonPressure));
                }
                else
                {
                    menuPressed = false;
                    OnAliasMenuOff(SetButtonEvent(ref buttonBool, false, buttonPressure));
                }
            }
        }

        private bool Vector2ShallowEquals(Vector2 vectorA, Vector2 vectorB)
        {
            return (vectorA.x.ToString("F" + axisFidelity) == vectorB.x.ToString("F" + axisFidelity) &&
                    vectorA.y.ToString("F" + axisFidelity) == vectorB.y.ToString("F" + axisFidelity));
        }

        private void CacheControllerIndex()
        {
            uint tmpControllerIndex = 0;
            var trackedObject = VRTK_DeviceFinder.TrackedObjectOfGameObject(gameObject, out tmpControllerIndex);
            if (tmpControllerIndex > 0 && tmpControllerIndex < uint.MaxValue && tmpControllerIndex != controllerIndex)
            {
                RemoveControllerIndexFromCache();
                if (!VRTK_ObjectCache.trackedControllers.ContainsKey(tmpControllerIndex))
                {
                    VRTK_ObjectCache.trackedControllers.Add(tmpControllerIndex, trackedObject);
                }
                controllerIndex = tmpControllerIndex;
            }
        }

        private void RemoveControllerIndexFromCache()
        {
            if (VRTK_ObjectCache.trackedControllers.ContainsKey(controllerIndex))
            {
                VRTK_ObjectCache.trackedControllers.Remove(controllerIndex);
            }
        }

        private void EnableEvents()
        {
            CacheControllerIndex();
            bool nullBool = false;
            OnControllerEnabled(SetButtonEvent(ref nullBool, true, 0f));

        }

        private void DisableEvents()
        {
            RemoveControllerIndexFromCache();
            bool nullBool = false;
            OnControllerDisabled(SetButtonEvent(ref nullBool, false, 0f));

            if (triggerPressed)
            {
                OnTriggerReleased(SetButtonEvent(ref triggerPressed, false, 0f));
                EmitAlias(ButtonAlias.Trigger_Press, false, 0f, ref triggerPressed);
            }

            if (triggerTouched)
            {
                OnTriggerTouchEnd(SetButtonEvent(ref triggerTouched, false, 0f));
                EmitAlias(ButtonAlias.Trigger_Touch, false, 0f, ref triggerTouched);
            }

            if (triggerHairlinePressed)
            {
                OnTriggerHairlineEnd(SetButtonEvent(ref triggerHairlinePressed, false, 0f));
                EmitAlias(ButtonAlias.Trigger_Hairline, false, 0f, ref triggerHairlinePressed);
            }

            if (triggerClicked)
            {
                OnTriggerUnclicked(SetButtonEvent(ref triggerClicked, false, 0f));
                EmitAlias(ButtonAlias.Trigger_Click, false, 0f, ref triggerClicked);
            }

            if (applicationMenuPressed)
            {
                OnApplicationMenuReleased(SetButtonEvent(ref applicationMenuPressed, false, 0f));
                EmitAlias(ButtonAlias.Application_Menu, false, 0f, ref applicationMenuPressed);
            }

            if (gripPressed)
            {
                OnGripReleased(SetButtonEvent(ref gripPressed, false, 0f));
                EmitAlias(ButtonAlias.Grip, false, 0f, ref gripPressed);
            }

            if (touchpadPressed)
            {
                OnTouchpadReleased(SetButtonEvent(ref touchpadPressed, false, 0f));
                EmitAlias(ButtonAlias.Touchpad_Press, false, 0f, ref touchpadPressed);
            }

            if (touchpadTouched)
            {
                OnTouchpadTouchEnd(SetButtonEvent(ref touchpadTouched, false, 0f));
                EmitAlias(ButtonAlias.Touchpad_Touch, false, 0f, ref touchpadTouched);
            }

            triggerAxisChanged = false;
            touchpadAxisChanged = false;

            CacheControllerIndex();
            if (controllerIndex < uint.MaxValue)
            {
                Vector2 currentTriggerAxis = VRTK_SDK_Bridge.GetTriggerAxisOnIndex(controllerIndex);
                Vector2 currentTouchpadAxis = VRTK_SDK_Bridge.GetTouchpadAxisOnIndex(controllerIndex);

                // Save current touch and trigger settings to detect next change.
                touchpadAxis = new Vector2(currentTouchpadAxis.x, currentTouchpadAxis.y);
                triggerAxis = new Vector2(currentTriggerAxis.x, currentTriggerAxis.y);
                hairTriggerDelta = VRTK_SDK_Bridge.GetTriggerHairlineDeltaOnIndex(controllerIndex);
            }
        }

        private void Update()
        {
            CacheControllerIndex();
            //Only continue if the controller index has been set to a sensible number
            if (controllerIndex >= uint.MaxValue)
            {
                return;
            }

            Vector2 currentTriggerAxis = VRTK_SDK_Bridge.GetTriggerAxisOnIndex(controllerIndex);
            Vector2 currentTouchpadAxis = VRTK_SDK_Bridge.GetTouchpadAxisOnIndex(controllerIndex);

            //Trigger Pressed
            if (VRTK_SDK_Bridge.IsTriggerPressedDownOnIndex(controllerIndex))
            {
                OnTriggerPressed(SetButtonEvent(ref triggerPressed, true, currentTriggerAxis.x));
                EmitAlias(ButtonAlias.Trigger_Press, true, currentTriggerAxis.x, ref triggerPressed);
            }
            else if (VRTK_SDK_Bridge.IsTriggerPressedUpOnIndex(controllerIndex))
            {
                OnTriggerReleased(SetButtonEvent(ref triggerPressed, false, 0f));
                EmitAlias(ButtonAlias.Trigger_Press, false, 0f, ref triggerPressed);
            }

            //Trigger Touched
            if (VRTK_SDK_Bridge.IsTriggerTouchedDownOnIndex(controllerIndex))
            {
                OnTriggerTouchStart(SetButtonEvent(ref triggerTouched, true, currentTriggerAxis.x));
                EmitAlias(ButtonAlias.Trigger_Touch, true, currentTriggerAxis.x, ref triggerTouched);
            }
            else if (VRTK_SDK_Bridge.IsTriggerTouchedUpOnIndex(controllerIndex))
            {
                OnTriggerTouchEnd(SetButtonEvent(ref triggerTouched, false, 0f));
                EmitAlias(ButtonAlias.Trigger_Touch, false, 0f, ref triggerTouched);
            }

            //Trigger Hairline
            if (VRTK_SDK_Bridge.IsHairTriggerDownOnIndex(controllerIndex))
            {
                OnTriggerHairlineStart(SetButtonEvent(ref triggerHairlinePressed, true, currentTriggerAxis.x));
                EmitAlias(ButtonAlias.Trigger_Hairline, true, currentTriggerAxis.x, ref triggerHairlinePressed);
            }
            else if (VRTK_SDK_Bridge.IsHairTriggerUpOnIndex(controllerIndex))
            {
                OnTriggerHairlineEnd(SetButtonEvent(ref triggerHairlinePressed, false, 0f));
                EmitAlias(ButtonAlias.Trigger_Hairline, false, 0f, ref triggerHairlinePressed);
            }

            //Trigger Clicked
            if (!triggerClicked && currentTriggerAxis.x >= triggerClickThreshold)
            {
                OnTriggerClicked(SetButtonEvent(ref triggerClicked, true, currentTriggerAxis.x));
                EmitAlias(ButtonAlias.Trigger_Click, true, currentTriggerAxis.x, ref triggerClicked);
            }
            else if (triggerClicked && currentTriggerAxis.x < triggerClickThreshold)
            {
                OnTriggerUnclicked(SetButtonEvent(ref triggerClicked, false, 0f));
                EmitAlias(ButtonAlias.Trigger_Click, false, 0f, ref triggerClicked);
            }

            //Trigger Axis
            if (Vector2ShallowEquals(triggerAxis, currentTriggerAxis))
            {
                triggerAxisChanged = false;
            }
            else
            {
                OnTriggerAxisChanged(SetButtonEvent(ref triggerAxisChanged, true, currentTriggerAxis.x));
            }

            //ApplicationMenu
            if (VRTK_SDK_Bridge.IsApplicationMenuPressedDownOnIndex(controllerIndex))
            {
                OnApplicationMenuPressed(SetButtonEvent(ref applicationMenuPressed, true, 1f));
                EmitAlias(ButtonAlias.Application_Menu, true, 1f, ref applicationMenuPressed);
            }
            else if (VRTK_SDK_Bridge.IsApplicationMenuPressedUpOnIndex(controllerIndex))
            {

                OnApplicationMenuReleased(SetButtonEvent(ref applicationMenuPressed, false, 0f));
                EmitAlias(ButtonAlias.Application_Menu, false, 0f, ref applicationMenuPressed);
            }

            //Grip
            if (VRTK_SDK_Bridge.IsGripPressedDownOnIndex(controllerIndex))
            {
                OnGripPressed(SetButtonEvent(ref gripPressed, true, 1f));
                EmitAlias(ButtonAlias.Grip, true, 1f, ref gripPressed);
            }
            else if (VRTK_SDK_Bridge.IsGripPressedUpOnIndex(controllerIndex))
            {
                OnGripReleased(SetButtonEvent(ref gripPressed, false, 0f));
                EmitAlias(ButtonAlias.Grip, false, 0f, ref gripPressed);
            }

            //Touchpad Pressed
            if (VRTK_SDK_Bridge.IsTouchpadPressedDownOnIndex(controllerIndex))
            {
                OnTouchpadPressed(SetButtonEvent(ref touchpadPressed, true, 1f));
                EmitAlias(ButtonAlias.Touchpad_Press, true, 1f, ref touchpadPressed);
            }
            else if (VRTK_SDK_Bridge.IsTouchpadPressedUpOnIndex(controllerIndex))
            {
                OnTouchpadReleased(SetButtonEvent(ref touchpadPressed, false, 0f));
                EmitAlias(ButtonAlias.Touchpad_Press, false, 0f, ref touchpadPressed);
            }

            //Touchpad Touched
            if (VRTK_SDK_Bridge.IsTouchpadTouchedDownOnIndex(controllerIndex))
            {
                OnTouchpadTouchStart(SetButtonEvent(ref touchpadTouched, true, 1f));
                EmitAlias(ButtonAlias.Touchpad_Touch, true, 1f, ref touchpadTouched);
            }
            else if (VRTK_SDK_Bridge.IsTouchpadTouchedUpOnIndex(controllerIndex))
            {
                OnTouchpadTouchEnd(SetButtonEvent(ref touchpadTouched, false, 0f));
                EmitAlias(ButtonAlias.Touchpad_Touch, false, 0f, ref touchpadTouched);
            }

            if (Vector2ShallowEquals(touchpadAxis, currentTouchpadAxis))
            {
                touchpadAxisChanged = false;
            }
            else
            {
                OnTouchpadAxisChanged(SetButtonEvent(ref touchpadAxisChanged, true, 1f));
            }

            // Save current touch and trigger settings to detect next change.
            touchpadAxis = new Vector2(currentTouchpadAxis.x, currentTouchpadAxis.y);
            triggerAxis = new Vector2(currentTriggerAxis.x, currentTriggerAxis.y);
            hairTriggerDelta = VRTK_SDK_Bridge.GetTriggerHairlineDeltaOnIndex(controllerIndex);
        }

        private void SetVelocity()
        {
            controllerVelocity = VRTK_SDK_Bridge.GetVelocityOnIndex(controllerIndex); ;
            controllerAngularVelocity = VRTK_SDK_Bridge.GetAngularVelocityOnIndex(controllerIndex); ;
        }
    }
}