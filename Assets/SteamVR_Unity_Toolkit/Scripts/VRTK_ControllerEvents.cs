namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public struct ControllerInteractionEventArgs
    {
        public uint controllerIndex;
        public float buttonPressure;
        public Vector2 touchpadAxis;
        public float touchpadAngle;
    }

    public delegate void ControllerInteractionEventHandler(object sender, ControllerInteractionEventArgs e);

    public class VRTK_ControllerEvents : MonoBehaviour
    {
        public enum ButtonAlias
        {
            Trigger,
            Grip,
            Touchpad_Touch,
            Touchpad_Press,
            Application_Menu
        }

        public ButtonAlias pointerToggleButton = ButtonAlias.Touchpad_Press;
        public ButtonAlias grabToggleButton = ButtonAlias.Grip;
        public ButtonAlias useToggleButton = ButtonAlias.Trigger;
        public ButtonAlias menuToggleButton = ButtonAlias.Application_Menu;

        public int axisFidelity = 1;

        public bool triggerPressed = false;
        public bool triggerAxisChanged = false;
        public bool applicationMenuPressed = false;
        public bool touchpadPressed = false;
        public bool touchpadTouched = false;
        public bool touchpadAxisChanged = false;
        public bool gripPressed = false;

        public bool pointerPressed = false;
        public bool grabPressed = false;
        public bool usePressed = false;
        public bool menuPressed = false;

        public event ControllerInteractionEventHandler TriggerPressed;
        public event ControllerInteractionEventHandler TriggerReleased;

        public event ControllerInteractionEventHandler TriggerAxisChanged;

        public event ControllerInteractionEventHandler ApplicationMenuPressed;
        public event ControllerInteractionEventHandler ApplicationMenuReleased;

        public event ControllerInteractionEventHandler GripPressed;
        public event ControllerInteractionEventHandler GripReleased;

        public event ControllerInteractionEventHandler TouchpadPressed;
        public event ControllerInteractionEventHandler TouchpadReleased;

        public event ControllerInteractionEventHandler TouchpadTouchStart;
        public event ControllerInteractionEventHandler TouchpadTouchEnd;

        public event ControllerInteractionEventHandler TouchpadAxisChanged;

        public event ControllerInteractionEventHandler AliasPointerOn;
        public event ControllerInteractionEventHandler AliasPointerOff;

        public event ControllerInteractionEventHandler AliasGrabOn;
        public event ControllerInteractionEventHandler AliasGrabOff;

        public event ControllerInteractionEventHandler AliasUseOn;
        public event ControllerInteractionEventHandler AliasUseOff;

        public event ControllerInteractionEventHandler AliasMenuOn;
        public event ControllerInteractionEventHandler AliasMenuOff;

        private uint controllerIndex;
        private SteamVR_TrackedObject trackedController;
        private SteamVR_Controller.Device device;

        private Vector2 touchpadAxis = Vector2.zero;
        private Vector2 triggerAxis = Vector2.zero;

        private Vector3 controllerVelocity = Vector3.zero;
        private Vector3 controllerAngularVelocity = Vector3.zero;

        public virtual void OnTriggerPressed(ControllerInteractionEventArgs e)
        {
            if (TriggerPressed != null)
                TriggerPressed(this, e);
        }

        public virtual void OnTriggerReleased(ControllerInteractionEventArgs e)
        {
            if (TriggerReleased != null)
                TriggerReleased(this, e);
        }

        public virtual void OnTriggerAxisChanged(ControllerInteractionEventArgs e)
        {
            if (TriggerAxisChanged != null)
                TriggerAxisChanged(this, e);
        }

        public virtual void OnApplicationMenuPressed(ControllerInteractionEventArgs e)
        {
            if (ApplicationMenuPressed != null)
                ApplicationMenuPressed(this, e);
        }

        public virtual void OnApplicationMenuReleased(ControllerInteractionEventArgs e)
        {
            if (ApplicationMenuReleased != null)
                ApplicationMenuReleased(this, e);
        }

        public virtual void OnGripPressed(ControllerInteractionEventArgs e)
        {
            if (GripPressed != null)
                GripPressed(this, e);
        }

        public virtual void OnGripReleased(ControllerInteractionEventArgs e)
        {
            if (GripReleased != null)
                GripReleased(this, e);
        }

        public virtual void OnTouchpadPressed(ControllerInteractionEventArgs e)
        {
            if (TouchpadPressed != null)
                TouchpadPressed(this, e);
        }

        public virtual void OnTouchpadReleased(ControllerInteractionEventArgs e)
        {
            if (TouchpadReleased != null)
                TouchpadReleased(this, e);
        }

        public virtual void OnTouchpadTouchStart(ControllerInteractionEventArgs e)
        {
            if (TouchpadTouchStart != null)
                TouchpadTouchStart(this, e);
        }

        public virtual void OnTouchpadTouchEnd(ControllerInteractionEventArgs e)
        {
            if (TouchpadTouchEnd != null)
                TouchpadTouchEnd(this, e);
        }

        public virtual void OnTouchpadAxisChanged(ControllerInteractionEventArgs e)
        {
            if (TouchpadAxisChanged != null)
                TouchpadAxisChanged(this, e);
        }

        public virtual void OnAliasPointerOn(ControllerInteractionEventArgs e)
        {
            if (AliasPointerOn != null)
                AliasPointerOn(this, e);
        }

        public virtual void OnAliasPointerOff(ControllerInteractionEventArgs e)
        {
            if (AliasPointerOff != null)
                AliasPointerOff(this, e);
        }

        public virtual void OnAliasGrabOn(ControllerInteractionEventArgs e)
        {
            if (AliasGrabOn != null)
                AliasGrabOn(this, e);
        }

        public virtual void OnAliasGrabOff(ControllerInteractionEventArgs e)
        {
            if (AliasGrabOff != null)
                AliasGrabOff(this, e);
        }

        public virtual void OnAliasUseOn(ControllerInteractionEventArgs e)
        {
            if (AliasUseOn != null)
                AliasUseOn(this, e);
        }

        public virtual void OnAliasUseOff(ControllerInteractionEventArgs e)
        {
            if (AliasUseOff != null)
                AliasUseOff(this, e);
        }

        public virtual void OnAliasMenuOn(ControllerInteractionEventArgs e)
        {
            if (AliasMenuOn != null)
                AliasMenuOn(this, e);
        }

        public virtual void OnAliasMenuOff(ControllerInteractionEventArgs e)
        {
            if (AliasMenuOff != null)
                AliasMenuOff(this, e);
        }

        public Vector3 GetVelocity()
        {
            SetVelocity();
            return controllerVelocity;
        }

        public Vector3 GetAngularVelocity()
        {
            SetVelocity();
            return controllerAngularVelocity;
        }

        public Vector2 GetTouchpadAxis()
        {
            return touchpadAxis;
        }

        public float GetTouchpadAxisAngle()
        {
            return CalculateTouchpadAxisAngle(touchpadAxis);
        }

        private ControllerInteractionEventArgs SetButtonEvent(ref bool buttonBool, bool value, float buttonPressure)
        {
            buttonBool = value;
            ControllerInteractionEventArgs e;
            e.controllerIndex = controllerIndex;
            e.buttonPressure = buttonPressure;
            e.touchpadAxis = device.GetAxis();
            e.touchpadAngle = CalculateTouchpadAxisAngle(e.touchpadAxis);

            return e;
        }

        private void Awake()
        {
            trackedController = GetComponent<SteamVR_TrackedObject>();
            this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        private void Start()
        {
            controllerIndex = (uint)trackedController.index;
            device = SteamVR_Controller.Input((int)controllerIndex);
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

        private void Update()
        {
            controllerIndex = (uint)trackedController.index;
            device = SteamVR_Controller.Input((int)controllerIndex);

            Vector2 currentTriggerAxis = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
            Vector2 currentTouchpadAxis = device.GetAxis();

            //Trigger
            if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                OnTriggerPressed(SetButtonEvent(ref triggerPressed, true, currentTriggerAxis.x));
                EmitAlias(ButtonAlias.Trigger, true, currentTriggerAxis.x, ref triggerPressed);
            }
            else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                OnTriggerReleased(SetButtonEvent(ref triggerPressed, false, 0f));
                EmitAlias(ButtonAlias.Trigger, false, 0f, ref triggerPressed);
            }
            else
            {
                if (Vector2ShallowEquals(triggerAxis, currentTriggerAxis))
                {
                    triggerAxisChanged = false;
                }
                else
                {
                    OnTriggerAxisChanged(SetButtonEvent(ref triggerAxisChanged, true, currentTriggerAxis.x));
                }
            }

            //ApplicationMenu
            if (device.GetTouchDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
            {
                OnApplicationMenuPressed(SetButtonEvent(ref applicationMenuPressed, true, 1f));
                EmitAlias(ButtonAlias.Application_Menu, true, 1f, ref applicationMenuPressed);
            }
            else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.ApplicationMenu))
            {

                OnApplicationMenuReleased(SetButtonEvent(ref applicationMenuPressed, false, 0f));
                EmitAlias(ButtonAlias.Application_Menu, false, 0f, ref applicationMenuPressed);
            }

            //Grip
            if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Grip))
            {
                OnGripPressed(SetButtonEvent(ref gripPressed, true, 1f));
                EmitAlias(ButtonAlias.Grip, true, 1f, ref gripPressed);
            }
            else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Grip))
            {
                OnGripReleased(SetButtonEvent(ref gripPressed, false, 0f));
                EmitAlias(ButtonAlias.Grip, false, 0f, ref gripPressed);
            }

            //Touchpad Pressed
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                OnTouchpadPressed(SetButtonEvent(ref touchpadPressed, true, 1f));
                EmitAlias(ButtonAlias.Touchpad_Press, true, 1f, ref touchpadPressed);
            }
            else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
                OnTouchpadReleased(SetButtonEvent(ref touchpadPressed, false, 0f));
                EmitAlias(ButtonAlias.Touchpad_Press, false, 0f, ref touchpadPressed);
            }

            //Touchpad Touched
            if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                OnTouchpadTouchStart(SetButtonEvent(ref touchpadTouched, true, 1f));
                EmitAlias(ButtonAlias.Touchpad_Touch, true, 1f, ref touchpadTouched);
            }
            else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
                OnTouchpadTouchEnd(SetButtonEvent(ref touchpadTouched, false, 0f));
                EmitAlias(ButtonAlias.Touchpad_Touch, false, 0f, ref touchpadTouched);
            }
            else
            {
                if (Vector2ShallowEquals(touchpadAxis, currentTouchpadAxis))
                {
                    touchpadAxisChanged = false;
                }
                else {
                    OnTouchpadAxisChanged(SetButtonEvent(ref touchpadTouched, true, 1f));
                    touchpadAxisChanged = true;
                }
            }

            // Save current touch and trigger settings to detect next change.
            touchpadAxis = new Vector2(currentTouchpadAxis.x, currentTouchpadAxis.y);
            triggerAxis = new Vector2(currentTriggerAxis.x, currentTriggerAxis.y);
        }

        private void SetVelocity()
        {
            var origin = trackedController.origin ? trackedController.origin : trackedController.transform.parent;
            if (origin != null)
            {
                controllerVelocity = origin.TransformDirection(device.velocity);
                controllerAngularVelocity = origin.TransformDirection(device.angularVelocity);
            }
            else
            {
                controllerVelocity = device.velocity;
                controllerAngularVelocity = device.angularVelocity;
            }
        }
    }
}