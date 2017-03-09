// Wheel|Controls3D|100062
namespace VRTK
{
    using UnityEngine;
    using GrabAttachMechanics;

    /// <summary>
    /// Attaching the script to a game object will allow the user to interact with it as if it were a spinnable wheel.
    /// </summary>
    /// <remarks>
    /// The script will instantiate the required Rigidbody and Interactable components automatically in case they do not exist yet.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/025_Controls_Overview` has a collection of wheels that can be rotated by grabbing with the controller and then rotating the controller in the desired direction.
    /// </example>
    public class VRTK_Wheel : VRTK_Control
    {
        public enum GrabTypes
        {
            TrackObject,
            RotatorTrack
        }

        [Tooltip("An optional game object to which the wheel will be connected. If the game object moves the wheel will follow along.")]
        public GameObject connectedTo;
        [Tooltip("The grab attach mechanic to use. Track Object allows for rotations of the controller, Rotator Track allows for grabbing the wheel and spinning it.")]
        public GrabTypes grabType = GrabTypes.TrackObject;
        [Tooltip("The maximum distance the grabbing controller is away from the wheel before it is automatically released.")]
        public float detatchDistance = 0.5f;
        [Tooltip("The minimum value the wheel can be set to.")]
        public float minimumValue = 0f;
        [Tooltip("The maximum value the wheel can be set to.")]
        public float maximumValue = 10f;
        [Tooltip("The increments in which values can change.")]
        public float stepSize = 1f;
        [Tooltip("If this is checked then when the wheel is released, it will snap to the step rotation.")]
        public bool snapToStep = false;
        [Tooltip("The amount of friction the wheel will have when it is grabbed.")]
        public float grabbedFriction = 25f;
        [Tooltip("The amount of friction the wheel will have when it is released.")]
        public float releasedFriction = 10f;
        [Range(0f, 359f)]
        [Tooltip("The maximum angle the wheel has to be turned to reach it's maximum value.")]
        public float maxAngle = 359f;
        [Tooltip("If this is checked then the wheel cannot be turned beyond the minimum and maximum value.")]
        public bool lockAtLimits = false;

        protected float angularVelocityLimit = 150f;
        protected float springStrengthValue = 150f;
        protected float springDamperValue = 5f;
        protected Quaternion initialLocalRotation;
        protected Rigidbody wheelRigidbody;
        protected HingeJoint wheelHinge;
        protected bool wheelHingeCreated = false;
        protected bool initialValueCalculated = false;
        protected float springAngle;

        protected override void InitRequiredComponents()
        {
            initialLocalRotation = transform.localRotation;
            InitWheel();
        }

        protected override bool DetectSetup()
        {
            if (wheelHingeCreated)
            {
                wheelHinge.anchor = Vector3.up;
                wheelHinge.axis = Vector3.up;

                if (connectedTo)
                {
                    wheelHinge.connectedBody = connectedTo.GetComponent<Rigidbody>();
                }
            }

            return true;
        }

        protected override ControlValueRange RegisterValueRange()
        {
            return new ControlValueRange()
            {
                controlMin = minimumValue,
                controlMax = maximumValue
            };
        }

        protected override void HandleUpdate()
        {
            CalculateValue();
            if (lockAtLimits && !initialValueCalculated)
            {
                transform.localRotation = initialLocalRotation;
                initialValueCalculated = true;
            }
        }

        protected virtual void InitWheel()
        {
            SetupRigidbody();
            SetupHinge();
            SetupInteractableObject();
        }

        protected virtual void SetupRigidbody()
        {
            wheelRigidbody = GetComponent<Rigidbody>();
            if (wheelRigidbody == null)
            {
                wheelRigidbody = gameObject.AddComponent<Rigidbody>();
                wheelRigidbody.angularDrag = releasedFriction;
            }
            wheelRigidbody.isKinematic = false;
            wheelRigidbody.useGravity = false;

            if (connectedTo)
            {
                Rigidbody connectedToRigidbody = connectedTo.GetComponent<Rigidbody>();
                if (connectedToRigidbody == null)
                {
                    connectedToRigidbody = connectedTo.AddComponent<Rigidbody>();
                    connectedToRigidbody.useGravity = false;
                    connectedToRigidbody.isKinematic = true;
                }
            }
        }

        protected virtual void SetupHinge()
        {
            wheelHinge = GetComponent<HingeJoint>();
            if (wheelHinge == null)
            {
                wheelHinge = gameObject.AddComponent<HingeJoint>();
                wheelHingeCreated = true;
            }

            SetupHingeRestrictions();
        }

