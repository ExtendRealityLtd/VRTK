namespace VRTK.Prefabs.Interactions.Interactables.Grab.Action
{
    using UnityEngine;
    using Zinnia.Extension;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Velocity;
    using Zinnia.Tracking.Follow;
    using Zinnia.Tracking.Follow.Modifier;

    /// <summary>
    /// Describes an action that allows the Interactable to follow an Interactor's position, rotation and scale.
    /// </summary>
    public class GrabInteractableFollowAction : GrabInteractableAction
    {
        /// <summary>
        /// The way in which the object is moved.
        /// </summary>
        public enum TrackingType
        {
            /// <summary>
            /// Updates the transform data directly, outside of the physics system.
            /// </summary>
            FollowTransform,
            /// <summary>
            /// Updates the rigidbody using velocity to stay within the bounds of the physics system.
            /// </summary>
            FollowRigidbody,
            /// <summary>
            /// Updates the rigidbody rotation by using a force at position.
            /// </summary>
            FollowRigidbodyForceRotate
        }

        /// <summary>
        /// The offset to apply on grab.
        /// </summary>
        public enum OffsetType
        {
            /// <summary>
            /// No offset is applied.
            /// </summary>
            None,
            /// <summary>
            /// An offset of a specified <see cref="GameObject"/> is applied to orientate the interactable on grab.
            /// </summary>
            OrientationHandle,
            /// <summary>
            /// An offset of where the collision between the Interactor and Interactable is applied for precision grabbing.
            /// </summary>
            PrecisionPoint,
            /// <summary>
            /// The precision point offset will always be re-created based on the latest Interactor grabbing the Interactable.
            /// </summary>
            ForcePrecisionPoint
        }

        #region Interactable Settings
        [Header("Interactable Settings"), Tooltip("Determines how to track the interactable to the interactor."), SerializeField]
        private TrackingType _followTracking = TrackingType.FollowTransform;
        /// <summary>
        /// Determines how to track the movement of interactable to the interactor.
        /// </summary>
        public TrackingType FollowTracking
        {
            get { return _followTracking; }
            set
            {
                _followTracking = value;
                ConfigureFollowTracking();
            }
        }

        [Tooltip("The offset to apply when grabbing the Interactable."), SerializeField]
        private OffsetType _grabOffset = OffsetType.None;
        /// <summary>
        /// The offset to apply when grabbing the Interactable.
        /// </summary>
        public OffsetType GrabOffset
        {
            get { return _grabOffset; }
            set
            {
                _grabOffset = value;
                ConfigureGrabOffset();
            }
        }
        #endregion

        #region Tracking Settings
        /// <summary>
        /// The <see cref="ObjectFollower"/> for tracking movement.
        /// </summary>
        [Header("Tracking Settings"), Tooltip("The ObjectFollower for tracking movement."), InternalSetting, SerializeField]
        protected ObjectFollower objectFollower;
        /// <summary>
        /// The <see cref="FollowModifier"/> to move by following the transform.
        /// </summary>
        [Tooltip("The FollowModifier to move by following the transform."), InternalSetting, SerializeField]
        protected FollowModifier followTransform;
        /// <summary>
        /// The <see cref="FollowModifier"/> to move by applying velocities to the rigidbody.
        /// </summary>
        [Tooltip("The FollowModifier to move by applying velocities to the rigidbody."), InternalSetting, SerializeField]
        protected FollowModifier followRigidbody;
        /// <summary>
        /// The <see cref="FollowModifier"/> to rotate by applying a force to the rigidbody.
        /// </summary>
        [Tooltip("The FollowModifier to rotate by applying a force to the rigidbody."), InternalSetting, SerializeField]
        protected FollowModifier followRigidbodyForceRotate;
        /// <summary>
        /// The <see cref="VelocityApplier"/> to apply velocity on ungrab.
        /// </summary>
        [Tooltip("The VelocityApplier to apply velocity on ungrab"), InternalSetting, SerializeField]
        protected VelocityApplier velocityApplier;
        #endregion

        #region Grab Offset Settings
        /// <summary>
        /// The container for the precision point logic.
        /// </summary>
        [Header("Grab Offset Settings"), Tooltip("The container for the precision point logic."), InternalSetting, SerializeField]
        protected GameObject precisionLogicContainer;
        /// <summary>
        /// The container for the precision point creation logic.
        /// </summary>
        [Tooltip("The container for the precision point creation logic."), InternalSetting, SerializeField]
        protected GameObject precisionCreateContainer;
        /// <summary>
        /// The container for the precision point force creation logic.
        /// </summary>
        [Tooltip("The container for the precision point force creation logic."), InternalSetting, SerializeField]
        protected GameObject precisionForceCreateContainer;
        /// <summary>
        /// The container for the orientation handle  logic.
        /// </summary>
        [Tooltip("The container for the orientation handle logic."), InternalSetting, SerializeField]
        protected GameObject orientationLogicContainer;
        #endregion

        /// <inheritdoc />
        public override void SetUp(GrabInteractableInternalSetup grabSetup)
        {
            base.SetUp(grabSetup);
            objectFollower.ClearTargets();
            objectFollower.AddTarget(grabSetup.Facade.ConsumerContainer);
            velocityApplier.target = grabSetup.Facade.ConsumerRigidbody;
        }

        /// <summary>
        /// Turns on the kinematic state on the Interactable <see cref="Rigidbody"/>.
        /// </summary>
        public virtual void EnableKinematics()
        {
            grabSetup.Facade.ConsumerRigidbody.isKinematic = true;
        }

        /// <summary>
        /// Turns off the kinematic state on the Interactable <see cref="Rigidbody"/>.
        /// </summary>
        public virtual void DisableKinematics()
        {
            grabSetup.Facade.ConsumerRigidbody.isKinematic = false;
        }

        protected virtual void OnEnable()
        {
            ConfigureFollowTracking();
            ConfigureGrabOffset();
        }

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            ConfigureFollowTracking();
            ConfigureGrabOffset();
        }

        /// <summary>
        /// Configures the appropriate processes to be used for follow tracking based on the <see cref="FollowTracking"/> setting.
        /// </summary>
        protected virtual void ConfigureFollowTracking()
        {
            switch (FollowTracking)
            {
                case TrackingType.FollowTransform:
                    followTransform.gameObject.SetActive(true);
                    followRigidbody.gameObject.SetActive(false);
                    followRigidbodyForceRotate.gameObject.SetActive(false);
                    objectFollower.followModifier = followTransform;
                    break;
                case TrackingType.FollowRigidbody:
                    followTransform.gameObject.SetActive(false);
                    followRigidbody.gameObject.SetActive(true);
                    followRigidbodyForceRotate.gameObject.SetActive(false);
                    objectFollower.followModifier = followRigidbody;
                    break;
                case TrackingType.FollowRigidbodyForceRotate:
                    followTransform.gameObject.SetActive(false);
                    followRigidbody.gameObject.SetActive(false);
                    followRigidbodyForceRotate.gameObject.SetActive(true);
                    objectFollower.followModifier = followRigidbodyForceRotate;
                    break;
            }
        }

        /// <summary>
        /// Configures the appropriate processes to be used for grab offset based on the <see cref="GrabOffset"/> setting.
        /// </summary>
        protected virtual void ConfigureGrabOffset()
        {
            switch (GrabOffset)
            {
                case OffsetType.None:
                    precisionLogicContainer.TrySetActive(false);
                    orientationLogicContainer.TrySetActive(false);
                    break;
                case OffsetType.PrecisionPoint:
                    precisionLogicContainer.TrySetActive(true);
                    precisionCreateContainer.TrySetActive(true);
                    precisionForceCreateContainer.TrySetActive(false);
                    orientationLogicContainer.TrySetActive(false);
                    break;
                case OffsetType.ForcePrecisionPoint:
                    precisionLogicContainer.TrySetActive(true);
                    precisionForceCreateContainer.TrySetActive(true);
                    precisionCreateContainer.TrySetActive(false);
                    orientationLogicContainer.TrySetActive(false);
                    break;
                case OffsetType.OrientationHandle:
                    precisionLogicContainer.TrySetActive(false);
                    orientationLogicContainer.TrySetActive(true);
                    break;
            }
        }
    }
}