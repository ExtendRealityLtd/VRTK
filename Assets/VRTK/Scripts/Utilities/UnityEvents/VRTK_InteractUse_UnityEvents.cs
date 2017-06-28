namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_InteractUse_UnityEvents")]
    public sealed class VRTK_InteractUse_UnityEvents : VRTK_UnityEvents<VRTK_InteractUse>
    {
        [Serializable]
        public sealed class ObjectInteractEvent : UnityEvent<object, ObjectInteractEventArgs> { }

        public ObjectInteractEvent OnControllerStartUseInteractableObject = new ObjectInteractEvent();
        public ObjectInteractEvent OnControllerUseInteractableObject = new ObjectInteractEvent();
        public ObjectInteractEvent OnControllerStartUnuseInteractableObject = new ObjectInteractEvent();
        public ObjectInteractEvent OnControllerUnuseInteractableObject = new ObjectInteractEvent();
        public VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent OnUseButtonPressed = new VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent();
        public VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent OnUseButtonReleased = new VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent();

        protected override void AddListeners(VRTK_InteractUse component)
        {
            component.ControllerStartUseInteractableObject += ControllerStartUseInteractableObject;
            component.ControllerUseInteractableObject += ControllerUseInteractableObject;
            component.ControllerStartUnuseInteractableObject += ControllerStartUnuseInteractableObject;
            component.ControllerUnuseInteractableObject += ControllerUnuseInteractableObject;
            component.UseButtonPressed += UseButtonPressed;
            component.UseButtonReleased += UseButtonReleased;
        }

        protected override void RemoveListeners(VRTK_InteractUse component)
        {
            component.ControllerStartUseInteractableObject -= ControllerStartUseInteractableObject;
            component.ControllerUseInteractableObject -= ControllerUseInteractableObject;
            component.ControllerStartUnuseInteractableObject -= ControllerStartUnuseInteractableObject;
            component.ControllerUnuseInteractableObject -= ControllerUnuseInteractableObject;
            component.UseButtonPressed -= UseButtonPressed;
            component.UseButtonReleased -= UseButtonReleased;
        }

        private void ControllerStartUseInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerStartUseInteractableObject.Invoke(o, e);
        }

        private void ControllerUseInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerUseInteractableObject.Invoke(o, e);
        }

        private void ControllerStartUnuseInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerStartUnuseInteractableObject.Invoke(o, e);
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