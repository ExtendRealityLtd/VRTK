﻿// Daydream System|SDK_Daydream|002
namespace VRTK
{
    /// <summary>
    /// The Daydream System SDK script provides dummy functions for system functions.
    /// </summary>
    [SDK_Description("Google Daydream (Android:Daydream)", SDK_DaydreamDefines.ScriptingDefineSymbol, "Daydream", "Android")]
    public class SDK_DaydreamSystem
#if VRTK_DEFINE_SDK_DAYDREAM
        : SDK_BaseSystem
#else
        : SDK_FallbackSystem
#endif
    {
#if VRTK_DEFINE_SDK_DAYDREAM
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