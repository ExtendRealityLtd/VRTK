// Fixed Joint Grab Attach|GrabAttachMechanics|50030
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// Attaches the grabbed Interactable Object to the grabbing object via a Fixed Joint.
    /// </summary>
    /// <remarks>
    ///   > The Interactable Object will be attached to the grabbing object via a Fixed Joint and the Joint can be broken upon colliding the Interactable Object with other colliders.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_FixedJointGrabAttach` script on either:
    ///    * The GameObject of the Interactable Object to detect interactions on.
    ///    * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/005_Controller_BasicObjectGrabbing` demonstrates this grab attach mechanic all of the grabbable objects in the scene.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/Grab Attach Mechanics/VRTK_FixedJointGrabAttach")]
    public class VRTK_FixedJointGrabAttach : VRTK_BaseJointGrabAttach
    {
        [Tooltip("Maximum force the Joint can withstand before breaking. Setting to `infinity` ensures the Joint is unbreakable.")]
        public float breakForce = 1500f;

        protected override void CreateJoint(GameObject obj)
        {
            givenJoint = obj.AddComponent<FixedJoint>();
            givenJoint.breakForce = (grabbedObjectScript.IsDroppable() ? breakForce : Mathf.Infinity);
            base.CreateJoint(obj);
        }
    }
}