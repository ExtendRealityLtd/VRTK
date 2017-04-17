// Destination Marker|Pointers|10010
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="distance">The distance between the origin and the collided destination.</param>
    /// <param name="target">The Transform of the collided destination object.</param>
    /// <param name="raycastHit">The optional RaycastHit generated from when the ray collided.</param>
    /// <param name="destinationPosition">The world position of the destination marker.</param>
    /// <param name="destinationRotation">The world rotation of the destination marker.</param>
    /// <param name="forceDestinationPosition">If true then the given destination position should not be altered by anything consuming the payload.</param>
    /// <param name="enableTeleport">Whether the destination set event should trigger teleport.</param>
    /// <param name="controllerIndex">The optional index of the controller emitting the beam.</param>
    public struct DestinationMarkerEventArgs
    {
        public float distance;
        public Transform target;
        public RaycastHit raycastHit;
        public Vector3 destinationPosition;
        public Quaternion? destinationRotation;
        public bool forceDestinationPosition;
        public bool enableTeleport;
        public uint controllerIndex;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="DestinationMarkerEventArgs"/></param>
    public delegate void DestinationMarkerEventHandler(object sender, DestinationMarkerEventArgs e);

    /// <summary>
    /// This abstract class provides the ability to emit events of destination markers within the game world. It can be useful for tagging locations for specific purposes such as teleporting.
    /// </summary>
    /// <remarks>
    /// It is utilised by the `VRTK_BasePointer` for dealing with pointer events when the pointer cursor touches areas within the game world.
    /// </remarks>
    public abstract class VRTK_DestinationMarker : MonoBehaviour
    {
        [Header("Destination Marker Settings", order = 1)]
        [Tooltip("If this is checked then the teleport flag is set to true in the Destination Set event so teleport scripts will know whether to action the new destination.")]
        public bool enableTeleport = true;

        /// <summary>
        /// Emitted when a collision with another collider has first occurred.
        /// </summary>
        public event DestinationMarkerEventHandler DestinationMarkerEnter;
        /// <summary>
        /// Emitted when the collision with the other collider ends.
        /// </summary>
        public event DestinationMarkerEventHandler DestinationMarkerExit;
        /// Emitted when a collision the existing collider is continuing.
        /// </summary>
        public event DestinationMarkerEventHandler DestinationMarkerHover;
        /// <summary>
        /// Emitted when the destination marker is active in the scene to determine the last destination position (useful for selecting and teleporting).
        /// </summary>
        public event DestinationMarkerEventHandler DestinationMarkerSet;

        protected VRTK_PolicyList invalidListPolicy;
        protected float navMeshCheckDistance;
        protected bool headsetPositionCompensation;
        protected bool forceHoverOnRepeatedEnter = true;
        protected Collider existingCollider;

        public virtual void OnDestinationMarkerEnter(DestinationMarkerEventArgs e)
        {
            if (DestinationMarkerEnter != null && (!forceHoverOnRepeatedEnter || (e.raycastHit.collider != existingCollider)))
            {
                existingCollider = e.raycastHit.collider;
                DestinationMarkerEnter(this, e);
            }

            if (forceHoverOnRepeatedEnter && e.raycastHit.collider == existingCollider)
            {
                OnDestinationMarkerHover(e);
            }
        }

        public virtual void OnDestinationMarkerExit(DestinationMarkerEventArgs e)
        {
            if (DestinationMarkerExit != null)
            {
                DestinationMarkerExit(this, e);
                existingCollider = null;
            }
        }

        public virtual void OnDestinationMarkerHover(DestinationMarkerEventArgs e)
        {
            if (DestinationMarkerHover != null)
            {
                DestinationMarkerHover(this, e);
            }
        }

        public virtual void OnDestinationMarkerSet(DestinationMarkerEventArgs e)
        {
            if (DestinationMarkerSet != null)
            {
                DestinationMarkerSet(this, e);
            }
        }

        /// <summary>
        /// The SetInvalidTarget method is used to set objects that contain the given tag or class matching the name as invalid destination targets. It accepts a VRTK_PolicyList for a custom level of policy management.
        /// </summary>
        /// <param name="list">The Tag Or Script list policy to check the set operation on.</param>
        public virtual void SetInvalidTarget(VRTK_PolicyList list = null)
        {
            invalidListPolicy = list;
        }

        /// <summary>
        /// The SetNavMeshCheckDistance method sets the max distance the destination marker position can be from the edge of a nav mesh to be considered a valid destination.
        /// </summary>
        /// <param name="distance">The max distance the nav mesh can be from the sample point to be valid.</param>
        public virtual void SetNavMeshCheckDistance(float distance)
        {
            navMeshCheckDistance = distance;
        }

        /// <summary>
        /// The SetHeadsetPositionCompensation method determines whether the offset position of the headset from the centre of the play area should be taken into consideration when setting the destination marker. If `true` then it will take the offset position into consideration.
        /// </summary>
        /// <param name="state">The state of whether to take the position of the headset within the play area into account when setting the destination marker.</param>
        public virtual void SetHeadsetPositionCompensation(bool state)
        {
            headsetPositionCompensation = state;
        }

        /// <summary>
        /// The SetForceHoverOnRepeatedEnter method is used to set whether the Enter event will forciably call the Hover event if the existing colliding object is the same as it was the previous enter call.
        /// </summary>
        /// <param name="state">The state of whether to force the hover on or off.</param>
        public virtual void SetForceHoverOnRepeatedEnter(bool state)
        {
            forceHoverOnRepeatedEnter = state;
        }

        protected virtual void OnEnable()
        {
            VRTK_ObjectCache.registeredDestinationMarkers.Add(this);
        }

        protected virtual void OnDisable()
        {
            VRTK_ObjectCache.registeredDestinationMarkers.Remove(this);
        }

        protected virtual DestinationMarkerEventArgs SetDestinationMarkerEvent(float distance, Transform target, RaycastHit raycastHit, Vector3 position, uint controllerIndex, bool forceDestinationPosition = false, Quaternion? rotation = null)
        {
            DestinationMarkerEventArgs e;
            e.controllerIndex = controllerIndex;
            e.distance = distance;
            e.target = target;
            e.raycastHit = raycastHit;
            e.destinationPosition = position;
            e.destinationRotation = rotation;
            e.enableTeleport = enableTeleport;
            e.forceDestinationPosition = forceDestinationPosition;
            return e;
        }
    }
}