        protected virtual void SetupHingeRestrictions()
        {
            var minJointLimit = 0f;
            var maxJointLimit = maxAngle;
            var limitOffset = maxAngle - 180f;

            if (limitOffset > 0f)
            {
                minJointLimit -= limitOffset;
                maxJointLimit = 180f;
            }

            if (lockAtLimits)
            {
                wheelHinge.useLimits = true;
                JointLimits wheelLimits = new JointLimits();
                wheelLimits.min = minJointLimit;
                wheelLimits.max = maxJointLimit;
                wheelHinge.limits = wheelLimits;
                Vector3 adjustedLimitsAngle = transform.localEulerAngles;

                switch (Mathf.RoundToInt(initialLocalRotation.eulerAngles.z))
                {
                    case 0:
                        adjustedLimitsAngle = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y - minJointLimit, transform.localEulerAngles.z);
                        break;
                    case 90:
                        adjustedLimitsAngle = new Vector3(transform.localEulerAngles.x + minJointLimit, transform.localEulerAngles.y, transform.localEulerAngles.z);
                        break;
                    case 180:
                        adjustedLimitsAngle = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + minJointLimit, transform.localEulerAngles.z);
                        break;
                }
                transform.localEulerAngles = adjustedLimitsAngle;
                initialValueCalculated = false;
            }
        }

        protected virtual void ConfigureHingeSpring()
        {
            JointSpring snapSpring = new JointSpring();
            snapSpring.spring = springStrengthValue;
            snapSpring.damper = springDamperValue;
            snapSpring.targetPosition = springAngle + wheelHinge.limits.min;
            wheelHinge.spring = snapSpring;
        }

        protected virtual void SetupInteractableObject()
        {
            VRTK_InteractableObject wheelInteractableObject = GetComponent<VRTK_InteractableObject>();
            if (wheelInteractableObject == null)
            {
                wheelInteractableObject = gameObject.AddComponent<VRTK_InteractableObject>();
            }
            wheelInteractableObject.isGrabbable = true;

            VRTK_TrackObjectGrabAttach attachMechanic;

            if (grabType == GrabTypes.TrackObject)
            {
                attachMechanic = gameObject.AddComponent<VRTK_TrackObjectGrabAttach>();
                if (lockAtLimits)
                {
                    attachMechanic.velocityLimit = 0f;
                    attachMechanic.angularVelocityLimit = angularVelocityLimit;
                }
            }
            else
            {
                attachMechanic = gameObject.AddComponent<VRTK_RotatorTrackGrabAttach>();
            }

            attachMechanic.precisionGrab = true;
            attachMechanic.detachDistance = detatchDistance;

            wheelInteractableObject.grabAttachMechanicScript = attachMechanic;

            wheelInteractableObject.secondaryGrabActionScript = gameObject.AddComponent<SecondaryControllerGrabActions.VRTK_SwapControllerGrabAction>();
            wheelInteractableObject.stayGrabbedOnTeleport = false;

            wheelInteractableObject.InteractableObjectGrabbed += WheelInteractableObjectGrabbed;
            wheelInteractableObject.InteractableObjectUngrabbed += WheelInteractableObjectUngrabbed;
        }

        protected virtual void WheelInteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            wheelRigidbody.angularDrag = grabbedFriction;
            wheelHinge.useSpring = false;
        }

        protected virtual void WheelInteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            wheelRigidbody.angularDrag = releasedFriction;
            if (snapToStep)
            {
                wheelHinge.useSpring = true;
                ConfigureHingeSpring();
            }
        }

        protected virtual void CalculateValue()
        {
            ControlValueRange controlValueRange = RegisterValueRange();
            float angle;
            Vector3 axis;

            Quaternion rotationDelta = transform.localRotation * Quaternion.Inverse(initialLocalRotation);
            rotationDelta.ToAngleAxis(out angle, out axis);

            float calculatedValue = Mathf.Round((controlValueRange.controlMin + Mathf.Clamp01(angle / maxAngle) * (controlValueRange.controlMax - controlValueRange.controlMin)) / stepSize) * stepSize;

            float flatValue = calculatedValue - controlValueRange.controlMin;
            float controlRange = controlValueRange.controlMax - controlValueRange.controlMin;
            springAngle = (flatValue / controlRange) * maxAngle;

            value = calculatedValue;
        }
    }
}