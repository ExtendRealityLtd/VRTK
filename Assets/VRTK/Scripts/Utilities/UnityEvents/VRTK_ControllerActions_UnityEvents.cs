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
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, new string[] { "VRTK_ControllerActions_UnityEvents", "VRTK_ControllerActions", "the same" }));
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