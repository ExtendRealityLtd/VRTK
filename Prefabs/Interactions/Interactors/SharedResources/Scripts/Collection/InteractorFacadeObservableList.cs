namespace VRTK.Prefabs.Interactions.Interactors.Collection
{
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;
    using Zinnia.Data.Collection.List;
    using VRTK.Prefabs.Interactions.Interactors;

    /// <summary>
    /// Allows observing changes to a <see cref="List{T}"/> of <see cref="InteractorFacade"/>s.
    /// </summary>
    public class InteractorFacadeObservableList : DefaultObservableList<InteractorFacade, InteractorFacadeObservableList.UnityEvent>
    {
        /// <summary>
        /// Defines the event with the <see cref="InteractorFacade"/>.
        /// </summary>
        [Serializable]
        public class UnityEvent : UnityEvent<InteractorFacade>
        {
        }
    }
}
