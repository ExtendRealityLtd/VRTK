// Base Joint Grab Attach|GrabAttachMechanics|50020
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// The Base Joint Grab Attach script is an abstract class that all joint grab attach types inherit.
    /// </summary>
    /// <remarks>
    /// As this is an abstract class, it cannot be applied directly to a game object and performs no logic.
    /// </remarks>
    public abstract class VRTK_BaseJointGrabAttach : VRTK_BaseGrabAttach
    {
        [Header("Joint Options", order = 2)]

        [Tooltip("Determines whether the joint should be destroyed immediately on release or whether to wait till the end of the frame before being destroyed.")]
        public bool destroyImmediatelyOnThrow = true;

        protected Joint givenJoint;
        protected Joint controllerAttachJoint;

        /// <summary>
        /// The ValidGrab method determines if the grab attempt is valid.
        /// </summary>
        /// <param name="checkAttachPoint"></param>
        /// <returns>Returns true if there is no current grab happening, or the grab is initiated by another grabbing object.</returns>
        public override bool ValidGrab(Rigidbody checkAttachPoint)
        {
            return (controllerAttachJoint == null || controllerAttachJoint.connectedBody != checkAttachPoint);
        }

        /// <summary>
        /// The StartGrab method sets up the grab attach mechanic as soon as an object is grabbed. It is also responsible for creating the joint on the grabbed object.
        /// </summary>
        /// <param name="grabbingObject">The object that is doing the grabbing.</param>
        /// <param name="givenGrabbedObject">The object that is being grabbed.</param>
        /// <param name="givenControllerAttachPoint">The point on the grabbing object that the grabbed object should be attached to after grab occurs.</param>
        /// <returns>Is true if the grab is successful, false if the grab is unsuccessful.</returns>
        public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)
        {
            if (base.StartGrab(grabbingObject, givenGrabbedObject, givenControllerAttachPoint))
            {
                SnapObjectToGrabToController(givenGrabbedObject);
                return true;
            }
            return false;
        }

        /// <summary>
        /// The StopGrab method ends the grab of the current object and cleans up the state. It is also responsible for removing the joint from the grabbed object.
        /// </summary>
        /// <param name="applyGrabbingObjectVelocity">If true will apply the current velocity of the grabbing object to the grabbed object on release.</param>
        public override void StopGrab(bool applyGrabbingObjectVelocity)
        {
            ReleaseObject(applyGrabbingObjectVelocity);
            base.StopGrab(applyGrabbingObjectVelocity);
        }

        protected override void Initialise()
        {
            tracked = false;
            climbable = false;
            kinematic = false;
            controllerAttachJoint = null;
        }

        protected override Rigidbody ReleaseFromController(bool applyGrabbingObjectVelocity)
        {
            if(controllerAttachJoint)
            {
                var jointRigidbody = controllerAttachJoint.GetComponent<Rigidbody>();
                DestroyJoint(destroyImmediatelyOnThrow, applyGrabbingObjectVelocity);
                controllerAttachJoint = null;

                return jointRigidbody;
            }
            return null;
        }

        protected virtual void OnJointBreak(float force)
        {
            ForceReleaseGrab();
        }

        protected virtual void CreateJoint(GameObject obj)
        {
            if (precisionGrab)
            {
                givenJoint.anchor = obj.transform.InverseTransformPoint(controllerAttachPoint.position);
            }
            controllerAttachJoint = givenJoint;
            controllerAttachJoint.breakForce = (grabbedObjectScript.IsDroppable() ? controllerAttachJoint.breakForce : Mathf.Infinity);
            controllerAttachJoint.connectedBody = controllerAttachPoint;
        }

        protected virtual void DestroyJoint(bool withDestroyImmediate, bool applyGrabbingObjectVelocity)
        {
            controllerAttachJoint.connectedBody = null;
            if (withDestroyImmediate && applyGrabbingObjectVelocity)
            {
                DestroyImmediate(controllerAttachJoint);
            }
            else
            {
                Destroy(controllerAttachJoint);
            }
        }

        protected virtual void SetSnappedObjectPosition(GameObject obj)
        {
            if (grabbedSnapHandle == null)
            {
                obj.transform.position = controllerAttachPoint.transform.position;
            }
            else
            {
                obj.transform.rotation = controllerAttachPoint.transform.rotation * Quaternion.Euler(grabbedSnapHandle.transform.localEulerAngles);
                obj.transform.position = controllerAttachPoint.transform.position - (grabbedSnapHandle.transform.position - obj.transform.position);
            }
        }

        protected virtual void SnapObjectToGrabToController(GameObject obj)
        {
            if (!precisionGrab)
            {
                SetSnappedObjectPosition(obj);
            }
            CreateJoint(obj);
        }
    }
}