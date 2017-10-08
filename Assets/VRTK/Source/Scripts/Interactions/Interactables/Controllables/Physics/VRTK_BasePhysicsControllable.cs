// Base Physics Controllable|PhysicsControllables|110010
namespace VRTK.Controllables.PhysicsBased
{
    using UnityEngine;

    /// <summary>
    /// Provides a base that all physics based Controllables can inherit from.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///   > This is an abstract class that is to be inherited to a concrete class that provides physics based controllable functionality, therefore this script should not be directly used.
    /// </remarks>
    public abstract class VRTK_BasePhysicsControllable : VRTK_BaseControllable
    {
        [Header("Physics Settings")]

        [Tooltip("If this is checked then a VRTK_ControllerRigidbodyActivator will automatically be added to the Controllable so the interacting object's rigidbody is enabled on touch.")]
        public bool autoInteraction = true;
        [Tooltip("The Rigidbody that the Controllable is connected to.")]
        public Rigidbody connectedTo;

        protected Rigidbody controlRigidbody;
        protected bool createCustomRigidbody;
        protected GameObject rigidbodyActivatorContainer;
        protected bool createCustomRigidbodyActivator;

        /// <summary>
        /// The GetControlRigidbody method returns the rigidbody associated with the control.
        /// </summary>
        /// <returns>The Rigidbody associated with the control.</returns>
        public virtual Rigidbody GetControlRigidbody()
        {
            return controlRigidbody;
        }

        /// <summary>
        /// The GetControlActivatorContainer method returns the GameObject that contains the Controller Rigidbody Activator associated with the control.
        /// </summary>
        /// <returns>The GameObject that contains the Controller Rigidbody Activator associated with the control.</returns>
        public virtual GameObject GetControlActivatorContainer()
        {
            return rigidbodyActivatorContainer;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetupRigidbody();
            SetupRigidbodyActivator();
        }

        protected override void OnDisable()
        {
            if (createCustomRigidbodyActivator && rigidbodyActivatorContainer != null)
            {
                Destroy(rigidbodyActivatorContainer);
            }
            if (createCustomRigidbody)
            {
                Destroy(controlRigidbody);
            }
            base.OnDisable();
        }

        protected virtual void SetupRigidbodyActivator()
        {
            createCustomRigidbodyActivator = false;
            if (GetComponentInChildren<VRTK_ControllerRigidbodyActivator>() == null && autoInteraction)
            {
                rigidbodyActivatorContainer = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, name, "Controllable", "PhysicsBased", "ControllerRigidbodyActivator"));
                rigidbodyActivatorContainer.transform.SetParent(transform);
                rigidbodyActivatorContainer.transform.localPosition = Vector3.zero;
                rigidbodyActivatorContainer.transform.localRotation = Quaternion.identity;
                rigidbodyActivatorContainer.transform.localScale = Vector3.one;
                rigidbodyActivatorContainer.AddComponent<VRTK_ControllerRigidbodyActivator>();
                BoxCollider rigidbodyActivatorCollider = rigidbodyActivatorContainer.AddComponent<BoxCollider>();
                rigidbodyActivatorCollider.isTrigger = true;
                rigidbodyActivatorCollider.size *= 1.2f;
                createCustomRigidbodyActivator = true;
            }
        }

        protected virtual void SetupRigidbody()
        {
            controlRigidbody = GetComponent<Rigidbody>();
            createCustomRigidbody = false;
            if (controlRigidbody == null)
            {
                controlRigidbody = gameObject.AddComponent<Rigidbody>();
                createCustomRigidbody = true;
                ConfigueRigidbody();
            }
            SetRigidbodyKinematic(false);
        }

        protected virtual void ConfigueRigidbody()
        {
        }

        protected virtual void SetRigidbodyVelocity(Vector3 newVelocity)
        {
            if (controlRigidbody != null)
            {
                controlRigidbody.velocity = newVelocity;
            }
        }

        protected virtual void SetRigidbodyDrag(float givenDrag)
        {
            if (controlRigidbody != null)
            {
                controlRigidbody.drag = givenDrag;
            }
        }

        protected virtual void SetRigidbodyAngularDrag(float givenDrag)
        {
            if (controlRigidbody != null)
            {
                controlRigidbody.angularDrag = givenDrag;
            }
        }

        protected virtual void SetRigidbodyGravity(bool useGravity)
        {
            if (controlRigidbody != null)
            {
                controlRigidbody.useGravity = useGravity;
            }
        }

        protected virtual void SetRigidbodyKinematic(bool isKinematic)
        {
            if (controlRigidbody != null)
            {
                controlRigidbody.isKinematic = isKinematic;
            }
        }

        protected virtual void SetRigidbodyConstraints(RigidbodyConstraints newConstraints)
        {
            if (controlRigidbody != null)
            {
                controlRigidbody.constraints = newConstraints;
            }
        }

        protected virtual void SetRigidbodyCollisionDetectionMode(CollisionDetectionMode newDetectionMode)
        {
            if (controlRigidbody != null)
            {
                controlRigidbody.collisionDetectionMode = newDetectionMode;
            }
        }
    }
}