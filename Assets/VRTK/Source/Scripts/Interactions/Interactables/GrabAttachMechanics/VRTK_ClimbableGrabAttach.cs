// Climbable Grab Attach|GrabAttachMechanics|50090
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// Marks the Interactable Object as being climbable.
    /// </summary>
    /// <remarks>
    ///   > The Interactable Object will not be grabbed to the controller, instead in conjunction with the `VRTK_PlayerClimb` script will enable the PlayArea to be moved around as if it was climbing.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_ClimbableGrabAttach` script on either:
    ///    * The GameObject of the Interactable Object to detect interactions on.
    ///    * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/037_CameraRig_ClimbingFalling` uses this grab attach mechanic for each item that is climbable in the scene.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/Grab Attach Mechanics/VRTK_ClimbableGrabAttach")]
    public class VRTK_ClimbableGrabAttach : VRTK_BaseGrabAttach
    {
        [Header("Climbable Settings", order = 2)]

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