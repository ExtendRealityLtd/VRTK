namespace VRTK.Prefabs.Interactions.Interactables.Grab.Provider
{
    using UnityEngine;
    using System.Collections.Generic;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Collection.Stack;
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