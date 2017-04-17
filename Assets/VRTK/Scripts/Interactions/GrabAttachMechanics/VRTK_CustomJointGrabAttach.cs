// Custom Joint Grab Attach|GrabAttachMechanics|50060
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// The Custom Joint Grab Attach script allows a custom joint to be provided for the grab attach mechanic.
    /// </summary>
    /// <remarks>
    /// The custom joint is placed on the interactable object and at runtime the joint is copied into a `JointHolder` game object that becomes a child of the interactable object.
    ///
    /// The custom joint is then copied from this `JointHolder` to the interactable object when a grab happens and is removed when a grab ends.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/021_Controller_GrabbingObjectsWithJoints` demonstrates this grab attach mechanic on the Lamp object in the scene.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Grab Attach Mechanics/VRTK_CustomJointGrabAttach")]
    public class VRTK_CustomJointGrabAttach : VRTK_BaseJointGrabAttach
    {
        [Tooltip("The joint to use for the grab attach joint.")]
        public Joint customJoint;

        protected GameObject jointHolder;

        protected override void Initialise()
        {
            base.Initialise();
            CopyCustomJoint();
        }

        protected override void CreateJoint(GameObject obj)
        {
            if (!jointHolder)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_CustomJointGrabAttach", "Joint", "customJoint", "the same"));
                return;
            }
            var storedJoint = jointHolder.GetComponent<Joint>();
            var storeName = gameObject.name;
            VRTK_SharedMethods.CloneComponent(storedJoint, obj, true);
            gameObject.name = storeName;
            givenJoint = obj.GetComponent(storedJoint.GetType()) as Joint;

            base.CreateJoint(obj);
        }

        protected override void DestroyJoint(bool withDestroyImmediate, bool applyGrabbingObjectVelocity)
        {
            base.DestroyJoint(true, true);
        }

        protected virtual void CopyCustomJoint()
        {
            if (customJoint)
            {
                jointHolder = new GameObject();
                jointHolder.transform.SetParent(transform);
                jointHolder.AddComponent<Rigidbody>().isKinematic = true;
                VRTK_SharedMethods.CloneComponent(customJoint, jointHolder, true);
                jointHolder.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, "JointHolder");
                jointHolder.SetActive(false);
                Destroy(customJoint);
                customJoint = jointHolder.GetComponent<Joint>();
            }
        }
    }
}