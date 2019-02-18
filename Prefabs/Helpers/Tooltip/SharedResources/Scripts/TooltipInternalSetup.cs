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
        /// The UI <see cref="Text"/> element to display the text in.
        /// </summary>
        [Header("Reference Settings"), Tooltip("The UI Text element to display the text in."), InternalSetting, SerializeField]
        protected Text displayText;
        /// <summary>
        /// The UI <see cref="Image"/> element to use as the background.
        /// </summary>
        [Tooltip("The UI Image element to use as the background."), InternalSetting, SerializeField]
        protected Image backgroundImage;
        /// <summary>
        /// The <see cref="LineRenderer"/> to draw a line from tooltip to target.
        /// </summary>
        [Tooltip("The LineRenderer to draw a line from tooltip to target."), InternalSetting, SerializeField]
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
            if (facade.FacingTarget != null)
            {
                facade.transform.LookAt(facade.FacingTarget.transform);
            }

            ToggleLineVisibility();
            SetLine(facade.LineTarget, facade.LineWidth);
        }

        /// <summary>
        /// Sets the tooltip text appearance.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="size">The size of the font.</param>
        /// <param name="color">The color of the font.</param>
        public virtual void SetDisplayText(string text, int size, Color color)
        {
            displayText.text = text;
            displayText.fontSize = size * 10;
            displayText.color = color;
        }

        /// <summary>
        /// Sets the tooltip container appearance.
        /// </summary>
        /// <param name="size">The size of the container.</param>
        /// <param name="backgroundColor">The background color of the container.</param>
        public virtual void SetContainer(Vector2 size, Color backgroundColor)
        {
            backgroundImage.GetComponent<RectTransform>().sizeDelta = size;
            backgroundImage.color = backgroundColor;
        }

        /// <summary>
        /// Sets the tooltip pointer line appearance.
        /// </summary>
        /// <param name="target">The target to point the line towards.</param>
        /// <param name="width">The width of the line.</param>
        public virtual void SetLine(GameObject target, float width)
        {
            if (target == null)
            {
                return;
            }

            lineRenderer.SetPosition(0, lineOrigin.transform.position);
            lineRenderer.SetPosition(1, target.transform.position);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }

        protected virtual void OnEnable()
        {
            SetDisplayText(facade.DisplayText, facade.TextSize, facade.TextColor);
            SetContainer(facade.ContainerSize, facade.ContainerColor);
            SetLine(facade.LineTarget, facade.LineWidth);
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