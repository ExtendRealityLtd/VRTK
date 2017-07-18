namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_DestinationMarker_UnityEvents")]
    public sealed class VRTK_DestinationMarker_UnityEvents : VRTK_UnityEvents<VRTK_DestinationMarker>
    {
        [Serializable]
        public sealed class DestinationMarkerEvent : UnityEvent<object, DestinationMarkerEventArgs> { }

        public DestinationMarkerEvent OnDestinationMarkerEnter = new DestinationMarkerEvent();
        public DestinationMarkerEvent OnDestinationMarkerExit = new DestinationMarkerEvent();
        public DestinationMarkerEvent OnDestinationMarkerHover = new DestinationMarkerEvent();
        public DestinationMarkerEvent OnDestinationMarkerSet = new DestinationMarkerEvent();

        protected override void AddListeners(VRTK_DestinationMarker component)
        {
            component.DestinationMarkerEnter += DestinationMarkerEnter;
            component.DestinationMarkerExit += DestinationMarkerExit;
            component.DestinationMarkerHover += DestinationMarkerHover;
            component.DestinationMarkerSet += DestinationMarkerSet;
        }

        protected override void RemoveListeners(VRTK_DestinationMarker component)
        {
            component.DestinationMarkerEnter -= DestinationMarkerEnter;
            component.DestinationMarkerExit -= DestinationMarkerExit;
            component.DestinationMarkerHover -= DestinationMarkerHover;
            component.DestinationMarkerSet -= DestinationMarkerSet;
        }

        private void DestinationMarkerEnter(object o, DestinationMarkerEventArgs e)
        {
            OnDestinationMarkerEnter.Invoke(o, e);
        }

        private void DestinationMarkerExit(object o, DestinationMarkerEventArgs e)
        {
            OnDestinationMarkerExit.Invoke(o, e);
        }

        private void DestinationMarkerHover(object o, DestinationMarkerEventArgs e)
        {
            OnDestinationMarkerHover.Invoke(o, e);
        }

        private void DestinationMarkerSet(object o, DestinationMarkerEventArgs e)
        {
            OnDestinationMarkerSet.Invoke(o, e);
        }
    }
}