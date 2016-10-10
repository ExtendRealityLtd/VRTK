namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_HeadsetControllerAware))]
    public class VRTK_HeadsetControllerAware_UnityEvents : MonoBehaviour
    {
        private VRTK_HeadsetControllerAware hca;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, HeadsetControllerAwareEventArgs> { };

        /// <summary>
        /// Emits the ControllerObscured class event.
        /// </summary>
        public UnityObjectEvent OnControllerObscured;
        /// <summary>
        /// Emits the ControllerUnobscured class event.
        /// </summary>
        public UnityObjectEvent OnControllerUnobscured;

        /// <summary>
        /// Emits the ControllerGlanceEnter class event.
        /// </summary>
        public UnityObjectEvent OnControllerGlanceEnter;
        /// <summary>
        /// Emits the ControllerGlanceExit class event.
        /// </summary>
        public UnityObjectEvent OnControllerGlanceExit;

        private void SetHeadsetControllerAware()
        {
            if (hca == null)
            {
                hca = GetComponent<VRTK_HeadsetControllerAware>();
            }
        }

        private void OnEnable()
        {
            SetHeadsetControllerAware();
            if (hca == null)
            {
                Debug.LogError("The VRTK_HeadsetControllerAware_UnityEvents script requires to be attached to a GameObject that contains a VRTK_HeadsetControllerAware script");
                return;
            }

            hca.ControllerObscured += ControllerObscured;
            hca.ControllerUnobscured += ControllerUnobscured;
            hca.ControllerGlanceEnter += ControllerGlanceEnter;
            hca.ControllerGlanceExit += ControllerGlanceExit;
        }

        private void ControllerObscured(object o, HeadsetControllerAwareEventArgs e)
        {
            OnControllerObscured.Invoke(o, e);
        }

        private void ControllerUnobscured(object o, HeadsetControllerAwareEventArgs e)
        {
            OnControllerUnobscured.Invoke(o, e);
        }

        private void ControllerGlanceEnter(object o, HeadsetControllerAwareEventArgs e)
        {
            OnControllerGlanceEnter.Invoke(o, e);
        }

        private void ControllerGlanceExit(object o, HeadsetControllerAwareEventArgs e)
        {
            OnControllerGlanceExit.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (hca == null)
            {
                return;
            }

            hca.ControllerObscured -= ControllerObscured;
            hca.ControllerUnobscured -= ControllerUnobscured;
            hca.ControllerGlanceEnter -= ControllerGlanceEnter;
            hca.ControllerGlanceExit -= ControllerGlanceExit;
        }
    }
}