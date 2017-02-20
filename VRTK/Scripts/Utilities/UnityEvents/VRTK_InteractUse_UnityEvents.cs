namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_InteractUse))]
    public class VRTK_InteractUse_UnityEvents : MonoBehaviour
    {
        private VRTK_InteractUse iu;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, ObjectInteractEventArgs> { };

        /// <summary>
        /// Emits the ControllerUseInteractableObject class event.
        /// </summary>
        public UnityObjectEvent OnControllerUseInteractableObject = new UnityObjectEvent();
        /// <summary>
        /// Emits the ControllerUnuseInteractableObject class event.
        /// </summary>
        public UnityObjectEvent OnControllerUnuseInteractableObject = new UnityObjectEvent();

        private void SetInteractUse()
        {
            if (iu == null)
            {
                iu = GetComponent<VRTK_InteractUse>();
            }
        }

        private void OnEnable()
        {
            SetInteractUse();
            if (iu == null)
            {
                Debug.LogError("The VRTK_InteractUse_UnityEvents script requires to be attached to a GameObject that contains a VRTK_InteractUse script");
                return;
            }

            iu.ControllerUseInteractableObject += ControllerUseInteractableObject;
            iu.ControllerUnuseInteractableObject += ControllerUnuseInteractableObject;
        }

        private void ControllerUseInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerUseInteractableObject.Invoke(o, e);
        }

        private void ControllerUnuseInteractableObject(object o, ObjectInteractEventArgs e)
        {
            OnControllerUnuseInteractableObject.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (iu == null)
            {
                return;
            }

            iu.ControllerUseInteractableObject -= ControllerUseInteractableObject;
            iu.ControllerUnuseInteractableObject -= ControllerUnuseInteractableObject;
        }
    }
}