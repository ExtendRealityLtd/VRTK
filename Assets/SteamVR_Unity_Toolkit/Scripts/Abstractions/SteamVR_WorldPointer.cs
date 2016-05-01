//====================================================================================
//
// Purpose: Provide abstraction into projecting a raycast into the game world.
// As this is an abstract class, it should never be used on it's own.
//
// Events Emitted:
//
// WorldPointerIn - is emitted when the pointer collides with an object
// WorldPointerOut - is emitted when the pointer stops colliding with an object
// WorldPointerDestinationSet - is emmited when the pointer is deactivated
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

public struct WorldPointerEventArgs
{
    public uint controllerIndex;
    public float distance;
    public Transform target;
    public Vector3 destinationPosition;
}

public delegate void WorldPointerEventHandler(object sender, WorldPointerEventArgs e);

public class SteamVR_PlayAreaCollider : MonoBehaviour
{
    private GameObject parent;

    public void SetParent(GameObject setParent)
    {
        parent = setParent;
    }

    void OnTriggerStay(Collider collider)
    {
        if (parent.GetComponent<SteamVR_WorldPointer>().IsActive() && !collider.name.Contains("PlayerObject_"))
        {
            parent.GetComponent<SteamVR_WorldPointer>().setPlayAreaCursorCollision(true);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (! collider.name.Contains("PlayerObject_")) {
            parent.GetComponent<SteamVR_WorldPointer>().setPlayAreaCursorCollision(false);
        }
    }
}

public abstract class SteamVR_WorldPointer : MonoBehaviour {
    public enum AxisType
    {
        XAxis,
        ZAxis
    }

    public Color pointerHitColor = new Color(0f, 0.5f, 0f, 1f);
    public Color pointerMissColor = new Color(0.8f, 0f, 0f, 1f);
    public bool showPlayAreaCursor = false;
    public bool handlePlayAreaCursorCollisions = false;
    public AxisType pointerFacingAxis = AxisType.ZAxis;

    protected Vector3 destinationPosition;
    protected float pointerContactDistance = 0f;
    protected Transform pointerContactTarget = null;
    protected uint controllerIndex;

    protected Material pointerMaterial;
    protected bool playAreaCursorCollided = false;

    private SteamVR_PlayArea playArea;
    private GameObject playAreaCursor;
    private GameObject[] playAreaCursorBoundaries;
    private bool isActive;

    public event WorldPointerEventHandler WorldPointerIn;
    public event WorldPointerEventHandler WorldPointerOut;
    public event WorldPointerEventHandler WorldPointerDestinationSet;

    public virtual void OnWorldPointerIn(WorldPointerEventArgs e)
    {
        if (WorldPointerIn != null)
            WorldPointerIn(this, e);
    }

    public virtual void OnWorldPointerOut(WorldPointerEventArgs e)
    {
        if (WorldPointerOut != null)
            WorldPointerOut(this, e);
    }

    public virtual void OnWorldPointerDestinationSet(WorldPointerEventArgs e)
    {
        if (WorldPointerDestinationSet != null)
            WorldPointerDestinationSet(this, e);
    }

    public virtual void setPlayAreaCursorCollision(bool state)
    {
        if (handlePlayAreaCursorCollisions)
        {
            playAreaCursorCollided = state;
        }
    }

    public virtual bool IsActive()
    {
        return isActive;
    }

    protected WorldPointerEventArgs SetPointerEvent(uint controllerIndex, float distance, Transform target, Vector3 position)
    {
        WorldPointerEventArgs e;
        e.controllerIndex = controllerIndex;
        e.distance = distance;
        e.target = target;
        e.destinationPosition = position;
        return e;
    }

    protected virtual void Start()
    {
        if (GetComponent<SteamVR_ControllerEvents>() == null)
        {
            Debug.LogError("SteamVR_WorldPointer is required to be attached to a SteamVR Controller that has the SteamVR_ControllerEvents script attached to it");
            return;
        }

        this.name = "PlayerObject_" + this.name;

        //Setup controller event listeners
        GetComponent<SteamVR_ControllerEvents>().AliasPointerOn += new ControllerClickedEventHandler(EnablePointerBeam);
        GetComponent<SteamVR_ControllerEvents>().AliasPointerOff += new ControllerClickedEventHandler(DisablePointerBeam);

        playArea = GameObject.FindObjectOfType<SteamVR_PlayArea>();
        playAreaCursorBoundaries = new GameObject[4];

        pointerMaterial = new Material(Shader.Find("Unlit/TransparentColor"));
        pointerMaterial.color = pointerMissColor;
    }

    protected virtual void InitPointer()
    {
        InitPlayAreaCursor();
    }

    protected virtual void SetPlayAreaCursorTransform(Vector3 destination)
    {
        playAreaCursor.transform.position = destination;
    }

    protected virtual void EnablePointerBeam(object sender, ControllerClickedEventArgs e)
    {
        setPlayAreaCursorCollision(false);
        controllerIndex = e.controllerIndex;
        TogglePointer(true);
        isActive = true;
    }

