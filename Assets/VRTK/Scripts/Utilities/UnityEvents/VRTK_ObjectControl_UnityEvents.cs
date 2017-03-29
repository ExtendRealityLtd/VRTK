namespace VRTK.UnityEventHelper
{
    using UnityEngine.Events;
    using System;

    public sealed class VRTK_ObjectControl_UnityEvents : VRTK_UnityEvents<VRTK_ObjectControl>
    {
        [Serializable]
        public sealed class ObjectControlEvent : UnityEvent<object, ObjectControlEventArgs> { }

        public ObjectControlEvent OnXAxisChanged = new ObjectControlEvent();
        public ObjectControlEvent OnYAxisChanged = new ObjectControlEvent();

        protected override void AddListeners(VRTK_ObjectControl component)
        {
            component.XAxisChanged += XAxisChanged;
            component.YAxisChanged += YAxisChanged;
        }

        protected override void RemoveListeners(VRTK_ObjectControl component)
        {
            component.XAxisChanged -= XAxisChanged;
            component.YAxisChanged -= YAxisChanged;
        }

        private void XAxisChanged(object o, ObjectControlEventArgs e)
        {
            OnXAxisChanged.Invoke(o, e);
        }

        private void YAxisChanged(object o, ObjectControlEventArgs e)
        {
            OnYAxisChanged.Invoke(o, e);
        }
    }
}