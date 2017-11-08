// Lever|Controls3D|100070
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Attaching the script to a game object will allow the user to interact with it as if it were a lever. The direction can be freely set.
    /// </summary>
    /// <remarks>
    /// The script will instantiate the required Rigidbody, Interactable and HingeJoint components automatically in case they do not exist yet. The joint is very tricky to setup automatically though and will only work in straight forward cases. If there are any issues, then create the HingeJoint component manually and configure it as needed.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/025_Controls_Overview` has a couple of levers that can be grabbed and moved. One lever is horizontal and the other is vertical.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Controls/3D/VRTK_Lever")]
    [System.Obsolete("`VRTK.VRTK_Lever` has been deprecated and can be recreated with `VRTK.Controllables.PhysicsBased.VRTK_PhysicsRotator`. This script will be removed in a future version of VRTK.")]
    public class VRTK_Lever : VRTK_Control
    {
        /// <summary>
        /// The direction of the lever.
        /// </summary>
        public enum LeverDirection
        {
            /// <summary>
            /// The world x direction.
            /// </summary>
            x,
            /// <summary>
            /// The world y direction.
            /// </summary>
            y,
            /// <summary>
            /// The world z direction.
            /// </summary>
            z
        }

        [Tooltip("An optional game object to which the lever will be connected. If the game object moves the lever will follow along.")]
        public GameObject connectedTo;
        [Tooltip("The axis on which the lever should rotate. All other axis will be frozen.")]
        public LeverDirection direction = LeverDirection.y;
        [Tooltip("The minimum angle of the lever counted from its initial position.")]
        public float minAngle = 0f;
        [Tooltip("The maximum angle of the lever counted from its initial position.")]
        public float maxAngle = 130f;
        [Tooltip("The increments in which lever values can change.")]
        public float stepSize = 1f;
        [Tooltip("The amount of friction the lever will have whilst swinging when it is not grabbed.")]
        public float releasedFriction = 30f;
        [Tooltip("The amount of friction the lever will have whilst swinging when it is grabbed.")]
        public float grabbedFriction = 60f;

        protected HingeJoint leverHingeJoint;
        protected bool leverHingeJointCreated = false;
        protected Rigidbody leverRigidbody;

        protected override void InitRequiredComponents()
        {
            if (GetComponentInChildren<Collider>() == null)
            {
                VRTK_SharedMethods.CreateColliders(gameObject);
            }

            InitRigidbody();
            InitInteractableObject();
            InitHingeJoint();
        }

        protected override bool DetectSetup()
        {
            if (leverHingeJointCreated)
            {
                Bounds bounds = VRTK_SharedMethods.GetBounds(transform, transform);
                switch (direction)
                {
                    case LeverDirection.x:
                        leverHingeJoint.anchor = (bounds.extents.y > bounds.extents.z) ? new Vector3(0, bounds.extents.y / transform.lossyScale.y, 0) : new Vector3(0, 0, bounds.extents.z / transform.lossyScale.z);
                        break;
                    case LeverDirection.y:
                        leverHingeJoint.axis = new Vector3(0, 1, 0);
                        leverHingeJoint.anchor = (bounds.extents.x > bounds.extents.z) ? new Vector3(bounds.extents.x / transform.lossyScale.x, 0, 0) : new Vector3(0, 0, bounds.extents.z / transform.lossyScale.z);
                        break;
                    case LeverDirection.z:
                        leverHingeJoint.axis = new Vector3(0, 0, 1);
                        leverHingeJoint.anchor = (bounds.extents.y > bounds.extents.x) ? new Vector3(0, bounds.extents.y / transform.lossyScale.y, 0) : new Vector3(bounds.extents.x / transform.lossyScale.x, 0);
                        break;
                }
                leverHingeJoint.anchor *= -1; // subdirection detection not yet implemented
            }

            if (leverHingeJoint)
            {
                leverHingeJoint.useLimits = true;
                JointLimits leverJointLimits = leverHingeJoint.limits;
                leverJointLimits.min = minAngle;
                leverJointLimits.max = maxAngle;
                leverHingeJoint.limits = leverJointLimits;

                if (connectedTo)
                {
                    leverHingeJoint.connectedBody = connectedTo.GetComponent<Rigidbody>();
                }
            }

            return true;
        }

        protected override ControlValueRange RegisterValueRange()
        {
            return new ControlValueRange()
            {
                controlMin = minAngle,
                controlMax = maxAngle
            };
        }

        protected override void HandleUpdate()
        {
            value = CalculateValue();
            SnapToValue(value);
        }

        protected virtual void InitRigidbody()
        {
            leverRigidbody = GetComponent<Rigidbody>();
            if (leverRigidbody == null)
            {
                leverRigidbody = gameObject.AddComponent<Rigidbody>();
                leverRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                leverRigidbody.angularDrag = releasedFriction; // otherwise lever will continue to move too far on its own
            }
            leverRigidbody.isKinematic = false;
            leverRigidbody.useGravity = false;
        }

        protected virtual void InitInteractableObject()
        {
            VRTK_InteractableObject leverInteractableObject = GetComponent<VRTK_InteractableObject>();
            if (leverInteractableObject == null)
            {
                leverInteractableObject = gameObject.AddComponent<VRTK_InteractableObject>();
            }
            leverInteractableObject.isGrabbable = true;
            leverInteractableObject.grabAttachMechanicScript = gameObject.AddComponent<GrabAttachMechanics.VRTK_RotatorTrackGrabAttach>();
            leverInteractableObject.secondaryGrabActionScript = gameObject.AddComponent<SecondaryControllerGrabActions.VRTK_SwapControllerGrabAction>();
            leverInteractableObject.grabAttachMechanicScript.precisionGrab = true;
            leverInteractableObject.stayGrabbedOnTeleport = false;

            leverInteractableObject.InteractableObjectGrabbed += InteractableObjectGrabbed;
            leverInteractableObject.InteractableObjectUngrabbed += InteractableObjectUngrabbed;
        }

        protected virtual void InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            leverRigidbody.angularDrag = grabbedFriction;
        }

        protected virtual void InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            leverRigidbody.angularDrag = releasedFriction;
        }

        protected virtual void InitHingeJoint()
        {
            leverHingeJoint = GetComponent<HingeJoint>();
            if (leverHingeJoint == null)
            {
                leverHingeJoint = gameObject.AddComponent<HingeJoint>();
                leverHingeJointCreated = true;
            }

            if (connectedTo)
            {
                Rigidbody leverConnectedToRigidbody = connectedTo.GetComponent<Rigidbody>();
                if (leverConnectedToRigidbody == null)
                {
                    leverConnectedToRigidbody = connectedTo.AddComponent<Rigidbody>();
                }
                leverConnectedToRigidbody.useGravity = false;
            }
        }

        protected virtual float CalculateValue()
        {
            return Mathf.Round((leverHingeJoint.angle) / stepSize) * stepSize;
        }

        protected virtual void SnapToValue(float value)
        {
            float angle = ((value - minAngle) / (maxAngle - minAngle)) * (leverHingeJoint.limits.max - leverHingeJoint.limits.min);

            // TODO: there is no direct setter, one recommendation by Unity staff is to "abuse" min/max which seems the most reliable but not working so far
            JointLimits oldLimits = leverHingeJoint.limits;
            JointLimits tempLimits = leverHingeJoint.limits;
            tempLimits.min = angle;
            tempLimits.max = angle;
            leverHingeJoint.limits = tempLimits;
            leverHingeJoint.limits = oldLimits;
        }
    }
}