    protected virtual void DisablePointerBeam(object sender, ControllerClickedEventArgs e)
    {
        controllerIndex = e.controllerIndex;
        TogglePointer(false);
        isActive = false;
    }

    protected virtual void PointerIn()
    {
        OnWorldPointerIn(SetPointerEvent(controllerIndex, pointerContactDistance, pointerContactTarget, destinationPosition));
    }

    protected virtual void PointerOut()
    {
        OnWorldPointerOut(SetPointerEvent(controllerIndex, pointerContactDistance, pointerContactTarget, destinationPosition));
    }

    protected virtual void PointerSet()
    {
        if (!playAreaCursorCollided)
        {
            OnWorldPointerDestinationSet(SetPointerEvent(controllerIndex, pointerContactDistance, pointerContactTarget, destinationPosition));
        }
    }

    protected virtual void TogglePointer(bool state)
    {
        bool playAreaState = (showPlayAreaCursor ? state: false);
        playAreaCursor.gameObject.SetActive(playAreaState);
    }

    protected virtual void SetPointerMaterial()
    {
        foreach(GameObject playAreaCursorBoundary in playAreaCursorBoundaries)
        {
            playAreaCursorBoundary.GetComponent<MeshRenderer>().material = pointerMaterial;
        }
    }

    protected void UpdatePointerMaterial(Color color)
    {
        if (playAreaCursorCollided)
        {
            color = pointerMissColor;
        }
        pointerMaterial.color = color;
        SetPointerMaterial();
    }

    private void DrawPlayAreaCursorBoundary(int index, float left, float right, float top, float bottom, float thickness, Vector3 localPosition)
    {

        GameObject playAreaCursorBoundary = GameObject.CreatePrimitive(PrimitiveType.Cube);
        playAreaCursorBoundary.name = "PlayerObject_WorldPointer_PlayAreaCursorBoundary_" + index;

        float width = (right - left) / 1.065f;
        float length = (top - bottom) / 1.08f;
        float height = thickness;

        playAreaCursorBoundary.transform.localScale = new Vector3(width, height, length);
        Destroy(playAreaCursorBoundary.GetComponent<BoxCollider>());
        playAreaCursorBoundary.layer = 2;

        playAreaCursorBoundary.transform.parent = playAreaCursor.transform;
        playAreaCursorBoundary.transform.localPosition = localPosition;

        playAreaCursorBoundaries[index] = playAreaCursorBoundary;
    }

    private void InitPlayAreaCursor()
    {
        int btmRight = 4;
        int topLeft = 6;

        float width = playArea.vertices[btmRight].x - playArea.vertices[topLeft].x;
        float length = playArea.vertices[topLeft].z - playArea.vertices[btmRight].z;
        float height = 0.01f;

        playAreaCursor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        playAreaCursor.name = "PlayerObject_WorldPointer_PlayAreaCursor";
        playAreaCursor.transform.parent = null;
        playAreaCursor.transform.localScale = new Vector3(width, height, length);

        playAreaCursor.GetComponent<MeshRenderer>().enabled = false;

        BoxCollider playAreaCursorCollider = playAreaCursor.GetComponent<BoxCollider>();
        playAreaCursorCollider.isTrigger = true;
        playAreaCursorCollider.center = new Vector3(0f, 65f, 0f);
        playAreaCursorCollider.size = new Vector3(1f, 100f, 1f);
        playAreaCursor.AddComponent<Rigidbody>().isKinematic = true;
        SteamVR_PlayAreaCollider playAreaCursorScript = playAreaCursor.AddComponent<SteamVR_PlayAreaCollider>();
        playAreaCursorScript.SetParent(this.gameObject);
        playAreaCursor.layer = 2;

        float playAreaBoundaryX = playArea.transform.localScale.x / 2;
        float playAreaBoundaryZ = playArea.transform.localScale.z / 2;
        float heightOffset = 0f;

        DrawPlayAreaCursorBoundary(0, playArea.vertices[5].x, playArea.vertices[4].x, playArea.vertices[0].z, playArea.vertices[4].z, height, new Vector3(0f, heightOffset, playAreaBoundaryZ));
        DrawPlayAreaCursorBoundary(1, playArea.vertices[5].x, playArea.vertices[1].x, playArea.vertices[6].z, playArea.vertices[5].z, height, new Vector3(playAreaBoundaryX, heightOffset, 0f));
        DrawPlayAreaCursorBoundary(2, playArea.vertices[5].x, playArea.vertices[4].x, playArea.vertices[0].z, playArea.vertices[4].z, height, new Vector3(0f, heightOffset, -playAreaBoundaryZ));
        DrawPlayAreaCursorBoundary(3, playArea.vertices[5].x, playArea.vertices[1].x, playArea.vertices[6].z, playArea.vertices[5].z, height, new Vector3(-playAreaBoundaryX, heightOffset, 0f));
    }
}