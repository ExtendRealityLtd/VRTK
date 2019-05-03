namespace VRTK.Prefabs.Locomotion.Teleporters
{
    using UnityEngine;
    using Malimbe.MemberChangeMethod;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Rule;
    using Zinnia.Data.Type;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Modification;

    /// <summary>
    /// The public interface into the Teleporter Prefab.
    /// </summary>
    public class TeleporterFacade : MonoBehaviour
    {
        /// <summary>
        /// The type of offset to apply when teleporting.
        /// </summary>
        public enum OffsetType
        {
            /// <summary>
            /// Updates the teleported position with the <see cref="Offset"/> affecting the position but the destination rotation has no effect on the teleported rotation.
            /// </summary>
            OffsetAlwaysIgnoreDestinationRotation,
            /// <summary>
            /// Updates the teleported position with the <see cref="Offset"/> affecting the position and the destination rotation affecting the teleported rotation.
            /// </summary>
            OffsetAlwaysWithDestinationRotation,
            /// <summary>
            /// Updates the teleported position but only uses the <see cref="Offset"/> when affecting the floor snap position but the destination rotation has no effect on the teleported rotation.
            /// </summary>
            OffsetFloorSnapOnlyIgnoreDestinationRotation
        }

        #region Teleporter Settings
        /// <summary>
        /// The target to move to the teleported position.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Teleporter Settings"), DocumentedByXml]
        public GameObject Target { get; set; }
        /// <summary>
        /// The offset to compensate the teleported target position by for both floor snapping and position movement.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject Offset { get; set; }
        /// <summary>
        /// Determines how to use the <see cref="Offset"/> when calculating the teleport location.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public OffsetType OffsetUsage { get; set; }
        /// <summary>
        /// Determines if the teleport destination <see cref="Transform.rotation"/> will be applied to the <see cref="Target"/>.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public bool ApplyDestinationRotation { get; set; } = true;
        /// <summary>
        /// The <see cref="CameraList"/> of scene <see cref="Camera"/>s to apply a fade to.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public RuleContainer CameraValidity { get; set; }
        /// <summary>
        /// Allows to optionally determine targets based on the set rules.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public RuleContainer TargetValidity { get; set; }
        #endregion

        #region Teleporter Events
        /// <summary>
        /// Emitted when the teleporting is about to initiate.
        /// </summary>
        [Header("Teleporter Events"), DocumentedByXml]
        public TransformPropertyApplier.UnityEvent Teleporting = new TransformPropertyApplier.UnityEvent();
        /// <summary>
        /// Emitted when the teleporting has completed.
        /// </summary>
        [DocumentedByXml]
        public TransformPropertyApplier.UnityEvent Teleported = new TransformPropertyApplier.UnityEvent();
        #endregion

        #region Reference Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public TeleporterConfigurator Configuration { get; protected set; }
        #endregion

        /// <summary>
        /// Attempts to teleport the <see cref="Target"/>.
        /// </summary>
        /// <param name="destination">The location to attempt to teleport to.</param>
        public virtual void Teleport(TransformData destination)
        {
            Configuration.Teleport(destination);
        }

        /// <summary>
        /// Sets <see cref="OffsetUsage"/>.
        /// </summary>
        /// <param name="offsetTypeIndex">The index of the <see cref="OffsetType"/>.</param>
        public virtual void SetOffsetUsage(int offsetTypeIndex)
        {
            OffsetUsage = (OffsetType)Mathf.Clamp(offsetTypeIndex, 0, System.Enum.GetValues(typeof(OffsetType)).Length);
        }

        /// <summary>
        /// Called after <see cref="Target"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Target))]
        protected virtual void OnAfterTargetChange()
        {
            Configuration.ConfigureTransformPropertyAppliers();
        }

        /// <summary>
        /// Called after <see cref="Offset"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Offset))]
        protected virtual void OnAfterOffsetChange()
        {
            Configuration.ConfigureSurfaceLocatorAliases();
            Configuration.ConfigureTransformPropertyAppliers();
        }

        /// <summary>
        /// Called after <see cref="OffsetUsage"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(OffsetUsage))]
        protected virtual void OnAfterOffsetUsageChange()
        {
            Configuration.ConfigureTransformPropertyAppliers();
        }

        /// <summary>
        /// Called after <see cref="ApplyDestinationRotation"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(ApplyDestinationRotation))]
        protected virtual void OnAfterApplyDestinationRotationChange()
        {
            Configuration.ConfigureRotationAbility(ApplyDestinationRotation);
        }

        /// <summary>
        /// Called after <see cref="CameraValidity"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(CameraValidity))]
        protected virtual void OnAfterCameraValidityChange()
        {
            Configuration.ConfigureCameraColorOverlays();
        }

        /// <summary>
        /// Called after <see cref="TargetValidity"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(TargetValidity))]
        protected virtual void OnAfterTargetValidityChange()
        {
            Configuration.ConfigureSurfaceLocatorRules();
        }
    }
}