namespace VRTK.Examples.Prefabs.OptionsMenu
{
    using UnityEngine;
    using Malimbe.MemberChangeMethod;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Action;
    using Zinnia.Extension;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// Displays a control station with options to change functionality at runtime.
    /// </summary>
    public class OptionsMenuFacade : MonoBehaviour
    {
        #region Options Menu Settings
        /// <summary>
        /// The action that will toggle the visibility of the options control station.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Options Menu Settings"), DocumentedByXml]
        public BooleanAction ActivationAction { get; set; }
        /// <summary>
        /// The target location to set the control station to when it appears.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject LocationTarget { get; set; }
        /// <summary>
        /// The direction the control station will be placed in relation to the target location when it appears.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject LocationDirection { get; set; }
        /// <summary>
        /// The offset distance from the target location to place the control station when it appears.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float LocationOffset { get; set; } = 0.5f;
        #endregion

        #region Reference Settings
        /// <summary>
        /// The control station.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public GameObject ControlStation { get; protected set; }
        /// <summary>
        /// The internal linked action for controlling visibility activation.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public BooleanAction LinkedInternalAction { get; protected set; }
        #endregion

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
        [CalledAfterChangeOf(nameof(ActivationAction))]
        protected virtual void OnAfterActivationActionChange()
        {
            ConfigureToggleVisibility();
        }
    }
}