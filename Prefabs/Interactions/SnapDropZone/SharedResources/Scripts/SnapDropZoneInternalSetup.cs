namespace VRTK.Prefabs.Interactions.SnapDropZone
{
    using UnityEngine;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.PropertyValidationMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Operation;
    using Zinnia.Tracking.Collision;
    using Zinnia.Tracking.Follow;
    using Zinnia.Tracking.Modification;

    /// <summary>
    /// Sets up the SnapDropZone prefab based on the provided user settings.
    /// </summary>
    public class SnapDropZoneInternalSetup : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Serialized, Validated, Cleared]
        [field: Header("Facade Settings"), InternalSetting, DocumentedByXml]
        public SnapDropZoneFacade Facade { get; private set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// Tracks objects entering and exiting the <see cref="SnapDropZoneFacade"/>.
        /// </summary>
        [Serialized, Validated, Cleared]
        [field: Header("Reference Settings"), InternalSetting, DocumentedByXml]
        public CollisionTracker CollisionTracker { get; private set; }
        /// <summary>
        /// Enables and disables snapped and unsnapped objects' <see cref="Rigidbody.isKinematic"/> state.
        /// </summary>
        [Serialized, Validated, Cleared]
        [field: InternalSetting, DocumentedByXml]
        public RigidbodyKinematicMutator RigidbodyKinematicMutator { get; private set; }
        /// <summary>
        /// Lerps <see cref="SnapDropZoneFacade.SnappedObject"/> to the configured target transform state.
        /// </summary>
        [Serialized, Validated, Cleared]
        [field: InternalSetting, DocumentedByXml]
        public TransformPropertyApplier Lerper { get; private set; }
        /// <summary>
        /// Ensures <see cref="SnapDropZoneFacade.SnappedObject"/> follows the <see cref="SnapDropZoneFacade"/>.
        /// </summary>
        [Serialized, Validated, Cleared]
        [field: InternalSetting, DocumentedByXml]
        public ObjectFollower ObjectFollower { get; private set; }
        /// <summary>
        /// Duplicates <see cref="SnapDropZoneFacade.SnappedObject"/> on raise of <see cref="SnapDropZoneFacade.Unsnapped"/>.
        /// </summary>
        [Serialized, Validated, Cleared]
        [field: InternalSetting, DocumentedByXml]
        public GameObjectCloner Cloner { get; private set; }
        #endregion

        /// <summary>
        /// Configures the internal setup for the snapped object.
        /// </summary>
        /// <param name="snappedObject">The snapped object.</param>
        public virtual void ConfigureForSnappedObject(GameObject snappedObject)
        {
            ConfigureCollisionTracker(snappedObject);
            ConfigureLerp(snappedObject);
            ConfigureFollow(snappedObject);
        }

        /// <summary>
        /// Configures <see cref="Cloner"/>.
        /// </summary>
        /// <param name="clonesOnUnsnap">Whether to clone the snapped object on raise of <see cref="SnapDropZoneFacade.Unsnapped"/>.</param>
        public virtual void ConfigureCloning(bool clonesOnUnsnap)
        {
            Cloner.enabled = clonesOnUnsnap;
        }

        protected virtual void OnEnable()
        {
            Cloner.enabled = Facade.ClonesOnUnsnap;
            Facade.Snapped.AddListener(OnSnapped);
            Facade.Unsnapped.AddListener(OnUnsnapped);
        }

        protected virtual void OnDisable()
        {
            Facade.Unsnapped.RemoveListener(OnUnsnapped);
            Facade.Snapped.RemoveListener(OnSnapped);
        }

        /// <summary>
        /// Handles an object getting snapped by <see cref="Facade"/>.
        /// </summary>
        /// <param name="snappedObject">The snapped object.</param>
        protected virtual void OnSnapped(GameObject snappedObject)
        {
            Rigidbody snappedRigidbody = snappedObject.GetComponent<Rigidbody>();
            if (snappedRigidbody != null)
            {
                RigidbodyKinematicMutator.Target = snappedRigidbody;
            }
        }

        /// <summary>
        /// Handles an object getting unsnapped from <see cref="Facade"/>.
        /// </summary>
        /// <param name="unsnappedObject">The unsnapped object.</param>
        protected virtual void OnUnsnapped(GameObject unsnappedObject)
        {
            if (RigidbodyKinematicMutator != null)
            {
                RigidbodyKinematicMutator.Target = null;
            }

            if (Cloner == null)
            {
                return;
            }

            GameObject clone = Cloner.Clone(unsnappedObject);
            if (clone != null && RigidbodyKinematicMutator != null)
            {
                RigidbodyKinematicMutator.Target = clone.GetComponent<Rigidbody>();
            }
        }

        /// <summary>
        /// Configures <see cref="CollisionTracker"/>.
        /// </summary>
        /// <param name="snappedObject">The snapped object.</param>
        protected virtual void ConfigureCollisionTracker(GameObject snappedObject)
        {
            if (CollisionTracker == null)
            {
                return;
            }

            CollisionTracker.enabled = snappedObject == null;
        }

        /// <summary>
        /// Configures <see cref="Lerper"/>.
        /// </summary>
        /// <param name="snappedObject">The snapped object.</param>
        protected virtual void ConfigureLerp(GameObject snappedObject)
        {
            if (Lerper == null)
            {
                return;
            }

            Lerper.enabled = false;
            Lerper.Target = snappedObject == null ? null : snappedObject;
            Lerper.enabled = true;
            Lerper.Apply();
        }

        /// <summary>
        /// Configures <see cref="ObjectFollower"/>.
        /// </summary>
        /// <param name="snappedObject">The snapped object.</param>
        protected virtual void ConfigureFollow(GameObject snappedObject)
        {
            if (ObjectFollower == null)
            {
                return;
            }

            ObjectFollower.ClearTargets();
            if (snappedObject != null)
            {
                ObjectFollower.AddTarget(snappedObject);
            }
        }
    }
}
