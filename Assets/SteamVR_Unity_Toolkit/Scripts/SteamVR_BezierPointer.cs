//====================================================================================
//
// Purpose: Provide curved laser pointer at the ground to VR Controller
//
// This script must be attached to a Controller within the [CameraRig] Prefab
//
// The SteamVR_ControllerEvents script must also be attached to the Controller
//
// Press the default 'Grip' button on the controller to activate the beam
// Released the default 'Grip' button on the controller to deactivate the beam
//
// This script is an implementation of the SteamVR_WorldPointer.
//
//====================================================================================

using UnityEngine;
using System.Collections;

public class SteamVR_BezierPointer : SteamVR_WorldPointer
{
    public float pointerLength = 10f;
    public int pointerDensity = 10;
    public bool showPointerCursor = true;
    public float pointerCursorRadius = 0.5f;
    public float beamCurveOffset = 1f;
    public GameObject customPointerTracer;
    public GameObject customPointerCursor;

    private Transform projectedBeamContainer;
    private Transform projectedBeamForward;
    private Transform projectedBeamJoint;
    private Transform projectedBeamDown;

    private GameObject pointerCursor;
    private CurveGenerator curvedBeam;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        InitProjectedBeams();
        InitPointer();
        TogglePointer(false);
    }

    protected override void Update()
    {
        base.Update();
        if (projectedBeamForward.gameObject.activeSelf)
        {
            ProjectForwardBeam();
            ProjectDownBeam();
            DisplayCurvedBeam();
            SetPointerCursor();
        }
    }

    protected override void InitPointer()
    {
        pointerCursor = (customPointerCursor ? Instantiate(customPointerCursor) : CreateCursor());

        pointerCursor.name = string.Format("[{0}]PlayerObject_WorldPointer_BezierPointer_PointerCursor", this.gameObject.name);
        pointerCursor.layer = 2;
        pointerCursor.SetActive(false);

        GameObject global = new GameObject(string.Format("[{0}]PlayerObject_WorldPointer_BezierPointer_CurvedBeamContainer", this.gameObject.name));
        global.SetActive(false);
        curvedBeam = global.gameObject.AddComponent<CurveGenerator>();
        curvedBeam.transform.parent = null;
        curvedBeam.Create(pointerDensity, pointerCursorRadius, customPointerTracer);
        base.InitPointer();
    }

    private GameObject CreateCursor()
    {
        float cursorYOffset = 0.02f;
        GameObject cursor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cursor.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        cursor.GetComponent<MeshRenderer>().receiveShadows = false;
        cursor.transform.localScale = new Vector3(pointerCursorRadius, cursorYOffset, pointerCursorRadius);
        Destroy(cursor.GetComponent<CapsuleCollider>());
        return cursor;
    }

    protected override void SetPointerMaterial()
    {
        if (pointerCursor.GetComponent<MeshRenderer>())
        {
            pointerCursor.GetComponent<MeshRenderer>().material = pointerMaterial;
        }

        foreach(MeshRenderer mr in pointerCursor.GetComponentsInChildren<MeshRenderer>())
        {
            mr.material = pointerMaterial;
        }

        if (pointerCursor.GetComponent<SkinnedMeshRenderer>())
        {
            pointerCursor.GetComponent<SkinnedMeshRenderer>().material = pointerMaterial;
        }

        foreach (SkinnedMeshRenderer mr in pointerCursor.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            mr.material = pointerMaterial;
        }

        base.SetPointerMaterial();
    }

    protected override void TogglePointer(bool state)
    {
        state = (beamAlwaysOn ? true : state);

        projectedBeamForward.gameObject.SetActive(state);
        projectedBeamJoint.gameObject.SetActive(state);
        projectedBeamDown.gameObject.SetActive(state);
    }

    protected override void DisablePointerBeam(object sender, ControllerClickedEventArgs e)
    {
        base.PointerSet();
        base.DisablePointerBeam(sender, e);
        TogglePointerCursor(false);
        curvedBeam.TogglePoints(false);
    }

    private void TogglePointerCursor(bool state)
    {
        state = (beamAlwaysOn ? true : state);

        bool pointerCursorState = (showPointerCursor && state ? showPointerCursor : false);
        bool playAreaCursorState = (showPlayAreaCursor && state ? showPlayAreaCursor : false);
        pointerCursor.gameObject.SetActive(pointerCursorState);
        base.TogglePointer(playAreaCursorState);
    }

    private void InitProjectedBeams()
    {
        projectedBeamContainer = new GameObject(string.Format("[{0}]PlayerObject_WorldPointer_BezierPointer_ProjectedBeamContainer", this.gameObject.name)).transform;
        projectedBeamContainer.transform.parent = this.transform;
        projectedBeamContainer.transform.localPosition = Vector3.zero;

        projectedBeamForward = new GameObject(string.Format("[{0}]PlayerObject_WorldPointer_BezierPointer_ProjectedBeamForward", this.gameObject.name)).transform;
        projectedBeamForward.transform.parent = projectedBeamContainer.transform;

        projectedBeamJoint = new GameObject(string.Format("[{0}]PlayerObject_WorldPointer_BezierPointer_ProjectedBeamJoint", this.gameObject.name)).transform;
        projectedBeamJoint.transform.parent = projectedBeamContainer.transform;
        projectedBeamJoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        projectedBeamDown = new GameObject(string.Format("[{0}]PlayerObject_WorldPointer_BezierPointer_ProjectedBeamDown", this.gameObject.name)).transform;
    }

    private float GetForwardBeamLength()
    {
        float actualLength = pointerLength;
        Ray pointerRaycast = new Ray(transform.position, transform.forward);
        RaycastHit collidedWith;
        bool hasRayHit = Physics.Raycast(pointerRaycast, out collidedWith);

        //reset if beam not hitting or hitting new target
        if (!hasRayHit || (pointerContactTarget && pointerContactTarget != collidedWith.transform))
        {
            pointerContactDistance = 0f;
        }

        //check if beam has hit a new target
        if (hasRayHit)
        {
            pointerContactDistance = collidedWith.distance;
        }

        //adjust beam length if something is blocking it
        if (hasRayHit && pointerContactDistance < pointerLength)
        {
            actualLength = pointerContactDistance;
        }

        return actualLength;
    }

    private void ProjectForwardBeam()
    {
        float setThicknes = 0.01f;
        float setLength = GetForwardBeamLength();
        //if the additional decimal isn't added then the beam position glitches
        float beamPosition = setLength / (2 + 0.00001f);

        projectedBeamForward.transform.localScale = new Vector3(setThicknes, setThicknes, setLength);
        projectedBeamForward.transform.localPosition = new Vector3(0f, 0f, beamPosition);
        projectedBeamJoint.transform.localPosition = new Vector3(0f, 0f, setLength - (projectedBeamJoint.transform.localScale.z / 2));
        projectedBeamContainer.transform.localRotation = Quaternion.identity;
    }

    private void ProjectDownBeam()
    {
        projectedBeamDown.transform.position = new Vector3(projectedBeamJoint.transform.position.x, projectedBeamJoint.transform.position.y, projectedBeamJoint.transform.position.z);

        Ray projectedBeamDownRaycast = new Ray(projectedBeamDown.transform.position, Vector3.down);
        RaycastHit collidedWith;
        bool downRayHit = Physics.Raycast(projectedBeamDownRaycast, out collidedWith);

        if (!downRayHit || (pointerContactTarget && pointerContactTarget != collidedWith.transform))
        {
            if (pointerContactTarget != null)
            {
                base.PointerOut();
            }
            pointerContactTarget = null;
            destinationPosition = Vector3.zero;
        }

        if (downRayHit)
        {
            projectedBeamDown.transform.position = new Vector3(projectedBeamJoint.transform.position.x, projectedBeamJoint.transform.position.y - collidedWith.distance, projectedBeamJoint.transform.position.z);
            projectedBeamDown.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            pointerContactTarget = collidedWith.transform;
            destinationPosition = projectedBeamDown.transform.position;

            base.PointerIn();
        }
    }

    private void SetPointerCursor()
    {
        if (pointerContactTarget != null)
        {
            TogglePointerCursor(true);
            pointerCursor.transform.position = projectedBeamDown.transform.position;
            base.SetPlayAreaCursorTransform(pointerCursor.transform.position);
            UpdatePointerMaterial(pointerHitColor);
        } else
        {
            TogglePointerCursor(false);
            UpdatePointerMaterial(pointerMissColor);
        }
    }

    private void DisplayCurvedBeam()
    {
        Vector3[] beamPoints = new Vector3[]
        {
            this.transform.position,
            projectedBeamJoint.transform.position + new Vector3(0f, beamCurveOffset, 0f),
            projectedBeamDown.transform.position,
            projectedBeamDown.transform.position,
        };
        curvedBeam.SetPoints(beamPoints, pointerMaterial);
        curvedBeam.TogglePoints(true);
    }
}