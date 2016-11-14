namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_PlayerClimb))]
    public class VRTK_PlayerClimb_UnityEvents : MonoBehaviour
    {
        private VRTK_PlayerClimb pc;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, PlayerClimbEventArgs> { };

        /// <summary>
        /// Emits the PlayerClimbStarted class event.
        /// </summary>
        public UnityObjectEvent OnPlayerClimbStarted;
        /// <summary>
        /// Emits the PlayerClimbEnded class event.
        /// </summary>
        public UnityObjectEvent OnPlayerClimbEnded;

        private void SetPlayerClimb()
        {
            if (pc == null)
            {
                pc = GetComponent<VRTK_PlayerClimb>();
            }
        }

        private void OnEnable()
        {
            SetPlayerClimb();
            if (pc == null)
            {
                Debug.LogError("The VRTK_PlayerClimb_UnityEvents script requires to be attached to a GameObject that contains a VRTK_PlayerClimb script");
                return;
            }

            pc.PlayerClimbStarted += PlayerClimbStarted;
            pc.PlayerClimbEnded += PlayerClimbEnded;
        }

        private void PlayerClimbStarted(object o, PlayerClimbEventArgs e)
        {
            OnPlayerClimbStarted.Invoke(o, e);
        }

        private void PlayerClimbEnded(object o, PlayerClimbEventArgs e)
        {
            OnPlayerClimbEnded.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (pc == null)
            {
                return;
            }

            pc.PlayerClimbStarted -= PlayerClimbStarted;
            pc.PlayerClimbEnded -= PlayerClimbEnded;
        }
    }
}