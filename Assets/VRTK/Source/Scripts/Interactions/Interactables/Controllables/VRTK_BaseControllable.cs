// Base Controllable|Controllables|101010
namespace VRTK.Controllables
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="interactingObject">The GameObject that is performing the interaction (e.g. a controller).</param>
    /// <param name="value">The current value being reported by the controllable.</param>
    /// <param name="normalizedValue">The normalized value being reported by the controllable.</param>
    public struct ControllableEventArgs
    {
        public GameObject interactingObject;
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

        /// <summary>
        /// Emitted when the Controllable value has changed.
        /// </summary>
        public event ControllableEventHandler ValueChanged;
        /// <summary>
        /// Emitted when the Controllable value has reached it's minimum limit.
        /// </summary>
        public event ControllableEventHandler MinLimitReached;
        /// <summary>
        /// Emitted when the Controllable value has exited it's minimum limit.
        /// </summary>
        public event ControllableEventHandler MinLimitExited;
        /// <summary>
        /// Emitted when the Controllable value has reached it's maximum limit.
        /// </summary>
        public event ControllableEventHandler MaxLimitReached;
        /// <summary>
        /// Emitted when the Controllable value has exited it's maximum limit.
        /// </summary>
        public event ControllableEventHandler MaxLimitExited;

        protected Vector3 originalLocalPosition;
        protected Quaternion originalLocalRotation;
        protected bool atMinLimit;
        protected bool atMaxLimit;
        protected GameObject interactingObject;
        protected Collider[] controlColliders;
        protected bool createCustomCollider;
        protected Coroutine disableColliderRoutine;

        public virtual void OnValueChanged(ControllableEventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
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

        protected virtual void OnEnable()
        {
            atMinLimit = false;
            atMaxLimit = false;
            originalLocalPosition = transform.localPosition;
            originalLocalRotation = transform.localRotation;
            SetupCollider();
            disableColliderRoutine = StartCoroutine(DisableCollidersAtEndOfFrame());
        }

        protected virtual void OnDisable()
        {
            if (disableColliderRoutine != null)
            {
                StopCoroutine(disableColliderRoutine);
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
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            interactingObject = collision.gameObject;
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            interactingObject = null;
        }

        protected virtual void SetupCollider()
        {
            controlColliders = GetComponentsInChildren<Collider>();
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

        protected virtual IEnumerator DisableCollidersAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            ManageCollisions(true);
            disableColliderRoutine = null;
        }

        protected virtual void ManageCollisions(bool ignore)
        {
            for (int ignoredGameObjectColliderIndex = 0; ignoredGameObjectColliderIndex < ignoreCollisionsWith.Length; ignoredGameObjectColliderIndex++)
            {
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
            e.interactingObject = interactingObject;
            e.value = GetValue();
            e.normalizedValue = GetNormalizedValue();
            return e;
        }
    }
}