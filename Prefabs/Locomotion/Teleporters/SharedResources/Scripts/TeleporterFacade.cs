namespace VRTK.Prefabs.Locomotion.Teleporters
{
    using UnityEngine;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Type;
    using Zinnia.Rule;
    using Zinnia.Tracking.Modification;

    /// <summary>
    /// The public interface into the Teleporter Prefab.
    /// </summary>
    public class TeleporterFacade : MonoBehaviour
    {
        #region Teleporter Settings
        [Header("Teleporter Settings"), Tooltip("The target to move to the teleported position."), SerializeField]
        private GameObject _target;
        /// <summary>
        /// The target to move to the teleported position.
        /// </summary>
        public GameObject Target
        {
            get { return _target; }
            set
            {
                _target = value;
                internalSetup.ConfigureTransformPropertyAppliers();
            }
        }

        [Tooltip("The offset to compensate the teleported target position by for both floor snapping and position movement."), SerializeField]
        private GameObject _offset;
        /// <summary>
        /// The offset to compensate the teleported target position by for both floor snapping and position movement.
        /// </summary>
        public GameObject Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                internalSetup.ConfigureSurfaceLocatorAliases();
                internalSetup.ConfigureTransformPropertyAppliers();
            }
        }

        [Tooltip("Determines if only the floor snap should only be compensated by the offset or whether the teleported target position should also be compensated by the offset."), SerializeField]
        private bool _onlyOffsetFloorSnap;
        /// <summary>
        /// Determines if only the floor snap should only be compensated by the <see cref="offset"/> or whether the teleported target position should also be compensated by the <see cref="offset"/>.
        /// </summary>
        public bool OnlyOffsetFloorSnap
        {
            get { return _onlyOffsetFloorSnap; }
            set
            {
                _onlyOffsetFloorSnap = value;
                internalSetup.ConfigureTransformPropertyAppliers();
            }
        }

        [Tooltip("The list of scene Cameras to apply a fade to."), SerializeField]
        private CameraList _sceneCameras;
        /// <summary>
        /// The <see cref="CameraList"/> of scene <see cref="Camera"/>s to apply a fade to.
        /// </summary>
        public CameraList SceneCameras
        {
            get { return _sceneCameras; }
            set
            {
                _sceneCameras = value;
                internalSetup.ConfigureCameraColorOverlays();
            }
        }

        [Tooltip("Allows to optionally determine targets based on the set rules."), SerializeField]
        private RuleContainer _targetValidity;
        /// <summary>
        /// Allows to optionally determine targets based on the set rules.
        /// </summary>
        public RuleContainer TargetValidity
        {
            get { return _targetValidity; }
            set
            {
                _targetValidity = value;
                internalSetup.ConfigureSurfaceLocatorRules();
            }
        }
        #endregion

        #region Teleporter Events
        /// <summary>
        /// Emitted when the teleporting is about to initiate.
        /// </summary>
        [Header("Teleporter Events")]
        public TransformPropertyApplier.UnityEvent Teleporting = new TransformPropertyApplier.UnityEvent();
        /// <summary>
        /// Emitted when the teleporting has completed.
        /// </summary>
        public TransformPropertyApplier.UnityEvent Teleported = new TransformPropertyApplier.UnityEvent();
        #endregion

        #region Internal Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Header("Internal Settings"), Tooltip("The linked Internal Setup."), InternalSetting, SerializeField]
        protected TeleporterInternalSetup internalSetup;
        #endregion

        /// <summary>
        /// Attempts to teleport the <see cref="playAreaAlias"/>.
        /// </summary>
        /// <param name="destination">The location to attempt to teleport to.</param>
        public virtual void Teleport(TransformData destination)
        {
            internalSetup.Teleport(destination);
        }

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            internalSetup.ConfigureSurfaceLocatorAliases();
            internalSetup.ConfigureSurfaceLocatorRules();
            internalSetup.ConfigureTransformPropertyAppliers();
            internalSetup.ConfigureCameraColorOverlays();
        }
    }
}