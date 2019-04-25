namespace VRTK.Prefabs.Locomotion.Movement.AxesToVector3
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.BehaviourStateRequirementMethod;
    using Zinnia.Action;
    using Zinnia.Extension;
    using Zinnia.Process;
    using Zinnia.Process.Moment;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Type.Transformation.Aggregation;

    /// <summary>
    /// Sets up the AxisSlide prefab based on the provided settings and implements the logic to allow moving an object via input from two axes.
    /// </summary>
    public class AxesToVector3Processor : MonoBehaviour, IProcessable
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public AxesToVector3Facade Facade { get; protected set; }
        #endregion

        #region Axis Data Settings
        /// <summary>
        /// The current axis data to use when processing movement.
        /// </summary>
        [Serialized]
        [field: Header("Axis Data Settings"), DocumentedByXml]
        public Vector3 CurrentAxisData { get; set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The lateral <see cref="FloatAction"/> to map to.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public FloatAction LateralAxis { get; protected set; }
        /// <summary>
        /// The longitudinal <see cref="FloatAction"/> to map to.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public FloatAction LongitudinalAxis { get; protected set; }
        /// <summary>
        /// The multiplier that operates on the final <see cref="Vector3"/> to modify the speed.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public Vector3Multiplier SpeedMultiplier { get; protected set; }
        /// <summary>
        /// The multiplier to use as the input mask to limit the forward direction.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public Vector3Multiplier InputMask { get; protected set; }
        /// <summary>
        /// The GameObject that contains the incremental axis logic.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObject IncrementalAxis { get; protected set; }
        /// <summary>
        /// The GameObject that contains the directional axis logic.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObject DirectionalAxis { get; protected set; }
        /// <summary>
        /// The MomentProcessor for processing the axis logic.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public MomentProcessor MomentProcessor { get; protected set; }
        #endregion

        /// <summary>
        /// The current calculated movement.
        /// </summary>
        public Vector3 CurrentMovement { get; protected set; }

        /// <summary>
        /// Emits the Converted event for the last calculated <see cref="Vector3"/>.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void Process()
        {
            Vector3 axisDirection = Facade.SourceOfForwardDirection != null ? ApplyForwardSourceToAxis(CurrentAxisData) : CurrentAxisData;
            float multiplier = Facade.AxisUsageType == AxesToVector3Facade.AxisUsage.Incremental ? (Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime) : 1f;
            CurrentMovement = InputMask.Transform(axisDirection) * multiplier;
            Facade.Processed?.Invoke(CurrentMovement);
        }

        /// <summary>
        /// Configures the speed multipliers.
        /// </summary>
        public virtual void ConfigureMultipliers()
        {
            SpeedMultiplier.RunWhenActiveAndEnabled(() => SpeedMultiplier.SetComponentX(Facade.LateralSpeedMultiplier, 1));
            SpeedMultiplier.RunWhenActiveAndEnabled(() => SpeedMultiplier.SetComponentZ(Facade.LongitudinalSpeedMultiplier, 1));
            SpeedMultiplier.RunWhenActiveAndEnabled(() => SpeedMultiplier.Collection.CurrentIndex = 0);
        }

        /// <summary>
        /// Configures the axis sources.
        /// </summary>
        /// <param name="clearOnly">Whether to only clear the existing sources and not add new ones.</param>
        public virtual void ConfigureAxisSources(bool clearOnly = false)
        {
            if (LateralAxis != null)
            {
                LateralAxis.RunWhenActiveAndEnabled(() => LateralAxis.ClearSources());
                if (!clearOnly && Facade.LateralAxis != null)
                {
                    LateralAxis.RunWhenActiveAndEnabled(() => LateralAxis.AddSource(Facade.LateralAxis));
                }
            }

            if (LongitudinalAxis != null)
            {
                LongitudinalAxis.RunWhenActiveAndEnabled(() => LongitudinalAxis.ClearSources());
                if (!clearOnly && Facade.LongitudinalAxis != null)
                {
                    LongitudinalAxis.RunWhenActiveAndEnabled(() => LongitudinalAxis.AddSource(Facade.LongitudinalAxis));
                }
            }
        }

        /// <summary>
        /// Configures the Axis Usage Type.
        /// </summary>
        public virtual void ConfigureAxisUsageType()
        {
            switch (Facade.AxisUsageType)
            {
                case AxesToVector3Facade.AxisUsage.Directional:
                    IncrementalAxis.SetActive(false);
                    MomentProcessor.gameObject.SetActive(false);
                    DirectionalAxis.SetActive(true);
                    break;
                case AxesToVector3Facade.AxisUsage.Incremental:
                    DirectionalAxis.SetActive(false);
                    IncrementalAxis.SetActive(true);
                    MomentProcessor.gameObject.SetActive(true);
                    break;
            }
        }

        protected virtual void OnEnable()
        {
            ConfigureAxisSources();
            ConfigureMultipliers();
            ConfigureAxisUsageType();
        }

        protected virtual void OnDisable()
        {
            ConfigureAxisSources(true);
        }

        /// <summary>
        /// Applies the forward following source data to the axis data.
        /// </summary>
        /// <param name="axisData">The axis data to apply.</param>
        /// <returns>The applied result of the forward following data against the axis data.</returns>
        protected virtual Vector3 ApplyForwardSourceToAxis(Vector3 axisData)
        {
            return (Facade.SourceOfForwardDirection.transform.right * axisData.x)
                + (Facade.SourceOfForwardDirection.transform.up * axisData.y)
                + (Facade.SourceOfForwardDirection.transform.forward * axisData.z);
        }
    }
}