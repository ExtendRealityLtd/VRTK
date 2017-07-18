// Rotator Track Grab Attach|GrabAttachMechanics|50090
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// The Rotator Track Grab Attach script is used to track the object but instead of the object tracking the direction of the controller, a force is applied to the object to cause it to rotate.
    /// </summary>
    /// <remarks>
    /// This is ideal for hinged joints on items such as wheels or doors.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/021_Controller_GrabbingObjectsWithJoints` demonstrates this grab attach mechanic on the Wheel and Door objects in the scene.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Grab Attach Mechanics/VRTK_RotatorTrackGrabAttach")]
    public class VRTK_RotatorTrackGrabAttach : VRTK_TrackObjectGrabAttach
    {
        /// <summary>
        /// The StopGrab method ends the grab of the current object and cleans up the state.
        /// </summary>
        /// <param name="applyGrabbingObjectVelocity">If true will apply the current velocity of the grabbing object to the grabbed object on release.</param>
        public override void StopGrab(bool applyGrabbingObjectVelocity)
        {
            isReleasable = false;
            base.StopGrab(applyGrabbingObjectVelocity);
        }

        /// <summary>
        /// The ProcessFixedUpdate method is run in every FixedUpdate method on the interactable object. It applies a force to the grabbed object to move it in the direction of the grabbing object.
        /// </summary>
        public override void ProcessFixedUpdate()
        {
            var rotateForce = trackPoint.position - initialAttachPoint.position;
            grabbedObjectRigidBody.AddForceAtPosition(rotateForce, initialAttachPoint.position, ForceMode.VelocityChange);
        }

        protected override void SetTrackPointOrientation(ref Transform trackPoint, Transform currentGrabbedObject, Transform controllerPoint)
        {
            trackPoint.position = controllerPoint.position;
            trackPoint.rotation = controllerPoint.rotation;
        }
    }
}