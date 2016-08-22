namespace VRTK
{
    using UnityEngine;

    public class VRTK_Lever : VRTK_Control
    {
        public enum LeverDirection
        {
            x, y, z
        }

        public LeverDirection direction = LeverDirection.y;
        public float minAngle = 0f;
        public float maxAngle = 130f;

        private float stepSize = 1f;

        private Rigidbody rb;
        private VRTK_InteractableObject io;
        private HingeJoint hj;
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
            io.grabAttachMechanic = VRTK_InteractableObject.GrabAttachType.Track_Object;

            hj = GetComponent<HingeJoint>();
            if (hj == null)
            {
                hj = gameObject.AddComponent<HingeJoint>();
                hjCreated = true;
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