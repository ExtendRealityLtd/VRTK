namespace VRTK.Prefabs.Pointers
{
    using UnityEngine;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.BehaviourStateRequirementMethod;
    using Zinnia.Pointer;
    using Zinnia.Data.Type;
    using Zinnia.Data.Operation.Extraction;

    /// <summary>
    /// Extracts and emits the <see cref="Source"/> residing <see cref="GameObject"/>.
    /// </summary>
    public class PointerFacadeGameObjectExtractor : GameObjectExtractor
    {
        /// <summary>
        /// The source to extract from.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public PointerFacade Source { get; set; }

        /// <inheritdoc />
        public override GameObject Extract()
        {
            if (Source == null)
            {
                Result = null;
                return null;
            }

            Result = Source.gameObject;
            return base.Extract();
        }

        /// <summary>
        /// Extracts the <see cref="Source"/> <see cref="GameObject"/> from the given <see cref="SurfaceData"/> payload data.
        /// </summary>
        /// <param name="data">The <see cref="SurfaceData"/> payload data to extract from.</param>
        /// <returns>The <see cref="Source"/> <see cref="GameObject"/> within the <see cref="SurfaceData"/>.</returns>
        public virtual GameObject Extract(SurfaceData data)
        {
            SetSource(data);
            return Extract();
        }

        /// <summary>
        /// Extracts the <see cref="Source"/> <see cref="GameObject"/> from the given <see cref="SurfaceData"/> payload data.
        /// </summary>
        /// <param name="data">The <see cref="SurfaceData"/> payload data to extract from.</param>
        public virtual void DoExtract(SurfaceData data)
        {
            Extract(data);
        }

        /// <summary>
        /// Extracts the <see cref="Source"/> <see cref="GameObject"/> from the given <see cref="ObjectPointer.EventData"/> payload data.
        /// </summary>
        /// <param name="data">The <see cref="ObjectPointer.EventData"/> payload data to extract from.</param>
        /// <returns>The <see cref="Source"/> <see cref="GameObject"/> within the <see cref="ObjectPointer.EventData"/>.</returns>
        public virtual GameObject Extract(ObjectPointer.EventData data)
        {
            return Extract((SurfaceData)data);
        }

        /// <summary>
        /// Extracts the <see cref="Source"/> <see cref="GameObject"/> from the given <see cref="ObjectPointer.EventData"/> payload data.
        /// </summary>
        /// <param name="data">The <see cref="ObjectPointer.EventData"/> payload data to extract from.</param>
        public virtual void DoExtract(ObjectPointer.EventData data)
        {
            Extract(data);
        }

        /// <summary>
        /// Sets the <see cref="Source"/> based on given <see cref="SurfaceData"/>.
        /// </summary>
        /// <param name="source">The data that contains the source transform.</param>
        [RequiresBehaviourState]
        public virtual void SetSource(SurfaceData source)
        {
            if (source == null || source.Transform == null)
            {
                return;
            }

            Source = source.Transform.GetComponentInParent<PointerFacade>();
        }

        /// <summary>
        /// Sets the <see cref="Source"/> based on given <see cref="ObjectPointer.EventData"/>.
        /// </summary>
        /// <param name="source">The data that contains the source transform.</param>
        public virtual void SetSource(ObjectPointer.EventData source)
        {
            SetSource((SurfaceData)source);
        }
    }
}