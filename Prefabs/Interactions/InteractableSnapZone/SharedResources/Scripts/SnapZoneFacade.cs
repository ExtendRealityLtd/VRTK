namespace VRTK.Prefabs.Interactions.InteractableSnapZone
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using Malimbe.MemberChangeMethod;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Rule;
    using Zinnia.Data.Type;
    using Zinnia.Data.Attribute;
    using VRTK.Prefabs.Interactions.Interactables;

    public class SnapZoneFacade : MonoBehaviour
    {
        /// <summary>
        /// Defines the event with the <see cref="GameObject"/>.
        /// </summary>
        [Serializable]
        public class UnityEvent : UnityEvent<GameObject> { }

        /// <summary>
        /// The state the SnapZone is in.
        /// </summary>
        public enum SnapZoneState
        {
            /// <summary>
            /// No valid <see cref="GameObject"/> is colliding with the SnapZone and nothing has been snapped into the SnapZone.
            /// </summary>
            ZoneIsEmpty,
            /// <summary>
            /// At least one valid <see cref="GameObject"/> is colliding with the SnapZone but nothing has been snapped into the SnapZone.
            /// </summary>
            ZoneIsActivated,
            /// <summary>
            /// A valid <see cref="GameObject"/> has been snapped into the SnapZone.
            /// </summary>
            ZoneIsSnapped
        }

        #region Snap Settings
        /// <summary>
        /// The rules that determine which <see cref="GameObject"/> can be snapped to this snap zone.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Snap Settings"), DocumentedByXml]
        public RuleContainer SnapValidity { get; set; }
        /// <summary>
        /// The duration for the transition of the snapped <see cref="GameObject"/> to reach the snap zone destination.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float TransitionDuration { get; set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The linked Configurator Setup.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public SnapZoneConfigurator Configuration { get; protected set; }
        #endregion

        #region Zone Events
        /// <summary>
        /// Emitted when a valid <see cref="GameObject"/> enters the zone.
        /// </summary>
        [Header("Zone Events"), DocumentedByXml]
        public UnityEvent Entered = new UnityEvent();
        /// <summary>
        /// Emitted when a valid <see cref="GameObject"/> exits the zone.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Exited = new UnityEvent();
        /// <summary>
        /// Emitted when a valid <see cref="GameObject"/> activates the zone.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Activated = new UnityEvent();
        /// <summary>
        /// Emitted when a valid <see cref="GameObject"/> deactivates the zone.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Deactivated = new UnityEvent();
        /// <summary>
        /// Emitted when a valid <see cref="GameObject"/> is snapped to the zone.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Snapped = new UnityEvent();
        /// <summary>
        /// Emitted when a valid <see cref="GameObject"/> is unsnapped from the zone.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Unsnapped = new UnityEvent();
        #endregion

        /// <summary>
        /// Returns the collection of <see cref="GameObject"/>s that are currently colliding with the snap zone and are valid to be snapped.
        /// </summary>
        public HeapAllocationFreeReadOnlyList<GameObject> SnappableGameObjects => Configuration.SnappableInteractables;
        /// <summary>
        /// Returns the currently snapped <see cref="GameObject"/>.
        /// </summary>
        public GameObject SnappedGameObject => Configuration.SnappedInteractable;
        /// <summary>
        /// The state of the SnapZone.
        /// </summary>
        public SnapZoneState ZoneState
        {
            get
            {
                if (SnappedGameObject != null)
                {
                    return SnapZoneState.ZoneIsSnapped;
                }
                else if (SnappableGameObjects.Count > 0)
                {
                    return SnapZoneState.ZoneIsActivated;
                }

                return SnapZoneState.ZoneIsEmpty;
            }
        }

        /// <summary>
        /// Attempts to snap a given <see cref="InteractableFacade"/> to the snap zone.
        /// </summary>
        /// <param name="interactableToSnap">The interactable to attempt to snap.</param>
        public virtual void Snap(InteractableFacade interactableToSnap)
        {
            Snap(interactableToSnap.gameObject);
        }

        /// <summary>
        /// Attempts to snap a given <see cref="GameObject"/> to the snap zone.
        /// </summary>
        /// <param name="objectToSnap">The object to attempt to snap.</param>
        public virtual void Snap(GameObject objectToSnap)
        {
            Configuration.Snap(objectToSnap);
        }

        /// <summary>
        /// Attempts to unsnap any existing <see cref="GameObject"/> that is currently snapped to the snap zone.
        /// </summary>
        public virtual void Unsnap()
        {
            Configuration.Unsnap();
        }

        /// <summary>
        /// Called after <see cref="SnapValidity"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(SnapValidity))]
        protected virtual void OnAfterSnapValidityChange()
        {
            Configuration.ConfigureValidityRules();
        }

        /// <summary>
        /// Called after <see cref="TransitionDuration"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(TransitionDuration))]
        protected virtual void OnAfterTransitionDurationChange()
        {
            Configuration.ConfigurePropertyApplier();
        }
    }
}