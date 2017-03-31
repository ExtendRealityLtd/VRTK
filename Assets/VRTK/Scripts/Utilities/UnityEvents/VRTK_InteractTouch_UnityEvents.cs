namespace VRTK.UnityEventHelper
{
    using UnityEngine.Events;
    using System;

    public sealed class VRTK_InteractTouch_UnityEvents : VRTK_UnityEvents<VRTK_InteractTouch>
    {
        [Serializable]
        public sealed class ObjectInteractEvent : UnityEvent<object, ObjectInteractEventArgs> { }

        public ObjectInteractEvent OnControllerTouchInteractableObject = new ObjectInteractEvent();
        public ObjectInteractEvent OnControllerUntouchInteractableObject = new ObjectInteractEvent();

        protected override void AddListeners(VRTK_InteractTouch component)
        {
            component.ControllerTouchInteractableObject += ControllerTouchInteractableObject;
            component.ControllerUntouchInteractableObject += ControllerUntouchInteractableObject;
        }

        protected override void RemoveListeners(VRTK_InteractTouch component)
        {
            component.ControllerTouchInteractableObject -= ControllerTouchInteractableObject;
            component.ControllerUntouchInteractableObject -= ControllerUntouchInteractableObject;
        }

        private void ControllerTouchInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerTouchInteractableObject.Invoke(o, e);
        }

        private void ControllerUntouchInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerUntouchInteractableObject.Invoke(o, e);
        }
    }
}