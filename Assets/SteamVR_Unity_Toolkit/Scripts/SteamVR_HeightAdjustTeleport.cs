//====================================================================================
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

    GameObject currentFloor = null;

    protected override void Start()
    {
        base.Start();
        adjustYForTerrain = true;
    }

    protected override void DoTeleport(object sender, WorldPointerEventArgs e)
    {
        base.DoTeleport(sender, e);
        DropToNearestFloor(false);
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
        if (target && tipPosition.y > (target.position.y + (target.localScale.y / 2)))
        {
            newY = (target.position.y + (target.localScale.y / 2));
        }

        return newY;
    }

    private void DropToNearestFloor(bool withBlink)
    {
        //send a ray down to find the closest object to stand on
        Ray ray = new Ray(eyeCamera.transform.position, -transform.up);
        RaycastHit rayCollidedWith;
        bool rayHit = Physics.Raycast(ray, out rayCollidedWith);

        if (rayHit && currentFloor != rayCollidedWith.transform.gameObject)
        {
            currentFloor = rayCollidedWith.transform.gameObject;
            float floorY = (currentFloor.transform.position.y + (currentFloor.transform.localScale.y / 2));

            if (withBlink)
            {
                Blink();
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
