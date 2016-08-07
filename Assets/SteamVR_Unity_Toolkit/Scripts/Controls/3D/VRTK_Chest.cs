namespace VRTK
{
    using UnityEngine;

    public class VRTK_Chest : VRTK_Control
    {
        public Direction direction = Direction.autodetect;

        public GameObject lid;
        public GameObject body;
        public GameObject handle;
        [Tooltip("An optional game object that is the parent of the content inside the chest. If set all interactables will become managed so that they only react if the lid is wide enough open.")]
        public GameObject content;
        [Tooltip("Will make the content invisible if the chest is closed. This way players cannot peak into it by moving the camera.")]
        public bool hideContent = true;
        public float maxAngle = 160f;

        private float minAngle = 0f;
        private float stepSize = 1f;

        private Rigidbody handleRb;
        private FixedJoint handleFj;
        private VRTK_InteractableObject handleIo;
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
            Bounds handleBounds = Utilities.GetBounds(handle.transform, handle.transform);
            float length = handleBounds.extents.y * 5f;
            Vector3 point = handleBounds.center + new Vector3(0, length, 0);
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

            Gizmos.DrawLine(handleBounds.center + new Vector3(0, handleBounds.extents.y, 0), point);
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
            if (lid == null || body == null || handle == null)
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
            Bounds lidBounds = Utilities.GetBounds(lid.transform, transform);
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

            if (lid & handle)
            {
                // handle should be outside lid hierarchy, otherwise anchor-by-bounds calculation is off
                if (handle.transform.IsChildOf(lid.transform))
                {
                    return false;
                }
            }
            if (lidHjCreated && handle)
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

            Bounds handleBounds = Utilities.GetBounds(handle.transform, transform);
            Bounds lidBounds = Utilities.GetBounds(lid.transform, transform);

            float lengthX = Mathf.Abs(handleBounds.center.x - (lidBounds.center.x + lidBounds.extents.x));
            float lengthZ = Mathf.Abs(handleBounds.center.z - (lidBounds.center.z + lidBounds.extents.z));
            float lengthNegX = Mathf.Abs(handleBounds.center.x - (lidBounds.center.x - lidBounds.extents.x));
            float lengthNegZ = Mathf.Abs(handleBounds.center.z - (lidBounds.center.z - lidBounds.extents.z));

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
                handleFj.connectedBody = lidRb;
            }

            handleIo = handle.GetComponent<VRTK_InteractableObject>();
            if (handleIo == null)
            {
                handleIo = handle.AddComponent<VRTK_InteractableObject>();
            }
            handleIo.isGrabbable = true;
            handleIo.precisionSnap = true;
            handleIo.grabAttachMechanic = VRTK_InteractableObject.GrabAttachType.Track_Object;
        }

        private float CalculateValue()
        {
            return Mathf.Round((minAngle + Mathf.Clamp01(Mathf.Abs(lidHj.angle / (lidHj.limits.max - lidHj.limits.min))) * (maxAngle - minAngle)) / stepSize) * stepSize;
        }
    }
}