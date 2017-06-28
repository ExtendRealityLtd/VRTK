namespace VRTK
{
    using UnityEngine;

    public class VRTK_ScreenFade : MonoBehaviour
    {
        public static VRTK_ScreenFade instance;

        protected Material fadeMaterial = null;
        protected Color currentColor = new Color(0f, 0f, 0f, 0f);
        protected Color targetColor = new Color(0f, 0f, 0f, 0f);
        protected Color deltaColor = new Color(0f, 0f, 0f, 0f);

        public static void Start(Color newColor, float duration)
        {
            if (instance)
            {
                instance.StartFade(newColor, duration);
            }
        }

        public virtual void StartFade(Color newColor, float duration)
        {
            if (duration > 0.0f)
            {
                targetColor = newColor;
                deltaColor = (targetColor - currentColor) / duration;
            }
            else
            {
                currentColor = newColor;
            }
        }

        protected virtual void Awake()
        {
            fadeMaterial = new Material(Shader.Find("Unlit/TransparentColor"));
            instance = this;
        }

        protected virtual void OnPostRender()
        {
            if (currentColor != targetColor)
            {
                if (Mathf.Abs(currentColor.a - targetColor.a) < Mathf.Abs(deltaColor.a) * Time.deltaTime)
                {
                    currentColor = targetColor;
                    deltaColor = new Color(0, 0, 0, 0);
                }
                else
                {
                    currentColor += deltaColor * Time.deltaTime;
                }
            }

            if (currentColor.a > 0 && fadeMaterial)
            {
                currentColor.a = (targetColor.a > currentColor.a && currentColor.a > 0.98f ? 1f : currentColor.a);
                fadeMaterial.color = currentColor;
                fadeMaterial.SetPass(0);
                GL.PushMatrix();
                GL.LoadOrtho();
                GL.Color(fadeMaterial.color);
                GL.Begin(GL.QUADS);
                GL.Vertex3(0f, 0f, 0.9999f);
                GL.Vertex3(0f, 1f, 0.9999f);
                GL.Vertex3(1f, 1f, 0.9999f);
                GL.Vertex3(1f, 0f, 0.9999f);
                GL.End();
                GL.PopMatrix();
            }
        }
    }
}