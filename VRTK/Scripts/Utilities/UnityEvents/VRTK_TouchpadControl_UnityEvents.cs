namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_TouchpadControl))]
    public class VRTK_TouchpadControl_UnityEvents : MonoBehaviour
    {
        private VRTK_TouchpadControl tc;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, TouchpadControlEventArgs> { };

        /// <summary>
        /// Emits the XAxisChanged class event.
        /// </summary>
        public UnityObjectEvent OnXAxisChanged = new UnityObjectEvent();

        /// <summary>
        /// Emits the YAxisChanged class event.
        /// </summary>
        public UnityObjectEvent OnYAxisChanged = new UnityObjectEvent();

        private void SetTouchpadControl()
        {
            if (tc == null)
            {
                tc = GetComponent<VRTK_TouchpadControl>();
            }
        }

        private void OnEnable()
        {
            SetTouchpadControl();
            if (tc == null)
            {
                Debug.LogError("The VRTK_TouchpadControl_UnityEvents script requires to be attached to a GameObject that contains a VRTK_TouchpadControl script");
                return;
            }

            tc.XAxisChanged += XAxisChanged;
            tc.YAxisChanged += YAxisChanged;
        }

        private void XAxisChanged(object o, TouchpadControlEventArgs e)
        {
            OnXAxisChanged.Invoke(o, e);
        }

        private void YAxisChanged(object o, TouchpadControlEventArgs e)
        {
            OnYAxisChanged.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (tc == null)
            {
                return;
            }

            tc.XAxisChanged -= XAxisChanged;
            tc.YAxisChanged -= YAxisChanged;
        }
    }
}