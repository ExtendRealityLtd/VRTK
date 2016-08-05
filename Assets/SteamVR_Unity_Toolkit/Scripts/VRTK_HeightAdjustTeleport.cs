﻿//====================================================================================
//
// Purpose: Provide basic teleportation of VR CameraRig
//
// This script must be attached to the [CameraRig] Prefab
//
// A GameObject must have the VRTK_WorldPointer attached to it to listen for the
// updated world position to teleport to.
//
//====================================================================================
namespace VRTK
{
    using UnityEngine;

    public class VRTK_HeightAdjustTeleport : VRTK_BasicTeleport
    {
        public bool playSpaceFalling = true;

        private float currentRayDownY = 0f;
        private GameObject currentFloor = null;

        protected override void Start()
        {
            base.Start();
            adjustYForTerrain = true;
        }

        protected override void DoTeleport(object sender, DestinationMarkerEventArgs e)
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
            var newY = transform.position.y;
            var heightOffset = 0.1f;
            //Check to see if the tip is on top of an object
            var rayStartPositionOffset = Vector3.up * heightOffset;
            var ray = new Ray(tipPosition + rayStartPositionOffset, -transform.up);
            RaycastHit rayCollidedWith;
            if (target && Physics.Raycast(ray, out rayCollidedWith))
            {
                newY = (tipPosition.y - rayCollidedWith.distance) + heightOffset;
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
            return (collidedObj.transform.GetComponent<VRTK_InteractableObject>() && collidedObj.transform.GetComponent<VRTK_InteractableObject>().IsGrabbed());
        }

        private void DropToNearestFloor(bool withBlink)
        {
            if (enableTeleport && eyeCamera.transform.position.y > transform.position.y)
            {
                //send a ray down to find the closest object to stand on
                Ray ray = new Ray(eyeCamera.transform.position, -transform.up);
                RaycastHit rayCollidedWith;
                bool rayHit = Physics.Raycast(ray, out rayCollidedWith);
                float floorY = eyeCamera.transform.position.y - rayCollidedWith.distance;

                if (rayHit && ValidLocation(rayCollidedWith.transform) && !FloorIsGrabbedObject(rayCollidedWith) && (MeshYChanged(rayCollidedWith, floorY) || CurrentFloorChanged(rayCollidedWith)))
                {
                    currentFloor = rayCollidedWith.transform.gameObject;
                    currentRayDownY = floorY;

                    if (withBlink && !rayCollidedWith.transform.GetComponent<MeshCollider>())
                    {
                        Blink(blinkTransitionSpeed);
                    }

                    Vector3 newPosition = new Vector3(transform.position.x, floorY, transform.position.z);
                    var teleportArgs = new DestinationMarkerEventArgs
                    {
                        destinationPosition = newPosition,
                        distance = rayCollidedWith.distance,
                        enableTeleport = true,
                        target = currentFloor.transform
                    };
                    OnTeleporting(gameObject, teleportArgs);
                    SetNewPosition(newPosition, currentFloor.transform);
                    OnTeleported(gameObject, teleportArgs);
                }
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
}