namespace VRTK.Examples
{
    using UnityEngine;
    using UnityEngine.UI;

    public abstract class VRTKExample_OptionTile : MonoBehaviour
    {
        public Image backgroundImage;
        public Color highlightColor = Color.yellow;

        public abstract void Activate();

        protected Color originalColor = Color.clear;

        public virtual void Highlight()
        {
            if (backgroundImage != null)
            {
                originalColor = backgroundImage.color;
                backgroundImage.color = highlightColor;
            }
        }

        public virtual void Unhighlight()
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = originalColor;
            }
        }
    }
}