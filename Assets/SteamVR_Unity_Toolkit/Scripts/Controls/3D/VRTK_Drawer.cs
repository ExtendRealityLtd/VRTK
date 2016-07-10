namespace VRTK
{
    using UnityEngine;

    public class VRTK_Drawer : VRTK_Control
    {
        public enum Direction
        {
            autodetect, x, z
        }

        public Direction direction = Direction.autodetect;

        public GameObject body;
        public GameObject handle;
        [Tooltip("An optional game object that is the parent of the content inside the drawer. If set all interactables will become managed so that they only react if the drawer is wide enough open.")]
        public GameObject content;

        private static float MIN_OPENING_DISTANCE = 20f; // percentage open

        private Rigidbody rb;
        private Rigidbody handleRb;
        private FixedJoint handleFj;
        private ConfigurableJoint cj;
        private VRTK_InteractableObject io;

        private Direction finalDirection;
        private float subDirection = 1; // positive or negative can be determined automatically since handle dictates that
        private float pullDistance = 0f;
        private Vector3 initialPosition;
        private bool cjCreated;

        private VRTK_InteractableObject[] contentIOs;

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (!setupSuccessful)
            {
                return;
            }

            // show opening direction
            Bounds handleBounds = Utilities.GetBounds(handle.transform, handle.transform);
            float length = handleBounds.extents.y * 5f;
            Vector3 point = handleBounds.center;
            switch (finalDirection)
            {
                case Direction.x:
                    point -= transform.right.normalized * length * subDirection;
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

            if (content)
            {
                contentIOs = content.GetComponentsInChildren<VRTK_InteractableObject>();
            }
        }

        protected override bool DetectSetup()
        {
            if (body == null || handle == null)
            {
                return false;
            }

            finalDirection = (direction == Direction.autodetect) ? DetectDirection() : direction;
            if (finalDirection == Direction.autodetect)
            {
                return false;
            }
            // determin sub-direction depending on handle
            Bounds handleBounds = Utilities.GetBounds(handle.transform, transform);
            Bounds bodyBounds = Utilities.GetBounds(body.transform, transform);
            switch (finalDirection)
            {
                case Direction.x:
                    subDirection = (handleBounds.center.x > bodyBounds.center.x) ? -1 : 1;
                    break;
                case Direction.z:
                    subDirection = (handleBounds.center.z > bodyBounds.center.z) ? -1 : 1;
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
            if (cjCreated && handle)
            {
                cj.xMotion = ConfigurableJointMotion.Locked;
                cj.yMotion = ConfigurableJointMotion.Locked;
                cj.zMotion = ConfigurableJointMotion.Locked;

                switch (finalDirection)
                {
                    case Direction.x:
                        cj.anchor = new Vector3(-subDirection * bodyBounds.extents.x, 0, 0);
                        cj.axis = new Vector3(1, 0, 0);
                        cj.xMotion = ConfigurableJointMotion.Limited;
                        pullDistance = bodyBounds.extents.x;
                        break;
                    case Direction.z:
                        cj.anchor = new Vector3(0, 0, -subDirection * bodyBounds.extents.z);
                        cj.axis = new Vector3(0, 0, 1);
                        cj.xMotion = ConfigurableJointMotion.Limited;
                        pullDistance = bodyBounds.extents.z;
                        break;
                }
                pullDistance *= 1.8f; // don't let it pull out completely

                SoftJointLimit limit = cj.linearLimit;
                limit.limit = pullDistance;
                cj.linearLimit = limit;
            }

            return true;
        }

        protected override void HandleUpdate()
        {
            value = CalculateValue();

            if (contentIOs != null)
            {
                HandleInteractables();
            }
        }

        private void HandleInteractables()
        {
            foreach (VRTK_InteractableObject io in contentIOs)
            {
                io.enabled = value > MIN_OPENING_DISTANCE;
            }
        }

        private void InitBody()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
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
                cj.angularXMotion = ConfigurableJointMotion.Locked;
                cj.angularYMotion = ConfigurableJointMotion.Locked;
                cj.angularZMotion = ConfigurableJointMotion.Locked;
                cj.configuredInWorldSpace = false;
                cjCreated = true;
            }
        }

        private void InitHandle()
        {
            handleRb = handle.GetComponent<Rigidbody>();
            if (handleRb == null)
            {
                handleRb = handle.AddComponent<Rigidbody>();
            }
            handleRb.isKinematic = false;
            handleRb.useGravity = false;

            handleFj = handle.GetComponent<FixedJoint>();
            if (handleFj == null)
            {
                handleFj = handle.AddComponent<FixedJoint>();
                handleFj.connectedBody = rb;
            }
        }

        private Direction DetectDirection()
        {
            Direction direction = Direction.autodetect;

            Bounds handleBounds = Utilities.GetBounds(handle.transform, transform);
            Bounds bodyBounds = Utilities.GetBounds(body.transform, transform);

            float lengthX = Mathf.Abs(handleBounds.center.x - (bodyBounds.center.x + bodyBounds.extents.x));
            float lengthZ = Mathf.Abs(handleBounds.center.z - (bodyBounds.center.z + bodyBounds.extents.z));
            float lengthNegX = Mathf.Abs(handleBounds.center.x - (bodyBounds.center.x - bodyBounds.extents.x));
            float lengthNegZ = Mathf.Abs(handleBounds.center.z - (bodyBounds.center.z - bodyBounds.extents.z));

            if (Utilities.IsLowest(lengthX, new float[] { lengthZ, lengthNegX, lengthNegZ }))
            {
                direction = Direction.x;
            }
            else if (Utilities.IsLowest(lengthNegX, new float[] { lengthX, lengthZ, lengthNegZ }))
            {
                direction = Direction.x;
            }
            else if (Utilities.IsLowest(lengthZ, new float[] { lengthX, lengthNegX, lengthNegZ }))
            {
                direction = Direction.z;
            }
            else if (Utilities.IsLowest(lengthNegZ, new float[] { lengthX, lengthZ, lengthNegX }))
            {
                direction = Direction.z;
            }

            return direction;
        }

        private float CalculateValue()
        {
            return Mathf.Round((transform.position - initialPosition).magnitude / pullDistance * 100);
        }
    }
}