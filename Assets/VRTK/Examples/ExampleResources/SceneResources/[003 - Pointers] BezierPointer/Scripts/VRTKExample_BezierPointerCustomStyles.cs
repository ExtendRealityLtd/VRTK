namespace VRTK.Examples
{
    using UnityEngine;

    public class VRTKExample_BezierPointerCustomStyles : VRTKExample_BezierPointerChanger
    {
        public Color validLocationColor = new Color(0.431f, 0.682f, 0.788f);
        public Color invalidLocationColor = new Color(0.545f, 0.149f, 0.208f);
        public GameObject tracerPrefab;
        public GameObject cursorPrefab;
        public GameObject validLocationPrefab;
        public GameObject invalidLocationPrefab;

        protected override void StyleRenderer(VRTK_BezierPointerRenderer renderer)
        {
            if (renderer != null)
            {
                ResetRenderer(renderer);
                renderer.gameObject.SetActive(false);
                renderer.rescaleTracer = true;
                renderer.validCollisionColor = validLocationColor;
                renderer.invalidCollisionColor = invalidLocationColor;
                renderer.customTracer = tracerPrefab;
                renderer.customCursor = cursorPrefab;
                renderer.validLocationObject = validLocationPrefab;
                renderer.invalidLocationObject = invalidLocationPrefab;
                renderer.gameObject.SetActive(true);
            }
        }
    }
}