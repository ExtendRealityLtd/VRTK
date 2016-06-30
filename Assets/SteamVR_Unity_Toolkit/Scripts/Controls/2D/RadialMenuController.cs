namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    // Radial Menu input from Vive Controller
    [RequireComponent (typeof(RadialMenu))]
    public class RadialMenuController : MonoBehaviour {

        RadialMenu menu;
        public VRTK_ControllerEvents events;

        void Start () {
            menu = GetComponent<RadialMenu>();
            if(events == null)
            {
                events = GetComponentInParent<VRTK_ControllerEvents>();
            }
            if (events == null)
            {
                Debug.LogError("The radial menu must be a child of the controller or be set in the inspector!");
            }
            else
            {   //Start listening for controller events
                events.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadClicked);
                events.TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadUnclicked);
                events.TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouched);
                events.TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadUntouched);
                events.TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
            }
        }

        float currentAngle; //Keep track of angle for when we click

        void DoTouchpadClicked(object sender, ControllerInteractionEventArgs e)
        {
            menu.ClickButton(currentAngle);
        }

        void DoTouchpadUnclicked(object sender, ControllerInteractionEventArgs e)
        {
            menu.UnClickButton(currentAngle);
        }

        void DoTouchpadTouched(object sender, ControllerInteractionEventArgs e)
        {
            menu.ShowMenu();
        }

        void DoTouchpadUntouched(object sender, ControllerInteractionEventArgs e)
        {
            menu.StopTouching();
            menu.HideMenu(false);
        }

        //Touchpad finger moved position
        void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            //Convert Touchpad Vector2 to Angle (0 to 360)
            float angle = Mathf.Atan2(e.touchpadAxis.y, e.touchpadAxis.x) * Mathf.Rad2Deg;
            angle = 90.0f - angle;
            if (angle < 0)
            {
                angle += 360.0f;
            }    
            currentAngle = 360 - angle;

            menu.HoverButton(currentAngle);
        }

    }
}
