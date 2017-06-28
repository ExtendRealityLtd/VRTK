namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_PlayAreaCursor_UnityEvents")]
    public sealed class VRTK_PlayAreaCursor_UnityEvents : VRTK_UnityEvents<VRTK_PlayAreaCursor>
    {
        [Serializable]
        public sealed class PlayAreaCursorEvent : UnityEvent<object, PlayAreaCursorEventArgs> { }

        public PlayAreaCursorEvent OnPlayAreaCursorStartCollision = new PlayAreaCursorEvent();
        public PlayAreaCursorEvent OnPlayAreaCursorEndCollision = new PlayAreaCursorEvent();

        protected override void AddListeners(VRTK_PlayAreaCursor component)
        {
            component.PlayAreaCursorStartCollision += PlayAreaCursorStartCollision;
            component.PlayAreaCursorEndCollision += PlayAreaCursorEndCollision;
        }

        protected override void RemoveListeners(VRTK_PlayAreaCursor component)
        {
            component.PlayAreaCursorStartCollision -= PlayAreaCursorStartCollision;
            component.PlayAreaCursorEndCollision -= PlayAreaCursorEndCollision;
        }

        private void PlayAreaCursorStartCollision(object o, PlayAreaCursorEventArgs e)
        {
            OnPlayAreaCursorStartCollision.Invoke(o, e);
        }

        private void PlayAreaCursorEndCollision(object o, PlayAreaCursorEventArgs e)
        {
            OnPlayAreaCursorEndCollision.Invoke(o, e);
        }
    }
}