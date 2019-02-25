namespace VRTK.Examples.Prefabs.OptionsMenu
{
    using UnityEngine;
    using Zinnia.Action;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// Displays a control station with options to change functionality at runtime.
    /// </summary>
    public class OptionsMenuFacade : MonoBehaviour
    {
        #region Options Menu Settings
        [Header("Options Menu Settings"), Tooltip("The action that will toggle the visibility of the options control station."), SerializeField]
        private BooleanAction _activationAction;
        /// <summary>
        /// The action that will toggle the visibility of the options control station.
        /// </summary>
        public BooleanAction ActivationAction
        {
            get { return _activationAction; }
            set
            {
                _activationAction = value;
                ConfigureToggleVisibility();
            }
        }

        /// <summary>
        /// The target location to set the control station to when it appears.
        /// </summary>
        [Tooltip("The target location to set the control station to when it appears.")]
        public GameObject locationTarget;
        /// <summary>
        /// The direction the control station will be placed in relation to the target location when it appears.
        /// </summary>
        [Tooltip("The direction the control station will be placed in relation to the target location when it appears.")]
        public GameObject locationDirection;
        /// <summary>
        /// The offset distance from the target location to place the control station when it appears.
        /// </summary>
        [Tooltip("The offset distance from the target location to place the control station when it appears.")]
        public float locationOffset = 0.5f;
        #endregion

        #region Internal Settings
        /// <summary>
        /// The control station.
        /// </summary>
        [Header("Internal Settings"), Tooltip("The control station."), InternalSetting, SerializeField]
        private GameObject controlStation = null;
        /// <summary>
        /// The internal linked action for controlling visibility activation.
        /// </summary>
        [Tooltip("The internal linked action for controlling visibility activation."), InternalSetting, SerializeField]
        private BooleanAction linkedInternalAction = null;
        #endregion

        public virtual void SetLocation()
        {
            if (locationTarget == null || locationDirection == null)
            {
                return;
            }

            Vector3 locationDirectionPosition = locationDirection.transform.position;
            Vector3 locationTargetPosition = locationTarget.transform.position;

            transform.position = new Vector3(locationDirectionPosition.x, locationTargetPosition.y, locationDirectionPosition.z);
            controlStation.transform.localPosition = locationDirection.transform.forward * locationOffset;

            Vector3.Scale(controlStation.transform.localPosition, Vector3.right + Vector3.forward);
            Vector3 targetPosition = locationDirectionPosition;
            targetPosition.y = locationTargetPosition.y;
            controlStation.transform.LookAt(targetPosition);
            controlStation.transform.localEulerAngles = Vector3.up * (controlStation.transform.localEulerAngles.y + 180f);
        }

        protected virtual void OnEnable()
        {
            ConfigureToggleVisibility();
        }

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            ConfigureToggleVisibility();
        }

        /// <summary>
        /// Links the provided <see cref="ActivationAction"/> action to the internal <see cref="linkedInternalAction"/>.
        /// </summary>
        protected virtual void ConfigureToggleVisibility()
        {
            linkedInternalAction.ClearSources();
            linkedInternalAction.AddSource(ActivationAction);
        }
    }
}