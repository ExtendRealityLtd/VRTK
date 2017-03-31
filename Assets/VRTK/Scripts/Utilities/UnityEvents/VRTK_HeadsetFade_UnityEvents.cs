namespace VRTK.UnityEventHelper
{
    using UnityEngine.Events;
    using System;

    public sealed class VRTK_HeadsetFade_UnityEvents : VRTK_UnityEvents<VRTK_HeadsetFade>
    {
        [Serializable]
        public sealed class HeadsetFadeEvent : UnityEvent<object, HeadsetFadeEventArgs> { }

        public HeadsetFadeEvent OnHeadsetFadeStart = new HeadsetFadeEvent();
        public HeadsetFadeEvent OnHeadsetFadeComplete = new HeadsetFadeEvent();

        public HeadsetFadeEvent OnHeadsetUnfadeStart = new HeadsetFadeEvent();
        public HeadsetFadeEvent OnHeadsetUnfadeComplete = new HeadsetFadeEvent();

        protected override void AddListeners(VRTK_HeadsetFade component)
        {
            component.HeadsetFadeStart += HeadsetFadeStart;
            component.HeadsetFadeComplete += HeadsetFadeComplete;

            component.HeadsetUnfadeStart += HeadsetUnfadeStart;
            component.HeadsetUnfadeComplete += HeadsetUnfadeComplete;
        }

        protected override void RemoveListeners(VRTK_HeadsetFade component)
        {
            component.HeadsetFadeStart -= HeadsetFadeStart;
            component.HeadsetFadeComplete -= HeadsetFadeComplete;

            component.HeadsetUnfadeStart -= HeadsetUnfadeStart;
            component.HeadsetUnfadeComplete -= HeadsetUnfadeComplete;
        }

        private void HeadsetFadeStart(object o, HeadsetFadeEventArgs e)
        {
            OnHeadsetFadeStart.Invoke(o, e);
        }

        private void HeadsetFadeComplete(object o, HeadsetFadeEventArgs e)
        {
            OnHeadsetFadeComplete.Invoke(o, e);
        }

        private void HeadsetUnfadeStart(object o, HeadsetFadeEventArgs e)
        {
            OnHeadsetUnfadeStart.Invoke(o, e);
        }

        private void HeadsetUnfadeComplete(object o, HeadsetFadeEventArgs e)
        {
            OnHeadsetUnfadeComplete.Invoke(o, e);
        }
    }
}