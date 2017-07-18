// Controller Events|Interactions|30010
namespace VRTK
{
    using UnityEngine;
    using System;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controllerIndex">**OBSOLETE** The index of the controller that was used.</param>
    /// <param name="controllerReference">The reference for the controller that was used.</param>
    /// <param name="buttonPressure">The amount of pressure being applied to the button pressed. `0f` to `1f`.</param>
    /// <param name="touchpadAxis">The position the touchpad is touched at. `(0,0)` to `(1,1)`.</param>
    /// <param name="touchpadAngle">The rotational position the touchpad is being touched at, 0 being top, 180 being bottom and all other angles accordingly. `0f` to `360f`.</param>
    public struct ControllerInteractionEventArgs
    {
        [Obsolete("`ControllerInteractionEventArgs.controllerIndex` has been replaced with `ControllerInteractionEventArgs.controllerReference`. This parameter will be removed in a future version of VRTK.")]
        public uint controllerIndex;
        public VRTK_ControllerReference controllerReference;
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
    [AddComponentMenu("VRTK/Scripts/Interactions/VRTK_ControllerEvents")]
    public class VRTK_ControllerEvents : MonoBehaviour
    {
        /// <summary>
        /// Button types
        /// </summary>
        /// <param name="Undefined">No button specified</param>
        /// <param name="TriggerHairline">The trigger is squeezed past the current hairline threshold.</param>
        /// <param name="TriggerTouch">The trigger is squeezed a small amount.</param>
        /// <param name="TriggerPress">The trigger is squeezed about half way in.</param>
        /// <param name="TriggerClick">The trigger is squeezed all the way down.</param>
        /// <param name="GripHairline">The grip is squeezed past the current hairline threshold.</param>
        /// <param name="GripTouch">The grip button is touched.</param>
        /// <param name="GripPress">The grip button is pressed.</param>
        /// <param name="GripClick">The grip button is pressed all the way down.</param>
        /// <param name="TouchpadTouch">The touchpad is touched (without pressing down to click).</param>
        /// <param name="TouchpadPress">The touchpad is pressed (to the point of hearing a click).</param>
        /// <param name="ButtonOneTouch">The button one is touched.</param>
        /// <param name="ButtonOnePress">The button one is pressed.</param>
        /// <param name="ButtonTwoTouch">The button one is touched.</param>
        /// <param name="ButtonTwoPress">The button one is pressed.</param>
        /// <param name="StartMenuPress">The button one is pressed.</param>
        public enum ButtonAlias
        {
            Undefined,
            TriggerHairline,
            TriggerTouch,
            TriggerPress,
            TriggerClick,
            GripHairline,
            GripTouch,
            GripPress,
            GripClick,
            TouchpadTouch,
            TouchpadPress,
            ButtonOneTouch,
            ButtonOnePress,
            ButtonTwoTouch,
            ButtonTwoPress,
            StartMenuPress
        }

        [Header("Action Alias Buttons")]

        [Tooltip("**OBSOLETE [use VRTK_Pointer.activationButton]** The button to use for the action of turning a laser pointer on / off.")]
        [Obsolete("`VRTK_ControllerEvents.pointerToggleButton` is no longer used in the new `VRTK_Pointer` class, use `VRTK_Pointer.activationButton` instead. This parameter will be removed in a future version of VRTK.")]
        public ButtonAlias pointerToggleButton = ButtonAlias.TouchpadPress;
        [Tooltip("**OBSOLETE [use VRTK_Pointer.selectionButton]** The button to use for the action of setting a destination marker from the cursor position of the pointer.")]
        [Obsolete("`VRTK_ControllerEvents.pointerSetButton` is no longer used in the new `VRTK_Pointer` class, use `VRTK_Pointer.selectionButton` instead. This parameter will be removed in a future version of VRTK.")]
        public ButtonAlias pointerSetButton = ButtonAlias.TouchpadPress;
        [Tooltip("**OBSOLETE [use VRTK_InteractGrab.grabButton]** The button to use for the action of grabbing game objects.")]
        [Obsolete("`VRTK_ControllerEvents.grabToggleButton` is no longer used in the `VRTK_InteractGrab` class, use `VRTK_InteractGrab.grabButton` instead. This parameter will be removed in a future version of VRTK.")]
        public ButtonAlias grabToggleButton = ButtonAlias.GripPress;
        [Tooltip("**OBSOLETE [use VRTK_InteractUse.useButton]** The button to use for the action of using game objects.")]
        [Obsolete("`VRTK_ControllerEvents.useToggleButton` is no longer used in the `VRTK_InteractUse` class, use `VRTK_InteractUse.useButton` instead. This parameter will be removed in a future version of VRTK.")]
        public ButtonAlias useToggleButton = ButtonAlias.TriggerPress;
        [Tooltip("**OBSOLETE [use VRTK_UIPointer.selectionButton]** The button to use for the action of clicking a UI element.")]
        [Obsolete("`VRTK_ControllerEvents.uiClickButton` is no longer used in the `VRTK_UIPointer` class, use `VRTK_UIPointer.selectionButton` instead. This parameter will be removed in a future version of VRTK.")]
        public ButtonAlias uiClickButton = ButtonAlias.TriggerPress;
        [Tooltip("**OBSOLETE [use VRTK_ControllerEvents.buttonTwoPressed]** The button to use for the action of bringing up an in-game menu.")]
        [Obsolete("`VRTK_ControllerEvents.menuToggleButton` is no longer used, use `VRTK_ControllerEvents.buttonTwoPressed` instead. This parameter will be removed in a future version of VRTK.")]
        public ButtonAlias menuToggleButton = ButtonAlias.ButtonTwoPress;

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
        [Obsolete("`VRTK_ControllerEvents.pointerPressed` is no longer used, use `VRTK_Pointer.IsActivationButtonPressed()` instead. This parameter will be removed in a future version of VRTK.")]
        public bool pointerPressed = false;
        /// <summary>
        /// This will be true if the button aliased to the grab is held down.
        /// </summary>
        [HideInInspector]
        [Obsolete("`VRTK_ControllerEvents.grabPressed` is no longer used, use `VRTK_InteractGrab.IsGrabButtonPressed()` instead. This parameter will be removed in a future version of VRTK.")]
        public bool grabPressed = false;
        /// <summary>
        /// This will be true if the button aliased to the use is held down.
        /// </summary>
        [HideInInspector]
        [Obsolete("`VRTK_ControllerEvents.usePressed` is no longer used, use `VRTK_InteractUse.IsUseButtonPressed()` instead. This parameter will be removed in a future version of VRTK.")]
        public bool usePressed = false;
        /// <summary>
        /// This will be true if the button aliased to the UI click is held down.
        /// </summary>
        [HideInInspector]
        [Obsolete("`VRTK_ControllerEvents.uiClickPressed` is no longer used, use `VRTK_UIPointer.IsSelectionButtonPressed()` instead. This parameter will be removed in a future version of VRTK.")]
        public bool uiClickPressed = false;
        /// <summary>
        /// This will be true if the button aliased to the menu is held down.
        /// </summary>
        [HideInInspector]
        [Obsolete("`VRTK_ControllerEvents.menuPressed` is no longer used, use `VRTK_ControllerEvents.buttonTwoPressed` instead. This parameter will be removed in a future version of VRTK.")]
        public bool menuPressed = false;

