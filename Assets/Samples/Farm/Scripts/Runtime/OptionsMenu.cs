namespace VRTK.Examples
{
    using UnityEngine;
    using Zinnia.Action;
    using Zinnia.Data.Attribute;
    using Zinnia.Extension;

    /// <summary>
    /// Displays a control station with options to change functionality at runtime.
    /// </summary>
    public class OptionsMenu : MonoBehaviour
    {
        #region Options Menu Settings
        [Header("Options Menu Settings")]
        [Tooltip("The action that will toggle the visibility of the options control station.")]
        [SerializeField]
        private BooleanAction activationAction;
        /// <summary>
        /// The action that will toggle the visibility of the options control station.
        /// </summary>
        public BooleanAction ActivationAction
        {
            get
            {
                return activationAction;
            }
            set
            {
                activationAction = value;
                if (this.IsMemberChangeAllowed())
                {
                    OnAfterActivationActionChange();
                }
            }
        }
        [Tooltip("The target location to set the control station to when it appears.")]
        [SerializeField]
        private GameObject locationTarget;
        /// <summary>
        /// The target location to set the control station to when it appears.
        /// </summary>
        public GameObject LocationTarget
        {
            get
            {
                return locationTarget;
            }
            set
            {
                locationTarget = value;
            }
        }
        [Tooltip("The direction the control station will be placed in relation to the target location when it appears.")]
        [SerializeField]
        private GameObject locationDirection;
        /// <summary>
        /// The direction the control station will be placed in relation to the target location when it appears.
        /// </summary>
        public GameObject LocationDirection
        {
            get
            {
                return locationDirection;
            }
            set
            {
                locationDirection = value;
            }
        }
        [Tooltip("The offset distance from the target location to place the control station when it appears.")]
        [SerializeField]
        private float locationOffset = 0.5f;
        /// <summary>
        /// The offset distance from the target location to place the control station when it appears.
        /// </summary>
        public float LocationOffset
        {
            get
            {
                return locationOffset;
            }
            set
            {
                locationOffset = value;
            }
        }
        #endregion

        #region Reference Settings
        [Header("Reference Settings")]
        [Tooltip("The control station.")]
        [SerializeField]
        [Restricted]
        private GameObject controlStation;
        /// <summary>
        /// The control station.
        /// </summary>
        public GameObject ControlStation
        {
            get
            {
                return controlStation;
            }
            protected set
            {
                controlStation = value;
            }
        }
        [Tooltip("The internal linked action for controlling visibility activation.")]
        [SerializeField]
        [Restricted]
        private BooleanAction linkedInternalAction;
        /// <summary>
        /// The internal linked action for controlling visibility activation.
        /// </summary>
        public BooleanAction LinkedInternalAction
        {
            get
            {
                return linkedInternalAction;
            }
            protected set
            {
                linkedInternalAction = value;
            }
        }
        #endregion

        /// <summary>
        /// Clears the <see cref="ActivationAction"/>.
        /// </summary>
        public virtual void ClearActivationAction()
        {
            if (!this.IsValidState())
            {
                return;
            }

            ActivationAction = default;
        }

        /// <summary>
        /// Clears the <see cref="LocationTarget"/>.
        /// </summary>
        public virtual void ClearLocationTarget()
        {
            if (!this.IsValidState())
            {
                return;
            }

            LocationTarget = default;
        }

        /// <summary>
        /// Clears the <see cref="LocationDirection"/>.
        /// </summary>
        public virtual void ClearLocationDirection()
        {
            if (!this.IsValidState())
            {
                return;
            }

            LocationDirection = default;
        }

        /// <summary>
        /// Sets the location of the options menu.
        /// </summary>
        public virtual void SetLocation()
        {
            if (LocationTarget == null || LocationDirection == null)
            {
                return;
            }

            Vector3 locationDirectionPosition = LocationDirection.transform.position;
            Vector3 locationTargetPosition = LocationTarget.transform.position;

            transform.position = new Vector3(locationDirectionPosition.x, locationTargetPosition.y, locationDirectionPosition.z);
            ControlStation.transform.localPosition = LocationDirection.transform.forward * LocationOffset;

            Vector3.Scale(ControlStation.transform.localPosition, Vector3.right + Vector3.forward);
            Vector3 targetPosition = locationDirectionPosition;
            targetPosition.y = locationTargetPosition.y;
            ControlStation.transform.LookAt(targetPosition);
            ControlStation.transform.localEulerAngles = Vector3.up * (ControlStation.transform.localEulerAngles.y + 180f);
        }

        protected virtual void OnEnable()
        {
            ConfigureToggleVisibility();
        }

        /// <summary>
        /// Links the provided <see cref="ActivationAction"/> action to the internal <see cref="LinkedInternalAction"/>.
        /// </summary>
        protected virtual void ConfigureToggleVisibility()
        {
            LinkedInternalAction.RunWhenActiveAndEnabled(() => LinkedInternalAction.ClearSources());
            LinkedInternalAction.RunWhenActiveAndEnabled(() => LinkedInternalAction.AddSource(ActivationAction));
        }

        /// <summary>
        /// Called after <see cref="ActivationAction"/> has been changed.
        /// </summary>
        protected virtual void OnAfterActivationActionChange()
        {
            ConfigureToggleVisibility();
        }
    }
}