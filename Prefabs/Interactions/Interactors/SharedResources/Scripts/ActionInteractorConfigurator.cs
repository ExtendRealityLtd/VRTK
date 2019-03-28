namespace VRTK.Prefabs.Interactions.Interactors
{
    using UnityEngine;
    using Malimbe.MemberChangeMethod;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Action;
    using Zinnia.Extension;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Collision.Active;

    /// <summary>
    /// Sets up the Interactor Prefab action settings based on the provided user settings.
    /// </summary>
    public class ActionInteractorConfigurator : MonoBehaviour
    {
        #region Action Settings
        /// <summary>
        /// The <see cref="Action"/> to be monitored to control the interaction.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Action Settings"), DocumentedByXml]
        public Action SourceAction { get; set; }
        /// <summary>
        /// The source <see cref="GameObject"/> for the interaction publishers to publish as.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Action Settings"), DocumentedByXml]
        public GameObject SourceContainer { get; set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The <see cref="Action"/> that will be linked to the <see cref="SourceAction"/>.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public Action TargetAction { get; protected set; }
        /// <summary>
        /// The <see cref="ActiveCollisionPublisher"/> for checking valid start action.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public ActiveCollisionPublisher StartActionPublisher { get; protected set; }
        /// <summary>
        /// The <see cref="ActiveCollisionPublisher"/> for checking valid stop action.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public ActiveCollisionPublisher StopActionPublisher { get; protected set; }
        #endregion

        protected virtual void OnEnable()
        {
            LinkSourceActionToTargetAction();
            LinkSourceContainerToPublishers();
        }

        /// <summary>
        /// Links the <see cref="SourceAction"/> to the internal <see cref="TargetAction"/>.
        /// </summary>
        protected virtual void LinkSourceActionToTargetAction()
        {
            TargetAction.RunWhenActiveAndEnabled(() => TargetAction.ClearSources());
            TargetAction.RunWhenActiveAndEnabled(() => TargetAction.AddSource(SourceAction));
        }

        /// <summary>
        /// Links the <see cref="SourceContainer"/> to the payload source containers on the start and stop publishers.
        /// </summary>
        protected virtual void LinkSourceContainerToPublishers()
        {
            StartActionPublisher.Payload.SourceContainer = SourceContainer;
            StopActionPublisher.Payload.SourceContainer = SourceContainer;
        }

        /// <summary>
        /// Called after <see cref="SourceAction"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(SourceAction))]
        protected virtual void OnAfterSourceActionChange()
        {
            LinkSourceActionToTargetAction();
        }

        /// <summary>
        /// Called after <see cref="SourceContainer"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(SourceContainer))]
        protected virtual void OnAfterSourceContainerChange()
        {
            LinkSourceContainerToPublishers();
        }
    }
}