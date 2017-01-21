// Button|Controls3D|100020
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Attaching the script to a game object will allow the user to interact with it as if it were a push button. The direction into which the button should be pushable can be freely set and auto-detection is supported. Since this is physics-based there needs to be empty space in the push direction so that the button can move.
    /// </summary>
    /// <remarks>
    /// The script will instantiate the required Rigidbody and ConstantForce components automatically in case they do not exist yet.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/025_Controls_Overview` shows a collection of pressable buttons that are interacted with by activating the rigidbody on the controller by pressing the grab button without grabbing an object.
    /// </example>
    public class VRTK_Button : VRTK_Control
    {
        public enum ButtonDirection
        {
            autodetect, x, y, z, negX, negY, negZ
        }

        [Tooltip("An optional game object to which the button will be connected. If the game object moves the button will follow along.")]
        public GameObject connectedTo;
        [Tooltip("The axis on which the button should move. All other axis will be frozen.")]
        public ButtonDirection direction = ButtonDirection.autodetect;
        [Tooltip("The local distance the button needs to be pushed until a push event is triggered.")]
        public float activationDistance = 1.0f;
        [Tooltip("The amount of force needed to push the button down as well as the speed with which it will go back into its original position.")]
        public float buttonStrength = 5.0f;

        [System.Serializable]
        public class ButtonEvents
        {
            /// <summary>
            /// Emitted when the button is successfully pushed.
            /// </summary>
            public UnityEvent OnPush;
        }

        public ButtonEvents events;

        private static float MAX_AUTODETECT_ACTIVATION_LENGTH = 4; // full hight of button
        private ButtonDirection finalDirection;
        private Vector3 restingPosition;
        private Vector3 activationDir;
        private Rigidbody rb;
        private ConfigurableJoint cj;
        private ConstantForce cf;
        private int forceCount = 0;

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (!enabled || !setupSuccessful)
            {
                return;
            }

            // visualize activation distance
            Gizmos.DrawLine(bounds.center, bounds.center + activationDir);
        }

        protected override void InitRequiredComponents()
        {
            restingPosition = transform.position;

            if (!GetComponent<Collider>())
            {
                gameObject.AddComponent<BoxCollider>();
            }

            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = false;
            rb.useGravity = false;

            cf = GetComponent<ConstantForce>();
            if (cf == null)
            {
                cf = gameObject.AddComponent<ConstantForce>();
            }

            if (connectedTo)
            {
                Rigidbody rb2 = connectedTo.GetComponent<Rigidbody>();
                if (rb2 == null)
                {
                    rb2 = connectedTo.AddComponent<Rigidbody>();
                }
                rb2.useGravity = false;
            }
        }

        protected override bool DetectSetup()
        {
            finalDirection = (direction == ButtonDirection.autodetect) ? DetectDirection() : direction;
            if (finalDirection == ButtonDirection.autodetect)
            {
                activationDir = Vector3.zero;
                return false;
            }
            if (direction != ButtonDirection.autodetect)
            {
                activationDir = CalculateActivationDir();
            }

            if (cf)
            {
                cf.force = GetForceVector();
            }

            if (Application.isPlaying)
            {
                cj = GetComponent<ConfigurableJoint>();

                bool recreate = false;
                Rigidbody oldBody = null;
                Vector3 oldAnchor = Vector3.zero;
                Vector3 oldAxis = Vector3.zero;

                if (cj)
                {
                    // save old values, needs to be recreated
                    oldBody = cj.connectedBody;
                    oldAnchor = cj.anchor;
                    oldAxis = cj.axis;
                    DestroyImmediate(cj);
                    recreate = true;
                }

                // since limit applies to both directions object needs to be moved halfway to activation before adding joint
                transform.position = transform.position + activationDir.normalized * activationDistance * 0.5f;
                cj = gameObject.AddComponent<ConfigurableJoint>();

                if (recreate)
                {
                    cj.connectedBody = oldBody;
                    cj.anchor = oldAnchor;
                    cj.axis = oldAxis;
                }
                if (connectedTo)
                {
                    cj.connectedBody = connectedTo.GetComponent<Rigidbody>();
                }

                SoftJointLimit sjl = new SoftJointLimit();
                sjl.limit = activationDistance * 0.501f; // set limit to half (since it applies to both directions) and a tiny bit larger since otherwise activation distance might be missed
                cj.linearLimit = sjl;

                cj.angularXMotion = ConfigurableJointMotion.Locked;
                cj.angularYMotion = ConfigurableJointMotion.Locked;
                cj.angularZMotion = ConfigurableJointMotion.Locked;
                cj.xMotion = ConfigurableJointMotion.Locked;
                cj.yMotion = ConfigurableJointMotion.Locked;
                cj.zMotion = ConfigurableJointMotion.Locked;

                switch (finalDirection)
                {
                    case ButtonDirection.x:
                    case ButtonDirection.negX:
                        if (Mathf.RoundToInt(Mathf.Abs(transform.right.x)) == 1)
                        {
                            cj.xMotion = ConfigurableJointMotion.Limited;
                        }
                        else if (Mathf.RoundToInt(Mathf.Abs(transform.up.x)) == 1)
                        {
                            cj.yMotion = ConfigurableJointMotion.Limited;
                        }
                        else if (Mathf.RoundToInt(Mathf.Abs(transform.forward.x)) == 1)
                        {
                            cj.zMotion = ConfigurableJointMotion.Limited;
                        }
                        break;
                    case ButtonDirection.y:
                    case ButtonDirection.negY:
                        if (Mathf.RoundToInt(Mathf.Abs(transform.right.y)) == 1)
                        {
                            cj.xMotion = ConfigurableJointMotion.Limited;
                        }
                        else if (Mathf.RoundToInt(Mathf.Abs(transform.up.y)) == 1)
                        {
                            cj.yMotion = ConfigurableJointMotion.Limited;
                        }
                        else if (Mathf.RoundToInt(Mathf.Abs(transform.forward.y)) == 1)
                        {
                            cj.zMotion = ConfigurableJointMotion.Limited;
                        }
                        break;
                    case ButtonDirection.z:
                    case ButtonDirection.negZ:
                        if (Mathf.RoundToInt(Mathf.Abs(transform.right.z)) == 1)
                        {
                            cj.xMotion = ConfigurableJointMotion.Limited;
                        }
                        else if (Mathf.RoundToInt(Mathf.Abs(transform.up.z)) == 1)
                        {
                            cj.yMotion = ConfigurableJointMotion.Limited;
                        }
                        else if (Mathf.RoundToInt(Mathf.Abs(transform.forward.z)) == 1)
                        {
                            cj.zMotion = ConfigurableJointMotion.Limited;
                        }
                        break;
                }
            }

            return true;
        }

        protected override ControlValueRange RegisterValueRange()
        {
            return new ControlValueRange() { controlMin = 0, controlMax = 1 };
        }

        protected override void HandleUpdate()
        {
            // trigger events
            float oldState = value;
            if (ReachedActivationDistance())
            {
                if (oldState == 0)
                {
                    value = 1;
                    events.OnPush.Invoke();
                }
            }
            else
            {
                value = 0;
            }
        }

        protected virtual void FixedUpdate()
        {
            // update reference position if no force is acting on the button to support scenarios where the button is moved at runtime with a connected body
            if (forceCount == 0 && cj.connectedBody)
            {
                restingPosition = transform.position;
            }
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            // TODO: this will not always be triggered for some reason, we probably need some "healing"
            forceCount -= 1;
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            forceCount += 1;
        }

        private ButtonDirection DetectDirection()
        {
            ButtonDirection direction = ButtonDirection.autodetect;
            Bounds bounds = VRTK_SharedMethods.GetBounds(transform);

            // shoot rays from the center of the button to learn about surroundings
            RaycastHit hitForward;
            RaycastHit hitBack;
            RaycastHit hitLeft;
            RaycastHit hitRight;
            RaycastHit hitUp;
            RaycastHit hitDown;
            Physics.Raycast(bounds.center, Vector3.forward, out hitForward, bounds.extents.z * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.back, out hitBack, bounds.extents.z * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.left, out hitLeft, bounds.extents.x * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.right, out hitRight, bounds.extents.x * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.up, out hitUp, bounds.extents.y * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.down, out hitDown, bounds.extents.y * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);

            // shortest valid ray wins
            float lengthX = (hitRight.collider != null) ? hitRight.distance : float.MaxValue;
            float lengthY = (hitDown.collider != null) ? hitDown.distance : float.MaxValue;
            float lengthZ = (hitBack.collider != null) ? hitBack.distance : float.MaxValue;
            float lengthNegX = (hitLeft.collider != null) ? hitLeft.distance : float.MaxValue;
            float lengthNegY = (hitUp.collider != null) ? hitUp.distance : float.MaxValue;
            float lengthNegZ = (hitForward.collider != null) ? hitForward.distance : float.MaxValue;

            float extents = 0;
            Vector3 hitPoint = Vector3.zero;
            if (VRTK_SharedMethods.IsLowest(lengthX, new float[] { lengthY, lengthZ, lengthNegX, lengthNegY, lengthNegZ }))
            {
                direction = ButtonDirection.negX;
                hitPoint = hitRight.point;
                extents = bounds.extents.x;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthY, new float[] { lengthX, lengthZ, lengthNegX, lengthNegY, lengthNegZ }))
            {
                direction = ButtonDirection.y;
                hitPoint = hitDown.point;
                extents = bounds.extents.y;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthZ, new float[] { lengthX, lengthY, lengthNegX, lengthNegY, lengthNegZ }))
            {
                direction = ButtonDirection.z;
                hitPoint = hitBack.point;
                extents = bounds.extents.z;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthNegX, new float[] { lengthX, lengthY, lengthZ, lengthNegY, lengthNegZ }))
            {
                direction = ButtonDirection.x;
                hitPoint = hitLeft.point;
                extents = bounds.extents.x;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthNegY, new float[] { lengthX, lengthY, lengthZ, lengthNegX, lengthNegZ }))
            {
                direction = ButtonDirection.negY;
                hitPoint = hitUp.point;
                extents = bounds.extents.y;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthNegZ, new float[] { lengthX, lengthY, lengthZ, lengthNegX, lengthNegY }))
            {
                direction = ButtonDirection.negZ;
                hitPoint = hitForward.point;
                extents = bounds.extents.z;
            }

            // determin activation distance
            activationDistance = (Vector3.Distance(hitPoint, bounds.center) - extents) * 0.95f;

            if (direction == ButtonDirection.autodetect || activationDistance < 0.001f)
            {
                // auto-detection was not possible or colliding with object already
                direction = ButtonDirection.autodetect;
                activationDistance = 0;
            }
            else
            {
                activationDir = hitPoint - bounds.center;
            }

            return direction;
        }

        private Vector3 CalculateActivationDir()
        {
            Bounds bounds = VRTK_SharedMethods.GetBounds(transform, transform);

            Vector3 buttonDirection = Vector3.zero;
            float extents = 0;
            switch (direction)
            {
                case ButtonDirection.x:
                case ButtonDirection.negX:
                    if (Mathf.RoundToInt(Mathf.Abs(transform.right.x)) == 1)
                    {
                        buttonDirection = transform.right;
                        extents = bounds.extents.x;
                    }
                    else if (Mathf.RoundToInt(Mathf.Abs(transform.up.x)) == 1)
                    {
                        buttonDirection = transform.up;
                        extents = bounds.extents.y;
                    }
                    else if (Mathf.RoundToInt(Mathf.Abs(transform.forward.x)) == 1)
                    {
                        buttonDirection = transform.forward;
                        extents = bounds.extents.z;
                    }
                    buttonDirection *= (direction == ButtonDirection.x) ? -1 : 1;
                    break;
                case ButtonDirection.y:
                case ButtonDirection.negY:
                    if (Mathf.RoundToInt(Mathf.Abs(transform.right.y)) == 1)
                    {
                        buttonDirection = transform.right;
                        extents = bounds.extents.x;
                    }
                    else if (Mathf.RoundToInt(Mathf.Abs(transform.up.y)) == 1)
                    {
                        buttonDirection = transform.up;
                        extents = bounds.extents.y;
                    }
                    else if (Mathf.RoundToInt(Mathf.Abs(transform.forward.y)) == 1)
                    {
                        buttonDirection = transform.forward;
                        extents = bounds.extents.z;
                    }
                    buttonDirection *= (direction == ButtonDirection.y) ? -1 : 1;
                    break;
                case ButtonDirection.z:
                case ButtonDirection.negZ:
                    if (Mathf.RoundToInt(Mathf.Abs(transform.right.z)) == 1)
                    {
                        buttonDirection = transform.right;
                        extents = bounds.extents.x;
                    }
                    else if (Mathf.RoundToInt(Mathf.Abs(transform.up.z)) == 1)
                    {
                        buttonDirection = transform.up;
                        extents = bounds.extents.y;
                    }
                    else if (Mathf.RoundToInt(Mathf.Abs(transform.forward.z)) == 1)
                    {
                        buttonDirection = transform.forward;
                        extents = bounds.extents.z;
                    }
                    buttonDirection *= (direction == ButtonDirection.z) ? -1 : 1;
                    break;
            }

            // subtract width of button
            return buttonDirection * (extents + activationDistance);
        }

        private bool ReachedActivationDistance()
        {
            return Vector3.Distance(transform.position, restingPosition) >= activationDistance;
        }

        private Vector3 GetForceVector()
        {
            return -activationDir.normalized * buttonStrength;
        }
    }
}