namespace VRTK.Prefabs.Interactions.Interactables.Grab.Provider
{
    using UnityEngine;
    using System.Collections.Generic;
    using Zinnia.Extension;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Collection;
    using VRTK.Prefabs.Interactions.Interactors;

    /// <summary>
    /// Processes a received grab event into an Observable Set to handle a simplified grab process.
    /// </summary>
    public class GrabInteractableSetInteractorProvider : GrabInteractableInteractorProvider
    {
        #region Stack Settings
        [Header("Set Settings"), Tooltip("The set to get the current interactors from."), InternalSetting, SerializeField]
        private GameObjectObservableSet _eventSet = null;
        /// <summary>
        /// The set to get the current interactors from.
        /// </summary>
        public GameObjectObservableSet EventSet => _eventSet;
        #endregion

        /// <inheritdoc />
        public override List<InteractorFacade> GetGrabbingInteractors()
        {
            return GetGrabbingInteractors(EventSet.Elements.EmptyIfNull());
        }
    }
}