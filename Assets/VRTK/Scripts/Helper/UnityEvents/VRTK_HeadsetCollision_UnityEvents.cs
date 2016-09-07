﻿using UnityEngine;
using UnityEngine.Events;
using VRTK;

[RequireComponent(typeof(VRTK_HeadsetCollision))]
public class VRTK_HeadsetCollision_UnityEvents : MonoBehaviour
{
    private VRTK_HeadsetCollision hc;

    [System.Serializable]
    public class UnityObjectEvent : UnityEvent<HeadsetCollisionEventArgs> { };
    public UnityObjectEvent OnHeadsetCollisionDetect;
    public UnityObjectEvent OnHeadsetCollisionEnded;

    private void SetHeadsetCollision()
    {
        if (hc == null)
        {
            hc = GetComponent<VRTK_HeadsetCollision>();
        }
    }

    private void OnEnable()
    {
        SetHeadsetCollision();
        if (hc == null)
        {
            Debug.LogError("The VRTK_HeadsetCollision_UnityEvents script requires to be attached to a GameObject that contains a VRTK_HeadsetCollision script");
            return;
        }

        hc.HeadsetCollisionDetect += HeadsetCollisionDetect;
        hc.HeadsetCollisionEnded += HeadsetCollisionEnded;
    }

    private void HeadsetCollisionDetect(object o, HeadsetCollisionEventArgs e)
    {
        OnHeadsetCollisionDetect.Invoke(e);
    }

    private void HeadsetCollisionEnded(object o, HeadsetCollisionEventArgs e)
    {
        OnHeadsetCollisionEnded.Invoke(e);
    }

    private void OnDisable()
    {
        if (hc == null)
        {
            return;
        }

        hc.HeadsetCollisionDetect -= HeadsetCollisionDetect;
        hc.HeadsetCollisionEnded -= HeadsetCollisionEnded;
    }
}