        /// <summary>
        /// This will be true if the controller model alias renderers are visible.
        /// </summary>
        [HideInInspector]
        public bool controllerVisible = true;

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
        [Obsolete("`VRTK_ControllerEvents.AliasPointerOn` has been replaced with `VRTK_Pointer.ActivationButtonPressed`. This parameter will be removed in a future version of VRTK.")]
        public event ControllerInteractionEventHandler AliasPointerOn;
        /// <summary>
        /// Emitted when the pointer toggle alias button is released.
        /// </summary>
        [Obsolete("`VRTK_ControllerEvents.AliasPointerOff` has been replaced with `VRTK_Pointer.ActivationButtonReleased`. This parameter will be removed in a future version of VRTK.")]
        public event ControllerInteractionEventHandler AliasPointerOff;
        /// <summary>
        /// Emitted when the pointer set alias button is released.
        /// </summary>
        [Obsolete("`VRTK_ControllerEvents.AliasPointerSet` has been replaced with `VRTK_Pointer.SelectionButtonReleased`. This parameter will be removed in a future version of VRTK.")]
        public event ControllerInteractionEventHandler AliasPointerSet;

        /// <summary>
        /// Emitted when the grab toggle alias button is pressed.
        /// </summary>
        [Obsolete("`VRTK_ControllerEvents.AliasGrabOn` has been replaced with `VRTK_InteractGrab.GrabButtonPressed`. This parameter will be removed in a future version of VRTK.")]
        public event ControllerInteractionEventHandler AliasGrabOn;
        /// <summary>
        /// Emitted when the grab toggle alias button is released.
        /// </summary>
        [Obsolete("`VRTK_ControllerEvents.AliasGrabOff` has been replaced with `VRTK_InteractGrab.GrabButtonReleased`. This parameter will be removed in a future version of VRTK.")]
        public event ControllerInteractionEventHandler AliasGrabOff;

        /// <summary>
        /// Emitted when the use toggle alias button is pressed.
        /// </summary>
        [Obsolete("`VRTK_ControllerEvents.AliasUseOn` has been replaced with `VRTK_InteractUse.UseButtonPressed`. This parameter will be removed in a future version of VRTK.")]
        public event ControllerInteractionEventHandler AliasUseOn;
        /// <summary>
        /// Emitted when the use toggle alias button is released.
        /// </summary>
        [Obsolete("`VRTK_ControllerEvents.AliasUseOff` has been replaced with `VRTK_InteractUse.UseButtonReleased`. This parameter will be removed in a future version of VRTK.")]
        public event ControllerInteractionEventHandler AliasUseOff;

        /// <summary>
        /// Emitted when the menu toggle alias button is pressed.
        /// </summary>
        [Obsolete("`VRTK_ControllerEvents.AliasMenuOn` is no longer used, use `VRTK_ControllerEvents.ButtonTwoPressed` instead. This parameter will be removed in a future version of VRTK.")]
        public event ControllerInteractionEventHandler AliasMenuOn;
        /// <summary>
        /// Emitted when the menu toggle alias button is released.
        /// </summary>
        [Obsolete("`VRTK_ControllerEvents.AliasMenuOff` is no longer used, use `VRTK_ControllerEvents.ButtonTwoReleased` instead. This parameter will be removed in a future version of VRTK.")]
        public event ControllerInteractionEventHandler AliasMenuOff;

        /// <summary>
        /// Emitted when the UI click alias button is pressed.
        /// </summary>
        [Obsolete("`VRTK_ControllerEvents.AliasUIClickOn` has been replaced with `VRTK_UIPointer.SelectionButtonPressed`. This parameter will be removed in a future version of VRTK.")]
        public event ControllerInteractionEventHandler AliasUIClickOn;
        /// <summary>
        /// Emitted when the UI click alias button is released.
        /// </summary>
        [Obsolete("`VRTK_ControllerEvents.AliasUIClickOff` has been replaced with `VRTK_UIPointer.SelectionButtonReleased`. This parameter will be removed in a future version of VRTK.")]
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

