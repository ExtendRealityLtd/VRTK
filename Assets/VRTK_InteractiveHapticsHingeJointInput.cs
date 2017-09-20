// Interactive Haptics Input|Interactions|30103
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Interactive Haptics script is attached on the same GameObject as an Interactable Object script and provides customizable haptic feedback curves for more detailed interactions.
    /// </summary>
    public class VRTK_InteractiveHapticsHingeJointInput : VRTK_InteractiveHapticsInput
    {
        /// <summary>
        /// An optional HingeJoint to use, will use HingeJoint on this game object if not specified.
        /// </summary>
        public HingeJoint joint;
        
        private float lastAngle;

        protected virtual void OnEnable()
        {
            joint = (joint != null ? joint : GetComponent<HingeJoint>());

            if (joint == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_InteractiveHaptics", "HingeJoint", "the same"));
            }
        }
        
        protected virtual void FixedUpdate()
        {
            if (lastAngle != joint.angle)
            {
                OnInputProvided(GetNormalizedAngle(joint.angle));

                lastAngle = joint.angle;
            }
        }        

        protected virtual float GetNormalizedAngle(float angle)
        {
            float normalizedValue;

            if (joint.useLimits)
            {
                normalizedValue = angle / (Mathf.Abs(joint.limits.max) + Mathf.Abs(joint.limits.min));
            }
            else
            {
                normalizedValue = angle / 180f;
            }
            
            return Mathf.Abs(normalizedValue);
        }        
    }
}