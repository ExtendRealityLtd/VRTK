//====================================================================================
//
// Purpose: Provide basic laser pointer to VR Controller
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

public class SteamVR_SimplePointer : SteamVR_WorldPointer
{
    public float pointerThickness = 0.002f;    
    public float pointerLength = 100f;
    public bool showPointerTip = true;

    private GameObject pointerHolder;
    private GameObject pointer;
    private GameObject pointerTip;
    private Vector3 pointerTipScale = new Vector3(0.05f, 0.05f, 0.05f);

    // Use this for initialization
    protected override void Start () {
        base.Start();
        InitPointer();
    }

    protected override void Update()
    {
        base.Update();
        if (pointer.gameObject.activeSelf)
        {
            Ray pointerRaycast = new Ray(transform.position, transform.forward);
            RaycastHit pointerCollidedWith;
            bool rayHit = Physics.Raycast(pointerRaycast, out pointerCollidedWith);
            float pointerBeamLength = GetPointerBeamLength(rayHit, pointerCollidedWith);
            SetPointerTransform(pointerBeamLength, pointerThickness);
        }
    }

    protected override void InitPointer()
    {
        pointerHolder = new GameObject(string.Format("[{0}]PlayerObject_WorldPointer_SimplePointer_Holder", this.gameObject.name));
        pointerHolder.transform.parent = this.transform;
        pointerHolder.transform.localPosition = Vector3.zero;

        pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointer.transform.name = string.Format("[{0}]PlayerObject_WorldPointer_SimplePointer_Pointer", this.gameObject.name);
        pointer.transform.parent = pointerHolder.transform;

        pointer.GetComponent<BoxCollider>().isTrigger = true;
        pointer.AddComponent<Rigidbody>().isKinematic = true;
        pointer.layer = 2;

        pointerTip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pointerTip.transform.name = string.Format("[{0}]PlayerObject_WorldPointer_SimplePointer_PointerTip", this.gameObject.name);
        pointerTip.transform.parent = pointerHolder.transform;
        pointerTip.transform.localScale = pointerTipScale;

        pointerTip.GetComponent<SphereCollider>().isTrigger = true;
        pointerTip.AddComponent<Rigidbody>().isKinematic = true;
        pointerTip.layer = 2;

        base.InitPointer();

        SetPointerTransform(pointerLength, pointerThickness);
        TogglePointer(false);
    }

    protected override void SetPointerMaterial()
    {
        base.SetPointerMaterial();
        pointer.GetComponent<MeshRenderer>().material = pointerMaterial;
        pointerTip.GetComponent<MeshRenderer>().material = pointerMaterial;
    }

    protected override void TogglePointer(bool state)
    {
        state = (beamAlwaysOn ? true : state);

        base.TogglePointer(state);
        pointer.gameObject.SetActive(state);
        bool tipState = (showPointerTip ? state : false);
        pointerTip.gameObject.SetActive(tipState);
    }

    protected override void DisablePointerBeam(object sender, ControllerClickedEventArgs e)
    {
        base.PointerSet();
        base.DisablePointerBeam(sender, e);
    }

    private void SetPointerTransform(float setLength, float setThicknes)
    {
        //if the additional decimal isn't added then the beam position glitches
        float beamPosition = setLength / (2 + 0.00001f);

        pointer.transform.localScale = new Vector3(setThicknes, setThicknes, setLength);
        pointer.transform.localPosition = new Vector3(0f, 0f, beamPosition);
        pointerTip.transform.localPosition = new Vector3(0f, 0f, setLength - (pointerTip.transform.localScale.z / 2));
        pointerHolder.transform.localRotation = Quaternion.identity;
        base.SetPlayAreaCursorTransform(pointerTip.transform.position);
    }

    private float GetPointerBeamLength(bool hasRayHit, RaycastHit collidedWith)
    {
        float actualLength = pointerLength;

        //reset if beam not hitting or hitting new target
        if (!hasRayHit || (pointerContactTarget && pointerContactTarget != collidedWith.transform))
        {
            if (pointerContactTarget != null)
            {
                base.PointerOut();
            }

            pointerContactDistance = 0f;
            pointerContactTarget = null;
            destinationPosition = Vector3.zero;

            UpdatePointerMaterial(pointerMissColor);
        }

        //check if beam has hit a new target
        if (hasRayHit)
        {
            pointerContactDistance = collidedWith.distance;
            pointerContactTarget = collidedWith.transform;
            destinationPosition = pointerTip.transform.position;

            UpdatePointerMaterial(pointerHitColor);

            base.PointerIn();
        }

        //adjust beam length if something is blocking it
        if (hasRayHit && pointerContactDistance < pointerLength)
        {
            actualLength = pointerContactDistance;
        }

        return actualLength;
    }
}