        /// <summary>
        /// Emitted when the controller is set to visible.
        /// </summary>
        public event ControllerInteractionEventHandler ControllerVisible;
        /// <summary>
        /// Emitted when the controller is set to hidden.
        /// </summary>
        public event ControllerInteractionEventHandler ControllerHidden;

        protected Vector2 touchpadAxis = Vector2.zero;
        protected Vector2 triggerAxis = Vector2.zero;
        protected Vector2 gripAxis = Vector2.zero;
        protected float hairTriggerDelta;
        protected float hairGripDelta;

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

        [Obsolete("`VRTK_ControllerEvents.OnAliasPointerOn` has been replaced with `VRTK_Pointer.OnActivationButtonPressed`. This method will be removed in a future version of VRTK.")]
        public virtual void OnAliasPointerOn(ControllerInteractionEventArgs e)
        {
            if (AliasPointerOn != null)
            {
                AliasPointerOn(this, e);
            }
        }

        [Obsolete("`VRTK_ControllerEvents.OnAliasPointerOff` has been replaced with `VRTK_Pointer.OnActivationButtonReleased`. This method will be removed in a future version of VRTK.")]
        public virtual void OnAliasPointerOff(ControllerInteractionEventArgs e)
        {
            if (AliasPointerOff != null)
            {
                AliasPointerOff(this, e);
            }
        }

        [Obsolete("`VRTK_ControllerEvents.OnAliasPointerSet` has been replaced with `VRTK_Pointer.OnSelectionButtonReleased`. This method will be removed in a future version of VRTK.")]
        public virtual void OnAliasPointerSet(ControllerInteractionEventArgs e)
        {
            if (AliasPointerSet != null)
            {
                AliasPointerSet(this, e);
            }
        }

        [Obsolete("`VRTK_ControllerEvents.OnAliasGrabOn` has been replaced with `VRTK_InteractGrab.OnGrabButtonPressed`. This method will be removed in a future version of VRTK.")]
        public virtual void OnAliasGrabOn(ControllerInteractionEventArgs e)
        {
            if (AliasGrabOn != null)
            {
                AliasGrabOn(this, e);
            }
        }

        [Obsolete("`VRTK_ControllerEvents.OnAliasGrabOff` has been replaced with `VRTK_InteractGrab.OnGrabButtonReleased`. This method will be removed in a future version of VRTK.")]
        public virtual void OnAliasGrabOff(ControllerInteractionEventArgs e)
        {
            if (AliasGrabOff != null)
            {
                AliasGrabOff(this, e);
            }
        }

        [Obsolete("`VRTK_ControllerEvents.OnAliasUseOn` has been replaced with `VRTK_InteractUse.OnUseButtonPressed`. This method will be removed in a future version of VRTK.")]
        public virtual void OnAliasUseOn(ControllerInteractionEventArgs e)
        {
            if (AliasUseOn != null)
            {
                AliasUseOn(this, e);
            }
        }

        [Obsolete("`VRTK_ControllerEvents.OnAliasUseOff` has been replaced with `VRTK_InteractUse.OnUseButtonReleased`. This method will be removed in a future version of VRTK.")]
        public virtual void OnAliasUseOff(ControllerInteractionEventArgs e)
        {
            if (AliasUseOff != null)
            {
                AliasUseOff(this, e);
            }
        }

        [Obsolete("`VRTK_ControllerEvents.OnAliasUIClickOn` has been replaced with `VRTK_UIPointer.OnSelectionButtonPressed`. This method will be removed in a future version of VRTK.")]
        public virtual void OnAliasUIClickOn(ControllerInteractionEventArgs e)
        {
            if (AliasUIClickOn != null)
            {
                AliasUIClickOn(this, e);
            }
        }

        [Obsolete("`VRTK_ControllerEvents.OnAliasUIClickOff` has been replaced with `VRTK_UIPointer.OnSelectionButtonReleased`. This method will be removed in a future version of VRTK.")]
        public virtual void OnAliasUIClickOff(ControllerInteractionEventArgs e)
        {
            if (AliasUIClickOff != null)
            {
                AliasUIClickOff(this, e);
            }
        }

        [Obsolete("`VRTK_ControllerEvents.OnAliasMenuOn` has been replaced with `VRTK_ControllerEvents.OnButtonTwoPressed`. This method will be removed in a future version of VRTK.")]
        public virtual void OnAliasMenuOn(ControllerInteractionEventArgs e)
        {
            if (AliasMenuOn != null)
            {
                AliasMenuOn(this, e);
            }
        }

        [Obsolete("`VRTK_ControllerEvents.OnAliasMenuOff` has been replaced with `VRTK_ControllerEvents.OnButtonTwoReleased`. This method will be removed in a future version of VRTK.")]
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

        public virtual void OnControllerVisible(ControllerInteractionEventArgs e)
        {
            controllerVisible = true;
            if (ControllerVisible != null)
            {
                ControllerVisible(this, e);
            }
        }

        public virtual void OnControllerHidden(ControllerInteractionEventArgs e)
        {
            controllerVisible = false;
            if (ControllerHidden != null)
            {
                ControllerHidden(this, e);
            }
        }

