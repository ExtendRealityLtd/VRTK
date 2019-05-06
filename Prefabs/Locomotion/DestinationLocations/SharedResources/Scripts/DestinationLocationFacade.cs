namespace VRTK.Prefabs.Locomotion.DestinationLocations
{
    using UnityEngine;
    using UnityEngine.Events;
    using Malimbe.MemberChangeMethod;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Rule;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface into the DestinationLocation Prefab.
    /// </summary>
    public class DestinationLocationFacade : MonoBehaviour
    {
        #region Location Settings
        /// <summary>
        /// Determines if the location is in the locked and unusable state.
        /// </summary>
        [Serialized]
        [field: Header("Location Settings"), DocumentedByXml]
        public bool IsLocked { get; set; }
        /// <summary>
        /// Whether to apply the rotation of the custom destination location to the selected action output.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public bool ApplyDestinationRotation { get; set; } = true;
        /// <summary>
        /// Allows to optionally determine which <see cref="SurfaceData"/> sources can affect the location.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public RuleContainer SourceValidity { get; set; }
        #endregion

        #region Location Events
        /// <summary>
        /// Emitted when the Destination Location is entered for the first time.
        /// </summary>
        [Header("Location Events"), DocumentedByXml]
        public UnityEvent HoverActivated = new UnityEvent();
        /// <summary>
        /// Emitted when the Destination Location is entered.
        /// </summary>
        [DocumentedByXml]
        public DestinationLocation.SurfaceDataUnityEvent Entered = new DestinationLocation.SurfaceDataUnityEvent();
        /// <summary>
        /// Emitted when the Destination Location is exited.
        /// </summary>
        [DocumentedByXml]
        public DestinationLocation.SurfaceDataUnityEvent Exited = new DestinationLocation.SurfaceDataUnityEvent();
        /// <summary>
        /// Emitted when the Destination Location is exited for the last time.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent HoverDeactivated = new UnityEvent();
        /// <summary>
        /// Emitted when the Destination Location is activated.
        /// </summary>
        [DocumentedByXml]
        public DestinationLocation.TransformDataUnityEvent Activated = new DestinationLocation.TransformDataUnityEvent();
        /// <summary>
        /// Emitted when the Destination Location is deactivated.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Deactivated = new UnityEvent();
        #endregion

        #region Reference Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public DestinationLocationConfigurator Configuration { get; protected set; }
        #endregion

        /// <summary>
        /// Called after <see cref="IsLocked"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(IsLocked))]
        protected virtual void OnAfterIsLockedChange()
        {
            Configuration.SetLockedState(IsLocked);
        }

        /// <summary>
        /// Called after <see cref="ApplyDestinationRotation"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(ApplyDestinationRotation))]
        protected virtual void OnAfterApplyDestinationRotationChange()
        {
            Configuration.LocationController.ApplyDestinationRotation = ApplyDestinationRotation;
        }

        /// <summary>
        /// Called after <see cref="SourceValidity"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(SourceValidity))]
        protected virtual void OnAfterSourceValidityChange()
        {
            Configuration.LocationController.SourceValidity = SourceValidity;
        }
    }
}