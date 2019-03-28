namespace VRTK.Prefabs.Locomotion.Movement.MovementAmplifier
{
    using UnityEngine;
    using Malimbe.MemberChangeMethod;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface for the MovementAmplifier prefab.
    /// </summary>
    public class MovementAmplifierFacade : MonoBehaviour
    {
        #region Tracking Settings
        /// <summary>
        /// The source to observe movement of.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Tracking Settings"), DocumentedByXml]
        public GameObject Source { get; set; }
        /// <summary>
        /// The target to apply amplified movement to.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject Target { get; set; }
        #endregion

        #region Movement Settings
        /// <summary>
        /// The radius in which <see cref="Source"/> movement is ignored. Too small values can result in movement amplification happening during crouching which is often unexpected.
        /// </summary>
        [Serialized]
        [field: Header("Movement Settings"), DocumentedByXml]
        public float IgnoredRadius { get; set; } = 0.25f;
        /// <summary>
        /// How much to amplify movement of <see cref="Source"/> to apply to <see cref="Target"/>.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float Multiplier { get; set; } = 2f;
        #endregion

        #region Reference Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public MovementAmplifierConfigurator Configuration { get; protected set; }
        #endregion

        /// <summary>
        /// Called after <see cref="Source"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Source))]
        protected virtual void OnAfterSourceChange()
        {
            Configuration.ConfigureRadiusOriginMover();
            Configuration.ConfigureDistanceChecker();
            Configuration.ConfigureObjectMover();
        }

        /// <summary>
        /// Called after <see cref="Target"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Target))]
        protected virtual void OnAfterTargetChange()
        {
            Configuration.ConfigureTargetPositionMutator();
        }

        /// <summary>
        /// Called after <see cref="IgnoredRadius"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(IgnoredRadius))]
        protected virtual void OnAfterIgnoredRadiusChange()
        {
            Configuration.ConfigureDistanceChecker();
            Configuration.ConfigureRadiusSubtractor();
        }

        /// <summary>
        /// Called after <see cref="Multiplier"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Multiplier))]
        protected virtual void OnAfterMultiplierChange()
        {
            Configuration.ConfigureMovementMultiplier();
        }
    }
}