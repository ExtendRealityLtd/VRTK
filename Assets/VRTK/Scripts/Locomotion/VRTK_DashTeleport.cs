// Dash Teleport|Locomotion|20030
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="hits">An array of objects that the CapsuleCast has collided with.</param>
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
    /// The dash teleporter extends the height adjust teleporter and allows to have the user's position dashing to a new teleport location. 
    /// </summary>
    /// <remarks>
    /// The basic principle is to dash for a very short amount of time, to avoid sim sickness. The default value is 100 miliseconds. This value is fixed for all normal and longer distances. When the distances get very short the minimum speed is clamped to 50 mps, so the dash time becomes even shorter.
    ///
    /// The minimum distance for the fixed time dash is determined by the minSpeed and normalLerpTime values, if you want to always lerp with a fixed mps speed instead, set the normalLerpTime to a high value. Right before the teleport a capsule is cast towards the target and registers all colliders blocking the way. These obstacles are then broadcast in an event so that for example their gameobjects or renderers can be turned off while the dash is in progress.
    /// </remarks>
    /// <example>
    /// `SteamVR_Unity_Toolkit/Examples/038_CameraRig_DashTeleport` shows how to turn off the mesh renderers of objects that are in the way during the dash.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_DashTeleport")]
    public class VRTK_DashTeleport : VRTK_HeightAdjustTeleport
    {
        [Header("Dash Settings")]

        [Tooltip("The fixed time it takes to dash to a new position.")]
        public float normalLerpTime = 0.1f; // 100ms for every dash above minDistanceForNormalLerp
        [Tooltip("The minimum speed for dashing in meters per second.")]
        public float minSpeedMps = 50.0f; // clamped to minimum speed 50m/s to avoid sickness
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

        // The minimum distance for fixed time lerp is determined by the minSpeed and the normalLerpTime
        // If you want to always lerp with a fixed mps speed, set the normalLerpTime to a high value
        protected float minDistanceForNormalLerp;
        protected float lerpTime = 0.1f;

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
            minDistanceForNormalLerp = minSpeedMps * normalLerpTime; // default values give 5.0f
        }

        protected override Vector3 SetNewPosition(Vector3 position, Transform target, bool forceDestinationPosition)
        {
            return CheckTerrainCollision(position, target, forceDestinationPosition);
        }

        protected override void StartTeleport(object sender, DestinationMarkerEventArgs e)
        {
            base.StartTeleport(sender, e);
        }

        protected override void ProcessOrientation(object sender, DestinationMarkerEventArgs e, Vector3 newPosition, Quaternion newRotation)
        {
            if (ValidRigObjects())
            {
                StartCoroutine(lerpToPosition(sender, e, newPosition));
            }
        }

        protected override void EndTeleport(object sender, DestinationMarkerEventArgs e)
        {
        }

        protected virtual IEnumerator lerpToPosition(object sender, DestinationMarkerEventArgs e, Vector3 targetPosition)
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

            foreach (RaycastHit hit in allHits)
            {
                gameObjectInTheWay = (hit.collider.gameObject != e.target.gameObject ? true : false);
            }

            if (gameObjectInTheWay)
            {
                OnWillDashThruObjects(SetDashTeleportEvent(allHits));
            }

            if (maxDistance >= minDistanceForNormalLerp)
            {
                lerpTime = normalLerpTime; // fixed time for all bigger dashes
            }
            else
            {
                lerpTime = (1f / minSpeedMps) * maxDistance; // clamped to speed for small dashes
            }

            Vector3 startPosition = new Vector3(playArea.position.x, playArea.position.y, playArea.position.z);
            float elapsedTime = 0;
            float t = 0;

            while (t < 1)
            {
                playArea.position = Vector3.Lerp(startPosition, targetPosition, t);
                elapsedTime += Time.deltaTime;
                t = elapsedTime / lerpTime;
                if (t > 1)
                {
                    if (playArea.position != targetPosition)
                    {
                        playArea.position = targetPosition;
                    }
                    t = 1;
                }
                yield return new WaitForEndOfFrame();
            }

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