// WindowsMR System|SDK_WindowsMR|002
namespace VRTK
{
	/// <summary>
    /// The Windows Mixed Reality System SDK script provides a bridge to the Windows Mixed Reality Unity SDK.
    /// </summary>
    [SDK_Description("WindowsMR", SDK_WindowsMRDefines.ScriptingDefineSymbol, "WindowsMR", "WSA")]
    public class SDK_WindowsMR
#if VRTK_DEFINE_SDK_WINDOWSMR && UNITY_2017_2_OR_NEWER
        : SDK_BaseSystem
#else
        : SDK_FallbackSystem
#endif
    {
#if VRTK_DEFINE_SDK_WINDOWSMR && UNITY_2017_2_OR_NEWER
        /// <summary>
        /// The ForceInterleavedReprojectionOn method determines whether Interleaved Reprojection should be forced on or off.
        /// </summary>
        /// <param name="force">If true then Interleaved Reprojection will be forced on, if false it will not be forced on.</param>
        public override void ForceInterleavedReprojectionOn(bool force)
        {
            // TODO: Compare with Oculus and SteamVR
        }

        /// <summary>
        /// The IsDisplayOnDesktop method returns true if the display is extending the desktop.
        /// </summary>
        /// <returns>Returns true if the display is extending the desktop</returns>
        public override bool IsDisplayOnDesktop()
        {
            // TODO: Compare with Oculus and SteamVR
            return false;
        }

        /// <summary>
        /// The ShouldAppRenderWithLowResources method is used to determine if the Unity app should use low resource mode. Typically true when the dashboard is showing.
        /// </summary>
        /// <returns>Returns true if the Unity app should render with low resources.</returns>
        public override bool ShouldAppRenderWithLowResources()
        {
            // TODO: Compare with Oculus and SteamVR
            return false;
        }
#endif
    }
}
