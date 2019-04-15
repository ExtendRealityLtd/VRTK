namespace VRTK.Prefabs.Helpers.AxisRotator
{
    using UnityEngine;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Zinnia.Action;
    using Zinnia.Extension;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Operation.Mutation;
    using Zinnia.Data.Operation.Extraction;

    /// <summary>
    /// Sets up the AxisRotator prefab based on the provided settings and implements the logic to allow rotating an object based on axis data.
    /// </summary>
    public class AxisRotatorConfigurator : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public AxisRotatorFacade Facade { get; protected set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The lateral <see cref="FloatAction"/> to map to.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public FloatAction LateralAxis { get; protected set; }
        /// <summary>
        /// The longitudinal <see cref="FloatAction"/> to map to.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public FloatAction LongitudinalAxis { get; protected set; }
        /// <summary>
        /// The mutator to update the target rotation.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public TransformEulerRotationMutator RotationMutator { get; protected set; }
        /// <summary>
        /// The extractor to get the target offset direction data.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public TransformDirectionExtractor DirectionExtractor { get; protected set; }
        #endregion

        /// <summary>
        /// Sets the axis sources.
        /// </summary>
        /// <param name="clearOnly">Whether to only clear the existing sources and not add new ones.</param>
        public virtual void SetAxisSources(bool clearOnly = false)
        {
            if (LateralAxis != null)
            {
                LateralAxis.RunWhenActiveAndEnabled(() => LateralAxis.ClearSources());
                if (!clearOnly && Facade.LateralAxis != null)
                {
                    LateralAxis.RunWhenActiveAndEnabled(() => LateralAxis.AddSource(Facade.LateralAxis));
                }
            }

            if (LongitudinalAxis != null)
            {
                LongitudinalAxis.RunWhenActiveAndEnabled(() => LongitudinalAxis.ClearSources());
                if (!clearOnly && Facade.LongitudinalAxis != null)
                {
                    LongitudinalAxis.RunWhenActiveAndEnabled(() => LongitudinalAxis.AddSource(Facade.LongitudinalAxis));
                }
            }
        }

        /// <summary>
        /// Sets the target of the rotation mutator.
        /// </summary>
        public virtual void SetMutator()
        {
            RotationMutator.Target = Facade.Target;
        }

        /// <summary>
        /// Sets the source of the rotation extractor.
        /// </summary>
        public virtual void SetExtractor()
        {
            DirectionExtractor.Source = Facade.DirectionOffset;
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