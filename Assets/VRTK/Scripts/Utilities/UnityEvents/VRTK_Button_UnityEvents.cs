namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_Button_UnityEvents")]
    public sealed class VRTK_Button_UnityEvents : VRTK_UnityEvents<VRTK_Button>
    {
        [Serializable]
        public sealed class Button3DEvent : UnityEvent<object, Control3DEventArgs> { }

        public Button3DEvent OnPushed = new Button3DEvent();

        protected override void AddListeners(VRTK_Button component)
        {
            component.Pushed += Pushed;
        }

        protected override void RemoveListeners(VRTK_Button component)
        {
            component.Pushed -= Pushed;
        }

        private void Pushed(object o, Control3DEventArgs e)
        {
            OnPushed.Invoke(o, e);
        }
    }
}