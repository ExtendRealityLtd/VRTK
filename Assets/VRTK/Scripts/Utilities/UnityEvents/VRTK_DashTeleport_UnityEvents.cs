namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_DashTeleport))]
    public class VRTK_DashTeleport_UnityEvents : MonoBehaviour
    {
        private VRTK_DashTeleport dt;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, DashTeleportEventArgs> { };

        /// <summary>
        /// Emits the WillDashThruObjects class event.
        /// </summary>
        public UnityObjectEvent OnWillDashThruObjects;
        /// <summary>
        /// Emits the DashedThruObjects class event.
        /// </summary>
        public UnityObjectEvent OnDashedThruObjects;

        private void SetDashTeleport()
        {
            if (dt == null)
            {
                dt = GetComponent<VRTK_DashTeleport>();
            }
        }

        private void OnEnable()
        {
            SetDashTeleport();
            if (dt == null)
            {
                Debug.LogError("The VRTK_DashTeleport_UnityEvents script requires to be attached to a GameObject that contains a VRTK_DashTeleport script");
                return;
            }

            dt.WillDashThruObjects += WillDashThruObjects;
            dt.DashedThruObjects += DashedThruObjects;
        }

        private void WillDashThruObjects(object o, DashTeleportEventArgs e)
        {
            OnWillDashThruObjects.Invoke(o, e);
        }

        private void DashedThruObjects(object o, DashTeleportEventArgs e)
        {
            OnDashedThruObjects.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (dt == null)
            {
                return;
            }

            dt.WillDashThruObjects -= WillDashThruObjects;
            dt.DashedThruObjects -= DashedThruObjects;
        }
    }
}