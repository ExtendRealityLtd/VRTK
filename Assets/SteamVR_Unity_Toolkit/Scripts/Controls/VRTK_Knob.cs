namespace VRTK {
    using UnityEngine;

    public class VRTK_Knob : VRTK_Control {
        public enum Direction {
            x, y, z // TODO: autodetect not yet done, it's a bit more difficult to get it right
        }

        public Direction direction = Direction.x;
        public float min = 0f;
        public float max = 100f;
        public float stepSize = 1f;

        private static float MAX_AUTODETECT_KNOB_WIDTH = 3; // multiple of the knob width

        private Direction finalDirection;
        private Quaternion initialRotation;
        private Vector3 initialLocalRotation;
        private Rigidbody rb;
        private VRTK_InteractableObject io;

        protected override void initRequiredComponents() {
            initialRotation = transform.rotation;
            initialLocalRotation = transform.localRotation.eulerAngles;
            initRigidBody();
            initInteractable();
        }

        protected override bool detectSetup() {
            finalDirection = direction;
            /*
            if (direction == Direction.autodetect) {
                finalDirection = detectDirection();
            } 
            */

            setConstraints(finalDirection);

            return true;
        }

        private void initRigidBody() {
            rb = GetComponent<Rigidbody>();
            if (rb == null) {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.angularDrag = 10; // otherwise knob will continue to move too far on its own
        }

        private void setConstraints(Direction direction) {
            if (!rb) return;

            rb.constraints = RigidbodyConstraints.FreezeAll;
            switch (direction) {
                case Direction.x:
                    rb.constraints -= RigidbodyConstraints.FreezeRotationX;
                    break;
                case Direction.y:
                    rb.constraints -= RigidbodyConstraints.FreezeRotationY;
                    break;
                case Direction.z:
                    rb.constraints -= RigidbodyConstraints.FreezeRotationZ;
                    break;

            }
        }

        private void initInteractable() {
            io = GetComponent<VRTK_InteractableObject>();
            if (io == null) {
                io = gameObject.AddComponent<VRTK_InteractableObject>();
            }
            io.isGrabbable = true;
            io.precisionSnap = true;
            io.grabAttachMechanic = VRTK_InteractableObject.GrabAttachType.Track_Object;
        }

        private Direction detectDirection() {
            Direction direction = Direction.x;
            Bounds bounds = Utilities.getBounds(transform);

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
            if (Utilities.isLowest(lengthX, new float[] { lengthY, lengthZ, lengthNegX, lengthNegY, lengthNegZ })) {
                direction = Direction.z;
            } else if (Utilities.isLowest(lengthY, new float[] { lengthX, lengthZ, lengthNegX, lengthNegY, lengthNegZ })) {
                direction = Direction.y;
            } else if (Utilities.isLowest(lengthZ, new float[] { lengthX, lengthY, lengthNegX, lengthNegY, lengthNegZ })) {
                direction = Direction.x;
            } else if (Utilities.isLowest(lengthNegX, new float[] { lengthX, lengthY, lengthZ, lengthNegY, lengthNegZ })) {
                direction = Direction.z;
            } else if (Utilities.isLowest(lengthNegY, new float[] { lengthX, lengthY, lengthZ, lengthNegX, lengthNegZ })) {
                direction = Direction.y;
            } else if (Utilities.isLowest(lengthNegZ, new float[] { lengthX, lengthY, lengthZ, lengthNegX, lengthNegY })) {
                direction = Direction.x;
            }

            return direction;
        }

        protected override void handleUpdate() {
            value = calculateValue();
            snapToValue(value);
        }

        private float calculateValue() {
            float angle = 0;
            switch (finalDirection) {
                case Direction.x:
                    angle = transform.localRotation.eulerAngles.x - initialLocalRotation.x;
                    break;
                case Direction.y:
                    angle = transform.localRotation.eulerAngles.y - initialLocalRotation.y;
                    break;
                case Direction.z:
                    angle = transform.localRotation.eulerAngles.z - initialLocalRotation.z;
                    break;
            }
            angle = Mathf.Round(angle * 1000f) / 1000f; // not rounding will produce slight offsets in 4th digit that mess up initial value

            // Quaternion.angle will calculate shortest route and only go to 180
            float value = 0;
            if (angle > 0 && angle <= 180) {
                value = 360 - Quaternion.Angle(initialRotation, transform.rotation);
            } else {
                value = Quaternion.Angle(initialRotation, transform.rotation);
            }

            // adjust to value scale
            value = Mathf.Round((min + Mathf.Clamp01(value / 360f) * (max - min)) / stepSize) * stepSize;
            if (min > max && angle != 0) {
                value = (max + min) - value;
            }

            return value;
        }

        private void snapToValue(float value) {
            // TODO: how can we safely snap to a certain rotation?
            // float angle = 360 - 360 * (value - min) / (max - min);
            // Debug.Log(angle);
            // transform.localEulerAngles = new Vector3(angle, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }
}