namespace VRTK.Prefabs.Interactions.InteractableSnapZone
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Extension;
    using Zinnia.Event.Proxy;
    using Zinnia.Data.Type;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Collection.List;
    using Zinnia.Rule.Collection;
    using Zinnia.Tracking.Modification;
    using VRTK.Prefabs.Interactions.Interactables;
    using VRTK.Prefabs.Interactions.Interactables.Grab;

    public class SnapZoneConfigurator : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public SnapZoneFacade Facade { get; protected set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The <see cref="RuleContainerObservableList"/> that determines the valid snappable Interactables.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml]
        public RuleContainerObservableList ValidCollisionRules { get; protected set; }
        /// <summary>
        /// The <see cref="InteractableGrabStateEmitter"/> that processes if the interactable entering the zone is being grabbed.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public InteractableGrabStateEmitter GrabStateEmitter { get; protected set; }
        /// <summary>
        /// The <see cref="ActivationValidator"/> that determines if the activation of the zone is valid.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public SnapZoneActivator ActivationArea { get; protected set; }
        /// <summary>
        /// The <see cref="ActivationValidator"/> that determines if the activation of the zone is valid.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public ActivationValidator ActivationValidator { get; protected set; }
        /// <summary>
        /// The <see cref="TransformPropertyApplier"/> that transitions the Interactable to the snapped destination.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public TransformPropertyApplier PropertyApplier { get; protected set; }
        /// <summary>
        /// The <see cref="GameObjectObservableList"/> containing the list of objects that are currently colliding with the zone.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectObservableList CollidingObjectsList { get; protected set; }
        /// <summary>
        /// The <see cref="GameObjectObservableList"/> containing the list of Interactables that can be snapped.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectObservableList ValidSnappableInteractablesList { get; protected set; }
        /// <summary>
        /// The <see cref="GameObjectObservableList"/> containing the list of snapped Interactables.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectObservableList SnappedInteractablesList { get; protected set; }
        /// <summary>
        /// The <see cref="GameObjectEventProxyEmitter"/> that is responsible for processing the snap of a valid dropped Interactable.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter SnapDroppedInteractableProcess { get; protected set; }
        /// <summary>
        /// The <see cref="GameObjectEventProxyEmitter"/> that is responsible for forcing an unsnap of the snapped Interactable.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter ForceUnsnapInteractableProcess { get; protected set; }
        #endregion

        /// <summary>
        /// Returns the collection of <see cref="GameObject"/>s that are currently colliding with the snap zone and are valid to be snapped.
        /// </summary>
        public HeapAllocationFreeReadOnlyList<GameObject> SnappableInteractables => ValidSnappableInteractablesList.NonSubscribableElements;
        /// <summary>
        /// Returns the currently snapped <see cref="GameObject"/>.
        /// </summary>
        public GameObject SnappedInteractable => SnappedInteractablesList.NonSubscribableElements.Count > 0 ? SnappedInteractablesList.NonSubscribableElements[0] : null;

        /// <summary>
        /// Attempts to snap a given <see cref="GameObject"/> to the snap zone.
        /// </summary>
        /// <param name="objectToSnap">The object to attempt to snap.</param>
        public virtual void Snap(GameObject objectToSnap)
        {
            if (SnappedInteractable != null)
            {
                return;
            }

            SnapDroppedInteractableProcess.Receive(objectToSnap);
        }

        /// <summary>
        /// Attempts to unsnap any existing <see cref="InteractableFacade"/> that is currently snapped to the snap zone.
        /// </summary>
        public virtual void Unsnap()
        {
            if (SnappedInteractable == null)
            {
                return;
            }

            ForceUnsnapInteractableProcess.Receive(SnappedInteractable.gameObject);
        }

        /// <summary>
        /// Emits the Entered event.
        /// </summary>
        /// <param name="entered">The <see cref="GameObject"/> that has entered the zone.</param>
        public virtual void EmitEntered(GameObject entered)
        {
            if (entered == null)
            {
                return;
            }

            Facade.Entered?.Invoke(entered);
        }

        /// <summary>
        /// Emits the Exited event.
        /// </summary>
        /// <param name="exited">The <see cref="GameObject"/> that has exited the zone.</param>
        public virtual void EmitExited(GameObject exited)
        {
            if (exited == null)
            {
                return;
            }

            Facade.Exited?.Invoke(exited);
        }

        /// <summary>
        /// Emits the Activated event.
        /// </summary>
        /// <param name="activator">The <see cref="GameObject"/> that has activated the zone.</param>
        public virtual void EmitActivated(GameObject activator)
        {
            if (activator == null)
            {
                return;
            }

            Facade.Activated?.Invoke(activator);
        }

        /// <summary>
        /// Emits the Deactivated event.
        /// </summary>
        /// <param name="deactivator">The <see cref="GameObject"/> that has deactivated the zone.</param>
        public virtual void EmitDeactivated(GameObject deactivator)
        {
            if (deactivator == null)
            {
                return;
            }

            Facade.Deactivated?.Invoke(deactivator);
        }

        /// <summary>
        /// Emits the Snapped event.
        /// </summary>
        /// <param name="snapped">The <see cref="GameObject"/> is snapped to the zone.</param>
        public virtual void EmitSnapped(GameObject snapped)
        {
            if (snapped == null)
            {
                return;
            }

            Facade.Snapped?.Invoke(snapped);
        }

        /// <summary>
        /// Emits the Unsnapped event.
        /// </summary>
        /// <param name="unsnapped">The <see cref="GameObject"/> is unsnapped from the zone.</param>
        public virtual void EmitUnsnapped(GameObject unsnapped)
        {
            if (unsnapped == null)
            {
                return;
            }

            Facade.Unsnapped?.Invoke(unsnapped);
        }

        /// <summary>
        /// Configures the validity rules for the snap zone.
        /// </summary>
        public virtual void ConfigureValidityRules()
        {
            if (ValidCollisionRules.NonSubscribableElements.Count > 1)
            {
                ValidCollisionRules.RunWhenActiveAndEnabled(() => ValidCollisionRules.RemoveAt(1));
            }
            if (Facade.SnapValidity.Interface != null)
            {
                ValidCollisionRules.RunWhenActiveAndEnabled(() => ValidCollisionRules.Add(Facade.SnapValidity));
            }
        }

        /// <summary>
        /// Configures the transition duration for the snapping process.
        /// </summary>
        public virtual void ConfigurePropertyApplier()
        {
            PropertyApplier.RunWhenActiveAndEnabled(() => PropertyApplier.TransitionDuration = Facade.TransitionDuration);
        }

        /// <summary>
        /// Attempts to process any other valid snappable objects against any other potential SnapZones if their primary activating zone is snapped by another object.
        /// </summary>
        public virtual void ProcessOtherSnappablesOnSnap()
        {
            foreach (GameObject snappable in SnappableInteractables)
            {
                if (snappable == null || snappable == SnappedInteractable)
                {
                    continue;
                }

                InteractableFacade snappableInteractable = snappable.TryGetComponent<InteractableFacade>(true, true);

                if (snappableInteractable == null)
                {
                    continue;
                }

                foreach (GameObject collidingWith in snappableInteractable.ActiveCollisions.SubscribableElements)
                {
                    SnapZoneActivator activatingZone = collidingWith.GetComponent<SnapZoneActivator>();

                    if (activatingZone == null || activatingZone == ActivationArea || activatingZone.Facade.ZoneState == SnapZoneFacade.SnapZoneState.ZoneIsActivated)
                    {
                        continue;
                    }

                    activatingZone.Facade.Configuration.GrabStateEmitter.DoIsGrabbed(snappableInteractable);
                }
            }
        }

        protected virtual void OnEnable()
        {
            ConfigureValidityRules();
            ConfigurePropertyApplier();
            if (SnappedInteractable != null)
            {
                SnappedInteractable.gameObject.SetActive(true);
            }
        }

        protected virtual void OnDisable()
        {
            if (SnappedInteractable != null)
            {
                SnappedInteractable.gameObject.SetActive(false);
            }
        }
    }
}