namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_ControllerTooltips_UnityEvents")]
    public sealed class VRTK_ControllerTooltips_UnityEvents : VRTK_UnityEvents<VRTK_ControllerTooltips>
    {
        [Serializable]
        public sealed class ControllerTooltipsEvent : UnityEvent<object, ControllerTooltipsEventArgs> { }

        public ControllerTooltipsEvent OnControllerTooltipOn = new ControllerTooltipsEvent();
        public ControllerTooltipsEvent OnControllerTooltipOff = new ControllerTooltipsEvent();

        protected override void AddListeners(VRTK_ControllerTooltips component)
        {
            component.ControllerTooltipOn += ControllerTooltipOn;
            component.ControllerTooltipOff += ControllerTooltipOff;
        }

        protected override void RemoveListeners(VRTK_ControllerTooltips component)
        {
            component.ControllerTooltipOn -= ControllerTooltipOn;
            component.ControllerTooltipOff -= ControllerTooltipOff;
        }

        private void ControllerTooltipOn(object o, ControllerTooltipsEventArgs e)
        {
            OnControllerTooltipOn.Invoke(o, e);
        }

        private void ControllerTooltipOff(object o, ControllerTooltipsEventArgs e)
        {
            OnControllerTooltipOff.Invoke(o, e);
        }
    }
}