namespace VRTK
{
    using UnityEngine;

    // Radial Menu input from Vive Controller
    [RequireComponent(typeof(RadialMenu))]
    public class RadialMenuController : MonoBehaviour
    {
        public VRTK_ControllerEvents events;

        protected RadialMenu menu;
        protected float currentAngle; //Keep track of angle for when we click
        protected bool touchpadTouched;

        protected virtual void Awake()
        {
            menu = GetComponent<RadialMenu>();

            Initialize();
        }

        protected virtual void Initialize()
        {
            if (events == null)
            {
                events = GetComponentInParent<VRTK_ControllerEvents>();
            }
        }

        protected virtual void OnEnable()
        {
            if (events == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "RadialMenuController", "VRTK_ControllerEvents", "events", "the parent"));
                return;
            }
            else
            {
                events.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadClicked);
                events.TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadUnclicked);
                events.TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouched);
                events.TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadUntouched);
                events.TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);

                menu.FireHapticPulse += new HapticPulseEventHandler(AttemptHapticPulse);
            }
        }

        protected virtual void OnDisable()
        {
            events.TouchpadPressed -= new ControllerInteractionEventHandler(DoTouchpadClicked);
            events.TouchpadReleased -= new ControllerInteractionEventHandler(DoTouchpadUnclicked);
            events.TouchpadTouchStart -= new ControllerInteractionEventHandler(DoTouchpadTouched);
            events.TouchpadTouchEnd -= new ControllerInteractionEventHandler(DoTouchpadUntouched);
            events.TouchpadAxisChanged -= new ControllerInteractionEventHandler(DoTouchpadAxisChanged);

            menu.FireHapticPulse -= new HapticPulseEventHandler(AttemptHapticPulse);
        }

        protected virtual void DoClickButton(object sender = null) // The optional argument reduces the need for middleman functions in subclasses whose events likely pass object sender
        {
            menu.ClickButton(currentAngle);
        }

        protected virtual void DoUnClickButton(object sender = null)
        {
            menu.UnClickButton(currentAngle);
        }

        protected virtual void DoShowMenu(float initialAngle, object sender = null)
        {
            menu.ShowMenu();
            DoChangeAngle(initialAngle); // Needed to register initial touch position before the touchpad axis actually changes
        }

        protected virtual void DoHideMenu(bool force, object sender = null)
        {
            menu.StopTouching();
            menu.HideMenu(force);
        }

        protected virtual void DoChangeAngle(float angle, object sender = null)
        {
            currentAngle = angle;

            menu.HoverButton(currentAngle);
        }

        protected virtual void AttemptHapticPulse(float strength)
        {
            if (events)
            {
                VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(events.gameObject), strength);
            }
        }

        #region Private Controller Listeners

        protected virtual void DoTouchpadClicked(object sender, ControllerInteractionEventArgs e)
        {
            DoClickButton();
        }

        protected virtual void DoTouchpadUnclicked(object sender, ControllerInteractionEventArgs e)
        {
            DoUnClickButton();
        }

        protected virtual void DoTouchpadTouched(object sender, ControllerInteractionEventArgs e)
        {
            touchpadTouched = true;
            DoShowMenu(CalculateAngle(e));
        }

        protected virtual void DoTouchpadUntouched(object sender, ControllerInteractionEventArgs e)
        {
            touchpadTouched = false;
            DoHideMenu(false);
        }

        //Touchpad finger moved position
        protected virtual void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if (touchpadTouched)
            {
                DoChangeAngle(CalculateAngle(e));
            }
        }

        #endregion Private Controller Listeners

        protected virtual float CalculateAngle(ControllerInteractionEventArgs e)
        {
            return 360 - e.touchpadAngle;
        }
    }
}