//====================================================================================
//
// Purpose: Provide basic laser pointer to VR Controller
//
// This script must be attached to a Controller within the [CameraRig] Prefab
//
// The SteamVR_ControllerEvents script must also be attached to the Controller
//
// Press the 'Grip' button on the controller to activate the beam
// Released the 'Grip' button on the controller to deactivate the beam
//
// Events Emitted:
//
// ControllerPointerIn - is emitted when the pointer collides with an object
// ControllerPointerOut - is emitted when the pointer stops colliding with an object
// PointerDestinationSet - is emmited when the pointer is deactivated
//
// Event Payload:
//
// controllerIndex - The index of the controller the pointer is attached to
// distance - The distance from the collided object the controller is
// target - The Transform of the object the pointer has collided with
// tipPosition - The world position of the beam tip
//
//====================================================================================

using UnityEngine;
using System.Collections;

public struct ControllerPointerEventArgs
{
    public uint controllerIndex;
    public float distance;
    public Transform target;
    public Vector3 tipPosition;
}

public delegate void ControllerPointerEventHandler(object sender, ControllerPointerEventArgs e);

public class SteamVR_SimplePointer : MonoBehaviour {
    public enum AxisType
    {
        XAxis,
        ZAxis
    }

    public Color pointerColor;
    public float pointerThickness = 0.002f;    
    public float pointerLength = 100f;
    public bool showPointerTip = true;
    public AxisType pointerFacingAxis = AxisType.ZAxis;

    private GameObject pointerHolder;
    private GameObject pointer;
    private GameObject pointerTip;

    public event ControllerPointerEventHandler ControllerPointerIn;
    public event ControllerPointerEventHandler ControllerPointerOut;
    public event ControllerPointerEventHandler PointerDestinationSet;

    private Vector3 pointerTipScale = new Vector3(0.05f, 0.05f, 0.05f);

    private float pointerContactDistance = 0f;
    private Transform pointerContactTarget = null;

    private uint controllerIndex;

    // Use this for initialization
    void Start () {
        if (GetComponent<SteamVR_ControllerEvents>() == null)
        {
            Debug.LogError("SteamVR_SimplePointer is required to be attached to a SteamVR Controller that has the SteamVR_ControllerEvents script attached to it");
            return;
        }

        //Setup controller event listeners
        GetComponent<SteamVR_ControllerEvents>().GripClicked += new ControllerClickedEventHandler(EnablePointerBeam);
        GetComponent<SteamVR_ControllerEvents>().GripUnclicked += new ControllerClickedEventHandler(DisablePointerBeam);

        InitPointer();
    }

    public virtual void OnPointerIn(ControllerPointerEventArgs e)
    {
        if (ControllerPointerIn != null)
            ControllerPointerIn(this, e);
    }

    public virtual void OnPointerOut(ControllerPointerEventArgs e)
    {
        if (ControllerPointerOut != null)
            ControllerPointerOut(this, e);
    }

    public virtual void OnPointerDestinationSet(ControllerPointerEventArgs e)
    {
        if (PointerDestinationSet != null)
            PointerDestinationSet(this, e);
    }

    void InitPointer()
    {
        Material newMaterial = new Material(Shader.Find("Unlit/Color"));
        newMaterial.SetColor("_Color", pointerColor);

        pointerHolder = new GameObject();
        pointerHolder.transform.parent = this.transform;
        pointerHolder.transform.localPosition = Vector3.zero;

        pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointer.transform.parent = pointerHolder.transform;
        pointer.GetComponent<MeshRenderer>().material = newMaterial;

        pointer.GetComponent<BoxCollider>().isTrigger = true;
        pointer.AddComponent<Rigidbody>().isKinematic = true;
        pointer.layer = 2;

        pointerTip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pointerTip.transform.parent = pointerHolder.transform;
        pointerTip.GetComponent<MeshRenderer>().material = newMaterial;
        pointerTip.transform.localScale = pointerTipScale;

        pointerTip.GetComponent<SphereCollider>().isTrigger = true;
        pointerTip.AddComponent<Rigidbody>().isKinematic = true;
        pointerTip.layer = 2;

        SetPointerTransform(pointerLength, pointerThickness);
        TogglePointer(false);
    }

    void SetPointerTransform(float setLength, float setThicknes)
    {
        //if the additional decimal isn't added then the beam position glitches
        float beamPosition = setLength / (2 + 0.00001f);

        if (pointerFacingAxis == AxisType.XAxis)
        {
            pointer.transform.localScale = new Vector3(setLength, setThicknes, setThicknes);
            pointer.transform.localPosition = new Vector3(beamPosition, 0f, 0f);
            pointerTip.transform.localPosition = new Vector3(setLength - (pointerTip.transform.localScale.x / 2), 0f, 0f);
        }
        else
        {
            pointer.transform.localScale = new Vector3(setThicknes, setThicknes, setLength);
            pointer.transform.localPosition = new Vector3(0f, 0f, beamPosition);
            pointerTip.transform.localPosition = new Vector3(0f, 0f, setLength - (pointerTip.transform.localScale.z / 2));
        }
    }

    void TogglePointer(bool state)
    {
        pointer.gameObject.SetActive(state);
        bool tipState = (showPointerTip ? state : false);
        pointerTip.gameObject.SetActive(tipState);
    }

    ControllerPointerEventArgs SetPointerEvent(float distance, Transform target)
    {
        ControllerPointerEventArgs e;
        e.controllerIndex = controllerIndex;
        e.distance = distance;
        e.target = target;
        e.tipPosition = pointerTip.transform.position;
        return e;
    }

    float GetPointerBeamLength(bool hasRayHit, RaycastHit collidedWith)
    {
        float actualLength = pointerLength;

        //reset if beam not hitting or hitting new target
        if (!hasRayHit || (pointerContactTarget && pointerContactTarget != collidedWith.transform))
        {
            if (pointerContactTarget != null)
            {
                OnPointerOut(SetPointerEvent(pointerContactDistance, pointerContactTarget));
            }

            pointerContactDistance = 0f;
            pointerContactTarget = null;
        }

        //check if beam has hit a new target
        if (hasRayHit)
        {
            pointerContactDistance = collidedWith.distance;
            pointerContactTarget = collidedWith.transform;
                        
            OnPointerIn(SetPointerEvent(pointerContactDistance, pointerContactTarget));
        }

        //adjust beam length if something is blocking it
        if (hasRayHit && pointerContactDistance < pointerLength)
        {
            actualLength = pointerContactDistance;
        }

        return actualLength; ;
    }

    void EnablePointerBeam(object sender, ControllerClickedEventArgs e)
    {
        controllerIndex = e.controllerIndex;
        TogglePointer(true);
    }

    void DisablePointerBeam(object sender, ControllerClickedEventArgs e)
    {
        controllerIndex = e.controllerIndex;
        OnPointerDestinationSet(SetPointerEvent(pointerContactDistance, pointerContactTarget));
        TogglePointer(false);
    }

    // Update is called once per frame
    void Update () {
        if (pointer.gameObject.activeSelf)
        {
            Ray pointerRaycast = new Ray(transform.position, transform.forward);
            RaycastHit pointerCollidedWith;
            bool rayHit = Physics.Raycast(pointerRaycast, out pointerCollidedWith);
            float pointerBeamLength = GetPointerBeamLength(rayHit, pointerCollidedWith);
            SetPointerTransform(pointerBeamLength, pointerThickness);
        }
    }
}
