namespace VRTK
{
    using UnityEngine;

    public class VRTK_Knob : VRTK_Control
    {
        public enum KnobDirection
        {
            x, y, z // TODO: autodetect not yet done, it's a bit more difficult to get it right
        }

        public KnobDirection direction = KnobDirection.x;
        public float min = 0f;
        public float max = 100f;
        public float stepSize = 1f;

        private static float MAX_AUTODETECT_KNOB_WIDTH = 3; // multiple of the knob width

        private KnobDirection finalDirection;
        private Quaternion initialRotation;
        private Vector3 initialLocalRotation;
        private Rigidbody rb;
        private VRTK_InteractableObject io;

        protected override void InitRequiredComponents()
        {
            initialRotation = transform.rotation;
            initialLocalRotation = transform.localRotation.eulerAngles;
            InitRigidBody();
            InitInteractable();
        }

        protected override bool DetectSetup()
        {
            finalDirection = direction;
            SetConstraints(finalDirection);

            return true;
        }

        protected override ControlValueRange RegisterValueRange()
        {
            return new ControlValueRange() { controlMin = min, controlMax = max };
        }

        protected override void HandleUpdate()
        {
            value = CalculateValue();
        }

        private void InitRigidBody()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.angularDrag = 10; // otherwise knob will continue to move too far on its own
        }

        private void SetConstraints(KnobDirection direction)
        {
            if (!rb) return;

            rb.constraints = RigidbodyConstraints.FreezeAll;
            switch (direction)
            {
                case KnobDirection.x:
                    rb.constraints -= RigidbodyConstraints.FreezeRotationX;
                    break;
                case KnobDirection.y:
                    rb.constraints -= RigidbodyConstraints.FreezeRotationY;
                    break;
                case KnobDirection.z:
                    rb.constraints -= RigidbodyConstraints.FreezeRotationZ;
                    break;
            }
        }

        private void InitInteractable()
        {
            io = GetComponent<VRTK_InteractableObject>();
            if (io == null)
            {
                io = gameObject.AddComponent<VRTK_InteractableObject>();
            }
            io.isGrabbable = true;
            io.precisionSnap = true;
            io.grabAttachMechanic = VRTK_InteractableObject.GrabAttachType.Track_Object;
        }

        private KnobDirection DetectDirection()
        {
            KnobDirection direction = KnobDirection.x;
            Bounds bounds = Utilities.GetBounds(transform);

            // shoot rays in all directions to learn about surroundings
            RaycastHit hitForward;
            RaycastHit hitBack;
            RaycastHit hitLeft;
            RaycastHit hitRight;
            RaycastHit hitUp;
            RaycastHit hitDown;
            Physics.Raycast(bounds.center, Vector3.forward, out hitForward, bounds.extents.z * MAX_AUTODETECT_KNOB_WIDTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.back, out hitBack, bounds.extents.z * MAX_AUTODETECT_KNOB_WIDTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.left, out hitLeft, bounds.extents.x * MAX_AUTODETECT_KNOB_WIDTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.right, out hitRight, bounds.extents.x * MAX_AUTODETECT_KNOB_WIDTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.up, out hitUp, bounds.extents.y * MAX_AUTODETECT_KNOB_WIDTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.down, out hitDown, bounds.extents.y * MAX_AUTODETECT_KNOB_WIDTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);

            // shortest valid ray wins
            float lengthX = (hitRight.collider != null) ? hitRight.distance : float.MaxValue;
            float lengthY = (hitDown.collider != null) ? hitDown.distance : float.MaxValue;
            float lengthZ = (hitBack.collider != null) ? hitBack.distance : float.MaxValue;
            float lengthNegX = (hitLeft.collider != null) ? hitLeft.distance : float.MaxValue;
            float lengthNegY = (hitUp.collider != null) ? hitUp.distance : float.MaxValue;
            float lengthNegZ = (hitForward.collider != null) ? hitForward.distance : float.MaxValue;

            // TODO: not yet the right decision strategy, works only partially
            if (Utilities.IsLowest(lengthX, new float[] { lengthY, lengthZ, lengthNegX, lengthNegY, lengthNegZ }))
            {
                direction = KnobDirection.z;
            }
            else if (Utilities.IsLowest(lengthY, new float[] { lengthX, lengthZ, lengthNegX, lengthNegY, lengthNegZ }))
            {
                direction = KnobDirection.y;
            }
            else if (Utilities.IsLowest(lengthZ, new float[] { lengthX, lengthY, lengthNegX, lengthNegY, lengthNegZ }))
            {
                direction = KnobDirection.x;
            }
            else if (Utilities.IsLowest(lengthNegX, new float[] { lengthX, lengthY, lengthZ, lengthNegY, lengthNegZ }))
            {
                direction = KnobDirection.z;
            }
            else if (Utilities.IsLowest(lengthNegY, new float[] { lengthX, lengthY, lengthZ, lengthNegX, lengthNegZ }))
            {
                direction = KnobDirection.y;
            }
            else if (Utilities.IsLowest(lengthNegZ, new float[] { lengthX, lengthY, lengthZ, lengthNegX, lengthNegY }))
            {
                direction = KnobDirection.x;
            }

            return direction;
        }

        private float CalculateValue()
        {
            float angle = 0;
            switch (finalDirection)
            {
                case KnobDirection.x:
                    angle = transform.localRotation.eulerAngles.x - initialLocalRotation.x;
                    break;
                case KnobDirection.y:
                    angle = transform.localRotation.eulerAngles.y - initialLocalRotation.y;
                    break;
                case KnobDirection.z:
                    angle = transform.localRotation.eulerAngles.z - initialLocalRotation.z;
                    break;
            }
            angle = Mathf.Round(angle * 1000f) / 1000f; // not rounding will produce slight offsets in 4th digit that mess up initial value

            // Quaternion.angle will calculate shortest route and only go to 180
            float value = 0;
            if (angle > 0 && angle <= 180)
            {
                value = 360 - Quaternion.Angle(initialRotation, transform.rotation);
            }
            else
            {
                value = Quaternion.Angle(initialRotation, transform.rotation);
            }

            // adjust to value scale
            value = Mathf.Round((min + Mathf.Clamp01(value / 360f) * (max - min)) / stepSize) * stepSize;
            if (min > max && angle != 0)
            {
                value = (max + min) - value;
            }

            return value;
        }
    }
}