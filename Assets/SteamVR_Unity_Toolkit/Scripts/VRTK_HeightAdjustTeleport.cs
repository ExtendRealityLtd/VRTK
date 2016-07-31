//====================================================================================
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
        public bool useGravity = true;
        public float gravityFallHeight = 1.0f;

        private float currentRayDownY = 0f;
        private GameObject currentFloor = null;
        private bool originalPlaySpaceFalling;
        private bool isClimbing = false;

        private VRTK_PlayerPhysics playerPhysics;

        protected override void Start()
        {
            base.Start();
            adjustYForTerrain = true;

            originalPlaySpaceFalling = playSpaceFalling;

            // Listen for climb events 
            VRTK_PlayerClimb climbComponent = GetComponent<VRTK_PlayerClimb>();
            if (climbComponent)
            {
                climbComponent.PlayerClimbStarted += new PlayerClimbEventHandler(OnClimbStarted);
                climbComponent.PlayerClimbEnded += new PlayerClimbEventHandler(OnClimbEnded);
            }

            // Required Component: VRTK_PlayerPhysics
            playerPhysics = GetComponent<VRTK_PlayerPhysics>();
            if (!playerPhysics && useGravity)
            {
                playerPhysics = gameObject.AddComponent<VRTK_PlayerPhysics>();
            }
        }

        protected override void DoTeleport(object sender, DestinationMarkerEventArgs e)
        {
            base.DoTeleport(sender, e);
            if (e.enableTeleport)
            {
                DropToNearestFloor(false, false);
            }
        }

        protected void OnClimbStarted(object sender, PlayerClimbEventArgs e)
        {
            isClimbing = true;
            playSpaceFalling = false;
        }

        protected void OnClimbEnded(object sender, PlayerClimbEventArgs e)
        {
            isClimbing = false;
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

        private void DropToNearestFloor(bool withBlink, bool useGravityFall)
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

                    float fallDistance = transform.position.y - floorY;
                    bool shouldDoGravityFall = useGravityFall && (playerPhysics.IsFalling() || fallDistance > gravityFallHeight);

                    if (withBlink && !shouldDoGravityFall && !rayCollidedWith.transform.GetComponent<MeshCollider>())
                    {
                        Blink(blinkTransitionSpeed);
                    }

                    if (shouldDoGravityFall)
                    {
                        playerPhysics.StartPhysicsFall(Vector3.zero);
                    }
                    else // teleport fall
                    {
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
        }

        private bool isExternalSystemManipulatingPlaySpace()
        {
            return playerPhysics.IsFalling() || isClimbing;
        }

        private void Update()
        {
            if (useGravity)
            {
                // if we aren't climbing or falling we can go back to height adjusted falling
                if (!isExternalSystemManipulatingPlaySpace())
                {
                    playSpaceFalling = originalPlaySpaceFalling;
                }
            }

            if (playSpaceFalling)
            {
                DropToNearestFloor(true, useGravity);
            }
        }
    }
}