namespace VRTK
{
    public interface SDK_InterfaceSystem
    {
        bool IsDisplayOnDesktop();
        bool ShouldAppRenderWithLowResources();
        void ForceInterleavedReprojectionOn(bool force);
    }
}