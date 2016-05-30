﻿using UnityEngine;
using System.Collections;

public class BowAim : MonoBehaviour {
    public float powerMultiplier;
    public float pullMultiplier;
    public float pullOffset;

    private BowAnimation bowAnimation;
    private GameObject currentArrow;
    private BowHandle handle;

    private VRTK_InteractableObject interact;

    private SteamVR_ControllerManager controllers;
    private VRTK_ControllerEvents holdControl;
    private VRTK_ControllerEvents stringControl;

    private VRTK_ControllerActions stringActions;
    private VRTK_ControllerActions holdActions;

    private Quaternion releaseRotation;
    private Quaternion baseRotation;
    private bool fired;
    private float fireOffset;
    private float currentPull;
    private float previousPull;

    public VRTK_ControllerEvents GetPullHand()
    {
        return stringControl;
    }

    public bool IsHeld()
    {
        return interact.IsGrabbed();
    }

    public bool HasArrow()
    {
        return currentArrow != null;
    }

    public void SetArrow(GameObject arrow)
    {
        currentArrow = arrow;
    }

    private void Start()
    {
        bowAnimation = GetComponent<BowAnimation>();
        handle = GetComponentInChildren<BowHandle>();
        controllers = FindObjectOfType<SteamVR_ControllerManager>();
        interact = GetComponent<VRTK_InteractableObject>();
        interact.InteractableObjectGrabbed += new InteractableObjectEventHandler(DoObjectGrab);
    }

    private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
    {
        if (e.interactingObject == controllers.left)
        {
            holdControl = controllers.left.GetComponent<VRTK_ControllerEvents>();
            stringControl = controllers.right.GetComponent<VRTK_ControllerEvents>();

            holdActions = controllers.left.GetComponent<VRTK_ControllerActions>();
            stringActions= controllers.right.GetComponent<VRTK_ControllerActions>();
        }
        else
        {
            stringControl = controllers.left.GetComponent<VRTK_ControllerEvents>();
            holdControl = controllers.right.GetComponent<VRTK_ControllerEvents>();

            stringActions = controllers.left.GetComponent<VRTK_ControllerActions>();
            holdActions = controllers.right.GetComponent<VRTK_ControllerActions>();
        }
        StartCoroutine("GetBaseRotation");
    }

    private IEnumerator GetBaseRotation()
    {
        yield return new WaitForEndOfFrame();
        baseRotation = transform.localRotation;
    }

    private void Update()
    {
        if (currentArrow != null && IsHeld())
        {
            AimArrow();
            AimBow();
            PullString();
            if (!stringControl.grabPressed)
            {
                currentArrow.GetComponent<Arrow>().Fired();
                fired = true;
                releaseRotation = transform.localRotation;
                Release();
            }
        }
        else if (IsHeld())
        {
            if (fired)
            {
                fired = false;
                fireOffset = Time.time;
            }
            transform.localRotation = Quaternion.Lerp(releaseRotation, baseRotation, (Time.time - fireOffset) * 8);
        }

        if (!IsHeld())
        {
            if (currentArrow != null)
            {
                Release();
            }
        }
    }

    private void Release()
    {
        bowAnimation.SetFrame(0);
        currentArrow.transform.SetParent(null);
        Collider[] arrowCols = currentArrow.GetComponentsInChildren<Collider>();
        Collider[] BowCols = GetComponentsInChildren<Collider>();
        foreach (var c in arrowCols)
        {
            c.enabled = true;
            foreach (var C in BowCols)
            {
                Physics.IgnoreCollision(c, C);
            }
        }
        currentArrow.GetComponent<Rigidbody>().isKinematic = false;
        currentArrow.GetComponent<Rigidbody>().velocity = currentPull * powerMultiplier * currentArrow.transform.TransformDirection(Vector3.forward);
        currentArrow.GetComponent<Arrow>().inFlight = true;
        currentArrow = null;
        currentPull = 0;

        ReleaseArrow();
    }

    private void ReleaseArrow()
    {
        if (stringControl.gameObject.GetComponent<VRTK_InteractGrab>())
        {
            stringControl.gameObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
        }
    }

    private void AimArrow()
    {
        currentArrow.transform.localPosition = Vector3.zero;
        currentArrow.transform.LookAt(handle.nockSide.position);
    }

    private void AimBow()
    {
        transform.rotation = Quaternion.LookRotation(holdControl.transform.position - stringControl.transform.position, holdControl.transform.TransformDirection(Vector3.forward));
    }

    private void PullString()
    {
        currentPull = Mathf.Clamp((Vector3.Distance(holdControl.transform.position, stringControl.transform.position)-pullOffset) * pullMultiplier, 0, 3f);
        bowAnimation.SetFrame(currentPull);

        if (!currentPull.ToString("F2").Equals(previousPull.ToString("F2")))
        {
            holdActions.TriggerHapticPulse(1, 250);
            stringActions.TriggerHapticPulse(1, 150);
        }
        previousPull = currentPull;
    }
}