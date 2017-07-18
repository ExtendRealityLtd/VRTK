namespace VRTK.UnityEventHelper
{
    using UnityEngine.Events;
    using System;
#pragma warning disable 0618
    public sealed class VRTK_ControllerActions_UnityEvents : VRTK_UnityEvents<VRTK_ControllerActions>
    {
        [Serializable]
        public sealed class ControllerActionsEvent : UnityEvent<object, ControllerActionsEventArgs> { }

        public ControllerActionsEvent OnControllerModelVisible = new ControllerActionsEvent();
        public ControllerActionsEvent OnControllerModelInvisible = new ControllerActionsEvent();

        protected override void AddListeners(VRTK_ControllerActions component)
        {
            component.ControllerModelVisible += ControllerModelVisible;
            component.ControllerModelInvisible += ControllerModelInvisible;
        }

        protected override void RemoveListeners(VRTK_ControllerActions component)
        {
            component.ControllerModelVisible -= ControllerModelVisible;
            component.ControllerModelInvisible -= ControllerModelInvisible;
        }

        private void ControllerModelVisible(object o, ControllerActionsEventArgs e)
        {
            OnControllerModelVisible.Invoke(o, e);
        }

        private void ControllerModelInvisible(object o, ControllerActionsEventArgs e)
        {
            OnControllerModelInvisible.Invoke(o, e);
        }
    }
#pragma warning restore 0618
}