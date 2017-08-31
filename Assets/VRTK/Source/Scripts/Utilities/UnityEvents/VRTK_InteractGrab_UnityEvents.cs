namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_InteractGrab_UnityEvents")]
    public sealed class VRTK_InteractGrab_UnityEvents : VRTK_UnityEvents<VRTK_InteractGrab>
    {
        [Serializable]
        public sealed class ObjectInteractEvent : UnityEvent<object, ObjectInteractEventArgs> { }

        public ObjectInteractEvent OnControllerStartGrabInteractableObject = new ObjectInteractEvent();
        public ObjectInteractEvent OnControllerGrabInteractableObject = new ObjectInteractEvent();
        public ObjectInteractEvent OnControllerStartUngrabInteractableObject = new ObjectInteractEvent();
        public ObjectInteractEvent OnControllerUngrabInteractableObject = new ObjectInteractEvent();
        public VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent OnGrabButtonPressed = new VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent();
        public VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent OnGrabButtonReleased = new VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent();

        protected override void AddListeners(VRTK_InteractGrab component)
        {
            component.ControllerStartGrabInteractableObject += ControllerStartGrabInteractableObject;
            component.ControllerGrabInteractableObject += ControllerGrabInteractableObject;
            component.ControllerStartUngrabInteractableObject += ControllerStartUngrabInteractableObject;
            component.ControllerUngrabInteractableObject += ControllerUngrabInteractableObject;
            component.GrabButtonPressed += GrabButtonPressed;
            component.GrabButtonReleased += GrabButtonReleased;
        }

        protected override void RemoveListeners(VRTK_InteractGrab component)
        {
            component.ControllerStartGrabInteractableObject -= ControllerStartGrabInteractableObject;
            component.ControllerGrabInteractableObject -= ControllerGrabInteractableObject;
            component.ControllerStartUngrabInteractableObject -= ControllerStartUngrabInteractableObject;
            component.ControllerUngrabInteractableObject -= ControllerUngrabInteractableObject;
            component.GrabButtonPressed -= GrabButtonPressed;
            component.GrabButtonReleased -= GrabButtonReleased;
        }

        private void ControllerStartGrabInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerStartGrabInteractableObject.Invoke(o, e);
        }

        private void ControllerGrabInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerGrabInteractableObject.Invoke(o, e);
        }

        private void ControllerStartUngrabInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerStartUngrabInteractableObject.Invoke(o, e);
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