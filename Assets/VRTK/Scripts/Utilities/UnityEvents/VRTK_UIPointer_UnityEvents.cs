namespace VRTK.UnityEventHelper
{
    using UnityEngine.Events;
    using System;

    public sealed class VRTK_UIPointer_UnityEvents : VRTK_UnityEvents<VRTK_UIPointer>
    {
        [Serializable]
        public sealed class UIPointerEvent : UnityEvent<object, UIPointerEventArgs> { }

        public UIPointerEvent OnUIPointerElementEnter = new UIPointerEvent();
        public UIPointerEvent OnUIPointerElementExit = new UIPointerEvent();
        public UIPointerEvent OnUIPointerElementClick = new UIPointerEvent();
        public UIPointerEvent OnUIPointerElementDragStart = new UIPointerEvent();
        public UIPointerEvent OnUIPointerElementDragEnd = new UIPointerEvent();

        protected override void AddListeners(VRTK_UIPointer component)
        {
            component.UIPointerElementEnter += UIPointerElementEnter;
            component.UIPointerElementExit += UIPointerElementExit;
            component.UIPointerElementClick += UIPointerElementClick;
            component.UIPointerElementDragStart += UIPointerElementDragStart;
            component.UIPointerElementDragEnd += UIPointerElementDragEnd;
        }

        protected override void RemoveListeners(VRTK_UIPointer component)
        {
            component.UIPointerElementEnter -= UIPointerElementEnter;
            component.UIPointerElementExit -= UIPointerElementExit;
            component.UIPointerElementClick -= UIPointerElementClick;
            component.UIPointerElementDragStart -= UIPointerElementDragStart;
            component.UIPointerElementDragEnd -= UIPointerElementDragEnd;
        }

        private void UIPointerElementEnter(object o, UIPointerEventArgs e)
        {
            OnUIPointerElementEnter.Invoke(o, e);
        }

        private void UIPointerElementExit(object o, UIPointerEventArgs e)
        {
            OnUIPointerElementExit.Invoke(o, e);
        }

        private void UIPointerElementClick(object o, UIPointerEventArgs e)
        {
            OnUIPointerElementClick.Invoke(o, e);
        }

        private void UIPointerElementDragStart(object o, UIPointerEventArgs e)
        {
            OnUIPointerElementDragStart.Invoke(o, e);
        }

        private void UIPointerElementDragEnd(object o, UIPointerEventArgs e)
        {
            OnUIPointerElementDragEnd.Invoke(o, e);
        }
    }
}