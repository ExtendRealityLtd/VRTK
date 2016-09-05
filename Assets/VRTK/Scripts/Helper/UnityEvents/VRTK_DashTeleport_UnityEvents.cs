using UnityEngine;
using UnityEngine.Events;
using VRTK;

[RequireComponent(typeof(VRTK_DashTeleport))]
public class VRTK_DashTeleport_UnityEvents : MonoBehaviour
{
    [System.Serializable]
    public class UnityObjectEvent : UnityEvent<DashTeleportEventArgs> { };
    public UnityObjectEvent OnWillDashThruObjects;
    public UnityObjectEvent OnDashedThruObjects;

    private void OnEnable()
    {
        VRTK_DashTeleport.WillDashThruObjects += WillDashThruObjects;
        VRTK_DashTeleport.DashedThruObjects += DashedThruObjects;
    }

    private void WillDashThruObjects(object o, DashTeleportEventArgs e)
    {
        OnWillDashThruObjects.Invoke(e);
    }

    private void DashedThruObjects(object o, DashTeleportEventArgs e)
    {
        OnDashedThruObjects.Invoke(e);
    }

    private void OnDisable()
    {
        VRTK_DashTeleport.WillDashThruObjects -= WillDashThruObjects;
        VRTK_DashTeleport.DashedThruObjects -= DashedThruObjects;
    }
}