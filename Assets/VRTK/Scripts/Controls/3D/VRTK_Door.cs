// Door|Controls3D|100040
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
        [Tooltip("The range at which the door must be to being closed before it snaps shut. Only works if either inward or outward is selected, not both.")]
        [Range(0, 1)]
        public float minSnapClose = 1;
        [Tooltip("The amount of friction the door will have whilst swinging when it is not grabbed.")]
        public float releasedFriction = 10f;
        [Tooltip("The amount of friction the door will have whilst swinging when it is grabbed.")]
        public float grabbedFriction = 100f;
        [Tooltip("If this is checked then only the door handle is grabbale to operate the door.")]
        public bool handleInteractableOnly = false;

        private float stepSize = 1f;
        private Rigidbody doorRigidbody;
        private HingeJoint doorHinge;
        private ConstantForce doorSnapForce;
        private Rigidbody frameRigidbody;
        private Direction finalDirection;
        private float subDirection = 1; // positive or negative can be determined automatically since handles dictate that
        private Vector3 secondaryDirection;
        private bool doorHingeCreated = false;
        private bool doorSnapForceCreated = false;

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (!enabled || !setupSuccessful)
            {
                return;
            }

            // show opening direction
            Bounds handleBounds = new Bounds();
            Bounds doorBounds = VRTK_SharedMethods.GetBounds(GetDoor().transform, GetDoor().transform);
            float extensionLength = 0.5f;
            if (handles)
            {
                handleBounds = VRTK_SharedMethods.GetBounds(handles.transform, handles.transform);
            }
            Vector3 firstDirection = Vector3.zero;
            Vector3 secondDirection = Vector3.zero;
            Vector3 thirdDirection = GetThirdDirection(Direction2Axis(finalDirection), secondaryDirection);
            bool invertGizmos = false;

            switch (finalDirection)
            {
                case Direction.x:
                    if (thirdDirection == Vector3.up)
                    {
                        firstDirection = transform.up.normalized;
                        secondDirection = transform.forward.normalized;
                        extensionLength *= doorBounds.extents.z;
                    }
                    else
                    {
                        firstDirection = transform.forward.normalized;
                        secondDirection = transform.up.normalized;
                        extensionLength *= doorBounds.extents.y;
                        invertGizmos = true;
                    }
                    break;
                case Direction.y:
                    if (thirdDirection == Vector3.right)
                    {
                        firstDirection = transform.right.normalized;
                        secondDirection = transform.forward.normalized;
                        extensionLength *= doorBounds.extents.z;
                        invertGizmos = true;
                    }
                    else
                    {
                        firstDirection = transform.forward.normalized;
                        secondDirection = transform.right.normalized;
                        extensionLength *= doorBounds.extents.x;
                    }
                    break;
                case Direction.z:
                    if (thirdDirection == Vector3.up)
                    {
                        firstDirection = transform.up.normalized;
                        secondDirection = transform.right.normalized;
                        extensionLength *= doorBounds.extents.x;
                        invertGizmos = true;
                    }
                    else
                    {
                        firstDirection = transform.right.normalized;
                        secondDirection = transform.up.normalized;
                        extensionLength *= doorBounds.extents.y;
                    }
                    break;
            }

            if ((!invertGizmos && openInward) || (invertGizmos && openOutward))
            {
                Vector3 point1Start = (handles) ? handleBounds.center : doorBounds.center;
                Vector3 point1End = point1Start + secondDirection * extensionLength * subDirection - firstDirection * (extensionLength / 2f) * subDirection;
                Gizmos.DrawLine(point1Start, point1End);
                Gizmos.DrawSphere(point1End, extensionLength / 8f);
            }

            if ((!invertGizmos && openOutward) || (invertGizmos && openInward))
            {
                Vector3 point2Start = (handles) ? handleBounds.center : doorBounds.center;
                Vector3 point2End = point2Start + secondDirection * extensionLength * subDirection + firstDirection * (extensionLength / 2f) * subDirection;
                Gizmos.DrawLine(point2Start, point2End);
                Gizmos.DrawSphere(point2End, extensionLength / 8f);
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
            doorHinge = GetDoor().GetComponent<HingeJoint>();
            if (doorHinge && !doorHingeCreated)
            {
                direction = Direction.autodetect;
            }
            finalDirection = (direction == Direction.autodetect) ? DetectDirection() : direction;
            if (finalDirection == Direction.autodetect)
            {
                return false;
            }
            if (doorHinge && !doorHingeCreated)
            {
                // if there is a hinge joint already it overrides axis selection
                direction = finalDirection;
            }

            // detect opening direction
            Bounds doorBounds = VRTK_SharedMethods.GetBounds(GetDoor().transform, transform);
            if (doorHinge == null || doorHingeCreated)
            {
                if (handles)
                {
                    // determin sub-direction depending on handle location
                    Bounds handleBounds = VRTK_SharedMethods.GetBounds(handles.transform, transform);
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
                Vector3 existingAnchorDirection = doorBounds.center - doorHinge.connectedAnchor;
                if (existingAnchorDirection.x != 0)
                {
                    secondaryDirection = Vector3.right;
                    subDirection = existingAnchorDirection.x <= 0 ? 1 : -1;
                }
                else if (existingAnchorDirection.y != 0)
                {
                    secondaryDirection = Vector3.up;
                    subDirection = existingAnchorDirection.y <= 0 ? 1 : -1;
                }
                else if (existingAnchorDirection.z != 0)
                {
                    secondaryDirection = Vector3.forward;
                    subDirection = existingAnchorDirection.z <= 0 ? 1 : -1;
                }
            }

            if (doorHingeCreated)
            {
                float extents = 0;
                if (secondaryDirection == Vector3.right)
                {
                    extents = doorBounds.extents.x / GetDoor().transform.lossyScale.x;
                }
                else if (secondaryDirection == Vector3.up)
                {
                    extents = doorBounds.extents.y / GetDoor().transform.lossyScale.y;
                }
                else
                {
                    extents = doorBounds.extents.z / GetDoor().transform.lossyScale.z;
                }

                doorHinge.anchor = secondaryDirection * subDirection * extents;
                doorHinge.axis = Direction2Axis(finalDirection);
            }
            if (doorHinge)
            {
                doorHinge.useLimits = true;
                doorHinge.enableCollision = true;

                JointLimits limits = doorHinge.limits;
                limits.min = openInward ? -maxAngle : 0;
                limits.max = openOutward ? maxAngle : 0;
                limits.bounciness = 0;
                doorHinge.limits = limits;
            }
            if (doorSnapForceCreated)
            {
                doorSnapForce.relativeForce = GetThirdDirection(doorHinge.axis, secondaryDirection) * (subDirection * (-5f * subDirection));
            }

            return true;
        }

        protected override ControlValueRange RegisterValueRange()
        {
            return new ControlValueRange()
            {
                controlMin = doorHinge.limits.min,
                controlMax = doorHinge.limits.max
            };
        }

        protected override void HandleUpdate()
        {
            value = CalculateValue();
            doorSnapForce.enabled = (openOutward ^ openInward) && Mathf.Abs(value) < (minSnapClose * 100f); // snapping only works for single direction doors so far
        }

        private Vector3 Direction2Axis(Direction givenDirection)
        {
            Vector3 returnAxis = Vector3.zero;

            switch (givenDirection)
            {
                case Direction.x:
                    returnAxis = new Vector3(1, 0, 0);
                    break;
                case Direction.y:
                    returnAxis = new Vector3(0, 1, 0);
                    break;
                case Direction.z:
                    returnAxis = new Vector3(0, 0, 1);
                    break;
            }

            return returnAxis;
        }

        private Direction DetectDirection()
        {
            Direction returnDirection = Direction.autodetect;

            if (doorHinge && !doorHingeCreated)
            {
                // use direction of hinge joint
                if (doorHinge.axis == Vector3.right)
                {
                    returnDirection = Direction.x;
                }
                else if (doorHinge.axis == Vector3.up)
                {
                    returnDirection = Direction.y;
                }
                else if (doorHinge.axis == Vector3.forward)
                {
                    returnDirection = Direction.z;
                }
            }
            else
            {
                if (handles)
                {
                    Bounds handleBounds = VRTK_SharedMethods.GetBounds(handles.transform, transform);
                    Bounds doorBounds = VRTK_SharedMethods.GetBounds(GetDoor().transform, transform, handles.transform);

                    // handles determine direction, there are actually two directions possible depending on handle position, we'll just detect one of them for now, preference is y
                    if ((handleBounds.center.y + handleBounds.extents.y) > (doorBounds.center.y + doorBounds.extents.y) || (handleBounds.center.y - handleBounds.extents.y) < (doorBounds.center.y - doorBounds.extents.y))
                    {
                        returnDirection = Direction.x;
                    }
                    else
                    {
                        returnDirection = Direction.y;
                    }
                }
            }

            return returnDirection;
        }

        private void InitFrame()
        {
            if (frame == null)
            {
                return;
            }

            frameRigidbody = frame.GetComponent<Rigidbody>();
            if (frameRigidbody == null)
            {
                frameRigidbody = frame.AddComponent<Rigidbody>();
                frameRigidbody.isKinematic = true; // otherwise frame moves/falls over when door is moved or fully open
                frameRigidbody.angularDrag = releasedFriction; // in case this is a nested door
            }
        }

        private void InitDoor()
        {
            GameObject actualDoor = GetDoor();
            VRTK_SharedMethods.CreateColliders(actualDoor);

            doorRigidbody = actualDoor.GetComponent<Rigidbody>();
            if (doorRigidbody == null)
            {
                doorRigidbody = actualDoor.AddComponent<Rigidbody>();
                doorRigidbody.angularDrag = releasedFriction;
            }
            doorRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // otherwise door will not react to fast moving controller
            doorRigidbody.isKinematic = false; // in case nested door as already created this

            doorHinge = actualDoor.GetComponent<HingeJoint>();
            if (doorHinge == null)
            {
                doorHinge = actualDoor.AddComponent<HingeJoint>();
                doorHingeCreated = true;
            }
            doorHinge.connectedBody = frameRigidbody;

            doorSnapForce = actualDoor.GetComponent<ConstantForce>();
            if (doorSnapForce == null)
            {
                doorSnapForce = actualDoor.AddComponent<ConstantForce>();
                doorSnapForce.enabled = false;
                doorSnapForceCreated = true;
            }

            if (!handleInteractableOnly)
            {
                CreateInteractableObject(actualDoor);
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
                VRTK_SharedMethods.CreateColliders(handles);
            }

            Rigidbody handleRigidbody = handles.GetComponent<Rigidbody>();
            if (handleRigidbody == null)
            {
                handleRigidbody = handles.AddComponent<Rigidbody>();
            }
            handleRigidbody.isKinematic = false;
            handleRigidbody.useGravity = false;

            FixedJoint handleFixedJoint = handles.GetComponent<FixedJoint>();
            if (handleFixedJoint == null)
            {
                handleFixedJoint = handles.AddComponent<FixedJoint>();
                handleFixedJoint.connectedBody = doorRigidbody;
            }

            if (handleInteractableOnly)
            {
                CreateInteractableObject(handles);
            }
        }

        private void CreateInteractableObject(GameObject target)
        {
            VRTK_InteractableObject targetInteractableObject = target.GetComponent<VRTK_InteractableObject>();
            if (targetInteractableObject == null)
            {
                targetInteractableObject = target.AddComponent<VRTK_InteractableObject>();
            }
            targetInteractableObject.isGrabbable = true;
            targetInteractableObject.grabAttachMechanicScript = target.AddComponent<GrabAttachMechanics.VRTK_RotatorTrackGrabAttach>();
            targetInteractableObject.grabAttachMechanicScript.precisionGrab = true;
            targetInteractableObject.secondaryGrabActionScript = target.AddComponent<SecondaryControllerGrabActions.VRTK_SwapControllerGrabAction>();
            targetInteractableObject.stayGrabbedOnTeleport = false;

            targetInteractableObject.InteractableObjectGrabbed += InteractableObjectGrabbed;
            targetInteractableObject.InteractableObjectUngrabbed += InteractableObjectUngrabbed;
        }

        private void InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            doorRigidbody.angularDrag = grabbedFriction;
        }

        private void InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            doorRigidbody.angularDrag = releasedFriction;
        }

        private float CalculateValue()
        {
            return Mathf.Round((doorHinge.angle) / stepSize) * stepSize;
        }

        private GameObject GetDoor()
        {
            return (door ? door : gameObject);
        }
    }
}