// Object Tooltip|Prefabs|0020
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// This adds a UI element into the World Space that can be used to provide additional information about an object by providing a piece of text with a line drawn to a destination point.
    /// </summary>
    /// <remarks>
    /// There are a number of parameters that can be set on the Prefab which are provided by the `VRTK/Scripts/VRTK_ObjectTooltip` script which is applied to the prefab.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/029_Controller_Tooltips` displays two cubes that have an object tooltip added to them along with tooltips that have been added to the controllers.
    /// </example>
    public class VRTK_ObjectTooltip : MonoBehaviour
    {
        [Tooltip("The text that is displayed on the tooltip.")]
        public string displayText;
        [Tooltip("The size of the text that is displayed.")]
        public int fontSize = 14;
        [Tooltip("The size of the tooltip container where `x = width` and `y = height`.")]
        public Vector2 containerSize = new Vector2(0.1f, 0.03f);
        [Tooltip("An optional transform of where to start drawing the line from. If one is not provided the centre of the tooltip is used for the initial line position.")]
        public Transform drawLineFrom;
        [Tooltip("A transform of another object in the scene that a line will be drawn from the tooltip to, this helps denote what the tooltip is in relation to. If no transform is provided and the tooltip is a child of another object, then the parent object's transform will be used as this destination position.")]
        public Transform drawLineTo;
        [Tooltip("The width of the line drawn between the tooltip and the destination transform.")]
        public float lineWidth = 0.001f;
        [Tooltip("The colour to use for the text on the tooltip.")]
        public Color fontColor = Color.black;
        [Tooltip("The colour to use for the background container of the tooltip.")]
        public Color containerColor = Color.black;
        [Tooltip("The colour to use for the line drawn between the tooltip and the destination transform.")]
        public Color lineColor = Color.black;

        private LineRenderer line;

        /// <summary>
        /// The Reset method resets the tooltip back to its initial state
        /// </summary>
        public void Reset()
        {
            SetContainer();
            SetText("UITextFront");
            SetText("UITextReverse");
            SetLine();
            if (drawLineTo == null && transform.parent != null)
            {
                drawLineTo = transform.parent;
            }
        }

        private void Start()
        {
            Reset();
        }

        private void SetContainer()
        {
            transform.FindChild("TooltipCanvas").GetComponent<RectTransform>().sizeDelta = containerSize;
            var tmpContainer = transform.FindChild("TooltipCanvas/UIContainer");
            tmpContainer.GetComponent<RectTransform>().sizeDelta = containerSize;
            tmpContainer.GetComponent<Image>().color = containerColor;
        }

        private void SetText(string name)
        {
            var tmpText = transform.FindChild("TooltipCanvas/" + name).GetComponent<Text>();
            tmpText.material = Resources.Load("UIText") as Material;
            tmpText.text = displayText.Replace("\\n", "\n");
            tmpText.color = fontColor;
            tmpText.fontSize = fontSize;
        }

        private void SetLine()
        {
            line = transform.FindChild("Line").GetComponent<LineRenderer>();
            line.material = Resources.Load("TooltipLine") as Material;
            line.material.color = lineColor;
            line.SetColors(lineColor, lineColor);
            line.SetWidth(lineWidth, lineWidth);
            if (drawLineFrom == null)
            {
                drawLineFrom = transform;
            }
        }

        private void DrawLine()
        {
            if (drawLineTo)
            {
                line.SetPosition(0, drawLineFrom.position);
                line.SetPosition(1, drawLineTo.position);
            }
        }

        private void Update()
        {
            DrawLine();
        }
    }
}