// Controller Events|Interactions|30010
namespace VRTK
{
    using UnityEngine;
    using System;

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
    /// The Controller Events script requires the Controller Mapper script on the same GameObject and provides event listeners for every button press on the controller (excluding the System Menu button as this cannot be overridden and is always used by Steam).
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
        /// <param name="Undefined">No button specified</param>
        /// <param name="Trigger_Hairline">The trigger is squeezed past the current hairline threshold.</param>
        /// <param name="Trigger_Touch">The trigger is squeezed a small amount.</param>
        /// <param name="Trigger_Press">The trigger is squeezed about half way in.</param>
        /// <param name="Trigger_Click">The trigger is squeezed all the way down.</param>
        /// <param name="Grip_Hairline">The grip is squeezed past the current hairline threshold.</param>
        /// <param name="Grip_Touch">The grip button is touched.</param>
        /// <param name="Grip_Press">The grip button is pressed.</param>
        /// <param name="Grip_Click">The grip button is pressed all the way down.</param>
        /// <param name="Touchpad_Touch">The touchpad is touched (without pressing down to click).</param>
        /// <param name="Touchpad_Press">The touchpad is pressed (to the point of hearing a click).</param>
        /// <param name="Button_One_Touch">The button one is touched.</param>
        /// <param name="Button_One_Press">The button one is pressed.</param>
        /// <param name="Button_Two_Touch">The button one is touched.</param>
        /// <param name="Button_Two_Press">The button one is pressed.</param>
        /// <param name="Start_Menu_Press">The button one is pressed.</param>
        public enum ButtonAlias
        {
            Undefined,
            Trigger_Hairline,
            Trigger_Touch,
            Trigger_Press,
            Trigger_Click,
            Grip_Hairline,
            Grip_Touch,
            Grip_Press,
            Grip_Click,
            Touchpad_Touch,
            Touchpad_Press,
            Button_One_Touch,
            Button_One_Press,
            Button_Two_Touch,
            Button_Two_Press,
            Start_Menu_Press
        }

        [Header("Action Alias Buttons")]

        [Tooltip("The button to use for the action of turning a laser pointer on / off.")]
        [Obsolete("`VRTK_ControllerEvents.pointerToggleButton` is no longer used in the new `VRTK_Pointer` class. This parameter will be removed in a future version of VRTK.")]
        public ButtonAlias pointerToggleButton = ButtonAlias.Touchpad_Press;
        [Tooltip("The button to use for the action of setting a destination marker from the cursor position of the pointer.")]
        [Obsolete("`VRTK_ControllerEvents.pointerSetButton` is no longer used in the new `VRTK_Pointer` class. This parameter will be removed in a future version of VRTK.")]
        public ButtonAlias pointerSetButton = ButtonAlias.Touchpad_Press;
        [Tooltip("The button to use for the action of grabbing game objects.")]
        public ButtonAlias grabToggleButton = ButtonAlias.Grip_Press;
        [Tooltip("The button to use for the action of using game objects.")]
        public ButtonAlias useToggleButton = ButtonAlias.Trigger_Press;
        [Tooltip("The button to use for the action of clicking a UI element.")]
        [Obsolete("`VRTK_ControllerEvents.uiClickButton` is no longer used in the `VRTK_UIPointer` class. This parameter will be removed in a future version of VRTK.")]
        public ButtonAlias uiClickButton = ButtonAlias.Trigger_Press;
        [Tooltip("The button to use for the action of bringing up an in-game menu.")]
        public ButtonAlias menuToggleButton = ButtonAlias.Button_Two_Press;

        [Header("Axis Refinement")]

        [Tooltip("The amount of fidelity in the changes on the axis, which is defaulted to 1. Any number higher than 2 will probably give too sensitive results.")]
        public int axisFidelity = 1;
        [Tooltip("The level on the trigger axis to reach before a click is registered.")]
        public float triggerClickThreshold = 1f;
        [Tooltip("The level on the trigger axis to reach before the axis is forced to 0f.")]
        public float triggerForceZeroThreshold = 0.01f;
        [Tooltip("If this is checked then the trigger axis will be forced to 0f when the trigger button reports an untouch event.")]
        public bool triggerAxisZeroOnUntouch = false;
        [Tooltip("The level on the grip axis to reach before a click is registered.")]
        public float gripClickThreshold = 1f;
        [Tooltip("The level on the grip axis to reach before the axis is forced to 0f.")]
        public float gripForceZeroThreshold = 0.01f;
        [Tooltip("If this is checked then the grip axis will be forced to 0f when the grip button reports an untouch event.")]
        public bool gripAxisZeroOnUntouch = false;

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
        /// This will be true if the trigger is squeezed all the way down.
        /// </summary>
        [HideInInspector]
        public bool triggerClicked = false;
        /// <summary>
        /// This will be true if the trigger has been squeezed more or less.
        /// </summary>
        [HideInInspector]
        public bool triggerAxisChanged = false;

        /// <summary>
        /// This will be true if the grip is squeezed about half way in.
        /// </summary>
        [HideInInspector]
        public bool gripPressed = false;
        /// <summary>
        /// This will be true if the grip is touched.
        /// </summary>
        [HideInInspector]
        public bool gripTouched = false;
        /// <summary>
        /// This will be true if the grip is squeezed a small amount more from any previous squeeze on the grip.
        /// </summary>
        [HideInInspector]
        public bool gripHairlinePressed = false;
        /// <summary>
        /// This will be true if the grip is squeezed all the way down.
        /// </summary>
        [HideInInspector]
        public bool gripClicked = false;
        /// <summary>
        /// This will be true if the grip has been squeezed more or less.
        /// </summary>
        [HideInInspector]
        public bool gripAxisChanged = false;

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
        /// This will be true if button one is held down.
        /// </summary>
        [HideInInspector]
        public bool buttonOnePressed = false;
        /// <summary>
        /// This will be true if button one is being touched.
        /// </summary>
        [HideInInspector]
        public bool buttonOneTouched = false;

