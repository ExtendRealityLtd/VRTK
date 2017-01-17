// Climbable Grab Attach|GrabAttachMechanics|50100
namespace VRTK.GrabAttachMechanics
{

    /// <summary>
    /// The Climbable Grab Attach script is used to mark the object as a climbable interactable object.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/037_CameraRig_ClimbingFalling` uses this grab attach mechanic for each item that is climbable in the scene.
    /// </example>
    public class VRTK_ClimbableGrabAttach : VRTK_BaseGrabAttach
    {
        protected override void Initialise()
        {
            tracked = false;
            climbable = true;
            kinematic = true;
        }
    }
}