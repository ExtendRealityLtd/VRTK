// Rotator Track Grab Attach|GrabAttachMechanics|50080
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// Applies a rotational force to the grabbed Interactable Object.
    /// </summary>
    /// <remarks>
    ///   > The Interactable Object is not attached to the grabbing object but rather has a rotational force applied based on the rotation of the grabbing object.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_RotatorTrackGrabAttach` script on either:
    ///    * The GameObject of the Interactable Object to detect interactions on.
    ///    * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/021_Controller_GrabbingObjectsWithJoints` demonstrates this grab attach mechanic on the Wheel and Door objects in the scene.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/Grab Attach Mechanics/VRTK_RotatorTrackGrabAttach")]
    public class VRTK_RotatorTrackGrabAttach : VRTK_TrackObjectGrabAttach
    {
        /// <summary>
        /// The StopGrab method ends the grab of the current Interactable Object and cleans up the state.
        /// </summary>
        /// <param name="applyGrabbingObjectVelocity">If `true` will apply the current velocity of the grabbing object to the grabbed Interactable Object on release.</param>
        public override void StopGrab(bool applyGrabbingObjectVelocity)
        {
            isReleasable = false;
            base.StopGrab(applyGrabbingObjectVelocity);
        }

        /// <summary>
        /// The ProcessFixedUpdate method is run in every FixedUpdate method on the Interactable Object. It applies a force to the grabbed Interactable Object to move it in the direction of the grabbing object.
        /// </summary>
        public override void ProcessFixedUpdate()
        {
            Vector3 rotateForce = trackPoint.position - initialAttachPoint.position;
            grabbedObjectRigidBody.AddForceAtPosition(rotateForce, initialAttachPoint.position, ForceMode.VelocityChange);
        }

        protected override Transform SetTrackPointOrientation(Transform givenTrackPoint, Transform currentGrabbedObject, Transform controllerPoint)
        {
            givenTrackPoint.position = controllerPoint.position;
            givenTrackPoint.rotation = controllerPoint.rotation;
            return givenTrackPoint;
        }
    }
}