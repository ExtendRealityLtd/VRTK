namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_ControllerEvents_UnityEvents")]
    public sealed class VRTK_ControllerEvents_UnityEvents : VRTK_UnityEvents<VRTK_ControllerEvents>
    {
        [Serializable]
        public sealed class ControllerInteractionEvent : UnityEvent<object, ControllerInteractionEventArgs> { }

        public ControllerInteractionEvent OnTriggerPressed = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnTriggerReleased = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnTriggerTouchStart = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnTriggerTouchEnd = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnTriggerHairlineStart = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnTriggerHairlineEnd = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnTriggerClicked = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnTriggerUnclicked = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnTriggerAxisChanged = new ControllerInteractionEvent();

        public ControllerInteractionEvent OnGripPressed = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnGripReleased = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnGripTouchStart = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnGripTouchEnd = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnGripHairlineStart = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnGripHairlineEnd = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnGripClicked = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnGripUnclicked = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnGripAxisChanged = new ControllerInteractionEvent();

        public ControllerInteractionEvent OnTouchpadPressed = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnTouchpadReleased = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnTouchpadTouchStart = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnTouchpadTouchEnd = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnTouchpadAxisChanged = new ControllerInteractionEvent();

        public ControllerInteractionEvent OnButtonOnePressed = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnButtonOneReleased = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnButtonOneTouchStart = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnButtonOneTouchEnd = new ControllerInteractionEvent();

        public ControllerInteractionEvent OnButtonTwoPressed = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnButtonTwoReleased = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnButtonTwoTouchStart = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnButtonTwoTouchEnd = new ControllerInteractionEvent();

        public ControllerInteractionEvent OnStartMenuPressed = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnStartMenuReleased = new ControllerInteractionEvent();

        public ControllerInteractionEvent OnAliasPointerOn = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnAliasPointerOff = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnAliasPointerSet = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnAliasGrabOn = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnAliasGrabOff = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnAliasUseOn = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnAliasUseOff = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnAliasUIClickOn = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnAliasUIClickOff = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnAliasMenuOn = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnAliasMenuOff = new ControllerInteractionEvent();

        public ControllerInteractionEvent OnControllerEnabled = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnControllerDisabled = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnControllerIndexChanged = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnControllerModelAvailable = new ControllerInteractionEvent();

        public ControllerInteractionEvent OnControllerVisible = new ControllerInteractionEvent();
        public ControllerInteractionEvent OnControllerHidden = new ControllerInteractionEvent();

        protected override void AddListeners(VRTK_ControllerEvents component)
        {
            component.TriggerPressed += TriggerPressed;
            component.TriggerReleased += TriggerReleased;
            component.TriggerTouchStart += TriggerTouchStart;
            component.TriggerTouchEnd += TriggerTouchEnd;
            component.TriggerHairlineStart += TriggerHairlineStart;
            component.TriggerHairlineEnd += TriggerHairlineEnd;
            component.TriggerClicked += TriggerClicked;
            component.TriggerUnclicked += TriggerUnclicked;
            component.TriggerAxisChanged += TriggerAxisChanged;

            component.GripPressed += GripPressed;
            component.GripReleased += GripReleased;
            component.GripTouchStart += GripTouchStart;
            component.GripTouchEnd += GripTouchEnd;
            component.GripHairlineStart += GripHairlineStart;
            component.GripHairlineEnd += GripHairlineEnd;
            component.GripClicked += GripClicked;
            component.GripUnclicked += GripUnclicked;
            component.GripAxisChanged += GripAxisChanged;

            component.TouchpadPressed += TouchpadPressed;
            component.TouchpadReleased += TouchpadReleased;
            component.TouchpadTouchStart += TouchpadTouchStart;
            component.TouchpadTouchEnd += TouchpadTouchEnd;
            component.TouchpadAxisChanged += TouchpadAxisChanged;

            component.ButtonOnePressed += ButtonOnePressed;
            component.ButtonOneReleased += ButtonOneReleased;
            component.ButtonOneTouchStart += ButtonOneTouchStart;
            component.ButtonOneTouchEnd += ButtonOneTouchEnd;

            component.ButtonTwoPressed += ButtonTwoPressed;
            component.ButtonTwoReleased += ButtonTwoReleased;
            component.ButtonTwoTouchStart += ButtonTwoTouchStart;
            component.ButtonTwoTouchEnd += ButtonTwoTouchEnd;

            component.StartMenuPressed += StartMenuPressed;
            component.StartMenuReleased += StartMenuReleased;

            component.ControllerEnabled += ControllerEnabled;
            component.ControllerDisabled += ControllerDisabled;
            component.ControllerIndexChanged += ControllerIndexChanged;
            component.ControllerModelAvailable += ControllerModelAvailable;

            component.ControllerVisible += ControllerVisible;
            component.ControllerHidden += ControllerHidden;
        }

        protected override void RemoveListeners(VRTK_ControllerEvents component)
        {
            component.TriggerPressed -= TriggerPressed;
            component.TriggerReleased -= TriggerReleased;
            component.TriggerTouchStart -= TriggerTouchStart;
            component.TriggerTouchEnd -= TriggerTouchEnd;
            component.TriggerHairlineStart -= TriggerHairlineStart;
            component.TriggerHairlineEnd -= TriggerHairlineEnd;
            component.TriggerClicked -= TriggerClicked;
            component.TriggerUnclicked -= TriggerUnclicked;
            component.TriggerAxisChanged -= TriggerAxisChanged;

            component.GripPressed -= GripPressed;
            component.GripReleased -= GripReleased;
            component.GripTouchStart -= GripTouchStart;
            component.GripTouchEnd -= GripTouchEnd;
            component.GripHairlineStart -= GripHairlineStart;
            component.GripHairlineEnd -= GripHairlineEnd;
            component.GripClicked -= GripClicked;
            component.GripUnclicked -= GripUnclicked;
            component.GripAxisChanged -= GripAxisChanged;

            component.TouchpadPressed -= TouchpadPressed;
            component.TouchpadReleased -= TouchpadReleased;
            component.TouchpadTouchStart -= TouchpadTouchStart;
            component.TouchpadTouchEnd -= TouchpadTouchEnd;
            component.TouchpadAxisChanged -= TouchpadAxisChanged;

            component.ButtonOnePressed -= ButtonOnePressed;
            component.ButtonOneReleased -= ButtonOneReleased;
            component.ButtonOneTouchStart -= ButtonOneTouchStart;
            component.ButtonOneTouchEnd -= ButtonOneTouchEnd;

            component.ButtonTwoPressed -= ButtonTwoPressed;
            component.ButtonTwoReleased -= ButtonTwoReleased;
            component.ButtonTwoTouchStart -= ButtonTwoTouchStart;
            component.ButtonTwoTouchEnd -= ButtonTwoTouchEnd;

            component.StartMenuPressed -= StartMenuPressed;
            component.StartMenuReleased -= StartMenuReleased;

            component.ControllerEnabled -= ControllerEnabled;
            component.ControllerDisabled -= ControllerDisabled;
            component.ControllerIndexChanged -= ControllerIndexChanged;
            component.ControllerModelAvailable -= ControllerModelAvailable;

            component.ControllerVisible -= ControllerVisible;
            component.ControllerHidden -= ControllerHidden;
        }

        private void TriggerPressed(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerPressed.Invoke(o, e);
        }

        private void TriggerReleased(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerReleased.Invoke(o, e);
        }

        private void TriggerTouchStart(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerTouchStart.Invoke(o, e);
        }

        private void TriggerTouchEnd(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerTouchEnd.Invoke(o, e);
        }

        private void TriggerHairlineStart(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerHairlineStart.Invoke(o, e);
        }

        private void TriggerHairlineEnd(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerHairlineEnd.Invoke(o, e);
        }

        private void TriggerClicked(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerClicked.Invoke(o, e);
        }

        private void TriggerUnclicked(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerUnclicked.Invoke(o, e);
        }

        private void TriggerAxisChanged(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerAxisChanged.Invoke(o, e);
        }

        private void GripPressed(object o, ControllerInteractionEventArgs e)
        {
            OnGripPressed.Invoke(o, e);
        }

        private void GripReleased(object o, ControllerInteractionEventArgs e)
        {
            OnGripReleased.Invoke(o, e);
        }

        private void GripTouchStart(object o, ControllerInteractionEventArgs e)
        {
            OnGripTouchStart.Invoke(o, e);
        }

        private void GripTouchEnd(object o, ControllerInteractionEventArgs e)
        {
            OnGripTouchEnd.Invoke(o, e);
        }

        private void GripHairlineStart(object o, ControllerInteractionEventArgs e)
        {
            OnGripHairlineStart.Invoke(o, e);
        }

        private void GripHairlineEnd(object o, ControllerInteractionEventArgs e)
        {
            OnGripHairlineEnd.Invoke(o, e);
        }

        private void GripClicked(object o, ControllerInteractionEventArgs e)
        {
            OnGripClicked.Invoke(o, e);
        }

        private void GripUnclicked(object o, ControllerInteractionEventArgs e)
        {
            OnGripUnclicked.Invoke(o, e);
        }

        private void GripAxisChanged(object o, ControllerInteractionEventArgs e)
        {
            OnGripAxisChanged.Invoke(o, e);
        }

        private void TouchpadPressed(object o, ControllerInteractionEventArgs e)
        {
            OnTouchpadPressed.Invoke(o, e);
        }

        private void TouchpadReleased(object o, ControllerInteractionEventArgs e)
        {
            OnTouchpadReleased.Invoke(o, e);
        }

        private void TouchpadTouchStart(object o, ControllerInteractionEventArgs e)
        {
            OnTouchpadTouchStart.Invoke(o, e);
        }

        private void TouchpadTouchEnd(object o, ControllerInteractionEventArgs e)
        {
            OnTouchpadTouchEnd.Invoke(o, e);
        }

        private void TouchpadAxisChanged(object o, ControllerInteractionEventArgs e)
        {
            OnTouchpadAxisChanged.Invoke(o, e);
        }

        private void ButtonOnePressed(object o, ControllerInteractionEventArgs e)
        {
            OnButtonOnePressed.Invoke(o, e);
        }

        private void ButtonOneReleased(object o, ControllerInteractionEventArgs e)
        {
            OnButtonOneReleased.Invoke(o, e);
        }

        private void ButtonOneTouchStart(object o, ControllerInteractionEventArgs e)
        {
            OnButtonOneTouchStart.Invoke(o, e);
        }

        private void ButtonOneTouchEnd(object o, ControllerInteractionEventArgs e)
        {
            OnButtonOneTouchEnd.Invoke(o, e);
        }

        private void ButtonTwoPressed(object o, ControllerInteractionEventArgs e)
        {
            OnButtonTwoPressed.Invoke(o, e);
        }

        private void ButtonTwoReleased(object o, ControllerInteractionEventArgs e)
        {
            OnButtonTwoReleased.Invoke(o, e);
        }

        private void ButtonTwoTouchStart(object o, ControllerInteractionEventArgs e)
        {
            OnButtonTwoTouchStart.Invoke(o, e);
        }

        private void ButtonTwoTouchEnd(object o, ControllerInteractionEventArgs e)
        {
            OnButtonTwoTouchEnd.Invoke(o, e);
        }

        private void StartMenuPressed(object o, ControllerInteractionEventArgs e)
        {
            OnStartMenuPressed.Invoke(o, e);
        }

        private void StartMenuReleased(object o, ControllerInteractionEventArgs e)
        {
            OnStartMenuReleased.Invoke(o, e);
        }

        private void AliasPointerOn(object o, ControllerInteractionEventArgs e)
        {
            OnAliasPointerOn.Invoke(o, e);
        }

        private void AliasPointerOff(object o, ControllerInteractionEventArgs e)
        {
            OnAliasPointerOff.Invoke(o, e);
        }

        private void AliasPointerSet(object o, ControllerInteractionEventArgs e)
        {
            OnAliasPointerSet.Invoke(o, e);
        }

        private void AliasGrabOn(object o, ControllerInteractionEventArgs e)
        {
            OnAliasGrabOn.Invoke(o, e);
        }

        private void AliasGrabOff(object o, ControllerInteractionEventArgs e)
        {
            OnAliasGrabOff.Invoke(o, e);
        }

        private void AliasUseOn(object o, ControllerInteractionEventArgs e)
        {
            OnAliasUseOn.Invoke(o, e);
        }

        private void AliasUseOff(object o, ControllerInteractionEventArgs e)
        {
            OnAliasUseOff.Invoke(o, e);
        }

        private void AliasUIClickOn(object o, ControllerInteractionEventArgs e)
        {
            OnAliasUIClickOn.Invoke(o, e);
        }

        private void AliasUIClickOff(object o, ControllerInteractionEventArgs e)
        {
            OnAliasUIClickOff.Invoke(o, e);
        }

        private void AliasMenuOn(object o, ControllerInteractionEventArgs e)
        {
            OnAliasMenuOn.Invoke(o, e);
        }

        private void AliasMenuOff(object o, ControllerInteractionEventArgs e)
        {
            OnAliasMenuOff.Invoke(o, e);
        }

        private void ControllerEnabled(object o, ControllerInteractionEventArgs e)
        {
            OnControllerEnabled.Invoke(o, e);
        }

        private void ControllerDisabled(object o, ControllerInteractionEventArgs e)
        {
            OnControllerDisabled.Invoke(o, e);
        }

        private void ControllerIndexChanged(object o, ControllerInteractionEventArgs e)
        {
            OnControllerIndexChanged.Invoke(o, e);
        }

        private void ControllerModelAvailable(object o, ControllerInteractionEventArgs e)
        {
            OnControllerModelAvailable.Invoke(o, e);
        }

        private void ControllerVisible(object o, ControllerInteractionEventArgs e)
        {
            OnControllerVisible.Invoke(o, e);
        }

        private void ControllerHidden(object o, ControllerInteractionEventArgs e)
        {
            OnControllerHidden.Invoke(o, e);
        }
    }
}