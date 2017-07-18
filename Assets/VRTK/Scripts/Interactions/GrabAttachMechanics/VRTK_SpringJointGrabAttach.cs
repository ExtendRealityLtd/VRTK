// Spring Joint Grab Attach|GrabAttachMechanics|50050
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// The Spring Joint Grab Attach script is used to create a simple Spring Joint connection between the object and the grabbing object.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/021_Controller_GrabbingObjectsWithJoints` demonstrates this grab attach mechanic on the Drawer object in the scene.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Grab Attach Mechanics/VRTK_SpringJointGrabAttach")]
    public class VRTK_SpringJointGrabAttach : VRTK_BaseJointGrabAttach
    {
        [Tooltip("Maximum force the joint can withstand before breaking. Infinity means unbreakable.")]
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