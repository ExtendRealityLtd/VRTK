namespace VRTK.Examples.InteractableHelper
{
    using UnityEngine;

    public class DrawerInitiator : MonoBehaviour
    {
        public ConfigurableJoint joint;

        public virtual void Initiate()
        {
            if (joint == null)
            {
                return;
            }

            joint.targetPosition = Vector3.zero;
            joint.targetVelocity = Vector3.zero;
            JointDrive xDrive = new JointDrive
            {
                positionSpring = 0f,
                positionDamper = 0f,
                maximumForce = 0f
            };
            joint.xDrive = xDrive;
        }
    }
}