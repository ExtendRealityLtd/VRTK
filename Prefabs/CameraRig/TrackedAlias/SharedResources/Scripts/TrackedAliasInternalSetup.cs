namespace VRTK.Prefabs.CameraRig.TrackedAlias
{
    using UnityEngine;
    using Zinnia.Extension;
    using Zinnia.Data.Type;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Follow;
    using Zinnia.Tracking.Velocity;

    /// <summary>
    /// Sets up the Tracked Alias Prefab based on the provided user settings.
    /// </summary>
    public class TrackedAliasInternalSetup : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Header("Facade Settings"), Tooltip("The public interface facade."), InternalSetting, SerializeField]
        protected TrackedAliasFacade facade;
        #endregion

        #region Object Follow Settings
        [Header("Object Follow Settings"), Tooltip("The ObjectFollower component for the PlayArea."), InternalSetting, SerializeField]
        private ObjectFollower _playArea = null;
        /// <summary>
        /// The <see cref="ObjectFollower"/> component for the PlayArea.
        /// </summary>
        public ObjectFollower PlayArea => _playArea;

        [Tooltip("The ObjectFollower component for the Headset."), InternalSetting, SerializeField]
        private ObjectFollower _headset = null;
        /// <summary>
        /// The <see cref="ObjectFollower"/> component for the Headset.
        /// </summary>
        public ObjectFollower Headset => _headset;

        [Tooltip("The ObjectFollower component for the Left Controller."), InternalSetting, SerializeField]
        private ObjectFollower _leftController = null;
        /// <summary>
        /// The <see cref="ObjectFollower"/> component for the Left Controller.
        /// </summary>
        public ObjectFollower LeftController => _leftController;

        [Tooltip("The ObjectFollower component for the Right Controller."), InternalSetting, SerializeField]
        private ObjectFollower _rightController = null;
        /// <summary>
        /// The <see cref="ObjectFollower"/> component for the Right Controller.
        /// </summary>
        public ObjectFollower RightController => _rightController;
        #endregion

        #region Velocity Tracker Settings
        /// <summary>
        /// The <see cref="VelocityTrackerProcessor"/> component containing the Headset Velocity Trackers.
        /// </summary>
        [Header("Velocity Tracker Settings"), Tooltip("The VelocityTrackerProcessor component containing the Headset Velocity Trackers."), InternalSetting, SerializeField]
        protected VelocityTrackerProcessor headsetVelocityTrackers;
        /// <summary>
        /// The <see cref="VelocityTrackerProcessor"/> component containing the Left Controller Velocity Trackers.
        /// </summary>
        [Tooltip("The VelocityTrackerProcessor component containing the Left Controller Velocity Trackers."), InternalSetting, SerializeField]
        protected VelocityTrackerProcessor leftControllerVelocityTrackers;
        /// <summary>
        /// The <see cref="VelocityTrackerProcessor"/> component containing the Right Controller Velocity Trackers.
        /// </summary>
        [Tooltip("The VelocityTrackerProcessor component containing the Right Controller Velocity Trackers."), InternalSetting, SerializeField]
        protected VelocityTrackerProcessor rightControllerVelocityTrackers;
        #endregion

        #region Other Settings
        /// <summary>
        /// The <see cref="CameraList"/> component containing the valid scene cameras.
        /// </summary>
        [Header("Other Settings"), Tooltip("The CameraList component containing the valid scene cameras."), InternalSetting, SerializeField]
        protected CameraList sceneCameras;
        #endregion

        /// <summary>
        /// Sets up the TrackedAlias prefab with the specified settings.
        /// </summary>
        public virtual void SetUpCameraRigsConfiguration()
        {
            PlayArea.ClearTargets();
            PlayArea.targets.AddRange(facade.PlayAreas.EmptyIfNull());

            Headset.ClearSources();
            Headset.sources.AddRange(facade.Headsets.EmptyIfNull());

            sceneCameras.cameras.Clear();
            sceneCameras.cameras.AddRange(facade.HeadsetCameras.EmptyIfNull());

            headsetVelocityTrackers.velocityTrackers.Clear();
            headsetVelocityTrackers.velocityTrackers.AddRange(facade.HeadsetVelocityTrackers.EmptyIfNull());

            LeftController.ClearSources();
            LeftController.sources.AddRange(facade.LeftControllers.EmptyIfNull());

            RightController.ClearSources();
            RightController.sources.AddRange(facade.RightControllers.EmptyIfNull());

            leftControllerVelocityTrackers.velocityTrackers.Clear();
            leftControllerVelocityTrackers.velocityTrackers.AddRange(facade.LeftControllerVelocityTrackers.EmptyIfNull());

            rightControllerVelocityTrackers.velocityTrackers.Clear();
            rightControllerVelocityTrackers.velocityTrackers.AddRange(facade.RightControllerVelocityTrackers.EmptyIfNull());
        }

        /// <summary>
        /// Notifies that the headset has started being tracked.
        /// </summary>
        public virtual void NotifyHeadsetTrackingBegun()
        {
            facade.HeadsetTrackingBegun?.Invoke();
        }

        /// <summary>
        /// Notifies that the left controller has started being tracked.
        /// </summary>
        public virtual void NotifyLeftControllerTrackingBegun()
        {
            facade.LeftControllerTrackingBegun?.Invoke();
        }

        /// <summary>
        /// Notifies that the right controller has started being tracked.
        /// </summary>
        public virtual void NotifyRightControllerTrackingBegun()
        {
            facade.RightControllerTrackingBegun?.Invoke();
        }

        protected virtual void OnEnable()
        {
            SetUpCameraRigsConfiguration();
        }
    }
}