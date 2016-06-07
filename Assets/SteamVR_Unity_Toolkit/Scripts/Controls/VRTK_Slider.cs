namespace VRTK {
    using System;
    using UnityEngine;

    public class VRTK_Slider : VRTK_Control {
        public enum Direction {
            autodetect, x, y, z
        }

        public Direction direction = Direction.autodetect;

        public float min = 0f;
        public float max = 100f;
        public float stepSize = 0.1f;

        public Vector3 minPoint;
        public Vector3 maxPoint;

        private static float MAX_AUTODETECT_SLIDER_LENGTH = 30; // multiple of the slider width

        private Direction finalDirection;
        private Vector3 finalMinPoint;
        private Vector3 finalMaxPoint;
        private Rigidbody rb;
        private VRTK_InteractableObject io;

        protected override void initRequiredComponents() {
            initRigidBody();
            initInteractable();
        }

        protected override bool detectSetup() {
            finalDirection = direction;
            if (direction == Direction.autodetect) {
                finalDirection = detectDirection();
                if (finalDirection == Direction.autodetect) return false;
            } else {
                // convert local positions of min/max to world coordinates
                Vector3 curPos = transform.localPosition;
                transform.localPosition = minPoint;
                finalMinPoint = transform.position;
                transform.localPosition = maxPoint;
                finalMaxPoint = transform.position;
                transform.localPosition = curPos;
            }
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
            rb.drag = 10; // otherwise slider will continue to move too far on its own
        }

        private void setConstraints(Direction direction) {
            if (!rb) return;

            rb.constraints = RigidbodyConstraints.FreezeAll;
            switch (direction) {
                case Direction.x:
                    rb.constraints -= RigidbodyConstraints.FreezePositionX;
                    break;
                case Direction.y:
                    rb.constraints -= RigidbodyConstraints.FreezePositionY;
                    break;
                case Direction.z:
                    rb.constraints -= RigidbodyConstraints.FreezePositionZ;
                    break;

            }
        }

        private void initInteractable() {
            io = GetComponent<VRTK_InteractableObject>();
            if (io == null) {
                io = gameObject.AddComponent<VRTK_InteractableObject>();
            }
            io.isGrabbable = true;
            io.grabSnapType = VRTK_InteractableObject.GrabSnapType.Precision_Snap;
            io.grabAttachMechanic = VRTK_InteractableObject.GrabAttachType.Track_Object;
        }

        private Direction detectDirection() {
            Direction direction = Direction.autodetect;
            Bounds bounds = Utilities.getBounds(transform);

            // shoot rays from the center of the slider, this means the center should be inside the frame to work properly
            RaycastHit hitForward;
            RaycastHit hitBack;
            RaycastHit hitLeft;
            RaycastHit hitRight;
            RaycastHit hitUp;
            RaycastHit hitDown;
            Physics.Raycast(bounds.center, Vector3.forward, out hitForward, bounds.extents.z * MAX_AUTODETECT_SLIDER_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.back, out hitBack, bounds.extents.z * MAX_AUTODETECT_SLIDER_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.left, out hitLeft, bounds.extents.x * MAX_AUTODETECT_SLIDER_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.right, out hitRight, bounds.extents.x * MAX_AUTODETECT_SLIDER_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.up, out hitUp, bounds.extents.y * MAX_AUTODETECT_SLIDER_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.down, out hitDown, bounds.extents.y * MAX_AUTODETECT_SLIDER_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);

            // shortest valid ray pair identifies first axis
            float lengthX = (hitLeft.collider != null && hitRight.collider != null) ? hitLeft.distance + hitRight.distance : float.MaxValue;
            float lengthY = (hitUp.collider != null && hitDown.collider != null) ? hitUp.distance + hitDown.distance : float.MaxValue;
            float lengthZ = (hitForward.collider != null && hitBack.collider != null) ? hitForward.distance + hitBack.distance : float.MaxValue;

            float extents = 0;
            if (lengthX < lengthY & lengthX < lengthZ) {
                if (lengthY < lengthZ) {
                    direction = Direction.y;
                    minPoint = hitUp.point;
                    maxPoint = hitDown.point;
                    extents = bounds.extents.y;
                } else if (lengthZ < lengthY) {
                    direction = Direction.z;
                    minPoint = hitBack.point;
                    maxPoint = hitForward.point;
                    extents = bounds.extents.z;
                }
            }
            if (lengthY < lengthX & lengthY < lengthZ) {
                if (lengthX < lengthZ) {
                    direction = Direction.x;
                    minPoint = hitLeft.point;
                    maxPoint = hitRight.point;
                    extents = bounds.extents.x;
                } else if (lengthZ < lengthX) {
                    direction = Direction.z;
                    minPoint = hitBack.point;
                    maxPoint = hitForward.point;
                    extents = bounds.extents.z;
                }
            }
            if (lengthZ < lengthX & lengthZ < lengthY) {
                if (lengthX < lengthY) {
                    direction = Direction.x;
                    minPoint = hitLeft.point;
                    maxPoint = hitRight.point;
                    extents = bounds.extents.x;
                } else if (lengthY < lengthX) {
                    direction = Direction.y;
                    minPoint = hitUp.point;
                    maxPoint = hitDown.point;
                    extents = bounds.extents.y;
                }
            }

            if (direction != Direction.autodetect) {
                // subtract width of slider to reach all values
                minPoint = minPoint + (maxPoint - minPoint).normalized * extents;
                maxPoint = maxPoint - (maxPoint - minPoint).normalized * extents;

                finalMinPoint = minPoint;
                finalMaxPoint = maxPoint;
            } else {
                finalMinPoint = transform.position;
                finalMaxPoint = transform.position;
            }

            return direction;
        }

        protected override void handleUpdate() {
            ensureSliderInRange();

            value = calculateValue();
            snapToValue(value);
        }

        private void ensureSliderInRange() {
            switch (direction) {
                case Direction.x:
                    if (transform.localPosition.x > maxPoint.x) {
                        transform.localPosition = maxPoint;
                    } else if (transform.localPosition.x < minPoint.x) {
                        transform.localPosition = minPoint;
                    }
                    break;
                case Direction.y:
                    if (transform.localPosition.y > maxPoint.y) {
                        transform.localPosition = maxPoint;
                    } else if (transform.localPosition.y < minPoint.y) {
                        transform.localPosition = minPoint;
                    }
                    break;
                case Direction.z:
                    if (transform.localPosition.z > maxPoint.z) {
                        transform.localPosition = maxPoint;
                    } else if (transform.localPosition.z < minPoint.z) {
                        transform.localPosition = minPoint;
                    }
                    break;
            }
        }

        private float calculateValue() {
            Vector3 center = (direction == Direction.autodetect) ? Utilities.getBounds(transform).center : transform.position;
            float dist1 = Vector3.Distance(finalMinPoint, center);
            float dist2 = Vector3.Distance(center, finalMaxPoint);

            return Mathf.Round((min + Mathf.Clamp01(dist1 / (dist1 + dist2)) * (max - min)) / stepSize) * stepSize;
        }

        private void snapToValue(float value) {
            transform.position = finalMinPoint + ((value - min) / (max - min)) * (finalMaxPoint - finalMinPoint).normalized * Vector3.Distance(finalMinPoint, finalMaxPoint);
        }

        public override void OnDrawGizmos() {
            base.OnDrawGizmos();

            // axis and min/max
            Vector3 center = (direction == Direction.autodetect) ? bounds.center : transform.position;
            Gizmos.DrawLine(center, finalMinPoint);
            Gizmos.DrawLine(center, finalMaxPoint);
        }
    }
}