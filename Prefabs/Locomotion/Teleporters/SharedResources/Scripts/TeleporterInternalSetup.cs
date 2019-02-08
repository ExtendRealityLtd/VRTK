namespace VRTK.Prefabs.Locomotion.Teleporters
{
    using UnityEngine;
    using System.Collections.Generic;
    using Zinnia.Visual;
    using Zinnia.Extension;
    using Zinnia.Data.Type;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking;
    using Zinnia.Tracking.Modification;

    /// <summary>
    /// Sets up the Teleport Prefab based on the provided user settings.
    /// </summary>
    public class TeleporterInternalSetup : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Header("Facade Settings"), Tooltip("The public interface facade."), InternalSetting, SerializeField]
        protected TeleporterFacade facade;
        #endregion

        #region Teleporter Settings
        /// <summary>
        /// The <see cref="SurfaceLocator"/> to use for the teleporting event.
        /// </summary>
        [Header("Teleporter Settings"), Tooltip("The Surface Locator to use for the teleporting event."), InternalSetting, SerializeField]
        protected SurfaceLocator surfaceTeleporter;
        /// <summary>
        /// The <see cref="TransformPropertyApplier"/> to use for the teleporting event.
        /// </summary>
        [Tooltip("The Transform Property Applier to use for the teleporting event."), InternalSetting, SerializeField]
        protected TransformPropertyApplier modifyTeleporter;
        #endregion

        #region Alias Settings
        /// <summary>
        /// The <see cref="SurfaceLocator"/> to set aliases on.
        /// </summary>
        [Header("Alias Settings"), Tooltip("The Surface Locators to set aliases on."), InternalSetting, SerializeField]
        protected List<SurfaceLocator> surfaceLocatorAliases = new List<SurfaceLocator>();
        /// <summary>
        /// The <see cref="SurfaceLocator"/> to set rules on.
        /// </summary>
        [Tooltip("The Surface Locators to set rules on."), InternalSetting, SerializeField]
        protected List<SurfaceLocator> surfaceLocatorRules = new List<SurfaceLocator>();
        /// <summary>
        /// The <see cref="TransformPropertyApplier"/> collection to set aliases on.
        /// </summary>
        [Tooltip("The Transform Property Applier collection to set aliases on."), InternalSetting, SerializeField]
        protected List<TransformPropertyApplier> transformPropertyApplierAliases = new List<TransformPropertyApplier>();
        /// <summary>
        /// The <see cref="TransformPropertyApplier"/> collection to ignore offsets on.
        /// </summary>
        [Tooltip("The Transform Property Applier collection to ignore offsets on."), InternalSetting, SerializeField]
        protected List<TransformPropertyApplier> transformPropertyApplierIgnoreOffsetAliases = new List<TransformPropertyApplier>();
        /// <summary>
        /// The scene <see cref="Camera"/>s to set the <see cref="CameraColorOverlay"/>s to affect.
        /// </summary>
        [Tooltip("The scene Cameras to set the CameraColorOverlays to affect."), InternalSetting, SerializeField]
        protected List<CameraColorOverlay> cameraColorOverlays = new List<CameraColorOverlay>();
        #endregion

        /// <summary>
        /// Attempts to teleport the <see cref="playAreaAlias"/>.
        /// </summary>
        /// <param name="destination">The location to attempt to teleport to.</param>
        public virtual void Teleport(TransformData destination)
        {
            if (surfaceTeleporter != null)
            {
                surfaceTeleporter.Locate(destination);
            }

            if (modifyTeleporter != null)
            {
                modifyTeleporter.Source = destination;
                modifyTeleporter.Apply();
            }
        }

        /// <summary>
        /// Notifies that the teleporter is about to initiate.
        /// </summary>
        /// <param name="data">The location data.</param>
        public virtual void NotifyTeleporting(TransformPropertyApplier.EventData data)
        {
            facade.Teleporting?.Invoke(data);
        }

        /// <summary>
        /// Notifies that the teleporter has completed.
        /// </summary>
        /// <param name="data">The location data.</param>
        public virtual void NotifyTeleported(TransformPropertyApplier.EventData data)
        {
            facade.Teleported?.Invoke(data);
        }

        /// <summary>
        /// Configures the surface locator aliases with the offset provided in the facade.
        /// </summary>
        public virtual void ConfigureSurfaceLocatorAliases()
        {
            foreach (SurfaceLocator currentLocator in surfaceLocatorAliases.EmptyIfNull())
            {
                currentLocator.SearchOrigin = facade.Offset;
            }
        }

        /// <summary>
        /// Configures the surface locator rules with the valid targets provided in the facade.
        /// </summary>
        public virtual void ConfigureSurfaceLocatorRules()
        {
            foreach (SurfaceLocator currentLocator in surfaceLocatorRules.EmptyIfNull())
            {
                currentLocator.targetValidity = facade.TargetValidity;
            }
        }

        /// <summary>
        /// Configures the transform properties applies with the settings applied in the facade.
        /// </summary>
        public virtual void ConfigureTransformPropertyAppliers()
        {
            foreach (TransformPropertyApplier currentApplier in transformPropertyApplierAliases.EmptyIfNull())
            {
                currentApplier.Target = facade.Target;
                currentApplier.Offset = null;
                if (!facade.OnlyOffsetFloorSnap || !transformPropertyApplierIgnoreOffsetAliases.Contains(currentApplier))
                {
                    currentApplier.Offset = facade.Offset;
                }
            }
        }

        /// <summary>
        /// Configures the camera color overlays with the scene cameras provided in the facade.
        /// </summary>
        public virtual void ConfigureCameraColorOverlays()
        {
            foreach (CameraColorOverlay currentOverlay in cameraColorOverlays.EmptyIfNull())
            {
                currentOverlay.validCameras = facade.SceneCameras;
            }
        }

        protected virtual void OnEnable()
        {
            ConfigureSurfaceLocatorAliases();
            ConfigureSurfaceLocatorRules();
            ConfigureTransformPropertyAppliers();
            ConfigureCameraColorOverlays();
        }
    }
}