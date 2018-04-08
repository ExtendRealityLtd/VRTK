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
    /// <param name="controllerReference">The optional reference to the controller controlling the destination marker.</param>
    public struct DestinationMarkerEventArgs
    {
        public float distance;
        public Transform target;
        public RaycastHit raycastHit;
        public Vector3 destinationPosition;
        public Quaternion? destinationRotation;
        public bool forceDestinationPosition;
        public bool enableTeleport;
        public VRTK_ControllerReference controllerReference;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="DestinationMarkerEventArgs"/></param>
    public delegate void DestinationMarkerEventHandler(object sender, DestinationMarkerEventArgs e);

    /// <summary>
    /// Provides a base that all destination markers can inherit from.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///   > This is an abstract class that is to be inherited to a concrete class that provides object control action functionality, therefore this script should not be directly used.
    /// </remarks>
    public abstract class VRTK_DestinationMarker : MonoBehaviour
    {
        [Header("Destination Marker Settings", order = 1)]

        [Tooltip("If this is checked then the teleport flag is set to true in the Destination Set event so teleport scripts will know whether to action the new destination.")]
        public bool enableTeleport = true;
        [Tooltip("A specified VRTK_PolicyList to use to determine whether destination targets will be considered valid or invalid.")]
        public VRTK_PolicyList targetListPolicy;

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

        [System.Obsolete("`VRTK_DestinationMarker.navMeshCheckDistance` is no longer used. This parameter will be removed in a future version of VRTK.")]
        protected float navMeshCheckDistance = 0f;

        protected VRTK_NavMeshData navmeshData;
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
        /// The SetNavMeshCheckDistance method sets the max distance the destination marker position can be from the edge of a nav mesh to be considered a valid destination.
        /// </summary>
        /// <param name="distance">The max distance the nav mesh can be from the sample point to be valid.</param>
        [System.Obsolete("`DestinationMarker.SetNavMeshCheckDistance(distance)` has been replaced with the method `DestinationMarker.SetNavMeshCheckDistance(givenData)`. This method will be removed in a future version of VRTK.")]
        public virtual void SetNavMeshCheckDistance(float distance)
        {
            VRTK_NavMeshData givenData = gameObject.AddComponent<VRTK_NavMeshData>();
            givenData.distanceLimit = distance;
            SetNavMeshData(givenData);
        }

        /// <summary>
        /// The SetNavMeshData method is used to limit the destination marker to the scene NavMesh based on the settings in the given NavMeshData object.
        /// </summary>
        /// <param name="givenData">The NavMeshData object that contains the NavMesh restriction settings.</param>
        public virtual void SetNavMeshData(VRTK_NavMeshData givenData)
        {
            navmeshData = givenData;
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

        protected virtual DestinationMarkerEventArgs SetDestinationMarkerEvent(float distance, Transform target, RaycastHit raycastHit, Vector3 position, VRTK_ControllerReference controllerReference, bool forceDestinationPosition = false, Quaternion? rotation = null)
        {
            DestinationMarkerEventArgs e;
            e.controllerReference = controllerReference;
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