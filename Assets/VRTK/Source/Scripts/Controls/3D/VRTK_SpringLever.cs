// Spring Lever|Controls3D|100080
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// This script extends VRTK_Lever to add spring force toward whichever end of the lever's range it is closest to.
    /// </summary>
    /// <remarks>
    /// The script will instantiate the required Rigidbody, Interactable and HingeJoint components automatically in case they do not exist yet. The joint is very tricky to setup automatically though and will only work in straight forward cases. If there are any issues, then create the HingeJoint component manually and configure it as needed.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Controls/3D/VRTK_SpringLever")]
    [System.Obsolete("`VRTK.VRTK_SpringLever` has been deprecated and can be recreated with `VRTK.Controllables.PhysicsBased.VRTK_PhysicsRotator`. This script will be removed in a future version of VRTK.")]
    public class VRTK_SpringLever : VRTK_Lever
    {
        [Tooltip("The strength of the spring force that will be applied upon the lever.")]
        public float springStrength = 10;
        [Tooltip("The damper of the spring force that will be applied upon the lever.")]
        public float springDamper = 10;
        [Tooltip("If this is checked then the spring will snap the lever to the nearest end point (either min or max angle). If it is unchecked, the lever will always snap to the min angle position.")]
        public bool snapToNearestLimit = false;
        [Tooltip("If this is checked then the spring will always be active even when grabbing the lever.")]
        public bool alwaysActive = false;

        protected bool wasTowardZero = true;
        protected bool isGrabbed = false;

        /// <summary>
        /// Override the original InitRequiredComponents() to add
        /// handling of spring forces on the hingeJoint
        /// </summary>
        protected override void InitRequiredComponents()
        {
            base.InitRequiredComponents();
            if (!leverHingeJoint.useSpring)
            {
                // If useSpring isn't set, the hingeJoint was probably automatically added - fix it
                leverHingeJoint.useSpring = true;
                JointSpring leverSpring = leverHingeJoint.spring;
                leverSpring.spring = springStrength;
                leverSpring.damper = springDamper;
                leverSpring.targetPosition = minAngle;
                leverHingeJoint.spring = leverSpring;
            }
            else
            {
                // If useSpring is set, the hingeJoint was manually added - respect its settings
                springStrength = leverHingeJoint.spring.spring;
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

        protected override void InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            base.InteractableObjectGrabbed(sender, e);
            isGrabbed = true;
        }

        protected override void InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            base.InteractableObjectUngrabbed(sender, e);
            isGrabbed = false;
        }

        protected virtual float GetSpringTarget(bool towardZero)
        {
            return (towardZero ? minAngle : maxAngle);
        }

        /// <summary>
        /// Check which direction the lever needs to be pushed in and
        /// switch spring direction as necessary
        /// </summary>
        protected virtual void ApplySpringForce()
        {
            leverHingeJoint.useSpring = (alwaysActive || !isGrabbed);

            if (leverHingeJoint.useSpring)
            {
                // get normalized value
                bool towardZero = (snapToNearestLimit ? (GetNormalizedValue() <= 50) : true);
                if (towardZero != wasTowardZero)
                {
                    JointSpring leverSpring = leverHingeJoint.spring;
                    leverSpring.targetPosition = GetSpringTarget(towardZero);
                    leverHingeJoint.spring = leverSpring;
                    wasTowardZero = towardZero;
                }
            }
        }
    }
}