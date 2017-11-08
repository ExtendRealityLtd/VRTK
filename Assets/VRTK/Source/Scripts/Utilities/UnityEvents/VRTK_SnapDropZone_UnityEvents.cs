namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_SnapDropZone_UnityEvents")]
    public sealed class VRTK_SnapDropZone_UnityEvents : VRTK_UnityEvents<VRTK_SnapDropZone>
    {
        [Serializable]
        public sealed class SnapDropZoneEvent : UnityEvent<object, SnapDropZoneEventArgs> { }

        public SnapDropZoneEvent OnObjectEnteredSnapDropZone = new SnapDropZoneEvent();
        public SnapDropZoneEvent OnObjectExitedSnapDropZone = new SnapDropZoneEvent();

        public SnapDropZoneEvent OnObjectSnappedToDropZone = new SnapDropZoneEvent();
        public SnapDropZoneEvent OnObjectUnsnappedFromDropZone = new SnapDropZoneEvent();

        protected override void AddListeners(VRTK_SnapDropZone component)
        {
            component.ObjectEnteredSnapDropZone += ObjectEnteredSnapDropZone;
            component.ObjectExitedSnapDropZone += ObjectExitedSnapDropZone;

            component.ObjectSnappedToDropZone += ObjectSnappedToDropZone;
            component.ObjectUnsnappedFromDropZone += ObjectUnsnappedFromDropZone;
        }

        protected override void RemoveListeners(VRTK_SnapDropZone component)
        {
            component.ObjectEnteredSnapDropZone -= ObjectEnteredSnapDropZone;
            component.ObjectExitedSnapDropZone -= ObjectExitedSnapDropZone;

            component.ObjectSnappedToDropZone -= ObjectSnappedToDropZone;
            component.ObjectUnsnappedFromDropZone -= ObjectUnsnappedFromDropZone;
        }

        private void ObjectEnteredSnapDropZone(object o, SnapDropZoneEventArgs e)
        {
            OnObjectEnteredSnapDropZone.Invoke(o, e);
        }

        private void ObjectExitedSnapDropZone(object o, SnapDropZoneEventArgs e)
        {
            OnObjectExitedSnapDropZone.Invoke(o, e);
        }

        private void ObjectSnappedToDropZone(object o, SnapDropZoneEventArgs e)
        {
            OnObjectSnappedToDropZone.Invoke(o, e);
        }

        private void ObjectUnsnappedFromDropZone(object o, SnapDropZoneEventArgs e)
        {
            OnObjectUnsnappedFromDropZone.Invoke(o, e);
        }
    }
}