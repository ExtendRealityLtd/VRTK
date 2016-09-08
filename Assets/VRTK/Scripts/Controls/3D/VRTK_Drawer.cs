// Drawer|Controls3D|0050
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Transforms a game object into a drawer. The direction can be freely set and also auto-detected with very high reliability.
    /// </summary>
    /// <remarks>
    /// The script will instantiate the required Rigidbody, Interactable and Joint components automatically in case they do not exist yet. There are situations when it can be very hard to automatically calculate the correct axis for the joint. If this situation is encountered simply add the configurable joint manually and set the axis. All the rest will still be handled by the script.
    ///
    /// It will expect two distinct game objects: a body and a handle. These should be independent and not children of each other. The distance to which the drawer can be pulled out will automatically set depending on the length of it. If no body is specified the current object is assumed to be the body.
    ///
    /// It is possible to supply a third game object which is the root of the contents inside the drawer. When this is specified the VRTK_InteractableObject components will be automatically deactivated in case the drawer is closed or not yet far enough open. This eliminates the issue that a user could grab an object inside a drawer although it is closed.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/025_Controls_Overview` shows a drawer with contents that can be opened and closed freely and the contents can be removed from the drawer.
    /// </example>
    public class VRTK_Drawer : VRTK_Control
    {
        [Tooltip("The axis on which the drawer should open. All other axis will be frozen.")]
        public Direction direction = Direction.autodetect;
        [Tooltip("The game object for the body.")]
        public GameObject body;
        [Tooltip("The game object for the handle.")]
        public GameObject handle;
        [Tooltip("The parent game object for the drawer content elements.")]
        public GameObject content;
        [Tooltip("Makes the content invisible while the drawer is closed.")]
        public bool hideContent = true;
        [Tooltip("Keeps the drawer closed with a slight force. This way the drawer will not gradually open due to some minor physics effect.")]
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
            io.stayGrabbedOnTeleport = false;
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