using UnityEngine;
using System.Collections;

public class SteamVR_HeadsetCollisionFade : MonoBehaviour {
    public float blinkTransitionSpeed = 0.1f;
    public Color fadeColor = Color.black;

    void Start () {
        BoxCollider collider = this.gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(0.1f, 0.1f, 0.1f);
        
        Rigidbody rb = this.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
	}

    void OnTriggerStay(Collider collider)
    {
        SteamVR_Fade.Start(fadeColor, blinkTransitionSpeed);
    }

    void OnTriggerExit(Collider collider)
    {
        SteamVR_Fade.Start(Color.clear, blinkTransitionSpeed);
    }
}
