namespace VRTK.UnityEventHelper
{
    using UnityEngine.Events;
    using System;

    public sealed class VRTK_BasicTeleport_UnityEvents : VRTK_UnityEvents<VRTK_BasicTeleport>
    {
        [Serializable]
        public sealed class TeleportEvent : UnityEvent<object, DestinationMarkerEventArgs> { }

        public TeleportEvent OnTeleporting = new TeleportEvent();
        public TeleportEvent OnTeleported = new TeleportEvent();

        protected override void AddListeners(VRTK_BasicTeleport component)
        {
            component.Teleporting += Teleporting;
            component.Teleported += Teleported;
        }

        protected override void RemoveListeners(VRTK_BasicTeleport component)
        {
            component.Teleporting -= Teleporting;
            component.Teleported -= Teleported;
        }

        private void Teleporting(object o, DestinationMarkerEventArgs e)
        {
            OnTeleporting.Invoke(o, e);
        }

        private void Teleported(object o, DestinationMarkerEventArgs e)
        {
            OnTeleported.Invoke(o, e);
        }
    }
}