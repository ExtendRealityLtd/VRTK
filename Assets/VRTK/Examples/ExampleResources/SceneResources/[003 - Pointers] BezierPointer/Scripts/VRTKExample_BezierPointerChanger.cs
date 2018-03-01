namespace VRTK.Examples
{
    using UnityEngine;

    public abstract class VRTKExample_BezierPointerChanger : VRTKExample_OptionTile
    {
        public VRTK_BezierPointerRenderer leftRenderer;
        public VRTK_BezierPointerRenderer rightRenderer;

        public override void Activate()
        {
            StyleRenderer(leftRenderer);
            StyleRenderer(rightRenderer);
        }

        protected abstract void StyleRenderer(VRTK_BezierPointerRenderer renderer);

        protected virtual void ResetRenderer(VRTK_BezierPointerRenderer renderer)
        {
            if (renderer != null)
            {
                renderer.gameObject.SetActive(false);
                renderer.validCollisionColor = Color.green;
                renderer.invalidCollisionColor = Color.red;
                renderer.customTracer = null;
                renderer.customCursor = null;
                renderer.validLocationObject = null;
                renderer.invalidLocationObject = null;
                renderer.rescaleTracer = false;
                renderer.gameObject.SetActive(true);
            }
        }
    }
}