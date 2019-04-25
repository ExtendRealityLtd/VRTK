namespace VRTK.Prefabs.Interactions.Interactables.Grab.Action
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Collection.List;
    using Zinnia.Tracking.Modification;

    /// <summary>
    /// Describes an action that allows the Interactable to point in the direction of a given Interactor.
    /// </summary>
    public class GrabInteractableControlDirectionAction : GrabInteractableAction
    {
        #region Interactable Settings
        /// <summary>
        /// A <see cref="GameObject"/> collection to enable/disabled as part of the direction control process.
        /// </summary>
        [Serialized]
        [field: Header("Interactable Settings"), DocumentedByXml, Restricted]
        public GameObjectObservableList LinkedObjects { get; protected set; }
        /// <summary>
        /// The <see cref="Zinnia.Tracking.Modification.DirectionModifier"/> to process the direction control.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public DirectionModifier DirectionModifier { get; protected set; }
        #endregion

        /// <summary>
        /// Enables the <see cref="GameObject"/> state of each of the items in the <see cref="LinkedObjects"/> collection.
        /// </summary>
        public virtual void EnableLinkedObjects()
        {
            ToggleLinkedObjectState(true);
        }

        /// <summary>
        /// Disables the <see cref="GameObject"/> state of each of the items in the <see cref="LinkedObjects"/> collection.
        /// </summary>
        public virtual void DisableLinkedObjects()
        {
            ToggleLinkedObjectState(false);
        }

        /// <summary>
        /// Toggles the <see cref="GameObject"/> state of each of the items in the <see cref="LinkedObjects"/> collection.
        /// </summary>
        /// <param name="state">The state to set the <see cref="GameObject"/> active state to.</param>
        protected virtual void ToggleLinkedObjectState(bool state)
        {
            foreach (GameObject linkedObject in LinkedObjects.NonSubscribableElements)
            {
                linkedObject.SetActive(state);
            }
        }

        /// <inheritdoc />
        protected override void OnAfterGrabSetupChange()
        {
            DirectionModifier.Target = GrabSetup.Facade.ConsumerContainer;
        }
    }
}