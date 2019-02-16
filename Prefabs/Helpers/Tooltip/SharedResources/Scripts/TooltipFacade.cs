namespace VRTK.Prefabs.Helpers.Tooltip
{
    using UnityEngine;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface for the TooltipFacade prefab.
    /// </summary>
    public class TooltipFacade : MonoBehaviour
    {
        #region Text Settings
        [Header("Text Settings"), Tooltip("The text to display in the tooltip."), SerializeField]
        private string displayText = "Tooltip Text";
        /// <summary>
        /// The text to display in the tooltip.
        /// </summary>
        public string DisplayText
        {
            get
            {
                return displayText;
            }
            set
            {
                displayText = value;
                internalSetup.SetDisplayText(displayText, TextSize, TextColor);
            }
        }

        [Tooltip("The size of the text for the tooltip."), SerializeField]
        private int textSize = 14;
        /// <summary>
        /// The size of the text for the tooltip.
        /// </summary>
        public int TextSize
        {
            get
            {
                return textSize;
            }
            set
            {
                textSize = value;
                internalSetup.SetDisplayText(DisplayText, textSize, TextColor);
            }
        }

        [Tooltip("The color of the text for the tooltip."), SerializeField]
        private Color textColor = Color.black;
        /// <summary>
        /// The color of the text for the tooltip.
        /// </summary>
        public Color TextColor
        {
            get
            {
                return textColor;
            }
            set
            {
                textColor = value;
                internalSetup.SetDisplayText(DisplayText, TextSize, textColor);
            }
        }
        #endregion

        #region Container Settings
        [Header("Container Settings"), Tooltip("The size of the background for the tooltip container."), SerializeField]
        private Vector2 containerSize = new Vector2(100f, 30f);
        /// <summary>
        /// The size of the background for the tooltip container.
        /// </summary>
        public Vector2 ContainerSize
        {
            get
            {
                return containerSize;
            }
            set
            {
                containerSize = value;
                internalSetup.SetContainer(containerSize, ContainerColor);
            }
        }

        [Tooltip("The color of the background for the tooltip container."), SerializeField]
        private Color containerColor = Color.white;
        /// <summary>
        /// The color of the background for the tooltip container.
        /// </summary>
        public Color ContainerColor
        {
            get
            {
                return containerColor;
            }
            set
            {
                containerColor = value;
                internalSetup.SetContainer(ContainerSize, containerColor);
            }
        }

        [Tooltip("The object that the tooltip will face towards."), SerializeField]
        private GameObject facingTarget;
        /// <summary>
        /// The object that the tooltip will face towards.
        /// </summary>
        public GameObject FacingTarget
        {
            get
            {
                return facingTarget;
            }
            set
            {
                facingTarget = value;
            }
        }
        #endregion

        #region Line Settings
        [Header("Line Settings"), Tooltip("The target to draw the tooltip line to."), SerializeField]
        private GameObject lineTarget;
        /// <summary>
        /// The target to draw the tooltip line to.
        /// </summary>
        public GameObject LineTarget
        {
            get
            {
                return lineTarget;
            }
            set
            {
                lineTarget = value;
                internalSetup.SetLine(lineTarget, LineWidth);
            }
        }

        [Tooltip("The width of the line for the tooltip."), SerializeField]
        private float lineWidth = 0.001f;
        /// <summary>
        /// The width of the line for the tooltip.
        /// </summary>
        public float LineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                lineWidth = value;
                internalSetup.SetLine(LineTarget, lineWidth);
            }
        }
        #endregion

        #region Internal Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Header("Internal Settings"), Tooltip("The linked Internal Setup."), InternalSetting, SerializeField]
        protected TooltipInternalSetup internalSetup;
        #endregion

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            internalSetup.SetDisplayText(DisplayText, TextSize, TextColor);
            internalSetup.SetContainer(ContainerSize, ContainerColor);
            internalSetup.SetLine(LineTarget, LineWidth);
        }
    }
}