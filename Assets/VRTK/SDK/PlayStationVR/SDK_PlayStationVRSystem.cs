// Simulator System|SDK_Simulator|001

namespace VRTK
{
    /// <summary>
    ///     The PlayStationVR System SDK script provides system functions.
    /// </summary>
    [SDK_Description("PlayStationVR", null)]
    public class SDK_PlayStationVRSystem : SDK_BaseSystem
    {
        /// <summary>
        ///     The IsDisplayOnDesktop method returns true if the display is extending the desktop.
        /// </summary>
        /// <returns>Returns true if the display is extending the desktop</returns>
        public override bool IsDisplayOnDesktop()
        {
#if UNITY_PS4
           return SDK_PlayStationVRManager.instance.showHmdViewOnMonitor;
#endif
            return false;
        }

        /// <summary>
        ///     The ShouldAppRenderWithLowResources method is used to determine if the Unity app should use low resource mode.
        ///     Typically true when the dashboard is showing.
        /// </summary>
        /// <returns>Returns true if the Unity app should render with low resources.</returns>
        public override bool ShouldAppRenderWithLowResources()
        {
            //TODO
            return false;
        }

        /// <summary>
        ///     The ForceInterleavedReprojectionOn method determines whether Interleaved Reprojection should be forced on or off.
        /// </summary>
        /// <param name="force">If true then Interleaved Reprojection will be forced on, if false it will not be forced on.</param>
        public override void ForceInterleavedReprojectionOn(bool force)
        {
            //TODO
        }
        }
}