        /// <summary>
        /// The SetControllerEvent/0 method is used to set the Controller Event payload.
        /// </summary>
        /// <returns>The payload for a Controller Event.</returns>
        public virtual ControllerInteractionEventArgs SetControllerEvent()
        {
            var nullBool = false;
            return SetControllerEvent(ref nullBool);
        }

        /// <summary>
        /// The SetControllerEvent/3 method is used to set the Controller Event payload.
        /// </summary>
        /// <param name="buttonBool">The state of the pressed button if required.</param>
        /// <param name="value">The value to set the buttonBool reference to.</param>
        /// <param name="buttonPressure">The pressure of the button pressed if required.</param>
        /// <returns>The payload for a Controller Event.</returns>
        public virtual ControllerInteractionEventArgs SetControllerEvent(ref bool buttonBool, bool value = false, float buttonPressure = 0f)
        {
            VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(gameObject);
            buttonBool = value;
            ControllerInteractionEventArgs e;
#pragma warning disable 0618
            e.controllerIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
#pragma warning restore 0618
            e.controllerReference = controllerReference;
            e.buttonPressure = buttonPressure;
            e.touchpadAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Touchpad, controllerReference);
            e.touchpadAngle = CalculateTouchpadAxisAngle(e.touchpadAxis);
            return e;
        }

        /// <summary>
        /// The GetVelocity method is useful for getting the current velocity of the physical game controller. This can be useful to determine the speed at which the controller is being swung or the direction it is being moved in.
        /// </summary>
        /// <returns>A 3 dimensional vector containing the current real world physical controller velocity.</returns>
        [Obsolete("`VRTK_ControllerEvents.GetVelocity()` has been replaced with `VRTK_DeviceFinder.GetControllerVelocity(givenController)`. This method will be removed in a future version of VRTK.")]
        public virtual Vector3 GetVelocity()
        {
            return VRTK_DeviceFinder.GetControllerVelocity(gameObject);
        }

        /// <summary>
        /// The GetAngularVelocity method is useful for getting the current rotational velocity of the physical game controller. This can be useful for determining which way the controller is being rotated and at what speed the rotation is occurring.
        /// </summary>
        /// <returns>A 3 dimensional vector containing the current real world physical controller angular (rotational) velocity.</returns>
        [Obsolete("`VRTK_ControllerEvents.GetAngularVelocity()` has been replaced with `VRTK_DeviceFinder.GetControllerAngularVelocity(givenController)`. This method will be removed in a future version of VRTK.")]
        public virtual Vector3 GetAngularVelocity()
        {
            return VRTK_DeviceFinder.GetControllerAngularVelocity(gameObject);
        }

        /// <summary>
        /// The GetTouchpadAxis method returns the coordinates of where the touchpad is being touched and can be used for directional input via the touchpad. The `x` value is the horizontal touch plane and the `y` value is the vertical touch plane.
        /// </summary>
        /// <returns>A 2 dimensional vector containing the x and y position of where the touchpad is being touched. `(0,0)` to `(1,1)`.</returns>
        public virtual Vector2 GetTouchpadAxis()
        {
            return touchpadAxis;
        }

        /// <summary>
        /// The GetTouchpadAxisAngle method returns the angle of where the touchpad is currently being touched with the top of the touchpad being 0 degrees and the bottom of the touchpad being 180 degrees.
        /// </summary>
        /// <returns>A float representing the angle of where the touchpad is being touched. `0f` to `360f`.</returns>
        public virtual float GetTouchpadAxisAngle()
        {
            return CalculateTouchpadAxisAngle(touchpadAxis);
        }

        /// <summary>
        /// The GetTriggerAxis method returns a float that represents how much the trigger is being squeezed. This can be useful for using the trigger axis to perform high fidelity tasks or only activating the trigger press once it has exceeded a given press threshold.
        /// </summary>
        /// <returns>A float representing the amount of squeeze that is being applied to the trigger. `0f` to `1f`.</returns>
        public virtual float GetTriggerAxis()
        {
            return triggerAxis.x;
        }

        /// <summary>
        /// The GetGripAxis method returns a float that represents how much the grip is being squeezed. This can be useful for using the grip axis to perform high fidelity tasks or only activating the grip press once it has exceeded a given press threshold.
        /// </summary>
        /// <returns>A float representing the amount of squeeze that is being applied to the grip. `0f` to `1f`.</returns>
        public virtual float GetGripAxis()
        {
            return gripAxis.x;
        }

        /// <summary>
        /// The GetHairTriggerDelta method returns a float representing the difference in how much the trigger is being pressed in relation to the hairline threshold start.
        /// </summary>
        /// <returns>A float representing the difference in the trigger pressure from the hairline threshold start to current position.</returns>
        public virtual float GetHairTriggerDelta()
        {
            return hairTriggerDelta;
        }

        /// <summary>
        /// The GetHairTriggerDelta method returns a float representing the difference in how much the trigger is being pressed in relation to the hairline threshold start.
        /// </summary>
        /// <returns>A float representing the difference in the trigger pressure from the hairline threshold start to current position.</returns>
        public virtual float GetHairGripDelta()
        {
            return hairGripDelta;
        }

        /// <summary>
        /// The AnyButtonPressed method returns true if any of the controller buttons are being pressed and this can be useful to determine if an action can be taken whilst the user is using the controller.
        /// </summary>
        /// <returns>Is true if any of the controller buttons are currently being pressed.</returns>
        public virtual bool AnyButtonPressed()
        {
            return (triggerPressed || gripPressed || touchpadPressed || buttonOnePressed || buttonTwoPressed || startMenuPressed);
        }

