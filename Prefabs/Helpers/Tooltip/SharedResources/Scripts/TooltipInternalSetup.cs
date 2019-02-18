namespace VRTK.Prefabs.Helpers.Tooltip
{
    using UnityEngine;
    using UnityEngine.UI;
    using Zinnia.Process;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// Sets up the Tooltip prefab based on the provided settings and implements the logic to display the tooltip.
    /// </summary>
    public class TooltipInternalSetup : MonoBehaviour, IProcessable
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Header("Facade Settings"), Tooltip("The public interface facade."), InternalSetting, SerializeField]
        protected TooltipFacade facade;
        #endregion

        #region Reference Settings
        /// <summary>
        /// The <see cref="LineRenderer"/> to draw a line from tooltip to target.
        /// </summary>
        [Header("Reference Settings"), Tooltip("The LineRenderer to draw a line from tooltip to target."), InternalSetting, SerializeField]
        protected LineRenderer lineRenderer;
        /// <summary>
        /// The <see cref="GameObject"/> use as the origin point for the line.
        /// </summary>
        [Tooltip("The GameObject use as the origin point for the line."), InternalSetting, SerializeField]
        protected GameObject lineOrigin;
        #endregion

        /// <summary>
        /// Processes the visualization of the tooltip.
        /// </summary>
        public virtual void Process()
        {
            if (facade.FacingSource != null)
            {
                facade.transform.LookAt(facade.FacingSource.transform);
            }

            SetLine(facade.LineTarget);
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

            lineRenderer.SetPosition(0, lineOrigin.transform.position);
            lineRenderer.SetPosition(1, target.transform.position);
        }

        protected virtual void OnEnable()
        {
            SetLine(facade.LineTarget);
            ToggleLineVisibility();
        }

        /// <summary>
        /// Toggles the visibility of the tooltip line depending on whether there is a valid target.
        /// </summary>
        protected virtual void ToggleLineVisibility()
        {
            lineRenderer.gameObject.SetActive(facade.LineTarget != null);
        }
    }
}