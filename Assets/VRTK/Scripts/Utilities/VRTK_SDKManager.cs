// SDK Manager|Utilities|90010
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The SDK Manager script enables easy configuration of supported SDKs
    /// </summary>
    public class VRTK_SDKManager : MonoBehaviour
    {
        /// <summary>
        /// The supported SDKs
        /// </summary>
        public enum SupportedSDKs
        {
            None,
            SteamVR
        }

        [Tooltip("The SDK to use to deal with all system actions.")]
        public SupportedSDKs systemSDK = SupportedSDKs.None;
        [Tooltip("The SDK to use to utilise the VR headset.")]
        public SupportedSDKs headsetSDK = SupportedSDKs.None;
        [Tooltip("The SDK to use to utilise the input devices.")]
        public SupportedSDKs controllerSDK = SupportedSDKs.None;
        [Tooltip("The SDK to use to utilise room scale boundaries.")]
        public SupportedSDKs boundariesSDK = SupportedSDKs.None;
    }
}