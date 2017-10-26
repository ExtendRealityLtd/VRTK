namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_Control_UnityEvents")]
    [Obsolete("`VRTK_Control_UnityEvents` has been deprecated and can be recreated with `VRTK_BaseControllable_UnityEvents`. This script will be removed in a future version of VRTK.")]
    public sealed class VRTK_Control_UnityEvents : VRTK_UnityEvents<VRTK_Control>
    {
        [Serializable]
        public sealed class Control3DEvent : UnityEvent<object, Control3DEventArgs> { }

        public Control3DEvent OnValueChanged = new Control3DEvent();

        protected override void AddListeners(VRTK_Control component)
        {
            component.ValueChanged += ValueChanged;
        }

        protected override void RemoveListeners(VRTK_Control component)
        {
            component.ValueChanged -= ValueChanged;
        }

        private void ValueChanged(object o, Control3DEventArgs e)
        {
            OnValueChanged.Invoke(o, e);
        }
    }
}