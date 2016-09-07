﻿using UnityEngine;
using UnityEngine.Events;
using VRTK;

[RequireComponent(typeof(VRTK_DashTeleport))]
public class VRTK_DashTeleport_UnityEvents : MonoBehaviour
{
    private VRTK_DashTeleport dt;

    [System.Serializable]
    public class UnityObjectEvent : UnityEvent<DashTeleportEventArgs> { };
    public UnityObjectEvent OnWillDashThruObjects;
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
        OnWillDashThruObjects.Invoke(e);
    }

    private void DashedThruObjects(object o, DashTeleportEventArgs e)
    {
        OnDashedThruObjects.Invoke(e);
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