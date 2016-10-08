// Lever|Controls3D|0070
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
    public class VRTK_Lever : VRTK_Control
    {
        public enum LeverDirection
        {
            x, y, z
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

        protected HingeJoint hj;

        private Rigidbody rb;
        private VRTK_InteractableObject io;
        private bool hjCreated = false;

        protected override void InitRequiredComponents()
        {
            if (GetComponentInChildren<Collider>() == null)
            {
                Utilities.CreateColliders(gameObject);
            }

            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rb.angularDrag = 30; // otherwise lever will continue to move too far on its own
            }
            rb.isKinematic = false;
            rb.useGravity = false;

            io = GetComponent<VRTK_InteractableObject>();
            if (io == null)
            {
                io = gameObject.AddComponent<VRTK_InteractableObject>();
            }
            io.isGrabbable = true;
            io.precisionSnap = true;
            io.stayGrabbedOnTeleport = false;
            io.grabAttachMechanic = VRTK_InteractableObject.GrabAttachType.Rotator_Track;

            hj = GetComponent<HingeJoint>();
            if (hj == null)
            {
                hj = gameObject.AddComponent<HingeJoint>();
                hjCreated = true;
            }

            if (connectedTo)
            {
                Rigidbody rb2 = connectedTo.GetComponent<Rigidbody>();
                if (rb2 == null)
                {
                    rb2 = connectedTo.AddComponent<Rigidbody>();
                }
                rb2.useGravity = false;
            }
        }

        protected override bool DetectSetup()
        {
            if (hjCreated)
            {
                Bounds bounds = Utilities.GetBounds(transform, transform);
                switch (direction)
                {
                    case LeverDirection.x:
                        hj.anchor = (bounds.extents.y > bounds.extents.z) ? new Vector3(0, bounds.extents.y / transform.lossyScale.y, 0) : new Vector3(0, 0, bounds.extents.z / transform.lossyScale.z);
                        break;
                    case LeverDirection.y:
                        hj.axis = new Vector3(0, 1, 0);
                        hj.anchor = (bounds.extents.x > bounds.extents.z) ? new Vector3(bounds.extents.x / transform.lossyScale.x, 0, 0) : new Vector3(0, 0, bounds.extents.z / transform.lossyScale.z);
                        break;
                    case LeverDirection.z:
                        hj.axis = new Vector3(0, 0, 1);
                        hj.anchor = (bounds.extents.y > bounds.extents.x) ? new Vector3(0, bounds.extents.y / transform.lossyScale.y, 0) : new Vector3(bounds.extents.x / transform.lossyScale.x, 0);
                        break;
                }
                hj.anchor *= -1; // subdirection detection not yet implemented
            }

            if (hj)
            {
                hj.useLimits = true;
                JointLimits limits = hj.limits;
                limits.min = minAngle;
                limits.max = maxAngle;
                hj.limits = limits;

                if (connectedTo)
                {
                    hj.connectedBody = connectedTo.GetComponent<Rigidbody>();
                }
            }

            return true;
        }

        protected override ControlValueRange RegisterValueRange()
        {
            return new ControlValueRange() { controlMin = minAngle, controlMax = maxAngle };
        }

        protected override void HandleUpdate()
        {
            value = CalculateValue();
            SnapToValue(value);
        }

        private float CalculateValue()
        {
            return Mathf.Round((hj.angle) / stepSize) * stepSize;
        }

        private void SnapToValue(float value)
        {
            float angle = ((value - minAngle) / (maxAngle - minAngle)) * (hj.limits.max - hj.limits.min);

            // TODO: there is no direct setter, one recommendation by Unity staff is to "abuse" min/max which seems the most reliable but not working so far
            JointLimits oldLimits = hj.limits;
            JointLimits tempLimits = hj.limits;
            tempLimits.min = angle;
            tempLimits.max = angle;
            hj.limits = tempLimits;
            hj.limits = oldLimits;
        }
    }
}