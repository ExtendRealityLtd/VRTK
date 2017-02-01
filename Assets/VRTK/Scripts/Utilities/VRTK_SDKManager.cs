// SDK Manager|Utilities|90010
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// The SDK Manager script provides configuration of supported SDKs
    /// </summary>
    [ExecuteInEditMode]
    public class VRTK_SDKManager : MonoBehaviour
    {
        /// <summary>
        /// The supported SDKs
        /// </summary>
        public enum SupportedSDKs
        {
            None,
            SteamVR,
            OculusVR,
            Daydream,
            Simulator
        }

        public Dictionary<SupportedSDKs, VRTK_SDKDetails> sdkDetails = new Dictionary<SupportedSDKs, VRTK_SDKDetails>()
        {
            { SupportedSDKs.None, new VRTK_SDKDetails("", "", "") },
            { SupportedSDKs.SteamVR, new VRTK_SDKDetails("VRTK_SDK_STEAMVR", "SteamVR", "SteamVR") },
            { SupportedSDKs.OculusVR, new VRTK_SDKDetails("VRTK_SDK_OCULUSVR", "OculusVR", "OVRInput") },
            { SupportedSDKs.Daydream, new VRTK_SDKDetails("VRTK_SDK_DAYDREAM", "Daydream", "VRTK_SDKManager") }, // JSL: maybe use GVR here?
            { SupportedSDKs.Simulator, new VRTK_SDKDetails("VRTK_SDK_SIM", "Simulator", "VRTK_SDKManager") }
        };

        /// <summary>
        /// The singleton instance to access the SDK Manager variables from.
        /// </summary>
        public static VRTK_SDKManager instance = null;

        [Header("SDK Selection")]

        [Tooltip("If this is true then the instance of the SDK Manager won't be destroyed on every scene load.")]
        public bool persistOnLoad = false;

        [Tooltip("The SDK to use to deal with all system actions.")]
        public SupportedSDKs systemSDK = SupportedSDKs.None;
        [Tooltip("The SDK to use to utilise room scale boundaries.")]
        public SupportedSDKs boundariesSDK = SupportedSDKs.None;
        [Tooltip("The SDK to use to utilise the VR headset.")]
        public SupportedSDKs headsetSDK = SupportedSDKs.None;
        [Tooltip("The SDK to use to utilise the input devices.")]
        public SupportedSDKs controllerSDK = SupportedSDKs.None;

        [Tooltip("This determines whether the scripting define symbols required by the selected SDKs are automatically added to the player settings when using the SDK Manager inspector window.")]
        public bool autoManageScriptDefines = true;

        [Header("Linked Objects")]

        [Tooltip("A reference to the GameObject that is the user's boundary or play area, most likely provided by the SDK's Camera Rig.")]
        public GameObject actualBoundaries;
        [Tooltip("A reference to the GameObject that contains the VR camera, most likely provided by the SDK's Camera Rig Headset.")]
        public GameObject actualHeadset;
        [Tooltip("A reference to the GameObject that contains the SDK Left Hand Controller.")]
        public GameObject actualLeftController;
        [Tooltip("A reference to the GameObject that contains the SDK Right Hand Controller.")]
        public GameObject actualRightController;

        [Header("Controller Aliases")]

        [Tooltip("A reference to the GameObject that models for the Left Hand Controller.")]
        public GameObject modelAliasLeftController;
        [Tooltip("A reference to the GameObject that models for the Right Hand Controller")]
        public GameObject modelAliasRightController;
        [Tooltip("A reference to the GameObject that contains any scripts that apply to the Left Hand Controller.")]
        public GameObject scriptAliasLeftController;
        [Tooltip("A reference to the GameObject that contains any scripts that apply to the Right Hand Controller.")]
        public GameObject scriptAliasRightController;

        /// <summary>
        /// The GetSystemSDK method returns the selected system SDK
        /// </summary>
        /// <returns>The currently selected System SDK</returns>
        public SDK_BaseSystem GetSystemSDK()
        {
            SDK_BaseSystem returnSDK = null;
            switch (systemSDK)
            {
                case SupportedSDKs.SteamVR:
                    returnSDK = ScriptableObject.CreateInstance<SDK_SteamVRSystem>();
                    break;
                case SupportedSDKs.OculusVR:
                    returnSDK = ScriptableObject.CreateInstance<SDK_OculusVRSystem>();
                    break;
                case SupportedSDKs.Daydream:
                    returnSDK = ScriptableObject.CreateInstance<SDK_DaydreamSystem>();
                    break;
                case SupportedSDKs.Simulator:
                    returnSDK = ScriptableObject.CreateInstance<SDK_SimSystem>();
                    break;
                default:
                    Debug.LogError("No valid System SDK has been selected. If you're seeing this error in Unity Edit mode, then try selecting the GameObject with the `VRTK_SDKManager` script and selecting a valid System SDK from the dropdown list.");
                    returnSDK = ScriptableObject.CreateInstance<SDK_FallbackSystem>();
                    break;
            }
            return returnSDK;
        }

        /// <summary>
        /// The GetHeadsetSDK method returns the selected headset SDK
        /// </summary>
        /// <returns>The currently selected Headset SDK</returns>
        public SDK_BaseHeadset GetHeadsetSDK()
        {
            SDK_BaseHeadset returnSDK = null;
            switch (headsetSDK)
            {
                case SupportedSDKs.SteamVR:
                    returnSDK = ScriptableObject.CreateInstance<SDK_SteamVRHeadset>();
                    break;
                case SupportedSDKs.OculusVR:
                    returnSDK = ScriptableObject.CreateInstance<SDK_OculusVRHeadset>();
                    break;
                case SupportedSDKs.Daydream:
                    returnSDK = ScriptableObject.CreateInstance<SDK_DaydreamHeadset>();
                    break;
                case SupportedSDKs.Simulator:
                    returnSDK = ScriptableObject.CreateInstance<SDK_SimHeadset>();
                    break;
                default:
                    Debug.LogError("No valid Headset SDK has been selected. If you're seeing this error in Unity Edit mode, then try selecting the GameObject with the `VRTK_SDKManager` script and selecting a valid Headset SDK from the dropdown list.");
                    returnSDK = ScriptableObject.CreateInstance<SDK_FallbackHeadset>();
                    break;
            }
            return returnSDK;
        }

        /// <summary>
        /// The GetControllerSDK method returns the selected controller SDK
        /// </summary>
        /// <returns>The currently selected Controller SDK</returns>
        public SDK_BaseController GetControllerSDK()
        {
            SDK_BaseController returnSDK = null;
            switch (controllerSDK)
            {
                case SupportedSDKs.SteamVR:
                    returnSDK = ScriptableObject.CreateInstance<SDK_SteamVRController>();
                    break;
                case SupportedSDKs.OculusVR:
                    returnSDK = ScriptableObject.CreateInstance<SDK_OculusVRController>();
                    break;
                case SupportedSDKs.Daydream:
                    returnSDK = ScriptableObject.CreateInstance<SDK_DaydreamController>();
                    break;
                case SupportedSDKs.Simulator:
                    returnSDK = ScriptableObject.CreateInstance<SDK_SimController>();
                    break;
                default:
                    Debug.LogError("No valid Controller SDK has been selected. If you're seeing this error in Unity Edit mode, then try selecting the GameObject with the `VRTK_SDKManager` script and selecting a valid Controller SDK from the dropdown list.");
                    returnSDK = ScriptableObject.CreateInstance<SDK_FallbackController>();
                    break;
            }
            return returnSDK;
        }

        /// <summary>
        /// The GetBoundariesSDK method returns the selected boundaries SDK
        /// </summary>
        /// <returns>The currently selected Boundaries SDK</returns>
        public SDK_BaseBoundaries GetBoundariesSDK()
        {
            SDK_BaseBoundaries returnSDK = null;
            switch (boundariesSDK)
            {
                case SupportedSDKs.SteamVR:
                    returnSDK = ScriptableObject.CreateInstance<SDK_SteamVRBoundaries>();
                    break;
                case SupportedSDKs.OculusVR:
                    returnSDK = ScriptableObject.CreateInstance<SDK_OculusVRBoundaries>();
                    break;
                case SupportedSDKs.Daydream:
                    returnSDK = ScriptableObject.CreateInstance<SDK_DaydreamBoundaries>();
                    break;
                case SupportedSDKs.Simulator:
                    returnSDK = ScriptableObject.CreateInstance<SDK_SimBoundaries>();
                    break;
                default:
                    Debug.LogError("No valid Boundaries SDK has been selected. If you're seeing this error in Unity Edit mode, then try selecting the GameObject with the `VRTK_SDKManager` script and selecting a valid Boundaries SDK from the dropdown list.");
                    returnSDK = ScriptableObject.CreateInstance<SDK_FallbackBoundaries>();
                    break;
            }
            return returnSDK;
        }

        protected virtual void Awake()
        {
            CreateInstance();
            if (!VRTK_SharedMethods.IsEditTime())
            {
                SetupHeadset();
                SetupControllers();
                GetBoundariesSDK().InitBoundaries();
            }
        }

        private void SetupHeadset()
        {
            if (!actualHeadset.GetComponent<VRTK_TrackedHeadset>())
            {
                actualHeadset.AddComponent<VRTK_TrackedHeadset>();
            }
        }

        private void SetupControllers()
        {
            if (actualLeftController && 
                !actualLeftController.GetComponent<VRTK_TrackedController>())
            {
                actualLeftController.AddComponent<VRTK_TrackedController>();
            }

            if (!actualRightController.GetComponent<VRTK_TrackedController>())
            {
                actualRightController.AddComponent<VRTK_TrackedController>();
            }

            if (scriptAliasLeftController && !scriptAliasLeftController.GetComponent<VRTK_ControllerTracker>())
            {
                scriptAliasLeftController.AddComponent<VRTK_ControllerTracker>();
            }

            if (scriptAliasRightController && !scriptAliasRightController.GetComponent<VRTK_ControllerTracker>())
            {
                scriptAliasRightController.AddComponent<VRTK_ControllerTracker>();
            }
        }

        private void CreateInstance()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            if (persistOnLoad && !VRTK_SharedMethods.IsEditTime())
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    public class VRTK_SDKDetails
    {
        public string defineSymbol;
        public string prettyName;
        public string checkType;

        public VRTK_SDKDetails(string givenDefineSymbol, string givenPrettyName, string givenCheckType)
        {
            defineSymbol = givenDefineSymbol;
            prettyName = givenPrettyName;
            checkType = givenCheckType;
        }
    }
}