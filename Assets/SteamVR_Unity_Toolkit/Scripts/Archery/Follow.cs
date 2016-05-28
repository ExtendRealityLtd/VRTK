using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {

    public bool followPos;
    public bool followRot;

    public Transform target;

	// Update is called once per frame
	void Update ()
    {
        if (target != null)
        {
            if (followRot)
                transform.rotation = target.rotation;
            if (followPos)
                transform.position = target.position;
        }
        else
            Debug.LogError("No follow target defined!");
	}
}
