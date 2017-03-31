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

        protected override void AddListeners(VRTK_InteractGrab component)
        {
            component.ControllerGrabInteractableObject += ControllerGrabInteractableObject;
            component.ControllerUngrabInteractableObject += ControllerUngrabInteractableObject;
        }

        protected override void RemoveListeners(VRTK_InteractGrab component)
        {
            component.ControllerGrabInteractableObject -= ControllerGrabInteractableObject;
            component.ControllerUngrabInteractableObject -= ControllerUngrabInteractableObject;
        }

        private void ControllerGrabInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerGrabInteractableObject.Invoke(o, e);
        }

        private void ControllerUngrabInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerUngrabInteractableObject.Invoke(o, e);
        }
    }
}