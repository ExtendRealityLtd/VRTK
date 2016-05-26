//=====================================================================================
//
// Purpose: Provide a mechanism for determining if a game world object is interactable
//
// This script should be attached to any object that needs touch, use or grab
//
// An optional highlight color can be set to change the object's appearance if it is
// invoked.
//
//=====================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SteamVR_InteractableObject : MonoBehaviour
{
    public enum GrabSnapType
    {
        Simple_Snap,
        Rotation_Snap,
        Precision_Snap
    }

    public enum GrabAttatchType
    {
        Fixed_Joint,
        Spring_Joint,
        Track_Object
    }

    [Header("Touch Interactions", order = 1)]
    public bool highlightOnTouch = false;
    public Color touchHighlightColor = Color.clear;

    [Header("Grab Interactions", order = 2)]
    public bool isGrabbable = false;
    public bool holdButtonToGrab = true;
    public bool pauseCollisionsOnGrab = true;
    public GrabSnapType grabSnapType = GrabSnapType.Simple_Snap;
    public Vector3 snapToRotation = Vector3.zero;
    public Vector3 snapToPosition = Vector3.zero;

    [Header("Grab Mechanics", order = 3)]
    public GrabAttatchType grabAttatchMechanic = GrabAttatchType.Fixed_Joint;
    public float detatchThreshold = 500f;
    public float springJointStrength = 500f;
    public float springJointDamper = 50f;

    [Header("Use Interactions", order = 4)]
    public bool isUsable = false;
    public bool holdButtonToUse = true;
    public bool pointerActivatesUseAction = false;

    protected Rigidbody rb;
    protected GameObject grabbingObject = null;

    private bool isTouched = false;
    private bool isUsing = false;
    private int usingState = 0;
    private Color[] originalObjectColours = null;

    private Transform trackPoint;
    private bool customTrackPoint = false;

    public bool IsTouched()
    {
        return isTouched;
    }

    public bool IsGrabbed()
    {
        return (grabbingObject != null);
    }

    public bool IsUsing()
    {
        return isUsing;
    }

    public virtual void StartTouching(GameObject touchingObject)
    {
        isTouched = true;
    }

    public virtual void StopTouching(GameObject previousTouchingObject)
    {
        isTouched = false;
    }

    public virtual void Grabbed(GameObject currentGrabbingObject)
    {
        ForceReleaseGrab();
        RemoveTrackPoint();
        grabbingObject = currentGrabbingObject;
        SetTrackPoint(grabbingObject);
    }

    public virtual void Ungrabbed(GameObject previousGrabbingObject)
    {
        RemoveTrackPoint();
        grabbingObject = null;
    }

    public virtual void StartUsing(GameObject usingObject)
    {
        isUsing = true;
    }

    public virtual void StopUsing(GameObject previousUsingObject)
    {
        isUsing = false;
    }

    public virtual void ToggleHighlight(bool toggle)
    {
        ToggleHighlight(toggle, Color.clear);
    }

    public virtual void ToggleHighlight(bool toggle, Color globalHighlightColor)
    {
        if (highlightOnTouch)
        {
            if (toggle && !IsGrabbed() && !isUsing)
            {
                Color color = (touchHighlightColor != Color.clear ? touchHighlightColor : globalHighlightColor);
                if (color != Color.clear)
                {
                    var colorArray = BuildHighlightColorArray(color);
                    ChangeColor(colorArray);
                }
            }
            else
            {
                if(originalObjectColours == null)
                {
                    Debug.LogError("SteamVR_InteractableObject has not had the Start() method called, if you are inheriting this class then call base.Start() in your Start() method. [Error raised on line 78 of SteamVR_InteractableObject.cs]");
                    return;
                }
                ChangeColor(originalObjectColours);
            }
        }
    }

    public int UsingState
    {
        get { return usingState; }
        set{ usingState = value;  }
    }

    public void PauseCollisions(float pauseTime)
    {
        if (pauseCollisionsOnGrab)
        {
            if (this.GetComponent<Rigidbody>())
            {
                this.GetComponent<Rigidbody>().detectCollisions = false;
            }
            foreach (Rigidbody rb in this.GetComponentsInChildren<Rigidbody>())
            {
                rb.detectCollisions = false;
            }
            Invoke("UnpauseCollisions", pauseTime);
        }
    }

    public bool AttatchIsTrackObject()
    {
        return (grabAttatchMechanic == GrabAttatchType.Track_Object);
    }

    public void ZeroVelocity()
    {
        if (this.GetComponent<Rigidbody>())
        {
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }

    protected virtual void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        originalObjectColours = StoreOriginalColors();
    }

    protected virtual void Update()
    {
        if (grabAttatchMechanic == GrabAttatchType.Track_Object)
        {
            CheckBreakDistance();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (grabAttatchMechanic == GrabAttatchType.Track_Object)
        {
            FixedUpdateTrackedObject();
        }
    }

    protected virtual void OnJointBreak(float force)
    {
        ForceReleaseGrab();
    }

    private void ForceReleaseGrab()
    {
        if (grabbingObject)
        {
            grabbingObject.GetComponent<SteamVR_InteractGrab>().ForceRelease();
        }
    }

    private void UnpauseCollisions()
    {
        if (this.GetComponent<Rigidbody>())
        {
            this.GetComponent<Rigidbody>().detectCollisions = true;
        }
        foreach (Rigidbody rb in this.GetComponentsInChildren<Rigidbody>())
        {
            rb.detectCollisions = true;
        }
    }

    private Renderer[] GetRendererArray()
    {
        return (GetComponents<Renderer>().Length > 0 ? GetComponents<Renderer>() : GetComponentsInChildren<Renderer>());
    }

    private Color[] StoreOriginalColors()
    {
        Renderer[] rendererArray = GetRendererArray();
        int length = rendererArray.Length;
        Color[] colors = new Color[length];

        for (int i = 0; i < length; i++)
        {
            var renderer = rendererArray[i];
            if (renderer.material.HasProperty("_Color"))
            {
                colors[i] = renderer.material.color;
            }
        }
        return colors;
    }

    private Color[] BuildHighlightColorArray(Color color)
    {
        Renderer[] rendererArray = GetRendererArray();
        int length = rendererArray.Length;
        Color[] colors = new Color[length];

        for (int i = 0; i < length; i++)
        {
            colors[i] = color;
        }
        return colors;
    }

    private void ChangeColor(Color[] colors)
    {
        Renderer[] rendererArray = GetRendererArray();
        int i = 0;
        foreach (Renderer renderer in rendererArray)
        {
            if (renderer.material.HasProperty("_Color"))
            {
                renderer.material.color = colors[i];
            }
            i++;
        }
    }

    private void CheckBreakDistance()
    {
        if(trackPoint)
        {
            float distance = Vector3.Distance(trackPoint.position, this.transform.position);
            if (distance > (detatchThreshold / 1000))
            {
                ForceReleaseGrab();
            }
        }
    }

    private void SetTrackPoint(GameObject point)
    {
        Transform controllerPoint = point.transform;

        if (point.GetComponent<SteamVR_InteractGrab>() && point.GetComponent<SteamVR_InteractGrab>().controllerAttachPoint)
        {
            controllerPoint = point.GetComponent<SteamVR_InteractGrab>().controllerAttachPoint.transform;
        }

        if (grabAttatchMechanic == GrabAttatchType.Track_Object && grabSnapType == GrabSnapType.Precision_Snap)
        {
            trackPoint = new GameObject(string.Format("[{0}]TrackObject_PrecisionSnap_AttatchPoint", this.gameObject.name)).transform;
            trackPoint.parent = point.transform;
            trackPoint.position = this.transform.position;
            trackPoint.rotation = this.transform.rotation;
            customTrackPoint = true;
        } else
        {
            trackPoint = controllerPoint;
            customTrackPoint = false;
        }
    }

    private void RemoveTrackPoint()
    {
        if (customTrackPoint && trackPoint)
        {
            Destroy(trackPoint.gameObject);
        }
        else
        {
            trackPoint = null;
        }
    }

    private void FixedUpdateTrackedObject()
    {
        if (trackPoint)
        {
            float rotationMultiplier = 20f;
            float positionMultiplier = 3000f;
            float maxDistanceDelta = 10f;

            Quaternion rotationDelta = trackPoint.rotation * Quaternion.Inverse(this.transform.rotation);
            Vector3 positionDelta = (trackPoint.position - this.transform.position);

            float angle;
            Vector3 axis;
            rotationDelta.ToAngleAxis(out angle, out axis);
            angle = (angle > 180 ? angle -= 360 : angle);

            if (angle != 0)
            {
                Vector3 AngularTarget = (Time.fixedDeltaTime * angle * axis) * rotationMultiplier;
                rb.angularVelocity = Vector3.MoveTowards(rb.angularVelocity, AngularTarget, maxDistanceDelta);
            }

            Vector3 VelocityTarget = positionDelta * positionMultiplier * Time.fixedDeltaTime;
            rb.velocity = Vector3.MoveTowards(rb.velocity, VelocityTarget, maxDistanceDelta);
        }
    }
}