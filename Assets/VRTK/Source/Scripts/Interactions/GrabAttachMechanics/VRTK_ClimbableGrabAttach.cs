// Climbable Grab Attach|GrabAttachMechanics|50100
using UnityEngine;

namespace VRTK.GrabAttachMechanics
{

    /// <summary>
    /// The Climbable Grab Attach script is used to mark the object as a climbable interactable object.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/037_CameraRig_ClimbingFalling` uses this grab attach mechanic for each item that is climbable in the scene.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Grab Attach Mechanics/VRTK_ClimbableGrabAttach")]
    public class VRTK_ClimbableGrabAttach : VRTK_BaseGrabAttach
    {

        [Tooltip("Will respect the grabbed climbing object's rotation if it changes dynamically")]
        public bool useObjectRotation = false;

        protected override void Initialise()
        {
            tracked = false;
            climbable = true;
            kinematic = true;
        }
    }
}