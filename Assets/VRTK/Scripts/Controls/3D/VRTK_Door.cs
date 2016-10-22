// Door|Controls3D|0040
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Transforms a game object into a door with an optional handle attached to an optional frame. The direction can be freely set and also very reliably auto-detected.
    /// </summary>
    /// <remarks>
    /// There are situations when it can be very hard to automatically calculate the correct axis and anchor values for the hinge joint. If this situation is encountered then simply add the hinge joint manually and set these two values. All the rest will still be handled by the script.
    ///
    /// The script will instantiate the required Rigidbodies, Interactable and HingeJoint components automatically in case they do not exist yet. Gizmos will indicate the direction.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/025_Controls_Overview` shows a selection of door types, from a normal door and trapdoor, to a door with a cat-flap in the middle.
    /// </example>
    public class VRTK_Door : VRTK_Control
    {
        [Tooltip("The axis on which the door should open.")]
        public Direction direction = Direction.autodetect;
        [Tooltip("The game object for the door. Can also be an empty parent or left empty if the script is put onto the actual door mesh. If no colliders exist yet a collider will tried to be automatically attached to all children that expose renderers.")]
        public GameObject door;
        [Tooltip("The game object for the handles. Can also be an empty parent or left empty. If empty the door can only be moved using the rigidbody mode of the controller. If no collider exists yet a compound collider made up of all children will try to be calculated but this will fail if the door is rotated. In that case a manual collider will need to be assigned.")]
        public GameObject handles;
        [Tooltip("The game object for the frame to which the door is attached. Should only be set if the frame will move as well to ensure that the door moves along with the frame.")]
        public GameObject frame;
        [Tooltip("The parent game object for the door content elements.")]
        public GameObject content;
        [Tooltip("Makes the content invisible while the door is closed.")]
        public bool hideContent = true;
        [Tooltip("The maximum opening angle of the door.")]
        public float maxAngle = 120f;
        [Tooltip("Can the door be pulled to open.")]
        public bool openInward = false;
        [Tooltip("Can the door be pushed to open.")]
        public bool openOutward = true;
        [Tooltip("Keeps the door closed with a slight force. This way the door will not gradually open due to some minor physics effect. Only works if either inward or outward is selected, not both.")]
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
            Bounds handleBounds = new Bounds();
            Bounds doorBounds = Utilities.GetBounds(getDoor().transform, getDoor().transform);
            float length = 0.5f;
            if (handles)
            {
                handleBounds = Utilities.GetBounds(handles.transform, handles.transform);
            }
            Vector3 dir = Vector3.zero;
            Vector3 dir2 = Vector3.zero;
            Vector3 thirdDirection = getThirdDirection(Direction2Axis(finalDirection), secondaryDirection);
            bool invertGizmos = false;

            switch (finalDirection)
            {
                case Direction.x:
                    if (thirdDirection == Vector3.up)
                    {
                        dir = transform.up.normalized;
                        dir2 = transform.forward.normalized;
                        length *= doorBounds.extents.z;
                    }
                    else
                    {
                        dir = transform.forward.normalized;
                        dir2 = transform.up.normalized;
                        length *= doorBounds.extents.y;
                        invertGizmos = true;
                    }
                    break;
                case Direction.y:
                    if (thirdDirection == Vector3.right)
                    {
                        dir = transform.right.normalized;
                        dir2 = transform.forward.normalized;
                        length *= doorBounds.extents.z;
                        invertGizmos = true;
                    }
                    else
                    {
                        dir = transform.forward.normalized;
                        dir2 = transform.right.normalized;
                        length *= doorBounds.extents.x;
                    }
                    break;
                case Direction.z:
                    if (thirdDirection == Vector3.up)
                    {
                        dir = transform.up.normalized;
                        dir2 = transform.right.normalized;
                        length *= doorBounds.extents.x;
                        invertGizmos = true;
                    }
                    else
                    {
                        dir = transform.right.normalized;
                        dir2 = transform.up.normalized;
                        length *= doorBounds.extents.y;
                    }
                    break;
            }

            if ((!invertGizmos && openInward) || (invertGizmos && openOutward))
            {
                Vector3 p1 = (handles) ? handleBounds.center : doorBounds.center;
                Vector3 p1end = p1 + dir2 * length * subDirection - dir * (length / 2f) * subDirection;
                Gizmos.DrawLine(p1, p1end);
                Gizmos.DrawSphere(p1end, length / 8f);
            }

            if ((!invertGizmos && openOutward) || (invertGizmos && openInward))
            {
                Vector3 p2 = (handles) ? handleBounds.center : doorBounds.center;
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
            // detect axis
            doorHj = getDoor().GetComponent<HingeJoint>();
            if (doorHj && !doorHjCreated)
            {
                direction = Direction.autodetect;
            }
            finalDirection = (direction == Direction.autodetect) ? DetectDirection() : direction;
            if (finalDirection == Direction.autodetect)
            {
                return false;
            }
            if (doorHj && !doorHjCreated)
            {
                // if there is a hinge joint already it overrides axis selection
                direction = finalDirection;
            }

            // detect opening direction
            Bounds doorBounds = Utilities.GetBounds(getDoor().transform, transform);
            if (doorHj == null || doorHjCreated)
            {
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
                    // TODO: derive how to detect -1
                    subDirection = 1;
                }
            }
            else
            {
                // calculate directions from existing anchor
                Vector3 dir = doorBounds.center - doorHj.connectedAnchor;
                if (dir.x != 0)
                {
                    secondaryDirection = Vector3.right;
                    subDirection = dir.x <= 0 ? 1 : -1;
                }
                else if (dir.y != 0)
                {
                    secondaryDirection = Vector3.up;
                    subDirection = dir.y <= 0 ? 1 : -1;
                }
                else if (dir.z != 0)
                {
                    secondaryDirection = Vector3.forward;
                    subDirection = dir.z <= 0 ? 1 : -1;
                }
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
                doorHj.axis = Direction2Axis(finalDirection);
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
                doorCf.relativeForce = getThirdDirection(doorHj.axis, secondaryDirection) * subDirection * -50f;
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

        private Vector3 Direction2Axis(Direction direction)
        {
            Vector3 axis = Vector3.zero;

            switch (direction)
            {
                case Direction.x:
                    axis = new Vector3(1, 0, 0);
                    break;
                case Direction.y:
                    axis = new Vector3(0, 1, 0);
                    break;
                case Direction.z:
                    axis = new Vector3(0, 0, 1);
                    break;
            }

            return axis;
        }

        private Direction DetectDirection()
        {
            Direction direction = Direction.autodetect;

            if (doorHj && !doorHjCreated)
            {
                // use direction of hinge joint
                if (doorHj.axis == Vector3.right)
                {
                    direction = Direction.x;
                }
                else if (doorHj.axis == Vector3.up)
                {
                    direction = Direction.y;
                }
                else if (doorHj.axis == Vector3.forward)
                {
                    direction = Direction.z;
                }
            }
            else
            {
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
            handleIo.stayGrabbedOnTeleport = false;
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