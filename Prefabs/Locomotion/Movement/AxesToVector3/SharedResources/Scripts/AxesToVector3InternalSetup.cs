namespace VRTK.Prefabs.Locomotion.Movement.AxesToVector3
{
    using UnityEngine;
    using Zinnia.Action;
    using Zinnia.Process;
    using Zinnia.Process.Moment;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Type.Transformation;

    /// <summary>
    /// Sets up the AxisSlide prefab based on the provided settings and implements the logic to allow moving an object via input from two axes.
    /// </summary>
    public class AxesToVector3InternalSetup : MonoBehaviour, IProcessable
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Header("Facade Settings"), Tooltip("The public interface facade."), InternalSetting, SerializeField]
        protected AxesToVector3Facade facade;
        #endregion

        #region Reference Settings
        /// <summary>
        /// The lateral <see cref="FloatAction"/> to map to.
        /// </summary>
        [Header("Reference Settings"), Tooltip("The lateral FloatAction to map to."), InternalSetting, SerializeField]
        protected FloatAction lateralAxis;
        /// <summary>
        /// The longitudinal <see cref="FloatAction"/> to map to.
        /// </summary>
        [Tooltip("The longitudinal FloatAction to map to."), InternalSetting, SerializeField]
        protected FloatAction longitudinalAxis;
        /// <summary>
        /// The multiplier that operates on the final <see cref="Vector3"/> to modify the speed.
        /// </summary>
        [Tooltip("The multiplier that operates on the final Vector3 to modify the speed."), InternalSetting, SerializeField]
        protected Vector3Multiplier speedMultiplier;
        /// <summary>
        /// The multiplier to use as the input mask to limit the forward direction.
        /// </summary>
        [Tooltip("The multiplier to use as the input mask to limit the forward direction."), InternalSetting, SerializeField]
        protected Vector3Multiplier inputMask;
        /// <summary>
        /// The GameObject that contains the incremental axis logic.
        /// </summary>
        [Tooltip("The GameObject that contains the incremental axis logic."), InternalSetting, SerializeField]
        protected GameObject incrementalAxis;
        /// <summary>
        /// The GameObject that contains the directional axis logic.
        /// </summary>
        [Tooltip("The GameObject that contains the directional axis logic."), InternalSetting, SerializeField]
        protected GameObject directionalAxis;
        /// <summary>
        /// The MomentProcessor for processing the axis logic.
        /// </summary>
        [Tooltip("The MomentProcessor for processing the axis logic."), InternalSetting, SerializeField]
        protected MomentProcessor momentProcessor;
        #endregion

        /// <summary>
        /// The current calculated movement.
        /// </summary>
        public Vector3 CurrentMovement
        {
            get;
            protected set;
        }

        /// <summary>
        /// The current axis data to use when processing movement.
        /// </summary>
        protected Vector3 currentAxisData = Vector3.zero;

        /// <summary>
        /// Emits the Converted event for the last calculated <see cref="Vector3"/>.
        /// </summary>
        public virtual void Process()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            Vector3 axisDirection = facade.sourceOfForwardDirection != null ? ApplyForwardSourceToAxis(currentAxisData) : currentAxisData;
            float multiplier = (facade.AxisUsageType == AxesToVector3Facade.AxisUsage.Incremental ? (Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime) : 1f);
            CurrentMovement = inputMask.Transform(axisDirection) * multiplier;
            facade.Processed?.Invoke(CurrentMovement);
        }

        /// <summary>
        /// Sets the speed multipliers.
        /// </summary>
        public virtual void SetMultipliers()
        {
            speedMultiplier.SetElementX(1, facade.LateralSpeedMultiplier);
            speedMultiplier.SetElementZ(1, facade.LongitudinalSpeedMultiplier);
            speedMultiplier.CurrentIndex = 0;
        }

        /// <summary>
        /// Sets the axis sources.
        /// </summary>
        /// <param name="clearOnly">Whether to only clear the existing sources and not add new ones.</param>
        public virtual void SetAxisSources(bool clearOnly = false)
        {
            if (lateralAxis != null)
            {
                lateralAxis.ClearSources();
                if (!clearOnly && facade.LateralAxis != null)
                {
                    lateralAxis.AddSource(facade.LateralAxis);
                }
            }

            if (longitudinalAxis != null)
            {
                longitudinalAxis.ClearSources();
                if (!clearOnly && facade.LongitudinalAxis != null)
                {
                    longitudinalAxis.AddSource(facade.LongitudinalAxis);
                }
            }
        }

        /// <summary>
        /// Set the Axis Usage Type.
        /// </summary>
        public virtual void SetAxisUsageType()
        {
            switch (facade.AxisUsageType)
            {
                case AxesToVector3Facade.AxisUsage.Directional:
                    incrementalAxis.SetActive(false);
                    momentProcessor.gameObject.SetActive(false);
                    directionalAxis.SetActive(true);
                    break;
                case AxesToVector3Facade.AxisUsage.Incremental:
                    directionalAxis.SetActive(false);
                    incrementalAxis.SetActive(true);
                    momentProcessor.gameObject.SetActive(true);
                    break;
            }
        }

        /// <summary>
        /// Processes the axis data into a movement.
        /// </summary>
        /// <param name="axisData">The axis data to process.</param>
        public virtual void SetAxisData(Vector3 axisData)
        {
            currentAxisData = axisData;
        }

        protected virtual void OnEnable()
        {
            SetAxisSources();
            SetMultipliers();
            SetAxisUsageType();
        }

        protected virtual void OnDisable()
        {
            SetAxisSources(true);
        }

        /// <summary>
        /// Applies the forward following source data to the axis data.
        /// </summary>
        /// <param name="axisData">The axis data to apply.</param>
        /// <returns>The applied result of the forward following data against the axis data.</returns>
        protected virtual Vector3 ApplyForwardSourceToAxis(Vector3 axisData)
        {
            return (facade.sourceOfForwardDirection.transform.right * axisData.x)
                + (facade.sourceOfForwardDirection.transform.up * axisData.y)
                + (facade.sourceOfForwardDirection.transform.forward * axisData.z);
        }
    }
}