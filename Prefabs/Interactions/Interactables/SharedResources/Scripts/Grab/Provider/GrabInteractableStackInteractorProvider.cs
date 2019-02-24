namespace VRTK.Prefabs.Interactions.Interactables.Grab.Provider
{
    using UnityEngine;
    using System.Collections.Generic;
    using Zinnia.Data.Collection;
    using Zinnia.Data.Attribute;
    using VRTK.Prefabs.Interactions.Interactors;

    /// <summary>
    /// Processes a received grab event into an Observable Stack to handle multiple output options for each grab type.
    /// </summary>
    public class GrabInteractableStackInteractorProvider : GrabInteractableInteractorProvider
    {
        #region Stack Settings
        [Header("Stack Settings"), Tooltip("The stack to get the current interactors from."), InternalSetting, SerializeField]
        private GameObjectObservableStack _eventStack = null;
        /// <summary>
        /// The stack to get the current interactors from.
        /// </summary>
        public GameObjectObservableStack EventStack => _eventStack;
        #endregion

        /// <inheritdoc />
        public override List<InteractorFacade> GetGrabbingInteractors()
        {
            return GetGrabbingInteractors(EventStack.Stack);
        }
    }
}