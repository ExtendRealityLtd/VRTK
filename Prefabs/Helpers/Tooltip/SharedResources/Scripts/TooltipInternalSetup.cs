namespace VRTK.Prefabs.Helpers.Tooltip
{
    using UnityEngine;
    using UnityEngine.UI;
    using Zinnia.Process;
    using Zinnia.Data.Attribute;

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

        public virtual void Process()
        {
            if (facade.FacingTarget != null)
            {
                facade.transform.LookAt(facade.FacingTarget.transform);
            }
            SetLine(facade.LineTarget, facade.LineWidth);
        }

        public virtual void SetDisplayText(string text, int size, Color color)
        {
            displayText.text = text;
            displayText.fontSize = size * 10;
            displayText.color = color;
        }

        public virtual void SetContainer(Vector2 size, Color color)
        {
            backgroundImage.GetComponent<RectTransform>().sizeDelta = size;
            backgroundImage.color = color;
        }

        public virtual void SetLine(GameObject target, float width)
        {
            if (target == null)
            {
                lineRenderer.gameObject.SetActive(false);
                return;
            }

            lineRenderer.gameObject.SetActive(true);
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
        }
    }
}