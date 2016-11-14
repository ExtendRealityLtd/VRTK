namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_PlayerPresence))]
    public class VRTK_PlayerPresence_UnityEvents : MonoBehaviour
    {
        private VRTK_PlayerPresence pp;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, PlayerPresenceEventArgs> { };

        /// <summary>
        /// Emits the PresenceFallStarted class event.
        /// </summary>
        public UnityObjectEvent OnPresenceFallStarted;
        /// <summary>
        /// Emits the PresenceFallEnded class event.
        /// </summary>
        public UnityObjectEvent OnPresenceFallEnded;

        private void SetPlayerPresence()
        {
            if (pp == null)
            {
                pp = GetComponent<VRTK_PlayerPresence>();
            }
        }

        private void OnEnable()
        {
            SetPlayerPresence();
            if (pp == null)
            {
                Debug.LogError("The VRTK_PlayerPresence_UnityEvents script requires to be attached to a GameObject that contains a VRTK_PlayerPresence script");
                return;
            }

            pp.PresenceFallStarted += PresenceFallStarted;
            pp.PresenceFallEnded += PresenceFallEnded;
        }

        private void PresenceFallStarted(object o, PlayerPresenceEventArgs e)
        {
            OnPresenceFallStarted.Invoke(o, e);
        }

        private void PresenceFallEnded(object o, PlayerPresenceEventArgs e)
        {
            OnPresenceFallEnded.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (pp == null)
            {
                return;
            }

            pp.PresenceFallStarted -= PresenceFallStarted;
            pp.PresenceFallEnded -= PresenceFallEnded;
        }
    }
}