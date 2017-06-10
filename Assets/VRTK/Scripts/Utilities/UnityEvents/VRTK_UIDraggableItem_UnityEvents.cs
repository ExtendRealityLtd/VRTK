namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_UIDraggableItem_UnityEvents")]
    public sealed class VRTK_UIDraggableItem_UnityEvents : VRTK_UnityEvents<VRTK_UIDraggableItem>
    {
        [Serializable]
        public sealed class UIDraggableItemEvent : UnityEvent<object, UIDraggableItemEventArgs> { }

        public UIDraggableItemEvent OnDraggableItemDropped = new UIDraggableItemEvent();
        public UIDraggableItemEvent OnDraggableItemReset = new UIDraggableItemEvent();

        protected override void AddListeners(VRTK_UIDraggableItem component)
        {
            component.DraggableItemDropped += DraggableItemDropped;
            component.DraggableItemReset += DraggableItemReset;
        }

        protected override void RemoveListeners(VRTK_UIDraggableItem component)
        {
            component.DraggableItemDropped -= DraggableItemDropped;
            component.DraggableItemReset -= DraggableItemReset;
        }

        private void DraggableItemDropped(object o, UIDraggableItemEventArgs e)
        {
            OnDraggableItemDropped.Invoke(o, e);
        }

        private void DraggableItemReset(object o, UIDraggableItemEventArgs e)
        {
            OnDraggableItemReset.Invoke(o, e);
        }
    }
}