        /// <summary>
        /// The IsButtonPressed method takes a given button alias and returns a boolean whether that given button is currently being pressed or not.
        /// </summary>
        /// <param name="button">The button to check if it's being pressed.</param>
        /// <returns>Is true if the button is being pressed.</returns>
        public virtual bool IsButtonPressed(ButtonAlias button)
        {
            switch (button)
            {
                case ButtonAlias.TriggerHairline:
                    return triggerHairlinePressed;
                case ButtonAlias.TriggerTouch:
                    return triggerTouched;
                case ButtonAlias.TriggerPress:
                    return triggerPressed;
                case ButtonAlias.TriggerClick:
                    return triggerClicked;
                case ButtonAlias.GripHairline:
                    return gripHairlinePressed;
                case ButtonAlias.GripTouch:
                    return gripTouched;
                case ButtonAlias.GripPress:
                    return gripPressed;
                case ButtonAlias.GripClick:
                    return gripClicked;
                case ButtonAlias.TouchpadTouch:
                    return touchpadTouched;
                case ButtonAlias.TouchpadPress:
                    return touchpadPressed;
                case ButtonAlias.ButtonOnePress:
                    return buttonOnePressed;
                case ButtonAlias.ButtonOneTouch:
                    return buttonOneTouched;
                case ButtonAlias.ButtonTwoPress:
                    return buttonTwoPressed;
                case ButtonAlias.ButtonTwoTouch:
                    return buttonTwoTouched;
                case ButtonAlias.StartMenuPress:
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
        public virtual void SubscribeToButtonAliasEvent(ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)
        {
            ButtonAliasEventSubscription(true, givenButton, startEvent, callbackMethod);
        }

        /// <summary>
        /// The UnsubscribeToButtonAliasEvent method makes it easier to unsubscribe to from button event on either the start or end action.
        /// </summary>
        /// <param name="givenButton">The ButtonAlias to unregister the event on.</param>
        /// <param name="startEvent">If this is `true` then the start event related to the button is used (e.g. OnPress). If this is `false` then the end event related to the button is used (e.g. OnRelease). </param>
        /// <param name="callbackMethod">The method to unsubscribe from the event.</param>
        public virtual void UnsubscribeToButtonAliasEvent(ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)
        {
            ButtonAliasEventSubscription(false, givenButton, startEvent, callbackMethod);
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
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

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Update()
        {
            VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(gameObject);

            //Only continue if the controller reference is valid
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return;
            }

            Vector2 currentTriggerAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Trigger, controllerReference);
            Vector2 currentGripAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Grip, controllerReference);
            Vector2 currentTouchpadAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Touchpad, controllerReference);