        /// <summary>
        /// This will be true if button two is held down.
        /// </summary>
        [HideInInspector]
        public bool buttonTwoPressed = false;
        /// <summary>
        /// This will be true if button two is being touched.
        /// </summary>
        [HideInInspector]
        public bool buttonTwoTouched = false;

        /// <summary>
        /// This will be true if start menu is held down.
        /// </summary>
        [HideInInspector]
        public bool startMenuPressed = false;

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
        /// This will be true if the button aliased to the menu is held down.
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
        /// Emitted when the trigger is squeezed all the way down.
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
        /// Emitted when the grip is squeezed about half way in.
        /// </summary>
        public event ControllerInteractionEventHandler GripPressed;
        /// <summary>
        /// Emitted when the grip is released under half way.
        /// </summary>
        public event ControllerInteractionEventHandler GripReleased;

        /// <summary>
        /// Emitted when the grip is squeezed a small amount.
        /// </summary>
        public event ControllerInteractionEventHandler GripTouchStart;
        /// <summary>
        /// Emitted when the grip is no longer being squeezed at all.
        /// </summary>
        public event ControllerInteractionEventHandler GripTouchEnd;

        /// <summary>
        /// Emitted when the grip is squeezed past the current hairline threshold.
        /// </summary>
        public event ControllerInteractionEventHandler GripHairlineStart;
        /// <summary>
        /// Emitted when the grip is released past the current hairline threshold.
        /// </summary>
        public event ControllerInteractionEventHandler GripHairlineEnd;

        /// <summary>
        /// Emitted when the grip is squeezed all the way down.
        /// </summary>
        public event ControllerInteractionEventHandler GripClicked;
        /// <summary>
        /// Emitted when the grip is no longer being held all the way down.
        /// </summary>
        public event ControllerInteractionEventHandler GripUnclicked;

        /// <summary>
        /// Emitted when the amount of squeeze on the grip changes.
        /// </summary>
        public event ControllerInteractionEventHandler GripAxisChanged;

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
        /// Emitted when button one is touched.
        /// </summary>
        public event ControllerInteractionEventHandler ButtonOneTouchStart;
        /// <summary>
        /// Emitted when button one is no longer being touched.
        /// </summary>
        public event ControllerInteractionEventHandler ButtonOneTouchEnd;
        /// <summary>
        /// Emitted when button one is pressed.
        /// </summary>
        public event ControllerInteractionEventHandler ButtonOnePressed;
        /// <summary>
        /// Emitted when button one is released.
        /// </summary>
        public event ControllerInteractionEventHandler ButtonOneReleased;

        /// <summary>
        /// Emitted when button two is touched.
        /// </summary>
        public event ControllerInteractionEventHandler ButtonTwoTouchStart;
        /// <summary>
        /// Emitted when button two is no longer being touched.
        /// </summary>
        public event ControllerInteractionEventHandler ButtonTwoTouchEnd;
        /// <summary>
        /// Emitted when button two is pressed.
        /// </summary>
        public event ControllerInteractionEventHandler ButtonTwoPressed;
        /// <summary>
        /// Emitted when button two is released.
        /// </summary>
        public event ControllerInteractionEventHandler ButtonTwoReleased;

        /// <summary>
        /// Emitted when start menu is pressed.
        /// </summary>
        public event ControllerInteractionEventHandler StartMenuPressed;
        /// <summary>
        /// Emitted when start menu is released.
        /// </summary>
        public event ControllerInteractionEventHandler StartMenuReleased;

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
        /// <summary>
        /// Emitted when the controller index changed.
        /// </summary>
        public event ControllerInteractionEventHandler ControllerIndexChanged;

        private Vector2 touchpadAxis = Vector2.zero;
        private Vector2 triggerAxis = Vector2.zero;
        private Vector2 gripAxis = Vector2.zero;

        private float hairTriggerDelta;
        private float hairGripDelta;

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

        public virtual void OnGripTouchStart(ControllerInteractionEventArgs e)
        {
            if (GripTouchStart != null)
            {
                GripTouchStart(this, e);
            }
        }

        public virtual void OnGripTouchEnd(ControllerInteractionEventArgs e)
        {
            if (GripTouchEnd != null)
            {
                GripTouchEnd(this, e);
            }
        }

        public virtual void OnGripHairlineStart(ControllerInteractionEventArgs e)
        {
            if (GripHairlineStart != null)
            {
                GripHairlineStart(this, e);
            }
        }

        public virtual void OnGripHairlineEnd(ControllerInteractionEventArgs e)
        {
            if (GripHairlineEnd != null)
            {
                GripHairlineEnd(this, e);
            }
        }

        public virtual void OnGripClicked(ControllerInteractionEventArgs e)
        {
            if (GripClicked != null)
            {
                GripClicked(this, e);
            }
        }

        public virtual void OnGripUnclicked(ControllerInteractionEventArgs e)
        {
            if (GripUnclicked != null)
            {
                GripUnclicked(this, e);
            }
        }

