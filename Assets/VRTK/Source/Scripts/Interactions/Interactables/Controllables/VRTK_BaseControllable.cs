// Base Controllable|Controllables|101010
namespace VRTK.Controllables
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="interactingCollider">The Collider that is initiating the interaction.</param>
    /// <param name="interactingTouchScript">The optional Interact Touch script that is initiating the interaction.</param>
    /// <param name="value">The current value being reported by the controllable.</param>
    /// <param name="normalizedValue">The normalized value being reported by the controllable.</param>
    public struct ControllableEventArgs
    {
        public Collider interactingCollider;
        public VRTK_InteractTouch interactingTouchScript;
        public float value;
        public float normalizedValue;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="ControllableEventArgs"/></param>
    public delegate void ControllableEventHandler(object sender, ControllableEventArgs e);

    /// <summary>
    /// Provides a base that all Controllables can inherit from.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///   > This is an abstract class that is to be inherited to a concrete class that provides controllable functionality, therefore this script should not be directly used.
    /// </remarks>
    public abstract class VRTK_BaseControllable : MonoBehaviour
    {
        /// <summary>
        /// The local axis that the Controllable will be operated through.
        /// </summary>
        public enum OperatingAxis
        {
            /// <summary>
            /// The local x axis.
            /// </summary>
            xAxis,
            /// <summary>
            /// The local y axis.
            /// </summary>
            yAxis,
            /// <summary>
            /// The local z axis.
            /// </summary>
            zAxis
        }

        [Header("Controllable Settings")]

        [Tooltip("The local axis in which the Controllable will operate through.")]
        public OperatingAxis operateAxis = OperatingAxis.yAxis;
        [Tooltip("A collection of GameObjects to ignore collision events with.")]
        public GameObject[] ignoreCollisionsWith = new GameObject[0];
        [Tooltip("A collection of GameObjects to exclude when determining if a default collider should be created.")]
        public GameObject[] excludeColliderCheckOn = new GameObject[0];
        [Tooltip("The amount of fidelity when comparing the position of the control with the previous position. Determines if it's equal above a certain decimal place threshold.")]
        public float equalityFidelity = 0.001f;

        /// <summary>
        /// Emitted when the Controllable value has changed.
        /// </summary>
        public event ControllableEventHandler ValueChanged;
        /// <summary>
        /// Emitted when the Controllable value has reached the resting point.
        /// </summary>
        public event ControllableEventHandler RestingPointReached;
        /// <summary>
        /// Emitted when the Controllable value has reached the minimum limit.
        /// </summary>
        public event ControllableEventHandler MinLimitReached;
        /// <summary>
        /// Emitted when the Controllable value has exited the minimum limit.
        /// </summary>
        public event ControllableEventHandler MinLimitExited;
        /// <summary>
        /// Emitted when the Controllable value has reached the maximum limit.
        /// </summary>
        public event ControllableEventHandler MaxLimitReached;
        /// <summary>
        /// Emitted when the Controllable value has exited the maximum limit.
        /// </summary>
        public event ControllableEventHandler MaxLimitExited;

        protected Vector3 originalLocalPosition;
        protected Quaternion originalLocalRotation;
        protected Vector3 actualTransformPosition;
        protected bool atMinLimit;
        protected bool atMaxLimit;
        protected Collider interactingCollider;
        protected VRTK_InteractTouch interactingTouchScript;
        protected Collider[] controlColliders = new Collider[0];
        protected bool createCustomCollider;
        protected Coroutine processAtEndOfFrame;

        public virtual void OnValueChanged(ControllableEventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        public virtual void OnRestingPointReached(ControllableEventArgs e)
        {
            if (RestingPointReached != null)
            {
                RestingPointReached(this, e);
            }
        }

        public virtual void OnMinLimitReached(ControllableEventArgs e)
        {
            if (MinLimitReached != null)
            {
                MinLimitReached(this, e);
            }
        }

        public virtual void OnMinLimitExited(ControllableEventArgs e)
        {
            if (MinLimitExited != null)
            {
                MinLimitExited(this, e);
            }
        }

        public virtual void OnMaxLimitReached(ControllableEventArgs e)
        {
            if (MaxLimitReached != null)
            {
                MaxLimitReached(this, e);
            }
        }

        public virtual void OnMaxLimitExited(ControllableEventArgs e)
        {
            if (MaxLimitExited != null)
            {
                MaxLimitExited(this, e);
            }
        }

        public abstract float GetValue();
        public abstract float GetNormalizedValue();
        public abstract bool IsResting();

        /// <summary>
        /// The AtMinLimit method returns whether the Controllable is currently at it's minimum limit.
        /// </summary>
        /// <returns>Returns `true` if the Controllable is at it's minimum limit.</returns>
        public virtual bool AtMinLimit()
        {
            return atMinLimit;
        }

        /// <summary>
        /// The AtMaxLimit method returns whether the Controllable is currently at it's maximum limit.
        /// </summary>
        /// <returns>Returns `true` if the Controllable is at it's maximum limit.</returns>
        public virtual bool AtMaxLimit()
        {
            return atMaxLimit;
        }

        /// <summary>
        /// The GetOriginalLocalPosition method returns the original local position of the control.
        /// </summary>
        /// <returns>A Vector3 of the original local position.</returns>
        public virtual Vector3 GetOriginalLocalPosition()
        {
            return originalLocalPosition;
        }

        /// <summary>
        /// The GetOriginalLocalRotation method returns the original local rotation of the control.
        /// </summary>
        /// <returns>A quaternion of the original local rotation.</returns>
        public virtual Quaternion GetOriginalLocalRotation()
        {
            return originalLocalRotation;
        }

        /// <summary>
        /// The GetControlColliders method returns the Colliders array associated with the control.
        /// </summary>
        /// <returns>The Colliders array associated with the control.</returns>
        public virtual Collider[] GetControlColliders()
        {
            return controlColliders;
        }

        /// <summary>
        /// The GetInteractingCollider method returns the Collider of the GameObject currently interacting with the control.
        /// </summary>
        /// <returns>The Collider currently interacting with the control.</returns>
        public virtual Collider GetInteractingCollider()
        {
            return interactingCollider;
        }

        /// <summary>
        /// The GetInteractingTouch method returns the Interact Touch script of the GameObject currently interacting with the control.
        /// </summary>
        /// <returns>The Interact Touch script currently interacting with the control.</returns>
        public virtual VRTK_InteractTouch GetInteractingTouch()
        {
            return interactingTouchScript;
        }

        protected abstract void EmitEvents();

        protected virtual void Awake()
        {
            originalLocalPosition = transform.localPosition;
            originalLocalRotation = transform.localRotation;
        }

        protected virtual void OnEnable()
        {
            atMinLimit = false;
            atMaxLimit = false;
            SetupCollider();
            processAtEndOfFrame = StartCoroutine(ProcessAtEndOfFrame());
        }

        protected virtual void OnDisable()
        {
            if (processAtEndOfFrame != null)
            {
                StopCoroutine(processAtEndOfFrame);
            }
            ManageCollisions(false);
            if (createCustomCollider)
            {
                for (int i = 0; i < controlColliders.Length; i++)
                {
                    Destroy(controlColliders[i]);
                }
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            actualTransformPosition = transform.position;
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            OnTouched(collision.collider);
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            OnUntouched(collision.collider);
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            OnTouched(collider);
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            OnUntouched(collider);
        }

        protected virtual void OnTouched(Collider collider)
        {
            interactingCollider = collider;
            interactingTouchScript = interactingCollider.GetComponentInParent<VRTK_InteractTouch>();
        }

        protected virtual void OnUntouched(Collider collider)
        {
            interactingCollider = null;
            interactingTouchScript = null;
        }

        protected virtual void SetupCollider()
        {
            controlColliders = VRTK_SharedMethods.ColliderExclude(GetComponentsInChildren<Collider>(), VRTK_SharedMethods.GetCollidersInGameObjects(excludeColliderCheckOn, true, true));
            createCustomCollider = false;
            if (controlColliders.Length == 0)
            {
                controlColliders = new Collider[1] { gameObject.AddComponent<BoxCollider>() };
                createCustomCollider = true;
                ConfigureColliders();
            }
        }

        protected virtual void ConfigureColliders()
        {
        }

        protected virtual IEnumerator ProcessAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            ManageCollisions(true);
            EmitEvents();
            processAtEndOfFrame = null;
        }

        protected virtual void ManageCollisions(bool ignore)
        {
            for (int ignoredGameObjectColliderIndex = 0; ignoredGameObjectColliderIndex < ignoreCollisionsWith.Length; ignoredGameObjectColliderIndex++)
            {
                if (ignoreCollisionsWith[ignoredGameObjectColliderIndex] == null)
                {
                    continue;
                }

                Collider[] ignoredGameObjectColliders = ignoreCollisionsWith[ignoredGameObjectColliderIndex].GetComponentsInChildren<Collider>();

                for (int ignoredColliderIndex = 0; ignoredColliderIndex < ignoredGameObjectColliders.Length; ignoredColliderIndex++)
                {
                    for (int controlColliderIndex = 0; controlColliderIndex < controlColliders.Length; controlColliderIndex++)
                    {
                        if (ignoredGameObjectColliders[ignoredColliderIndex] != null && controlColliders[controlColliderIndex] != null)
                        {
                            Physics.IgnoreCollision(controlColliders[controlColliderIndex], ignoredGameObjectColliders[ignoredColliderIndex], ignore);
                        }
                    }
                }
            }
        }

        protected virtual Vector3 AxisDirection(bool local = false)
        {
            return VRTK_SharedMethods.AxisDirection((int)operateAxis, (local ? transform : null));
        }

        protected virtual ControllableEventArgs EventPayload()
        {
            ControllableEventArgs e;
            e.interactingCollider = interactingCollider;
            e.interactingTouchScript = interactingTouchScript;
            e.value = GetValue();
            e.normalizedValue = GetNormalizedValue();
            return e;
        }
    }
}