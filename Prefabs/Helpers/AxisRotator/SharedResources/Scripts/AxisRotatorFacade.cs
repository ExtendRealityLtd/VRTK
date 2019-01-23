namespace VRTK.Prefabs.Helpers.AxisRotator
{
    using UnityEngine;
    using Zinnia.Action;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface for the AxisRotator prefab.
    /// </summary>
    public class AxisRotatorFacade : MonoBehaviour
    {
        #region Axis Settings
        [Header("Axis Settings"), Tooltip("The FloatAction to get the lateral (left/right direction) data from."), SerializeField]
        private FloatAction _lateralAxis;
        /// <summary>
        /// The <see cref="FloatAction"/> to get the lateral (left/right direction) data from.
        /// </summary>
        public FloatAction LateralAxis
        {
            get
            {
                return _lateralAxis;
            }
            set
            {
                _lateralAxis = value;
                internalSetup.SetAxisSources();
            }
        }

        [Tooltip("The FloatAction to get the longitudinal (forward/backward direction) data from."), SerializeField]
        private FloatAction _longitudinalAxis;
        /// <summary>
        /// The <see cref="FloatAction"/> to get the longitudinal (forward/backward direction) data from.
        /// </summary>
        public FloatAction LongitudinalAxis
        {
            get
            {
                return _longitudinalAxis;
            }
            set
            {
                _longitudinalAxis = value;
                internalSetup.SetAxisSources();
            }
        }
        #endregion

        #region Target Settings
        [Header("Target Settings"), Tooltip("The target to rotate."), SerializeField]
        private GameObject _target;
        /// <summary>
        /// The target to rotate.
        /// </summary>
        public GameObject Target
        {
            get { return _target; }
            set
            {
                _target = value;
                internalSetup.SetMutator();
            }
        }

        [Tooltip("The direction offset to use when considering the rotation origin."), SerializeField]
        private GameObject _directionOffset;
        /// <summary>
        /// The direction offset to use when considering the rotation origin.
        /// </summary>
        public GameObject DirectionOffset
        {
            get { return _directionOffset; }
            set
            {
                _directionOffset = value;
                internalSetup.SetExtractor();
            }
        }
        #endregion

        #region Internal Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Header("Internal Settings"), Tooltip("The linked Internal Setup."), InternalSetting, SerializeField]
        protected AxisRotatorInternalSetup internalSetup;
        #endregion

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            internalSetup.SetAxisSources();
            internalSetup.SetMutator();
            internalSetup.SetExtractor();
        }
    }
}