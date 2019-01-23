namespace VRTK.Prefabs.Locomotion.Movement.MovementAmplifier
{
    using UnityEngine;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface for the MovementAmplifier prefab.
    /// </summary>
    public class MovementAmplifierFacade : MonoBehaviour
    {
        #region Tracking Settings
        [Header("Tracking Settings"), Tooltip("The source to observe movement of."), SerializeField]
        private GameObject _source;
        /// <summary>
        /// The source to observe movement of.
        /// </summary>
        public GameObject Source
        {
            get { return _source; }
            set
            {
                _source = value;
                internalSetup.ConfigureRadiusOriginMover();
                internalSetup.ConfigureDistanceChecker();
                internalSetup.ConfigureObjectMover();
            }
        }

        [Tooltip("The target to apply amplified movement to."), SerializeField]
        private GameObject _target;
        /// <summary>
        /// The target to apply amplified movement to.
        /// </summary>
        public GameObject Target
        {
            get { return _target; }
            set
            {
                _target = value;
                internalSetup.ConfigureTargetPositionMutator();
            }
        }
        #endregion

        #region Movement Settings
        [Header("Movement Settings"), Tooltip("The radius in which source movement is ignored. Too small values can result in movement amplification happening during crouching which is often unexpected."), SerializeField]
        private float _ignoredRadius = 0.25f;
        /// <summary>
        /// The radius in which <see cref="source"/> movement is ignored. Too small values can result in movement amplification happening during crouching which is often unexpected.
        /// </summary>
        public float IgnoredRadius
        {
            get { return _ignoredRadius; }
            set
            {
                _ignoredRadius = value;
                internalSetup.ConfigureDistanceChecker();
                internalSetup.ConfigureRadiusSubtractor();
            }
        }

        [Tooltip("How much to amplify movement of source to apply to target."), SerializeField]
        private float _multiplier = 2f;
        /// <summary>
        /// How much to amplify movement of <see cref="source"/> to apply to <see cref="target"/>.
        /// </summary>
        public float Multiplier
        {
            get { return _multiplier; }
            set
            {
                _multiplier = value;
                internalSetup.ConfigureMovementMultiplier();
            }
        }
        #endregion

        #region Internal Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Header("Internal Settings"), Tooltip("The linked Internal Setup."), InternalSetting, SerializeField]
        protected MovementAmplifierInternalSetup internalSetup;
        #endregion

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            internalSetup.ConfigureRadiusOriginMover();
            internalSetup.ConfigureDistanceChecker();
            internalSetup.ConfigureObjectMover();
            internalSetup.ConfigureRadiusSubtractor();
            internalSetup.ConfigureMovementMultiplier();
            internalSetup.ConfigureTargetPositionMutator();
        }
    }
}