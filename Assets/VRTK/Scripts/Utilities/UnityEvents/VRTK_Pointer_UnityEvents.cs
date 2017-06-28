namespace VRTK.UnityEventHelper
{
    using UnityEngine;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_Pointer_UnityEvents")]
    public sealed class VRTK_Pointer_UnityEvents : VRTK_UnityEvents<VRTK_Pointer>
    {
        public VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent OnActivationButtonPressed = new VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent();
        public VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent OnActivationButtonReleased = new VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent();
        public VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent OnSelectionButtonPressed = new VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent();
        public VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent OnSelectionButtonReleased = new VRTK_ControllerEvents_UnityEvents.ControllerInteractionEvent();
        public VRTK_DestinationMarker_UnityEvents.DestinationMarkerEvent OnPointerStateValid = new VRTK_DestinationMarker_UnityEvents.DestinationMarkerEvent();
        public VRTK_DestinationMarker_UnityEvents.DestinationMarkerEvent OnPointerStateInvalid = new VRTK_DestinationMarker_UnityEvents.DestinationMarkerEvent();

        protected override void AddListeners(VRTK_Pointer component)
        {
            component.ActivationButtonPressed += ActivationButtonPressed;
            component.ActivationButtonReleased += ActivationButtonReleased;
            component.SelectionButtonPressed += SelectionButtonPressed;
            component.SelectionButtonReleased += SelectionButtonReleased;
            component.PointerStateValid += PointerStateValid;
            component.PointerStateInvalid += PointerStateInvalid;
        }

        protected override void RemoveListeners(VRTK_Pointer component)
        {
            component.ActivationButtonPressed -= ActivationButtonPressed;
            component.ActivationButtonReleased -= ActivationButtonReleased;
            component.SelectionButtonPressed -= SelectionButtonPressed;
            component.SelectionButtonReleased -= SelectionButtonReleased;
            component.PointerStateValid -= PointerStateValid;
            component.PointerStateInvalid -= PointerStateInvalid;
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

        private void PointerStateValid(object o, DestinationMarkerEventArgs e)
        {
            OnPointerStateValid.Invoke(o, e);
        }

        private void PointerStateInvalid(object o, DestinationMarkerEventArgs e)
        {
            OnPointerStateInvalid.Invoke(o, e);
        }
    }
}