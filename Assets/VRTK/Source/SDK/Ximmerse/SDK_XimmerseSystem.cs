// Ximmerse System|SDK_Ximmerse|002
namespace VRTK
{
    /// <summary>
    /// The Ximmerse System SDK script provides a bridge to the Ximmerse SDK.
    /// </summary>
    [SDK_Description("Ximmerse (Standalone:Oculus)", SDK_XimmerseDefines.ScriptingDefineSymbol, "Oculus", "Standalone")]
    [SDK_Description("Ximmerse (Android:Daydream)", SDK_XimmerseDefines.ScriptingDefineSymbol, "daydream", "Android", 1)]
    public class SDK_XimmerseSystem
#if VRTK_DEFINE_SDK_XIMMERSE
        : SDK_BaseSystem
#else
        : SDK_FallbackSystem
#endif
    {
#if VRTK_DEFINE_SDK_XIMMERSE
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
            return true;
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