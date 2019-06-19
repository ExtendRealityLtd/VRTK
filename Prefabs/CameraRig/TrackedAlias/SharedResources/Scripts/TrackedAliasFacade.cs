namespace VRTK.Prefabs.CameraRig.TrackedAlias
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.MemberChangeMethod;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Follow;
    using Zinnia.Tracking.Velocity;
    using Zinnia.Tracking.CameraRig;
    using Zinnia.Tracking.CameraRig.Collection;

    /// <summary>
    /// The public interface into the Tracked Alias Prefab.
    /// </summary>
    public class TrackedAliasFacade : MonoBehaviour
    {
        #region Tracked Alias Settings
        /// <summary>
        /// The associated CameraRigs to track.
        /// </summary>
        [Serialized]
        [field: Header("Tracked Alias Settings"), DocumentedByXml]
        public LinkedAliasAssociationCollectionObservableList CameraRigs { get; set; }
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

        #region Reference Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public TrackedAliasConfigurator Configuration { get; protected set; }
        #endregion

        /// <summary>
        /// Retrieves the active PlayArea that the TrackedAlias is using.
        /// </summary>
        public GameObject ActivePlayArea => GetFirstActiveGameObject(PlayAreas);
        /// <summary>
        /// Retrieves the active Headset that the TrackedAlias is using.
        /// </summary>
        public GameObject ActiveHeadset => GetFirstActiveGameObject(Headsets);
        /// <summary>
        /// Retrieves the active Headset Camera that the TrackedAlias is using.
        /// </summary>
        public Camera ActiveHeadsetCamera => GetFirstActiveCamera(HeadsetCameras);
        /// <summary>
        /// Retrieves the active Headset Velocity Tracker that the TrackedAlias is using.
        /// </summary>
        public VelocityTracker ActiveHeadsetVelocity => GetFirstActiveVelocityTracker(HeadsetVelocityTrackers);
        /// <summary>
        /// Retrieves the active Left Controller that the TrackedAlias is using.
        /// </summary>
        public GameObject ActiveLeftController => GetFirstActiveGameObject(LeftControllers);
        /// <summary>
        /// Retrieves the active Left Controller Velocity Tracker that the TrackedAlias is using.
        /// </summary>
        public VelocityTracker ActiveLeftControllerVelocity => GetFirstActiveVelocityTracker(LeftControllerVelocityTrackers);
        /// <summary>
        /// Retrieves the active Right Controller that the TrackedAlias is using.
        /// </summary>
        public GameObject ActiveRightController => GetFirstActiveGameObject(RightControllers);
        /// <summary>
        /// Retrieves the active Right Controller Velocity Tracker that the TrackedAlias is using.
        /// </summary>
        public VelocityTracker ActiveRightControllerVelocity => GetFirstActiveVelocityTracker(RightControllerVelocityTrackers);

        /// <summary>
        /// Retrieves all of the linked CameraRig PlayAreas.
        /// </summary>
        public IEnumerable<GameObject> PlayAreas
        {
            get
            {
                if (CameraRigs == null)
                {
                    yield break;
                }

                foreach (LinkedAliasAssociationCollection cameraRig in CameraRigs.NonSubscribableElements)
                {
                    if (cameraRig == null)
                    {
                        continue;
                    }

                    GameObject playArea = cameraRig.PlayArea;
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
                if (CameraRigs == null)
                {
                    yield break;
                }

                foreach (LinkedAliasAssociationCollection cameraRig in CameraRigs.NonSubscribableElements)
                {
                    if (cameraRig == null)
                    {
                        continue;
                    }

                    GameObject headset = cameraRig.Headset;
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
                if (CameraRigs == null)
                {
                    yield break;
                }

                foreach (LinkedAliasAssociationCollection cameraRig in CameraRigs.NonSubscribableElements)
                {
                    if (cameraRig == null)
                    {
                        continue;
                    }

                    Camera headsetCamera = cameraRig.HeadsetCamera;
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
                if (CameraRigs == null)
                {
                    yield break;
                }

                foreach (LinkedAliasAssociationCollection cameraRig in CameraRigs.NonSubscribableElements)
                {
                    if (cameraRig == null)
                    {
                        continue;
                    }

                    VelocityTracker headsetVelocityTracker = cameraRig.HeadsetVelocityTracker;
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
                if (CameraRigs == null)
                {
                    yield break;
                }

                foreach (LinkedAliasAssociationCollection cameraRig in CameraRigs.NonSubscribableElements)
                {
                    if (cameraRig == null)
                    {
                        continue;
                    }

                    GameObject leftController = cameraRig.LeftController;
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
                if (CameraRigs == null)
                {
                    yield break;
                }

                foreach (LinkedAliasAssociationCollection cameraRig in CameraRigs.NonSubscribableElements)
                {
                    if (cameraRig == null)
                    {
                        continue;
                    }

                    GameObject rightController = cameraRig.RightController;
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
                if (CameraRigs == null)
                {
                    yield break;
                }

                foreach (LinkedAliasAssociationCollection cameraRig in CameraRigs.NonSubscribableElements)
                {
                    if (cameraRig == null)
                    {
                        continue;
                    }

                    VelocityTracker leftControllerVelocityTracker = cameraRig.LeftControllerVelocityTracker;
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
                if (CameraRigs == null)
                {
                    yield break;
                }

                foreach (LinkedAliasAssociationCollection cameraRig in CameraRigs.NonSubscribableElements)
                {
                    if (cameraRig == null)
                    {
                        continue;
                    }

                    VelocityTracker rightControllerVelocityTracker = cameraRig.RightControllerVelocityTracker;
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
        public ObjectFollower PlayAreaAlias => Configuration.PlayArea;
        /// <summary>
        /// The alias follower for the Headset.
        /// </summary>
        public ObjectFollower HeadsetAlias => Configuration.Headset;
        /// <summary>
        /// The alias follower for the LeftController.
        /// </summary>
        public ObjectFollower LeftControllerAlias => Configuration.LeftController;
        /// <summary>
        /// The alias follower for the RightController.
        /// </summary>
        public ObjectFollower RightControllerAlias => Configuration.RightController;

        protected virtual void OnEnable()
        {
            SubscribeToCameraRigsEvents();
        }

        protected virtual void OnDisable()
        {
            UnsubscribeFromCameraRigsEvents();
        }

        /// <summary>
        /// Subscribes to the events on the current <see cref="CameraRigs"/> collection.
        /// </summary>
        protected virtual void SubscribeToCameraRigsEvents()
        {
            if (CameraRigs == null)
            {
                return;
            }

            CameraRigs.Added.AddListener(OnCameraRigAdded);
            CameraRigs.Removed.AddListener(OnCameraRigRemoved);
        }

        /// <summary>
        /// Unsubscribes from the events on the current <see cref="CameraRigs"/> collection.
        /// </summary>
        protected virtual void UnsubscribeFromCameraRigsEvents()
        {
            if (CameraRigs == null)
            {
                return;
            }

            CameraRigs.Added.RemoveListener(OnCameraRigAdded);
            CameraRigs.Removed.RemoveListener(OnCameraRigRemoved);
        }

        /// <summary>
        /// Occurs when an item is added to the <see cref="CameraRigs"/> collection.
        /// </summary>
        /// <param name="cameraRig">The added element.</param>
        protected virtual void OnCameraRigAdded(LinkedAliasAssociationCollection cameraRig)
        {
            RefreshCameraRigsConfiguration();
        }

        /// <summary>
        /// Occurs when an item is removed from the <see cref="CameraRigs"/> collection.
        /// </summary>
        /// <param name="cameraRig">The removed element.</param>
        protected virtual void OnCameraRigRemoved(LinkedAliasAssociationCollection cameraRig)
        {
            RefreshCameraRigsConfiguration();
        }

        /// <summary>
        /// Refreshes any changes made to the <see cref="CameraRigs"/> collection.
        /// </summary>
        protected virtual void RefreshCameraRigsConfiguration()
        {
            Configuration.SetUpCameraRigsConfiguration();
        }

        /// <summary>
        /// Gets the first active <see cref="GameObject"/> found in the given collection.
        /// </summary>
        /// <param name="collection">The collection to look for the first active in.</param>
        /// <returns>The found first active element in the collection.</returns>
        protected virtual GameObject GetFirstActiveGameObject(IEnumerable<GameObject> collection)
        {
            foreach (GameObject element in collection)
            {
                if (element.gameObject.activeInHierarchy)
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the first active <see cref="Camera"/> found in the given collection.
        /// </summary>
        /// <param name="collection">The collection to look for the first active in.</param>
        /// <returns>The found first active element in the collection.</returns>
        protected virtual Camera GetFirstActiveCamera(IEnumerable<Camera> collection)
        {
            foreach (Camera element in collection)
            {
                if (element.gameObject.activeInHierarchy)
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the first active <see cref="VelocityTracker"/> found in the given collection.
        /// </summary>
        /// <param name="collection">The collection to look for the first active in.</param>
        /// <returns>The found first active element in the collection.</returns>
        protected virtual VelocityTracker GetFirstActiveVelocityTracker(IEnumerable<VelocityTracker> collection)
        {
            foreach (VelocityTracker element in collection)
            {
                if (element.gameObject.activeInHierarchy)
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// Called before <see cref="CameraRigs"/> has been changed.
        /// </summary>
        [CalledBeforeChangeOf(nameof(CameraRigs))]
        protected virtual void OnBeforeCameraRigsChange()
        {
            UnsubscribeFromCameraRigsEvents();
        }

        /// <summary>
        /// Called after <see cref="CameraRigs"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(CameraRigs))]
        protected virtual void OnAfterCameraRigsChange()
        {
            SubscribeToCameraRigsEvents();
            RefreshCameraRigsConfiguration();
        }
    }
}