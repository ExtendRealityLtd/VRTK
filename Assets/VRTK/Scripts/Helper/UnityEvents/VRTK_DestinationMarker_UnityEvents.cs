﻿using UnityEngine;
using UnityEngine.Events;
using VRTK;

[RequireComponent(typeof(VRTK_DestinationMarker))]
public class VRTK_DestinationMarker_UnityEvents : MonoBehaviour
{
    private VRTK_DestinationMarker dm;

    [System.Serializable]
    public class UnityObjectEvent : UnityEvent<DestinationMarkerEventArgs> { };
    public UnityObjectEvent OnDestinationMarkerEnter;
    public UnityObjectEvent OnDestinationMarkerExit;
    public UnityObjectEvent OnDestinationMarkerSet;

    private void SetDestinationMarker()
    {
        if (dm == null)
        {
            dm = GetComponent<VRTK_DestinationMarker>();
        }
    }

    private void OnEnable()
    {
        SetDestinationMarker();
        if (dm == null)
        {
            Debug.LogError("The VRTK_DestinationMarker_UnityEvents script requires to be attached to a GameObject that contains a VRTK_DestinationMarker script");
            return;
        }

        dm.DestinationMarkerEnter += DestinationMarkerEnter;
        dm.DestinationMarkerExit += DestinationMarkerExit;
        dm.DestinationMarkerSet += DestinationMarkerSet;
    }

    private void DestinationMarkerEnter(object o, DestinationMarkerEventArgs e)
    {
        OnDestinationMarkerEnter.Invoke(e);
    }

    private void DestinationMarkerExit(object o, DestinationMarkerEventArgs e)
    {
        OnDestinationMarkerExit.Invoke(e);
    }

    private void DestinationMarkerSet(object o, DestinationMarkerEventArgs e)
    {
        OnDestinationMarkerSet.Invoke(e);
    }

    private void OnDisable()
    {
        if (dm == null)
        {
            return;
        }

        dm.DestinationMarkerEnter -= DestinationMarkerEnter;
        dm.DestinationMarkerExit -= DestinationMarkerExit;
        dm.DestinationMarkerSet -= DestinationMarkerSet;
    }
}