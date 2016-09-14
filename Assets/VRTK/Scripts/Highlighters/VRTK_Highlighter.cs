namespace VRTK.Highlighters
{
    using UnityEngine;

    public abstract class VRTK_Highlighter : MonoBehaviour
    {
        public abstract void Initialise(Color? color = null);
        public abstract void Highlight(Color? color = null, float duration = 0f);
        public abstract void Unhighlight(Color? color = null, float duration = 0f);
    }
}