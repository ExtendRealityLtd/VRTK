namespace VRTK.Prefabs.Helpers.TrackedCollider
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Extension;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Follow;
    using Zinnia.Data.Operation.Extraction;

    /// <summary>
    /// Sets up the TrackedCollider prefab based on the provided settings and implements the logic to follow the relevant source.
    /// </summary>
    public class TrackedColliderConfigurator : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public TrackedColliderFacade Facade { get; protected set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The <see cref="Zinnia.Tracking.Follow.ObjectFollower"/> that performs the source follow.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public ObjectFollower ObjectFollower { get; protected set; }
        /// <summary>
        /// The <see cref="TransformPositionExtractor"/> that extracts the source position.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public TransformPositionExtractor PositionExtractor { get; protected set; }
        #endregion

        /// <summary>
        /// Sets the source on the relevant references.
        /// </summary>
        /// <param name="source">The source to set.</param>
        public virtual void SetSource(GameObject source)
        {
            ObjectFollower.Sources.RunWhenActiveAndEnabled(() => ObjectFollower.Sources.Clear());
            ObjectFollower.Sources.RunWhenActiveAndEnabled(() => ObjectFollower.Sources.Add(source));
            PositionExtractor.Source = source;
        }

        /// <summary>
        /// Snaps the tracked collider directly to the source current position.
        /// </summary>
        public virtual void SnapToSource()
        {
            PositionExtractor.DoExtract();
        }

        protected virtual void OnEnable()
        {
            SetSource(Facade.Source);
        }
    }
}