namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_InteractGrab))]
    public class VRTK_InteractGrab_UnityEvents : MonoBehaviour
    {
        private VRTK_InteractGrab ig;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, ObjectInteractEventArgs> { };

        /// <summary>
        /// Emits the ControllerGrabInteractableObject class event.
        /// </summary>
        public UnityObjectEvent OnControllerGrabInteractableObject = new UnityObjectEvent();
        /// <summary>
        /// Emits the ControllerUngrabInteractableObject class event.
        /// </summary>
        public UnityObjectEvent OnControllerUngrabInteractableObject = new UnityObjectEvent();

        private void SetInteractGrab()
        {
            if (ig == null)
            {
                ig = GetComponent<VRTK_InteractGrab>();
            }
        }

        private void OnEnable()
        {
            SetInteractGrab();
            if (ig == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, new string[] { "VRTK_InteractGrab_UnityEvents", "VRTK_InteractGrab", "the same" }));
                return;
            }

            ig.ControllerGrabInteractableObject += ControllerGrabInteractableObject;
            ig.ControllerUngrabInteractableObject += ControllerUngrabInteractableObject;
        }

        private void ControllerGrabInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerGrabInteractableObject.Invoke(o, e);
        }

        private void ControllerUngrabInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerUngrabInteractableObject.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (ig == null)
            {
                return;
            }

            ig.ControllerGrabInteractableObject -= ControllerGrabInteractableObject;
            ig.ControllerUngrabInteractableObject -= ControllerUngrabInteractableObject;
        }
    }
}