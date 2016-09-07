﻿//====================================================================================
//
// Purpose: Provide abstraction into setting a destination position in the scene
// As this is an abstract class, it should never be used on it's own.
//
// Events Emitted:
//
// DestinationMarkerEnter - is emitted when an object is collided with
// DestinationMarkerExit - is emitted when the object is no longer collided
// DestinationMarkerSet - is emmited when the destination is set
//
// Event Payload:
//
// distance - The distance between the origin and the collided destination
// target - The Transform of the destination object
// destiationPosition - The world position of the destination marker
// enableTeleport - Determine if the DestinationSet event should allow teleporting
// controllerIndex - The optional index of the controller the pointer is attached to
//
//====================================================================================
namespace VRTK
{
    using UnityEngine;

    public struct DestinationMarkerEventArgs
    {
        public float distance;
        public Transform target;
        public Vector3 destinationPosition;
        public bool enableTeleport;
        public uint controllerIndex;
    }

    public delegate void DestinationMarkerEventHandler(object sender, DestinationMarkerEventArgs e);

    public abstract class VRTK_DestinationMarker : MonoBehaviour
    {
        public bool enableTeleport = true;

        public event DestinationMarkerEventHandler DestinationMarkerEnter;
        public event DestinationMarkerEventHandler DestinationMarkerExit;
        public event DestinationMarkerEventHandler DestinationMarkerSet;

        protected string invalidTargetWithTagOrClass;
        protected float navMeshCheckDistance;
        protected bool headsetPositionCompensation;

        public virtual void OnDestinationMarkerEnter(DestinationMarkerEventArgs e)
        {
            if (DestinationMarkerEnter != null)
            {
                DestinationMarkerEnter(this, e);
            }
        }

        public virtual void OnDestinationMarkerExit(DestinationMarkerEventArgs e)
        {
            if (DestinationMarkerExit != null)
            {
                DestinationMarkerExit(this, e);
            }
        }

        public virtual void OnDestinationMarkerSet(DestinationMarkerEventArgs e)
        {
            if (DestinationMarkerSet != null)
            {
                DestinationMarkerSet(this, e);
            }
        }

        public virtual void SetInvalidTarget(string name)
        {
            invalidTargetWithTagOrClass = name;
        }

        public virtual void SetNavMeshCheckDistance(float distance)
        {
            navMeshCheckDistance = distance;
        }

        public virtual void SetHeadsetPositionCompensation(bool state)
        {
            headsetPositionCompensation = state;
        }

        protected virtual void OnEnable()
        {
            VRTK_ObjectCache.registeredDestinationMarkers.Add(this);
        }

        protected virtual void OnDisable()
        {
            VRTK_ObjectCache.registeredDestinationMarkers.Remove(this);
        }

        protected DestinationMarkerEventArgs SetDestinationMarkerEvent(float distance, Transform target, Vector3 position, uint controllerIndex)
        {
            DestinationMarkerEventArgs e;
            e.controllerIndex = controllerIndex;
            e.distance = distance;
            e.target = target;
            e.destinationPosition = position;
            e.enableTeleport = enableTeleport;
            return e;
        }
    }
}