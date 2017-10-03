// Dash Teleport|Locomotion|20030
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="hits">An array of RaycastHits that the CapsuleCast has collided with.</param>
    public struct DashTeleportEventArgs
    {
        public RaycastHit[] hits;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="DashTeleportEventArgs"/></param>
    public delegate void DashTeleportEventHandler(object sender, DashTeleportEventArgs e);

    /// <summary>
    /// Updates the `x/y/z` position of the SDK Camera Rig with a lerp to the new position creating a dash effect.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///  * Place the `VRTK_DashTeleport` script on any active scene GameObject.
    ///
    /// **Script Dependencies:**
    ///  * An optional Destination Marker (such as a Pointer) to set the destination of the teleport location.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/038_CameraRig_DashTeleport` shows how to turn off the mesh renderers of objects that are in the way during the dash.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_DashTeleport")]
    public class VRTK_DashTeleport : VRTK_HeightAdjustTeleport
    {
        [Header("Dash Settings")]

        [Tooltip("The fixed time it takes to dash to a new position.")]
        public float normalLerpTime = 0.1f;
        [Tooltip("The minimum speed for dashing in meters per second.")]
        public float minSpeedMps = 50.0f;
        [Tooltip("The Offset of the CapsuleCast above the camera.")]
        public float capsuleTopOffset = 0.2f;
        [Tooltip("The Offset of the CapsuleCast below the camera.")]
        public float capsuleBottomOffset = 0.5f;
        [Tooltip("The radius of the CapsuleCast.")]
        public float capsuleRadius = 0.5f;

        /// <summary>
        /// Emitted when the CapsuleCast towards the target has found that obstacles are in the way.
        /// </summary>
        public event DashTeleportEventHandler WillDashThruObjects;
        /// <summary>
        /// Emitted when obstacles have been crossed and the dash has ended.
        /// </summary>
        public event DashTeleportEventHandler DashedThruObjects;

        protected float minDistanceForNormalLerp;
        protected float lerpTime = 0.1f;
        protected Coroutine attemptLerpRoutine;

        public virtual void OnWillDashThruObjects(DashTeleportEventArgs e)
        {
            if (WillDashThruObjects != null)
            {
                WillDashThruObjects(this, e);
            }
        }

        public virtual void OnDashedThruObjects(DashTeleportEventArgs e)
        {
            if (DashedThruObjects != null)
            {
                DashedThruObjects(this, e);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            minDistanceForNormalLerp = minSpeedMps * normalLerpTime;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (attemptLerpRoutine != null)
            {
                StopCoroutine(attemptLerpRoutine);
                attemptLerpRoutine = null;
            }
        }

        protected override Vector3 SetNewPosition(Vector3 position, Transform target, bool forceDestinationPosition)
        {
            return CheckTerrainCollision(position, target, forceDestinationPosition);
        }

        protected override Quaternion SetNewRotation(Quaternion? rotation)
        {
            if (ValidRigObjects())
            {
                return (rotation != null ? (Quaternion)rotation : playArea.rotation);
            }
            return Quaternion.identity;
        }

        protected override void StartTeleport(object sender, DestinationMarkerEventArgs e)
        {
            base.StartTeleport(sender, e);
        }

        protected override void ProcessOrientation(object sender, DestinationMarkerEventArgs e, Vector3 targetPosition, Quaternion targetRotation)
        {
            if (ValidRigObjects())
            {
                Vector3 finalPosition = CalculateOffsetPosition(targetPosition, targetRotation);
                attemptLerpRoutine = StartCoroutine(lerpToPosition(sender, e, playArea.position, finalPosition, playArea.rotation, targetRotation));
            }
        }

        protected virtual Vector3 CalculateOffsetPosition(Vector3 targetPosition, Quaternion targetRotation)
        {
            if (!headsetPositionCompensation)
            {
                return targetPosition;
            }

            Vector3 playerOffset = new Vector3(headset.position.x - playArea.position.x, 0, headset.position.z - playArea.position.z);
            Quaternion relativeRotation = Quaternion.Inverse(playArea.rotation) * targetRotation;
            Vector3 adjustedOffset = relativeRotation * playerOffset;
            return targetPosition - (adjustedOffset - playerOffset);
        }

        protected override void EndTeleport(object sender, DestinationMarkerEventArgs e)
        {
        }

        protected virtual IEnumerator lerpToPosition(object sender, DestinationMarkerEventArgs e, Vector3 startPosition, Vector3 targetPosition, Quaternion startRotation, Quaternion targetRotation)
        {
            enableTeleport = false;
            bool gameObjectInTheWay = false;

            // Find the objects we will be dashing through and broadcast them via events
            Vector3 eyeCameraPosition = headset.transform.position;
            Vector3 eyeCameraPositionOnGround = new Vector3(eyeCameraPosition.x, playArea.position.y, eyeCameraPosition.z);
            Vector3 eyeCameraRelativeToRig = eyeCameraPosition - playArea.position;
            Vector3 targetEyeCameraPosition = targetPosition + eyeCameraRelativeToRig;
            Vector3 direction = (targetEyeCameraPosition - eyeCameraPosition).normalized;
            Vector3 bottomPoint = eyeCameraPositionOnGround + (Vector3.up * capsuleBottomOffset) + direction;
            Vector3 topPoint = eyeCameraPosition + (Vector3.up * capsuleTopOffset) + direction;
            float maxDistance = Vector3.Distance(playArea.position, targetPosition - direction * 0.5f);
            RaycastHit[] allHits = Physics.CapsuleCastAll(bottomPoint, topPoint, capsuleRadius, direction, maxDistance);

            for (int i = 0; i < allHits.Length; i++)
            {
                gameObjectInTheWay = (allHits[i].collider.gameObject != e.target.gameObject ? true : false);
            }

            if (gameObjectInTheWay)
            {
                OnWillDashThruObjects(SetDashTeleportEvent(allHits));
            }

            lerpTime = (maxDistance >= minDistanceForNormalLerp ? normalLerpTime : VRTK_SharedMethods.DividerToMultiplier(minSpeedMps) * maxDistance);

            float elapsedTime = 0f;
            float currentLerpedTime = 0f;
            WaitForEndOfFrame delayInstruction = new WaitForEndOfFrame();

            while (currentLerpedTime < 1f)
            {
                playArea.position = Vector3.Lerp(startPosition, targetPosition, currentLerpedTime);
                playArea.rotation = Quaternion.Lerp(startRotation, targetRotation, currentLerpedTime);
                elapsedTime += Time.deltaTime;
                currentLerpedTime = elapsedTime / lerpTime;
                yield return delayInstruction;
            }

            playArea.position = targetPosition;
            playArea.rotation = targetRotation;

            if (gameObjectInTheWay)
            {
                OnDashedThruObjects(SetDashTeleportEvent(allHits));
            }

            base.EndTeleport(sender, e);
            gameObjectInTheWay = false;
            enableTeleport = true;
        }

        protected virtual DashTeleportEventArgs SetDashTeleportEvent(RaycastHit[] hits)
        {
            DashTeleportEventArgs e;
            e.hits = hits;
            return e;
        }
    }
}