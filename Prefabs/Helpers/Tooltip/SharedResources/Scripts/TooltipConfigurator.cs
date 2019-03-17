namespace VRTK.Prefabs.Helpers.Tooltip
{
    using UnityEngine;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Zinnia.Process;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// Sets up the Tooltip prefab based on the provided settings and implements the logic to display the tooltip.
    /// </summary>
    public class TooltipConfigurator : MonoBehaviour, IProcessable
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public TooltipFacade Facade { get; protected set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The <see cref="UnityEngine.LineRenderer"/> to draw a line from tooltip to target.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public LineRenderer LineRenderer { get; protected set; }
        /// <summary>
        /// The <see cref="GameObject"/> use as the origin point for the line.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObject LineOrigin { get; protected set; }
        #endregion

        /// <summary>
        /// Processes the visualization of the tooltip.
        /// </summary>
        public virtual void Process()
        {
            if (Facade.FacingSource != null)
            {
                Facade.transform.LookAt(Facade.FacingSource.transform);
            }

            SetLine(Facade.LineTarget);
            ToggleLineVisibility();
        }

        /// <summary>
        /// Sets the tooltip pointer line appearance.
        /// </summary>
        /// <param name="target">The target to point the line towards.</param>
        public virtual void SetLine(GameObject target)
        {
            if (target == null)
            {
                return;
            }

            LineRenderer.SetPosition(0, LineOrigin.transform.position);
            LineRenderer.SetPosition(1, target.transform.position);
        }

        protected virtual void OnEnable()
        {
            SetLine(Facade.LineTarget);
            ToggleLineVisibility();
        }

        /// <summary>
        /// Toggles the visibility of the tooltip line depending on whether there is a valid target.
        /// </summary>
        protected virtual void ToggleLineVisibility()
        {
            LineRenderer.gameObject.SetActive(Facade.LineTarget != null);
        }
    }
}