//====================================================================================
//
// Purpose: Provide interface into projecting a raycast into the game world. It is an
// abstract class so should never be used on it's own.
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

public abstract class SteamVR_WorldPointer : MonoBehaviour {

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

    protected WorldPointerEventArgs SetPointerEvent(uint controllerIndex, float distance, Transform target, Vector3 position)
    {
        WorldPointerEventArgs e;
        e.controllerIndex = controllerIndex;
        e.distance = distance;
        e.target = target;
        e.destinationPosition = position;
        return e;
    }
}
