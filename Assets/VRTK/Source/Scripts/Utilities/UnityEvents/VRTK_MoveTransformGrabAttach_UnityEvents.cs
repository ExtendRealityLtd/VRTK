namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using VRTK.GrabAttachMechanics;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_MoveTransformGrabAttach_UnityEvents")]
    public sealed class VRTK_MoveTransformGrabAttach_UnityEvents : VRTK_UnityEvents<VRTK_MoveTransformGrabAttach>
    {
        [Serializable]
        public sealed class MoveTransformGrabAttachEvent : UnityEvent<object, MoveTransformGrabAttachEventArgs> { }

        public MoveTransformGrabAttachEvent OnTransformPositionChanged = new MoveTransformGrabAttachEvent();
        public MoveTransformGrabAttachEvent OnXAxisMinLimitReached = new MoveTransformGrabAttachEvent();
        public MoveTransformGrabAttachEvent OnXAxisMinLimitExited = new MoveTransformGrabAttachEvent();
        public MoveTransformGrabAttachEvent OnXAxisMaxLimitReached = new MoveTransformGrabAttachEvent();
        public MoveTransformGrabAttachEvent OnXAxisMaxLimitExited = new MoveTransformGrabAttachEvent();
        public MoveTransformGrabAttachEvent OnYAxisMinLimitReached = new MoveTransformGrabAttachEvent();
        public MoveTransformGrabAttachEvent OnYAxisMinLimitExited = new MoveTransformGrabAttachEvent();
        public MoveTransformGrabAttachEvent OnYAxisMaxLimitReached = new MoveTransformGrabAttachEvent();
        public MoveTransformGrabAttachEvent OnYAxisMaxLimitExited = new MoveTransformGrabAttachEvent();
        public MoveTransformGrabAttachEvent OnZAxisMinLimitReached = new MoveTransformGrabAttachEvent();
        public MoveTransformGrabAttachEvent OnZAxisMinLimitExited = new MoveTransformGrabAttachEvent();
        public MoveTransformGrabAttachEvent OnZAxisMaxLimitReached = new MoveTransformGrabAttachEvent();
        public MoveTransformGrabAttachEvent OnZAxisMaxLimitExited = new MoveTransformGrabAttachEvent();

        protected override void AddListeners(VRTK_MoveTransformGrabAttach component)
        {
            component.TransformPositionChanged += TransformPositionChanged;
            component.XAxisMinLimitReached += XAxisMinLimitReached;
            component.XAxisMaxLimitReached += XAxisMaxLimitReached;
            component.YAxisMinLimitReached += YAxisMinLimitReached;
            component.YAxisMaxLimitReached += YAxisMaxLimitReached;
            component.ZAxisMinLimitReached += ZAxisMinLimitReached;
            component.ZAxisMaxLimitReached += ZAxisMaxLimitReached;
            component.XAxisMinLimitExited += XAxisMinLimitExited;
            component.XAxisMaxLimitExited += XAxisMaxLimitExited;
            component.YAxisMinLimitExited += YAxisMinLimitExited;
            component.YAxisMaxLimitExited += YAxisMaxLimitExited;
            component.ZAxisMinLimitExited += ZAxisMinLimitExited;
            component.ZAxisMaxLimitExited += ZAxisMaxLimitExited;
        }

        protected override void RemoveListeners(VRTK_MoveTransformGrabAttach component)
        {
            component.TransformPositionChanged -= TransformPositionChanged;
            component.XAxisMinLimitReached -= XAxisMinLimitReached;
            component.XAxisMaxLimitReached -= XAxisMaxLimitReached;
            component.YAxisMinLimitReached -= YAxisMinLimitReached;
            component.YAxisMaxLimitReached -= YAxisMaxLimitReached;
            component.ZAxisMinLimitReached -= ZAxisMinLimitReached;
            component.ZAxisMaxLimitReached -= ZAxisMaxLimitReached;
            component.XAxisMinLimitExited -= XAxisMinLimitExited;
            component.XAxisMaxLimitExited -= XAxisMaxLimitExited;
            component.YAxisMinLimitExited -= YAxisMinLimitExited;
            component.YAxisMaxLimitExited -= YAxisMaxLimitExited;
            component.ZAxisMinLimitExited -= ZAxisMinLimitExited;
            component.ZAxisMaxLimitExited -= ZAxisMaxLimitExited;
        }

        private void TransformPositionChanged(object o, MoveTransformGrabAttachEventArgs e)
        {
            OnTransformPositionChanged.Invoke(o, e);
        }

        private void XAxisMinLimitReached(object o, MoveTransformGrabAttachEventArgs e)
        {
            OnXAxisMinLimitReached.Invoke(o, e);
        }

        private void XAxisMaxLimitReached(object o, MoveTransformGrabAttachEventArgs e)
        {
            OnXAxisMaxLimitReached.Invoke(o, e);
        }

        private void YAxisMinLimitReached(object o, MoveTransformGrabAttachEventArgs e)
        {
            OnYAxisMinLimitReached.Invoke(o, e);
        }

        private void YAxisMaxLimitReached(object o, MoveTransformGrabAttachEventArgs e)
        {
            OnYAxisMaxLimitReached.Invoke(o, e);
        }

        private void ZAxisMinLimitReached(object o, MoveTransformGrabAttachEventArgs e)
        {
            OnZAxisMinLimitReached.Invoke(o, e);
        }

        private void ZAxisMaxLimitReached(object o, MoveTransformGrabAttachEventArgs e)
        {
            OnZAxisMaxLimitReached.Invoke(o, e);
        }

        private void XAxisMinLimitExited(object o, MoveTransformGrabAttachEventArgs e)
        {
            OnXAxisMinLimitExited.Invoke(o, e);
        }

        private void XAxisMaxLimitExited(object o, MoveTransformGrabAttachEventArgs e)
        {
            OnXAxisMaxLimitExited.Invoke(o, e);
        }

        private void YAxisMinLimitExited(object o, MoveTransformGrabAttachEventArgs e)
        {
            OnYAxisMinLimitExited.Invoke(o, e);
        }

        private void YAxisMaxLimitExited(object o, MoveTransformGrabAttachEventArgs e)
        {
            OnYAxisMaxLimitExited.Invoke(o, e);
        }

        private void ZAxisMinLimitExited(object o, MoveTransformGrabAttachEventArgs e)
        {
            OnZAxisMinLimitExited.Invoke(o, e);
        }

        private void ZAxisMaxLimitExited(object o, MoveTransformGrabAttachEventArgs e)
        {
            OnZAxisMaxLimitExited.Invoke(o, e);
        }
    }
}