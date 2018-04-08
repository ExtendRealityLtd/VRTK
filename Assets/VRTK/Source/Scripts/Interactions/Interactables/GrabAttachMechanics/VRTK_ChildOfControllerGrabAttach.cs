// Child Of Controller Grab Attach|GrabAttachMechanics|50060
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// Sets the grabbed Interactable Object to be a child of the grabbing object.
    /// </summary>
    /// <remarks>
    ///   > The Interactable Object will have 1:1 tracking of the grabbing object, however it will also have reduced collision detection and will be able to pass through other colliders.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_ChildOfControllerGrabAttach` script on either:
    ///    * The GameObject of the Interactable Object to detect interactions on.
    ///    * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/023_Controller_ChildOfControllerOnGrab` uses this grab attach mechanic for the bow and the arrow.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/Grab Attach Mechanics/VRTK_ChildOfControllerGrabAttach")]
    public class VRTK_ChildOfControllerGrabAttach : VRTK_BaseGrabAttach
    {
        /// <summary>
        /// The StartGrab method sets up the grab attach mechanic as soon as an Interactable Object is grabbed. It is also responsible for creating the joint on the grabbed object.
        /// </summary>
        /// <param name="grabbingObject">The GameObject that is doing the grabbing.</param>
        /// <param name="givenGrabbedObject">The GameObject that is being grabbed.</param>
        /// <param name="givenControllerAttachPoint">The point on the grabbing object that the grabbed object should be attached to after grab occurs.</param>
        /// <returns>Returns `true` if the grab is successful, `false` if the grab is unsuccessful.</returns>
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
        /// The StopGrab method ends the grab of the current Interactable Object and cleans up the state.
        /// </summary>
        /// <param name="applyGrabbingObjectVelocity">If `true` will apply the current velocity of the grabbing object to the grabbed object on release.</param>
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
                obj.transform.rotation = controllerAttachPoint.transform.rotation * Quaternion.Inverse(grabbedSnapHandle.transform.localRotation);
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