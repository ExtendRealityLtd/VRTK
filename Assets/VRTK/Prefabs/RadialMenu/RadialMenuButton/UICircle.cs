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
                UIVertex vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }
            return vbo;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            float outer = -rectTransform.pivot.x * rectTransform.rect.width;
            float inner = -rectTransform.pivot.x * rectTransform.rect.width + thickness;
            vh.Clear();

            Vector2 prevX = Vector2.zero;
            Vector2 prevY = Vector2.zero;
            Vector2 uv0 = new Vector2(0, 0);
            Vector2 uv1 = new Vector2(0, 1);
            Vector2 uv2 = new Vector2(1, 1);
            Vector2 uv3 = new Vector2(1, 0);
            Vector2 pos0;
            Vector2 pos1;
            Vector2 pos2;
            Vector2 pos3;
            float f = (fillPercent / 100f);
            float degrees = 360f / segments;
            int fa = (int)((segments + 1) * f);
            for (int i = -1 - (fa / 2); i < fa / 2 + 1; i++)
            {
                float rad = Mathf.Deg2Rad * (i * degrees);
                float c = Mathf.Cos(rad);
                float s = Mathf.Sin(rad);
                uv0 = new Vector2(0, 1);
                uv1 = new Vector2(1, 1);
                uv2 = new Vector2(1, 0);
                uv3 = new Vector2(0, 0);
                pos0 = prevX;
                pos1 = new Vector2(outer * c, outer * s);
                if (fill)
                {
                    pos2 = Vector2.zero;
                    pos3 = Vector2.zero;
                }
                else
                {
                    pos2 = new Vector2(inner * c, inner * s);
                    pos3 = prevY;
                }
                prevX = pos1;
                prevY = pos2;
                vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
            }
        }
    }
}