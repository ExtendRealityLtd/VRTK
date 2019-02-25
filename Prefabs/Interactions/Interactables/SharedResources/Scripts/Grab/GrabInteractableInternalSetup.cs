namespace VRTK.Prefabs.Interactions.Interactables.Grab
{
    using UnityEngine;
    using System.Collections.Generic;
    using Zinnia.Extension;
    using VRTK.Prefabs.Interactions.Interactors;
    using VRTK.Prefabs.Interactions.Interactables.Grab.Action;
    using VRTK.Prefabs.Interactions.Interactables.Grab.Receiver;
    using VRTK.Prefabs.Interactions.Interactables.Grab.Provider;

    /// <summary>
    /// Sets up the Interactable Prefab grab settings based on the provided user settings.
    /// </summary>
    public class GrabInteractableInternalSetup : MonoBehaviour
    {
        #region Facade Settings
        [Header("Facade Settings"), Tooltip("The public interface facade."), SerializeField]
        private InteractableFacade _facade = null;
        /// <summary>
        /// The public interface facade.
        /// </summary>
        public InteractableFacade Facade => _facade;
        #endregion

        #region Action Settings
        [Header("Action Settings"), Tooltip("The action to perform when grabbing the interactable for the first time."), SerializeField]
        private GrabInteractableAction _primaryAction;
        /// <summary>
        /// The action to perform when grabbing the interactable for the first time.
        /// </summary>
        public GrabInteractableAction PrimaryAction
        {
            get { return _primaryAction; }
            set
            {
                _primaryAction = value;
                ConfigurePrimaryAction();
            }
        }
        /// <summary>
        /// The current cached version of the primary action.
        /// </summary>
        protected GrabInteractableAction cachedPrimaryAction;

        [Tooltip("The action to perform when grabbing the interactable for the second time."), SerializeField]
        private GrabInteractableAction _secondaryAction;
        /// <summary>
        /// The action to perform when grabbing the interactable for the second time.
        /// </summary>
        public GrabInteractableAction SecondaryAction
        {
            get { return _secondaryAction; }
            set
            {
                _secondaryAction = value;
                ConfigureSecondaryAction();
            }
        }
        /// <summary>
        /// The current cached version of the primary action.
        /// </summary>
        protected GrabInteractableAction cachedSecondaryAction;
        #endregion

        #region Reference Settings
        [Header("Reference Settings"), Tooltip("The Grab Receiver setup."), SerializeField]
        private GrabInteractableReceiver _grabReceiver = null;
        /// <summary>
        /// The Grab Receiver setup.
        /// </summary>
        public GrabInteractableReceiver GrabReceiver => _grabReceiver;

        [Tooltip("The Grab Provider setup."), SerializeField]
        private GrabInteractableInteractorProvider _grabProvider = null;
        /// <summary>
        /// The Grab Provider setup.
        /// </summary>
        public GrabInteractableInteractorProvider GrabProvider => _grabProvider;
        #endregion

        /// <summary>
        /// A collection of Interactors that are currently grabbing the Interactable.
        /// </summary>
        public IReadOnlyList<InteractorFacade> GrabbingInteractors => GrabProvider.GetGrabbingInteractors();

        /// <summary>
        /// Attempt to grab the Interactable to the given Interactor.
        /// </summary>
        /// <param name="interactor">The Interactor to attach the Interactable to.</param>
        public virtual void Grab(InteractorFacade interactor)
        {
            interactor.Grab(Facade);
        }

        /// <summary>
        /// Attempt to ungrab the Interactable.
        /// </summary>
        /// <param name="sequenceIndex">The Interactor sequence index to ungrab from.</param>
        public virtual void Ungrab(int sequenceIndex = 0)
        {
            if (GrabbingInteractors == null || GrabbingInteractors.Count == 0 || sequenceIndex >= GrabbingInteractors.Count)
            {
                return;
            }

            Ungrab(GrabbingInteractors[sequenceIndex]);
        }

        /// <summary>
        /// Attempts to ungrab the Interactable.
        /// </summary>
        /// <param name="interactor">The Interactor to ungrab from.</param>
        public virtual void Ungrab(InteractorFacade interactor)
        {
            interactor.Ungrab();
        }

        /// <summary>
        /// Notifies that the Interactable is being grabbed.
        /// </summary>
        /// <param name="data">The grabbing object.</param>
        public virtual void NotifyGrab(GameObject data)
        {
            InteractorFacade interactor = data.TryGetComponent<InteractorFacade>(true, true);
            if (interactor != null)
            {
                if (Facade.GrabbingInteractors.Count == 1)
                {
                    Facade.FirstGrabbed?.Invoke(interactor);
                }
                Facade.Grabbed?.Invoke(interactor);
                interactor.Grabbed?.Invoke(Facade);
                interactor.GrabInteractorSetup.grabbedObjectsCollection.AddElement(Facade.gameObject);
            }
        }

        /// <summary>
        /// Notifies that the Interactable is no longer being grabbed.
        /// </summary>
        /// <param name="data">The previous grabbing object.</param>
        public virtual void NotifyUngrab(GameObject data)
        {
            InteractorFacade interactor = data.TryGetComponent<InteractorFacade>(true, true);
            if (interactor != null)
            {
                Facade.Ungrabbed?.Invoke(interactor);
                interactor.Ungrabbed?.Invoke(Facade);
                interactor.GrabInteractorSetup.grabbedObjectsCollection.RemoveElement(Facade.gameObject);
                if (Facade.GrabbingInteractors.Count == 0)
                {
                    Facade.LastUngrabbed?.Invoke(interactor);
                }
            }
        }

        /// <summary>
        /// Determines if the grab type is set to toggle.
        /// </summary>
        /// <returns>Whether the grab type is of type toggle.</returns>
        public virtual bool IsGrabTypeToggle()
        {
            return GrabReceiver.GrabType == GrabInteractableReceiver.ActiveType.Toggle;
        }

        /// <summary>
        /// Configures the interactor grab validity.
        /// </summary>
        /// <param name="interactors">The interactors to add to the validity list.</param>
        public virtual void ConfigureGrabValidity(List<InteractorFacade> interactors)
        {
            GrabReceiver.ConfigureGrabValidity(interactors);
        }

        /// <summary>
        /// Sets the consumer containers to the current active container.
        /// </summary>
        public virtual void ConfigureContainer()
        {
            GrabReceiver.ConfigureConsumerContainers(Facade.ConsumerContainer);
            ConfigureActionContainer(PrimaryAction);
            ConfigureActionContainer(SecondaryAction);
        }

        protected virtual void OnEnable()
        {
            LinkReceiverToProvider();
            ConfigurePrimaryAction();
            ConfigureSecondaryAction();
            ConfigureContainer();
            ConfigureGrabValidity(Facade.disallowedGrabInteractors);
        }

        protected virtual void OnDisable()
        {
            UnlinkReceiverToProvider();
            UnlinkToPrimaryAction();
            UnlinkToSecondaryAction();
        }

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            ConfigurePrimaryAction();
            ConfigureSecondaryAction();
            ConfigureActionContainer(PrimaryAction);
            ConfigureActionContainer(SecondaryAction);
        }

        /// <summary>
        /// Configures the action containers.
        /// </summary>
        /// <param name="action">The action to configure.</param>
        protected virtual void ConfigureActionContainer(GrabInteractableAction action)
        {
            action.SetUp(this);
        }

        /// <summary>
        /// Links the Grab Receiver to the Grab Provider.
        /// </summary>
        protected virtual void LinkReceiverToProvider()
        {
            GrabReceiver.OutputGrabAction.Emitted.AddListener(GrabProvider.InputGrabReceived.Receive);
            GrabReceiver.OutputUngrabAction.Emitted.AddListener(GrabProvider.InputUngrabReceived.Receive);
            GrabReceiver.OutputUngrabOnUntouchAction.Emitted.AddListener(Facade.Ungrab);
        }

        /// <summary>
        /// Links the Grab Receiver and Grab Provider to the Primary Grab Action.
        /// </summary>
        protected virtual void LinkToPrimaryAction()
        {
            cachedPrimaryAction = PrimaryAction;
            GrabReceiver.OutputActiveCollisionConsumer.Emitted.AddListener(cachedPrimaryAction.InputActiveCollisionConsumer.Receive);
            GrabProvider.OutputPrimaryGrabAction.Emitted.AddListener(cachedPrimaryAction.InputGrabReceived.Receive);
            GrabProvider.OutputPrimaryUngrabAction.Emitted.AddListener(cachedPrimaryAction.InputUngrabReceived.Receive);
        }

        /// <summary>
        /// Links the Grab Receiver and Grab Provider to the Secondary Grab Action.
        /// </summary>
        protected virtual void LinkToSecondaryAction()
        {
            cachedSecondaryAction = SecondaryAction;
            GrabReceiver.OutputActiveCollisionConsumer.Emitted.AddListener(cachedSecondaryAction.InputActiveCollisionConsumer.Receive);
            GrabProvider.OutputPrimaryGrabSetupOnSecondaryAction.Emitted.AddListener(cachedSecondaryAction.InputGrabSetup.Receive);
            GrabProvider.OutputPrimaryUngrabResetOnSecondaryAction.Emitted.AddListener(cachedSecondaryAction.InputUngrabReset.Receive);
            GrabProvider.OutputSecondaryGrabAction.Emitted.AddListener(cachedSecondaryAction.InputGrabReceived.Receive);
            GrabProvider.OutputSecondaryUngrabAction.Emitted.AddListener(cachedSecondaryAction.InputUngrabReceived.Receive);
        }

        /// <summary>
        /// Unlinks the Grab Receiver to the Grab Provider.
        /// </summary>
        protected virtual void UnlinkReceiverToProvider()
        {
            GrabReceiver.OutputGrabAction.Emitted.RemoveListener(GrabProvider.InputGrabReceived.Receive);
            GrabReceiver.OutputUngrabAction.Emitted.RemoveListener(GrabProvider.InputUngrabReceived.Receive);
            GrabReceiver.OutputUngrabOnUntouchAction.Emitted.RemoveListener(Facade.Ungrab);
        }

        /// <summary>
        /// Unlinks the Grab Receiver and Grab Provider to the Primary Grab Action.
        /// </summary>
        protected virtual void UnlinkToPrimaryAction()
        {
            if (cachedPrimaryAction == null)
            {
                return;
            }

            GrabReceiver.OutputActiveCollisionConsumer.Emitted.RemoveListener(cachedPrimaryAction.InputActiveCollisionConsumer.Receive);
            GrabProvider.OutputPrimaryGrabAction.Emitted.RemoveListener(cachedPrimaryAction.InputGrabReceived.Receive);
            GrabProvider.OutputPrimaryUngrabAction.Emitted.RemoveListener(cachedPrimaryAction.InputUngrabReceived.Receive);
        }

        /// <summary>
        /// Unlinks the Grab Receiver and Grab Provider to the Secondary Grab Action.
        /// </summary>
        protected virtual void UnlinkToSecondaryAction()
        {
            if (cachedSecondaryAction == null)
            {
                return;
            }

            GrabReceiver.OutputActiveCollisionConsumer.Emitted.RemoveListener(cachedSecondaryAction.InputActiveCollisionConsumer.Receive);
            GrabProvider.OutputPrimaryGrabSetupOnSecondaryAction.Emitted.RemoveListener(cachedSecondaryAction.InputGrabSetup.Receive);
            GrabProvider.OutputPrimaryUngrabResetOnSecondaryAction.Emitted.RemoveListener(cachedSecondaryAction.InputUngrabReset.Receive);
            GrabProvider.OutputSecondaryGrabAction.Emitted.RemoveListener(cachedSecondaryAction.InputGrabReceived.Receive);
            GrabProvider.OutputSecondaryUngrabAction.Emitted.RemoveListener(cachedSecondaryAction.InputUngrabReceived.Receive);
        }

        /// <summary>
        /// Configures the primary action.
        /// </summary>
        protected virtual void ConfigurePrimaryAction()
        {
            UnlinkToPrimaryAction();
            LinkToPrimaryAction();
            ConfigureActionContainer(PrimaryAction);
        }

        /// <summary>
        /// Configures the secondary action.
        /// </summary>
        protected virtual void ConfigureSecondaryAction()
        {
            UnlinkToSecondaryAction();
            LinkToSecondaryAction();
            ConfigureActionContainer(SecondaryAction);
        }
    }
}