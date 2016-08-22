namespace VRTK
{
    using UnityEngine;

    public class VRTK_Drawer : VRTK_Control
    {
        public Direction direction = Direction.autodetect;

        public GameObject body;
        public GameObject handle;
        [Tooltip("An optional game object that is the parent of the content inside the drawer. If set all interactables will become managed so that they only react if the drawer is wide enough open.")]
        public GameObject content;
        [Tooltip("Will make the content invisible if the drawer is closed. This way players cannot peak into it by moving the camera.")]
        public bool hideContent = true;
        public bool snapping = false;

        private Rigidbody rb;
        private Rigidbody handleRb;
        private FixedJoint handleFj;
        private ConfigurableJoint cj;
        private VRTK_InteractableObject io;
        private ConstantForce cf;

        private Direction finalDirection;
        private float subDirection = 1; // positive or negative can be determined automatically since handle dictates that
        private float pullDistance = 0f;
        private Vector3 initialPosition;
        private bool cjCreated = false;
        private bool cfCreated = false;

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (!enabled || !setupSuccessful)
            {
                return;
            }

            // show opening direction
            Bounds handleBounds = Utilities.GetBounds(getHandle().transform, getHandle().transform);
            float length = handleBounds.extents.y * ((handle) ? 5f : 1f);
            Vector3 point = handleBounds.center;
            switch (finalDirection)
            {
                case Direction.x:
                    point -= transform.right.normalized * length * subDirection;
                    break;
                case Direction.y:
                    point -= transform.up.normalized * length * subDirection;
                    break;
                case Direction.z:
                    point -= transform.forward.normalized * length * subDirection;
                    break;
            }

            Gizmos.DrawLine(handleBounds.center, point);
            Gizmos.DrawSphere(point, length / 8f);
        }

        protected override void InitRequiredComponents()
        {
            initialPosition = transform.position;

            InitBody();
            InitHandle();

            SetContent(content, hideContent);
        }

        protected override bool DetectSetup()
        {
            finalDirection = (direction == Direction.autodetect) ? DetectDirection() : direction;
            if (finalDirection == Direction.autodetect)
            {
                return false;
            }

            // determin sub-direction depending on handle
            Bounds handleBounds = Utilities.GetBounds(getHandle().transform, transform);
            Bounds bodyBounds = Utilities.GetBounds(getBody().transform, transform);
            switch (finalDirection)
            {
                case Direction.x:
                    subDirection = (handleBounds.center.x > bodyBounds.center.x) ? -1 : 1;
                    pullDistance = bodyBounds.extents.x;
                    break;
                case Direction.y:
                    subDirection = (handleBounds.center.y > bodyBounds.center.y) ? -1 : 1;
                    pullDistance = bodyBounds.extents.y;
                    break;
                case Direction.z:
                    subDirection = (handleBounds.center.z > bodyBounds.center.z) ? -1 : 1;
                    pullDistance = bodyBounds.extents.z;
                    break;
            }

            if (body & handle)
            {
                // handle should be outside body hierarchy, otherwise anchor-by-bounds calculation is off
                if (handle.transform.IsChildOf(body.transform))
                {
                    return false;
                }
            }

            if (cjCreated)
            {
                cj.xMotion = ConfigurableJointMotion.Locked;
                cj.yMotion = ConfigurableJointMotion.Locked;
                cj.zMotion = ConfigurableJointMotion.Locked;

                switch (finalDirection)
                {
                    case Direction.x:
                        cj.axis = Vector3.right;
                        cj.xMotion = ConfigurableJointMotion.Limited;
                        break;
                    case Direction.y:
                        cj.axis = Vector3.up;
                        cj.yMotion = ConfigurableJointMotion.Limited;
                        break;
                    case Direction.z:
                        cj.axis = Vector3.forward;
                        cj.zMotion = ConfigurableJointMotion.Limited;
                        break;
                }
                cj.anchor = cj.axis * (-subDirection * pullDistance);
            }
            if (cj)
            {
                cj.angularXMotion = ConfigurableJointMotion.Locked;
                cj.angularYMotion = ConfigurableJointMotion.Locked;
                cj.angularZMotion = ConfigurableJointMotion.Locked;

                pullDistance *= 1.8f; // don't let it pull out completely

                SoftJointLimit limit = cj.linearLimit;
                limit.limit = pullDistance;
                cj.linearLimit = limit;
            }
            if (cfCreated)
            {
                cf.force = getThirdDirection(cj.axis, cj.secondaryAxis) * subDirection * -10f;
            }

            return true;
        }

        protected override ControlValueRange RegisterValueRange()
        {
            return new ControlValueRange() { controlMin = 0, controlMax = 100 };
        }

        protected override void HandleUpdate()
        {
            value = CalculateValue();
            cf.enabled = snapping && Mathf.Abs(value) < 2f;
        }

        private void InitBody()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
            rb.isKinematic = false;

            io = GetComponent<VRTK_InteractableObject>();
            if (io == null)
            {
                io = gameObject.AddComponent<VRTK_InteractableObject>();
            }
            io.isGrabbable = true;
            io.precisionSnap = true;
            io.grabAttachMechanic = VRTK_InteractableObject.GrabAttachType.Spring_Joint;

            cj = GetComponent<ConfigurableJoint>();
            if (cj == null)
            {
                cj = gameObject.AddComponent<ConfigurableJoint>();
                cjCreated = true;
            }

            cf = GetComponent<ConstantForce>();
            if (cf == null)
            {
                cf = gameObject.AddComponent<ConstantForce>();
                cf.enabled = false;
                cfCreated = true;
            }
        }

        private void InitHandle()
        {
            handleRb = getHandle().GetComponent<Rigidbody>();
            if (handleRb == null)
            {
                handleRb = getHandle().AddComponent<Rigidbody>();
            }
            handleRb.isKinematic = false;
            handleRb.useGravity = false;

            handleFj = getHandle().GetComponent<FixedJoint>();
            if (handleFj == null)
            {
                handleFj = getHandle().AddComponent<FixedJoint>();
                handleFj.connectedBody = rb;
            }
        }

        private Direction DetectDirection()
        {
            Direction direction = Direction.autodetect;

            Bounds handleBounds = Utilities.GetBounds(getHandle().transform, transform);
            Bounds bodyBounds = Utilities.GetBounds(getBody().transform, transform);

            float lengthX = Mathf.Abs(handleBounds.center.x - (bodyBounds.center.x + bodyBounds.extents.x));
            float lengthY = Mathf.Abs(handleBounds.center.y - (bodyBounds.center.y + bodyBounds.extents.y));
            float lengthZ = Mathf.Abs(handleBounds.center.z - (bodyBounds.center.z + bodyBounds.extents.z));
            float lengthNegX = Mathf.Abs(handleBounds.center.x - (bodyBounds.center.x - bodyBounds.extents.x));
            float lengthNegY = Mathf.Abs(handleBounds.center.y - (bodyBounds.center.y - bodyBounds.extents.y));
            float lengthNegZ = Mathf.Abs(handleBounds.center.z - (bodyBounds.center.z - bodyBounds.extents.z));

            if (Utilities.IsLowest(lengthX, new float[] { lengthY, lengthZ, lengthNegX, lengthNegY, lengthNegZ }))
            {
                direction = Direction.x;
            }
            else if (Utilities.IsLowest(lengthNegX, new float[] { lengthX, lengthY, lengthZ, lengthNegY, lengthNegZ }))
            {
                direction = Direction.x;
            }
            else if (Utilities.IsLowest(lengthY, new float[] { lengthX, lengthZ, lengthNegX, lengthNegY, lengthNegZ }))
            {
                direction = Direction.y;
            }
            else if (Utilities.IsLowest(lengthNegY, new float[] { lengthX, lengthY, lengthZ, lengthNegX, lengthNegZ }))
            {
                direction = Direction.y;
            }
            else if (Utilities.IsLowest(lengthZ, new float[] { lengthX, lengthY, lengthNegX, lengthNegY, lengthNegZ }))
            {
                direction = Direction.z;
            }
            else if (Utilities.IsLowest(lengthNegZ, new float[] { lengthX, lengthY, lengthZ, lengthNegY, lengthNegX }))
            {
                direction = Direction.z;
            }

            return direction;
        }

        private float CalculateValue()
        {
            return Mathf.Round((transform.position - initialPosition).magnitude / pullDistance * 100);
        }

        private GameObject getBody()
        {
            return (body) ? body : gameObject;
        }

        private GameObject getHandle()
        {
            return (handle) ? handle : gameObject;
        }
    }
}