namespace VRTK.Prefabs.PlayAreaRepresentation
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Operation.Mutation;
    using Zinnia.Data.Operation.Extraction;
    using Zinnia.Tracking.CameraRig.Operation.Extraction;

    /// <summary>
    /// Sets up the PlayAreaRepresentation Prefab based on the provided user settings.
    /// </summary>
    public class PlayAreaRepresentationConfigurator : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public PlayAreaRepresentationFacade Facade { get; protected set; }
        #endregion

        #region Operator Settings
        /// <summary>
        /// The <see cref="PlayAreaDimensionsExtractor"/> component for extracting the PlayArea dimension data.
        /// </summary>
        [Serialized]
        [field: Header("Operator Settings"), DocumentedByXml, Restricted]
        public PlayAreaDimensionsExtractor DimensionExtractor { get; protected set; }
        /// <summary>
        /// The <see cref="TransformScaleMutator"/> component for scaling the given target.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public TransformScaleMutator ObjectScaler { get; protected set; }
        /// <summary>
        /// The <see cref="TransformPositionMutator"/> component for positioning the given target.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public TransformPositionMutator ObjectPositioner { get; protected set; }
        /// <summary>
        /// The <see cref="TransformPositionExtractor"/> component extracting the offset origin position.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public TransformPositionExtractor OffsetOriginExtractor { get; protected set; }
        /// <summary>
        /// The <see cref="TransformPositionExtractor"/> component extracting the offset destination position.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public TransformPositionExtractor OffsetDestinationExtractor { get; protected set; }
        #endregion

        /// <summary>
        /// Configures the target settings.
        /// </summary>
        public virtual void ConfigureTarget()
        {
            ObjectScaler.Target = Facade.Target;
            ObjectPositioner.Target = Facade.Target;
        }

        /// <summary>
        /// Configures the offset origin settings.
        /// </summary>
        public virtual void ConfigureOffsetOrigin()
        {
            OffsetOriginExtractor.Source = Facade.OffsetOrigin;
        }

        /// <summary>
        /// Configures the offset destination settings.
        /// </summary>
        public virtual void ConfigureOffsetDestination()
        {
            OffsetDestinationExtractor.Source = Facade.OffsetDestination;
        }

        /// <summary>
        /// Recalculates the PlayArea dimensions.
        /// </summary>
        public virtual void RecalculateDimensions()
        {
            DimensionExtractor.DoExtract();
        }

        protected virtual void OnEnable()
        {
            ConfigureTarget();
            ConfigureOffsetOrigin();
            ConfigureOffsetDestination();
            ObjectScaler.gameObject.SetActive(true);
        }

        protected virtual void OnDisable()
        {
            ObjectScaler.gameObject.SetActive(false);
        }
    }
}