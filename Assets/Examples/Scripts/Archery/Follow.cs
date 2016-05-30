﻿using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {
    public bool followPosition;
    public bool followRotation;
    public Transform target;

    private void Update()
    {
        if (target != null)
        {
            if (followRotation)
            {
                transform.rotation = target.rotation;
            }

            if (followPosition)
            {
                transform.position = target.position;
            }
        }
        else
        {
            Debug.LogError("No follow target defined!");
        }
    }
}