        public virtual void OnGripAxisChanged(ControllerInteractionEventArgs e)
        {
            if (GripAxisChanged != null)
            {
                GripAxisChanged(this, e);
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

        public virtual void OnButtonOneTouchStart(ControllerInteractionEventArgs e)
        {
            if (ButtonOneTouchStart != null)
            {
                ButtonOneTouchStart(this, e);
            }
        }

        public virtual void OnButtonOneTouchEnd(ControllerInteractionEventArgs e)
        {
            if (ButtonOneTouchEnd != null)
            {
                ButtonOneTouchEnd(this, e);
            }
        }

        public virtual void OnButtonOnePressed(ControllerInteractionEventArgs e)
        {
            if (ButtonOnePressed != null)
            {
                ButtonOnePressed(this, e);
            }
        }

        public virtual void OnButtonOneReleased(ControllerInteractionEventArgs e)
        {
            if (ButtonOneReleased != null)
            {
                ButtonOneReleased(this, e);
            }
        }

        public virtual void OnButtonTwoTouchStart(ControllerInteractionEventArgs e)
        {
            if (ButtonTwoTouchStart != null)
            {
                ButtonTwoTouchStart(this, e);
            }
        }

        public virtual void OnButtonTwoTouchEnd(ControllerInteractionEventArgs e)
        {
            if (ButtonTwoTouchEnd != null)
            {
                ButtonTwoTouchEnd(this, e);
            }
        }

        public virtual void OnButtonTwoPressed(ControllerInteractionEventArgs e)
        {
            if (ButtonTwoPressed != null)
            {
                ButtonTwoPressed(this, e);
            }
        }

        public virtual void OnButtonTwoReleased(ControllerInteractionEventArgs e)
        {
            if (ButtonTwoReleased != null)
            {
                ButtonTwoReleased(this, e);
            }
        }

        public virtual void OnStartMenuPressed(ControllerInteractionEventArgs e)
        {
            if (StartMenuPressed != null)
            {
                StartMenuPressed(this, e);
            }
        }

        public virtual void OnStartMenuReleased(ControllerInteractionEventArgs e)
        {
            if (StartMenuReleased != null)
            {
                StartMenuReleased(this, e);
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

        public virtual void OnControllerIndexChanged(ControllerInteractionEventArgs e)
        {
            if (ControllerIndexChanged != null)
            {
                ControllerIndexChanged(this, e);
            }
        }

        /// <summary>
        /// The GetVelocity method is useful for getting the current velocity of the physical game controller. This can be useful to determine the speed at which the controller is being swung or the direction it is being moved in.
        /// </summary>
        /// <returns>A 3 dimensional vector containing the current real world physical controller velocity.</returns>
        [Obsolete("`VRTK_ControllerEvents.GetVelocity()` has been replaced with `VRTK_DeviceFinder.GetControllerVelocity(givenController)`. This method will be removed in a future version of VRTK.")]
        public Vector3 GetVelocity()
        {
            return VRTK_DeviceFinder.GetControllerVelocity(gameObject);
        }

        /// <summary>
        /// The GetAngularVelocity method is useful for getting the current rotational velocity of the physical game controller. This can be useful for determining which way the controller is being rotated and at what speed the rotation is occurring.
        /// </summary>
        /// <returns>A 3 dimensional vector containing the current real world physical controller angular (rotational) velocity.</returns>
        [Obsolete("`VRTK_ControllerEvents.GetAngularVelocity()` has been replaced with `VRTK_DeviceFinder.GetControllerAngularVelocity(givenController)`. This method will be removed in a future version of VRTK.")]
        public Vector3 GetAngularVelocity()
        {
            return VRTK_DeviceFinder.GetControllerAngularVelocity(gameObject);
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
        /// The GetGripAxis method returns a float that represents how much the grip is being squeezed. This can be useful for using the grip axis to perform high fidelity tasks or only activating the grip press once it has exceeded a given press threshold.
        /// </summary>
        /// <returns>A float representing the amount of squeeze that is being applied to the grip. `0f` to `1f`.</returns>
        public float GetGripAxis()
        {
            return gripAxis.x;
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
        /// The GetHairTriggerDelta method returns a float representing the difference in how much the trigger is being pressed in relation to the hairline threshold start.
        /// </summary>
        /// <returns>A float representing the difference in the trigger pressure from the hairline threshold start to current position.</returns>
        public float GetHairGripDelta()
        {
            return hairGripDelta;
        }

        /// <summary>
        /// The AnyButtonPressed method returns true if any of the controller buttons are being pressed and this can be useful to determine if an action can be taken whilst the user is using the controller.
        /// </summary>
        /// <returns>Is true if any of the controller buttons are currently being pressed.</returns>
        public bool AnyButtonPressed()
        {
            return (triggerPressed || gripPressed || touchpadPressed || buttonOnePressed || buttonTwoPressed || startMenuPressed);
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
                case ButtonAlias.Grip_Hairline:
                    return gripHairlinePressed;
                case ButtonAlias.Grip_Touch:
                    return gripTouched;
                case ButtonAlias.Grip_Press:
                    return gripPressed;
                case ButtonAlias.Grip_Click:
                    return gripClicked;
                case ButtonAlias.Touchpad_Touch:
                    return touchpadTouched;
                case ButtonAlias.Touchpad_Press:
                    return touchpadPressed;
                case ButtonAlias.Button_One_Press:
                    return buttonOnePressed;
                case ButtonAlias.Button_One_Touch:
                    return buttonOneTouched;
                case ButtonAlias.Button_Two_Press:
                    return buttonTwoPressed;
                case ButtonAlias.Button_Two_Touch:
                    return buttonTwoTouched;
                case ButtonAlias.Start_Menu_Press:
                    return startMenuPressed;
            }
            return false;
        }

        /// <summary>
        /// The SubscribeToButtonAliasEvent method makes it easier to subscribe to a button event on either the start or end action. Upon the event firing, the given callback method is executed.
        /// </summary>
        /// <param name="givenButton">The ButtonAlias to register the event on.</param>
        /// <param name="startEvent">If this is `true` then the start event related to the button is used (e.g. OnPress). If this is `false` then the end event related to the button is used (e.g. OnRelease). </param>
        /// <param name="callbackMethod">The method to subscribe to the event.</param>
        public void SubscribeToButtonAliasEvent(ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)
        {
            ButtonAliasEventSubscription(true, givenButton, startEvent, callbackMethod);
        }

        /// <summary>
        /// The UnsubscribeToButtonAliasEvent method makes it easier to unsubscribe to from button event on either the start or end action.
        /// </summary>
        /// <param name="givenButton">The ButtonAlias to unregister the event on.</param>
        /// <param name="startEvent">If this is `true` then the start event related to the button is used (e.g. OnPress). If this is `false` then the end event related to the button is used (e.g. OnRelease). </param>
        /// <param name="callbackMethod">The method to unsubscribe from the event.</param>
        public void UnsubscribeToButtonAliasEvent(ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)
        {
            ButtonAliasEventSubscription(false, givenButton, startEvent, callbackMethod);
        }

        protected virtual void OnEnable()
        {
            var actualController = VRTK_DeviceFinder.GetActualController(gameObject);
            if (actualController)
            {
                var controllerTracker = actualController.GetComponent<VRTK_TrackedController>();
                if (controllerTracker)
                {
                    controllerTracker.ControllerEnabled += TrackedControllerEnabled;
                    controllerTracker.ControllerDisabled += TrackedControllerDisabled;
                    controllerTracker.ControllerIndexChanged += TrackedControllerIndexChanged;
                }
            }
        }

        protected virtual void OnDisable()
        {
            Invoke("DisableEvents", 0f);
            var actualController = VRTK_DeviceFinder.GetActualController(gameObject);
            if (actualController)
            {
                var controllerTracker = actualController.GetComponent<VRTK_TrackedController>();
                if (controllerTracker)
                {
                    controllerTracker.ControllerEnabled -= TrackedControllerEnabled;
                    controllerTracker.ControllerDisabled -= TrackedControllerDisabled;
                }
            }
        }

        protected virtual void Update()
        {
            var controllerIndex = VRTK_DeviceFinder.GetControllerIndex(gameObject);

            //Only continue if the controller index has been set to a sensible number
            if (controllerIndex >= uint.MaxValue)
            {
                return;
            }

            Vector2 currentTriggerAxis = VRTK_SDK_Bridge.GetTriggerAxisOnIndex(controllerIndex);
            Vector2 currentGripAxis = VRTK_SDK_Bridge.GetGripAxisOnIndex(controllerIndex);
            Vector2 currentTouchpadAxis = VRTK_SDK_Bridge.GetTouchpadAxisOnIndex(controllerIndex);

            //Trigger Touched
            if (VRTK_SDK_Bridge.IsTriggerTouchedDownOnIndex(controllerIndex))
            {
                OnTriggerTouchStart(SetButtonEvent(ref triggerTouched, true, currentTriggerAxis.x));
                EmitAlias(ButtonAlias.Trigger_Touch, true, currentTriggerAxis.x, ref triggerTouched);
            }

            //Trigger Hairline
            if (VRTK_SDK_Bridge.IsHairTriggerDownOnIndex(controllerIndex))
            {
                OnTriggerHairlineStart(SetButtonEvent(ref triggerHairlinePressed, true, currentTriggerAxis.x));
                EmitAlias(ButtonAlias.Trigger_Hairline, true, currentTriggerAxis.x, ref triggerHairlinePressed);
            }

            //Trigger Pressed
            if (VRTK_SDK_Bridge.IsTriggerPressedDownOnIndex(controllerIndex))
            {
                OnTriggerPressed(SetButtonEvent(ref triggerPressed, true, currentTriggerAxis.x));
                EmitAlias(ButtonAlias.Trigger_Press, true, currentTriggerAxis.x, ref triggerPressed);
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

            // Trigger Pressed end
            if (VRTK_SDK_Bridge.IsTriggerPressedUpOnIndex(controllerIndex))
            {
                OnTriggerReleased(SetButtonEvent(ref triggerPressed, false, 0f));
                EmitAlias(ButtonAlias.Trigger_Press, false, 0f, ref triggerPressed);
            }

            //Trigger Hairline End
            if (VRTK_SDK_Bridge.IsHairTriggerUpOnIndex(controllerIndex))
            {
                OnTriggerHairlineEnd(SetButtonEvent(ref triggerHairlinePressed, false, 0f));
                EmitAlias(ButtonAlias.Trigger_Hairline, false, 0f, ref triggerHairlinePressed);
            }

            //Trigger Touch End
            if (VRTK_SDK_Bridge.IsTriggerTouchedUpOnIndex(controllerIndex))
            {
                OnTriggerTouchEnd(SetButtonEvent(ref triggerTouched, false, 0f));
                EmitAlias(ButtonAlias.Trigger_Touch, false, 0f, ref triggerTouched);
            }

            //Trigger Axis
            currentTriggerAxis.x = ((!triggerTouched && triggerAxisZeroOnUntouch) || currentTriggerAxis.x < triggerForceZeroThreshold ? 0f : currentTriggerAxis.x);
            if (Vector2ShallowEquals(triggerAxis, currentTriggerAxis))
            {
                triggerAxisChanged = false;
            }
            else
            {
                OnTriggerAxisChanged(SetButtonEvent(ref triggerAxisChanged, true, currentTriggerAxis.x));
            }

            //Grip Touched
            if (VRTK_SDK_Bridge.IsGripTouchedDownOnIndex(controllerIndex))
            {
                OnGripTouchStart(SetButtonEvent(ref gripTouched, true, currentGripAxis.x));
                EmitAlias(ButtonAlias.Grip_Touch, true, currentGripAxis.x, ref gripTouched);
            }

            //Grip Hairline
            if (VRTK_SDK_Bridge.IsHairGripDownOnIndex(controllerIndex))
            {
                OnGripHairlineStart(SetButtonEvent(ref gripHairlinePressed, true, currentGripAxis.x));
                EmitAlias(ButtonAlias.Grip_Hairline, true, currentGripAxis.x, ref gripHairlinePressed);
            }

            //Grip Pressed
            if (VRTK_SDK_Bridge.IsGripPressedDownOnIndex(controllerIndex))
            {
                OnGripPressed(SetButtonEvent(ref gripPressed, true, currentGripAxis.x));
                EmitAlias(ButtonAlias.Grip_Press, true, currentGripAxis.x, ref gripPressed);
            }

            //Grip Clicked
            if (!gripClicked && currentGripAxis.x >= gripClickThreshold)
            {
                OnGripClicked(SetButtonEvent(ref gripClicked, true, currentGripAxis.x));
                EmitAlias(ButtonAlias.Grip_Click, true, currentGripAxis.x, ref gripClicked);
            }
            else if (gripClicked && currentGripAxis.x < gripClickThreshold)
            {
                OnGripUnclicked(SetButtonEvent(ref gripClicked, false, 0f));
                EmitAlias(ButtonAlias.Grip_Click, false, 0f, ref gripClicked);
            }

            // Grip Pressed End
            if (VRTK_SDK_Bridge.IsGripPressedUpOnIndex(controllerIndex))
            {
                OnGripReleased(SetButtonEvent(ref gripPressed, false, 0f));
                EmitAlias(ButtonAlias.Grip_Press, false, 0f, ref gripPressed);
            }

            //Grip Hairline End
            if (VRTK_SDK_Bridge.IsHairGripUpOnIndex(controllerIndex))
            {
                OnGripHairlineEnd(SetButtonEvent(ref gripHairlinePressed, false, 0f));
                EmitAlias(ButtonAlias.Grip_Hairline, false, 0f, ref gripHairlinePressed);
            }

            // Grip Touch End
            if (VRTK_SDK_Bridge.IsGripTouchedUpOnIndex(controllerIndex))
            {
                OnGripTouchEnd(SetButtonEvent(ref gripTouched, false, 0f));
                EmitAlias(ButtonAlias.Grip_Touch, false, 0f, ref gripTouched);
            }

            //Grip Axis
            currentGripAxis.x = ((!gripTouched && gripAxisZeroOnUntouch) || currentGripAxis.x < gripForceZeroThreshold ? 0f : currentGripAxis.x);
            if (Vector2ShallowEquals(gripAxis, currentGripAxis))
            {
                gripAxisChanged = false;
            }
            else
            {
                OnGripAxisChanged(SetButtonEvent(ref gripAxisChanged, true, currentGripAxis.x));
            }

            //Touchpad Touched
            if (VRTK_SDK_Bridge.IsTouchpadTouchedDownOnIndex(controllerIndex))
            {
                OnTouchpadTouchStart(SetButtonEvent(ref touchpadTouched, true, 1f));
                EmitAlias(ButtonAlias.Touchpad_Touch, true, 1f, ref touchpadTouched);
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

            //Touchpad Untouched
            if (VRTK_SDK_Bridge.IsTouchpadTouchedUpOnIndex(controllerIndex))
            {
                OnTouchpadTouchEnd(SetButtonEvent(ref touchpadTouched, false, 0f));
                EmitAlias(ButtonAlias.Touchpad_Touch, false, 0f, ref touchpadTouched);
                touchpadAxis = Vector2.zero;
            }

            //Touchpad Axis
            if (!touchpadTouched || Vector2ShallowEquals(touchpadAxis, currentTouchpadAxis))
            {
                touchpadAxisChanged = false;
            }
            else
            {
                OnTouchpadAxisChanged(SetButtonEvent(ref touchpadAxisChanged, true, 1f));
            }

            //ButtonOne Touched
            if (VRTK_SDK_Bridge.IsButtonOneTouchedDownOnIndex(controllerIndex))
            {
                OnButtonOneTouchStart(SetButtonEvent(ref buttonOneTouched, true, 1f));
                EmitAlias(ButtonAlias.Button_One_Touch, true, 1f, ref buttonOneTouched);
            }

            //ButtonOne Pressed
            if (VRTK_SDK_Bridge.IsButtonOnePressedDownOnIndex(controllerIndex))
            {
                OnButtonOnePressed(SetButtonEvent(ref buttonOnePressed, true, 1f));
                EmitAlias(ButtonAlias.Button_One_Press, true, 1f, ref buttonOnePressed);
            }
            else if (VRTK_SDK_Bridge.IsButtonOnePressedUpOnIndex(controllerIndex))
            {
                OnButtonOneReleased(SetButtonEvent(ref buttonOnePressed, false, 0f));
                EmitAlias(ButtonAlias.Button_One_Press, false, 0f, ref buttonOnePressed);
            }

            //ButtonOne Touched End
            if (VRTK_SDK_Bridge.IsButtonOneTouchedUpOnIndex(controllerIndex))
            {
                OnButtonOneTouchEnd(SetButtonEvent(ref buttonOneTouched, false, 0f));
                EmitAlias(ButtonAlias.Button_One_Touch, false, 0f, ref buttonOneTouched);
            }

            //ButtonTwo Touched
            if (VRTK_SDK_Bridge.IsButtonTwoTouchedDownOnIndex(controllerIndex))
            {
                OnButtonTwoTouchStart(SetButtonEvent(ref buttonTwoTouched, true, 1f));
                EmitAlias(ButtonAlias.Button_Two_Touch, true, 1f, ref buttonTwoTouched);
            }

            //ButtonTwo Pressed
            if (VRTK_SDK_Bridge.IsButtonTwoPressedDownOnIndex(controllerIndex))
            {
                OnButtonTwoPressed(SetButtonEvent(ref buttonTwoPressed, true, 1f));
                EmitAlias(ButtonAlias.Button_Two_Press, true, 1f, ref buttonTwoPressed);
            }
            else if (VRTK_SDK_Bridge.IsButtonTwoPressedUpOnIndex(controllerIndex))
            {
                OnButtonTwoReleased(SetButtonEvent(ref buttonTwoPressed, false, 0f));
                EmitAlias(ButtonAlias.Button_Two_Press, false, 0f, ref buttonTwoPressed);
            }

            //ButtonTwo Touched End
            if (VRTK_SDK_Bridge.IsButtonTwoTouchedUpOnIndex(controllerIndex))
            {
                OnButtonTwoTouchEnd(SetButtonEvent(ref buttonTwoTouched, false, 0f));
                EmitAlias(ButtonAlias.Button_Two_Touch, false, 0f, ref buttonTwoTouched);
            }

            //StartMenu Pressed
            if (VRTK_SDK_Bridge.IsStartMenuPressedDownOnIndex(controllerIndex))
            {
                OnStartMenuPressed(SetButtonEvent(ref startMenuPressed, true, 1f));
                EmitAlias(ButtonAlias.Start_Menu_Press, true, 1f, ref startMenuPressed);
            }
            else if (VRTK_SDK_Bridge.IsStartMenuPressedUpOnIndex(controllerIndex))
            {
                OnStartMenuReleased(SetButtonEvent(ref startMenuPressed, false, 0f));
                EmitAlias(ButtonAlias.Start_Menu_Press, false, 0f, ref startMenuPressed);
            }

            // Save current touch and trigger settings to detect next change.
            touchpadAxis = (touchpadAxisChanged ? new Vector2(currentTouchpadAxis.x, currentTouchpadAxis.y) : touchpadAxis);
            triggerAxis = (triggerAxisChanged ? new Vector2(currentTriggerAxis.x, currentTriggerAxis.y) : triggerAxis);
            gripAxis = (gripAxisChanged ? new Vector2(currentGripAxis.x, currentGripAxis.y) : gripAxis);

            hairTriggerDelta = VRTK_SDK_Bridge.GetTriggerHairlineDeltaOnIndex(controllerIndex);
            hairGripDelta = VRTK_SDK_Bridge.GetGripHairlineDeltaOnIndex(controllerIndex);
        }

        protected void ButtonAliasEventSubscription(bool subscribe, ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)
        {
            switch (givenButton)
            {
                case ButtonAlias.Trigger_Click:
                    if (subscribe)
                    {
                        if (startEvent)
                        {
                            TriggerClicked += callbackMethod;
                        }
                        else
                        {
                            TriggerUnclicked += callbackMethod;
                        }
                    }
                    else
                    {
                        if (startEvent)
                        {
                            TriggerClicked -= callbackMethod;
                        }
                        else
                        {
                            TriggerUnclicked -= callbackMethod;
                        }
                    }
                    break;
                case ButtonAlias.Trigger_Hairline:
                    if (subscribe)
                    {
                        if (startEvent)
                        {
                            TriggerHairlineStart += callbackMethod;
                        }
                        else
                        {
                            TriggerHairlineEnd += callbackMethod;
                        }
                    }
                    else
                    {
                        if (startEvent)
                        {
                            TriggerHairlineStart -= callbackMethod;
                        }
                        else
                        {
                            TriggerHairlineEnd -= callbackMethod;
                        }
                    }
                    break;
                case ButtonAlias.Trigger_Press:
                    if (subscribe)
                    {
                        if (startEvent)
                        {
                            TriggerPressed += callbackMethod;
                        }
                        else
                        {
                            TriggerReleased += callbackMethod;
                        }
                    }
                    else
                    {
                        if (startEvent)
                        {
                            TriggerPressed -= callbackMethod;
                        }
                        else
                        {
                            TriggerReleased -= callbackMethod;
                        }
                    }
                    break;
                case ButtonAlias.Trigger_Touch:
                    if (subscribe)
                    {
                        if (startEvent)
                        {
                            TriggerTouchStart += callbackMethod;
                        }
                        else
                        {
                            TriggerTouchEnd += callbackMethod;
                        }
                    }
                    else
                    {
                        if (startEvent)
                        {
                            TriggerTouchStart -= callbackMethod;
                        }
                        else
                        {
                            TriggerTouchEnd -= callbackMethod;
                        }
                    }
                    break;
                case ButtonAlias.Grip_Click:
                    if (subscribe)
                    {
                        if (startEvent)
                        {
                            GripClicked += callbackMethod;
                        }
                        else
                        {
                            GripUnclicked += callbackMethod;
                        }
                    }
                    else
                    {
                        if (startEvent)
                        {
                            GripClicked -= callbackMethod;
                        }
                        else
                        {
                            GripUnclicked -= callbackMethod;
                        }
                    }
                    break;
                case ButtonAlias.Grip_Hairline:
                    if (subscribe)
                    {
                        if (startEvent)
                        {
                            GripHairlineStart += callbackMethod;
                        }
                        else
                        {
                            GripHairlineEnd += callbackMethod;
                        }
                    }
                    else
                    {
                        if (startEvent)
                        {
                            GripHairlineStart -= callbackMethod;
                        }
                        else
                        {
                            GripHairlineEnd -= callbackMethod;
                        }
                    }
                    break;
                case ButtonAlias.Grip_Press:
                    if (subscribe)
                    {
                        if (startEvent)
                        {
                            GripPressed += callbackMethod;
                        }
                        else
                        {
                            GripReleased += callbackMethod;
                        }
                    }
                    else
                    {
                        if (startEvent)
                        {
                            GripPressed -= callbackMethod;
                        }
                        else
                        {
                            GripReleased -= callbackMethod;
                        }
                    }
                    break;
                case ButtonAlias.Grip_Touch:
                    if (subscribe)
                    {
                        if (startEvent)
                        {
                            GripTouchStart += callbackMethod;
                        }
                        else
                        {
                            GripTouchEnd += callbackMethod;
                        }
                    }
                    else
                    {
                        if (startEvent)
                        {
                            GripTouchStart -= callbackMethod;
                        }
                        else
                        {
                            GripTouchEnd -= callbackMethod;
                        }
                    }
                    break;
                case ButtonAlias.Touchpad_Press:
                    if (subscribe)
                    {
                        if (startEvent)
                        {
                            TouchpadPressed += callbackMethod;
                        }
                        else
                        {
                            TouchpadReleased += callbackMethod;
                        }
                    }
                    else
                    {
                        if (startEvent)
                        {
                            TouchpadPressed -= callbackMethod;
                        }
                        else
                        {
                            TouchpadReleased -= callbackMethod;
                        }
                    }
                    break;
                case ButtonAlias.Touchpad_Touch:
                    if (subscribe)
                    {
                        if (startEvent)
                        {
                            TouchpadTouchStart += callbackMethod;
                        }
                        else
                        {
                            TouchpadTouchEnd += callbackMethod;
                        }
                    }
                    else
                    {
                        if (startEvent)
                        {
                            TouchpadTouchStart -= callbackMethod;
                        }
                        else
                        {
                            TouchpadTouchEnd -= callbackMethod;
                        }
                    }
                    break;
                case ButtonAlias.Button_One_Press:
                    if (subscribe)
                    {
                        if (startEvent)
                        {
                            ButtonOnePressed += callbackMethod;
                        }
                        else
                        {
                            ButtonOneReleased += callbackMethod;
                        }
                    }
                    else
                    {
                        if (startEvent)
                        {
                            ButtonOnePressed -= callbackMethod;
                        }
                        else
                        {
                            ButtonOneReleased -= callbackMethod;
                        }
                    }
                    break;
                case ButtonAlias.Button_One_Touch:
                    if (subscribe)
                    {
                        if (startEvent)
                        {
                            ButtonOneTouchStart += callbackMethod;
                        }
                        else
                        {
                            ButtonOneTouchEnd += callbackMethod;
                        }
                    }
                    else
                    {
                        if (startEvent)
                        {
                            ButtonOneTouchStart -= callbackMethod;
                        }
                        else
                        {
                            ButtonOneTouchEnd -= callbackMethod;
                        }
                    }
                    break;
                case ButtonAlias.Button_Two_Press:
                    if (subscribe)
                    {
                        if (startEvent)
                        {
                            ButtonTwoPressed += callbackMethod;
                        }
                        else
                        {
                            ButtonTwoReleased += callbackMethod;
                        }
                    }
                    else
                    {
                        if (startEvent)
                        {
                            ButtonTwoPressed -= callbackMethod;
                        }
                        else
                        {
                            ButtonTwoReleased -= callbackMethod;
                        }
                    }
                    break;
                case ButtonAlias.Button_Two_Touch:
                    if (subscribe)
                    {
                        if (startEvent)
                        {
                            ButtonTwoTouchStart += callbackMethod;
                        }
                        else
                        {
                            ButtonTwoTouchEnd += callbackMethod;
                        }
                    }
                    else
                    {
                        if (startEvent)
                        {
                            ButtonTwoTouchStart -= callbackMethod;
                        }
                        else
                        {
                            ButtonTwoTouchEnd -= callbackMethod;
                        }
                    }
                    break;
                case ButtonAlias.Start_Menu_Press:
                    if (subscribe)
                    {
                        if (startEvent)
                        {
                            StartMenuPressed += callbackMethod;
                        }
                        else
                        {
                            StartMenuReleased += callbackMethod;
                        }
                    }
                    else
                    {
                        if (startEvent)
                        {
                            StartMenuPressed -= callbackMethod;
                        }
                        else
                        {
                            StartMenuReleased -= callbackMethod;
                        }
                    }
                    break;
            }
        }

        private ControllerInteractionEventArgs SetButtonEvent(ref bool buttonBool, bool value, float buttonPressure)
        {
            var controllerIndex = VRTK_DeviceFinder.GetControllerIndex(gameObject);
            buttonBool = value;
            ControllerInteractionEventArgs e;
            e.controllerIndex = controllerIndex;
            e.buttonPressure = buttonPressure;
            e.touchpadAxis = VRTK_SDK_Bridge.GetTouchpadAxisOnIndex(controllerIndex);
            e.touchpadAngle = CalculateTouchpadAxisAngle(e.touchpadAxis);
            return e;
        }

        private void TrackedControllerEnabled(object sender, VRTKTrackedControllerEventArgs e)
        {
            var nullBool = false;
            OnControllerEnabled(SetButtonEvent(ref nullBool, true, 0f));
        }

        private void TrackedControllerDisabled(object sender, VRTKTrackedControllerEventArgs e)
        {
            DisableEvents();
            var nullBool = false;
            OnControllerDisabled(SetButtonEvent(ref nullBool, false, 0f));
        }

        private void TrackedControllerIndexChanged(object sender, VRTKTrackedControllerEventArgs e)
        {
            var nullBool = false;
            OnControllerIndexChanged(SetButtonEvent(ref nullBool, false, 0f));
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
            var distanceVector = vectorA - vectorB;
            return Math.Round(Mathf.Abs(distanceVector.x), axisFidelity, MidpointRounding.AwayFromZero) < float.Epsilon
                   && Math.Round(Mathf.Abs(distanceVector.y), axisFidelity, MidpointRounding.AwayFromZero) < float.Epsilon;
        }

        private void DisableEvents()
        {
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

            if (gripPressed)
            {
                OnGripReleased(SetButtonEvent(ref gripPressed, false, 0f));
                EmitAlias(ButtonAlias.Grip_Press, false, 0f, ref gripPressed);
            }

            if (gripTouched)
            {
                OnGripTouchEnd(SetButtonEvent(ref gripTouched, false, 0f));
                EmitAlias(ButtonAlias.Grip_Touch, false, 0f, ref gripTouched);
            }

            if (gripHairlinePressed)
            {
                OnGripHairlineEnd(SetButtonEvent(ref gripHairlinePressed, false, 0f));
                EmitAlias(ButtonAlias.Grip_Hairline, false, 0f, ref gripHairlinePressed);
            }

            if (gripClicked)
            {
                OnGripUnclicked(SetButtonEvent(ref gripClicked, false, 0f));
                EmitAlias(ButtonAlias.Grip_Click, false, 0f, ref gripClicked);
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

            if (buttonOnePressed)
            {
                OnButtonOneReleased(SetButtonEvent(ref buttonOnePressed, false, 0f));
                EmitAlias(ButtonAlias.Button_One_Press, false, 0f, ref buttonOnePressed);
            }

            if (buttonOneTouched)
            {
                OnButtonOneTouchEnd(SetButtonEvent(ref buttonOneTouched, false, 0f));
                EmitAlias(ButtonAlias.Button_One_Touch, false, 0f, ref buttonOneTouched);
            }

            if (buttonTwoPressed)
            {
                OnButtonTwoReleased(SetButtonEvent(ref buttonTwoPressed, false, 0f));
                EmitAlias(ButtonAlias.Button_Two_Press, false, 0f, ref buttonTwoPressed);
            }

            if (buttonTwoTouched)
            {
                OnButtonTwoTouchEnd(SetButtonEvent(ref buttonTwoTouched, false, 0f));
                EmitAlias(ButtonAlias.Button_Two_Touch, false, 0f, ref buttonTwoTouched);
            }

            if (startMenuPressed)
            {
                OnStartMenuReleased(SetButtonEvent(ref startMenuPressed, false, 0f));
                EmitAlias(ButtonAlias.Start_Menu_Press, false, 0f, ref startMenuPressed);
            }

            triggerAxisChanged = false;
            gripAxisChanged = false;
            touchpadAxisChanged = false;

            var controllerIndex = VRTK_DeviceFinder.GetControllerIndex(gameObject);

            if (controllerIndex < uint.MaxValue)
            {
                Vector2 currentTriggerAxis = VRTK_SDK_Bridge.GetTriggerAxisOnIndex(controllerIndex);
                Vector2 currentGripAxis = VRTK_SDK_Bridge.GetGripAxisOnIndex(controllerIndex);
                Vector2 currentTouchpadAxis = VRTK_SDK_Bridge.GetTouchpadAxisOnIndex(controllerIndex);

                // Save current touch and trigger settings to detect next change.
                touchpadAxis = new Vector2(currentTouchpadAxis.x, currentTouchpadAxis.y);
                triggerAxis = new Vector2(currentTriggerAxis.x, currentTriggerAxis.y);
                gripAxis = new Vector2(currentGripAxis.x, currentGripAxis.y);
                hairTriggerDelta = VRTK_SDK_Bridge.GetTriggerHairlineDeltaOnIndex(controllerIndex);
                hairGripDelta = VRTK_SDK_Bridge.GetGripHairlineDeltaOnIndex(controllerIndex);
            }
        }
    }
}
