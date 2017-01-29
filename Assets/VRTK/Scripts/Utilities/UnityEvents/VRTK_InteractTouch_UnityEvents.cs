namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_InteractTouch))]
    public class VRTK_InteractTouch_UnityEvents : MonoBehaviour
    {
        private VRTK_InteractTouch it;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, ObjectInteractEventArgs> { };

        /// <summary>
        /// Emits the ControllerTouchInteractableObject class event.
        /// </summary>
        public UnityObjectEvent OnControllerTouchInteractableObject = new UnityObjectEvent();
        /// <summary>
        /// Emits the ControllerUntouchInteractableObject class event.
        /// </summary>
        public UnityObjectEvent OnControllerUntouchInteractableObject = new UnityObjectEvent();

        private void SetInteractTouch()
        {
            if (it == null)
            {
                it = GetComponent<VRTK_InteractTouch>();
            }
        }

        private void OnEnable()
        {
            SetInteractTouch();
            if (it == null)
            {
                Debug.LogError("The VRTK_InteractTouch_UnityEvents script requires to be attached to a GameObject that contains a VRTK_InteractTouch script");
                return;
            }

            it.ControllerTouchInteractableObject += ControllerTouchInteractableObject;
            it.ControllerUntouchInteractableObject += ControllerUntouchInteractableObject;
        }

        private void ControllerTouchInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerTouchInteractableObject.Invoke(o, e);
        }

        private void ControllerUntouchInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerUntouchInteractableObject.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (it == null)
            {
                return;
            }

            it.ControllerTouchInteractableObject -= ControllerTouchInteractableObject;
            it.ControllerUntouchInteractableObject -= ControllerUntouchInteractableObject;
        }
    }
}