namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_ObjectControl))]
    public class VRTK_ObjectControl_UnityEvents : MonoBehaviour
    {
        private VRTK_ObjectControl oc;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, ObjectControlEventArgs> { };

        /// <summary>
        /// Emits the XAxisChanged class event.
        /// </summary>
        public UnityObjectEvent OnXAxisChanged = new UnityObjectEvent();

        /// <summary>
        /// Emits the YAxisChanged class event.
        /// </summary>
        public UnityObjectEvent OnYAxisChanged = new UnityObjectEvent();

        private void SetObjectControl()
        {
            if (oc == null)
            {
                oc = GetComponent<VRTK_ObjectControl>();
            }
        }

        private void OnEnable()
        {
            SetObjectControl();
            if (oc == null)
            {
                Debug.LogError("The VRTK_ObjectControl_UnityEvents script requires to be attached to a GameObject that contains a VRTK_ObjectControl script");
                return;
            }

            oc.XAxisChanged += XAxisChanged;
            oc.YAxisChanged += YAxisChanged;
        }

        private void XAxisChanged(object o, ObjectControlEventArgs e)
        {
            OnXAxisChanged.Invoke(o, e);
        }

        private void YAxisChanged(object o, ObjectControlEventArgs e)
        {
            OnYAxisChanged.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (oc == null)
            {
                return;
            }

            oc.XAxisChanged -= XAxisChanged;
            oc.YAxisChanged -= YAxisChanged;
        }
    }
}