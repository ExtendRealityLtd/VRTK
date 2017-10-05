// Custom Joint Grab Attach|GrabAttachMechanics|50050
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// Attaches the grabbed Interactable Object to the grabbing object via a custom Joint.
    /// </summary>
    /// <remarks>
    ///   > The Interactable Object will be attached to the grabbing object via a custom Joint and the Joint can be broken upon colliding the Interactable Object with other colliders.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_CustomJointGrabAttach` script on either:
    ///    * The GameObject of the Interactable Object to detect interactions on.
    ///    * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.
    ///  * Create a `Joint` component suitable for attaching the grabbed Interactable Object to the grabbing object with and provide it to the `Custom Joint` parameter.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/021_Controller_GrabbingObjectsWithJoints` demonstrates this grab attach mechanic on the Lamp object in the scene.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/Grab Attach Mechanics/VRTK_CustomJointGrabAttach")]
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
            if (jointHolder == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_CustomJointGrabAttach", "Joint", "customJoint", "the same"));
                return;
            }
            Joint storedJoint = jointHolder.GetComponent<Joint>();
            string storeName = gameObject.name;
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
            if (customJoint != null)
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