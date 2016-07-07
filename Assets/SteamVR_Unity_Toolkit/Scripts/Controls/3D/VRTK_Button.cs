namespace VRTK
{
    using UnityEngine;

    public class VRTK_Button : VRTK_Control
    {
        public enum Direction
        {
            autodetect, x, y, z, negX, negY, negZ
        }

        public Direction direction = Direction.autodetect;
        public float activationDistance = 1.0f;
        public float buttonStrength = 5.0f;

        public delegate void PushAction();
        public event PushAction OnPushed;

        private static float MAX_AUTODETECT_ACTIVATION_LENGTH = 4; // full hight of button

        private Direction finalDirection;
        private Vector3 initialPosition;
        private Vector3 initialLocalPosition;
        private Vector3 activationPoint;

        private Rigidbody rb;
        private ConstantForce cf;

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            // visualize activation distance
            Gizmos.DrawLine(bounds.center, activationPoint);
        }

        protected override void InitRequiredComponents()
        {
            initialPosition = transform.position;
            initialLocalPosition = transform.localPosition;

            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = false;
            rb.useGravity = false;

            cf = GetComponent<ConstantForce>();
            if (cf == null)
            {
                cf = gameObject.AddComponent<ConstantForce>();
            }
            cf.enabled = false;
        }

        protected override bool DetectSetup()
        {
            finalDirection = (direction == Direction.autodetect) ? DetectDirection() : direction;
            if (finalDirection == Direction.autodetect)
            {
                activationPoint = transform.position;
                return false;
            }

            if (rb)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                switch (finalDirection)
                {
                    case Direction.x:
                    case Direction.negX:
                        rb.constraints -= RigidbodyConstraints.FreezePositionX;
                        break;
                    case Direction.y:
                    case Direction.negY:
                        rb.constraints -= RigidbodyConstraints.FreezePositionY;
                        break;
                    case Direction.z:
                    case Direction.negZ:
                        rb.constraints -= RigidbodyConstraints.FreezePositionZ;
                        break;
                }
            }

            if (cf)
            {
                cf.force = GetForceVector();
            }
            activationPoint = CalculateActivationPoint();

            return true;
        }

        protected override void HandleUpdate()
        {
            // ensure button does not move beyond original position
            if (!IsValidPosition())
            {
                transform.localPosition = initialLocalPosition;
            }

            // trigger events
            float oldState = value;
            if (ReachedActivationDistance())
            {
                if (oldState == 0)
                {
                    value = 1;
                    if (OnPushed != null)
                    {
                        OnPushed();
                    }
                }
            }
            else
            {
                value = 0;
            }

            // activate pushback
            cf.enabled = !transform.localPosition.Equals(initialLocalPosition);
        }

        private Direction DetectDirection()
        {
            Direction direction = Direction.autodetect;
            Bounds bounds = Utilities.GetBounds(transform);

            // shoot rays from the center of the button to learn about surroundings
            RaycastHit hitForward;
            RaycastHit hitBack;
            RaycastHit hitLeft;
            RaycastHit hitRight;
            RaycastHit hitUp;
            RaycastHit hitDown;
            Physics.Raycast(bounds.center, Vector3.forward, out hitForward, bounds.extents.z * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.back, out hitBack, bounds.extents.z * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.left, out hitLeft, bounds.extents.x * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.right, out hitRight, bounds.extents.x * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.up, out hitUp, bounds.extents.y * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.down, out hitDown, bounds.extents.y * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);

            // shortest valid ray wins
            float lengthX = (hitRight.collider != null) ? hitRight.distance : float.MaxValue;
            float lengthY = (hitDown.collider != null) ? hitDown.distance : float.MaxValue;
            float lengthZ = (hitBack.collider != null) ? hitBack.distance : float.MaxValue;
            float lengthNegX = (hitLeft.collider != null) ? hitLeft.distance : float.MaxValue;
            float lengthNegY = (hitUp.collider != null) ? hitUp.distance : float.MaxValue;
            float lengthNegZ = (hitForward.collider != null) ? hitForward.distance : float.MaxValue;

            float extents = 0;
            Vector3 hitPoint = Vector3.zero;
            if (Utilities.IsLowest(lengthX, new float[] { lengthY, lengthZ, lengthNegX, lengthNegY, lengthNegZ }))
            {
                direction = Direction.negX;
                hitPoint = hitRight.point;
                extents = bounds.extents.x;
            }
            else if (Utilities.IsLowest(lengthY, new float[] { lengthX, lengthZ, lengthNegX, lengthNegY, lengthNegZ }))
            {
                direction = Direction.y;
                hitPoint = hitDown.point;
                extents = bounds.extents.y;
            }
            else if (Utilities.IsLowest(lengthZ, new float[] { lengthX, lengthY, lengthNegX, lengthNegY, lengthNegZ }))
            {
                direction = Direction.z;
                hitPoint = hitBack.point;
                extents = bounds.extents.z;
            }
            else if (Utilities.IsLowest(lengthNegX, new float[] { lengthX, lengthY, lengthZ, lengthNegY, lengthNegZ }))
            {
                direction = Direction.x;
                hitPoint = hitLeft.point;
                extents = bounds.extents.x;
            }
            else if (Utilities.IsLowest(lengthNegY, new float[] { lengthX, lengthY, lengthZ, lengthNegX, lengthNegZ }))
            {
                direction = Direction.negY;
                hitPoint = hitUp.point;
                extents = bounds.extents.y;
            }
            else if (Utilities.IsLowest(lengthNegZ, new float[] { lengthX, lengthY, lengthZ, lengthNegX, lengthNegY }))
            {
                direction = Direction.negZ;
                hitPoint = hitForward.point;
                extents = bounds.extents.z;
            }

            // determin activation distance
            activationDistance = (Vector3.Distance(hitPoint, bounds.center) - extents) * 0.95f;

            if (direction == Direction.autodetect || activationDistance < 0)
            {
                // auto-detection was not possible or colliding with object already
                direction = Direction.autodetect;
                activationDistance = 0;
            }

            return direction;
        }

        private Vector3 CalculateActivationPoint()
        {
            Bounds bounds = Utilities.GetBounds(transform, transform);

            Vector3 buttonDirection = Vector3.zero;
            float extents = 0;
            switch (finalDirection)
            {
                case Direction.x:
                    buttonDirection = transform.right * -1;
                    extents = bounds.extents.x;
                    break;
                case Direction.negX:
                    buttonDirection = transform.right;
                    extents = bounds.extents.x;
                    break;
                case Direction.y:
                    buttonDirection = transform.up * -1;
                    extents = bounds.extents.y;
                    break;
                case Direction.negY:
                    buttonDirection = transform.up;
                    extents = bounds.extents.y;
                    break;
                case Direction.z:
                    buttonDirection = transform.forward * -1;
                    extents = bounds.extents.z;
                    break;
                case Direction.negZ:
                    buttonDirection = transform.forward;
                    extents = bounds.extents.z;
                    break;
            }

            // subtract width of button
            return bounds.center + buttonDirection * (extents + activationDistance);
        }

        private bool ReachedActivationDistance()
        {
            if (direction == Direction.autodetect)
            {
                // distance is in world coordinates in that case, not local
                return Vector3.Distance(transform.position, initialPosition) >= activationDistance;
            }
            else
            {
                return Vector3.Distance(transform.localPosition, initialLocalPosition) >= activationDistance;
            }
        }

        private bool IsValidPosition()
        {
            switch (finalDirection)
            {
                case Direction.x:
                    return transform.localPosition.x <= initialLocalPosition.x;
                case Direction.y:
                    return transform.localPosition.y <= initialLocalPosition.y;
                case Direction.z:
                    return transform.localPosition.z <= initialLocalPosition.z;
                case Direction.negX:
                    return transform.localPosition.x >= initialLocalPosition.x;
                case Direction.negY:
                    return transform.localPosition.y >= initialLocalPosition.y;
                case Direction.negZ:
                    return transform.localPosition.z >= initialLocalPosition.z;
                default:
                    return true;
            }
        }

        private Vector3 GetForceVector()
        {
            switch (finalDirection)
            {
                case Direction.x:
                    return new Vector3(buttonStrength, 0, 0);
                case Direction.y:
                    return new Vector3(0, buttonStrength, 0);
                case Direction.z:
                    return new Vector3(0, 0, buttonStrength);
                case Direction.negX:
                    return new Vector3(-buttonStrength, 0, 0);
                case Direction.negY:
                    return new Vector3(0, -buttonStrength, 0);
                case Direction.negZ:
                    return new Vector3(0, 0, -buttonStrength);
                default:
                    return new Vector3(0, 0, 0);
            }
        }
    }
}