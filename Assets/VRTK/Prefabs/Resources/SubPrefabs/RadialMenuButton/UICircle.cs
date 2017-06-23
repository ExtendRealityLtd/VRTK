namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    [ExecuteInEditMode]
    public class UICircle : Graphic
    {
        [Range(0, 100)]
        public int fillPercent;
        public bool fill = true;
        public int thickness = 5;
        [Range(0, 360)]
        public int segments = 360;

        [SerializeField]
        protected Texture setTexture;

        public override Texture mainTexture
        {
            get
            {
                return (setTexture == null ? s_WhiteTexture : setTexture);
            }
        }

        // Texture to be used.
        public Texture texture
        {
            get
            {
                return setTexture;
            }
            set
            {
                if (setTexture == value)
                {
                    return;
                }
                setTexture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        protected virtual void Update()
        {
            thickness = (int)Mathf.Clamp(thickness, 0, rectTransform.rect.width / 2);
        }

        protected virtual UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] vbo = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }
            return vbo;
        }
    }
}