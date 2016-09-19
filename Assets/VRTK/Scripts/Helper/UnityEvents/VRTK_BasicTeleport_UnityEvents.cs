namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_BasicTeleport))]
    public class VRTK_BasicTeleport_UnityEvents : MonoBehaviour
    {
        private VRTK_BasicTeleport bt;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, DestinationMarkerEventArgs> { };

        /// <summary>
        /// Emits the Teleporting class event.
        /// </summary>
        public UnityObjectEvent OnTeleporting;
        /// <summary>
        /// Emits the Teleported class event.
        /// </summary>
        public UnityObjectEvent OnTeleported;

        private void SetBasicTeleport()
        {
            if (bt == null)
            {
                bt = GetComponent<VRTK_BasicTeleport>();
            }
        }

        private void OnEnable()
        {
            SetBasicTeleport();
            if (bt == null)
            {
                Debug.LogError("The VRTK_BasicTeleport_UnityEvents script requires to be attached to a GameObject that contains a VRTK_BasicTeleport script");
                return;
            }

            bt.Teleporting += Teleporting;
            bt.Teleported += Teleported;
        }

        private void Teleporting(object o, DestinationMarkerEventArgs e)
        {
            OnTeleporting.Invoke(o, e);
        }

        private void Teleported(object o, DestinationMarkerEventArgs e)
        {
            OnTeleported.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (bt == null)
            {
                return;
            }

            bt.Teleporting += Teleporting;
            bt.Teleported += Teleported;
        }
    }
}