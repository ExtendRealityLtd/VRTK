// Controller Events|Interactors|30010
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controllerReference">The reference for the controller that initiated the event.</param>
    /// <param name="buttonPressure">The amount of pressure being applied to the button pressed. `0f` to `1f`.</param>
    /// <param name="touchpadAxis">The position the touchpad is touched at. `(0,0)` to `(1,1)`.</param>
    /// <param name="touchpadAngle">The rotational position the touchpad is being touched at, 0 being top, 180 being bottom and all other angles accordingly. `0f` to `360f`.</param>
    /// <param name="touchpadTwoAxis">The position the touchpad two is touched at. `(0,0)` to `(1,1)`.</param>
    /// <param name="touchpadTwoAngle">The rotational position the touchpad two is being touched at, 0 being top, 180 being bottom and all other angles accordingly. `0f` to `360f`.</param>
    public struct ControllerInteractionEventArgs
    {
        public VRTK_ControllerReference controllerReference;
        public float buttonPressure;
        public Vector2 touchpadAxis;
        public float touchpadAngle;
        public Vector2 touchpadTwoAxis;
        public float touchpadTwoAngle;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="ControllerInteractionEventArgs"/></param>
    public delegate void ControllerInteractionEventHandler(object sender, ControllerInteractionEventArgs e);

    /// <summary>
    /// A relationship to a physical VR controller and emits events based on the inputs of the controller.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///  * Place the `VRTK_ControllerEvents` script on the controller script alias GameObject of the controller to track (e.g. Right Controller Script Alias).
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/002_Controller_Events` shows how the events are utilised and listened to. The accompanying example script can be viewed in `VRTK/Examples/ExampleResources/Scripts/VRTK_ControllerEvents_ListenerExample.cs`.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactors/VRTK_ControllerEvents")]
    public class VRTK_ControllerEvents : MonoBehaviour
    {
        /// <summary>
        /// Button types
        /// </summary>
        public enum ButtonAlias
        {
            /// <summary>
            /// No button specified.
            /// </summary>
            Undefined,
            /// <summary>
            /// The trigger is squeezed past the current hairline threshold.
            /// </summary>
            TriggerHairline,
            /// <summary>
            /// The trigger is squeezed a small amount.
            /// </summary>
            TriggerTouch,
            /// <summary>
            /// The trigger is squeezed about half way in.
            /// </summary>
            TriggerPress,
            /// <summary>
            /// The trigger is squeezed all the way down.
            /// </summary>
            TriggerClick,
            /// <summary>
            /// The grip is squeezed past the current hairline threshold.
            /// </summary>
            GripHairline,
            /// <summary>
            /// The grip button is touched.
            /// </summary>
            GripTouch,
            /// <summary>
            /// The grip button is pressed.
            /// </summary>
            GripPress,
            /// <summary>
            /// The grip button is pressed all the way down.
            /// </summary>
            GripClick,
            /// <summary>
            /// The touchpad is touched (without pressing down to click).
            /// </summary>
            TouchpadTouch,
            /// <summary>
            /// The touchpad is pressed (to the point of hearing a click).
            /// </summary>
            TouchpadPress,
            /// <summary>
            /// The touchpad two is touched (without pressing down to click).
            /// </summary>
            TouchpadTwoTouch,
            /// <summary>
            /// The button one is touched.
            /// </summary>
            ButtonOneTouch,
            /// <summary>
            /// The button one is pressed.
            /// </summary>
            ButtonOnePress,
            /// <summary>
            /// The button two is touched.
            /// </summary>
            ButtonTwoTouch,
            /// <summary>
            /// The button two is pressed.
            /// </summary>
            ButtonTwoPress,
            /// <summary>
            /// The start menu is pressed.
            /// </summary>
            StartMenuPress,
            /// <summary>
            /// The touchpad sense touch is active.
            /// </summary>
            TouchpadSense,
            /// <summary>
            /// The trigger sense touch is active.
            /// </summary>
            TriggerSense,
            /// <summary>
            /// The middle finger sense touch is active.
            /// </summary>
            MiddleFingerSense,
            /// <summary>
            /// The ring finger sense touch is active.
            /// </summary>
            RingFingerSense,
            /// <summary>
            /// The pinky finger sense touch is active.
            /// </summary>
            PinkyFingerSense
        }

        /// <summary>
        /// Vector2 Axis Types.
        /// </summary>
        public enum Vector2AxisAlias
        {
            /// <summary>
            /// No axis specified.
            /// </summary>
            Undefined,
            /// <summary>
            /// Touchpad on the controller.
            /// </summary>
            Touchpad,
            /// <summary>
            /// Touchpad Two on the controller.
            /// </summary>
            TouchpadTwo
        }

        /// <summary>
        /// Axis Types
        /// </summary>
        public enum AxisType
        {
            /// <summary>
            /// A digital axis with a binary result of 0f not pressed or 1f is pressed.
            /// </summary>
            Digital,
            /// <summary>
            /// An analog axis ranging from no squeeze at 0f to full squeeze at 1f.
            /// </summary>
            Axis,
            /// <summary>
            /// A cap sens axis ranging from not near at 0f to touching at 1f.
            /// </summary>
            SenseAxis
        }

        [Header("Axis Refinement Settings")]

        [Tooltip("The amount of fidelity in the changes on the axis, which is defaulted to 1. Any number higher than 2 will probably give too sensitive results.")]
        public int axisFidelity = 1;
        [Tooltip("The level on a sense axis to reach before the sense axis is forced to 0f")]
        [Range(0f, 1f)]
        public float senseAxisForceZeroThreshold = 0.15f;
        [Tooltip("The amount of pressure required to be applied to a sense button before considering the sense button pressed.")]
        [Range(0f, 1f)]
        public float senseAxisPressThreshold = 0.95f;

        [Header("Trigger Refinement Settings")]

        [Tooltip("The level on the trigger axis to reach before a click is registered.")]
        public float triggerClickThreshold = 1f;
        [Tooltip("The level on the trigger axis to reach before the axis is forced to 0f.")]
        public float triggerForceZeroThreshold = 0.01f;
        [Tooltip("If this is checked then the trigger axis will be forced to 0f when the trigger button reports an untouch event.")]
        public bool triggerAxisZeroOnUntouch = false;

        [Header("Grip Refinement Settings")]

        [Tooltip("The level on the grip axis to reach before a click is registered.")]
        public float gripClickThreshold = 1f;
        [Tooltip("The level on the grip axis to reach before the axis is forced to 0f.")]
        public float gripForceZeroThreshold = 0.01f;
        [Tooltip("If this is checked then the grip axis will be forced to 0f when the grip button reports an untouch event.")]
        public bool gripAxisZeroOnUntouch = false;

        #region bool states

        #region trigger bool states
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
        /// This will be true if the trigger sense is being touched more or less.
        /// </summary>
        [HideInInspector]
        public bool triggerSenseAxisChanged = false;
        #endregion trigger bool states

        #region grip bool states
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
        #endregion grip bool states

        #region touchpad bool states
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
        /// This will be true if the touchpad position has changed.
        /// </summary>
        [HideInInspector]
        public bool touchpadAxisChanged = false;
        /// <summary>
        /// This will be true if the touchpad sense is being touched more or less.
        /// </summary>
        [HideInInspector]
        public bool touchpadSenseAxisChanged = false;
        /// <summary>
        /// This will be true if the touchpad two is being touched.
        /// </summary>
        [HideInInspector]
        public bool touchpadTwoTouched = false;
        /// This will be true if the touchpad two position has changed.
        /// </summary>
        [HideInInspector]
        public bool touchpadTwoAxisChanged = false;
        #endregion touchpad bool states

        #region button one bool states
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
        #endregion button one bool states

        #region button two bool states
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
        #endregion button two bool states

        #region start menu bool states
        /// <summary>
        /// This will be true if start menu is held down.
        /// </summary>
        [HideInInspector]
        public bool startMenuPressed = false;
        #endregion start menu bool states

        #region extra finger bool states
        /// <summary>
        /// This will be true if the middle finger sense is being touched more or less.
        /// </summary>
        [HideInInspector]
        public bool middleFingerSenseAxisChanged = false;
        /// <summary>
        /// This will be true if the ring finger sense is being touched more or less.
        /// </summary>
        [HideInInspector]
        public bool ringFingerSenseAxisChanged = false;
        /// <summary>
        /// This will be true if the pinky finger sense is being touched more or less.
        /// </summary>
        [HideInInspector]
        public bool pinkyFingerSenseAxisChanged = false;
        #endregion extra finger bool states

        /// <summary>
        /// This will be true if the controller model alias renderers are visible.
        /// </summary>
        [HideInInspector]
        public bool controllerVisible = true;

        #endregion bool states

        #region controller events

        #region controller trigger events
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
        /// Emitted when the amount of touch on the trigger sense changes.
        /// </summary>
        public event ControllerInteractionEventHandler TriggerSenseAxisChanged;
        #endregion controller trigger events

        #region controller grip events
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
        #endregion controller grip events

        #region controller touchpad events
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
        /// Emitted when the amount of touch on the touchpad sense changes.
        /// </summary>
        public event ControllerInteractionEventHandler TouchpadSenseAxisChanged;

        /// <summary>
        /// Emitted when the touchpad two is touched (without pressing down to click).
        /// </summary>
        public event ControllerInteractionEventHandler TouchpadTwoTouchStart;
        /// <summary>
        /// Emitted when the touchpad two is no longer being touched.
        /// </summary>
        public event ControllerInteractionEventHandler TouchpadTwoTouchEnd;

        /// <summary>
        /// Emitted when the touchpad two is being touched in a different location.
        /// </summary>
        public event ControllerInteractionEventHandler TouchpadTwoAxisChanged;
        #endregion controller touchpad events

        #region controller button one events
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
        #endregion controller button one events

        #region controller button two events
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
        #endregion controller button one events

        #region controller start menu events
        /// <summary>
        /// Emitted when start menu is pressed.
        /// </summary>
        public event ControllerInteractionEventHandler StartMenuPressed;
        /// <summary>
        /// Emitted when start menu is released.
        /// </summary>
        public event ControllerInteractionEventHandler StartMenuReleased;
        #endregion controller start menu events

        #region controller extra finger events
        /// <summary>
        /// Emitted when the amount of touch on the middle finger sense changes.
        /// </summary>
        public event ControllerInteractionEventHandler MiddleFingerSenseAxisChanged;

        /// <summary>
        /// Emitted when the amount of touch on the ring finger sense changes.
        /// </summary>
        public event ControllerInteractionEventHandler RingFingerSenseAxisChanged;

        /// <summary>
        /// Emitted when the amount of touch on the pinky finger sense changes.
        /// </summary>
        public event ControllerInteractionEventHandler PinkyFingerSenseAxisChanged;
        #endregion controller extra finger events

        #region controller generic events
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
        /// Emitted when the controller model becomes available.
        /// </summary>
        public event ControllerInteractionEventHandler ControllerModelAvailable;

        /// <summary>
        /// Emitted when the controller is set to visible.
        /// </summary>
        public event ControllerInteractionEventHandler ControllerVisible;
        /// <summary>
        /// Emitted when the controller is set to hidden.
        /// </summary>
        public event ControllerInteractionEventHandler ControllerHidden;
        #endregion controller generic events

        #endregion controller events

        protected Vector2 touchpadAxis = Vector2.zero;
        protected Vector2 touchpadTwoAxis = Vector2.zero;
        protected Vector2 triggerAxis = Vector2.zero;
        protected Vector2 gripAxis = Vector2.zero;
        protected float touchpadSenseAxis = 0f;
        protected float triggerSenseAxis = 0f;
        protected float middleFingerSenseAxis = 0f;
        protected float ringFingerSenseAxis = 0f;
        protected float pinkyFingerSenseAxis = 0f;
        protected float hairTriggerDelta;
        protected float hairGripDelta;
        protected VRTK_TrackedController trackedController;

        #region event methods

        #region event trigger methods
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

        public virtual void OnTriggerSenseAxisChanged(ControllerInteractionEventArgs e)
        {
            if (TriggerSenseAxisChanged != null)
            {
                TriggerSenseAxisChanged(this, e);
            }
        }
        #endregion event trigger methods

        #region event grip methods
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
        #endregion event grip methods

        #region event touchpad methods
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

        public virtual void OnTouchpadSenseAxisChanged(ControllerInteractionEventArgs e)
        {
            if (TouchpadSenseAxisChanged != null)
            {
                TouchpadSenseAxisChanged(this, e);
            }
        }

        public virtual void OnTouchpadTwoTouchStart(ControllerInteractionEventArgs e)
        {
            if (TouchpadTwoTouchStart != null)
            {
                TouchpadTwoTouchStart(this, e);
            }
        }

        public virtual void OnTouchpadTwoTouchEnd(ControllerInteractionEventArgs e)
        {
            if (TouchpadTwoTouchEnd != null)
            {
                TouchpadTwoTouchEnd(this, e);
            }
        }

        public virtual void OnTouchpadTwoAxisChanged(ControllerInteractionEventArgs e)
        {
            if (TouchpadTwoAxisChanged != null)
            {
                TouchpadTwoAxisChanged(this, e);
            }
        }
        #endregion event touchpad methods

        #region event button one methods
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
        #endregion event button one methods

        #region event button two methods
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
        #endregion event button two methods

        #region event start menu methods
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
        #endregion event start menu methods

        #region event extra finger methods
        public virtual void OnMiddleFingerSenseAxisChanged(ControllerInteractionEventArgs e)
        {
            if (MiddleFingerSenseAxisChanged != null)
            {
                MiddleFingerSenseAxisChanged(this, e);
            }
        }

        public virtual void OnRingFingerSenseAxisChanged(ControllerInteractionEventArgs e)
        {
            if (RingFingerSenseAxisChanged != null)
            {
                RingFingerSenseAxisChanged(this, e);
            }
        }

        public virtual void OnPinkyFingerSenseAxisChanged(ControllerInteractionEventArgs e)
        {
            if (PinkyFingerSenseAxisChanged != null)
            {
                PinkyFingerSenseAxisChanged(this, e);
            }
        }
        #endregion event extra finger methods

        #region event generic methods
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

        public virtual void OnControllerModelAvailable(ControllerInteractionEventArgs e)
        {
            if (ControllerModelAvailable != null)
            {
                ControllerModelAvailable(this, e);
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
        #endregion event generic methods

        #endregion event methods

        /// <summary>
        /// The SetControllerEvent/0 method is used to set the Controller Event payload.
        /// </summary>
        /// <returns>The payload for a Controller Event.</returns>
        public virtual ControllerInteractionEventArgs SetControllerEvent()
        {
            bool nullBool = false;
            return SetControllerEvent(ref nullBool);
        }

        /// <summary>
        /// The SetControllerEvent/3 method is used to set the Controller Event payload.
        /// </summary>
        /// <param name="buttonBool">The state of the pressed button if required.</param>
        /// <param name="value">The value to set the `buttonBool` reference to.</param>
        /// <param name="buttonPressure">The pressure of the button pressed if required.</param>
        /// <returns>The payload for a Controller Event.</returns>
        public virtual ControllerInteractionEventArgs SetControllerEvent(ref bool buttonBool, bool value = false, float buttonPressure = 0f)
        {
            VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(gameObject);
            buttonBool = value;
            ControllerInteractionEventArgs e;
            e.controllerReference = controllerReference;
            e.buttonPressure = buttonPressure;
            e.touchpadAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Touchpad, controllerReference);
            e.touchpadAngle = CalculateVector2AxisAngle(e.touchpadAxis);
            e.touchpadTwoAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.TouchpadTwo, controllerReference);
            e.touchpadTwoAngle = CalculateVector2AxisAngle(e.touchpadTwoAxis);
            return e;
        }

        /// <summary>
        /// The GetControllerType method is a shortcut to retrieve the current controller type the Controller Events is attached to.
        /// </summary>
        /// <returns>The type of controller that the Controller Events is attached to.</returns>
        public virtual SDK_BaseController.ControllerType GetControllerType()
        {
            return (trackedController != null ? trackedController.GetControllerType() : SDK_BaseController.ControllerType.Undefined);
        }

        #region axis getters
        /// <summary>
        /// The GetAxis method returns the coordinates of where the given axis type is being touched.
        /// </summary>
        /// <param name="vector2AxisType">The Vector2AxisType to check the touch position of.</param>
        /// <returns>A two dimensional vector containing the `x` and `y` position of where the given axis type is being touched. `(0,0)` to `(1,1)`.</returns>
        public virtual Vector2 GetAxis(Vector2AxisAlias vector2AxisType)
        {
            switch (vector2AxisType)
            {
                case Vector2AxisAlias.Touchpad:
                    return GetTouchpadAxis();
                case Vector2AxisAlias.TouchpadTwo:
                    return GetTouchpadTwoAxis();
            }
            return Vector2.zero;
        }

        /// <summary>
        /// The GetTouchpadAxis method returns the coordinates of where the touchpad is being touched and can be used for directional input via the touchpad. The `x` value is the horizontal touch plane and the `y` value is the vertical touch plane.
        /// </summary>
        /// <returns>A two dimensional vector containing the `x` and `y` position of where the touchpad is being touched. `(0,0)` to `(1,1)`.</returns>
        public virtual Vector2 GetTouchpadAxis()
        {
            return touchpadAxis;
        }

        /// <summary>
        /// The GetTouchpadTwoAxis method returns the coordinates of where the touchpad two is being touched and can be used for directional input via the touchpad two. The `x` value is the horizontal touch plane and the `y` value is the vertical touch plane.
        /// </summary>
        /// <returns>A two dimensional vector containing the `x` and `y` position of where the touchpad two is being touched. `(0,0)` to `(1,1)`.</returns>
        public virtual Vector2 GetTouchpadTwoAxis()
        {
            return touchpadTwoAxis;
        }

        /// <summary>
        /// The GetAxisAngle method returns the angle of where the given axis type is currently being touched with the top of the given axis type being `0` degrees and the bottom of the given axis type being `180` degrees.
        /// </summary>
        /// <param name="vector2AxisType">The Vector2AxisType to get the touch angle for.</param>
        /// <returns>A float representing the angle of where the given axis type is being touched. `0f` to `360f`.</returns>
        public virtual float GetAxisAngle(Vector2AxisAlias vector2AxisType)
        {
            switch (vector2AxisType)
            {
                case Vector2AxisAlias.Touchpad:
                    return GetTouchpadAxisAngle();
                case Vector2AxisAlias.TouchpadTwo:
                    return GetTouchpadTwoAxisAngle();
            }
            return 0f;
        }

        /// <summary>
        /// The GetTouchpadAxisAngle method returns the angle of where the touchpad is currently being touched with the top of the touchpad being `0` degrees and the bottom of the touchpad being `180` degrees.
        /// </summary>
        /// <returns>A float representing the angle of where the touchpad is being touched. `0f` to `360f`.</returns>
        public virtual float GetTouchpadAxisAngle()
        {
            return CalculateVector2AxisAngle(touchpadAxis);
        }

        /// <summary>
        /// The GetTouchpadTwoAxisAngle method returns the angle of where the touchpad two is currently being touched with the top of the touchpad two being `0` degrees and the bottom of the touchpad two being `180` degrees.
        /// </summary>
        /// <returns>A float representing the angle of where the touchpad two is being touched. `0f` to `360f`.</returns>
        public virtual float GetTouchpadTwoAxisAngle()
        {
            return CalculateVector2AxisAngle(touchpadTwoAxis);
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
        #endregion axis getters

        #region hairline delta getters
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
        #endregion hairline delta getters

        #region sense axis getters
        /// <summary>
        /// The GetTouchpadSenseAxis method returns a float representing how much of the touch sensor is being touched.
        /// </summary>
        /// <returns>A float representing how much the touch sensor is being touched.</returns>
        public virtual float GetTouchpadSenseAxis()
        {
            return touchpadSenseAxis;
        }

        /// <summary>
        /// The GetTriggerSenseAxis method returns a float representing how much of the touch sensor is being touched.
        /// </summary>
        /// <returns>A float representing how much the touch sensor is being touched.</returns>
        public virtual float GetTriggerSenseAxis()
        {
            return triggerSenseAxis;
        }

        /// <summary>
        /// The GetMiddleFingerSenseAxis method returns a float representing how much of the touch sensor is being touched.
        /// </summary>
        /// <returns>A float representing how much the touch sensor is being touched.</returns>
        public virtual float GetMiddleFingerSenseAxis()
        {
            return middleFingerSenseAxis;
        }

        /// <summary>
        /// The GetRingFingerSenseAxis method returns a float representing how much of the touch sensor is being touched.
        /// </summary>
        /// <returns>A float representing how much the touch sensor is being touched.</returns>
        public virtual float GetRingFingerSenseAxis()
        {
            return ringFingerSenseAxis;
        }

        /// <summary>
        /// The GetPinkyFingerSenseAxis method returns a float representing how much of the touch sensor is being touched.
        /// </summary>
        /// <returns>A float representing how much the touch sensor is being touched.</returns>
        public virtual float GetPinkyFingerSenseAxis()
        {
            return pinkyFingerSenseAxis;
        }
        #endregion sense axis getters

        #region button press getters
        /// <summary>
        /// The AnyButtonPressed method returns true if any of the controller buttons are being pressed and this can be useful to determine if an action can be taken whilst the user is using the controller.
        /// </summary>
        /// <returns>Returns `true` if any of the controller buttons are currently being pressed.</returns>
        public virtual bool AnyButtonPressed()
        {
            return (triggerPressed || gripPressed || touchpadPressed || buttonOnePressed || buttonTwoPressed || startMenuPressed);
        }

        /// <summary>
        /// The GetAxisState method takes a given Vector2Axis and returns a boolean whether that given axis is currently being touched or pressed.
        /// </summary>
        /// <param name="axis">The axis to check on.</param>
        /// <param name="pressType">The button press type to check for.</param>
        /// <returns>Returns `true` if the axis is being interacted with via the given press type.</returns>
        public virtual bool GetAxisState(Vector2AxisAlias axis, SDK_BaseController.ButtonPressTypes pressType)
        {
            switch (pressType)
            {
                case SDK_BaseController.ButtonPressTypes.Press:
                case SDK_BaseController.ButtonPressTypes.PressDown:
                case SDK_BaseController.ButtonPressTypes.PressUp:
                    return (axis == Vector2AxisAlias.Touchpad ? touchpadPressed : false);
                case SDK_BaseController.ButtonPressTypes.Touch:
                case SDK_BaseController.ButtonPressTypes.TouchDown:
                case SDK_BaseController.ButtonPressTypes.TouchUp:
                    return (axis == Vector2AxisAlias.Touchpad ? touchpadTouched : touchpadTwoTouched);
            }
            return false;
        }

        /// <summary>
        /// The IsButtonPressed method takes a given button alias and returns a boolean whether that given button is currently being pressed or not.
        /// </summary>
        /// <param name="button">The button to check if it's being pressed.</param>
        /// <returns>Returns `true` if the button is being pressed.</returns>
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
                case ButtonAlias.TriggerSense:
                    return (triggerSenseAxis >= senseAxisPressThreshold);
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
                case ButtonAlias.TouchpadTwoTouch:
                    return touchpadTwoTouched;
                case ButtonAlias.TouchpadSense:
                    return (touchpadSenseAxis >= senseAxisPressThreshold);
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
                case ButtonAlias.MiddleFingerSense:
                    return (middleFingerSenseAxis >= senseAxisPressThreshold);
                case ButtonAlias.RingFingerSense:
                    return (ringFingerSenseAxis >= senseAxisPressThreshold);
                case ButtonAlias.PinkyFingerSense:
                    return (pinkyFingerSenseAxis >= senseAxisPressThreshold);
            }
            return false;
        }
        #endregion button press getters

        #region subscription managers
        /// <summary>
        /// The SubscribeToButtonAliasEvent method makes it easier to subscribe to a button event on either the start or end action. Upon the event firing, the given callback method is executed.
        /// </summary>
        /// <param name="givenButton">The Button Alias to register the event on.</param>
        /// <param name="startEvent">If this is `true` then the start event related to the button is used (e.g. `OnPress`). If this is `false` then the end event related to the button is used (e.g. `OnRelease`). </param>
        /// <param name="callbackMethod">The method to subscribe to the event.</param>
        public virtual void SubscribeToButtonAliasEvent(ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)
        {
            ButtonAliasEventSubscription(true, givenButton, startEvent, callbackMethod);
        }

        /// <summary>
        /// The UnsubscribeToButtonAliasEvent method makes it easier to unsubscribe to from button event on either the start or end action.
        /// </summary>
        /// <param name="givenButton">The Button Alias to unregister the event on.</param>
        /// <param name="startEvent">If this is `true` then the start event related to the button is used (e.g. `OnPress`). If this is `false` then the end event related to the button is used (e.g. `OnRelease`). </param>
        /// <param name="callbackMethod">The method to unsubscribe from the event.</param>
        public virtual void UnsubscribeToButtonAliasEvent(ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)
        {
            ButtonAliasEventSubscription(false, givenButton, startEvent, callbackMethod);
        }

        /// <summary>
        /// The SubscribeToAxisAliasEvent method makes it easier to subscribe to axis changes on a given button for a given axis type.
        /// </summary>
        /// <param name="buttonType">The button to listen for axis changes on.</param>
        /// <param name="axisType">The type of axis change to listen for.</param>
        /// <param name="callbackMethod">The method to subscribe to the event.</param>
        public virtual void SubscribeToAxisAliasEvent(SDK_BaseController.ButtonTypes buttonType, AxisType axisType, ControllerInteractionEventHandler callbackMethod)
        {
            AxisAliasEventSubscription(true, buttonType, axisType, callbackMethod);
        }

        /// <summary>
        /// The UnsubscribeToAxisAliasEvent method makes it easier to unsubscribe from axis changes on a given button for a given axis type.
        /// </summary>
        /// <param name="buttonType">The button to unregister for axis changes on.</param>
        /// <param name="axisType">The type of axis change to unregister on.</param>
        /// <param name="callbackMethod">The method to unsubscribe from the event.</param>
        public virtual void UnsubscribeToAxisAliasEvent(SDK_BaseController.ButtonTypes buttonType, AxisType axisType, ControllerInteractionEventHandler callbackMethod)
        {
            AxisAliasEventSubscription(false, buttonType, axisType, callbackMethod);
        }
        #endregion subscription managers

        #region MonoBehaviour methods
        protected virtual void Awake()
        {
            VRTK_SDKManager.AttemptAddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            GameObject actualController = VRTK_DeviceFinder.GetActualController(gameObject);
            if (actualController != null)
            {
                trackedController = actualController.GetComponentInParent<VRTK_TrackedController>();
                if (trackedController != null)
                {
                    trackedController.ControllerEnabled += TrackedControllerEnabled;
                    trackedController.ControllerDisabled += TrackedControllerDisabled;
                    trackedController.ControllerIndexChanged += TrackedControllerIndexChanged;
                    trackedController.ControllerModelAvailable += TrackedControllerModelAvailable;
                }
            }
        }

        protected virtual void OnDisable()
        {
            Invoke("DisableEvents", 0f);
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.AttemptRemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Update()
        {
            VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(gameObject);

            //Only continue if the controller reference is valid
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return;
            }

            CheckTriggerEvents(controllerReference);
            CheckGripEvents(controllerReference);
            CheckTouchpadEvents(controllerReference);
            CheckTouchpadTwoEvents(controllerReference);
            CheckButtonOneEvents(controllerReference);
            CheckButtonTwoEvents(controllerReference);
            CheckStartMenuEvents(controllerReference);
            CheckExtraFingerEvents(controllerReference);
        }
        #endregion MonoBehaviour methods

        protected virtual float ProcessSenseAxis(float axisValue)
        {
            return (axisValue >= senseAxisForceZeroThreshold ? axisValue : 0f);
        }

        protected virtual void CheckTriggerEvents(VRTK_ControllerReference controllerReference)
        {
            Vector2 currentTriggerAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Trigger, controllerReference);
            float currentTriggerSenseAxis = ProcessSenseAxis(VRTK_SDK_Bridge.GetControllerSenseAxis(SDK_BaseController.ButtonTypes.Trigger, controllerReference));

            //Trigger Touched
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.TouchDown, controllerReference))
            {
                OnTriggerTouchStart(SetControllerEvent(ref triggerTouched, true, currentTriggerAxis.x));
            }

            //Trigger Hairline
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.TriggerHairline, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
            {
                OnTriggerHairlineStart(SetControllerEvent(ref triggerHairlinePressed, true, currentTriggerAxis.x));
            }

            //Trigger Pressed
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
            {
                OnTriggerPressed(SetControllerEvent(ref triggerPressed, true, currentTriggerAxis.x));
            }

            //Trigger Clicked
            if (!triggerClicked && currentTriggerAxis.x >= triggerClickThreshold)
            {
                OnTriggerClicked(SetControllerEvent(ref triggerClicked, true, currentTriggerAxis.x));
            }
            else if (triggerClicked && currentTriggerAxis.x < triggerClickThreshold)
            {
                OnTriggerUnclicked(SetControllerEvent(ref triggerClicked, false, 0f));
            }

            // Trigger Pressed end
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
                OnTriggerReleased(SetControllerEvent(ref triggerPressed, false, 0f));
            }

            //Trigger Hairline End
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.TriggerHairline, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
                OnTriggerHairlineEnd(SetControllerEvent(ref triggerHairlinePressed, false, 0f));
            }

            //Trigger Touch End
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.TouchUp, controllerReference))
            {
                OnTriggerTouchEnd(SetControllerEvent(ref triggerTouched, false, 0f));
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

            //Trigger Sense Axis
            if (VRTK_SharedMethods.RoundFloat(triggerSenseAxis, axisFidelity) == VRTK_SharedMethods.RoundFloat(currentTriggerSenseAxis, axisFidelity))
            {
                triggerSenseAxisChanged = false;
            }
            else
            {
                OnTriggerSenseAxisChanged(SetControllerEvent(ref triggerSenseAxisChanged, true, currentTriggerSenseAxis));
            }

            triggerAxis = (triggerAxisChanged ? new Vector2(currentTriggerAxis.x, currentTriggerAxis.y) : triggerAxis);
            triggerSenseAxis = (triggerSenseAxisChanged ? currentTriggerSenseAxis : triggerSenseAxis);
            hairTriggerDelta = VRTK_SDK_Bridge.GetControllerHairlineDelta(SDK_BaseController.ButtonTypes.TriggerHairline, controllerReference);
        }

        protected virtual void CheckGripEvents(VRTK_ControllerReference controllerReference)
        {
            Vector2 currentGripAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Grip, controllerReference);

            //Grip Touched
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.TouchDown, controllerReference))
            {
                OnGripTouchStart(SetControllerEvent(ref gripTouched, true, currentGripAxis.x));
            }

            //Grip Hairline
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.GripHairline, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
            {
                OnGripHairlineStart(SetControllerEvent(ref gripHairlinePressed, true, currentGripAxis.x));
            }

            //Grip Pressed
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
            {
                OnGripPressed(SetControllerEvent(ref gripPressed, true, currentGripAxis.x));
            }

            //Grip Clicked
            if (!gripClicked && currentGripAxis.x >= gripClickThreshold)
            {
                OnGripClicked(SetControllerEvent(ref gripClicked, true, currentGripAxis.x));
            }
            else if (gripClicked && currentGripAxis.x < gripClickThreshold)
            {
                OnGripUnclicked(SetControllerEvent(ref gripClicked, false, 0f));
            }

            // Grip Pressed End
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
                OnGripReleased(SetControllerEvent(ref gripPressed, false, 0f));
            }

            //Grip Hairline End
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.GripHairline, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
                OnGripHairlineEnd(SetControllerEvent(ref gripHairlinePressed, false, 0f));
            }

            // Grip Touch End
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.TouchUp, controllerReference))
            {
                OnGripTouchEnd(SetControllerEvent(ref gripTouched, false, 0f));
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

            gripAxis = (gripAxisChanged ? new Vector2(currentGripAxis.x, currentGripAxis.y) : gripAxis);
            hairGripDelta = VRTK_SDK_Bridge.GetControllerHairlineDelta(SDK_BaseController.ButtonTypes.GripHairline, controllerReference);
        }

        protected virtual void CheckTouchpadEvents(VRTK_ControllerReference controllerReference)
        {
            Vector2 currentTouchpadAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Touchpad, controllerReference);
            float currentTouchpadSenseAxis = ProcessSenseAxis(VRTK_SDK_Bridge.GetControllerSenseAxis(SDK_BaseController.ButtonTypes.Touchpad, controllerReference));

            //Touchpad Touched
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.TouchDown, controllerReference))
            {
                OnTouchpadTouchStart(SetControllerEvent(ref touchpadTouched, true, 1f));
            }

            //Touchpad Pressed
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
            {
                OnTouchpadPressed(SetControllerEvent(ref touchpadPressed, true, 1f));
            }
            else if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
                OnTouchpadReleased(SetControllerEvent(ref touchpadPressed, false, 0f));
            }

            //Touchpad Untouched
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.TouchUp, controllerReference))
            {
                OnTouchpadTouchEnd(SetControllerEvent(ref touchpadTouched, false, 0f));
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

            //Touchpad Sense Axis
            if (VRTK_SharedMethods.RoundFloat(touchpadSenseAxis, axisFidelity) == VRTK_SharedMethods.RoundFloat(currentTouchpadSenseAxis, axisFidelity))
            {
                touchpadSenseAxisChanged = false;
            }
            else
            {
                OnTouchpadSenseAxisChanged(SetControllerEvent(ref touchpadSenseAxisChanged, true, currentTouchpadSenseAxis));
            }

            touchpadAxis = (touchpadAxisChanged ? new Vector2(currentTouchpadAxis.x, currentTouchpadAxis.y) : touchpadAxis);
            touchpadSenseAxis = (touchpadSenseAxisChanged ? currentTouchpadSenseAxis : touchpadSenseAxis);
        }

        protected virtual void CheckTouchpadTwoEvents(VRTK_ControllerReference controllerReference)
        {
            Vector2 currentTouchpadTwoAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.TouchpadTwo, controllerReference);

            //Touchpad Two Touched
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.TouchpadTwo, SDK_BaseController.ButtonPressTypes.TouchDown, controllerReference))
            {
                OnTouchpadTwoTouchStart(SetControllerEvent(ref touchpadTwoTouched, true, 1f));
            }

            //Touchpad Two Untouched
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.TouchpadTwo, SDK_BaseController.ButtonPressTypes.TouchUp, controllerReference))
            {
                OnTouchpadTwoTouchEnd(SetControllerEvent(ref touchpadTwoTouched, false, 0f));
                touchpadTwoAxis = Vector2.zero;
            }

            //Touchpad Two Axis
            if (VRTK_SDK_Bridge.IsTouchpadStatic(true, touchpadTwoAxis, currentTouchpadTwoAxis, axisFidelity))
            {
                touchpadTwoAxisChanged = false;
            }
            else
            {
                OnTouchpadTwoAxisChanged(SetControllerEvent(ref touchpadTwoAxisChanged, true, 1f));
            }

            touchpadTwoAxis = (touchpadTwoAxisChanged ? new Vector2(currentTouchpadTwoAxis.x, currentTouchpadTwoAxis.y) : touchpadTwoAxis);
        }

        protected virtual void CheckButtonOneEvents(VRTK_ControllerReference controllerReference)
        {
            //ButtonOne Touched
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.TouchDown, controllerReference))
            {
                OnButtonOneTouchStart(SetControllerEvent(ref buttonOneTouched, true, 1f));
            }

            //ButtonOne Pressed
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
            {
                OnButtonOnePressed(SetControllerEvent(ref buttonOnePressed, true, 1f));
            }
            else if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
                OnButtonOneReleased(SetControllerEvent(ref buttonOnePressed, false, 0f));
            }

            //ButtonOne Touched End
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.TouchUp, controllerReference))
            {
                OnButtonOneTouchEnd(SetControllerEvent(ref buttonOneTouched, false, 0f));
            }
        }

        protected virtual void CheckButtonTwoEvents(VRTK_ControllerReference controllerReference)
        {
            //ButtonTwo Touched
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.TouchDown, controllerReference))
            {
                OnButtonTwoTouchStart(SetControllerEvent(ref buttonTwoTouched, true, 1f));
            }

            //ButtonTwo Pressed
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
            {
                OnButtonTwoPressed(SetControllerEvent(ref buttonTwoPressed, true, 1f));
            }
            else if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
                OnButtonTwoReleased(SetControllerEvent(ref buttonTwoPressed, false, 0f));
            }

            //ButtonTwo Touched End
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.TouchUp, controllerReference))
            {
                OnButtonTwoTouchEnd(SetControllerEvent(ref buttonTwoTouched, false, 0f));
            }
        }

        protected virtual void CheckStartMenuEvents(VRTK_ControllerReference controllerReference)
        {
            //StartMenu Pressed
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.StartMenu, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
            {
                OnStartMenuPressed(SetControllerEvent(ref startMenuPressed, true, 1f));
            }
            else if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.StartMenu, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
                OnStartMenuReleased(SetControllerEvent(ref startMenuPressed, false, 0f));
            }
        }

        protected virtual void CheckExtraFingerEvents(VRTK_ControllerReference controllerReference)
        {
            float currentMiddleFingerSenseAxis = ProcessSenseAxis(VRTK_SDK_Bridge.GetControllerSenseAxis(SDK_BaseController.ButtonTypes.MiddleFinger, controllerReference));
            float currentRingFingerSenseAxis = ProcessSenseAxis(VRTK_SDK_Bridge.GetControllerSenseAxis(SDK_BaseController.ButtonTypes.RingFinger, controllerReference));
            float currentPinkyFingerSenseAxis = ProcessSenseAxis(VRTK_SDK_Bridge.GetControllerSenseAxis(SDK_BaseController.ButtonTypes.PinkyFinger, controllerReference));

            //Middle Finger Sense Axis
            if (VRTK_SharedMethods.RoundFloat(middleFingerSenseAxis, axisFidelity) == VRTK_SharedMethods.RoundFloat(currentMiddleFingerSenseAxis, axisFidelity))
            {
                middleFingerSenseAxisChanged = false;
            }
            else
            {
                OnMiddleFingerSenseAxisChanged(SetControllerEvent(ref middleFingerSenseAxisChanged, true, currentMiddleFingerSenseAxis));
            }

            //Ring Finger Sense Axis
            if (VRTK_SharedMethods.RoundFloat(ringFingerSenseAxis, axisFidelity) == VRTK_SharedMethods.RoundFloat(currentRingFingerSenseAxis, axisFidelity))
            {
                ringFingerSenseAxisChanged = false;
            }
            else
            {
                OnRingFingerSenseAxisChanged(SetControllerEvent(ref ringFingerSenseAxisChanged, true, currentRingFingerSenseAxis));
            }

            //Pinky Finger Sense Axis
            if (VRTK_SharedMethods.RoundFloat(pinkyFingerSenseAxis, axisFidelity) == VRTK_SharedMethods.RoundFloat(currentPinkyFingerSenseAxis, axisFidelity))
            {
                pinkyFingerSenseAxisChanged = false;
            }
            else
            {
                OnPinkyFingerSenseAxisChanged(SetControllerEvent(ref pinkyFingerSenseAxisChanged, true, currentPinkyFingerSenseAxis));
            }

            middleFingerSenseAxis = (middleFingerSenseAxisChanged ? currentMiddleFingerSenseAxis : middleFingerSenseAxis);
            ringFingerSenseAxis = (ringFingerSenseAxisChanged ? currentRingFingerSenseAxis : ringFingerSenseAxis);
            pinkyFingerSenseAxis = (pinkyFingerSenseAxisChanged ? currentPinkyFingerSenseAxis : pinkyFingerSenseAxis);
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
                case ButtonAlias.TouchpadTwoTouch:
                    if (subscribe)
                    {
                        if (startEvent)
                        {
                            TouchpadTwoTouchStart += callbackMethod;
                        }
                        else
                        {
                            TouchpadTwoTouchEnd += callbackMethod;
                        }
                    }
                    else
                    {
                        if (startEvent)
                        {
                            TouchpadTwoTouchStart -= callbackMethod;
                        }
                        else
                        {
                            TouchpadTwoTouchEnd -= callbackMethod;
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

        protected virtual void AxisAliasEventSubscription(bool subscribe, SDK_BaseController.ButtonTypes buttonType, AxisType axisType, ControllerInteractionEventHandler callbackMethod)
        {
            switch (buttonType)
            {
                case SDK_BaseController.ButtonTypes.Trigger:
                    switch (axisType)
                    {
                        case AxisType.Axis:
                            if (subscribe)
                            {
                                TriggerAxisChanged += callbackMethod;
                            }
                            else
                            {
                                TriggerAxisChanged -= callbackMethod;
                            }
                            break;
                        case AxisType.SenseAxis:
                            if (subscribe)
                            {
                                TriggerSenseAxisChanged += callbackMethod;
                            }
                            else
                            {
                                TriggerSenseAxisChanged -= callbackMethod;
                            }
                            break;
                    }
                    break;
                case SDK_BaseController.ButtonTypes.Grip:
                    switch (axisType)
                    {
                        case AxisType.Axis:
                            if (subscribe)
                            {
                                GripAxisChanged += callbackMethod;
                            }
                            else
                            {
                                GripAxisChanged -= callbackMethod;
                            }
                            break;
                    }
                    break;
                case SDK_BaseController.ButtonTypes.Touchpad:
                    switch (axisType)
                    {
                        case AxisType.Axis:
                            if (subscribe)
                            {
                                TouchpadAxisChanged += callbackMethod;
                            }
                            else
                            {
                                TouchpadAxisChanged -= callbackMethod;
                            }
                            break;
                        case AxisType.SenseAxis:
                            if (subscribe)
                            {
                                TouchpadSenseAxisChanged += callbackMethod;
                            }
                            else
                            {
                                TouchpadSenseAxisChanged -= callbackMethod;
                            }
                            break;
                    }
                    break;
                case SDK_BaseController.ButtonTypes.TouchpadTwo:
                    if (subscribe)
                    {
                        TouchpadTwoAxisChanged += callbackMethod;
                    }
                    else
                    {
                        TouchpadTwoAxisChanged -= callbackMethod;
                    }
                    break;
                case SDK_BaseController.ButtonTypes.MiddleFinger:
                    switch (axisType)
                    {
                        case AxisType.SenseAxis:
                            if (subscribe)
                            {
                                MiddleFingerSenseAxisChanged += callbackMethod;
                            }
                            else
                            {
                                MiddleFingerSenseAxisChanged -= callbackMethod;
                            }
                            break;
                    }
                    break;
                case SDK_BaseController.ButtonTypes.RingFinger:
                    switch (axisType)
                    {
                        case AxisType.SenseAxis:
                            if (subscribe)
                            {
                                RingFingerSenseAxisChanged += callbackMethod;
                            }
                            else
                            {
                                RingFingerSenseAxisChanged -= callbackMethod;
                            }
                            break;
                    }
                    break;
                case SDK_BaseController.ButtonTypes.PinkyFinger:
                    switch (axisType)
                    {
                        case AxisType.SenseAxis:
                            if (subscribe)
                            {
                                PinkyFingerSenseAxisChanged += callbackMethod;
                            }
                            else
                            {
                                PinkyFingerSenseAxisChanged -= callbackMethod;
                            }
                            break;
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

        protected virtual void TrackedControllerModelAvailable(object sender, VRTKTrackedControllerEventArgs e)
        {
            OnControllerModelAvailable(SetControllerEvent());
        }

        protected virtual float CalculateVector2AxisAngle(Vector2 axis)
        {
            float angle = Mathf.Atan2(axis.y, axis.x) * Mathf.Rad2Deg;
            angle = 90.0f - angle;
            if (angle < 0)
            {
                angle += 360.0f;
            }
            return angle;
        }

        protected virtual void DisableEvents()
        {
            GameObject actualController = VRTK_DeviceFinder.GetActualController(gameObject);
            if (actualController != null)
            {
                if (trackedController != null)
                {
                    trackedController.ControllerEnabled -= TrackedControllerEnabled;
                    trackedController.ControllerDisabled -= TrackedControllerDisabled;
                    trackedController.ControllerIndexChanged -= TrackedControllerIndexChanged;
                    trackedController.ControllerModelAvailable -= TrackedControllerModelAvailable;
                }
            }

            if (triggerPressed)
            {
                OnTriggerReleased(SetControllerEvent(ref triggerPressed, false, 0f));
            }

            if (triggerTouched)
            {
                OnTriggerTouchEnd(SetControllerEvent(ref triggerTouched, false, 0f));
            }

            if (triggerHairlinePressed)
            {
                OnTriggerHairlineEnd(SetControllerEvent(ref triggerHairlinePressed, false, 0f));
            }

            if (triggerClicked)
            {
                OnTriggerUnclicked(SetControllerEvent(ref triggerClicked, false, 0f));
            }

            if (gripPressed)
            {
                OnGripReleased(SetControllerEvent(ref gripPressed, false, 0f));
            }

            if (gripTouched)
            {
                OnGripTouchEnd(SetControllerEvent(ref gripTouched, false, 0f));
            }

            if (gripHairlinePressed)
            {
                OnGripHairlineEnd(SetControllerEvent(ref gripHairlinePressed, false, 0f));
            }

            if (gripClicked)
            {
                OnGripUnclicked(SetControllerEvent(ref gripClicked, false, 0f));
            }

            if (touchpadPressed)
            {
                OnTouchpadReleased(SetControllerEvent(ref touchpadPressed, false, 0f));
            }

            if (touchpadTouched)
            {
                OnTouchpadTouchEnd(SetControllerEvent(ref touchpadTouched, false, 0f));
            }

            if (touchpadTwoTouched)
            {
                OnTouchpadTwoTouchEnd(SetControllerEvent(ref touchpadTwoTouched, false, 0f));
            }

            if (buttonOnePressed)
            {
                OnButtonOneReleased(SetControllerEvent(ref buttonOnePressed, false, 0f));
            }

            if (buttonOneTouched)
            {
                OnButtonOneTouchEnd(SetControllerEvent(ref buttonOneTouched, false, 0f));
            }

            if (buttonTwoPressed)
            {
                OnButtonTwoReleased(SetControllerEvent(ref buttonTwoPressed, false, 0f));
            }

            if (buttonTwoTouched)
            {
                OnButtonTwoTouchEnd(SetControllerEvent(ref buttonTwoTouched, false, 0f));
            }

            if (startMenuPressed)
            {
                OnStartMenuReleased(SetControllerEvent(ref startMenuPressed, false, 0f));
            }

            triggerAxisChanged = false;
            gripAxisChanged = false;
            touchpadAxisChanged = false;
            touchpadTwoAxisChanged = false;
            triggerSenseAxisChanged = false;
            touchpadSenseAxisChanged = false;
            middleFingerSenseAxisChanged = false;
            ringFingerSenseAxisChanged = false;
            pinkyFingerSenseAxisChanged = false;

            VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(gameObject);

            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                Vector2 currentTriggerAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Trigger, controllerReference);
                Vector2 currentGripAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Grip, controllerReference);
                Vector2 currentTouchpadAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Touchpad, controllerReference);
                Vector2 currentTouchpadTwoAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.TouchpadTwo, controllerReference);

                // Save current touch and trigger settings to detect next change.
                touchpadAxis = new Vector2(currentTouchpadAxis.x, currentTouchpadAxis.y);
                touchpadTwoAxis = new Vector2(currentTouchpadTwoAxis.x, currentTouchpadTwoAxis.y);
                triggerAxis = new Vector2(currentTriggerAxis.x, currentTriggerAxis.y);
                gripAxis = new Vector2(currentGripAxis.x, currentGripAxis.y);
                hairTriggerDelta = VRTK_SDK_Bridge.GetControllerHairlineDelta(SDK_BaseController.ButtonTypes.TriggerHairline, controllerReference);
                hairGripDelta = VRTK_SDK_Bridge.GetControllerHairlineDelta(SDK_BaseController.ButtonTypes.GripHairline, controllerReference);

                triggerSenseAxis = ProcessSenseAxis(VRTK_SDK_Bridge.GetControllerSenseAxis(SDK_BaseController.ButtonTypes.Trigger, controllerReference));
                touchpadSenseAxis = ProcessSenseAxis(VRTK_SDK_Bridge.GetControllerSenseAxis(SDK_BaseController.ButtonTypes.Touchpad, controllerReference));
                middleFingerSenseAxis = ProcessSenseAxis(VRTK_SDK_Bridge.GetControllerSenseAxis(SDK_BaseController.ButtonTypes.MiddleFinger, controllerReference));
                ringFingerSenseAxis = ProcessSenseAxis(VRTK_SDK_Bridge.GetControllerSenseAxis(SDK_BaseController.ButtonTypes.RingFinger, controllerReference));
                pinkyFingerSenseAxis = ProcessSenseAxis(VRTK_SDK_Bridge.GetControllerSenseAxis(SDK_BaseController.ButtonTypes.PinkyFinger, controllerReference));
            }
        }
    }
}