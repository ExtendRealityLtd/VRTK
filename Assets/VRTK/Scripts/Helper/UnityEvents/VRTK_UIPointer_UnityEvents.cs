﻿using UnityEngine;
using UnityEngine.Events;
using VRTK;

[RequireComponent(typeof(VRTK_UIPointer))]
public class VRTK_UIPointer_UnityEvents : MonoBehaviour
{
    private VRTK_UIPointer uip;

    [System.Serializable]
    public class UnityObjectEvent : UnityEvent<UIPointerEventArgs> { };
    public UnityObjectEvent OnUIPointerElementEnter;
    public UnityObjectEvent OnUIPointerElementExit;

    private void SetUIPointer()
    {
        if (uip == null)
        {
            uip = GetComponent<VRTK_UIPointer>();
        }
    }

    private void OnEnable()
    {
        SetUIPointer();
        if (uip == null)
        {
            Debug.LogError("The VRTK_UIPointer_UnityEvents script requires to be attached to a GameObject that contains a VRTK_UIPointer script");
            return;
        }
        uip.UIPointerElementEnter += UIPointerElementEnter;
        uip.UIPointerElementExit += UIPointerElementExit;
    }

    private void UIPointerElementEnter(object o, UIPointerEventArgs e)
    {
        OnUIPointerElementEnter.Invoke(e);
    }

    private void UIPointerElementExit(object o, UIPointerEventArgs e)
    {
        OnUIPointerElementExit.Invoke(e);
    }

    private void OnDisable()
    {
        if (uip == null)
        {
            return;
        }

        uip.UIPointerElementEnter -= UIPointerElementEnter;
        uip.UIPointerElementExit -= UIPointerElementExit;
    }
}