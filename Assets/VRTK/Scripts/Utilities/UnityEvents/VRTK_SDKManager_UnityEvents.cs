namespace VRTK.UnityEventHelper
{
    using UnityEngine.Events;
    using System;

    public sealed class VRTK_SDKManager_UnityEvents : VRTK_UnityEvents<VRTK_SDKManager>
    {
        [Serializable]
        public sealed class LoadedSetupChangeEvent : UnityEvent<VRTK_SDKManager, VRTK_SDKManager.LoadedSetupChangeEventArgs> { }

        public LoadedSetupChangeEvent OnLoadedSetupChanged = new LoadedSetupChangeEvent();

        protected override void AddListeners(VRTK_SDKManager component)
        {
            component.LoadedSetupChanged += LoadedSetupChanged;
        }

        protected override void RemoveListeners(VRTK_SDKManager component)
        {
            component.LoadedSetupChanged -= LoadedSetupChanged;
        }

        private void LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            OnLoadedSetupChanged.Invoke(sender, e);
        }
    }
}