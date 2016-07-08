namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;

    public class VRTK_ObjectTooltip : MonoBehaviour
    {
        public string displayText;
        public int fontSize = 14;
        public Vector2 containerSize = new Vector2(0.1f, 0.03f);
        public Transform drawLineFrom;
        public Transform drawLineTo;
        public float lineWidth = 0.001f;

        public Color fontColor = Color.black;
        public Color containerColor = Color.black;
        public Color lineColor = Color.black;

        private LineRenderer line;

        public void Reset()
        {
            SetContainer();
            SetText("UITextFront");
            SetText("UITextReverse");
            SetLine();
            if (drawLineTo == null && this.transform.parent != null)
            {
                drawLineTo = this.transform.parent;
            }
        }

        private void Start()
        {
            Reset();
        }

        private void SetContainer()
        {
            this.transform.FindChild("TooltipCanvas").GetComponent<RectTransform>().sizeDelta = containerSize;
            var tmpContainer = this.transform.FindChild("TooltipCanvas/UIContainer");
            tmpContainer.GetComponent<RectTransform>().sizeDelta = containerSize;
            tmpContainer.GetComponent<Image>().color = containerColor;
        }

        private void SetText(string name)
        {
            var tmpText = this.transform.FindChild("TooltipCanvas/" + name).GetComponent<Text>();
            tmpText.material = Resources.Load("UIText") as Material;
            tmpText.text = displayText;
            tmpText.color = fontColor;
            tmpText.fontSize = fontSize;
        }

        private void SetLine()
        {
            line = this.transform.FindChild("Line").GetComponent<LineRenderer>();
            line.material = Resources.Load("TooltipLine") as Material;
            line.material.color = lineColor;
            line.SetColors(lineColor, lineColor);
            line.SetWidth(lineWidth, lineWidth);
            if (drawLineFrom == null)
            {
                drawLineFrom = this.transform;
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