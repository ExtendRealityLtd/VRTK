namespace VRTK.Prefabs.Interactions.Interactors
{
    using UnityEngine;
    using Zinnia.Action;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Collision.Active;

    /// <summary>
    /// Sets up the Interactor Prefab action settings based on the provided user settings.
    /// </summary>
    public class ActionInteractorInternalSetup : MonoBehaviour
    {
        #region Action Settings
        [Header("Action Settings"), Tooltip("The Action to be monitored to control the interaction."), SerializeField]
        private Action _sourceAction;
        /// <summary>
        /// The <see cref="Action"/> to be monitored to control the interaction.
        /// </summary>
        public Action SourceAction
        {
            get { return _sourceAction; }
            set
            {
                _sourceAction = value;
                LinkSourceActionToTargetAction();
            }
        }

        [Tooltip("The source GameObject for the interaction publishers to publish as."), SerializeField]
        private GameObject _sourceContainer;
        /// <summary>
        /// The source <see cref="GameObject"/> for the interaction publishers to publish as.
        /// </summary>
        public GameObject SourceContainer
        {
            get { return _sourceContainer; }
            set
            {
                _sourceContainer = value;
                LinkSourceContainerToPublishers();
            }
        }
        #endregion

        #region Internal Settings
        /// <summary>
        /// The <see cref="Action"/> that will be linked to the <see cref="sourceAction"/>.
        /// </summary>
        [Header("Internal Settings"), Tooltip("The Action that will be linked to the sourceAction."), InternalSetting, SerializeField]
        protected Action targetAction;
        /// <summary>
        /// The <see cref="ActiveCollisionPublisher"/> for checking valid start action.
        /// </summary>
        [Tooltip("The ActiveCollisionPublisher for checking valid start action."), InternalSetting, SerializeField]
        protected ActiveCollisionPublisher startActionPublisher;
        /// <summary>
        /// The <see cref="ActiveCollisionPublisher"/> for checking valid stop action.
        /// </summary>
        [Tooltip("The ActiveCollisionPublisher for checking valid stop action."), InternalSetting, SerializeField]
        protected ActiveCollisionPublisher stopActionPublisher;
        #endregion

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            LinkSourceActionToTargetAction();
            LinkSourceContainerToPublishers();
        }

        /// <summary>
        /// Links the <see cref="SourceAction"/> to the internal <see cref="targetAction"/>.
        /// </summary>
        protected virtual void LinkSourceActionToTargetAction()
        {
            targetAction.ClearSources();
            targetAction.AddSource(SourceAction);
        }

        /// <summary>
        /// Links the <see cref="SourceContainer"/> to the payload source containers on the start and stop publishers.
        /// </summary>
        protected virtual void LinkSourceContainerToPublishers()
        {
            startActionPublisher.payload.sourceContainer = SourceContainer;
            stopActionPublisher.payload.sourceContainer = SourceContainer;
        }
    }
}