using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VRTK_InteractiveHapticsRigidbodyInput : VRTK_InteractiveHapticsInput {

    public float minMagnitude;
    public float maxMagnitude;

    private float lastMagnitude = 0.0f;

	void FixedUpdate () {
        float currentMagnitude = GetComponent<Rigidbody>().velocity.magnitude;

        if(lastMagnitude != currentMagnitude)
        {
            lastMagnitude = currentMagnitude;

            if (currentMagnitude >= minMagnitude)
            {
                float normalizedMagnitude = Mathf.Min(1, (currentMagnitude - minMagnitude) / (maxMagnitude - minMagnitude));
                OnInputProvided(normalizedMagnitude);
            }
        }		
	}
}
