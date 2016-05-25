using UnityEngine;
using System.Collections;

public struct HeadsetCollisionEventArgs
{
    public Collider collider;
    public Transform currentTransform;
}

public delegate void HeadsetCollisionEventHandler(object sender, HeadsetCollisionEventArgs e);

public class SteamVR_HeadsetCollisionFade : MonoBehaviour {
    public float blinkTransitionSpeed = 0.1f;
    public Color fadeColor = Color.black;

    public event HeadsetCollisionEventHandler HeadsetCollisionDetect;

    public virtual void OnHeadsetCollisionDetect(HeadsetCollisionEventArgs e)
    {
        if (HeadsetCollisionDetect != null)
            HeadsetCollisionDetect(this, e);
    }

    protected HeadsetCollisionEventArgs SetHeadsetCollisionEvent(Collider collider, Transform currentTransform)
    {
        HeadsetCollisionEventArgs e;
        e.collider = collider;
        e.currentTransform = currentTransform;
        return e;
    }

    protected void Start () {
        this.name = "PlayerObject_" + this.name;
        BoxCollider collider = this.gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(0.1f, 0.1f, 0.1f);
        
        Rigidbody rb = this.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
	}

    protected void OnTriggerStay(Collider collider)
    {
        if (!collider.name.Contains("PlayerObject_"))
        {
            OnHeadsetCollisionDetect(SetHeadsetCollisionEvent(collider, this.transform));
            SteamVR_Fade.Start(fadeColor, blinkTransitionSpeed);
        }
    }

    protected void OnTriggerExit(Collider collider)
    {
        if (!collider.name.Contains("PlayerObject_"))
        {
            SteamVR_Fade.Start(Color.clear, blinkTransitionSpeed);
        }
    }
}
