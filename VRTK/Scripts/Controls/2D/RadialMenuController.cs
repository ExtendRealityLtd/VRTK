namespace VRTK
{
    using UnityEngine;

    // Radial Menu input from Vive Controller
    [RequireComponent(typeof(RadialMenu))]
    public class RadialMenuController : MonoBehaviour
    {
        public VRTK_ControllerEvents events;

        protected RadialMenu menu;
        private float currentAngle; //Keep track of angle for when we click
        private bool touchpadTouched;

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
                Debug.LogError("The radial menu must be a child of the controller or be set in the inspector!");
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

        protected void DoClickButton(object sender = null) // The optional argument reduces the need for middleman functions in subclasses whose events likely pass object sender
        {
            menu.ClickButton(currentAngle);
        }

        protected void DoUnClickButton(object sender = null)
        {
            menu.UnClickButton(currentAngle);
        }

        protected void DoShowMenu(float initialAngle, object sender = null)
        {
            menu.ShowMenu();
            DoChangeAngle(initialAngle); // Needed to register initial touch position before the touchpad axis actually changes
        }

        protected void DoHideMenu(bool force, object sender = null)
        {
            menu.StopTouching();
            menu.HideMenu(force);
        }

        protected void DoChangeAngle(float angle, object sender = null)
        {
            currentAngle = angle;

            menu.HoverButton(currentAngle);
        }

        protected virtual void AttemptHapticPulse(float strength)
        {
            var controllerActions = GetComponentInParent<VRTK_ControllerActions>();
            if (controllerActions)
            {
                controllerActions.TriggerHapticPulse(strength);
            }
        }

        #region Private Controller Listeners

        private void DoTouchpadClicked(object sender, ControllerInteractionEventArgs e)
        {
            DoClickButton();
        }

        private void DoTouchpadUnclicked(object sender, ControllerInteractionEventArgs e)
        {
            DoUnClickButton();
        }

        private void DoTouchpadTouched(object sender, ControllerInteractionEventArgs e)
        {
            touchpadTouched = true;
            DoShowMenu(CalculateAngle(e));
        }

        private void DoTouchpadUntouched(object sender, ControllerInteractionEventArgs e)
        {
            touchpadTouched = false;
            DoHideMenu(false);
        }

        //Touchpad finger moved position
        private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if (touchpadTouched)
            {
                DoChangeAngle(CalculateAngle(e));
            }
        }

        #endregion Private Controller Listeners

        private float CalculateAngle(ControllerInteractionEventArgs e)
        {
            return 360 - e.touchpadAngle;
        }
    }
}