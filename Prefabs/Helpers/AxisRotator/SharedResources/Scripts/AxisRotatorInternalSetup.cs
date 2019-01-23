namespace VRTK.Prefabs.Helpers.AxisRotator
{
    using UnityEngine;
    using Zinnia.Action;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Operation;

    /// <summary>
    /// Sets up the AxisRotator prefab based on the provided settings and implements the logic to allow rotating an object based on axis data.
    /// </summary>
    public class AxisRotatorInternalSetup : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Header("Facade Settings"), Tooltip("The public interface facade."), InternalSetting, SerializeField]
        protected AxisRotatorFacade facade;
        #endregion

        #region Reference Settings
        /// <summary>
        /// The lateral <see cref="FloatAction"/> to map to.
        /// </summary>
        [Header("Reference Settings"), Tooltip("The lateral FloatAction to map to."), InternalSetting, SerializeField]
        protected FloatAction lateralAxis;
        /// <summary>
        /// The longitudinal <see cref="FloatAction"/> to map to.
        /// </summary>
        [Tooltip("The longitudinal FloatAction to map to."), InternalSetting, SerializeField]
        protected FloatAction longitudinalAxis;
        /// <summary>
        /// The mutator to update the target rotation.
        /// </summary>
        [Tooltip("The mutator to update the target rotation."), InternalSetting, SerializeField]
        protected TransformEulerRotationMutator rotationMutator;
        /// <summary>
        /// The extractor to get the target offset direction data.
        /// </summary>
        [Tooltip("The extractor to get the target offset direction data."), InternalSetting, SerializeField]
        protected TransformDirectionExtractor directionExtractor;
        #endregion

        /// <summary>
        /// Sets the axis sources.
        /// </summary>
        /// <param name="clearOnly">Whether to only clear the existing sources and not add new ones.</param>
        public virtual void SetAxisSources(bool clearOnly = false)
        {
            if (lateralAxis != null)
            {
                lateralAxis.ClearSources();
                if (!clearOnly && facade.LateralAxis != null)
                {
                    lateralAxis.AddSource(facade.LateralAxis);
                }
            }

            if (longitudinalAxis != null)
            {
                longitudinalAxis.ClearSources();
                if (!clearOnly && facade.LongitudinalAxis != null)
                {
                    longitudinalAxis.AddSource(facade.LongitudinalAxis);
                }
            }
        }

        /// <summary>
        /// Sets the target of the rotation mutator.
        /// </summary>
        public virtual void SetMutator()
        {
            rotationMutator.target = facade.Target;
        }

        /// <summary>
        /// Sets the source of the rotation extractor.
        /// </summary>
        public virtual void SetExtractor()
        {
            directionExtractor.source = facade.DirectionOffset;
        }

        protected virtual void OnEnable()
        {
            SetAxisSources();
            SetMutator();
            SetExtractor();
        }

        protected virtual void OnDisable()
        {
            SetAxisSources(true);
        }
    }
}