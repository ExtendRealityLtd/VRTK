namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_InteractableObject_UnityEvents")]
    public sealed class VRTK_InteractableObject_UnityEvents : VRTK_UnityEvents<VRTK_InteractableObject>
    {
        [Serializable]
        public sealed class InteractableObjectEvent : UnityEvent<object, InteractableObjectEventArgs> { }

        public InteractableObjectEvent OnObjectEnable = new InteractableObjectEvent();
        public InteractableObjectEvent OnObjectDisable = new InteractableObjectEvent();

        public InteractableObjectEvent OnNearTouch = new InteractableObjectEvent();
        public InteractableObjectEvent OnNearUntouch = new InteractableObjectEvent();

        public InteractableObjectEvent OnTouch = new InteractableObjectEvent();
        public InteractableObjectEvent OnUntouch = new InteractableObjectEvent();

        public InteractableObjectEvent OnGrab = new InteractableObjectEvent();
        public InteractableObjectEvent OnUngrab = new InteractableObjectEvent();

        public InteractableObjectEvent OnUse = new InteractableObjectEvent();
        public InteractableObjectEvent OnUnuse = new InteractableObjectEvent();

        public InteractableObjectEvent OnEnterSnapDropZone = new InteractableObjectEvent();
        public InteractableObjectEvent OnExitSnapDropZone = new InteractableObjectEvent();
        public InteractableObjectEvent OnSnapToDropZone = new InteractableObjectEvent();
        public InteractableObjectEvent OnUnsnapFromDropZone = new InteractableObjectEvent();

        protected override void AddListeners(VRTK_InteractableObject component)
        {
            component.InteractableObjectEnabled += Enable;
            component.InteractableObjectDisabled += Disable;

            component.InteractableObjectNearTouched += NearTouch;
            component.InteractableObjectNearUntouched += NearUnTouch;

            component.InteractableObjectTouched += Touch;
            component.InteractableObjectUntouched += UnTouch;

            component.InteractableObjectGrabbed += Grab;
            component.InteractableObjectUngrabbed += UnGrab;

            component.InteractableObjectUsed += Use;
            component.InteractableObjectUnused += Unuse;

            component.InteractableObjectEnteredSnapDropZone += EnterSnapDropZone;
            component.InteractableObjectExitedSnapDropZone += ExitSnapDropZone;
            component.InteractableObjectSnappedToDropZone += SnapToDropZone;
            component.InteractableObjectUnsnappedFromDropZone += UnsnapFromDropZone;
        }

        protected override void RemoveListeners(VRTK_InteractableObject component)
        {
            component.InteractableObjectEnabled -= Enable;
            component.InteractableObjectDisabled -= Disable;

            component.InteractableObjectNearTouched -= NearTouch;
            component.InteractableObjectNearUntouched -= NearUnTouch;

            component.InteractableObjectTouched -= Touch;
            component.InteractableObjectUntouched -= UnTouch;

            component.InteractableObjectGrabbed -= Grab;
            component.InteractableObjectUngrabbed -= UnGrab;

            component.InteractableObjectUsed -= Use;
            component.InteractableObjectUnused -= Unuse;

            component.InteractableObjectEnteredSnapDropZone -= EnterSnapDropZone;
            component.InteractableObjectExitedSnapDropZone -= ExitSnapDropZone;
            component.InteractableObjectSnappedToDropZone -= SnapToDropZone;
            component.InteractableObjectUnsnappedFromDropZone -= UnsnapFromDropZone;
        }

        private void Enable(object o, InteractableObjectEventArgs e)
        {
            OnObjectEnable.Invoke(o, e);
        }

        private void Disable(object o, InteractableObjectEventArgs e)
        {
            OnObjectDisable.Invoke(o, e);
        }

        private void NearTouch(object o, InteractableObjectEventArgs e)
        {
            OnNearTouch.Invoke(o, e);
        }

        private void NearUnTouch(object o, InteractableObjectEventArgs e)
        {
            OnNearUntouch.Invoke(o, e);
        }

        private void Touch(object o, InteractableObjectEventArgs e)
        {
            OnTouch.Invoke(o, e);
        }

        private void UnTouch(object o, InteractableObjectEventArgs e)
        {
            OnUntouch.Invoke(o, e);
        }

        private void Grab(object o, InteractableObjectEventArgs e)
        {
            OnGrab.Invoke(o, e);
        }

        private void UnGrab(object o, InteractableObjectEventArgs e)
        {
            OnUngrab.Invoke(o, e);
        }

        private void Use(object o, InteractableObjectEventArgs e)
        {
            OnUse.Invoke(o, e);
        }

        private void Unuse(object o, InteractableObjectEventArgs e)
        {
            OnUnuse.Invoke(o, e);
        }

        private void EnterSnapDropZone(object o, InteractableObjectEventArgs e)
        {
            OnEnterSnapDropZone.Invoke(o, e);
        }

        private void ExitSnapDropZone(object o, InteractableObjectEventArgs e)
        {
            OnExitSnapDropZone.Invoke(o, e);
        }

        private void SnapToDropZone(object o, InteractableObjectEventArgs e)
        {
            OnSnapToDropZone.Invoke(o, e);
        }

        private void UnsnapFromDropZone(object o, InteractableObjectEventArgs e)
        {
            OnUnsnapFromDropZone.Invoke(o, e);
        }
    }
}