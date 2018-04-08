// Track Object Grab Attach|GrabAttachMechanics|50070
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// Applies velocity to the grabbed Interactable Object to ensure it tracks the position of the grabbing object.
    /// </summary>
    /// <remarks>
    ///   > The Interactable Object follows the grabbing object based on velocity being applied and therefore fully interacts with all other scene Colliders but not at a true 1:1 tracking.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_TrackObjectGrabAttach` script on either:
    ///    * The GameObject of the Interactable Object to detect interactions on.
    ///    * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/021_Controller_GrabbingObjectsWithJoints` demonstrates this grab attach mechanic on the Chest handle and Fire Extinguisher body.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/Grab Attach Mechanics/VRTK_TrackObjectGrabAttach")]
    public class VRTK_TrackObjectGrabAttach : VRTK_BaseGrabAttach
    {
        [Header("Track Settings", order = 2)]

        [Tooltip("The maximum distance the grabbing object is away from the Interactable Object before it is automatically dropped.")]
        public float detachDistance = 1f;
        [Tooltip("The maximum amount of velocity magnitude that can be applied to the Interactable Object. Lowering this can prevent physics glitches if Interactable Objects are moving too fast.")]
        public float velocityLimit = float.PositiveInfinity;
        [Tooltip("The maximum amount of angular velocity magnitude that can be applied to the Interactable Object. Lowering this can prevent physics glitches if Interactable Objects are moving too fast.")]
        public float angularVelocityLimit = float.PositiveInfinity;
        [Tooltip("The maximum difference in distance to the tracked position.")]
        public float maxDistanceDelta = 10f;

        protected bool isReleasable = true;

        /// <summary>
        /// The StopGrab method ends the grab of the current Interactable Object and cleans up the state.
        /// </summary>
        /// <param name="applyGrabbingObjectVelocity">If `true` will apply the current velocity of the grabbing object to the grabbed Interactable Object on release.</param>
        public override void StopGrab(bool applyGrabbingObjectVelocity)
        {
            if (isReleasable)
            {
                ReleaseObject(applyGrabbingObjectVelocity);
            }
            base.StopGrab(applyGrabbingObjectVelocity);
        }

        /// <summary>
        /// The CreateTrackPoint method sets up the point of grab to track on the grabbed object.
        /// </summary>
        /// <param name="controllerPoint">The point on the controller where the grab was initiated.</param>
        /// <param name="currentGrabbedObject">The GameObject that is currently being grabbed.</param>
        /// <param name="currentGrabbingObject">The GameObject that is currently doing the grabbing.</param>
        /// <param name="customTrackPoint">A reference to whether the created track point is an auto generated custom object.</param>
        /// <returns>The Transform of the created track point.</returns>
        public override Transform CreateTrackPoint(Transform controllerPoint, GameObject currentGrabbedObject, GameObject currentGrabbingObject, ref bool customTrackPoint)
        {
            Transform returnTrackpoint = null;
            if (precisionGrab)
            {
                returnTrackpoint = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, currentGrabbedObject.name, "TrackObject", "PrecisionSnap", "AttachPoint")).transform;
                returnTrackpoint.SetParent(currentGrabbingObject.transform);
                returnTrackpoint = SetTrackPointOrientation(returnTrackpoint, currentGrabbedObject.transform, controllerPoint);
                customTrackPoint = true;
            }
            else
            {
                returnTrackpoint = base.CreateTrackPoint(controllerPoint, currentGrabbedObject, currentGrabbingObject, ref customTrackPoint);
            }
            return returnTrackpoint;
        }

        /// <summary>
        /// The ProcessUpdate method is run in every Update method on the Interactable Object. It is responsible for checking if the tracked object has exceeded it's detach distance.
        /// </summary>
        public override void ProcessUpdate()
        {
            if (trackPoint != null && grabbedObjectScript.IsDroppable())
            {
                float distance = Vector3.Distance(trackPoint.position, initialAttachPoint.position);
                if (distance > detachDistance)
                {
                    ForceReleaseGrab();
                }
            }
        }

        /// <summary>
        /// The ProcessFixedUpdate method is run in every FixedUpdate method on the Interactable Object. It applies velocity to the object to ensure it is tracking the grabbing object.
        /// </summary>
        public override void ProcessFixedUpdate()
        {
            if (grabbedObject == null)
            {
                return;
            }

            Vector3 positionDelta = trackPoint.position - (grabbedSnapHandle != null ? grabbedSnapHandle.position : grabbedObject.transform.position);
            Quaternion rotationDelta = trackPoint.rotation * Quaternion.Inverse((grabbedSnapHandle != null ? grabbedSnapHandle.rotation : grabbedObject.transform.rotation));

            float angle;
            Vector3 axis;
            rotationDelta.ToAngleAxis(out angle, out axis);

            angle = ((angle > 180) ? angle -= 360 : angle);

            if (angle != 0)
            {
                Vector3 angularTarget = angle * axis;
                Vector3 calculatedAngularVelocity = Vector3.MoveTowards(grabbedObjectRigidBody.angularVelocity, angularTarget, maxDistanceDelta);
                if (angularVelocityLimit == float.PositiveInfinity || calculatedAngularVelocity.sqrMagnitude < angularVelocityLimit)
                {
                    grabbedObjectRigidBody.angularVelocity = calculatedAngularVelocity;
                }
            }

            Vector3 velocityTarget = positionDelta / Time.fixedDeltaTime;
            Vector3 calculatedVelocity = Vector3.MoveTowards(grabbedObjectRigidBody.velocity, velocityTarget, maxDistanceDelta);

            if (velocityLimit == float.PositiveInfinity || calculatedVelocity.sqrMagnitude < velocityLimit)
            {
                grabbedObjectRigidBody.velocity = calculatedVelocity;
            }
        }

        protected override void Initialise()
        {
            tracked = true;
            climbable = false;
            kinematic = false;
        }

        protected virtual Transform SetTrackPointOrientation(Transform givenTrackPoint, Transform currentGrabbedObject, Transform controllerPoint)
        {
            givenTrackPoint.position = currentGrabbedObject.position;
            givenTrackPoint.rotation = currentGrabbedObject.rotation;
            return givenTrackPoint;
        }
    }
}