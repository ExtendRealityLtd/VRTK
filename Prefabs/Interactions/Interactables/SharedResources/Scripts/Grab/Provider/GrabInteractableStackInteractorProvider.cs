namespace VRTK.Prefabs.Interactions.Interactables.Grab.Provider
{
    using UnityEngine;
    using System.Collections.Generic;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Zinnia.Data.Collection;
    using Zinnia.Data.Attribute;
    using VRTK.Prefabs.Interactions.Interactors;

    /// <summary>
    /// Processes a received grab event into an Observable Stack to handle multiple output options for each grab type.
    /// </summary>
    public class GrabInteractableStackInteractorProvider : GrabInteractableInteractorProvider
    {
        #region Stack Settings
        /// <summary>
        /// The stack to get the current interactors from.
        /// </summary>
        [Serialized]
        [field: Header("Stack Settings"), DocumentedByXml, Restricted]
        public GameObjectObservableStack EventStack { get; protected set; }
        #endregion

        /// <inheritdoc />
        public override IReadOnlyList<InteractorFacade> GrabbingInteractors => GetGrabbingInteractors(EventStack.Stack);
    }
}