namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_InteractableObject))]
    public class VRTK_InteractableObject_UnityEvents : MonoBehaviour
    {
        private VRTK_InteractableObject io;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, InteractableObjectEventArgs> { };

        /// <summary>
        /// Emits the InteractableObjectTouched class event.
        /// </summary>
        public UnityObjectEvent OnTouch = new UnityObjectEvent();
        /// <summary>
        /// Emits the InteractableObjectUntouched class event.
        /// </summary>
        public UnityObjectEvent OnUntouch = new UnityObjectEvent();
        /// <summary>
        /// Emits the InteractableObjectGrabbed class event.
        /// </summary>
        public UnityObjectEvent OnGrab = new UnityObjectEvent();
        /// <summary>
        /// Emits the InteractableObjectUngrabbed class event.
        /// </summary>
        public UnityObjectEvent OnUngrab = new UnityObjectEvent();
        /// <summary>
        /// Emits the InteractableObjectUsed class event.
        /// </summary>
        public UnityObjectEvent OnUse = new UnityObjectEvent();
        /// <summary>
        /// Emits the InteractableObjectUnused class event.
        /// </summary>
        public UnityObjectEvent OnUnuse = new UnityObjectEvent();

        private void SetInteractableObject()
        {
            if (io == null)
            {
                io = GetComponent<VRTK_InteractableObject>();
            }
        }

        private void OnEnable()
        {
            SetInteractableObject();
            if (io == null)
            {
                Debug.LogError("The VRTK_InteractableObject_UnityEvents script requires to be attached to a GameObject that contains a VRTK_InteractableObject script");
                return;
            }

            io.InteractableObjectTouched += Touch;
            io.InteractableObjectUntouched += UnTouch;
            io.InteractableObjectGrabbed += Grab;
            io.InteractableObjectUngrabbed += UnGrab;
            io.InteractableObjectUsed += Use;
            io.InteractableObjectUnused += Unuse;
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

        private void OnDisable()
        {
            if (io == null)
            {
                return;
            }

            io.InteractableObjectTouched -= Touch;
            io.InteractableObjectUntouched -= UnTouch;
            io.InteractableObjectGrabbed -= Grab;
            io.InteractableObjectUngrabbed -= UnGrab;
            io.InteractableObjectUsed -= Use;
            io.InteractableObjectUnused -= Unuse;
        }
    }
}