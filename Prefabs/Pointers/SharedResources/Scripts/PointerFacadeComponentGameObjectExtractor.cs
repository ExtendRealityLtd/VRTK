namespace VRTK.Prefabs.Pointers
{
    using UnityEngine;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.BehaviourStateRequirementMethod;
    using Zinnia.Cast;
    using Zinnia.Pointer;
    using Zinnia.Extension;
    using Zinnia.Data.Operation.Extraction;

    /// <summary>
    /// Extracts and emits the selected <see cref="Component"/> residing <see cref="GameObject"/> from the <see cref="Source"/>.
    /// </summary>
    public class PointerFacadeComponentGameObjectExtractor : GameObjectExtractor
    {
        /// <summary>
        /// The Pointer Component to be extracted.
        /// </summary>
        public enum PointerComponentType
        {
            /// <summary>
            /// The pointer <see cref="PointsCast"/>.
            /// </summary>
            Caster,
            /// <summary>
            /// The <see cref="PointerElement"/> that represents the Origin.
            /// </summary>
            PointerElementOrigin,
            /// <summary>
            /// The <see cref="PointerElement"/> that represents the Repeated Segment.
            /// </summary>
            PointerElementRepeatedSegment,
            /// <summary>
            /// The <see cref="PointerElement"/> that represents the Destination.
            /// </summary>
            PointerElementDestination
        }

        /// <summary>
        /// The source to extract from.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public PointerFacade Source { get; set; }
        /// <summary>
        /// The <see cref="PointerComponentType"/> to extract from the <see cref="Source"/>.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public PointerComponentType PointerComponent { get; set; } = PointerComponentType.Caster;

        /// <inheritdoc />
        public override GameObject Extract()
        {
            Result = null;

            if (Source == null)
            {
                return null;
            }

            switch (PointerComponent)
            {
                case PointerComponentType.Caster:
                    Result = Source.Configuration.Caster.gameObject;
                    break;
                case PointerComponentType.PointerElementOrigin:
                    Result = Source.Configuration.ObjectPointer.Origin.gameObject;
                    break;
                case PointerComponentType.PointerElementRepeatedSegment:
                    Result = Source.Configuration.ObjectPointer.RepeatedSegment.gameObject;
                    break;
                case PointerComponentType.PointerElementDestination:
                    Result = Source.Configuration.ObjectPointer.Destination.gameObject;
                    break;
                default:
                    return null;
            }

            return base.Extract();
        }

        /// <summary>
        /// Extracts the <see cref="Source"/> <see cref="GameObject"/> from the given <see cref="PointerFacade"/> data.
        /// </summary>
        /// <param name="data">The <see cref="PointerFacade"/> payload data to extract from.</param>
        /// <returns>The <see cref="Source"/> <see cref="GameObject"/> within the <see cref="PointerFacade"/>.</returns>
        public virtual GameObject Extract(PointerFacade facade)
        {
            Source = facade;
            return Extract();
        }

        /// <summary>
        /// Extracts the <see cref="Source"/> <see cref="GameObject"/> from the given <see cref="PointerFacade"/> data.
        /// </summary>
        /// <param name="data">The <see cref="PointerFacade"/> payload data to extract from.</param>
        public virtual void DoExtract(PointerFacade facade)
        {
            Extract(facade);
        }

        /// <summary>
        /// Sets the <see cref="Source"/> based on a given <see cref="GameObject"/>.
        /// </summary>
        /// <param name="source">The data that contains the <see cref="PointerFacade"/> component.</param>
        [RequiresBehaviourState]
        public virtual void SetSource(GameObject source)
        {
            Source = source.TryGetComponent<PointerFacade>();
        }
    }
}