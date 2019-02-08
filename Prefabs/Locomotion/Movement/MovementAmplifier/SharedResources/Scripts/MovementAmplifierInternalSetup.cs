namespace VRTK.Prefabs.Locomotion.Movement.MovementAmplifier
{
    using UnityEngine;
    using Zinnia.Tracking.Follow;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Operation;
    using Zinnia.Data.Type.Transformation;

    /// <summary>
    /// Sets up the MovementAmplifier prefab based on the provided user settings.
    /// </summary>
    public class MovementAmplifierInternalSetup : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Header("Facade Settings"), Tooltip("The public interface facade."), InternalSetting, SerializeField]
        protected MovementAmplifierFacade facade;
        #endregion

        #region Reference Settings
        /// <summary>
        /// Moves the radius origin.
        /// </summary>
        [Header("Reference Settings"), Tooltip("Moves the radius origin."), InternalSetting, SerializeField]
        protected ObjectDistanceComparator radiusOriginMover;
        /// <summary>
        /// Determines whether <see cref="MovementAmplifierFacade.source"/> is inside the radius.
        /// </summary>
        [Tooltip("Determines whether MovementAmplifierFacade.source is inside the radius."), InternalSetting, SerializeField]
        protected ObjectDistanceComparator distanceChecker;
        /// <summary>
        /// Moves the objects.
        /// </summary>
        [Tooltip("Moves the objects."), InternalSetting, SerializeField]
        protected ObjectDistanceComparator objectMover;
        /// <summary>
        /// Subtracts the radius.
        /// </summary>
        [Tooltip("Subtracts the radius."), InternalSetting, SerializeField]
        protected FloatAdder radiusSubtractor;
        /// <summary>
        /// Stabilizes the radius by ensuring <see cref="MovementAmplifierFacade.target"/> moves back into the radius.
        /// </summary>
        [Tooltip("Stabilizes the radius by ensuring MovementAmplifierFacade.target moves back into the radius."), InternalSetting, SerializeField]
        protected float radiusStabilizer = 0.001f;
        /// <summary>
        /// Amplifies the movement.
        /// </summary>
        [Tooltip("Amplifies the movement."), InternalSetting, SerializeField]
        protected Vector3Multiplier movementMultiplier;
        /// <summary>
        /// Moves the target.
        /// </summary>
        [Tooltip("Moves the target."), InternalSetting, SerializeField]
        protected TransformPositionMutator targetPositionMutator;
        #endregion

        /// <summary>
        /// Configures the <see cref="radiusOriginMover"/> with the facade settings.
        /// </summary>
        public virtual void ConfigureRadiusOriginMover()
        {
            radiusOriginMover.transform.parent.position = facade.Source.transform.position;
            radiusOriginMover.Target = facade.Source;
        }

        /// <summary>
        /// Configures the <see cref="distanceChecker"/> with the facade settings.
        /// </summary>
        public virtual void ConfigureDistanceChecker()
        {
            distanceChecker.Source = facade.Source;
            distanceChecker.distanceThreshold = facade.IgnoredRadius;
        }

        /// <summary>
        /// Configures the <see cref="objectMover"/> with the facade settings.
        /// </summary>
        public virtual void ConfigureObjectMover()
        {
            objectMover.Source = facade.Source;
        }

        /// <summary>
        /// Configures the <see cref="radiusSubtractor"/> with the facade settings.
        /// </summary>
        public virtual void ConfigureRadiusSubtractor()
        {
            radiusSubtractor.SetElement(1, -facade.IgnoredRadius + radiusStabilizer);
        }

        /// <summary>
        /// Configures the <see cref="movementMultiplier"/> with the facade settings.
        /// </summary>
        public virtual void ConfigureMovementMultiplier()
        {
            movementMultiplier.SetElement(1, Vector3.one * (facade.Multiplier - 1f));
        }

        /// <summary>
        /// Configures the <see cref="targetPositionMutator"/> with the facade settings.
        /// </summary>
        public virtual void ConfigureTargetPositionMutator()
        {
            targetPositionMutator.Target = facade.Target;
        }

        protected virtual void OnEnable()
        {
            ConfigureRadiusOriginMover();
            ConfigureDistanceChecker();
            ConfigureObjectMover();
            ConfigureRadiusSubtractor();
            ConfigureMovementMultiplier();
            ConfigureTargetPositionMutator();
        }

        protected virtual void OnDisable()
        {
            objectMover.enabled = false;
        }
    }
}