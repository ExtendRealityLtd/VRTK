using UnityEngine;
using System.Collections;

public class BowAim : MonoBehaviour {



    public float PowerMult;

    [Header("Bow Animation Values", order = 1)]
    public float AnimPullMult;
    public float PullOffset;
    BowAnim Anim;

    bool LeftHand = true;
    GameObject currentArrow;
    BowHandle handle;
    SteamVR_InteractableObject interact;

    SteamVR_ControllerManager ctrls;
    SteamVR_ControllerEvents HoldControl;
    SteamVR_ControllerEvents StringControl;

    public SteamVR_ControllerEvents getPullHand()
    {
        return StringControl;
    }

	void Start ()
    {
        handle = GetComponentInChildren<BowHandle>();
        ctrls = FindObjectOfType<SteamVR_ControllerManager>();
        Anim = GetComponent<BowAnim>();
        interact = GetComponent<SteamVR_InteractableObject>();
        interact.OnGrab += OnGrab;
	}

    void OnGrab(GameObject obj)
    {
        if(obj == ctrls.left)
        {
            LeftHand = true;
            HoldControl = ctrls.left.GetComponent<SteamVR_ControllerEvents>();
            StringControl = ctrls.right.GetComponent<SteamVR_ControllerEvents>();
        }
        else
        {
            LeftHand = false;
            StringControl = ctrls.left.GetComponent<SteamVR_ControllerEvents>();
            HoldControl = ctrls.right.GetComponent<SteamVR_ControllerEvents>();
        }
        StartCoroutine("GetBaseRot");
    }

    IEnumerator GetBaseRot()
    {
        yield return new WaitForEndOfFrame();
        baseRot = transform.localRotation;
    }

    public bool isHeld()
    {
        return interact.IsGrabbed();
    }

    Quaternion releaseRot;
    Quaternion baseRot;
    bool fired;
    float offst;
	// Update is called once per frame
	void Update ()
    {
	    if(currentArrow != null && isHeld())
        {
            AimArrow();
            AimBow();
            PullString();
            if(!StringControl.triggerDown)
            {
                fired = true;
                releaseRot = transform.localRotation;
                Release();
            }
        }
        else if(isHeld())
        {
            if (fired)
            {
                fired = false;
                offst = Time.time;
            }
            transform.localRotation = Quaternion.Lerp(releaseRot, baseRot, (Time.time-offst)*8);
        }

        if(!isHeld())
        {
            if(currentArrow != null)
            {
                currentArrow.transform.SetParent(null);
                Destroy(currentArrow, 5);
            }
        }
	}


    float currentPull;
    void Release()
    {
        Anim.SetAnim(0);
        currentArrow.transform.SetParent(null);
        Collider[] arrowCols = currentArrow.GetComponentsInChildren<Collider>();
        Collider[] BowCols = GetComponentsInChildren<Collider>();
        foreach(var c in arrowCols)
        {
            c.enabled = true;
            foreach(var C in BowCols)
            {
                Physics.IgnoreCollision(c, C);
            }
        }
        currentArrow.GetComponent<Rigidbody>().isKinematic = false;
        currentArrow.GetComponent<Rigidbody>().velocity = currentPull * PowerMult * currentArrow.transform.TransformDirection(Vector3.forward);
        currentArrow = null;
        currentPull = 0;
    }

    public bool hasArrow()
    {
        return currentArrow != null;
    }

    void AimArrow()
    {
        Transform aimTarg = handle.RHAim;
        if (LeftHand)
            aimTarg = handle.LHAim;
        currentArrow.transform.localPosition = Vector3.zero;
        currentArrow.transform.LookAt(aimTarg.position);
    }

    void AimBow()
    {
        transform.rotation = Quaternion.LookRotation(HoldControl.transform.position-StringControl.transform.position, HoldControl.transform.TransformDirection(Vector3.forward));
    }

    void PullString()
    {
        currentPull = Mathf.Clamp((Vector3.Distance(HoldControl.transform.position, StringControl.transform.position)-PullOffset)*AnimPullMult, 0, 2.6f);
        Anim.SetAnim(currentPull);
    }

    public void SetArrow(GameObject a)
    {
        currentArrow = a;
    }

}
