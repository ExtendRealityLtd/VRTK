namespace VRTK.Prefabs.PlayAreaRepresentation
{
    using UnityEngine;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Operation;
    using Zinnia.Tracking.CameraRig;

    /// <summary>
    /// Sets up the PlayAreaRepresentation Prefab based on the provided user settings.
    /// </summary>
    public class PlayAreaRepresentationInternalSetup : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Header("Facade Settings"), Tooltip("The public interface facade."), InternalSetting, SerializeField]
        protected PlayAreaRepresentationFacade facade;
        #endregion

        #region Operator Settings
        /// <summary>
        /// The <see cref="PlayAreaDimensionsExtractor"/> component for extracting the PlayArea dimension data.
        /// </summary>
        [Header("Operator Settings"), Tooltip("The PlayAreaDimensionsExtractor component for extracting the PlayArea dimension data."), InternalSetting, SerializeField]
        protected PlayAreaDimensionsExtractor dimensionExtractor;
        /// <summary>
        /// The <see cref="TransformScaleMutator"/> component for scaling the given target.
        /// </summary>
        [Tooltip("The TransformScaleMutator component for scaling the given target."), InternalSetting, SerializeField]
        protected TransformScaleMutator objectScaler;
        /// <summary>
        /// The <see cref="TransformPositionMutator"/> component for positioning the given target.
        /// </summary>
        [Tooltip("The TransformPositionMutator component for positioning the given target."), InternalSetting, SerializeField]
        protected TransformPositionMutator objectPositioner;
        /// <summary>
        /// The <see cref="TransformPositionExtractor"/> component extracting the offset origin position.
        /// </summary>
        [Tooltip("The TransformPositionExtractor component extracting the offset origin position."), InternalSetting, SerializeField]
        protected TransformPositionExtractor offsetOriginExtractor;
        /// <summary>
        /// The <see cref="TransformPositionExtractor"/> component extracting the offset destination position.
        /// </summary>
        [Tooltip("The TransformPositionExtractor component extracting the offset destination position."), InternalSetting, SerializeField]
        protected TransformPositionExtractor offsetDestinationExtractor;
        #endregion

        /// <summary>
        /// Configures the target settings.
        /// </summary>
        public virtual void ConfigureTarget()
        {
            objectScaler.target = facade.Target;
            objectPositioner.target = facade.Target;
        }

        /// <summary>
        /// Configures the offset origin settings.
        /// </summary>
        public virtual void ConfigureOffsetOrigin()
        {
            offsetOriginExtractor.source = facade.OffsetOrigin;
        }

        /// <summary>
        /// Configures the offset destination settings.
        /// </summary>
        public virtual void ConfigureOffsetDestination()
        {
            offsetDestinationExtractor.source = facade.OffsetDestination;
        }

        /// <summary>
        /// Recalculates the PlayArea dimensions.
        public virtual void RecalculateDimensions()
        {
            dimensionExtractor.DoExtract();
        }

        protected virtual void OnEnable()
        {
            ConfigureTarget();
            ConfigureOffsetOrigin();
            ConfigureOffsetDestination();
            objectScaler.gameObject.SetActive(true);
        }

        protected virtual void OnDisable()
        {
            objectScaler.gameObject.SetActive(false);
        }
    }
}