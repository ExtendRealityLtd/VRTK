// Spring Lever|Controls3D|0071
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// This script extends VRTK_Lever to add spring force toward whichever end of the lever's range it is closest to.
    /// </summary>
    /// <remarks>
    /// The script will instantiate the required Rigidbody, Interactable and HingeJoint components automatically in case they do not exist yet. The joint is very tricky to setup automatically though and will only work in straight forward cases. If there are any issues, then create the HingeJoint component manually and configure it as needed.
    /// </remarks>
    public class VRTK_Spring_Lever : VRTK_Lever
    {
        [Tooltip("Strength of the spring force that will be applied toward either end of the lever's range.")]
        public float springStrength = 10;

        private bool wasTowardZero = true;
        private bool towardZero;

        /// <summary>
        /// Override the original InitRequiredComponents() to add
        /// handling of spring forces on the hingeJoint
        /// </summary>
        protected override void InitRequiredComponents()
        {
            base.InitRequiredComponents();
            if (!hj.useSpring)
            {
                // If useSpring isn't set, the hingeJoint was probably automatically added - fix it
                hj.useSpring = true;
                JointSpring spring = hj.spring;
                spring.spring = springStrength;
                spring.targetPosition = minAngle;
                hj.spring = spring;
            }
            else
            {
                // If useSpring is set, the hingeJoint was manually added - respect its settings
                springStrength = hj.spring.spring;
            }
        }

        /// <summary>
        /// Adjust spring force during HandleUpdate()
        /// </summary>
        protected override void HandleUpdate()
        {
            base.HandleUpdate();
            ApplySpringForce();
        }

        /// <summary>
        /// Check which direction the lever needs to be pushed in and
        /// switch spring direction as necessary
        /// </summary>
        private void ApplySpringForce()
        {
            // get normalized value
            towardZero = (GetNormalizedValue() <= 50);
            if (towardZero != wasTowardZero)
            {
                JointSpring spring = hj.spring;
                spring.targetPosition = (towardZero) ? minAngle : maxAngle;
                hj.spring = spring;
                wasTowardZero = towardZero;
            }
        }
    }
}