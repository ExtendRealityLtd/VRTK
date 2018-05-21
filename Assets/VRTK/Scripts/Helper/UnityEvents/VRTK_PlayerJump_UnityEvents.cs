namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_PlayerJump))]
    public class VRTK_PlayerJump_UnityEvents : MonoBehaviour
    {
        private VRTK_PlayerJump pj;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, PlayerJumpEventArgs> { };

        /// <summary>
        /// Emits the PlayerJumpStarted class event.
        /// </summary>
        public UnityObjectEvent OnPlayerJumpStarted;
        /// <summary>
        /// Emits the PlayerJumpEnded class event.
        /// </summary>
        public UnityObjectEvent OnPlayerJumpEnded;

        private void SetPlayerJump()
        {
            if (pj == null)
            {
                pj = GetComponent<VRTK_PlayerJump>();
            }
        }

        private void OnEnable()
        {
            SetPlayerJump();
            if (pj == null)
            {
                Debug.LogError("The VRTK_PlayerJump_UnityEvents script requires to be attached to a GameObject that contains a VRTK_PlayerJump script");
                return;
            }

            pj.PlayerJumpStarted += PlayerJumpStarted;
            pj.PlayerJumpEnded += PlayerJumpEnded;
        }

        private void PlayerJumpStarted(object o, PlayerJumpEventArgs e)
        {
            OnPlayerJumpStarted.Invoke(o, e);
        }

        private void PlayerJumpEnded(object o, PlayerJumpEventArgs e)
        {
            OnPlayerJumpEnded.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (pj == null)
            {
                return;
            }
            pj.PlayerJumpStarted -= PlayerJumpStarted;
            pj.PlayerJumpEnded -= PlayerJumpEnded;
        }
    }
}