namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using VRTK.GrabAttachMechanics;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_RotateTransformGrabAttach_UnityEvents")]
    public sealed class VRTK_RotateTransformGrabAttach_UnityEvents : VRTK_UnityEvents<VRTK_RotateTransformGrabAttach>
    {
        [Serializable]
        public sealed class RotateTransformGrabAttachEvent : UnityEvent<object, RotateTransformGrabAttachEventArgs> { }

        public RotateTransformGrabAttachEvent OnAngleChanged = new RotateTransformGrabAttachEvent();
        public RotateTransformGrabAttachEvent OnMinAngleReached = new RotateTransformGrabAttachEvent();
        public RotateTransformGrabAttachEvent OnMinAngleExited = new RotateTransformGrabAttachEvent();
        public RotateTransformGrabAttachEvent OnMaxAngleReached = new RotateTransformGrabAttachEvent();
        public RotateTransformGrabAttachEvent OnMaxAngleExited = new RotateTransformGrabAttachEvent();

        protected override void AddListeners(VRTK_RotateTransformGrabAttach component)
        {
            component.AngleChanged += AngleChanged;
            component.MinAngleReached += MinAngleReached;
            component.MinAngleExited += MinAngleExited;
            component.MaxAngleReached += MaxAngleReached;
            component.MaxAngleExited += MaxAngleExited;
        }

        protected override void RemoveListeners(VRTK_RotateTransformGrabAttach component)
        {
            component.AngleChanged -= AngleChanged;
            component.MinAngleReached -= MinAngleReached;
            component.MinAngleExited -= MinAngleExited;
            component.MaxAngleReached -= MaxAngleReached;
            component.MaxAngleExited -= MaxAngleExited;
        }

        private void AngleChanged(object o, RotateTransformGrabAttachEventArgs e)
        {
            OnAngleChanged.Invoke(o, e);
        }

        private void MinAngleReached(object o, RotateTransformGrabAttachEventArgs e)
        {
            OnMinAngleReached.Invoke(o, e);
        }

        private void MinAngleExited(object o, RotateTransformGrabAttachEventArgs e)
        {
            OnMinAngleExited.Invoke(o, e);
        }

        private void MaxAngleReached(object o, RotateTransformGrabAttachEventArgs e)
        {
            OnMaxAngleReached.Invoke(o, e);
        }

        private void MaxAngleExited(object o, RotateTransformGrabAttachEventArgs e)
        {
            OnMaxAngleExited.Invoke(o, e);
        }
    }
}