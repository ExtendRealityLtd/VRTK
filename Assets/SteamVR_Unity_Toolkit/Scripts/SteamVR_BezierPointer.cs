using UnityEngine;
using System.Collections;

public class SteamVR_BezierPointer : SteamVR_WorldPointer
{
    public enum AxisType
    {
        XAxis,
        ZAxis
    }

    public Color validTargetColor;
    public Color invalidTargetColor;
    public float pointerLength = 10f;
    public int pointerDensity = 10;
    public bool showPointerCursor = true;
    public float pointerCursorRaduis = 0.5f;
    public AxisType pointerFacingAxis = AxisType.ZAxis;

    private Transform projectedBeamContainer;
    private Transform projectedBeamForward;
    private Transform projectedBeamJoint;
    private Transform projectedBeamDown;

    private GameObject pointerCursor;
    private Material pointerColor;

    private float pointerContactDistance = 0f;
    private Transform pointerContactTarget = null;

    private uint controllerIndex;
    private CurveGenerator curvedBeam;

    // Use this for initialization
    void Start()
    {
        if (GetComponent<SteamVR_ControllerEvents>() == null)
        {
            Debug.LogError("SteamVR_BezierPointer is required to be attached to a SteamVR Controller that has the SteamVR_ControllerEvents script attached to it");
            return;
        }

        //Setup controller event listeners
        GetComponent<SteamVR_ControllerEvents>().AliasPointerOn += new ControllerClickedEventHandler(EnablePointerBeam);
        GetComponent<SteamVR_ControllerEvents>().AliasPointerOff += new ControllerClickedEventHandler(DisablePointerBeam);

        InitProjectedBeams();
        InitPointer();
        TogglePointer(false);
    }

    void InitProjectedBeams()
    {
        projectedBeamContainer = new GameObject("projectedBeamContainer").transform;
        projectedBeamContainer.transform.parent = this.transform;
        projectedBeamContainer.transform.localPosition = Vector3.zero;

        projectedBeamForward = new GameObject("projectedBeamForward").transform;
        projectedBeamForward.transform.parent = projectedBeamContainer.transform;

        projectedBeamJoint = new GameObject("projectedBeamJoint").transform;
        projectedBeamJoint.transform.parent = projectedBeamContainer.transform;
        projectedBeamJoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        projectedBeamDown = new GameObject("projectedBeamDown").transform;
    }

    void InitPointer()
    {
        pointerColor = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        pointerColor.SetColor("_Color", validTargetColor);

        pointerCursor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pointerCursor.GetComponent<MeshRenderer>().material = pointerColor;
        pointerCursor.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        pointerCursor.GetComponent<MeshRenderer>().receiveShadows = false;
        pointerCursor.transform.localScale = new Vector3(pointerCursorRaduis, 0.02f, pointerCursorRaduis);

        Destroy(pointerCursor.GetComponent<CapsuleCollider>());
        pointerCursor.layer = 2;

        GameObject global = new GameObject("CurvedBeamContainer");
        curvedBeam = global.gameObject.AddComponent<CurveGenerator>();
        curvedBeam.transform.parent = null;
        curvedBeam.Create(pointerDensity, pointerCursorRaduis);
    }

    float GetForwardBeamLength()
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

    void ProjectForwardBeam()
    {
        float setThicknes = 0.01f;
        float setLength = GetForwardBeamLength();
        //if the additional decimal isn't added then the beam position glitches
        float beamPosition = setLength / (2 + 0.00001f);

        if (pointerFacingAxis == AxisType.XAxis)
        {
            projectedBeamForward.transform.localScale = new Vector3(setLength, setThicknes, setThicknes);
            projectedBeamForward.transform.localPosition = new Vector3(beamPosition, 0f, 0f);
            projectedBeamJoint.transform.localPosition = new Vector3(setLength - (projectedBeamJoint.transform.localScale.x / 2), 0f, 0f);
        }
        else
        {
            projectedBeamForward.transform.localScale = new Vector3(setThicknes, setThicknes, setLength);
            projectedBeamForward.transform.localPosition = new Vector3(0f, 0f, beamPosition);
            projectedBeamJoint.transform.localPosition = new Vector3(0f, 0f, setLength - (projectedBeamJoint.transform.localScale.z / 2));
        }        
    }

