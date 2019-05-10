namespace VRTK.Prefabs.Locomotion.DestinationLocations
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Data.Type;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// Sets up the Destination Location Prefab based on the provided user settings.
    /// </summary>
    public class DestinationLocationConfigurator : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public DestinationLocationFacade Facade { get; protected set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The <see cref="GameObject"/> holding the visualizations for locked states.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public GameObject LockedContainer { get; protected set; }
        /// <summary>
        /// The <see cref="GameObject"/> holding the visualizations for unlocked states.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObject UnlockedContainer { get; protected set; }
        /// <summary>
        /// The <see cref="DestinationLocationLockedStateTag"/> that determines if the location is locked.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public DestinationLocationLockedStateTag LockedTag { get; protected set; }
        /// <summary>
        /// The <see cref="DestinationLocation"/> that controls the functionality.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public DestinationLocation LocationController { get; protected set; }
        #endregion

        /// <summary>
        /// Sets the locked state of the Destination Location.
        /// </summary>
        /// <param name="isLocked"></param>
        public virtual void SetLockedState(bool isLocked)
        {
            LockedContainer.SetActive(isLocked);
            UnlockedContainer.SetActive(!isLocked);
            LockedTag.enabled = isLocked;
        }

        /// <summary>
        /// Emits the Hover Activated event.
        /// </summary>
        public virtual void EmitHoverActivated()
        {
            Facade.HoverActivated?.Invoke();
        }

        /// <summary>
        /// Emits the Entered event.
        /// </summary>
        public virtual void EmitEntered(SurfaceData data)
        {
            Facade.Entered?.Invoke(data);
        }

        /// <summary>
        /// Emits the Exited event.
        /// </summary>
        public virtual void EmitExited(SurfaceData data)
        {
            Facade.Exited?.Invoke(data);
        }

        /// <summary>
        /// Emits the Hover Deactivated event.
        /// </summary>
        public virtual void EmitHoverDeactivated()
        {
            Facade.HoverDeactivated?.Invoke();
        }

        /// <summary>
        /// Emits the Activated event.
        /// </summary>
        public virtual void EmitActivated(TransformData data)
        {
            Facade.Activated?.Invoke(data);
        }

        /// <summary>
        /// Emits the Deactivated event.
        /// </summary>
        public virtual void EmitDeactivated()
        {
            Facade.Deactivated?.Invoke();
        }

        protected virtual void OnEnable()
        {
            SetLockedState(Facade.IsLocked);
            LocationController.SourceValidity = Facade.SourceValidity;
            LocationController.ApplyDestinationRotation = Facade.ApplyDestinationRotation;
        }
    }
}