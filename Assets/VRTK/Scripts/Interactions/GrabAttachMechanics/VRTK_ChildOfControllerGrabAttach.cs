// Child Of Controller Grab Attach|GrabAttachMechanics|50070
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// The Child Of Controller Grab Attach script is used to make the grabbed object a child of the grabbing object upon grab.
    /// </summary>
    /// <remarks>
    /// The object upon grab will naturally track the position and rotation of the grabbing object as it is a child of the grabbing game object.
    ///
    /// The rigidbody of the object will be set to kinematic upon grab and returned to it's original state on release.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/023_Controller_ChildOfControllerOnGrab` uses this grab attach mechanic for the bow and the arrow.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Grab Attach Mechanics/VRTK_ChildOfControllerGrabAttach")]
    public class VRTK_ChildOfControllerGrabAttach : VRTK_BaseGrabAttach
    {
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
                grabbedObjectScript.isKinematic = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// The StopGrab method ends the grab of the current object and cleans up the state.
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
            kinematic = true;
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
            obj.transform.SetParent(controllerAttachPoint.transform);
        }
    }
}