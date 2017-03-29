namespace VRTK.UnityEventHelper
{
    using UnityEngine.Events;
    using System;

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