            //Trigger Touched
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.TouchDown, controllerReference))
            {
                OnTriggerTouchStart(SetControllerEvent(ref triggerTouched, true, currentTriggerAxis.x));
                EmitAlias(ButtonAlias.TriggerTouch, true, currentTriggerAxis.x, ref triggerTouched);
            }

            //Trigger Hairline
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.TriggerHairline, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
            {
                OnTriggerHairlineStart(SetControllerEvent(ref triggerHairlinePressed, true, currentTriggerAxis.x));
                EmitAlias(ButtonAlias.TriggerHairline, true, currentTriggerAxis.x, ref triggerHairlinePressed);
            }

            //Trigger Pressed
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
            {
                OnTriggerPressed(SetControllerEvent(ref triggerPressed, true, currentTriggerAxis.x));
                EmitAlias(ButtonAlias.TriggerPress, true, currentTriggerAxis.x, ref triggerPressed);
            }

            //Trigger Clicked
            if (!triggerClicked && currentTriggerAxis.x >= triggerClickThreshold)
            {
                OnTriggerClicked(SetControllerEvent(ref triggerClicked, true, currentTriggerAxis.x));
                EmitAlias(ButtonAlias.TriggerClick, true, currentTriggerAxis.x, ref triggerClicked);
            }
            else if (triggerClicked && currentTriggerAxis.x < triggerClickThreshold)
            {
                OnTriggerUnclicked(SetControllerEvent(ref triggerClicked, false, 0f));
                EmitAlias(ButtonAlias.TriggerClick, false, 0f, ref triggerClicked);
            }

            // Trigger Pressed end
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
                OnTriggerReleased(SetControllerEvent(ref triggerPressed, false, 0f));
                EmitAlias(ButtonAlias.TriggerPress, false, 0f, ref triggerPressed);
            }

            //Trigger Hairline End
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.TriggerHairline, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
                OnTriggerHairlineEnd(SetControllerEvent(ref triggerHairlinePressed, false, 0f));
                EmitAlias(ButtonAlias.TriggerHairline, false, 0f, ref triggerHairlinePressed);
            }

            //Trigger Touch End
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.TouchUp, controllerReference))
            {
                OnTriggerTouchEnd(SetControllerEvent(ref triggerTouched, false, 0f));
                EmitAlias(ButtonAlias.TriggerTouch, false, 0f, ref triggerTouched);
            }

            //Trigger Axis
            currentTriggerAxis.x = ((!triggerTouched && triggerAxisZeroOnUntouch) || currentTriggerAxis.x < triggerForceZeroThreshold ? 0f : currentTriggerAxis.x);
            if (VRTK_SharedMethods.Vector2ShallowCompare(triggerAxis, currentTriggerAxis, axisFidelity))
            {
                triggerAxisChanged = false;
            }
            else
            {
                OnTriggerAxisChanged(SetControllerEvent(ref triggerAxisChanged, true, currentTriggerAxis.x));
            }

            //Grip Touched
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.TouchDown, controllerReference))
            {
                OnGripTouchStart(SetControllerEvent(ref gripTouched, true, currentGripAxis.x));
                EmitAlias(ButtonAlias.GripTouch, true, currentGripAxis.x, ref gripTouched);
            }

            //Grip Hairline
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.GripHairline, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
            {
                OnGripHairlineStart(SetControllerEvent(ref gripHairlinePressed, true, currentGripAxis.x));
                EmitAlias(ButtonAlias.GripHairline, true, currentGripAxis.x, ref gripHairlinePressed);
            }

            //Grip Pressed
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
            {
                OnGripPressed(SetControllerEvent(ref gripPressed, true, currentGripAxis.x));
                EmitAlias(ButtonAlias.GripPress, true, currentGripAxis.x, ref gripPressed);
            }

            //Grip Clicked
            if (!gripClicked && currentGripAxis.x >= gripClickThreshold)
            {
                OnGripClicked(SetControllerEvent(ref gripClicked, true, currentGripAxis.x));
                EmitAlias(ButtonAlias.GripClick, true, currentGripAxis.x, ref gripClicked);
            }
            else if (gripClicked && currentGripAxis.x < gripClickThreshold)
            {
                OnGripUnclicked(SetControllerEvent(ref gripClicked, false, 0f));
                EmitAlias(ButtonAlias.GripClick, false, 0f, ref gripClicked);
            }

            // Grip Pressed End
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
                OnGripReleased(SetControllerEvent(ref gripPressed, false, 0f));
                EmitAlias(ButtonAlias.GripPress, false, 0f, ref gripPressed);
            }

            //Grip Hairline End
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.GripHairline, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
                OnGripHairlineEnd(SetControllerEvent(ref gripHairlinePressed, false, 0f));
                EmitAlias(ButtonAlias.GripHairline, false, 0f, ref gripHairlinePressed);
            }

            // Grip Touch End
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.TouchUp, controllerReference))
            {
                OnGripTouchEnd(SetControllerEvent(ref gripTouched, false, 0f));
                EmitAlias(ButtonAlias.GripTouch, false, 0f, ref gripTouched);
            }

            //Grip Axis
            currentGripAxis.x = ((!gripTouched && gripAxisZeroOnUntouch) || currentGripAxis.x < gripForceZeroThreshold ? 0f : currentGripAxis.x);
            if (VRTK_SharedMethods.Vector2ShallowCompare(gripAxis, currentGripAxis, axisFidelity))
            {
                gripAxisChanged = false;
            }
            else
            {
                OnGripAxisChanged(SetControllerEvent(ref gripAxisChanged, true, currentGripAxis.x));
            }

            //Touchpad Touched
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.TouchDown, controllerReference))
            {
                OnTouchpadTouchStart(SetControllerEvent(ref touchpadTouched, true, 1f));
                EmitAlias(ButtonAlias.TouchpadTouch, true, 1f, ref touchpadTouched);
            }

            //Touchpad Pressed
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
            {
                OnTouchpadPressed(SetControllerEvent(ref touchpadPressed, true, 1f));
                EmitAlias(ButtonAlias.TouchpadPress, true, 1f, ref touchpadPressed);
            }
            else if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
                OnTouchpadReleased(SetControllerEvent(ref touchpadPressed, false, 0f));
                EmitAlias(ButtonAlias.TouchpadPress, false, 0f, ref touchpadPressed);
            }

            //Touchpad Untouched
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.TouchUp, controllerReference))
            {
                OnTouchpadTouchEnd(SetControllerEvent(ref touchpadTouched, false, 0f));
                EmitAlias(ButtonAlias.TouchpadTouch, false, 0f, ref touchpadTouched);
                touchpadAxis = Vector2.zero;
            }

            //Touchpad Axis
            if (VRTK_SDK_Bridge.IsTouchpadStatic(touchpadTouched, touchpadAxis, currentTouchpadAxis, axisFidelity))
            {
                touchpadAxisChanged = false;
            }
            else
            {
                OnTouchpadAxisChanged(SetControllerEvent(ref touchpadAxisChanged, true, 1f));
            }

            //ButtonOne Touched
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.TouchDown, controllerReference))
            {
                OnButtonOneTouchStart(SetControllerEvent(ref buttonOneTouched, true, 1f));
                EmitAlias(ButtonAlias.ButtonOneTouch, true, 1f, ref buttonOneTouched);
            }

            //ButtonOne Pressed
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
            {
                OnButtonOnePressed(SetControllerEvent(ref buttonOnePressed, true, 1f));
                EmitAlias(ButtonAlias.ButtonOnePress, true, 1f, ref buttonOnePressed);
            }
            else if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
                OnButtonOneReleased(SetControllerEvent(ref buttonOnePressed, false, 0f));
                EmitAlias(ButtonAlias.ButtonOnePress, false, 0f, ref buttonOnePressed);
            }

            //ButtonOne Touched End
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.TouchUp, controllerReference))
            {
                OnButtonOneTouchEnd(SetControllerEvent(ref buttonOneTouched, false, 0f));
                EmitAlias(ButtonAlias.ButtonOneTouch, false, 0f, ref buttonOneTouched);
            }

            //ButtonTwo Touched
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.TouchDown, controllerReference))
            {
                OnButtonTwoTouchStart(SetControllerEvent(ref buttonTwoTouched, true, 1f));
                EmitAlias(ButtonAlias.ButtonTwoTouch, true, 1f, ref buttonTwoTouched);
            }

            //ButtonTwo Pressed
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
            {
                OnButtonTwoPressed(SetControllerEvent(ref buttonTwoPressed, true, 1f));
                EmitAlias(ButtonAlias.ButtonTwoPress, true, 1f, ref buttonTwoPressed);
            }
            else if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
                OnButtonTwoReleased(SetControllerEvent(ref buttonTwoPressed, false, 0f));
                EmitAlias(ButtonAlias.ButtonTwoPress, false, 0f, ref buttonTwoPressed);
            }

            //ButtonTwo Touched End
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.TouchUp, controllerReference))
            {
                OnButtonTwoTouchEnd(SetControllerEvent(ref buttonTwoTouched, false, 0f));
                EmitAlias(ButtonAlias.ButtonTwoTouch, false, 0f, ref buttonTwoTouched);
            }

            //StartMenu Pressed
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.StartMenu, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
            {
                OnStartMenuPressed(SetControllerEvent(ref startMenuPressed, true, 1f));
                EmitAlias(ButtonAlias.StartMenuPress, true, 1f, ref startMenuPressed);
            }
            else if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.StartMenu, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
                OnStartMenuReleased(SetControllerEvent(ref startMenuPressed, false, 0f));
                EmitAlias(ButtonAlias.StartMenuPress, false, 0f, ref startMenuPressed);
            }

            // Save current touch and trigger settings to detect next change.
            touchpadAxis = (touchpadAxisChanged ? new Vector2(currentTouchpadAxis.x, currentTouchpadAxis.y) : touchpadAxis);
            triggerAxis = (triggerAxisChanged ? new Vector2(currentTriggerAxis.x, currentTriggerAxis.y) : triggerAxis);
            gripAxis = (gripAxisChanged ? new Vector2(currentGripAxis.x, currentGripAxis.y) : gripAxis);

            hairTriggerDelta = VRTK_SDK_Bridge.GetControllerHairlineDelta(SDK_BaseController.ButtonTypes.TriggerHairline, controllerReference);
            hairGripDelta = VRTK_SDK_Bridge.GetControllerHairlineDelta(SDK_BaseController.ButtonTypes.GripHairline, controllerReference);
        }

        protected virtual void ButtonAliasEventSubscription(bool subscribe, ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)
        {
            switch (givenButton)
            {
                case ButtonAlias.TriggerClick:
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
                case ButtonAlias.TriggerHairline:
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
                case ButtonAlias.TriggerPress:
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
                case ButtonAlias.TriggerTouch:
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
                case ButtonAlias.GripClick:
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
                case ButtonAlias.GripHairline:
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
                case ButtonAlias.GripPress:
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
                case ButtonAlias.GripTouch:
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
                case ButtonAlias.TouchpadPress:
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
                case ButtonAlias.TouchpadTouch:
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
                case ButtonAlias.ButtonOnePress:
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
                case ButtonAlias.ButtonOneTouch:
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
                case ButtonAlias.ButtonTwoPress:
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
                case ButtonAlias.ButtonTwoTouch:
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
                case ButtonAlias.StartMenuPress:
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

        protected virtual void TrackedControllerEnabled(object sender, VRTKTrackedControllerEventArgs e)
        {
            OnControllerEnabled(SetControllerEvent());
        }

        protected virtual void TrackedControllerDisabled(object sender, VRTKTrackedControllerEventArgs e)
        {
            DisableEvents();
            OnControllerDisabled(SetControllerEvent());
        }

        protected virtual void TrackedControllerIndexChanged(object sender, VRTKTrackedControllerEventArgs e)
        {
            OnControllerIndexChanged(SetControllerEvent());
        }

        protected virtual float CalculateTouchpadAxisAngle(Vector2 axis)
        {
            float angle = Mathf.Atan2(axis.y, axis.x) * Mathf.Rad2Deg;
            angle = 90.0f - angle;
            if (angle < 0)
            {
                angle += 360.0f;
            }
            return angle;
        }

#pragma warning disable 0618
        /// <obsolete>
        /// This is an obsolete method that will be removed in a future version
        /// </obsolete>
        protected virtual void EmitAlias(ButtonAlias type, bool touchDown, float buttonPressure, ref bool buttonBool)
        {
            if (pointerToggleButton == type)
            {
                if (touchDown)
                {
                    pointerPressed = true;
                    OnAliasPointerOn(SetControllerEvent(ref buttonBool, true, buttonPressure));
                }
                else
                {
                    pointerPressed = false;
                    OnAliasPointerOff(SetControllerEvent(ref buttonBool, false, buttonPressure));
                }
            }

            if (pointerSetButton == type)
            {
                if (!touchDown)
                {
                    OnAliasPointerSet(SetControllerEvent(ref buttonBool, false, buttonPressure));
                }
            }

            if (grabToggleButton == type)
            {
                if (touchDown)
                {
                    grabPressed = true;
                    OnAliasGrabOn(SetControllerEvent(ref buttonBool, true, buttonPressure));
                }
                else
                {
                    grabPressed = false;
                    OnAliasGrabOff(SetControllerEvent(ref buttonBool, false, buttonPressure));
                }
            }

            if (useToggleButton == type)
            {
                if (touchDown)
                {
                    usePressed = true;
                    OnAliasUseOn(SetControllerEvent(ref buttonBool, true, buttonPressure));
                }
                else
                {
                    usePressed = false;
                    OnAliasUseOff(SetControllerEvent(ref buttonBool, false, buttonPressure));
                }
            }

            if (uiClickButton == type)
            {
                if (touchDown)
                {
                    uiClickPressed = true;
                    OnAliasUIClickOn(SetControllerEvent(ref buttonBool, true, buttonPressure));
                }
                else
                {
                    uiClickPressed = false;
                    OnAliasUIClickOff(SetControllerEvent(ref buttonBool, false, buttonPressure));
                }
            }

            if (menuToggleButton == type)
            {
                if (touchDown)
                {
                    menuPressed = true;
                    OnAliasMenuOn(SetControllerEvent(ref buttonBool, true, buttonPressure));
                }
                else
                {
                    menuPressed = false;
                    OnAliasMenuOff(SetControllerEvent(ref buttonBool, false, buttonPressure));
                }
            }
        }
#pragma warning restore 0618

        protected virtual void DisableEvents()
        {
            if (triggerPressed)
            {
                OnTriggerReleased(SetControllerEvent(ref triggerPressed, false, 0f));
                EmitAlias(ButtonAlias.TriggerPress, false, 0f, ref triggerPressed);
            }

            if (triggerTouched)
            {
                OnTriggerTouchEnd(SetControllerEvent(ref triggerTouched, false, 0f));
                EmitAlias(ButtonAlias.TriggerTouch, false, 0f, ref triggerTouched);
            }

            if (triggerHairlinePressed)
            {
                OnTriggerHairlineEnd(SetControllerEvent(ref triggerHairlinePressed, false, 0f));
                EmitAlias(ButtonAlias.TriggerHairline, false, 0f, ref triggerHairlinePressed);
            }

            if (triggerClicked)
            {
                OnTriggerUnclicked(SetControllerEvent(ref triggerClicked, false, 0f));
                EmitAlias(ButtonAlias.TriggerClick, false, 0f, ref triggerClicked);
            }

            if (gripPressed)
            {
                OnGripReleased(SetControllerEvent(ref gripPressed, false, 0f));
                EmitAlias(ButtonAlias.GripPress, false, 0f, ref gripPressed);
            }

            if (gripTouched)
            {
                OnGripTouchEnd(SetControllerEvent(ref gripTouched, false, 0f));
                EmitAlias(ButtonAlias.GripTouch, false, 0f, ref gripTouched);
            }

            if (gripHairlinePressed)
            {
                OnGripHairlineEnd(SetControllerEvent(ref gripHairlinePressed, false, 0f));
                EmitAlias(ButtonAlias.GripHairline, false, 0f, ref gripHairlinePressed);
            }

            if (gripClicked)
            {
                OnGripUnclicked(SetControllerEvent(ref gripClicked, false, 0f));
                EmitAlias(ButtonAlias.GripClick, false, 0f, ref gripClicked);
            }

            if (touchpadPressed)
            {
                OnTouchpadReleased(SetControllerEvent(ref touchpadPressed, false, 0f));
                EmitAlias(ButtonAlias.TouchpadPress, false, 0f, ref touchpadPressed);
            }

            if (touchpadTouched)
            {
                OnTouchpadTouchEnd(SetControllerEvent(ref touchpadTouched, false, 0f));
                EmitAlias(ButtonAlias.TouchpadTouch, false, 0f, ref touchpadTouched);
            }

            if (buttonOnePressed)
            {
                OnButtonOneReleased(SetControllerEvent(ref buttonOnePressed, false, 0f));
                EmitAlias(ButtonAlias.ButtonOnePress, false, 0f, ref buttonOnePressed);
            }

            if (buttonOneTouched)
            {
                OnButtonOneTouchEnd(SetControllerEvent(ref buttonOneTouched, false, 0f));
                EmitAlias(ButtonAlias.ButtonOneTouch, false, 0f, ref buttonOneTouched);
            }

            if (buttonTwoPressed)
            {
                OnButtonTwoReleased(SetControllerEvent(ref buttonTwoPressed, false, 0f));
                EmitAlias(ButtonAlias.ButtonTwoPress, false, 0f, ref buttonTwoPressed);
            }

            if (buttonTwoTouched)
            {
                OnButtonTwoTouchEnd(SetControllerEvent(ref buttonTwoTouched, false, 0f));
                EmitAlias(ButtonAlias.ButtonTwoTouch, false, 0f, ref buttonTwoTouched);
            }

            if (startMenuPressed)
            {
                OnStartMenuReleased(SetControllerEvent(ref startMenuPressed, false, 0f));
                EmitAlias(ButtonAlias.StartMenuPress, false, 0f, ref startMenuPressed);
            }

            triggerAxisChanged = false;
            gripAxisChanged = false;
            touchpadAxisChanged = false;

            VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(gameObject);

            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                Vector2 currentTriggerAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Trigger, controllerReference);
                Vector2 currentGripAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Grip, controllerReference);
                Vector2 currentTouchpadAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Touchpad, controllerReference);

                // Save current touch and trigger settings to detect next change.
                touchpadAxis = new Vector2(currentTouchpadAxis.x, currentTouchpadAxis.y);
                triggerAxis = new Vector2(currentTriggerAxis.x, currentTriggerAxis.y);
                gripAxis = new Vector2(currentGripAxis.x, currentGripAxis.y);
                hairTriggerDelta = VRTK_SDK_Bridge.GetControllerHairlineDelta(SDK_BaseController.ButtonTypes.TriggerHairline, controllerReference);
                hairGripDelta = VRTK_SDK_Bridge.GetControllerHairlineDelta(SDK_BaseController.ButtonTypes.GripHairline, controllerReference);
            }
        }
    }
}
