namespace VRTK.Prefabs.Interactions.Interactables.Grab.Provider
{
    using UnityEngine;
    using System.Collections.Generic;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Collection.List;
    using VRTK.Prefabs.Interactions.Interactors;

    /// <summary>
    /// Processes a received grab event into an Observable Set to handle a simplified grab process.
    /// </summary>
    public class GrabInteractableListInteractorProvider : GrabInteractableInteractorProvider
    {
        #region List Settings
        /// <summary>
        /// The set to get the current interactors from.
        /// </summary>
        [Serialized]
        [field: Header("List Settings"), DocumentedByXml, Restricted]
        public GameObjectObservableList EventList { get; protected set; }
        #endregion

        /// <inheritdoc />
        public override IReadOnlyList<InteractorFacade> GrabbingInteractors => GetGrabbingInteractors(EventList.NonSubscribableElements);
    }
}