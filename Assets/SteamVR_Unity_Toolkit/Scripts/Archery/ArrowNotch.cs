using UnityEngine;
using System.Collections;

public class ArrowNotch : MonoBehaviour {

    public GameObject arrow;
    SteamVR_InteractableObject obj;

    void Start()
    {
        obj = GetComponent<SteamVR_InteractableObject>();
    }

	void OnTriggerEnter(Collider col)
    {
        var handle = col.GetComponent<BowHandle>();
        if (handle != null && handle.aim.isHeld() && obj.IsGrabbed())
        {
            arrow.transform.SetParent(handle.ArrowNotch);
            col.GetComponentInParent<BowAim>().SetArrow(arrow);
            Destroy(gameObject);
        }
    }

}
