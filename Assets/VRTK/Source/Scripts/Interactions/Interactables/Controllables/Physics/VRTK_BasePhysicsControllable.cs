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
            controlRigidbody.isKinematic = false;
        }

        protected virtual void ConfigueRigidbody()
        {
        }
    }
}