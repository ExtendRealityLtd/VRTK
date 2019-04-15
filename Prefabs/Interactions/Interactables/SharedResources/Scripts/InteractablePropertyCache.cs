namespace VRTK.Prefabs.Interactions.Interactables
{
    using UnityEngine;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.BehaviourStateRequirementMethod;

    /// <summary>
    /// Caches common properties for an <see cref="InteractableFacade"/> to be restored at a later point in time.
    /// </summary>
    public class InteractablePropertyCache : MonoBehaviour
    {
        /// <summary>
        /// The source to cache properties for.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public InteractableFacade Source { get; set; }

        /// <summary>
        /// The cached local position of the <see cref="Source"/>
        /// </summary>
        protected Vector3? cachedLocalPosition;
        /// <summary>
        /// The cached local rotation of the <see cref="Source"/>
        /// </summary>
        protected Quaternion cachedLocalRotation;
        /// <summary>
        /// The cached local scale of the <see cref="Source"/>
        /// </summary>
        protected Vector3 cachedLocalScale;
        /// <summary>
        /// The cached kinematic state of the the <see cref="Source"/>
        /// </summary>
        protected bool cachedRigidbodyKinematicState;

        /// <summary>
        /// Sets the <see cref="Source"/> property from a given <see cref="GameObject"/>.
        /// </summary>
        /// <param name="source">The source to set to.</param>
        [RequiresBehaviourState]
        public virtual void SetSource(GameObject source)
        {
            if (source == null)
            {
                return;
            }

            Source = source.GetComponent<InteractableFacade>();
        }

        /// <summary>
        /// Caches the position.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void CachePosition()
        {
            if (Source == null)
            {
                return;
            }

            cachedLocalPosition = Source.transform.localPosition;
        }

        /// <summary>
        /// Caches the rotation.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void CacheRotation()
        {
            if (Source == null)
            {
                return;
            }

            cachedLocalRotation = Source.transform.localRotation;
        }

        /// <summary>
        /// Caches the scale.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void CacheScale()
        {
            if (Source == null)
            {
                return;
            }

            cachedLocalScale = Source.transform.localScale;
        }

        /// <summary>
        /// Caches the rigidbody kinematic state..
        /// </summary>
        [RequiresBehaviourState]
        public virtual void CacheRigidbodyKinematicState()
        {
            if (Source == null)
            {
                return;
            }

            cachedRigidbodyKinematicState = Source.ConsumerRigidbody != null ? Source.ConsumerRigidbody.isKinematic : false;
        }

        /// <summary>
        /// Caches all of the properties.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void CacheAll()
        {
            CachePosition();
            CacheRotation();
            CacheScale();
            CacheRigidbodyKinematicState();
        }

        /// <summary>
        /// Restores the cached position.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void RestorePosition()
        {
            if (Source == null)
            {
                return;
            }

            Source.transform.localPosition = (Vector3)cachedLocalPosition;
        }

        /// <summary>
        /// Restores the cached rotation.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void RestoreRotation()
        {
            if (Source == null)
            {
                return;
            }

            Source.transform.localRotation = cachedLocalRotation;
        }

        /// <summary>
        /// Restores the cached scale.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void RestoreScale()
        {
            if (Source == null)
            {
                return;
            }

            Source.transform.localScale = cachedLocalScale;
        }

        /// <summary>
        /// Restores the cached rigidbody kinematic state.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void RestoreRigidbodyKinematicState()
        {
            if (Source == null)
            {
                return;
            }

            Source.ConsumerRigidbody.isKinematic = cachedRigidbodyKinematicState;
        }

        /// <summary>
        /// Restores the all of cached properties.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void RestoreAll()
        {
            RestorePosition();
            RestoreRotation();
            RestoreScale();
            RestoreRigidbodyKinematicState();
        }
    }
}