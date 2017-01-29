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
                Debug.LogError("The VRTK_HeadsetCollision_UnityEvents script requires to be attached to a GameObject that contains a VRTK_HeadsetCollision script");
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