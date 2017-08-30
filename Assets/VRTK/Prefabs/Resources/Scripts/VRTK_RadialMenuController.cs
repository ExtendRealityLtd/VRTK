// Radial Menu Controller|Prefabs|0045
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Allows the RadialMenu to be anchored to controller object.
    /// </summary>
    /// <remarks>
    ///  * Place the `VRTK/Prefabs/RadialMenu` prefab as a child of a Controller script alias GameObject.
    ///  * The Radial Menu should automatically find the `VRTK_ControllerEvents` component in the parent hierarchy, if one is not found but required then the `Events` parameter on the `VRTK_RadialMenuController` script can be populated.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/030_Controls_RadialTouchpadMenu` displays a radial menu for each controller. The left controller uses the `Hide On Release` variable, so it will only be visible if the left touchpad is being touched. It also uses the `Execute On Unclick` variable to delay execution until the touchpad button is unclicked. The example scene also contains a demonstration of anchoring the RadialMenu to an interactable cube instead of a controller.
    /// </example>
    [RequireComponent(typeof(VRTK_RadialMenu))]
    public class VRTK_RadialMenuController : MonoBehaviour
    {
        [Tooltip("The controller to listen to the controller events on.")]
        public VRTK_ControllerEvents events;

        protected VRTK_RadialMenu menu;
        protected float currentAngle; //Keep track of angle for when we click
        protected bool touchpadTouched;

        protected virtual void Awake()
        {
            menu = GetComponent<VRTK_RadialMenu>();

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
                VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(events.gameObject), strength);
            }
        }

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

        protected virtual float CalculateAngle(ControllerInteractionEventArgs e)
        {
            return 360 - e.touchpadAngle;
        }
    }
}