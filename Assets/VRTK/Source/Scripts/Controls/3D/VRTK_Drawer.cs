// Drawer|Controls3D|100050
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
    [AddComponentMenu("VRTK/Scripts/Controls/3D/VRTK_Drawer")]
    [System.Obsolete("`VRTK.VRTK_Drawer` has been deprecated and can be recreated with `VRTK.Controllables.PhysicsBased.VRTK_PhysicsSlider`. This script will be removed in a future version of VRTK.")]
    public class VRTK_Drawer : VRTK_Control
    {
        [Tooltip("An optional game object to which the drawer will be connected. If the game object moves the drawer will follow along.")]
        public GameObject connectedTo;
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
        [Tooltip("If the extension of the drawer is below this percentage then the drawer will snap shut.")]
        [Range(0, 1)]
        public float minSnapClose = 1;
        [Tooltip("The maximum percentage of the drawer's total length that the drawer will open to.")]
        [Range(0f, 1f)]
        public float maxExtend = 1f;

        protected Rigidbody drawerRigidbody;
        protected Rigidbody handleRigidbody;
        protected FixedJoint handleFixedJoint;
        protected ConfigurableJoint drawerJoint;
        protected VRTK_InteractableObject drawerInteractableObject;
        protected ConstantForce drawerSnapForce;
        protected Direction finalDirection;
        protected float subDirection = 1; // positive or negative can be determined automatically since handle dictates that
        protected float pullDistance = 0f;
        protected Vector3 initialPosition;
        protected bool drawerJointCreated = false;
        protected bool drawerSnapForceCreated = false;

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (!enabled || !setupSuccessful)
            {
                return;
            }

            // show opening direction
            Bounds handleBounds = VRTK_SharedMethods.GetBounds(GetHandle().transform, GetHandle().transform);
            float length = handleBounds.extents.y * ((handle) ? 5f : 1f);
            Vector3 point = handleBounds.center;
            switch (finalDirection)
            {
                case Direction.x:
                    point -= transform.right.normalized * (length * subDirection);
                    break;
                case Direction.y:
                    point -= transform.up.normalized * (length * subDirection);
                    break;
                case Direction.z:
                    point -= transform.forward.normalized * (length * subDirection);
                    break;
            }

            Gizmos.DrawLine(handleBounds.center, point);
            Gizmos.DrawSphere(point, length / 4f);
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
            finalDirection = (direction == Direction.autodetect ? DetectDirection() : direction);
            if (finalDirection == Direction.autodetect)
            {
                return false;
            }

            // determin sub-direction depending on handle
            Bounds handleBounds = VRTK_SharedMethods.GetBounds(GetHandle().transform, transform);
            Bounds bodyBounds = VRTK_SharedMethods.GetBounds(GetBody().transform, transform);

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

            if (drawerJointCreated)
            {
                drawerJoint.xMotion = ConfigurableJointMotion.Locked;
                drawerJoint.yMotion = ConfigurableJointMotion.Locked;
                drawerJoint.zMotion = ConfigurableJointMotion.Locked;

                switch (finalDirection)
                {
                    case Direction.x:
                        drawerJoint.axis = Vector3.right;
                        drawerJoint.xMotion = ConfigurableJointMotion.Limited;
                        break;
                    case Direction.y:
                        drawerJoint.axis = Vector3.up;
                        drawerJoint.yMotion = ConfigurableJointMotion.Limited;
                        break;
                    case Direction.z:
                        drawerJoint.axis = Vector3.forward;
                        drawerJoint.zMotion = ConfigurableJointMotion.Limited;
                        break;
                }
                drawerJoint.anchor = drawerJoint.axis * (-subDirection * pullDistance);
            }
            if (drawerJoint)
            {
                drawerJoint.angularXMotion = ConfigurableJointMotion.Locked;
                drawerJoint.angularYMotion = ConfigurableJointMotion.Locked;
                drawerJoint.angularZMotion = ConfigurableJointMotion.Locked;

                pullDistance *= (maxExtend * 1.8f); // don't let it pull out completely

                SoftJointLimit drawerJointLimit = drawerJoint.linearLimit;
                drawerJointLimit.limit = pullDistance;
                drawerJoint.linearLimit = drawerJointLimit;

                if (connectedTo)
                {
                    drawerJoint.connectedBody = connectedTo.GetComponent<Rigidbody>();
                }
            }
            if (drawerSnapForceCreated)
            {
                drawerSnapForce.force = transform.TransformDirection(drawerJoint.axis) * (subDirection * 50f);
            }

            return true;
        }

        protected override ControlValueRange RegisterValueRange()
        {
            return new ControlValueRange()
            {
                controlMin = 0,
                controlMax = 100
            };
        }

        protected override void HandleUpdate()
        {
            value = CalculateValue();
            bool withinSnapLimit = (Mathf.Abs(value) < minSnapClose * 100f);
            drawerSnapForce.enabled = withinSnapLimit;
            if (autoTriggerVolume)
            {
                autoTriggerVolume.isEnabled = !withinSnapLimit;
            }
        }

        protected virtual void InitBody()
        {
            drawerRigidbody = GetComponent<Rigidbody>();
            if (drawerRigidbody == null)
            {
                drawerRigidbody = gameObject.AddComponent<Rigidbody>();
                drawerRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
            drawerRigidbody.isKinematic = false;

            drawerInteractableObject = GetComponent<VRTK_InteractableObject>();
            if (drawerInteractableObject == null)
            {
                drawerInteractableObject = gameObject.AddComponent<VRTK_InteractableObject>();
            }

            drawerInteractableObject.isGrabbable = true;
            drawerInteractableObject.grabAttachMechanicScript = gameObject.AddComponent<GrabAttachMechanics.VRTK_SpringJointGrabAttach>();
            drawerInteractableObject.grabAttachMechanicScript.precisionGrab = true;
            drawerInteractableObject.secondaryGrabActionScript = gameObject.AddComponent<SecondaryControllerGrabActions.VRTK_SwapControllerGrabAction>();
            drawerInteractableObject.stayGrabbedOnTeleport = false;

            if (connectedTo)
            {
                Rigidbody drawerConnectedToRigidbody = connectedTo.GetComponent<Rigidbody>();
                if (drawerConnectedToRigidbody == null)
                {
                    drawerConnectedToRigidbody = connectedTo.AddComponent<Rigidbody>();
                    drawerConnectedToRigidbody.useGravity = false;
                    drawerConnectedToRigidbody.isKinematic = true;
                }
            }

            drawerJoint = GetComponent<ConfigurableJoint>();
            if (drawerJoint == null)
            {
                drawerJoint = gameObject.AddComponent<ConfigurableJoint>();
                drawerJointCreated = true;
            }

            drawerSnapForce = GetComponent<ConstantForce>();
            if (drawerSnapForce == null)
            {
                drawerSnapForce = gameObject.AddComponent<ConstantForce>();
                drawerSnapForce.enabled = false;
                drawerSnapForceCreated = true;
            }
        }

        protected virtual void InitHandle()
        {
            handleRigidbody = GetHandle().GetComponent<Rigidbody>();
            if (handleRigidbody == null)
            {
                handleRigidbody = GetHandle().AddComponent<Rigidbody>();
            }
            handleRigidbody.isKinematic = false;
            handleRigidbody.useGravity = false;

            handleFixedJoint = GetHandle().GetComponent<FixedJoint>();
            if (handleFixedJoint == null)
            {
                handleFixedJoint = GetHandle().AddComponent<FixedJoint>();
                handleFixedJoint.connectedBody = drawerRigidbody;
            }
        }

        protected virtual Direction DetectDirection()
        {
            Direction returnDirection = Direction.autodetect;

            Bounds handleBounds = VRTK_SharedMethods.GetBounds(GetHandle().transform, transform);
            Bounds bodyBounds = VRTK_SharedMethods.GetBounds(GetBody().transform, transform);

            float lengthX = Mathf.Abs(handleBounds.center.x - (bodyBounds.center.x + bodyBounds.extents.x));
            float lengthY = Mathf.Abs(handleBounds.center.y - (bodyBounds.center.y + bodyBounds.extents.y));
            float lengthZ = Mathf.Abs(handleBounds.center.z - (bodyBounds.center.z + bodyBounds.extents.z));
            float lengthNegX = Mathf.Abs(handleBounds.center.x - (bodyBounds.center.x - bodyBounds.extents.x));
            float lengthNegY = Mathf.Abs(handleBounds.center.y - (bodyBounds.center.y - bodyBounds.extents.y));
            float lengthNegZ = Mathf.Abs(handleBounds.center.z - (bodyBounds.center.z - bodyBounds.extents.z));

            if (VRTK_SharedMethods.IsLowest(lengthX, new float[] { lengthY, lengthZ, lengthNegX, lengthNegY, lengthNegZ }))
            {
                returnDirection = Direction.x;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthNegX, new float[] { lengthX, lengthY, lengthZ, lengthNegY, lengthNegZ }))
            {
                returnDirection = Direction.x;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthY, new float[] { lengthX, lengthZ, lengthNegX, lengthNegY, lengthNegZ }))
            {
                returnDirection = Direction.y;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthNegY, new float[] { lengthX, lengthY, lengthZ, lengthNegX, lengthNegZ }))
            {
                returnDirection = Direction.y;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthZ, new float[] { lengthX, lengthY, lengthNegX, lengthNegY, lengthNegZ }))
            {
                returnDirection = Direction.z;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthNegZ, new float[] { lengthX, lengthY, lengthZ, lengthNegY, lengthNegX }))
            {
                returnDirection = Direction.z;
            }

            return returnDirection;
        }

        protected virtual float CalculateValue()
        {
            return (Mathf.Round((transform.position - initialPosition).magnitude / pullDistance * 100));
        }

        protected virtual GameObject GetBody()
        {
            return (body ? body : gameObject);
        }

        protected virtual GameObject GetHandle()
        {
            return (handle ? handle : gameObject);
        }
    }
}