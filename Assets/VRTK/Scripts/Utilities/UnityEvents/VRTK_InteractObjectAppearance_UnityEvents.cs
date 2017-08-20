namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_InteractObjectAppearance_UnityEvents")]
    public sealed class VRTK_InteractObjectAppearance_UnityEvents : VRTK_UnityEvents<VRTK_InteractObjectAppearance>
    {
        [Serializable]
        public sealed class InteractObjectAppearanceEvent : UnityEvent<object, InteractObjectAppearanceEventArgs> { }

        public InteractObjectAppearanceEvent OnGameObjectEnabled = new InteractObjectAppearanceEvent();
        public InteractObjectAppearanceEvent OnGameObjectDisabled = new InteractObjectAppearanceEvent();
        public InteractObjectAppearanceEvent OnRenderersEnabled = new InteractObjectAppearanceEvent();
        public InteractObjectAppearanceEvent OnRenderersDisabled = new InteractObjectAppearanceEvent();

        protected override void AddListeners(VRTK_InteractObjectAppearance component)
        {
            component.GameObjectEnabled += GameObjectEnabled;
            component.GameObjectDisabled += GameObjectDisabled;
            component.RenderersEnabled += RenderersEnabled;
            component.RenderersDisabled += RenderersDisabled;
        }

        protected override void RemoveListeners(VRTK_InteractObjectAppearance component)
        {
            component.GameObjectEnabled -= GameObjectEnabled;
            component.GameObjectDisabled -= GameObjectDisabled;
            component.RenderersEnabled -= RenderersEnabled;
            component.RenderersDisabled -= RenderersDisabled;
        }

        private void GameObjectEnabled(object o, InteractObjectAppearanceEventArgs e)
        {
            OnGameObjectEnabled.Invoke(o, e);
        }

        private void GameObjectDisabled(object o, InteractObjectAppearanceEventArgs e)
        {
            OnGameObjectDisabled.Invoke(o, e);
        }

        private void RenderersEnabled(object o, InteractObjectAppearanceEventArgs e)
        {
            OnRenderersEnabled.Invoke(o, e);
        }

        private void RenderersDisabled(object o, InteractObjectAppearanceEventArgs e)
        {
            OnRenderersDisabled.Invoke(o, e);
        }
    }
}