namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_InteractControllerAppearance_UnityEvents")]
    public sealed class VRTK_InteractControllerAppearance_UnityEvents : VRTK_UnityEvents<VRTK_InteractControllerAppearance>
    {
        [Serializable]
        public sealed class InteractControllerAppearanceEvent : UnityEvent<object, InteractControllerAppearanceEventArgs> { }

        public InteractControllerAppearanceEvent OnControllerHidden = new InteractControllerAppearanceEvent();
        public InteractControllerAppearanceEvent OnControllerVisible = new InteractControllerAppearanceEvent();
        public InteractControllerAppearanceEvent OnHiddenOnTouch = new InteractControllerAppearanceEvent();
        public InteractControllerAppearanceEvent OnVisibleOnTouch = new InteractControllerAppearanceEvent();
        public InteractControllerAppearanceEvent OnHiddenOnGrab = new InteractControllerAppearanceEvent();
        public InteractControllerAppearanceEvent OnVisibleOnGrab = new InteractControllerAppearanceEvent();
        public InteractControllerAppearanceEvent OnHiddenOnUse = new InteractControllerAppearanceEvent();
        public InteractControllerAppearanceEvent OnVisibleOnUse = new InteractControllerAppearanceEvent();

        protected override void AddListeners(VRTK_InteractControllerAppearance component)
        {
            component.ControllerHidden += ControllerHidden;
            component.ControllerVisible += ControllerVisible;
            component.HiddenOnTouch += HiddenOnTouch;
            component.VisibleOnTouch += VisibleOnTouch;
            component.HiddenOnGrab += HiddenOnGrab;
            component.VisibleOnGrab += VisibleOnGrab;
            component.HiddenOnUse += HiddenOnUse;
            component.VisibleOnUse += VisibleOnUse;
        }

        protected override void RemoveListeners(VRTK_InteractControllerAppearance component)
        {
            component.ControllerHidden -= ControllerHidden;
            component.ControllerVisible -= ControllerVisible;
            component.HiddenOnTouch -= HiddenOnTouch;
            component.VisibleOnTouch -= VisibleOnTouch;
            component.HiddenOnGrab -= HiddenOnGrab;
            component.VisibleOnGrab -= VisibleOnGrab;
            component.HiddenOnUse -= HiddenOnUse;
            component.VisibleOnUse -= VisibleOnUse;
        }

        private void ControllerHidden(object o, InteractControllerAppearanceEventArgs e)
        {
            OnControllerHidden.Invoke(o, e);
        }

        private void ControllerVisible(object o, InteractControllerAppearanceEventArgs e)
        {
            OnControllerVisible.Invoke(o, e);
        }

        private void HiddenOnTouch(object o, InteractControllerAppearanceEventArgs e)
        {
            OnHiddenOnTouch.Invoke(o, e);
        }

        private void VisibleOnTouch(object o, InteractControllerAppearanceEventArgs e)
        {
            OnVisibleOnTouch.Invoke(o, e);
        }

        private void HiddenOnGrab(object o, InteractControllerAppearanceEventArgs e)
        {
            OnHiddenOnGrab.Invoke(o, e);
        }

        private void VisibleOnGrab(object o, InteractControllerAppearanceEventArgs e)
        {
            OnVisibleOnGrab.Invoke(o, e);
        }

        private void HiddenOnUse(object o, InteractControllerAppearanceEventArgs e)
        {
            OnHiddenOnUse.Invoke(o, e);
        }

        private void VisibleOnUse(object o, InteractControllerAppearanceEventArgs e)
        {
            OnVisibleOnUse.Invoke(o, e);
        }
    }
}