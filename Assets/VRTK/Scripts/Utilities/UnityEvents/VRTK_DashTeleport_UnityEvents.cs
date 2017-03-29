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
        public UnityObjectEvent OnWillDashThruObjects = new UnityObjectEvent();
        /// <summary>
        /// Emits the DashedThruObjects class event.
        /// </summary>
        public UnityObjectEvent OnDashedThruObjects = new UnityObjectEvent();

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
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, new string[] { "VRTK_DashTeleport_UnityEvents", "VRTK_DashTeleport", "the same" }));
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