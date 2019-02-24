namespace VRTK.Prefabs.CameraRig.TrackedAlias
{
    using UnityEngine;
    using UnityEngine.Events;
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
        public List<LinkedAliasAssociationCollection> cameraRigs = new List<LinkedAliasAssociationCollection>();
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
        public GameObject ActivePlayArea
        {
            get
            {
                foreach (GameObject playArea in PlayAreas)
                {
                    if (playArea.gameObject.activeInHierarchy)
                    {
                        return playArea;
                    }
                }

                return null;
            }
        }
        /// <summary>
        /// Retrieves the active Headset that the TrackedAlias is using.
        /// </summary>
        public GameObject ActiveHeadset
        {
            get
            {
                foreach (GameObject headset in Headsets)
                {
                    if (headset.gameObject.activeInHierarchy)
                    {
                        return headset;
                    }
                }

                return null;
            }
        }
        /// <summary>
        /// Retrieves the active Headset Camera that the TrackedAlias is using.
        /// </summary>
        public Camera ActiveHeadsetCamera
        {
            get
            {
                foreach (Camera headsetCamera in HeadsetCameras)
                {
                    if (headsetCamera.gameObject.activeInHierarchy)
                    {
                        return headsetCamera;
                    }
                }

                return null;
            }
        }
        /// <summary>
        /// Retrieves the active Headset Velocity Tracker that the TrackedAlias is using.
        /// </summary>
        public VelocityTracker ActiveHeadsetVelocity
        {
            get
            {
                foreach (VelocityTracker headsetVelocityTracker in HeadsetVelocityTrackers)
                {
                    if (headsetVelocityTracker.gameObject.activeInHierarchy)
                    {
                        return headsetVelocityTracker;
                    }
                }

                return null;
            }
        }
        /// <summary>
        /// Retrieves the active Left Controller that the TrackedAlias is using.
        /// </summary>
        public GameObject ActiveLeftController
        {
            get
            {
                foreach (GameObject leftController in LeftControllers)
                {
                    if (leftController.gameObject.activeInHierarchy)
                    {
                        return leftController;
                    }
                }

                return null;
            }
        }
        /// <summary>
        /// Retrieves the active Left Controller Velocity Tracker that the TrackedAlias is using.
        /// </summary>
        public VelocityTracker ActiveLeftControllerVelocity
        {
            get
            {
                foreach (VelocityTracker leftControllerVelocityTracker in LeftControllerVelocityTrackers)
                {
                    if (leftControllerVelocityTracker.gameObject.activeInHierarchy)
                    {
                        return leftControllerVelocityTracker;
                    }
                }

                return null;
            }
        }
        /// <summary>
        /// Retrieves the active Right Controller that the TrackedAlias is using.
        /// </summary>
        public GameObject ActiveRightController
        {
            get
            {
                foreach (GameObject rightController in RightControllers)
                {
                    if (rightController.gameObject.activeInHierarchy)
                    {
                        return rightController;
                    }
                }

                return null;
            }
        }
        /// <summary>
        /// Retrieves the active Right Controller Velocity Tracker that the TrackedAlias is using.
        /// </summary>
        public VelocityTracker ActiveRightControllerVelocity
        {
            get
            {
                foreach (VelocityTracker rightControllerVelocityTracker in RightControllerVelocityTrackers)
                {
                    if (rightControllerVelocityTracker.gameObject.activeInHierarchy)
                    {
                        return rightControllerVelocityTracker;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Retrieves all of the linked CameraRig PlayAreas.
        /// </summary>
        public IEnumerable<GameObject> PlayAreas
        {
            get
            {
                foreach (LinkedAliasAssociationCollection cameraRig in cameraRigs)
                {
                    GameObject playArea = cameraRig.playArea;
                    if (playArea != null)
                    {
                        yield return playArea;
                    }
                }
            }
        }
        /// <summary>
        /// Retrieves all of the linked CameraRig Headsets.
        /// </summary>
        public IEnumerable<GameObject> Headsets
        {
            get
            {
                foreach (LinkedAliasAssociationCollection cameraRig in cameraRigs)
                {
                    GameObject headset = cameraRig.headset;
                    if (headset != null)
                    {
                        yield return headset;
                    }
                }
            }
        }
        /// <summary>
        /// Retrieves all of the linked CameraRig Headset Cameras.
        /// </summary>
        public IEnumerable<Camera> HeadsetCameras
        {
            get
            {
                foreach (LinkedAliasAssociationCollection cameraRig in cameraRigs)
                {
                    Camera headsetCamera = cameraRig.headsetCamera;
                    if (headsetCamera != null)
                    {
                        yield return headsetCamera;
                    }
                }
            }
        }
        /// <summary>
        /// Retrieves all of the linked CameraRig Headset Velocity Trackers.
        /// </summary>
        public IEnumerable<VelocityTracker> HeadsetVelocityTrackers
        {
            get
            {
                foreach (LinkedAliasAssociationCollection cameraRig in cameraRigs)
                {
                    VelocityTracker headsetVelocityTracker = cameraRig.headsetVelocityTracker;
                    if (headsetVelocityTracker != null)
                    {
                        yield return headsetVelocityTracker;
                    }
                }
            }
        }
        /// <summary>
        /// Retrieves all of the linked CameraRig Left Controllers.
        /// </summary>
        public IEnumerable<GameObject> LeftControllers
        {
            get
            {
                foreach (LinkedAliasAssociationCollection cameraRig in cameraRigs)
                {
                    GameObject leftController = cameraRig.leftController;
                    if (leftController != null)
                    {
                        yield return leftController;
                    }
                }
            }
        }
        /// <summary>
        /// Retrieves all of the linked CameraRig Right Controllers.
        /// </summary>
        public IEnumerable<GameObject> RightControllers
        {
            get
            {
                foreach (LinkedAliasAssociationCollection cameraRig in cameraRigs)
                {
                    GameObject rightController = cameraRig.rightController;
                    if (rightController != null)
                    {
                        yield return rightController;
                    }
                }
            }
        }
        /// <summary>
        /// Retrieves all of the linked CameraRig Left Controller Velocity Trackers.
        /// </summary>
        public IEnumerable<VelocityTracker> LeftControllerVelocityTrackers
        {
            get
            {
                foreach (LinkedAliasAssociationCollection cameraRig in cameraRigs)
                {
                    VelocityTracker leftControllerVelocityTracker = cameraRig.leftControllerVelocityTracker;
                    if (leftControllerVelocityTracker != null)
                    {
                        yield return leftControllerVelocityTracker;
                    }
                }
            }
        }
        /// <summary>
        /// Retrieves all of the linked CameraRig Right Controller Velocity Trackers.
        /// </summary>
        public IEnumerable<VelocityTracker> RightControllerVelocityTrackers
        {
            get
            {
                foreach (LinkedAliasAssociationCollection cameraRig in cameraRigs)
                {
                    VelocityTracker rightControllerVelocityTracker = cameraRig.rightControllerVelocityTracker;
                    if (rightControllerVelocityTracker != null)
                    {
                        yield return rightControllerVelocityTracker;
                    }
                }
            }
        }
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