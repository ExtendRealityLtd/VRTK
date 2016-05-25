﻿//====================================================================================
//
// Purpose: Provide basic teleportation of VR CameraRig
//
// This script must be attached to the [CameraRig] Prefab
//
// A GameObject must have the SteamVR_WorldPointer attached to it to listen for the
// updated world position to teleport to.
//
//====================================================================================

using UnityEngine;
using System.Collections;

public class SteamVR_HeightAdjustTeleport : SteamVR_BasicTeleport {
    public bool playSpaceFalling = true;
    private float currentRayDownY = 0f;

    GameObject currentFloor = null;

    protected override void Start()
    {
        base.Start();
        adjustYForTerrain = true;
    }

    protected override void DoTeleport(object sender, WorldPointerEventArgs e)
    {
        base.DoTeleport(sender, e);
        if (e.enableTeleport)
        {
            DropToNearestFloor(false);
        }
    }

    protected override Vector3 GetNewPosition(Vector3 tipPosition, Transform target)
    {
        Vector3 basePosition = base.GetNewPosition(tipPosition, target);
        basePosition.y = GetTeleportY(target, tipPosition);
        return basePosition;
    }

    private float GetTeleportY(Transform target, Vector3 tipPosition)
    {
        float newY = this.transform.position.y;
        //Check to see if the tip is on top of an object
        if (target && (tipPosition.y + 0.5f) > (target.position.y + (target.localScale.y / 2)))
        {
            newY = tipPosition.y;
        }

        return newY;
    }

    private bool CurrentFloorChanged(RaycastHit collidedObj)
    {
        return (currentFloor != collidedObj.transform.gameObject);
    }

    private bool MeshYChanged(RaycastHit collidedObj, float floorY)
    {
        return (collidedObj.transform.GetComponent<MeshCollider>() && floorY != currentRayDownY);
    }

    private bool FloorIsGrabbedObject(RaycastHit collidedObj)
    {
        return (collidedObj.transform.GetComponent<SteamVR_InteractableObject>() && collidedObj.transform.GetComponent<SteamVR_InteractableObject>().IsGrabbed());
    }

    private void DropToNearestFloor(bool withBlink)
    {
        //send a ray down to find the closest object to stand on
        Ray ray = new Ray(eyeCamera.transform.position, -transform.up);
        RaycastHit rayCollidedWith;
        bool rayHit = Physics.Raycast(ray, out rayCollidedWith);
        float floorY = eyeCamera.transform.position.y - rayCollidedWith.distance;

        if (rayHit && ! FloorIsGrabbedObject(rayCollidedWith) && ( MeshYChanged(rayCollidedWith, floorY) || CurrentFloorChanged(rayCollidedWith) ) )
        {
            currentFloor = rayCollidedWith.transform.gameObject;
            currentRayDownY = floorY;

            if (withBlink && !rayCollidedWith.transform.GetComponent<MeshCollider>())
            {
                Blink(blinkTransitionSpeed);
            }

            Vector3 newPosition = new Vector3(this.transform.position.x, floorY, this.transform.position.z);
            SetNewPosition(newPosition, currentFloor.transform);
        }
    }

    private void Update()
    {
        if (playSpaceFalling)
        {
            DropToNearestFloor(true);
        }
    }
}
