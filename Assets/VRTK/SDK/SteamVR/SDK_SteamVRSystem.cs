// SteamVR System|SDK_SteamVR|001
namespace VRTK
{
#if VRTK_SDK_STEAMVR
    using Valve.VR;

    /// <summary>
    /// The SteamVR System SDK script provides a bridge to the SteamVR SDK.
    /// </summary>
    public class SDK_SteamVRSystem : SDK_BaseSystem
    {
        /// <summary>
        /// The IsDisplayOnDesktop method returns true if the display is extending the desktop.
        /// </summary>
        /// <returns>Returns true if the display is extending the desktop</returns>
        public override bool IsDisplayOnDesktop()
        {
            return (OpenVR.System == null || OpenVR.System.IsDisplayOnDesktop());
        }

        /// <summary>
        /// The ShouldAppRenderWithLowResources method is used to determine if the Unity app should use low resource mode. Typically true when the dashboard is showing.
        /// </summary>
        /// <returns>Returns true if the Unity app should render with low resources.</returns>
        public override bool ShouldAppRenderWithLowResources()
        {
            return (OpenVR.Compositor != null && OpenVR.Compositor.ShouldAppRenderWithLowResources());
        }

        /// <summary>
        /// The ForceInterleavedReprojectionOn method determines whether Interleaved Reprojection should be forced on or off.
        /// </summary>
        /// <param name="force">If true then Interleaved Reprojection will be forced on, if false it will not be forced on.</param>
        public override void ForceInterleavedReprojectionOn(bool force)
        {
            if (OpenVR.Compositor != null)
            {
                OpenVR.Compositor.ForceInterleavedReprojectionOn(force);
            }
        }
    }
#else
    public class SDK_SteamVRSystem : SDK_FallbackSystem
    {
    }
#endif
}