// HyperealVR System|SDK_HyperealVR|002
namespace VRTK
{
#if VRTK_DEFINE_SDK_HYPEREALVR
    using Hypereal;
#endif

    /// <summary>
    /// The HyperealVR System SDK script provides a bridge to the HyperealVR SDK.
    /// </summary>
    [SDK_Description("HyperealVR (Standalone)", SDK_HyperealVRDefines.ScriptingDefineSymbol, null, "Standalone")]
    public class SDK_HyperealVRSystem
#if VRTK_DEFINE_SDK_HYPEREALVR
        : SDK_BaseSystem
#else
        : SDK_FallbackSystem
#endif
    {
#if VRTK_DEFINE_SDK_HYPEREALVR
        /// <summary>
        /// The IsDisplayOnDesktop method returns true if the display is extending the desktop.
        /// </summary>
        /// <returns>Returns true if the display is extending the desktop</returns>
        public override bool IsDisplayOnDesktop()
        {
            return false;
        }

        /// <summary>
        /// The ShouldAppRenderWithLowResources method is used to determine if the Unity app should use low resource mode. Typically true when the dashboard is showing.
        /// </summary>
        /// <returns>Returns true if the Unity app should render with low resources.</returns>
        public override bool ShouldAppRenderWithLowResources()
        {
            return false;
        }

        /// <summary>
        /// The ForceInterleavedReprojectionOn method determines whether Interleaved Reprojection should be forced on or off.
        /// </summary>
        /// <param name="force">If true then Interleaved Reprojection will be forced on, if false it will not be forced on.</param>
        public override void ForceInterleavedReprojectionOn(bool force)
        {
        }
#endif
    }

}
