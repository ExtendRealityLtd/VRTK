namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_InteractObjectHighlighter_UnityEvents")]
    public sealed class VRTK_InteractObjectHighlighter_UnityEvents : VRTK_UnityEvents<VRTK_InteractObjectHighlighter>
    {
        [Serializable]
        public sealed class InteractObjectHighlighterEvent : UnityEvent<object, InteractObjectHighlighterEventArgs> { }

        public InteractObjectHighlighterEvent OnInteractObjectHighlighterHighlighted = new InteractObjectHighlighterEvent();
        public InteractObjectHighlighterEvent OnInteractObjectHighlighterUnhighlighted = new InteractObjectHighlighterEvent();

        protected override void AddListeners(VRTK_InteractObjectHighlighter component)
        {
            component.InteractObjectHighlighterHighlighted += InteractObjectHighlighterHighlighted;
            component.InteractObjectHighlighterUnhighlighted += InteractObjectHighlighterUnhighlighted;
        }

        protected override void RemoveListeners(VRTK_InteractObjectHighlighter component)
        {
            component.InteractObjectHighlighterHighlighted -= InteractObjectHighlighterHighlighted;
            component.InteractObjectHighlighterUnhighlighted -= InteractObjectHighlighterUnhighlighted;
        }

        private void InteractObjectHighlighterHighlighted(object o, InteractObjectHighlighterEventArgs e)
        {
            OnInteractObjectHighlighterHighlighted.Invoke(o, e);
        }

        private void InteractObjectHighlighterUnhighlighted(object o, InteractObjectHighlighterEventArgs e)
        {
            OnInteractObjectHighlighterUnhighlighted.Invoke(o, e);
        }
    }
}