using UnityEngine;
using System.Collections;

public class Quiver : MonoBehaviour {

    BowAim Bow;
    public GameObject ArrowPrefab;

    public GameObject VRHead;

    float internalCD = 0.15f;
    float cd;

    void Start()
    {
        Bow = GetComponent<BowAim>();
        if (VRHead == null)
            VRHead = FindObjectOfType<SteamVR_GameView>().gameObject;
    }

    void Update()
    {
        if (Bow.isHeld() && !Bow.hasArrow() && Bow.getPullHand().triggerDown && cd <= 0 && BehindHead(Bow.getPullHand().transform.position))
        {
            if (!Bow.getPullHand().gameObject.GetComponent<SteamVR_InteractGrab>().hasObject())
            {
                cd = internalCD;
                GameObject newArrow = Instantiate(ArrowPrefab);
                Bow.getPullHand().GetComponent<SteamVR_InteractTouch>().ForceTouch(newArrow);
                Bow.getPullHand().GetComponent<SteamVR_InteractGrab>().GrabInteractedObject();
            }
        }
        
        if (cd > 0)
            cd -= Time.deltaTime;
    }

    bool BehindHead(Vector3 pos)
    {
        float inFront = Vector3.Dot(VRHead.transform.TransformDirection(Vector3.forward), (VRHead.transform.position - pos).normalized);
        if (pos.y > VRHead.transform.position.y - 0.1f && inFront > .5f)
        {
            return true;
        }
        return false;
            
    }
}
