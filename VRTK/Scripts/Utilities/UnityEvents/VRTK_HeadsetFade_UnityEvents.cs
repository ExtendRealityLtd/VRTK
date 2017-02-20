namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_HeadsetFade))]
    public class VRTK_HeadsetFade_UnityEvents : MonoBehaviour
    {
        private VRTK_HeadsetFade hf;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, HeadsetFadeEventArgs> { };

        /// <summary>
        /// Emits the HeadsetFadeStart class event.
        /// </summary>
        public UnityObjectEvent OnHeadsetFadeStart = new UnityObjectEvent();
        /// <summary>
        /// Emits the HeadsetFadeComplete class event.
        /// </summary>
        public UnityObjectEvent OnHeadsetFadeComplete = new UnityObjectEvent();
        /// <summary>
        /// Emits the HeadsetUnfadeStart class event.
        /// </summary>
        public UnityObjectEvent OnHeadsetUnfadeStart = new UnityObjectEvent();
        /// <summary>
        /// Emits the HeadsetUnfadeComplete class event.
        /// </summary>
        public UnityObjectEvent OnHeadsetUnfadeComplete = new UnityObjectEvent();

        private void SetHeadsetFade()
        {
            if (hf == null)
            {
                hf = GetComponent<VRTK_HeadsetFade>();
            }
        }

        private void OnEnable()
        {
            SetHeadsetFade();
            if (hf == null)
            {
                Debug.LogError("The VRTK_HeadsetFade_UnityEvents script requires to be attached to a GameObject that contains a VRTK_HeadsetFade script");
                return;
            }

            hf.HeadsetFadeStart += HeadsetFadeStart;
            hf.HeadsetFadeComplete += HeadsetFadeComplete;
            hf.HeadsetUnfadeStart += HeadsetUnfadeStart;
            hf.HeadsetUnfadeComplete += HeadsetUnfadeComplete;
        }

        private void HeadsetFadeStart(object o, HeadsetFadeEventArgs e)
        {
            OnHeadsetFadeStart.Invoke(o, e);
        }

        private void HeadsetFadeComplete(object o, HeadsetFadeEventArgs e)
        {
            OnHeadsetFadeComplete.Invoke(o, e);
        }

        private void HeadsetUnfadeStart(object o, HeadsetFadeEventArgs e)
        {
            OnHeadsetUnfadeStart.Invoke(o, e);
        }

        private void HeadsetUnfadeComplete(object o, HeadsetFadeEventArgs e)
        {
            OnHeadsetUnfadeComplete.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (hf == null)
            {
                return;
            }

            hf.HeadsetFadeStart -= HeadsetFadeStart;
            hf.HeadsetFadeComplete -= HeadsetFadeComplete;
            hf.HeadsetUnfadeStart -= HeadsetUnfadeStart;
            hf.HeadsetUnfadeComplete -= HeadsetUnfadeComplete;
        }
    }
}