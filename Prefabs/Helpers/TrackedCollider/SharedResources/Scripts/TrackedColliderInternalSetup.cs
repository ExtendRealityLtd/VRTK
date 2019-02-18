namespace VRTK.Prefabs.Helpers.TrackedCollider
{
    using UnityEngine;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Operation;
    using Zinnia.Tracking.Follow;

    /// <summary>
    /// Sets up the TrackedCollider prefab based on the provided settings and implements the logic to follow the relevant source.
    /// </summary>
    public class TrackedColliderInternalSetup : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Header("Facade Settings"), Tooltip("The public interface facade."), InternalSetting, SerializeField]
        protected TrackedColliderFacade facade;
        #endregion

        #region Reference Settings
        /// <summary>
        /// The <see cref="ObjectFollower"/> that performs the source follow.
        /// </summary>
        [Header("Reference Settings"), Tooltip("The ObjectFollower that performs the source follow."), InternalSetting, SerializeField]
        protected ObjectFollower objectFollower;
        /// <summary>
        /// The <see cref="TransformPositionExtractor"/> that extracts the source position.
        /// </summary>
        [Tooltip("The TransformPositionExtractor that extracts the source position."), InternalSetting, SerializeField]
        protected TransformPositionExtractor positionExtractor;
        #endregion

        /// <summary>
        /// Sets the source on the relevant references.
        /// </summary>
        /// <param name="source">The source to set.</param>
        public virtual void SetSource(GameObject source)
        {
            objectFollower.AddSource(source);
            positionExtractor.source = source;
        }

        /// <summary>
        /// Snaps the tracked collider directly to the source current position.
        /// </summary>
        public virtual void SnapToSource()
        {
            positionExtractor.DoExtract();
        }

        protected virtual void OnEnable()
        {
            SetSource(facade.Source);
        }
    }
}