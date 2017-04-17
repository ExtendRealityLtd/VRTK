namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_HeadsetControllerAware_UnityEvents")]
    public sealed class VRTK_HeadsetControllerAware_UnityEvents : VRTK_UnityEvents<VRTK_HeadsetControllerAware>
    {
        [Serializable]
        public sealed class HeadsetControllerAwareEvent : UnityEvent<object, HeadsetControllerAwareEventArgs> { }

        public HeadsetControllerAwareEvent OnControllerObscured = new HeadsetControllerAwareEvent();
        public HeadsetControllerAwareEvent OnControllerUnobscured = new HeadsetControllerAwareEvent();

        public HeadsetControllerAwareEvent OnControllerGlanceEnter = new HeadsetControllerAwareEvent();
        public HeadsetControllerAwareEvent OnControllerGlanceExit = new HeadsetControllerAwareEvent();

        protected override void AddListeners(VRTK_HeadsetControllerAware component)
        {
            component.ControllerObscured += ControllerObscured;
            component.ControllerUnobscured += ControllerUnobscured;

            component.ControllerGlanceEnter += ControllerGlanceEnter;
            component.ControllerGlanceExit += ControllerGlanceExit;
        }

        protected override void RemoveListeners(VRTK_HeadsetControllerAware component)
        {
            component.ControllerObscured -= ControllerObscured;
            component.ControllerUnobscured -= ControllerUnobscured;

            component.ControllerGlanceEnter -= ControllerGlanceEnter;
            component.ControllerGlanceExit -= ControllerGlanceExit;
        }

        private void ControllerObscured(object o, HeadsetControllerAwareEventArgs e)
        {
            OnControllerObscured.Invoke(o, e);
        }

        private void ControllerUnobscured(object o, HeadsetControllerAwareEventArgs e)
        {
            OnControllerUnobscured.Invoke(o, e);
        }

        private void ControllerGlanceEnter(object o, HeadsetControllerAwareEventArgs e)
        {
            OnControllerGlanceEnter.Invoke(o, e);
        }

        private void ControllerGlanceExit(object o, HeadsetControllerAwareEventArgs e)
        {
            OnControllerGlanceExit.Invoke(o, e);
        }
    }
}