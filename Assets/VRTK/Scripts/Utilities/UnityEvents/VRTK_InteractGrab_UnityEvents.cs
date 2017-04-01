namespace VRTK.UnityEventHelper
{
    using UnityEngine.Events;
    using System;

    public sealed class VRTK_InteractGrab_UnityEvents : VRTK_UnityEvents<VRTK_InteractGrab>
    {
        [Serializable]
        public sealed class ObjectInteractEvent : UnityEvent<object, ObjectInteractEventArgs> { }

        public ObjectInteractEvent OnControllerGrabInteractableObject = new ObjectInteractEvent();
        public ObjectInteractEvent OnControllerUngrabInteractableObject = new ObjectInteractEvent();
        public VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent OnGrabButtonPressed = new VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent();
        public VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent OnGrabButtonReleased = new VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent();

        protected override void AddListeners(VRTK_InteractGrab component)
        {
            component.ControllerGrabInteractableObject += ControllerGrabInteractableObject;
            component.ControllerUngrabInteractableObject += ControllerUngrabInteractableObject;
            component.GrabButtonPressed += GrabButtonPressed;
            component.GrabButtonReleased += GrabButtonReleased;
        }

        protected override void RemoveListeners(VRTK_InteractGrab component)
        {
            component.ControllerGrabInteractableObject -= ControllerGrabInteractableObject;
            component.ControllerUngrabInteractableObject -= ControllerUngrabInteractableObject;
            component.GrabButtonPressed -= GrabButtonPressed;
            component.GrabButtonReleased -= GrabButtonReleased;
        }

        private void ControllerGrabInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerGrabInteractableObject.Invoke(o, e);
        }

        private void ControllerUngrabInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerUngrabInteractableObject.Invoke(o, e);
        }

        private void GrabButtonPressed(object o, ControllerInteractionEventArgs e)
        {
            OnGrabButtonPressed.Invoke(o, e);
        }

        private void GrabButtonReleased(object o, ControllerInteractionEventArgs e)
        {
            OnGrabButtonReleased.Invoke(o, e);
        }
    }
}