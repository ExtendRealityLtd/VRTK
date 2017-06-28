namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_ObjectTooltip_UnityEvents")]
    public sealed class VRTK_ObjectTooltip_UnityEvents : VRTK_UnityEvents<VRTK_ObjectTooltip>
    {
        [Serializable]
        public sealed class ObjectTooltipEvent : UnityEvent<object, ObjectTooltipEventArgs> { }

        public ObjectTooltipEvent OnObjectTooltipReset = new ObjectTooltipEvent();
        public ObjectTooltipEvent OnObjectTooltipTextUpdated = new ObjectTooltipEvent();

        protected override void AddListeners(VRTK_ObjectTooltip component)
        {
            component.ObjectTooltipReset += ObjectTooltipReset;
            component.ObjectTooltipTextUpdated += ObjectTooltipTextUpdated;
        }

        protected override void RemoveListeners(VRTK_ObjectTooltip component)
        {
            component.ObjectTooltipReset -= ObjectTooltipReset;
            component.ObjectTooltipTextUpdated -= ObjectTooltipTextUpdated;
        }

        private void ObjectTooltipReset(object o, ObjectTooltipEventArgs e)
        {
            OnObjectTooltipReset.Invoke(o, e);
        }

        private void ObjectTooltipTextUpdated(object o, ObjectTooltipEventArgs e)
        {
            OnObjectTooltipTextUpdated.Invoke(o, e);
        }
    }
}