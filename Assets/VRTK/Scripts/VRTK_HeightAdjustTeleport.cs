// Height Adjust Teleport|Scripts|0080
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The height adjust teleporter extends the basic teleporter and allows for the y position of the `[CameraRig]` to be altered based on whether the teleport location is on top of another object.
    /// </summary>
    /// <remarks>
    /// Like the basic teleporter the Height Adjust Teleport script is attached to the `[CameraRig]` prefab.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/007_CameraRig_HeightAdjustTeleport` has a collection of varying height objects that the user can either walk up and down or use the laser pointer to climb on top of them.
    ///
    /// `VRTK/Examples/010_CameraRig_TerrainTeleporting` shows how the teleportation of a user can also traverse terrain colliders.
    ///
    /// `VRTK/Examples/020_CameraRig_MeshTeleporting` shows how the teleportation of a user can also traverse mesh colliders.
    /// </example>
    public class VRTK_HeightAdjustTeleport : VRTK_BasicTeleport
    {
        /// <summary>
        /// Options for testing if a play space fall is valid
        /// </summary>
        /// <param name="No_Restriction">Always play space fall when the headset is no longer over the current standing object.</param>
        /// <param name="Left_Controller">Don't play space fall if the Left Controller is still over the current standing object even if the headset isn't.</param>
        /// <param name="Right_Controller">Don't play space fall if the Right Controller is still over the current standing object even if the headset isn't.</param>
        /// <param name="Either_Controller">Don't play space fall if Either Controller is still over the current standing object even if the headset isn't.</param>
        /// <param name="Both_Controllers">Don't play space fall only if Both Controllers are still over the current standing object even if the headset isn't.</param>
        public enum FallingRestrictors
        {
            No_Restriction,
            Left_Controller,
            Right_Controller,
            Either_Controller,
            Both_Controllers,
        }

        [Tooltip("Checks if the user steps off an object into a part of their play area that is not on the object then they are automatically teleported down to the nearest floor. The `Play Space Falling` option also works in the opposite way that if the user's headset is above an object then the user is teleported automatically on top of that object, which is useful for simulating climbing stairs without needing to use the pointer beam location. If this option is turned off then the user can hover in mid-air at the same y position of the object they are standing on.")]
        public bool playSpaceFalling = true;
        [Tooltip("An additional check to see if the play space fall should take place. If the selected restrictor is still over the current floor then the play space fall will not occur. Works well for being able to lean over ledges and look down. Only works for falling down not teleporting up.")]
        public FallingRestrictors playSpaceFallRestriction = FallingRestrictors.No_Restriction;
        [Tooltip("Allows for gravity based falling when the distance is greater than `Gravity Fall Height`.")]
        public bool useGravity = true;
        [Tooltip("Fall distance needed before gravity based falling can be triggered.")]
        public float gravityFallHeight = 1.0f;
        [Tooltip("The `y` distance between the floor and the headset that must change before the fade transition is initiated. If the new user location is at a higher distance than the threshold then the headset blink transition will activate on teleport. If the new user location is within the threshold then no blink transition will happen, which is useful for walking up slopes, meshes and terrains where constant blinking would be annoying.")]
        public float blinkYThreshold = 0.1f;
        [Tooltip("The amount the `y` position needs to change by between the current floor `y` position and the previous floor `y` position before a change in floor height is considered to have occurred. A higher value here will mean that a `Drop To Floor` teleport event will be less likely to happen if the `y` of the floor beneath the user hasn't changed as much as the given threshold.")]
        public float floorHeightTolerance = 0.001f;

        private float currentRayDownY = 0f;
        private GameObject currentFloor = null;
        private bool originalPlaySpaceFalling;
        private bool isClimbing = false;
        private float previousFloorY;
        private bool initialFloorDrop = false;
        private VRTK_PlayerPresence playerPresence;

        protected override void Awake()
        {
            base.Awake();

            // Required Component: VRTK_PlayerPresence
            playerPresence = GetComponent<VRTK_PlayerPresence>();
            if (useGravity)
            {
                if (!playerPresence)
                {
                    playerPresence = gameObject.AddComponent<VRTK_PlayerPresence>();
                }
                playerPresence.SetFallingPhysicsOnlyParams(true);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            initialFloorDrop = false;
            adjustYForTerrain = true;
            originalPlaySpaceFalling = playSpaceFalling;
            InitClimbEvents(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            InitClimbEvents(false);
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
            previousFloorY = float.MaxValue; // insure a height adjustment can trigger on climb release
        }

        protected override Vector3 GetNewPosition(Vector3 tipPosition, Transform target)
        {
            Vector3 basePosition = base.GetNewPosition(tipPosition, target);
            basePosition.y = GetTeleportY(target, tipPosition);
            return basePosition;
        }

        private void InitClimbEvents(bool state)
        {
            // Listen for climb events 
            var climbComponent = GetComponent<VRTK_PlayerClimb>();
            if (climbComponent)
            {
                if (state)
                {
                    climbComponent.PlayerClimbStarted += new PlayerClimbEventHandler(OnClimbStarted);
                    climbComponent.PlayerClimbEnded += new PlayerClimbEventHandler(OnClimbEnded);
                }
                else
                {
                    climbComponent.PlayerClimbStarted -= new PlayerClimbEventHandler(OnClimbStarted);
                    climbComponent.PlayerClimbEnded -= new PlayerClimbEventHandler(OnClimbEnded);
                }
            }
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

        private bool FloorIsGrabbedObject(RaycastHit collidedObj)
        {
            return (collidedObj.transform.GetComponent<VRTK_InteractableObject>() && collidedObj.transform.GetComponent<VRTK_InteractableObject>().IsGrabbed());
        }

        private bool FloorHeightChanged(float currentY)
        {
            var yDelta = Mathf.Abs(currentY - previousFloorY);
            return (yDelta > floorHeightTolerance || yDelta < -floorHeightTolerance);
        }

        private bool ValidDrop(bool rayHit, RaycastHit rayCollidedWith, float floorY)
        {
            return (rayHit && ValidLocation(rayCollidedWith.transform, rayCollidedWith.point) && !FloorIsGrabbedObject(rayCollidedWith) && FloorHeightChanged(floorY));
        }

        private bool UsePhysicsFall(bool useGravityFall, float floorY)
        {
            float fallDistance = transform.position.y - floorY;
            return (useGravityFall && (playerPresence.IsFalling() || fallDistance > gravityFallHeight));
        }

        private void TeleportFall(bool withBlink, float floorY, RaycastHit rayCollidedWith)
        {
            var floorDelta = currentRayDownY - floorY;
            currentFloor = rayCollidedWith.transform.gameObject;
            currentRayDownY = floorY;
            var newPosition = new Vector3(transform.position.x, floorY, transform.position.z);

            var teleportArgs = new DestinationMarkerEventArgs
            {
                destinationPosition = newPosition,
                distance = rayCollidedWith.distance,
                enableTeleport = true,
                target = currentFloor.transform
            };

            OnTeleporting(gameObject, teleportArgs);
            if (withBlink && (floorDelta > blinkYThreshold || floorDelta < -blinkYThreshold))
            {
                Blink(blinkTransitionSpeed);
            }
            SetNewPosition(newPosition, currentFloor.transform);
            OnTeleported(gameObject, teleportArgs);
        }

        private float ControllerHeightCheck(GameObject controllerObj)
        {
            Debug.DrawRay(controllerObj.transform.transform.position, -transform.up, Color.red);
            Ray ray = new Ray(controllerObj.transform.transform.position, -transform.up);
            RaycastHit rayCollidedWith;
            Physics.Raycast(ray, out rayCollidedWith);
            return controllerObj.transform.position.y - rayCollidedWith.distance;
        }

        private bool ControllersStillOverPreviousFloor()
        {
            if(playSpaceFallRestriction == FallingRestrictors.No_Restriction)
            {
                return false;
            }

            var controllerDropThreshold = 0.05f;
            var rightController = VRTK_DeviceFinder.GetControllerRightHand();
            var leftController = VRTK_DeviceFinder.GetControllerLeftHand();
            var previousY = transform.position.y;
            var rightCheck = (rightController.activeInHierarchy && Mathf.Abs(ControllerHeightCheck(rightController) - previousY) < controllerDropThreshold);
            var leftCheck = (leftController.activeInHierarchy && Mathf.Abs(ControllerHeightCheck(leftController) - previousY) < controllerDropThreshold);

            if(playSpaceFallRestriction == FallingRestrictors.Left_Controller)
            {
                rightCheck = false;
            }

            if (playSpaceFallRestriction == FallingRestrictors.Right_Controller)
            {
                leftCheck = false;
            }

            if(playSpaceFallRestriction == FallingRestrictors.Both_Controllers)
            {
                return (rightCheck && leftCheck);
            }

            return (rightCheck || leftCheck);
        }

        private void DropToNearestFloor(bool withBlink, bool useGravityFall)
        {
            if (enableTeleport && eyeCamera.transform.position.y > transform.position.y)
            {
                Ray ray = new Ray(eyeCamera.transform.position, -transform.up);
                RaycastHit rayCollidedWith;
                bool rayHit = Physics.Raycast(ray, out rayCollidedWith);
                float hitFloorY = eyeCamera.transform.position.y - rayCollidedWith.distance;

                if (ValidDrop(rayHit, rayCollidedWith, hitFloorY))
                {
                    if (initialFloorDrop && hitFloorY < previousFloorY && ControllersStillOverPreviousFloor())
                    {
                        return;
                    }

                    if (UsePhysicsFall(useGravityFall, hitFloorY))
                    {
                        playerPresence.StartPhysicsFall(Vector3.zero);
                    }
                    else
                    {
                        TeleportFall(withBlink, hitFloorY, rayCollidedWith);
                    }
                    initialFloorDrop = true;
                }
                previousFloorY = hitFloorY;
            }
        }

        private bool IsExternalSystemManipulatingPlaySpace()
        {
            return playerPresence.IsFalling() || isClimbing;
        }

        private void Update()
        {
            if (useGravity)
            {
                // if we aren't climbing or falling we can go back to height adjusted falling
                if (!IsExternalSystemManipulatingPlaySpace())
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