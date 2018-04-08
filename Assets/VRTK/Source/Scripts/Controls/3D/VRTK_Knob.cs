// Knob|Controls3D|100060
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Attaching the script to a game object will allow the user to interact with it as if it were a radial knob. The direction can be freely set.
    /// </summary>
    /// <remarks>
    /// The script will instantiate the required Rigidbody and Interactable components automatically in case they do not exist yet.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/025_Controls_Overview` has a couple of rotator knobs that can be rotated by grabbing with the controller and then rotating the controller in the desired direction.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Controls/3D/VRTK_Knob")]
    [System.Obsolete("`VRTK.VRTK_Knob` has been deprecated and can be recreated with `VRTK.Controllables.PhysicsBased.VRTK_PhysicsRotator`. This script will be removed in a future version of VRTK.")]
    public class VRTK_Knob : VRTK_Control
    {
        /// <summary>
        /// The direction of the knob.
        /// </summary>
        public enum KnobDirection
        {
            /// <summary>
            /// The world x direction.
            /// </summary>
            x,
            /// <summary>
            /// The world y direction.
            /// </summary>
            y,
            /// <summary>
            /// The world z direction.
            /// </summary>
            z
        }

        [Tooltip("An optional game object to which the knob will be connected. If the game object moves the knob will follow along.")]
        public GameObject connectedTo;
        [Tooltip("The axis on which the knob should rotate. All other axis will be frozen.")]
        public KnobDirection direction = KnobDirection.x;
        [Tooltip("The minimum value of the knob.")]
        public float min = 0f;
        [Tooltip("The maximum value of the knob.")]
        public float max = 100f;
        [Tooltip("The increments in which knob values can change.")]
        public float stepSize = 1f;

        protected const float MAX_AUTODETECT_KNOB_WIDTH = 3; // multiple of the knob width
        protected KnobDirection finalDirection;
        protected KnobDirection subDirection;
        protected bool subDirectionFound = false;
        protected Quaternion initialRotation;
        protected Vector3 initialLocalRotation;
        protected ConfigurableJoint knobJoint;
        protected bool knobJointCreated = false;

        protected override void InitRequiredComponents()
        {
            initialRotation = transform.rotation;
            initialLocalRotation = transform.localRotation.eulerAngles;
            InitKnob();
        }

        protected override bool DetectSetup()
        {
            finalDirection = direction;

            if (knobJointCreated)
            {
                knobJoint.angularXMotion = ConfigurableJointMotion.Locked;
                knobJoint.angularYMotion = ConfigurableJointMotion.Locked;
                knobJoint.angularZMotion = ConfigurableJointMotion.Locked;

                switch (finalDirection)
                {
                    case KnobDirection.x:
                        knobJoint.angularXMotion = ConfigurableJointMotion.Free;
                        break;
                    case KnobDirection.y:
                        knobJoint.angularYMotion = ConfigurableJointMotion.Free;
                        break;
                    case KnobDirection.z:
                        knobJoint.angularZMotion = ConfigurableJointMotion.Free;
                        break;
                }
            }

            if (knobJoint)
            {
                knobJoint.xMotion = ConfigurableJointMotion.Locked;
                knobJoint.yMotion = ConfigurableJointMotion.Locked;
                knobJoint.zMotion = ConfigurableJointMotion.Locked;

                if (connectedTo)
                {
                    knobJoint.connectedBody = connectedTo.GetComponent<Rigidbody>();
                }
            }

            return true;
        }

        protected override ControlValueRange RegisterValueRange()
        {
            return new ControlValueRange()
            {
                controlMin = min,
                controlMax = max
            };
        }

        protected override void HandleUpdate()
        {
            value = CalculateValue();
        }

        protected virtual void InitKnob()
        {
            Rigidbody knobRigidbody = GetComponent<Rigidbody>();
            if (knobRigidbody == null)
            {
                knobRigidbody = gameObject.AddComponent<Rigidbody>();
                knobRigidbody.angularDrag = 10; // otherwise knob will continue to move too far on its own
            }
            knobRigidbody.isKinematic = false;
            knobRigidbody.useGravity = false;

            VRTK_InteractableObject knobInteractableObject = GetComponent<VRTK_InteractableObject>();
            if (knobInteractableObject == null)
            {
                knobInteractableObject = gameObject.AddComponent<VRTK_InteractableObject>();
            }
            knobInteractableObject.isGrabbable = true;
            knobInteractableObject.grabAttachMechanicScript = gameObject.AddComponent<GrabAttachMechanics.VRTK_TrackObjectGrabAttach>();
            knobInteractableObject.grabAttachMechanicScript.precisionGrab = true;
            knobInteractableObject.secondaryGrabActionScript = gameObject.AddComponent<SecondaryControllerGrabActions.VRTK_SwapControllerGrabAction>();
            knobInteractableObject.stayGrabbedOnTeleport = false;

            knobJoint = GetComponent<ConfigurableJoint>();
            if (knobJoint == null)
            {
                knobJoint = gameObject.AddComponent<ConfigurableJoint>();
                knobJoint.configuredInWorldSpace = false;
                knobJointCreated = true;
            }

            if (connectedTo)
            {
                Rigidbody knobConnectedToRigidbody = connectedTo.GetComponent<Rigidbody>();
                if (knobConnectedToRigidbody == null)
                {
                    knobConnectedToRigidbody = connectedTo.AddComponent<Rigidbody>();
                    knobConnectedToRigidbody.useGravity = false;
                    knobConnectedToRigidbody.isKinematic = true;
                }
            }
        }

        protected virtual KnobDirection DetectDirection()
        {
            KnobDirection returnDirection = KnobDirection.x;
            Bounds bounds = VRTK_SharedMethods.GetBounds(transform);

            // shoot rays in all directions to learn about surroundings
            RaycastHit hitForward;
            RaycastHit hitBack;
            RaycastHit hitLeft;
            RaycastHit hitRight;
            RaycastHit hitUp;
            RaycastHit hitDown;
            Physics.Raycast(bounds.center, Vector3.forward, out hitForward, bounds.extents.z * MAX_AUTODETECT_KNOB_WIDTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.back, out hitBack, bounds.extents.z * MAX_AUTODETECT_KNOB_WIDTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.left, out hitLeft, bounds.extents.x * MAX_AUTODETECT_KNOB_WIDTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.right, out hitRight, bounds.extents.x * MAX_AUTODETECT_KNOB_WIDTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.up, out hitUp, bounds.extents.y * MAX_AUTODETECT_KNOB_WIDTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.down, out hitDown, bounds.extents.y * MAX_AUTODETECT_KNOB_WIDTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);

            // shortest valid ray wins
            float lengthX = (hitRight.collider != null) ? hitRight.distance : float.MaxValue;
            float lengthY = (hitDown.collider != null) ? hitDown.distance : float.MaxValue;
            float lengthZ = (hitBack.collider != null) ? hitBack.distance : float.MaxValue;
            float lengthNegX = (hitLeft.collider != null) ? hitLeft.distance : float.MaxValue;
            float lengthNegY = (hitUp.collider != null) ? hitUp.distance : float.MaxValue;
            float lengthNegZ = (hitForward.collider != null) ? hitForward.distance : float.MaxValue;

            // TODO: not yet the right decision strategy, works only partially
            if (VRTK_SharedMethods.IsLowest(lengthX, new float[] { lengthY, lengthZ, lengthNegX, lengthNegY, lengthNegZ }))
            {
                returnDirection = KnobDirection.z;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthY, new float[] { lengthX, lengthZ, lengthNegX, lengthNegY, lengthNegZ }))
            {
                returnDirection = KnobDirection.y;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthZ, new float[] { lengthX, lengthY, lengthNegX, lengthNegY, lengthNegZ }))
            {
                returnDirection = KnobDirection.x;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthNegX, new float[] { lengthX, lengthY, lengthZ, lengthNegY, lengthNegZ }))
            {
                returnDirection = KnobDirection.z;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthNegY, new float[] { lengthX, lengthY, lengthZ, lengthNegX, lengthNegZ }))
            {
                returnDirection = KnobDirection.y;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthNegZ, new float[] { lengthX, lengthY, lengthZ, lengthNegX, lengthNegY }))
            {
                returnDirection = KnobDirection.x;
            }

            return returnDirection;
        }

        protected virtual float CalculateValue()
        {
            if (!subDirectionFound)
            {
                float angleX = Mathf.Abs(transform.localRotation.eulerAngles.x - initialLocalRotation.x) % 90;
                float angleY = Mathf.Abs(transform.localRotation.eulerAngles.y - initialLocalRotation.y) % 90;
                float angleZ = Mathf.Abs(transform.localRotation.eulerAngles.z - initialLocalRotation.z) % 90;
                angleX = (Mathf.RoundToInt(angleX) >= 89) ? 0 : angleX;
                angleY = (Mathf.RoundToInt(angleY) >= 89) ? 0 : angleY;
                angleZ = (Mathf.RoundToInt(angleZ) >= 89) ? 0 : angleZ;

                if (Mathf.RoundToInt(angleX) != 0 || Mathf.RoundToInt(angleY) != 0 || Mathf.RoundToInt(angleZ) != 0)
                {
                    subDirection = angleX < angleY ? (angleY < angleZ ? KnobDirection.z : KnobDirection.y) : (angleX < angleZ ? KnobDirection.z : KnobDirection.x);
                    subDirectionFound = true;
                }
            }

            float angle = 0;
            switch (subDirection)
            {
                case KnobDirection.x:
                    angle = transform.localRotation.eulerAngles.x - initialLocalRotation.x;
                    break;
                case KnobDirection.y:
                    angle = transform.localRotation.eulerAngles.y - initialLocalRotation.y;
                    break;
                case KnobDirection.z:
                    angle = transform.localRotation.eulerAngles.z - initialLocalRotation.z;
                    break;
            }
            angle = Mathf.Round(angle * 1000f) / 1000f; // not rounding will produce slight offsets in 4th digit that mess up initial value

            // Quaternion.angle will calculate shortest route and only go to 180
            float calculatedValue = 0;
            if (angle > 0 && angle <= 180)
            {
                calculatedValue = 360 - Quaternion.Angle(initialRotation, transform.rotation);
            }
            else
            {
                calculatedValue = Quaternion.Angle(initialRotation, transform.rotation);
            }

            // adjust to value scale
            calculatedValue = Mathf.Round((min + Mathf.Clamp01(calculatedValue / 360f) * (max - min)) / stepSize) * stepSize;
            if (min > max && angle != 0)
            {
                calculatedValue = (max + min) - calculatedValue;
            }

            return calculatedValue;
        }
    }
}