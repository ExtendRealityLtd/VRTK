namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_InteractHaptics_UnityEvents")]
    public sealed class VRTK_InteractHaptics_UnityEvents : VRTK_UnityEvents<VRTK_InteractHaptics>
    {
        [Serializable]
        public sealed class InteractHapticsEvent : UnityEvent<object, InteractHapticsEventArgs> { }

        public InteractHapticsEvent OnInteractHapticsNearTouched = new InteractHapticsEvent();
        public InteractHapticsEvent OnInteractHapticsTouched = new InteractHapticsEvent();
        public InteractHapticsEvent OnInteractHapticsGrabbed = new InteractHapticsEvent();
        public InteractHapticsEvent OnInteractHapticsUsed = new InteractHapticsEvent();

        protected override void AddListeners(VRTK_InteractHaptics component)
        {
            component.InteractHapticsNearTouched += InteractHapticsNearTouched;
            component.InteractHapticsTouched += InteractHapticsTouched;
            component.InteractHapticsGrabbed += InteractHapticsGrabbed;
            component.InteractHapticsUsed += InteractHapticsUsed;
        }

        protected override void RemoveListeners(VRTK_InteractHaptics component)
        {
            component.InteractHapticsNearTouched -= InteractHapticsNearTouched;
            component.InteractHapticsTouched -= InteractHapticsTouched;
            component.InteractHapticsGrabbed -= InteractHapticsGrabbed;
            component.InteractHapticsUsed -= InteractHapticsUsed;
        }

        private void InteractHapticsNearTouched(object o, InteractHapticsEventArgs e)
        {
            OnInteractHapticsNearTouched.Invoke(o, e);
        }

        private void InteractHapticsTouched(object o, InteractHapticsEventArgs e)
        {
            OnInteractHapticsTouched.Invoke(o, e);
        }

        private void InteractHapticsGrabbed(object o, InteractHapticsEventArgs e)
        {
            OnInteractHapticsGrabbed.Invoke(o, e);
        }

        private void InteractHapticsUsed(object o, InteractHapticsEventArgs e)
        {
            OnInteractHapticsUsed.Invoke(o, e);
        }
    }
}