namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_ControllerActions))]
    public class VRTK_ControllerActions_UnityEvents : MonoBehaviour
    {
        private VRTK_ControllerActions ca;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, ControllerActionsEventArgs> { };

        /// <summary>
        /// Emits the ControllerModelVisible class event.
        /// </summary>
        public UnityObjectEvent OnControllerModelVisible = new UnityObjectEvent();
        /// <summary>
        /// Emits the ControllerModelInvisible class event.
        /// </summary>
        public UnityObjectEvent OnControllerModelInvisible = new UnityObjectEvent();

        private void SetControllerAction()
        {
            if (ca == null)
            {
                ca = GetComponent<VRTK_ControllerActions>();
            }
        }

        private void OnEnable()
        {
            SetControllerAction();
            if (ca == null)
            {
                Debug.LogError("The VRTK_ControllerActions_UnityEvents script requires to be attached to a GameObject that contains a VRTK_ControllerActions script");
                return;
            }

            ca.ControllerModelVisible += ControllerModelVisible;
            ca.ControllerModelInvisible += ControllerModelInvisible;
        }

        private void ControllerModelVisible(object o, ControllerActionsEventArgs e)
        {
            OnControllerModelVisible.Invoke(o, e);
        }

        private void ControllerModelInvisible(object o, ControllerActionsEventArgs e)
        {
            OnControllerModelInvisible.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (ca == null)
            {
                return;
            }

            ca.ControllerModelVisible -= ControllerModelVisible;
            ca.ControllerModelInvisible -= ControllerModelInvisible;
        }
    }
}