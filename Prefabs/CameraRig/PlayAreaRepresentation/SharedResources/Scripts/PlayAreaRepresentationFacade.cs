namespace VRTK.Prefabs.PlayAreaRepresentation
{
    using UnityEngine;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface into the PlayAreaRepresentation Prefab.
    /// </summary>
    public class PlayAreaRepresentationFacade : MonoBehaviour
    {
        #region Target Settings
        [Header("Target Settings"), Tooltip("The target to represent the PlayArea."), SerializeField]
        private GameObject _target;
        /// <summary>
        /// The target to represent the PlayArea.
        /// </summary>
        public GameObject Target
        {
            get { return _target; }
            set
            {
                _target = value;
                internalSetup.ConfigureTarget();
            }
        }

        [Tooltip("An optional origin to use in a position offset calculation."), SerializeField]
        private GameObject _offsetOrigin;
        /// <summary>
        /// An optional origin to use in a position offset calculation.
        /// </summary>
        public GameObject OffsetOrigin
        {
            get { return _offsetOrigin; }
            set
            {
                _offsetOrigin = value;
                internalSetup.ConfigureOffsetOrigin();
            }
        }

        [Tooltip("An optional destination to use in a position offset calculation."), SerializeField]
        private GameObject _offsetDestination;
        /// <summary>
        /// An optional destination to use in a position offset calculation.
        /// </summary>
        public GameObject OffsetDestination
        {
            get { return _offsetDestination; }
            set
            {
                _offsetDestination = value;
                internalSetup.ConfigureOffsetDestination();
            }
        }
        #endregion

        #region Internal Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Header("Internal Settings"), Tooltip("The linked Internal Setup."), InternalSetting, SerializeField]
        protected PlayAreaRepresentationInternalSetup internalSetup;
        #endregion

        /// <summary>
        /// Recalculates the PlayArea dimensions.
        /// </summary>
        public virtual void Recalculate()
        {
            internalSetup.RecalculateDimensions();
        }

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            internalSetup.ConfigureTarget();
            internalSetup.ConfigureOffsetOrigin();
            internalSetup.ConfigureOffsetDestination();
        }
    }
}