// Base Grab Attach|GrabAttachMechanics|50010
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// The Base Grab Attach script is an abstract class that all grab attach script inherit.
    /// </summary>
    /// <remarks>
    /// As this is an abstract class, it cannot be applied directly to a game object and performs no logic.
    /// </remarks>
    public abstract class VRTK_BaseGrabAttach : MonoBehaviour
    {
        [Header("Base Options", order = 1)]

        [Tooltip("If this is checked then when the controller grabs the object, it will grab it with precision and pick it up at the particular point on the object the controller is touching.")]
        public bool precisionGrab;
        [Tooltip("A Transform provided as an empty game object which must be the child of the item being grabbed and serves as an orientation point to rotate and position the grabbed item in relation to the right handed controller. If no Right Snap Handle is provided but a Left Snap Handle is provided, then the Left Snap Handle will be used in place. If no Snap Handle is provided then the object will be grabbed at its central point. Not required for `Precision Snap`.")]
        public Transform rightSnapHandle;
        [Tooltip("A Transform provided as an empty game object which must be the child of the item being grabbed and serves as an orientation point to rotate and position the grabbed item in relation to the left handed controller. If no Left Snap Handle is provided but a Right Snap Handle is provided, then the Right Snap Handle will be used in place. If no Snap Handle is provided then the object will be grabbed at its central point. Not required for `Precision Snap`.")]
        public Transform leftSnapHandle;
        [Tooltip("If checked then when the object is thrown, the distance between the object's attach point and the controller's attach point will be used to calculate a faster throwing velocity.")]
        public bool throwVelocityWithAttachDistance = false;
        [Tooltip("An amount to multiply the velocity of the given object when it is thrown. This can also be used in conjunction with the Interact Grab Throw Multiplier to have certain objects be thrown even further than normal (or thrown a shorter distance if a number below 1 is entered).")]
        public float throwMultiplier = 1f;
        [Tooltip("The amount of time to delay collisions affecting the object when it is first grabbed. This is useful if a game object may get stuck inside another object when it is being grabbed.")]
        public float onGrabCollisionDelay = 0f;

        protected bool tracked;
        protected bool climbable;
        protected bool kinematic;
        protected GameObject grabbedObject;
        protected Rigidbody grabbedObjectRigidBody;
        protected VRTK_InteractableObject grabbedObjectScript;
        protected Transform trackPoint;
        protected Transform grabbedSnapHandle;
        protected Transform initialAttachPoint;
        protected Rigidbody controllerAttachPoint;

        /// <summary>
        /// The IsTracked method determines if the grab attach mechanic is a track object type.
        /// </summary>
        /// <returns>Is true if the mechanic is of type tracked.</returns>
        public virtual bool IsTracked()
        {
            return tracked;
        }

        /// <summary>
        /// The IsClimbable method determines if the grab attach mechanic is a climbable object type.
        /// </summary>
        /// <returns>Is true if the mechanic is of type climbable.</returns>
        public virtual bool IsClimbable()
        {
            return climbable;
        }

        /// <summary>
        /// The IsKinematic method determines if the grab attach mechanic is a kinematic object type.
        /// </summary>
        /// <returns>Is true if the mechanic is of type kinematic.</returns>
        public virtual bool IsKinematic()
        {
            return kinematic;
        }

        /// <summary>
        /// The ValidGrab method determines if the grab attempt is valid.
        /// </summary>
        /// <param name="checkAttachPoint"></param>
        /// <returns>Always returns true for the base check.</returns>
        public virtual bool ValidGrab(Rigidbody checkAttachPoint)
        {
            return true;
        }

        /// <summary>
        /// The SetTrackPoint method sets the point on the grabbed object where the grab is happening.
        /// </summary>
        /// <param name="givenTrackPoint">The track point to set on the grabbed object.</param>
        public virtual void SetTrackPoint(Transform givenTrackPoint)
        {
            trackPoint = givenTrackPoint;
        }

        /// <summary>
        /// The SetInitialAttachPoint method sets the point on the grabbed object where the initial grab happened.
        /// </summary>
        /// <param name="givenInitialAttachPoint">The point where the initial grab took place.</param>
        public virtual void SetInitialAttachPoint(Transform givenInitialAttachPoint)
        {
            initialAttachPoint = givenInitialAttachPoint;
        }

        /// <summary>
        /// The StartGrab method sets up the grab attach mechanic as soon as an object is grabbed.
        /// </summary>
        /// <param name="grabbingObject">The object that is doing the grabbing.</param>
        /// <param name="givenGrabbedObject">The object that is being grabbed.</param>
        /// <param name="givenControllerAttachPoint">The point on the grabbing object that the grabbed object should be attached to after grab occurs.</param>
        /// <returns>Is true if the grab is successful, false if the grab is unsuccessful.</returns>
        public virtual bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)
        {
            grabbedObject = givenGrabbedObject;
            if (grabbedObject == null)
            {
                return false;
            }

            grabbedObjectScript = grabbedObject.GetComponent<VRTK_InteractableObject>();
            grabbedObjectRigidBody = grabbedObject.GetComponent<Rigidbody>();
            controllerAttachPoint = givenControllerAttachPoint;
            grabbedSnapHandle = GetSnapHandle(grabbingObject);

            grabbedObjectScript.PauseCollisions(onGrabCollisionDelay);
            return true;
        }

        /// <summary>
        /// The StopGrab method ends the grab of the current object and cleans up the state.
        /// </summary>
        /// <param name="applyGrabbingObjectVelocity">If true will apply the current velocity of the grabbing object to the grabbed object on release.</param>
        public virtual void StopGrab(bool applyGrabbingObjectVelocity)
        {
            grabbedObject = null;
            grabbedObjectScript = null;
            trackPoint = null;
            grabbedSnapHandle = null;
            initialAttachPoint = null;
            controllerAttachPoint = null;
        }

        /// <summary>
        /// The CreateTrackPoint method sets up the point of grab to track on the grabbed object.
        /// </summary>
        /// <param name="controllerPoint">The point on the controller where the grab was initiated.</param>
        /// <param name="currentGrabbedObject">The object that is currently being grabbed.</param>
        /// <param name="currentGrabbingObject">The object that is currently doing the grabbing.</param>
        /// <param name="customTrackPoint">A reference to whether the created track point is an auto generated custom object.</param>
        /// <returns>The transform of the created track point.</returns>
        public virtual Transform CreateTrackPoint(Transform controllerPoint, GameObject currentGrabbedObject, GameObject currentGrabbingObject, ref bool customTrackPoint)
        {
            customTrackPoint = false;
            return controllerPoint;
        }

        /// <summary>
        /// The ProcessUpdate method is run in every Update method on the interactable object.
        /// </summary>
        public virtual void ProcessUpdate()
        {
        }

        /// <summary>
        /// The ProcessFixedUpdate method is run in every FixedUpdate method on the interactable object.
        /// </summary>
        public virtual void ProcessFixedUpdate()
        {
        }

        protected abstract void Initialise();

        protected virtual Rigidbody ReleaseFromController(bool applyGrabbingObjectVelocity)
        {
            return grabbedObjectRigidBody;
        }

        protected virtual void ForceReleaseGrab()
        {
            if (grabbedObjectScript)
            {
                GameObject grabbingObject = grabbedObjectScript.GetGrabbingObject();
                if (grabbingObject != null)
                {
                    VRTK_InteractGrab grabbingObjectScript = grabbingObject.GetComponent<VRTK_InteractGrab>();
                    if (grabbingObjectScript != null)
                    {
                        grabbingObjectScript.ForceRelease();
                    }
                }
            }
        }

        protected virtual void ReleaseObject(bool applyGrabbingObjectVelocity)
        {
            Rigidbody releasedObjectRigidBody = ReleaseFromController(applyGrabbingObjectVelocity);
            if (releasedObjectRigidBody != null && applyGrabbingObjectVelocity)
            {
                ThrowReleasedObject(releasedObjectRigidBody);
            }
        }

        protected virtual void FlipSnapHandles()
        {
            FlipSnapHandle(rightSnapHandle);
            FlipSnapHandle(leftSnapHandle);
        }

        protected virtual void Awake()
        {
            Initialise();
        }

        protected virtual void ThrowReleasedObject(Rigidbody objectRigidbody)
        {
            if (grabbedObjectScript != null)
            {
                VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(grabbedObjectScript.GetGrabbingObject());
                if (VRTK_ControllerReference.IsValid(controllerReference) && controllerReference.scriptAlias != null)
                {
                    VRTK_InteractGrab grabbingObjectScript = controllerReference.scriptAlias.GetComponent<VRTK_InteractGrab>();
                    if (grabbingObjectScript != null)
                    {
                        Transform origin = VRTK_DeviceFinder.GetControllerOrigin(controllerReference);

                        Vector3 velocity = VRTK_DeviceFinder.GetControllerVelocity(controllerReference);
                        Vector3 angularVelocity = VRTK_DeviceFinder.GetControllerAngularVelocity(controllerReference);
                        float grabbingObjectThrowMultiplier = grabbingObjectScript.throwMultiplier;

                        if (origin != null)
                        {
                            objectRigidbody.velocity = origin.TransformVector(velocity) * (grabbingObjectThrowMultiplier * throwMultiplier);
                            objectRigidbody.angularVelocity = origin.TransformDirection(angularVelocity);
                        }
                        else
                        {
                            objectRigidbody.velocity = velocity * (grabbingObjectThrowMultiplier * throwMultiplier);
                            objectRigidbody.angularVelocity = angularVelocity;
                        }

                        if (throwVelocityWithAttachDistance)
                        {
                            Collider rigidbodyCollider = objectRigidbody.GetComponentInChildren<Collider>();
                            if (rigidbodyCollider != null)
                            {
                                Vector3 collisionCenter = rigidbodyCollider.bounds.center;
                                objectRigidbody.velocity = objectRigidbody.GetPointVelocity(collisionCenter + (collisionCenter - transform.position));
                            }
                            else
                            {
                                objectRigidbody.velocity = objectRigidbody.GetPointVelocity(objectRigidbody.position + (objectRigidbody.position - transform.position));
                            }
                        }
                    }
                }
            }
        }

        protected virtual Transform GetSnapHandle(GameObject grabbingObject)
        {
            if (rightSnapHandle == null && leftSnapHandle != null)
            {
                rightSnapHandle = leftSnapHandle;
            }

            if (leftSnapHandle == null && rightSnapHandle != null)
            {
                leftSnapHandle = rightSnapHandle;
            }

            if (VRTK_DeviceFinder.IsControllerRightHand(grabbingObject))
            {
                return rightSnapHandle;
            }

            if (VRTK_DeviceFinder.IsControllerLeftHand(grabbingObject))
            {
                return leftSnapHandle;
            }

            return null;
        }

        protected virtual void FlipSnapHandle(Transform snapHandle)
        {
            if (snapHandle != null)
            {
                snapHandle.localRotation = Quaternion.Inverse(snapHandle.localRotation);
            }
        }
    }
}