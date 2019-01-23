namespace VRTK.Prefabs.CameraRig.TrackedAlias
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Linq;
    using System.Collections.Generic;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Follow;
    using Zinnia.Tracking.Velocity;
    using Zinnia.Tracking.CameraRig;

    /// <summary>
    /// The public interface into the Tracked Alias Prefab.
    /// </summary>
    public class TrackedAliasFacade : MonoBehaviour
    {
        #region Tracked Alias Settings
        /// <summary>
        /// The associated CameraRigs to track.
        /// </summary>
        [Header("Tracked Alias Settings"), Tooltip("The associated CameraRigs to track.")]
        public List<AliasAssociationCollection> cameraRigs = new List<AliasAssociationCollection>();
        #endregion

        #region Tracking Begun Events
        /// <summary>
        /// Emitted when the headset starts tracking for the first time.
        /// </summary>
        [Header("Tracking Begun Events")]
        public UnityEvent HeadsetTrackingBegun = new UnityEvent();
        /// <summary>
        /// Emitted when the left controller starts tracking for the first time.
        /// </summary>
        public UnityEvent LeftControllerTrackingBegun = new UnityEvent();
        /// <summary>
        /// Emitted when the right controller starts tracking for the first time.
        /// </summary>
        public UnityEvent RightControllerTrackingBegun = new UnityEvent();
        #endregion

        #region Internal Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Header("Internal Settings"), Tooltip("The linked Internal Setup."), InternalSetting, SerializeField]
        protected TrackedAliasInternalSetup internalSetup;
        #endregion

        /// <summary>
        /// Retrieves the active PlayArea that the TrackedAlias is using.
        /// </summary>
        public GameObject ActivePlayArea => PlayAreas.Select(element => element.gameObject).FirstOrDefault(gameObject => gameObject.activeInHierarchy);
        /// <summary>
        /// Retrieves the active Headset that the TrackedAlias is using.
        /// </summary>
        public GameObject ActiveHeadset => Headsets.Select(element => element.gameObject).FirstOrDefault(gameObject => gameObject.activeInHierarchy);
        /// <summary>
        /// Retrieves the active Headset Camera that the TrackedAlias is using.
        /// </summary>
        public Camera ActiveHeadsetCamera => HeadsetCameras.Select(element => element).FirstOrDefault(camera => camera.gameObject.activeInHierarchy);
        /// <summary>
        /// Retrieves the active Headset Velocity Tracker that the TrackedAlias is using.
        /// </summary>
        public VelocityTracker ActiveHeadsetVelocity => HeadsetVelocityTrackers.Select(element => element).FirstOrDefault(velocityTracker => velocityTracker.gameObject.activeInHierarchy);
        /// <summary>
        /// Retrieves the active Left Controller that the TrackedAlias is using.
        /// </summary>
        public GameObject ActiveLeftController => LeftControllers.Select(element => element.gameObject).FirstOrDefault(gameObject => gameObject.activeInHierarchy);
        /// <summary>
        /// Retrieves the active Left Controller Velocity Tracker that the TrackedAlias is using.
        /// </summary>
        public VelocityTracker ActiveLeftControllerVelocity => LeftControllerVelocityTrackers.Select(element => element).FirstOrDefault(velocityTracker => velocityTracker.gameObject.activeInHierarchy);
        /// <summary>
        /// Retrieves the active Right Controller that the TrackedAlias is using.
        /// </summary>
        public GameObject ActiveRightController => RightControllers.Select(element => element.gameObject).FirstOrDefault(gameObject => gameObject.activeInHierarchy);
        /// <summary>
        /// Retrieves the active Right Controller Velocity Tracker that the TrackedAlias is using.
        /// </summary>
        public VelocityTracker ActiveRightControllerVelocity => RightControllerVelocityTrackers.Select(element => element).FirstOrDefault(velocityTracker => velocityTracker.gameObject.activeInHierarchy);

        /// <summary>
        /// Retreives all of the linked CameraRig PlayAreas.
        /// </summary>
        public List<GameObject> PlayAreas => cameraRigs.Select(rig => rig.PlayArea).Where(value => value != null).ToList();
        /// <summary>
        /// Retreives all of the linked CameraRig Headsets.
        /// </summary>
        public List<GameObject> Headsets => cameraRigs.Select(rig => rig.Headset).Where(value => value != null).ToList();
        /// <summary>
        /// Retreives all of the linked CameraRig Headset Cameras.
        /// </summary>
        public List<Camera> HeadsetCameras => cameraRigs.Select(rig => rig.HeadsetCamera).Where(value => value != null).ToList();
        /// <summary>
        /// Retreives all of the linked CameraRig Headset Velocity Trackers.
        /// </summary>
        public List<VelocityTracker> HeadsetVelocityTrackers => cameraRigs.Select(rig => rig.HeadsetVelocity).Where(value => value != null).ToList();
        /// <summary>
        /// Retreives all of the linked CameraRig Left Controllers.
        /// </summary>
        public List<GameObject> LeftControllers => cameraRigs.Select(rig => rig.LeftController).Where(value => value != null).ToList();
        /// <summary>
        /// Retreives all of the linked CameraRig Right Controllers.
        /// </summary>
        public List<GameObject> RightControllers => cameraRigs.Select(rig => rig.RightController).Where(value => value != null).ToList();
        /// <summary>
        /// Retreives all of the linked CameraRig Left Controller Velocity Trackers.
        /// </summary>
        public List<VelocityTracker> LeftControllerVelocityTrackers => cameraRigs.Select(rig => rig.LeftControllerVelocity).Where(value => value != null).ToList();
        /// <summary>
        /// Retreives all of the linked CameraRig Right Controller Velocity Trackers.
        /// </summary>
        public List<VelocityTracker> RightControllerVelocityTrackers => cameraRigs.Select(rig => rig.RightControllerVelocity).Where(value => value != null).ToList();
        /// <summary>
        /// The alias follower for the PlayArea.
        /// </summary>
        public ObjectFollower PlayAreaAlias => internalSetup.PlayArea;
        /// <summary>
        /// The alias follower for the Headset.
        /// </summary>
        public ObjectFollower HeadsetAlias => internalSetup.Headset;
        /// <summary>
        /// The alias follower for the LeftController.
        /// </summary>
        public ObjectFollower LeftControllerAlias => internalSetup.LeftController;
        /// <summary>
        /// The alias follower for the RightController.
        /// </summary>
        public ObjectFollower RightControllerAlias => internalSetup.RightController;

        /// <summary>
        /// Refreshes any changes made to the <see cref="cameraRigs"/> collection.
        /// </summary>
        public virtual void RefreshCameraRigsConfiguration()
        {
            internalSetup.SetUpCameraRigsConfiguration();
        }

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            RefreshCameraRigsConfiguration();
        }
    }
}