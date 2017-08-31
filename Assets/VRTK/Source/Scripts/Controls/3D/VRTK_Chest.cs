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
    [AddComponentMenu("VRTK/Scripts/Controls/3D/VRTK_Chest")]
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

        protected float minAngle = 0f;
        protected float stepSize = 1f;
        protected Rigidbody bodyRigidbody;
        protected Rigidbody handleRigidbody;
        protected FixedJoint handleJoint;
        protected Rigidbody lidRigidbody;
        protected HingeJoint lidJoint;
        protected bool lidJointCreated;
        protected Direction finalDirection;
        protected float subDirection = 1; // positive or negative can be determined automatically since handle dictates that

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
            if (lidJointCreated)
            {
                lidJoint.useLimits = true;
                lidJoint.enableCollision = true;

                JointLimits limits = lidJoint.limits;
                switch (finalDirection)
                {
                    case Direction.x:
                        lidJoint.anchor = new Vector3(subDirection * lidBounds.extents.x, 0, 0);
                        lidJoint.axis = new Vector3(0, 0, 1);
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
                        lidJoint.anchor = new Vector3(0, subDirection * lidBounds.extents.y, 0);
                        lidJoint.axis = new Vector3(0, 1, 0);
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
                        lidJoint.anchor = new Vector3(0, 0, subDirection * lidBounds.extents.z);
                        lidJoint.axis = new Vector3(1, 0, 0);
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
                lidJoint.limits = limits;
            }
            return true;
        }

        protected override ControlValueRange RegisterValueRange()
        {
            return new ControlValueRange()
            {
                controlMin = lidJoint.limits.min,
                controlMax = lidJoint.limits.max
            };
        }

        protected override void HandleUpdate()
        {
            value = CalculateValue();
        }

        protected virtual Direction DetectDirection()
        {
            Direction returnDirection = Direction.autodetect;

            if (!handle)
            {
                return returnDirection;
            }

            Bounds handleBounds = VRTK_SharedMethods.GetBounds(handle.transform, transform);
            Bounds lidBounds = VRTK_SharedMethods.GetBounds(lid.transform, transform);

            float lengthX = Mathf.Abs(handleBounds.center.x - (lidBounds.center.x + lidBounds.extents.x));
            float lengthZ = Mathf.Abs(handleBounds.center.z - (lidBounds.center.z + lidBounds.extents.z));
            float lengthNegX = Mathf.Abs(handleBounds.center.x - (lidBounds.center.x - lidBounds.extents.x));
            float lengthNegZ = Mathf.Abs(handleBounds.center.z - (lidBounds.center.z - lidBounds.extents.z));

            if (VRTK_SharedMethods.IsLowest(lengthX, new float[] { lengthZ, lengthNegX, lengthNegZ }))
            {
                returnDirection = Direction.x;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthNegX, new float[] { lengthX, lengthZ, lengthNegZ }))
            {
                returnDirection = Direction.x;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthZ, new float[] { lengthX, lengthNegX, lengthNegZ }))
            {
                returnDirection = Direction.z;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthNegZ, new float[] { lengthX, lengthZ, lengthNegX }))
            {
                returnDirection = Direction.z;
            }

            return returnDirection;
        }

        protected virtual void InitBody()
        {
            bodyRigidbody = body.GetComponent<Rigidbody>();
            if (bodyRigidbody == null)
            {
                bodyRigidbody = body.AddComponent<Rigidbody>();
                bodyRigidbody.isKinematic = true; // otherwise body moves/falls over when lid is moved or fully open
            }
        }

        protected virtual void InitLid()
        {
            lidRigidbody = lid.GetComponent<Rigidbody>();
            if (lidRigidbody == null)
            {
                lidRigidbody = lid.AddComponent<Rigidbody>();
            }

            lidJoint = lid.GetComponent<HingeJoint>();
            if (lidJoint == null)
            {
                lidJoint = lid.AddComponent<HingeJoint>();
                lidJointCreated = true;
            }
            lidJoint.connectedBody = bodyRigidbody;

            if (!handle)
            {
                CreateInteractableObject(lid);
            }
        }

        protected virtual void InitHandle()
        {
            if (!handle)
            {
                return;
            }
            handleRigidbody = handle.GetComponent<Rigidbody>();
            if (handleRigidbody == null)
            {
                handleRigidbody = handle.AddComponent<Rigidbody>();
            }
            handleRigidbody.isKinematic = false;
            handleRigidbody.useGravity = false;

            handleJoint = handle.GetComponent<FixedJoint>();
            if (handleJoint == null)
            {
                handleJoint = handle.AddComponent<FixedJoint>();
                handleJoint.connectedBody = lidRigidbody;
            }

            CreateInteractableObject(handle);
        }

        protected virtual void CreateInteractableObject(GameObject targetGameObject)
        {
            VRTK_InteractableObject targetInteractableObject = targetGameObject.GetComponent<VRTK_InteractableObject>();
            if (targetInteractableObject == null)
            {
                targetInteractableObject = targetGameObject.AddComponent<VRTK_InteractableObject>();
            }
            targetInteractableObject.isGrabbable = true;
            targetInteractableObject.grabAttachMechanicScript = gameObject.AddComponent<GrabAttachMechanics.VRTK_TrackObjectGrabAttach>();
            targetInteractableObject.secondaryGrabActionScript = gameObject.AddComponent<SecondaryControllerGrabActions.VRTK_SwapControllerGrabAction>();
            targetInteractableObject.grabAttachMechanicScript.precisionGrab = true;
            targetInteractableObject.stayGrabbedOnTeleport = false;
        }

        protected virtual float CalculateValue()
        {
            return (Mathf.Round((minAngle + Mathf.Clamp01(Mathf.Abs(lidJoint.angle / (lidJoint.limits.max - lidJoint.limits.min))) * (maxAngle - minAngle)) / stepSize) * stepSize);
        }
    }
}