namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using VRTK.GrabAttachMechanics;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_ControlAnimationGrabAttach_UnityEvents")]
    public sealed class VRTK_ControlAnimationGrabAttach_UnityEvents : VRTK_UnityEvents<VRTK_ControlAnimationGrabAttach>
    {
        [Serializable]
        public sealed class ControlAnimationGrabAttachEvent : UnityEvent<object, ControlAnimationGrabAttachEventArgs> { }

        public ControlAnimationGrabAttachEvent OnAnimationFrameAtStart = new ControlAnimationGrabAttachEvent();
        public ControlAnimationGrabAttachEvent OnAnimationFrameAtEnd = new ControlAnimationGrabAttachEvent();
        public ControlAnimationGrabAttachEvent OnAnimationFrameChanged = new ControlAnimationGrabAttachEvent();

        protected override void AddListeners(VRTK_ControlAnimationGrabAttach component)
        {
            component.AnimationFrameChanged += AnimationFrameChanged;
            component.AnimationFrameAtStart += AnimationFrameAtStart;
            component.AnimationFrameAtEnd += AnimationFrameAtEnd;
        }

        protected override void RemoveListeners(VRTK_ControlAnimationGrabAttach component)
        {
            component.AnimationFrameChanged -= AnimationFrameChanged;
            component.AnimationFrameAtStart -= AnimationFrameAtStart;
            component.AnimationFrameAtEnd -= AnimationFrameAtEnd;
        }

        private void AnimationFrameAtStart(object o, ControlAnimationGrabAttachEventArgs e)
        {
            OnAnimationFrameAtStart.Invoke(o, e);
        }

        private void AnimationFrameAtEnd(object o, ControlAnimationGrabAttachEventArgs e)
        {
            OnAnimationFrameAtEnd.Invoke(o, e);
        }

        private void AnimationFrameChanged(object o, ControlAnimationGrabAttachEventArgs e)
        {
            OnAnimationFrameChanged.Invoke(o, e);
        }
    }
}