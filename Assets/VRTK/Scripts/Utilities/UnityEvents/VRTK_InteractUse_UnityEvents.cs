namespace VRTK.UnityEventHelper
{
    using UnityEngine.Events;
    using System;

    public sealed class VRTK_InteractUse_UnityEvents : VRTK_UnityEvents<VRTK_InteractUse>
    {
        [Serializable]
        public sealed class ObjectInteractEvent : UnityEvent<object, ObjectInteractEventArgs> { }

        public ObjectInteractEvent OnControllerUseInteractableObject = new ObjectInteractEvent();
        public ObjectInteractEvent OnControllerUnuseInteractableObject = new ObjectInteractEvent();

        protected override void AddListeners(VRTK_InteractUse component)
        {
            component.ControllerUseInteractableObject += ControllerUseInteractableObject;
            component.ControllerUnuseInteractableObject += ControllerUnuseInteractableObject;
        }

        protected override void RemoveListeners(VRTK_InteractUse component)
        {
            component.ControllerUseInteractableObject -= ControllerUseInteractableObject;
            component.ControllerUnuseInteractableObject -= ControllerUnuseInteractableObject;
        }

        private void ControllerUseInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerUseInteractableObject.Invoke(o, e);
        }

        private void ControllerUnuseInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerUnuseInteractableObject.Invoke(o, e);
        }
    }
}