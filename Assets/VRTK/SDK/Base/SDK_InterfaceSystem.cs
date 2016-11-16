namespace VRTK
{
    using UnityEngine;

    public abstract class SDK_InterfaceSystem : ScriptableObject
    {
        public abstract bool IsDisplayOnDesktop();
        public abstract bool ShouldAppRenderWithLowResources();
        public abstract void ForceInterleavedReprojectionOn(bool force);
    }
}