    void ProjectDownBeam()
    {
        projectedBeamDown.transform.position = new Vector3(projectedBeamJoint.transform.position.x, projectedBeamJoint.transform.position.y, projectedBeamJoint.transform.position.z);

        Ray projectedBeamDownRaycast = new Ray(projectedBeamDown.transform.position, Vector3.down);
        RaycastHit collidedWith;
        bool downRayHit = Physics.Raycast(projectedBeamDownRaycast, out collidedWith);

        if (!downRayHit || (pointerContactTarget && pointerContactTarget != collidedWith.transform))
        {
            if (pointerContactTarget != null)
            {
                OnWorldPointerOut(SetPointerEvent(controllerIndex, pointerContactDistance, pointerContactTarget, projectedBeamDown.transform.position));
            }
            pointerContactTarget = null;
        }

        if (downRayHit)
        {
            projectedBeamDown.transform.position = new Vector3(projectedBeamJoint.transform.position.x, projectedBeamJoint.transform.position.y - collidedWith.distance, projectedBeamJoint.transform.position.z);
            projectedBeamDown.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            pointerContactTarget = collidedWith.transform;
            OnWorldPointerIn(SetPointerEvent(controllerIndex, pointerContactDistance, pointerContactTarget, projectedBeamDown.transform.position));
        }
    }

    void SetPointerCursor()
    {
        if (pointerContactTarget != null)
        {
            pointerCursor.gameObject.SetActive(showPointerCursor);
            pointerCursor.transform.position = projectedBeamDown.transform.position;
            pointerColor.SetColor("_Color", validTargetColor);
            pointerCursor.GetComponent<MeshRenderer>().material = pointerColor;
        } else
        {
            pointerColor.SetColor("_Color", invalidTargetColor);
            pointerCursor.GetComponent<MeshRenderer>().material = pointerColor;
            pointerCursor.gameObject.SetActive(false);
        }
    }

    void TogglePointer(bool state)
    {
        projectedBeamForward.gameObject.SetActive(state);
        projectedBeamJoint.gameObject.SetActive(state);
        projectedBeamDown.gameObject.SetActive(state);        
    }

    void EnablePointerBeam(object sender, ControllerClickedEventArgs e)
    {
        controllerIndex = e.controllerIndex;
        TogglePointer(true);
    }

    void DisablePointerBeam(object sender, ControllerClickedEventArgs e)
    {
        controllerIndex = e.controllerIndex;
        if (pointerContactTarget != null) {
            Vector3 teleportLocation = new Vector3(projectedBeamDown.transform.position.x, 
                                                   pointerContactTarget.transform.position.y + (pointerContactTarget.transform.localScale.y / 2) + 0.05f,
                                                   projectedBeamDown.transform.position.z);
            OnWorldPointerDestinationSet(SetPointerEvent(controllerIndex, pointerContactDistance, pointerContactTarget, teleportLocation));
        }
        TogglePointer(false);
        curvedBeam.TogglePoints(false);
        pointerCursor.gameObject.SetActive(false);
    }

    void DisplayCurvedBeam()
    {
        Vector3[] beamPoints = new Vector3[]
        {
            this.transform.position,
            projectedBeamJoint.transform.position + new Vector3(0f, 1f, 0f),
            projectedBeamDown.transform.position,
            projectedBeamDown.transform.position,
        };
        curvedBeam.SetPoints(beamPoints, pointerColor);
        curvedBeam.TogglePoints(true);
        pointerCursor.gameObject.SetActive((showPointerCursor ? true : false));
    }

    void Update()
    {
        if (projectedBeamForward.gameObject.activeSelf)
        {            
            ProjectForwardBeam();
            ProjectDownBeam();
            DisplayCurvedBeam();
            SetPointerCursor();
        }
    }
}
