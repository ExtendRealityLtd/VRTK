namespace VRTK.UnityEventHelper
{
    using UnityEngine.Events;
    using System;

    public sealed class VRTK_SDKSetup_UnityEvents : VRTK_UnityEvents<VRTK_SDKSetup>
    {
        [Serializable]
        public sealed class LoadEvent : UnityEvent<VRTK_SDKManager, VRTK_SDKSetup> { }

        public LoadEvent OnLoaded = new LoadEvent();
        public LoadEvent OnUnloaded = new LoadEvent();

        protected override void AddListeners(VRTK_SDKSetup component)
        {
            component.Loaded += Loaded;
            component.Unloaded += Unloaded;
        }

        protected override void RemoveListeners(VRTK_SDKSetup component)
        {
            component.Loaded -= Loaded;
            component.Unloaded -= Unloaded;
        }

        private void Loaded(VRTK_SDKManager sender, VRTK_SDKSetup setup)
        {
            OnLoaded.Invoke(sender, setup);
        }

        private void Unloaded(VRTK_SDKManager sender, VRTK_SDKSetup setup)
        {
            OnUnloaded.Invoke(sender, setup);
        }
    }
}