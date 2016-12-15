// Chest|Controls3D|100030
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Transforms a game object into a chest with a lid. The direction can be auto-detected with very high reliability or set manually.
    /// </summary>
    /// <remarks>
    /// The script will instantiate the required Rigidbody, Interactable and HingeJoint components automatically in case they do not exist yet. It will expect three distinct game objects: a body, a lid and a handle. These should be independent and not children of each other.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/025_Controls_Overview` shows a chest that can be open and closed, it also displays the current opening angle of the chest.
    /// </example>
    public class VRTK_Chest : VRTK_Control
    {
        [Tooltip("The axis on which the chest should open. All other axis will be frozen.")]
        public Direction direction = Direction.autodetect;
        [Tooltip("The game object for the lid.")]
        public GameObject lid;
        [Tooltip("The game object for the body.")]
        public GameObject body;
        [Tooltip("The game object for the handle.")]
        public GameObject handle;
        [Tooltip("The parent game object for the chest content elements.")]
        public GameObject content;
        [Tooltip("Makes the content invisible while the chest is closed.")]
        public bool hideContent = true;
        [Tooltip("The maximum opening angle of the chest.")]
        public float maxAngle = 160f;

        private float minAngle = 0f;
        private float stepSize = 1f;
        private Rigidbody handleRb;
        private FixedJoint handleFj;
        private VRTK_InteractableObject io;
        private Rigidbody lidRb;
        private HingeJoint lidHj;
        private Rigidbody bodyRb;
        private Direction finalDirection;
        private float subDirection = 1; // positive or negative can be determined automatically since handle dictates that
        private bool lidHjCreated;

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (!enabled || !setupSuccessful)
            {
                return;
            }

            // show opening direction
            Bounds bounds;
            if (handle)
            {
                bounds = VRTK_SharedMethods.GetBounds(handle.transform, handle.transform);
            }
            else
            {
                bounds = VRTK_SharedMethods.GetBounds(lid.transform, lid.transform);
            }
            float length = bounds.extents.y * 5f;
            Vector3 point = bounds.center + new Vector3(0, length, 0);
            switch (finalDirection)
            {
                case Direction.x:
                    point += transform.right.normalized * (length / 2f) * subDirection;
                    break;
                case Direction.y:
                    point += transform.up.normalized * (length / 2f) * subDirection;
                    break;
                case Direction.z:
                    point += transform.forward.normalized * (length / 2f) * subDirection;
                    break;
            }

            Gizmos.DrawLine(bounds.center + new Vector3(0, bounds.extents.y, 0), point);
            Gizmos.DrawSphere(point, length / 8f);
        }

        protected override void InitRequiredComponents()
        {
            InitBody();
            InitLid();
            InitHandle();

            SetContent(content, hideContent);
        }

        protected override bool DetectSetup()
        {
            if (lid == null || body == null)
            {
                return false;
            }

            finalDirection = (direction == Direction.autodetect) ? DetectDirection() : direction;
            if (finalDirection == Direction.autodetect)
            {
                return false;
            }

            Bounds lidBounds = VRTK_SharedMethods.GetBounds(lid.transform, transform);

            // determin sub-direction depending on handle
            if (handle)
            {
                Bounds handleBounds = VRTK_SharedMethods.GetBounds(handle.transform, transform);
                switch (finalDirection)
                {
                    case Direction.x:
                        subDirection = (handleBounds.center.x > lidBounds.center.x) ? -1 : 1;
                        break;
                    case Direction.y:
                        subDirection = (handleBounds.center.y > lidBounds.center.y) ? -1 : 1;
                        break;
                    case Direction.z:
                        subDirection = (handleBounds.center.z > lidBounds.center.z) ? -1 : 1;
                        break;
                }

                // handle should be outside lid hierarchy, otherwise anchor-by-bounds calculation is off
                if (handle.transform.IsChildOf(lid.transform))
                {
                    return false;
                }
            }
            else
            {
                subDirection = -1;
            }
            if (lidHjCreated)
            {
                lidHj.useLimits = true;
                lidHj.enableCollision = true;

                JointLimits limits = lidHj.limits;
                switch (finalDirection)
                {
                    case Direction.x:
                        lidHj.anchor = new Vector3(subDirection * lidBounds.extents.x, 0, 0);
                        lidHj.axis = new Vector3(0, 0, 1);
                        if (subDirection > 0)
                        {
                            limits.min = -maxAngle;
                            limits.max = minAngle;
                        }
                        else
                        {
                            limits.min = minAngle;
                            limits.max = maxAngle;
                        }
                        break;
                    case Direction.y:
                        lidHj.anchor = new Vector3(0, subDirection * lidBounds.extents.y, 0);
                        lidHj.axis = new Vector3(0, 1, 0);
                        if (subDirection > 0)
                        {
                            limits.min = -maxAngle;
                            limits.max = minAngle;
                        }
                        else
                        {
                            limits.min = minAngle;
                            limits.max = maxAngle;
                        }
                        break;
                    case Direction.z:
                        lidHj.anchor = new Vector3(0, 0, subDirection * lidBounds.extents.z);
                        lidHj.axis = new Vector3(1, 0, 0);
                        if (subDirection < 0)
                        {
                            limits.min = -maxAngle;
                            limits.max = minAngle;
                        }
                        else
                        {
                            limits.min = minAngle;
                            limits.max = maxAngle;
                        }
                        break;
                }
                lidHj.limits = limits;
            }

            return true;
        }

        protected override ControlValueRange RegisterValueRange()
        {
            return new ControlValueRange() { controlMin = lidHj.limits.min, controlMax = lidHj.limits.max };
        }

        protected override void HandleUpdate()
        {
            value = CalculateValue();
        }

        private Direction DetectDirection()
        {
            Direction direction = Direction.autodetect;

            if (!handle)
            {
                return direction;
            }

            Bounds handleBounds = VRTK_SharedMethods.GetBounds(handle.transform, transform);
            Bounds lidBounds = VRTK_SharedMethods.GetBounds(lid.transform, transform);

            float lengthX = Mathf.Abs(handleBounds.center.x - (lidBounds.center.x + lidBounds.extents.x));
            float lengthZ = Mathf.Abs(handleBounds.center.z - (lidBounds.center.z + lidBounds.extents.z));
            float lengthNegX = Mathf.Abs(handleBounds.center.x - (lidBounds.center.x - lidBounds.extents.x));
            float lengthNegZ = Mathf.Abs(handleBounds.center.z - (lidBounds.center.z - lidBounds.extents.z));

            if (VRTK_SharedMethods.IsLowest(lengthX, new float[] { lengthZ, lengthNegX, lengthNegZ }))
            {
                direction = Direction.x;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthNegX, new float[] { lengthX, lengthZ, lengthNegZ }))
            {
                direction = Direction.x;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthZ, new float[] { lengthX, lengthNegX, lengthNegZ }))
            {
                direction = Direction.z;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthNegZ, new float[] { lengthX, lengthZ, lengthNegX }))
            {
                direction = Direction.z;
            }

            return direction;
        }

        private void InitBody()
        {
            bodyRb = body.GetComponent<Rigidbody>();
            if (bodyRb == null)
            {
                bodyRb = body.AddComponent<Rigidbody>();
                bodyRb.isKinematic = true; // otherwise body moves/falls over when lid is moved or fully open
            }
        }

        private void InitLid()
        {
            lidRb = lid.GetComponent<Rigidbody>();
            if (lidRb == null)
            {
                lidRb = lid.AddComponent<Rigidbody>();
            }

            lidHj = lid.GetComponent<HingeJoint>();
            if (lidHj == null)
            {
                lidHj = lid.AddComponent<HingeJoint>();
                lidHjCreated = true;
            }
            lidHj.connectedBody = bodyRb;

            if (!handle)
            {
                CreateIO(lid);
            }
        }

        private void InitHandle()
        {
            if (!handle)
            {
                return;
            }
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
                handleFj.connectedBody = lidRb;
            }

            CreateIO(handle);
        }

        private void CreateIO(GameObject go)
        {
            io = go.GetComponent<VRTK_InteractableObject>();
            if (io == null)
            {
                io = go.AddComponent<VRTK_InteractableObject>();
            }
            io.isGrabbable = true;
            io.grabAttachMechanicScript = gameObject.AddComponent<GrabAttachMechanics.VRTK_TrackObjectGrabAttach>();
            io.grabAttachMechanicScript.precisionGrab = true;
            io.secondaryGrabActionScript = gameObject.AddComponent<SecondaryControllerGrabActions.VRTK_SwapControllerGrabAction>();
            io.stayGrabbedOnTeleport = false;
        }

        private float CalculateValue()
        {
            return Mathf.Round((minAngle + Mathf.Clamp01(Mathf.Abs(lidHj.angle / (lidHj.limits.max - lidHj.limits.min))) * (maxAngle - minAngle)) / stepSize) * stepSize;
        }
    }
}