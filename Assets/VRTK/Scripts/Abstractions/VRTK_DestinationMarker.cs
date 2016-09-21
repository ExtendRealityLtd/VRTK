// Destination Marker|Abstractions|0010
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
    /// <param name="enableTeleport">Whether the destination set event should trigger teleport.</param>
    /// <param name="controllerIndex">The optional index of the controller emitting the beam.</param>
    public struct DestinationMarkerEventArgs
    {
        public float distance;
        public Transform target;
        public RaycastHit raycastHit;
        public Vector3 destinationPosition;
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
    /// It is utilised by the `VRTK_WorldPointer` for dealing with pointer events when the pointer cursor touches areas within the game world.
    /// </remarks>
    public abstract class VRTK_DestinationMarker : MonoBehaviour
    {
        [Tooltip("If this is checked then the teleport flag is set to true in the Destination Set event so teleport scripts will know whether to action the new destination.")]
        public bool enableTeleport = true;

        /// <summary>
        /// Emitted when a collision with another game object has occurred.
        /// </summary>
        public event DestinationMarkerEventHandler DestinationMarkerEnter;
        /// <summary>
        /// Emitted when the collision with the other game object finishes.
        /// </summary>
        public event DestinationMarkerEventHandler DestinationMarkerExit;
        /// <summary>
        /// Emitted when the destination marker is active in the scene to determine the last destination position (useful for selecting and teleporting).
        /// </summary>
        public event DestinationMarkerEventHandler DestinationMarkerSet;

        protected string invalidTargetWithTagOrClass;
        protected VRTK_TagOrScriptPolicyList invalidTagOrScriptListPolicy;
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

        /// <summary>
        /// The SetInvalidTarget method is used to set objects that contain the given tag or class matching the name as invalid destination targets. It can also accept a VRTK_TagOrScriptPolicyList for a more custom level of policy management.
        /// </summary>
        /// <param name="name">The name of the tag or class that is the invalid target.</param>
        /// <param name="list">The Tag Or Script list policy to check the set operation on.</param>
        public virtual void SetInvalidTarget(string name, VRTK_TagOrScriptPolicyList list = null)
        {
            invalidTargetWithTagOrClass = name;
            invalidTagOrScriptListPolicy = list;

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

        protected virtual void OnEnable()
        {
            VRTK_ObjectCache.registeredDestinationMarkers.Add(this);
        }

        protected virtual void OnDisable()
        {
            VRTK_ObjectCache.registeredDestinationMarkers.Remove(this);
        }

        protected DestinationMarkerEventArgs SetDestinationMarkerEvent(float distance, Transform target, RaycastHit raycastHit, Vector3 position, uint controllerIndex)
        {
            DestinationMarkerEventArgs e;
            e.controllerIndex = controllerIndex;
            e.distance = distance;
            e.target = target;
            e.raycastHit = raycastHit;
            e.destinationPosition = position;
            e.enableTeleport = enableTeleport;
            return e;
        }
    }
}