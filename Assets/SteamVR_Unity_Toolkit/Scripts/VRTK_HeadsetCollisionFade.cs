using UnityEngine;
using System.Collections;

public struct HeadsetCollisionEventArgs
{
    public Collider collider;
    public Transform currentTransform;
}

public delegate void HeadsetCollisionEventHandler(object sender, HeadsetCollisionEventArgs e);

public class VRTK_HeadsetCollisionFade : MonoBehaviour {
    public float blinkTransitionSpeed = 0.1f;
    public Color fadeColor = Color.black;

    public event HeadsetCollisionEventHandler HeadsetCollisionDetect;
    public event HeadsetCollisionEventHandler HeadsetCollisionEnded;

    public virtual void OnHeadsetCollisionDetect(HeadsetCollisionEventArgs e)
    {
        if (HeadsetCollisionDetect != null)
            HeadsetCollisionDetect(this, e);
    }

    public virtual void OnHeadsetCollisionEnded(HeadsetCollisionEventArgs e)
    {
        if (HeadsetCollisionEnded != null)
            HeadsetCollisionEnded(this, e);
    }

    protected HeadsetCollisionEventArgs SetHeadsetCollisionEvent(Collider collider, Transform currentTransform)
    {
        HeadsetCollisionEventArgs e;
        e.collider = collider;
        e.currentTransform = currentTransform;
        return e;
    }

    protected void Start () {
        if (gameObject.GetComponentInChildren<SteamVR_Fade>() == null)
        {
            Debug.LogWarning("This 'VRTK_HeadsetCollisionFade' script needs a SteamVR_Fade script on the camera eye.");
        }

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
            OnHeadsetCollisionEnded(SetHeadsetCollisionEvent(collider, this.transform));
            SteamVR_Fade.Start(Color.clear, blinkTransitionSpeed);
        }
    }
}
