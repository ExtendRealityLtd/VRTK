namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_HeadsetCollision))]
    public class VRTK_HeadsetCollision_UnityEvents : MonoBehaviour
    {
        private VRTK_HeadsetCollision hc;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, HeadsetCollisionEventArgs> { };

        /// <summary>
        /// Emits the HeadsetCollisionDetect class event.
        /// </summary>
        public UnityObjectEvent OnHeadsetCollisionDetect = new UnityObjectEvent();
        /// <summary>
        /// Emits the HeadsetCollisionEnded class event.
        /// </summary>
        public UnityObjectEvent OnHeadsetCollisionEnded = new UnityObjectEvent();

        private void SetHeadsetCollision()
        {
            if (hc == null)
            {
                hc = GetComponent<VRTK_HeadsetCollision>();
            }
        }

        private void OnEnable()
        {
            SetHeadsetCollision();
            if (hc == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, new string[] { "VRTK_HeadsetCollision_UnityEvents", "VRTK_HeadsetCollision", "the same" }));
                return;
            }

            hc.HeadsetCollisionDetect += HeadsetCollisionDetect;
            hc.HeadsetCollisionEnded += HeadsetCollisionEnded;
        }

        private void HeadsetCollisionDetect(object o, HeadsetCollisionEventArgs e)
        {
            OnHeadsetCollisionDetect.Invoke(o, e);
        }

        private void HeadsetCollisionEnded(object o, HeadsetCollisionEventArgs e)
        {
            OnHeadsetCollisionEnded.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (hc == null)
            {
                return;
            }

            hc.HeadsetCollisionDetect -= HeadsetCollisionDetect;
            hc.HeadsetCollisionEnded -= HeadsetCollisionEnded;
        }
    }
}