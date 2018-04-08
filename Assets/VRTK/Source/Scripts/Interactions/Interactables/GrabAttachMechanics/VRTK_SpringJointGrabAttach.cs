// Spring Joint Grab Attach|GrabAttachMechanics|50040
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// Attaches the grabbed Interactable Object to the grabbing object via a Spring Joint.
    /// </summary>
    /// <remarks>
    ///   > The Interactable Object will be attached to the grabbing object via a Spring Joint and the Joint can be broken upon colliding the Interactable Object with other colliders.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_SpringJointGrabAttach` script on either:
    ///    * The GameObject of the Interactable Object to detect interactions on.
    ///    * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/021_Controller_GrabbingObjectsWithJoints` demonstrates this grab attach mechanic on the Drawer object in the scene.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/Grab Attach Mechanics/VRTK_SpringJointGrabAttach")]
    public class VRTK_SpringJointGrabAttach : VRTK_BaseJointGrabAttach
    {
        [Tooltip("Maximum force the Joint can withstand before breaking. Setting to `infinity` ensures the Joint is unbreakable.")]
        public float breakForce = 1500f;
        [Tooltip("The strength of the spring.")]
        public float strength = 500f;
        [Tooltip("The amount of dampening to apply to the spring.")]
        public float damper = 50f;

        protected override void CreateJoint(GameObject obj)
        {
            SpringJoint tmpJoint = obj.AddComponent<SpringJoint>();
            tmpJoint.breakForce = (grabbedObjectScript.IsDroppable() ? breakForce : Mathf.Infinity);
            tmpJoint.spring = strength;
            tmpJoint.damper = damper;
            givenJoint = tmpJoint;
            base.CreateJoint(obj);
        }
    }
}