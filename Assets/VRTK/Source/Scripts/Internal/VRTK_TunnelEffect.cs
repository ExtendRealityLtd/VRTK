namespace VRTK
{
    using UnityEngine;

    public class VRTK_TunnelEffect : MonoBehaviour
    {
        protected Material material;

        public virtual void SetMaterial(Material material)
        {
            this.material = material;
        }

        protected virtual void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest, material);
        }
    }
}