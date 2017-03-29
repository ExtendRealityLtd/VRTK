namespace VRTK.UnityEventHelper
{
    using UnityEngine.Events;
    using System;

    public sealed class VRTK_DashTeleport_UnityEvents : VRTK_UnityEvents<VRTK_DashTeleport>
    {
        [Serializable]
        public sealed class DashTeleportEvent : UnityEvent<object, DashTeleportEventArgs> { }

        public DashTeleportEvent OnWillDashThruObjects = new DashTeleportEvent();
        public DashTeleportEvent OnDashedThruObjects = new DashTeleportEvent();

        protected override void AddListeners(VRTK_DashTeleport component)
        {
            component.WillDashThruObjects += WillDashThruObjects;
            component.DashedThruObjects += DashedThruObjects;
        }

        protected override void RemoveListeners(VRTK_DashTeleport component)
        {
            component.WillDashThruObjects -= WillDashThruObjects;
            component.DashedThruObjects -= DashedThruObjects;
        }

        private void WillDashThruObjects(object o, DashTeleportEventArgs e)
        {
            OnWillDashThruObjects.Invoke(o, e);
        }

        private void DashedThruObjects(object o, DashTeleportEventArgs e)
        {
            OnDashedThruObjects.Invoke(o, e);
        }
    }
}