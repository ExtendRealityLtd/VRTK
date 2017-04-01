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
        public VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent OnUseButtonPressed = new VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent();
        public VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent OnUseButtonReleased = new VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent();

        protected override void AddListeners(VRTK_InteractUse component)
        {
            component.ControllerUseInteractableObject += ControllerUseInteractableObject;
            component.ControllerUnuseInteractableObject += ControllerUnuseInteractableObject;
            component.UseButtonPressed += UseButtonPressed;
            component.UseButtonReleased += UseButtonReleased;
        }

        protected override void RemoveListeners(VRTK_InteractUse component)
        {
            component.ControllerUseInteractableObject -= ControllerUseInteractableObject;
            component.ControllerUnuseInteractableObject -= ControllerUnuseInteractableObject;
            component.UseButtonPressed -= UseButtonPressed;
            component.UseButtonReleased -= UseButtonReleased;
        }

        private void ControllerUseInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerUseInteractableObject.Invoke(o, e);
        }

        private void ControllerUnuseInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerUnuseInteractableObject.Invoke(o, e);
        }

        private void UseButtonPressed(object o, ControllerInteractionEventArgs e)
        {
            OnUseButtonPressed.Invoke(o, e);
        }

        private void UseButtonReleased(object o, ControllerInteractionEventArgs e)
        {
            OnUseButtonReleased.Invoke(o, e);
        }
    }
}