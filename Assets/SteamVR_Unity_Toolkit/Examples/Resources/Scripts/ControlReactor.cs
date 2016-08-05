﻿using UnityEngine;
using VRTK;

public class ControlReactor : MonoBehaviour
{
    public TextMesh go;

    private void Start()
    {
        GetComponent<VRTK_Control>().defaultEvents.OnValueChanged.AddListener(HandleChange);
        go.text = GetComponent<VRTK_Control>().getValue().ToString();
    }

    private void HandleChange(float value)
    {
        go.text = value.ToString();
    }
}