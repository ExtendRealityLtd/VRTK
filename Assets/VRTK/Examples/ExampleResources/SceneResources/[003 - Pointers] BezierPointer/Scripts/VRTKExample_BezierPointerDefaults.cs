namespace VRTK.Examples
{
    using UnityEngine;

    public class VRTKExample_BezierPointerDefaults : VRTKExample_BezierPointerChanger
    {
        protected override void StyleRenderer(VRTK_BezierPointerRenderer renderer)
        {
            ResetRenderer(renderer);
        }
    }
}