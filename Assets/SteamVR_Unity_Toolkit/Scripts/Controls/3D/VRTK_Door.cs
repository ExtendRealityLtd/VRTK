namespace VRTK
{
    using UnityEngine;

    public class VRTK_Door : VRTK_Control
    {
        public Direction direction = Direction.autodetect;

        [Tooltip("An optional game object that will be used as the door. Otherwise the current object will be used.")]
        public GameObject door;
        [Tooltip("An optional game object that will be used to interact with the door. Otherwise the whole door will be interactable.")]
        public GameObject handles;
        [Tooltip("An optional game object that the door is connected to. This should be specified if the connected object will move as well.")]
        public GameObject frame;
        [Tooltip("An optional game object that is the parent of the content behind the door. If set all interactables will become managed so that they only react if the door is wide enough open.")]
        public GameObject content;
        [Tooltip("Will make the content invisible if the door is closed. This way players cannot peak into it by moving the camera.")]
        public bool hideContent = true;
        public float maxAngle = 120f;
        public bool openInward = false;
        public bool openOutward = true;
        public bool snapping = true;

        private static float DOOR_ANGULAR_DRAG = 10;
        private float stepSize = 1f;

        private Rigidbody handleRb;
        private FixedJoint handleFj;
        private VRTK_InteractableObject handleIo;
        private Rigidbody doorRb;
        private HingeJoint doorHj;
        private ConstantForce doorCf;
        private Rigidbody frameRb;

        private Direction finalDirection;
        private float subDirection = 1; // positive or negative can be determined automatically since handles dictate that
        private Vector3 secondaryDirection;
        private bool doorHjCreated = false;
        private bool doorCfCreated = false;

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (!enabled || !setupSuccessful)
            {
                return;
            }

            // show opening direction
            Bounds bounds;
            float length;
            if (handles)
            {
                bounds = Utilities.GetBounds(handles.transform, handles.transform);
                length = 5f;
            }
            else
            {
                bounds = Utilities.GetBounds(getDoor().transform, getDoor().transform);
                length = 1f;
            }
            Vector3 dir = Vector3.zero;
            Vector3 dir2 = Vector3.zero;
            bool invertGizmos = false;

            switch (finalDirection)
            {
                case Direction.x:
                    if (secondaryDirection == Vector3.up)
                    {
                        dir = transform.forward.normalized;
                        dir2 = transform.up.normalized;
                        length *= bounds.extents.y;
                        invertGizmos = true;
                    }
                    else
                    {
                        dir = transform.up.normalized;
                        dir2 = transform.forward.normalized;
                        length *= bounds.extents.z;
                    }
                    break;
                case Direction.y:
                    if (secondaryDirection == Vector3.right)
                    {
                        dir = transform.forward.normalized;
                        dir2 = transform.right.normalized;
                        length *= bounds.extents.x;
                    }
                    else
                    {
                        dir = transform.right.normalized;
                        dir2 = transform.forward.normalized;
                        length *= bounds.extents.z;
                        invertGizmos = true;
                    }
                    break;
                case Direction.z:
                    if (secondaryDirection == Vector3.up)
                    {
                        dir = transform.right.normalized;
                        dir2 = transform.up.normalized;
                        length *= bounds.extents.y;
                    }
                    else
                    {
                        dir = transform.up.normalized;
                        dir2 = transform.right.normalized;
                        length *= bounds.extents.z;
                        invertGizmos = true;
                    }
                    break;
            }

            if ((!invertGizmos && openInward) || (invertGizmos && openOutward))
            {
                Vector3 p1 = bounds.center;
                Vector3 p1end = p1 + dir2 * length * subDirection - dir * (length / 2f) * subDirection;
                Gizmos.DrawLine(p1, p1end);
                Gizmos.DrawSphere(p1end, length / 8f);
            }

            if ((!invertGizmos && openOutward) || (invertGizmos && openInward))
            {
                Vector3 p2 = bounds.center;
                Vector3 p2end = p2 + dir2 * length * subDirection + dir * (length / 2f) * subDirection;
                Gizmos.DrawLine(p2, p2end);
                Gizmos.DrawSphere(p2end, length / 8f);
            }
        }

        protected override void InitRequiredComponents()
        {
            InitFrame();
            InitDoor();
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

            Bounds doorBounds = Utilities.GetBounds(getDoor().transform, transform);
            if (handles)
            {
                // determin sub-direction depending on handle location
                Bounds handleBounds = Utilities.GetBounds(handles.transform, transform);
                switch (finalDirection)
                {
                    case Direction.x:
                        if ((handleBounds.center.z + handleBounds.extents.z) > (doorBounds.center.z + doorBounds.extents.z) || (handleBounds.center.z - handleBounds.extents.z) < (doorBounds.center.z - doorBounds.extents.z))
                        {
                            subDirection = (handleBounds.center.y > doorBounds.center.y) ? -1 : 1;
                            secondaryDirection = Vector3.up;
                        }
                        else
                        {
                            subDirection = (handleBounds.center.z > doorBounds.center.z) ? -1 : 1;
                            secondaryDirection = Vector3.forward;
                        }
                        break;
                    case Direction.y:
                        if ((handleBounds.center.z + handleBounds.extents.z) > (doorBounds.center.z + doorBounds.extents.z) || (handleBounds.center.z - handleBounds.extents.z) < (doorBounds.center.z - doorBounds.extents.z))
                        {
                            subDirection = (handleBounds.center.x > doorBounds.center.x) ? -1 : 1;
                            secondaryDirection = Vector3.right;
                        }
                        else
                        {
                            subDirection = (handleBounds.center.z > doorBounds.center.z) ? -1 : 1;
                            secondaryDirection = Vector3.forward;
                        }
                        break;
                    case Direction.z:
                        if ((handleBounds.center.x + handleBounds.extents.x) > (doorBounds.center.x + doorBounds.extents.x) || (handleBounds.center.x - handleBounds.extents.x) < (doorBounds.center.x - doorBounds.extents.x))
                        {
                            subDirection = (handleBounds.center.y > doorBounds.center.y) ? -1 : 1;
                            secondaryDirection = Vector3.up;
                        }
                        else
                        {
                            subDirection = (handleBounds.center.x > doorBounds.center.x) ? -1 : 1;
                            secondaryDirection = Vector3.right;
                        }
                        break;
                }
            }
            else
            {
                switch (finalDirection)
                {
                    case Direction.x:
                        secondaryDirection = (doorBounds.extents.y > doorBounds.extents.z) ? Vector3.up : Vector3.forward;
                        break;
                    case Direction.y:
                        secondaryDirection = (doorBounds.extents.x > doorBounds.extents.z) ? Vector3.right : Vector3.forward;
                        break;
                    case Direction.z:
                        secondaryDirection = (doorBounds.extents.y > doorBounds.extents.x) ? Vector3.up : Vector3.right;
                        break;
                }
                subDirection = 1;
            }

            if (doorHjCreated)
            {
                float extents = 0;
                if (secondaryDirection == Vector3.right)
                {
                    extents = doorBounds.extents.x / getDoor().transform.lossyScale.x;
                }
                else if (secondaryDirection == Vector3.up)
                {
                    extents = doorBounds.extents.y / getDoor().transform.lossyScale.y;
                }
                else
                {
                    extents = doorBounds.extents.z / getDoor().transform.lossyScale.z;
                }

                doorHj.anchor = secondaryDirection * subDirection * extents;
                switch (finalDirection)
                {
                    case Direction.x:
                        doorHj.axis = new Vector3(1, 0, 0);
                        break;
                    case Direction.y:
                        doorHj.axis = new Vector3(0, 1, 0);
                        break;
                    case Direction.z:
                        doorHj.axis = new Vector3(0, 0, 1);
                        break;
                }
            }
            if (doorHj)
            {
                doorHj.useLimits = true;
                doorHj.enableCollision = true;

                JointLimits limits = doorHj.limits;
                limits.min = openInward ? -maxAngle : 0;
                limits.max = openOutward ? maxAngle : 0;
                limits.bounciness = 0;
                doorHj.limits = limits;
            }
            if (doorCfCreated)
            {
                doorCf.force = getThirdDirection(doorHj.axis, secondaryDirection) * subDirection * -50f;
            }

            return true;
        }

        protected override ControlValueRange RegisterValueRange()
        {
            return new ControlValueRange() { controlMin = doorHj.limits.min, controlMax = doorHj.limits.max };
        }

        protected override void HandleUpdate()
        {
            value = CalculateValue();
            doorCf.enabled = snapping && (openOutward ^ openInward) && Mathf.Abs(value) < 2f; // snapping only works for single direction doors so far
        }

        private Direction DetectDirection()
        {
            Direction direction = Direction.autodetect;

            if (handles)
            {
                Bounds handleBounds = Utilities.GetBounds(handles.transform, transform);
                Bounds doorBounds = Utilities.GetBounds(getDoor().transform, transform, handles.transform);

                // handles determine direction, there are actually two directions possible depending on handle position, we'll just detect one of them for now, preference is y
                if ((handleBounds.center.y + handleBounds.extents.y) > (doorBounds.center.y + doorBounds.extents.y) || (handleBounds.center.y - handleBounds.extents.y) < (doorBounds.center.y - doorBounds.extents.y))
                {
                    direction = Direction.x;
                }
                else
                {
                    direction = Direction.y;
                }
            }

            return direction;
        }

        private void InitFrame()
        {
            if (frame == null)
            {
                return;
            }

            frameRb = frame.GetComponent<Rigidbody>();
            if (frameRb == null)
            {
                frameRb = frame.AddComponent<Rigidbody>();
                frameRb.isKinematic = true; // otherwise frame moves/falls over when door is moved or fully open
                frameRb.angularDrag = DOOR_ANGULAR_DRAG; // in case this is a nested door
            }
        }

        private void InitDoor()
        {
            Utilities.CreateColliders(getDoor());

            doorRb = getDoor().GetComponent<Rigidbody>();
            if (doorRb == null)
            {
                doorRb = getDoor().AddComponent<Rigidbody>();
                doorRb.angularDrag = DOOR_ANGULAR_DRAG;
            }
            doorRb.collisionDetectionMode = CollisionDetectionMode.Continuous; // otherwise door will not react to fast moving controller
            doorRb.isKinematic = false; // in case nested door as already created this

            doorHj = getDoor().GetComponent<HingeJoint>();
            if (doorHj == null)
            {
                doorHj = getDoor().AddComponent<HingeJoint>();
                doorHjCreated = true;
            }
            doorHj.connectedBody = frameRb;

            doorCf = getDoor().GetComponent<ConstantForce>();
            if (doorCf == null)
            {
                doorCf = getDoor().AddComponent<ConstantForce>();
                doorCf.enabled = false;
                doorCfCreated = true;
            }
        }

        private void InitHandle()
        {
            if (handles == null)
            {
                return;
            }

            if (handles.GetComponentInChildren<Collider>() == null)
            {
                Utilities.CreateColliders(handles);
            }

            handleRb = handles.GetComponent<Rigidbody>();
            if (handleRb == null)
            {
                handleRb = handles.AddComponent<Rigidbody>();
            }
            handleRb.isKinematic = false;
            handleRb.useGravity = false;

            handleFj = handles.GetComponent<FixedJoint>();
            if (handleFj == null)
            {
                handleFj = handles.AddComponent<FixedJoint>();
                handleFj.connectedBody = doorRb;
            }

            handleIo = handles.GetComponent<VRTK_InteractableObject>();
            if (handleIo == null)
            {
                handleIo = handles.AddComponent<VRTK_InteractableObject>();
            }
            handleIo.isGrabbable = true;
            handleIo.precisionSnap = true;
            handleIo.grabAttachMechanic = VRTK_InteractableObject.GrabAttachType.Track_Object;
        }

        private float CalculateValue()
        {
            return Mathf.Round((doorHj.angle) / stepSize) * stepSize;
        }

        private GameObject getDoor()
        {
            return (door) ? door : gameObject;
        }
    }
}