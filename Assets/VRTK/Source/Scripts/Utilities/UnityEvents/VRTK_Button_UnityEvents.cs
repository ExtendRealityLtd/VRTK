namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_Button_UnityEvents")]
    [Obsolete("`VRTK_Button_UnityEvents` has been deprecated and can be recreated with `VRTK_BaseControllable_UnityEvents`. This script will be removed in a future version of VRTK.")]
    public sealed class VRTK_Button_UnityEvents : VRTK_UnityEvents<VRTK_Button>
    {
        [Serializable]
        public sealed class Button3DEvent : UnityEvent<object, Control3DEventArgs> { }

        public Button3DEvent OnPushed = new Button3DEvent();
        public Button3DEvent OnReleased = new Button3DEvent();

        protected override void AddListeners(VRTK_Button component)
        {
            component.Pushed += Pushed;
            component.Released += Released;
        }

        protected override void RemoveListeners(VRTK_Button component)
        {
            component.Pushed -= Pushed;
            component.Released -= Released;
        }

        private void Pushed(object o, Control3DEventArgs e)
        {
            OnPushed.Invoke(o, e);
        }

        public void Released(object o, Control3DEventArgs e)
        {
            OnReleased.Invoke(o, e);
        }
    }
}