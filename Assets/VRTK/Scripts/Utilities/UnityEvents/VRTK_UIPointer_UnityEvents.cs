namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_UIPointer_UnityEvents")]
    public sealed class VRTK_UIPointer_UnityEvents : VRTK_UnityEvents<VRTK_UIPointer>
    {
        [Serializable]
        public sealed class UIPointerEvent : UnityEvent<object, UIPointerEventArgs> { }

        public UIPointerEvent OnUIPointerElementEnter = new UIPointerEvent();
        public UIPointerEvent OnUIPointerElementExit = new UIPointerEvent();
        public UIPointerEvent OnUIPointerElementClick = new UIPointerEvent();
        public UIPointerEvent OnUIPointerElementDragStart = new UIPointerEvent();
        public UIPointerEvent OnUIPointerElementDragEnd = new UIPointerEvent();
        public VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent OnActivationButtonPressed = new VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent();
        public VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent OnActivationButtonReleased = new VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent();
        public VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent OnSelectionButtonPressed = new VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent();
        public VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent OnSelectionButtonReleased = new VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent();

        protected override void AddListeners(VRTK_UIPointer component)
        {
            component.UIPointerElementEnter += UIPointerElementEnter;
            component.UIPointerElementExit += UIPointerElementExit;
            component.UIPointerElementClick += UIPointerElementClick;
            component.UIPointerElementDragStart += UIPointerElementDragStart;
            component.UIPointerElementDragEnd += UIPointerElementDragEnd;
            component.ActivationButtonPressed += ActivationButtonPressed;
            component.ActivationButtonReleased += ActivationButtonReleased;
            component.SelectionButtonPressed += SelectionButtonPressed;
            component.SelectionButtonReleased += SelectionButtonReleased;
        }

        protected override void RemoveListeners(VRTK_UIPointer component)
        {
            component.UIPointerElementEnter -= UIPointerElementEnter;
            component.UIPointerElementExit -= UIPointerElementExit;
            component.UIPointerElementClick -= UIPointerElementClick;
            component.UIPointerElementDragStart -= UIPointerElementDragStart;
            component.UIPointerElementDragEnd -= UIPointerElementDragEnd;
            component.ActivationButtonPressed -= ActivationButtonPressed;
            component.ActivationButtonReleased -= ActivationButtonReleased;
            component.SelectionButtonPressed -= SelectionButtonPressed;
            component.SelectionButtonReleased -= SelectionButtonReleased;
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

        private void ActivationButtonPressed(object o, ControllerInteractionEventArgs e)
        {
            OnActivationButtonPressed.Invoke(o, e);
        }

        private void ActivationButtonReleased(object o, ControllerInteractionEventArgs e)
        {
            OnActivationButtonReleased.Invoke(o, e);
        }

        private void SelectionButtonPressed(object o, ControllerInteractionEventArgs e)
        {
            OnSelectionButtonPressed.Invoke(o, e);
        }

        private void SelectionButtonReleased(object o, ControllerInteractionEventArgs e)
        {
            OnSelectionButtonReleased.Invoke(o, e);
        }
    }
}