namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_DestinationPoint_UnityEvents")]
    public sealed class VRTK_DestinationPoint_UnityEvents : VRTK_UnityEvents<VRTK_DestinationPoint>
    {
        [Serializable]
        public sealed class DestinationPointEvent : UnityEvent<object> { }

        public DestinationPointEvent OnDestinationPointEnabled = new DestinationPointEvent();
        public DestinationPointEvent OnDestinationPointDisabled = new DestinationPointEvent();
        public DestinationPointEvent OnDestinationPointLocked = new DestinationPointEvent();
        public DestinationPointEvent OnDestinationPointUnlocked = new DestinationPointEvent();
        public DestinationPointEvent OnDestinationPointReset = new DestinationPointEvent();

        protected override void AddListeners(VRTK_DestinationPoint component)
        {
            component.DestinationPointEnabled += DestinationPointEnabled;
            component.DestinationPointDisabled += DestinationPointDisabled;
            component.DestinationPointLocked += DestinationPointLocked;
            component.DestinationPointUnlocked += DestinationPointUnlocked;
            component.DestinationPointReset += DestinationPointReset;
        }

        protected override void RemoveListeners(VRTK_DestinationPoint component)
        {
            component.DestinationPointEnabled -= DestinationPointEnabled;
            component.DestinationPointDisabled -= DestinationPointDisabled;
            component.DestinationPointLocked -= DestinationPointLocked;
            component.DestinationPointUnlocked -= DestinationPointUnlocked;
            component.DestinationPointReset -= DestinationPointReset;
        }

        private void DestinationPointEnabled(object o)
        {
            OnDestinationPointEnabled.Invoke(o);
        }

        private void DestinationPointDisabled(object o)
        {
            OnDestinationPointDisabled.Invoke(o);
        }

        private void DestinationPointLocked(object o)
        {
            OnDestinationPointLocked.Invoke(o);
        }

        private void DestinationPointUnlocked(object o)
        {
            OnDestinationPointUnlocked.Invoke(o);
        }

        private void DestinationPointReset(object o)
        {
            OnDestinationPointReset.Invoke(o);
        }
    }
}