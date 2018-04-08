namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_InteractNearTouch_UnityEvents")]
    public sealed class VRTK_InteractNearTouch_UnityEvents : VRTK_UnityEvents<VRTK_InteractNearTouch>
    {
        [Serializable]
        public sealed class ObjectInteractEvent : UnityEvent<object, ObjectInteractEventArgs> { }

        public ObjectInteractEvent OnControllerNearTouchInteractableObject = new ObjectInteractEvent();
        public ObjectInteractEvent OnControllerNearUntouchInteractableObject = new ObjectInteractEvent();

        protected override void AddListeners(VRTK_InteractNearTouch component)
        {
            component.ControllerNearTouchInteractableObject += ControllerNearTouchInteractableObject;
            component.ControllerNearUntouchInteractableObject += ControllerNearUntouchInteractableObject;
        }

        protected override void RemoveListeners(VRTK_InteractNearTouch component)
        {
            component.ControllerNearTouchInteractableObject -= ControllerNearTouchInteractableObject;
            component.ControllerNearUntouchInteractableObject -= ControllerNearUntouchInteractableObject;
        }

        private void ControllerNearTouchInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerNearTouchInteractableObject.Invoke(o, e);
        }

        private void ControllerNearUntouchInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerNearUntouchInteractableObject.Invoke(o, e);
        }
    }
}