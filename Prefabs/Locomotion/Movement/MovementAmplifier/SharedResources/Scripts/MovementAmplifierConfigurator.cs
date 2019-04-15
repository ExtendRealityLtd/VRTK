namespace VRTK.Prefabs.Locomotion.Movement.MovementAmplifier
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Extension;
    using Zinnia.Tracking.Follow;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Operation.Mutation;
    using Zinnia.Data.Type.Transformation.Aggregation;

    /// <summary>
    /// Sets up the MovementAmplifier prefab based on the provided user settings.
    /// </summary>
    public class MovementAmplifierConfigurator : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public MovementAmplifierFacade Facade { get; protected set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// Moves the radius origin.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public ObjectDistanceComparator RadiusOriginMover { get; protected set; }
        /// <summary>
        /// Determines whether <see cref="MovementAmplifierFacade.Source"/> is inside the radius.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public ObjectDistanceComparator DistanceChecker { get; protected set; }
        /// <summary>
        /// Moves the objects.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public ObjectDistanceComparator ObjectMover { get; protected set; }
        /// <summary>
        /// Subtracts the radius.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public FloatAdder RadiusSubtractor { get; protected set; }
        /// <summary>
        /// Stabilizes the radius by ensuring <see cref="MovementAmplifierFacade.Target"/> moves back into the radius.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public float RadiusStabilizer { get; protected set; } = 0.001f;
        /// <summary>
        /// Amplifies the movement.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public Vector3Multiplier MovementMultiplier { get; protected set; }
        /// <summary>
        /// Moves the target.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public TransformPositionMutator TargetPositionMutator { get; protected set; }
        #endregion

        /// <summary>
        /// Configures the <see cref="RadiusOriginMover"/> with the facade settings.
        /// </summary>
        public virtual void ConfigureRadiusOriginMover()
        {
            RadiusOriginMover.transform.parent.position = Facade.Source.transform.position;
            RadiusOriginMover.RunWhenActiveAndEnabled(() => RadiusOriginMover.Target = Facade.Source);
        }

        /// <summary>
        /// Configures the <see cref="DistanceChecker"/> with the facade settings.
        /// </summary>
        public virtual void ConfigureDistanceChecker()
        {
            DistanceChecker.RunWhenActiveAndEnabled(() => DistanceChecker.Source = Facade.Source);
            DistanceChecker.DistanceThreshold = Facade.IgnoredRadius;
        }

        /// <summary>
        /// Configures the <see cref="ObjectMover"/> with the facade settings.
        /// </summary>
        public virtual void ConfigureObjectMover()
        {
            ObjectMover.RunWhenActiveAndEnabled(() => ObjectMover.Source = Facade.Source);
        }

        /// <summary>
        /// Configures the <see cref="RadiusSubtractor"/> with the facade settings.
        /// </summary>
        public virtual void ConfigureRadiusSubtractor()
        {
            RadiusSubtractor.RunWhenActiveAndEnabled(() => RadiusSubtractor.Collection.SetAt(-Facade.IgnoredRadius + RadiusStabilizer, 1));
        }

        /// <summary>
        /// Configures the <see cref="MovementMultiplier"/> with the facade settings.
        /// </summary>
        public virtual void ConfigureMovementMultiplier()
        {
            MovementMultiplier.RunWhenActiveAndEnabled(() => MovementMultiplier.Collection.SetAt(Vector3.one * (Facade.Multiplier - 1f), 1));
        }

        /// <summary>
        /// Configures the <see cref="TargetPositionMutator"/> with the facade settings.
        /// </summary>
        public virtual void ConfigureTargetPositionMutator()
        {
            TargetPositionMutator.Target = Facade.Target;
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
            ObjectMover.enabled = false;
        }
    }
}