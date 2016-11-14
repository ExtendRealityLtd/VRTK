// Slider|Controls3D|100090
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Attaching the script to a game object will allow the user to interact with it as if it were a horizontal or vertical slider. The direction can be freely set and auto-detection is supported.
    /// </summary>
    /// <remarks>
    /// The script will instantiate the required Rigidbody and Interactable components automatically in case they do not exist yet.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/025_Controls_Overview` has a selection of sliders at various angles with different step values to demonstrate their usage.
    /// </example>
    public class VRTK_Slider : VRTK_Control
    {
        [Tooltip("The axis on which the slider should move. All other axis will be frozen.")]
        public Direction direction = Direction.autodetect;
        [Tooltip("The minimum value of the slider.")]
        public float min = 0f;
        [Tooltip("The maximum value of the slider.")]
        public float max = 100f;
        [Tooltip("The increments in which slider values can change. The slider supports snapping.")]
        public float stepSize = 0.1f;
        [Tooltip("Automatically detect the minimum and maximum positions.")]
        public bool detectMinMax = true;
        [Tooltip("The minimum point on the slider.")]
        public Vector3 minPoint;
        [Tooltip("The maximum point on the slider.")]
        public Vector3 maxPoint;

        private static float MAX_AUTODETECT_SLIDER_LENGTH = 30; // multiple of the slider width
        private Direction finalDirection;
        private Vector3 finalMinPoint;
        private Vector3 finalMaxPoint;
        private Rigidbody rb;
        private VRTK_InteractableObject io;

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (!enabled || !setupSuccessful)
            {
                return;
            }

            // axis and min/max
            Vector3 center = (direction == Direction.autodetect) ? bounds.center : transform.position;
            Gizmos.DrawLine(center, finalMinPoint);
            Gizmos.DrawLine(center, finalMaxPoint);
        }

        protected override void InitRequiredComponents()
        {
            InitRigidBody();
            InitInteractable();
        }

        protected override bool DetectSetup()
        {
            finalDirection = direction;
            if (direction == Direction.autodetect)
            {
                finalDirection = DetectDirection();
                if (finalDirection == Direction.autodetect)
                {
                    return false;
                }
            }
            else if (detectMinMax)
            {
                if (!DoDetectMinMax(finalDirection))
                {
                    return false;
                }
            }
            else
            {
                // convert local positions of min/max to world coordinates
                Vector3 curPos = transform.localPosition;
                transform.localPosition = minPoint;
                finalMinPoint = transform.position;
                transform.localPosition = maxPoint;
                finalMaxPoint = transform.position;
                transform.localPosition = curPos;
            }
            SetConstraints(finalDirection);

            return true;
        }

        protected override ControlValueRange RegisterValueRange()
        {
            return new ControlValueRange() { controlMin = min, controlMax = max };
        }

        protected override void HandleUpdate()
        {
            EnsureSliderInRange();

            value = CalculateValue();
            SnapToValue(value);
        }

        private void InitRigidBody()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.drag = 10; // otherwise slider will continue to move too far on its own
        }

        private void SetConstraints(Direction direction)
        {
            if (!rb)
            {
                return;
            }

            rb.constraints = RigidbodyConstraints.FreezeAll;
            switch (direction)
            {
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

        private void InitInteractable()
        {
            io = GetComponent<VRTK_InteractableObject>();
            if (io == null)
            {
                io = gameObject.AddComponent<VRTK_InteractableObject>();
            }
            io.isGrabbable = true;
            io.grabAttachMechanicScript = gameObject.AddComponent<GrabAttachMechanics.VRTK_TrackObjectGrabAttach>();
            io.grabAttachMechanicScript.precisionGrab = true;
            io.secondaryGrabActionScript = gameObject.AddComponent<SecondaryControllerGrabActions.VRTK_SwapControllerGrabAction>();
            io.stayGrabbedOnTeleport = false;
        }

        private Direction DetectDirection()
        {
            Direction direction = Direction.autodetect;
            Bounds bounds = VRTK_SharedMethods.GetBounds(transform);

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

            if (lengthX < lengthY & lengthX < lengthZ)
            {
                if (lengthY < lengthZ)
                {
                    direction = Direction.y;
                }
                else if (lengthZ < lengthY)
                {
                    direction = Direction.z;
                }
                else
                { // onset
                    direction = Direction.x;
                }
            }
            if (lengthY < lengthX & lengthY < lengthZ)
            {
                if (lengthX < lengthZ)
                {
                    direction = Direction.x;
                }
                else if (lengthZ < lengthX)
                {
                    direction = Direction.z;
                }
                else
                { // onset
                    direction = Direction.y;
                }
            }
            if (lengthZ < lengthX & lengthZ < lengthY)
            {
                if (lengthX < lengthY)
                {
                    direction = Direction.x;
                }
                else if (lengthY < lengthX)
                {
                    direction = Direction.y;
                }
                else
                { // onset
                    direction = Direction.z;
                }
            }

            if (direction != Direction.autodetect)
            {
                if (!DoDetectMinMax(direction))
                {
                    direction = Direction.autodetect;
                }
            }
            else
            {
                finalMinPoint = transform.position;
                finalMaxPoint = transform.position;
            }

            return direction;
        }

        private bool DoDetectMinMax(Direction direction)
        {
            Bounds bounds = VRTK_SharedMethods.GetBounds(transform);
            Vector3 v = Vector3.zero;
            float extents = 0;

            switch (direction)
            {
                case Direction.x:
                    v = Vector3.left;
                    extents = bounds.extents.x;
                    break;
                case Direction.y:
                    v = Vector3.down;
                    extents = bounds.extents.y;
                    break;
                case Direction.z:
                    v = Vector3.forward;
                    extents = bounds.extents.z;
                    break;
            }

            RaycastHit hit1;
            RaycastHit hit2;
            Physics.Raycast(bounds.center, v, out hit1, extents * MAX_AUTODETECT_SLIDER_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, v * -1, out hit2, extents * MAX_AUTODETECT_SLIDER_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);

            if (hit1.collider && hit2.collider)
            {
                finalMinPoint = hit1.point;
                finalMaxPoint = hit2.point;

                // subtract width of slider to reach all values
                finalMinPoint = finalMinPoint + (finalMaxPoint - finalMinPoint).normalized * extents;
                finalMaxPoint = finalMaxPoint - (finalMaxPoint - finalMinPoint).normalized * extents;

                return true;
            }
            else
            {
                finalMinPoint = transform.position;
                finalMaxPoint = transform.position;
            }

            return false;
        }

        private void EnsureSliderInRange()
        {
            switch (finalDirection)
            {
                case Direction.x:
                    if (transform.position.x > finalMaxPoint.x)
                    {
                        transform.position = finalMaxPoint;
                    }
                    else if (transform.position.x < finalMinPoint.x)
                    {
                        transform.position = finalMinPoint;
                    }
                    break;
                case Direction.y:
                    if (transform.position.y > finalMaxPoint.y)
                    {
                        transform.position = finalMaxPoint;
                    }
                    else if (transform.position.y < finalMinPoint.y)
                    {
                        transform.position = finalMinPoint;
                    }
                    break;
                case Direction.z:
                    if (transform.position.z > finalMaxPoint.z)
                    {
                        transform.position = finalMaxPoint;
                    }
                    else if (transform.position.z < finalMinPoint.z)
                    {
                        transform.position = finalMinPoint;
                    }
                    break;
            }
        }

        private float CalculateValue()
        {
            Vector3 center = (direction == Direction.autodetect) ? VRTK_SharedMethods.GetBounds(transform).center : transform.position;
            float dist1 = Vector3.Distance(finalMinPoint, center);
            float dist2 = Vector3.Distance(center, finalMaxPoint);

            return Mathf.Round((min + Mathf.Clamp01(dist1 / (dist1 + dist2)) * (max - min)) / stepSize) * stepSize;
        }

        private void SnapToValue(float value)
        {
            transform.position = finalMinPoint + ((value - min) / (max - min)) * (finalMaxPoint - finalMinPoint).normalized * Vector3.Distance(finalMinPoint, finalMaxPoint);
        }
    }
}